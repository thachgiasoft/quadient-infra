using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.Core.Caching;
using Infrastructure.Core.Configuration;
using Infrastructure.Core.Helpers;
using Infrastructure.Core.Infrastructure;
using Infrastructure.ServiceModel.ServiceLibrary;
using Infrastructure.Services.SettingService;
using Tangosol.IO.Pof;
using Tangosol.IO.Resources;
using Tangosol.Net;
using Tangosol.Util;
using Tangosol.Util.Extractor;
using Tangosol.Util.Filter;

namespace Infrastructure.DistributedCaching
{
    public class CoherenceDistributedCacheManager : ServiceBase, ICacheManager
    {
        private readonly ConcurrentDictionary<string, CacheClient> _cacheClients;
        public ConcurrentDictionary<string, CacheClient> CacheClients { get { return _cacheClients; } }
        public event EventHandler<ObjectPutEventArgs> ObjectPut;
        private readonly IApplicationSettingService _applicationSettingService;

        private static string CacheFactoryName;

        private static string CoherenceConfigPath;
        //Parameterless constructor
        public CoherenceDistributedCacheManager()
        {
            _applicationSettingService = EngineContext.Current.Resolve<IApplicationSettingService>();
            EventLog.WriteEntry("Application", "Coherence Constructor", EventLogEntryType.Information, 9888);
            InitializeFactory();
        }

        public CoherenceDistributedCacheManager(IApplicationSettingService applicationSettingService)
        {
            _applicationSettingService = applicationSettingService;
            EventLog.WriteEntry("Application", "Coherence Constructor", EventLogEntryType.Information, 9888);
            InitializeFactory();
        }

        public T Get<T>(string key) where T : class
        {
            var cache = CacheFactory.GetCache(CacheFactoryName);
            try
            {
                var ttt = (T)GenericTypeHelper.Convert<object, T>(cache[key], () => new GenericConverter<T>()); //(T)cache[key];
                return ttt;
                //return (T)GenericTypeHelper.Convert<object, T>(cache[key], () => new GenericConverter<T>());
            }
            finally
            {
                cache.Release();
            }
        }

        public object Get(string key)
        {
            return Get<object>(key);
        }

        public void Set(string key, object data)
        {
            var cache = CacheFactory.GetCache(CacheFactoryName);
            var locked = cache.Lock(key, -1);
            try
            {
                cache.Add(key, data);
            }
            finally
            {
                // Always unlock in a "finally" block
                // to ensure that uncaught exceptions
                // don't leave data locked
                if (locked)
                    cache.Unlock(key);
                cache.Release();
            }
        }

        public void Set(string key, object data, TimeSpan cacheTime)
        {
            var cache = CacheFactory.GetCache(CacheFactoryName);
            var locked = cache.Lock(key, -1);
            try
            {
                cache.Insert(key, data, (long)cacheTime.TotalMilliseconds);
            }
            finally
            {
                // Always unlock in a "finally" block
                // to ensure that uncaught exceptions
                // don't leave data locked
                if (locked)
                    cache.Unlock(key);
                cache.Release();
            }
        }

        public bool IsSet(string key)
        {
            var cache = CacheFactory.GetCache(CacheFactoryName);
            try
            {
                return cache.Contains(key);
            }
            finally
            {
                // Always unlock in a "finally" block
                // to ensure that uncaught exceptions
                // don't leave data locked
                cache.Release();
            }
        }

        public void Remove(string key)
        {
            var cache = CacheFactory.GetCache(CacheFactoryName);
            var locked = cache.Lock(key, -1);
            try
            {
                cache.Remove(key);
            }
            finally
            {
                // Always unlock in a "finally" block
                // to ensure that uncaught exceptions
                // don't leave data locked
                if (locked)
                    cache.Unlock(key);
                cache.Release();
            }
        }

        public void RemoveByPattern(string pattern)
        {
            throw new NotImplementedException();
        }

        public object GetIfNewer(string key, object version)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            var cache = CacheFactory.GetCache(CacheFactoryName);
            cache.Clear();
            cache.Release();
        }

        public static string AssemblyDirectory
        {
            get
            {
                string codeBase = Assembly.GetExecutingAssembly().CodeBase;
                UriBuilder uri = new UriBuilder(codeBase);
                string path = Uri.UnescapeDataString(uri.Path);
                return Path.GetDirectoryName(path);
            }
        }

        private void InitializeFactory()
        {

            EventLog.WriteEntry("Application", "InitiliazeFactory e girdi", EventLogEntryType.Information, 9888);

            try
            {
                CacheFactoryName = _applicationSettingService.GetSetting("CoherenceCacheFactoryName");
                CoherenceConfigPath = _applicationSettingService.GetSetting("CoherenceConfigPath");
                //programmatically coherence cache configuration
                string configEnvironmentDirectory = _applicationSettingService.GetSetting("CoherenceConfigPath");

                var relativePath = AssemblyDirectory;
                string coherenceConfigPath = string.Format(relativePath + @"\" + configEnvironmentDirectory);

                CacheFactory.DefaultCacheConfigPath = string.Format("file://{0}/{1}", coherenceConfigPath, "coherence-cache-config.xml");
                CacheFactory.DefaultOperationalConfigPath = string.Format("file://{0}/{1}", coherenceConfigPath, @"coherence.xml");
                CacheFactory.DefaultPofConfigPath = string.Format("file://{0}/{1}", coherenceConfigPath, @"pof-config.xml");
                CacheFactory.Configure(CacheFactory.DefaultCacheConfigPath, CacheFactory.DefaultOperationalConfigPath);

                EventLog.WriteEntry("Application", "InitiliazeFactory "
                    + CacheFactory.DefaultCacheConfigPath + " "
                    + CacheFactory.DefaultOperationalConfigPath + " "
                    + CacheFactory.DefaultPofConfigPath, EventLogEntryType.Information, 9888);

                EventLog.WriteEntry("Application", string.Format("CacheFactoryName {0} ", CacheFactoryName), EventLogEntryType.Information, 9888);

                //dynamic set cachefactoryname
                var cache = CacheFactory.GetCache(CacheFactoryName);
                cache.Release();

            }
            catch (Exception ex)
            {
                EventLog.WriteEntry("Application", string.Format("Error : {0}", ex), EventLogEntryType.Error, 9888);
                throw;
            }
        }
    }
}