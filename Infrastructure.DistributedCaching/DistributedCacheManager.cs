using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Infrastructure.Core.Caching;
using Infrastructure.Core.Configuration;
using Infrastructure.Core.Infrastructure;
using Infrastructure.ServiceModel.ServiceLibrary;
using Infrastructure.Services.SettingService;
using Microsoft.ApplicationServer.Caching;
using System.Configuration;
using System.Threading;

namespace Infrastructure.DistributedCaching
{
    public class DistributedCacheManager : ServiceBase, ICacheManager
    {
        private readonly IApplicationSettingService _applicationSettingService;
        private readonly ConcurrentDictionary<string, CacheClient> _cacheClients;
        public ConcurrentDictionary<string, CacheClient> CacheClients { get { return _cacheClients; } }
        public event EventHandler<ObjectPutEventArgs> ObjectPut;
        private DataCacheFactory _factory;
        private DataCache _cache;
        private const string AppFabricCacheName = "EBakanlikCacheV2";
        public const string DistributedCacheHostsKey = "DistributedCacheHostsKey";
        private volatile object _cacheClientLock = new object();

        public DistributedCacheManager()
        {
            EventLog.WriteEntry("Application", "AppFabric Constructor", EventLogEntryType.Information, 9111);
            DataCacheClientLogManager.ChangeLogLevel(TraceLevel.Off);
            _applicationSettingService = EngineContext.Current.Resolve<IApplicationSettingService>();
            InitializeFactory();
            _cacheClients = new ConcurrentDictionary<string, CacheClient>();
        }
        public T Get<T>(string key) where T : class
        {
            return (T)_cache.Get(key);
        }

        public object Get(string key)
        {
            return Get<object>(key);
        }

        public List<T> Filter<T>(string key, string fieldName, string fieldValue)
        {
            throw new NotImplementedException();
        }

        public T Get<T>(string key, Func<T> defaultValueExpression)
        {
            var result = _cache.Get(key);
            if (result == null)
                return defaultValueExpression.Invoke();
            return (T)result;
        }

        public void Set(string key, object data)
        {
            DataCacheLockHandle handle = null;
            handle = RetryHelper.Execute(
                () =>
                {
                    DataCacheLockHandle innerHandle = null;
                    var output = _cache.GetAndLock(key, TimeSpan.FromSeconds(240), out innerHandle);
                    return innerHandle;
                }, (ex) => true, 50, false, 50);
            DataCacheItemVersion version = null;
            if (handle == null)
                version = _cache.Put(key, data);
            else
                version = _cache.PutAndUnlock(key, data, handle);
            OnObjectPut(new ObjectPutEventArgs() { Key = key, Version = version });
        }

        public void Set(string key, object data, TimeSpan cacheTime)
        {
            DataCacheLockHandle handle = null;
            handle = RetryHelper.Execute(
                () =>
                {
                    DataCacheLockHandle innerHandle = null;
                    var output = _cache.GetAndLock(key, TimeSpan.FromSeconds(240), out innerHandle);
                    return innerHandle;
                }, (ex) => true, 50, false, 50);
            DataCacheItemVersion version = null;
            if (handle == null)
                version = _cache.Put(key, data, cacheTime);
            else
                version = _cache.PutAndUnlock(key, data, handle, cacheTime);
            OnObjectPut(new ObjectPutEventArgs() { Key = key, Version = version });
        }

        public bool IsSet(string key)
        {
            return (_cache[key] != null);
        }

        public void Remove(string key)
        {
            try
            {
                DataCacheLockHandle handle = null;
                handle = RetryHelper.Execute(
                    () =>
                    {
                        DataCacheLockHandle innerHandle = null;
                        var output = _cache.GetAndLock(key, TimeSpan.FromSeconds(30), out innerHandle);
                        return innerHandle;
                    }, (ex) => true, 50, false, 50);
                if (handle != null)
                {
                    _cache.Remove(key, handle);
                    //_cache.Unlock(key, handle);
                }
            }
            catch (DataCacheException ex)
            {
                if (ex.ErrorCode != DataCacheErrorCode.KeyDoesNotExist)
                {
                    EventLog.WriteEntry("Application", ex.ToString(), EventLogEntryType.Error);
                }
            }
        }

        public void RemoveByPattern(string pattern)
        {
            throw new NotImplementedException();
        }

        public object GetIfNewer(string key, object version)
        {
            var vers = (DataCacheItemVersion)version;
            return _cache.GetIfNewer(key, ref vers);
        }

        public void Clear()
        {
            _cache.ClearRegion(AppFabricCacheName);
        }

        private void InitializeFactory()
        {
            var configuration = new DataCacheFactoryConfiguration("default");
            var hostsSetting = _applicationSettingService.GetSetting(DistributedCacheHostsKey);
            if (string.IsNullOrEmpty(hostsSetting))
                throw new DataCacheException("AppFabric Cache Manager : The hosts are not defined!");
            var hosts = hostsSetting.TrimEnd(';').Split(';').Where(s => !string.IsNullOrEmpty(s)).ToArray();
            if (!hosts.Any())
                throw new DataCacheException("AppFabric Cache Manager : The hosts are not defined!");
            var servers = hosts.Select(host => new DataCacheServerEndpoint(host.Trim(), 22233)).ToList();

            configuration.Servers = servers;
            
            _factory = new DataCacheFactory(configuration);
            var coreConfig = Engine.Resolve<CoreConfiguration>();
            _cache = _factory.GetCache(coreConfig.DistributedCacheName ?? AppFabricCacheName);
        }
        protected virtual void OnObjectPut(ObjectPutEventArgs e)
        {

            EventHandler<ObjectPutEventArgs> handler = ObjectPut;
            if (handler != null)
            {
                handler(this, e);
            }
        }
    }
    /// <summary>
    /// Helper class to retry the functionality
    /// </summary>
    public static class RetryHelper
    {
        /// <summary>
        /// Executes the action with the retry logic
        /// </summary>
        /// <param name="retryAction">Action to be executed</param>
        /// <param name="canContinue">Checks if the exception can informational log or exception handling log </param>
        /// <param name="buildLogEntry">Function delegate to return the log entry</param>
        /// <param name="maxRetryCount">Maximum number of the retry</param>
        /// <param name="canThrowException">True or false whether it can throw exception or not.</param>
        /// <param name="delay">Delay in retry</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "It is required to catch the generic exception here")]
        public static void Execute(Action retryAction, Predicate<Exception> canContinue, int maxRetryCount = 3, bool canThrowException = true, int delay = 200)
        {
            int retryCount = 0;

            //Iteration for the retry starts
            while (retryCount < maxRetryCount)
            {
                try
                {
                    // Invokes the delegate action
                    retryAction();
                    return;
                }

                catch (Exception ex)
                {
                    retryCount += 1;

                    var canRetryContinue = canContinue(ex);

                    //Logs the exception 
                    //LogExceptions(canRetryContinue, ex, retryCount, buildLogEntry, maxRetryCount, canThrowException);

                    //Get out of the loop if cannot be continued.
                    if (!canRetryContinue)
                    {
                        return;
                    }
                }

                //Adding the delay to the retry.
                System.Threading.Tasks.Task.Delay(delay);
            }
        }

        /// <summary>
        /// Executes the retry function
        /// </summary>
        /// <typeparam name="TResult">Type that is returned</typeparam>
        /// <param name="retryFunction">Retry function</param>
        /// <param name="canContinue">Condition to check whether the exception are logged as is or informational</param>
        /// <param name="buildLogEntry">Builds the log entry</param>
        /// <param name="maxRetryCount">Maximum retry count</param>
        /// <param name="canThrowException">True or False Whether it can throw the exception or not</param>
        /// <param name="delay">Delay interval to retry</param>
        /// <returns>Type that the retry function returns</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "It is required to catch the generic exception here")]
        public static TResult Execute<TResult>(Func<TResult> retryFunction, Predicate<Exception> canContinue, int maxRetryCount = 3, bool canThrowException = true, int delay = 200)
        {
            int retryCount = 0;
            TResult returnType = default(TResult);

            //Iteration for retry starts
            while (retryCount < maxRetryCount)
            {
                try
                {
                    // Invokes the delegate function
                    returnType = retryFunction();
                    return returnType; // returns the type
                }

                catch (Exception ex)
                {
                    retryCount += 1;

                    var canRetryContinue = canContinue(ex);

                    //Logs the exception
                    //LogExceptions(canRetryContinue, ex, retryCount, buildLogEntry, maxRetryCount, canThrowException);

                    //Get out of the loop if cannot be continued.
                    if (!canRetryContinue)
                    {
                        return returnType;
                    }
                }

                //Adding the delays
                System.Threading.Tasks.Task.Delay(delay);
            }

            return returnType;
        }
    }
}
