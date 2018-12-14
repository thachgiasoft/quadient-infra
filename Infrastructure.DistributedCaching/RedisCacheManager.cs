using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text.RegularExpressions;
using System.Threading;
using Infrastructure.Core.Caching;
using Infrastructure.Core.Helpers;
using Infrastructure.Core.Infrastructure;
using Infrastructure.ServiceModel.ServiceLibrary;
using Infrastructure.Services.SettingService;
using Newtonsoft.Json;
using StackExchange.Redis;


namespace Infrastructure.DistributedCaching
{
    public class RedisCacheManager : ServiceBase, ICacheManager
    {
        private readonly ConcurrentDictionary<string, CacheClient> _cacheClients;
        public ConcurrentDictionary<string, CacheClient> CacheClients { get { return _cacheClients; } }
        public event EventHandler<ObjectPutEventArgs> ObjectPut;
        ConfigurationOptions configurationOptions;

        private ConnectionMultiplexer LazyConnection()
        {
            var _applicationSettingService = EngineContext.Current.Resolve<IApplicationSettingService>();
            var redisSetting = _applicationSettingService.GetSetting("RedisSetting");
            var redisDbPartition = _applicationSettingService.GetSetting("RedisDbPartition");
            #region environments
            // local : 0
            // test : 1 
            // preprod : 2
            // prod : 0
            #endregion
            configurationOptions = new ConfigurationOptions() { };

            #region connections
            //configurationOptions.EndPoints.Add("localhost", 6379); // localhost
            //configurationOptions.EndPoints.Add("10.0.23.149", 6379); // test agents
            //configurationOptions.EndPoints.Add("10.0.23.240", 6379); // node 1
            //configurationOptions.EndPoints.Add("10.0.23.241", 6379); // node 2
            //configurationOptions.EndPoints.Add("10.0.90.240", 6379); // load balancer old
            //configurationOptions.EndPoints.Add("10.0.90.241", 26379); // sentinel
            #endregion
            #region sentinel - master - slave
            //var masterIp = string.Empty;
            //while (true)
            //{
            //    masterIp = GetMasterFromSentinel(redisSetting);
            //    if (masterIp.Contains("ip"))
            //    {
            //        masterIp = masterIp.Substring(masterIp.IndexOf("10."), 11);
            //        break;
            //    }
            //}
            //if (!configurationOptions.EndPoints.Any())
            #endregion

            foreach (var ip in redisSetting.Split(';'))
            {
                configurationOptions.EndPoints.Add(ip, 6379);
                //configurationOptions.EndPoints.Add("10.0.4.243", 6379);
            }
            configurationOptions.DefaultDatabase = Convert.ToInt32(redisDbPartition); // throw ex on error to see db fails
            configurationOptions.AllowAdmin = true;
            configurationOptions.AbortOnConnectFail = false;
            configurationOptions.SyncTimeout = int.MaxValue;
            //configurationOptions.Ssl = true; ??

            return ConnectionMultiplexer.Connect(configurationOptions);
        }               

        private static readonly Lazy<ConnectionMultiplexer> testconn = new Lazy<ConnectionMultiplexer>(() =>
        {
            return null;
        });
        public static ConnectionMultiplexer test => testconn.Value;
       
        public ConnectionMultiplexer redis
        {
            get
            {
                return LazyConnection();
            }
        }

        private IDatabase _db;

        public IDatabase db
        {
            get
            {
                // sentinel master bilgisi için yorum satırı yapıldı - heartbeat
                try
                {
                    if (_db == null)
                    {
                        _db = redis.GetDatabase();
                        return _db;
                    }
                    return _db;
                }
                catch (Exception)
                {
                    _db = redis.GetDatabase();
                    return _db;
                }

                //_db = redis.GetDatabase();
                //return _db;
            }
        }

        public RedisCacheManager()
        {
            EventLog.WriteEntry("Application", "Redis Constructor", EventLogEntryType.Information, 9888);
        }
        public RedisCacheManager(IApplicationSettingService applicationSettingService)
        {
            EventLog.WriteEntry("Application", "Redis Constructor", EventLogEntryType.Information, 9888);
        }

        protected virtual string Serialize(object serializableObject)
        {
            var serializedObjectToCache = JsonConvert.SerializeObject(serializableObject
                        , Formatting.Indented
                        , new JsonSerializerSettings
                        {
                            ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
                            PreserveReferencesHandling = PreserveReferencesHandling.Objects,
                            TypeNameHandling = TypeNameHandling.All
                        });

            return serializedObjectToCache;
        }

        protected virtual T Deserialize<T>(string serializedObject)
        {
            return JsonConvert.DeserializeObject<T>(serializedObject
                           , new JsonSerializerSettings
                           {
                               ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
                               PreserveReferencesHandling = PreserveReferencesHandling.Objects,
                               TypeNameHandling = TypeNameHandling.All
                           });
        }

        /// <summary>
        /// Belirtilen anahtar için cache deki değeri verilen tipte döndürür.
        /// </summary>
        /// <typeparam name="T">Cache' lenmiş nesnenin türü.</typeparam>
        /// <param name="key">Cache' den istenilen nesnenin anahtar değeri.</param>
        /// <returns>Belirtilen anahtarla ilişkilendirilmiş değeri verilen tipte döndürür.</returns>
        public T Get<T>(string key) where T : class
        {
            var value = db.StringGet(key, CommandFlags.PreferSlave);

            if (!value.HasValue)
                return default(T);

            return Deserialize<T>(value);
        }

        /// <summary>
        /// Belirtilen anahtar için cache deki değeri object türünde döndürür.
        /// </summary>
        /// <param name="key">İstenilen değerin anahtar değeri.</param>
        /// <returns>Belirtilen anahtarla ilişkilendirilmiş değeri object türünde döndürür.</returns>
        public object Get(string key)
        {
            return Get<object>(key);
        }


        /// <summary>
        /// Object türünde verilen değeri belirtilen anahtar ile ilişkili olarak Cache' e kalıcı olarak kaydeder.
        /// İlgili anahtar ile daha önce kaydedilmiş değer var ise üzerine yazılır.
        /// Verilen değer (data) null ise işlem gerçekleştirilmez.
        /// </summary>
        /// <param name="key">Cache' e kaydedilecek değerin anahtar değeri.</param>
        /// <param name="data">Cache' e kaydedilecek değer.</param>
        public void Set(string key, object data)
        {
            if (data == null || IsSet(key))
                return;

            db.StringSet(key, Serialize(data), null, When.Always, CommandFlags.DemandMaster);
        }

        // implementerı yok ama lazım olur diye yazıldı. Barış 07.2018
        //public List<string> GetKeyList()
        //{
        //    var list = new List<string>();

        //    var endPoints = Connection.GetEndPoints();

        //    foreach (var endpoint in endPoints)
        //    {
        //        var server = Connection.GetServer(endpoint);

        //        var enumerable = server.Keys(RedisCache.Database);

        //        foreach (var redisKey in enumerable)
        //            list.Add(redisKey);
        //    }

        //    return list;
        //}

        /// <summary>
        /// Object türünde verilen değeri (data) belirtilen anahtar ile ilişkili olarak Cache' e belirtilen süre boyunca saklanacak şekilde kaydeder.
        /// İlgili anahtar ile daha önce kaydedilmiş değer var ise üzerine yazılır.
        /// Verilen değer (data) null ise işlem gerçekleştirilmez.
        /// </summary>
        /// <param name="key">Cache' e kaydedilecek değerin anahtar değeri.</param>
        /// <param name="data">Cache' e kaydedilecek değer.</param>
        /// <param name="cacheTime">Cache' e kaydedilecek değerin hangi sıklıkla erişilmezse silineceğini belirleyen süre.</param>
        public void Set(string key, object data, TimeSpan cacheTime)
        {
            if (data == null || IsSet(key))
                return;
            var ts = cacheTime;
            db.StringSet(key, Serialize(data), ts, When.Always, CommandFlags.DemandMaster);
        }


        /// <summary>
        /// Belirtilen anahtar değer ile daha önce kaydedilmiş bir değerin olup olmadığı bilgisini verir.
        /// </summary>
        /// <param name="key">Cache' e kaydedilmiş değerin anahtar değeri.</param>
        /// <returns>Belirtilen ahantar değer ile daha önce kaydedilmiş bir değerin olup olmadığı bilgisini döndürür.</returns>
        public bool IsSet(string key)
        {
            return db.KeyExists(key, CommandFlags.PreferSlave);
        }

        /// <summary>
        /// Belirtilen ahantar değer için kaydedilmiş veriyi Cache' den siler.
        /// </summary>
        /// <param name="key">Silinmek istenen verinin anahtar değeri.</param>
        public void Remove(string key)
        {
            if (!IsSet(key))
                return;
            db.KeyDelete(key, CommandFlags.DemandMaster);
        }


        /// <summary>
        /// Anahtar değeri belli bir pattern da olan verileri cache den siler.
        /// </summary>
        /// <param name="pattern">Silinmek istenen verinin anahtar değerinin syntax yapısı.</param>
        public void RemoveByPattern(string pattern)
        {
            var endPoints = redis.GetEndPoints();

            foreach (var endpoint in endPoints)
            {
                var server = redis.GetServer(endpoint);
                var enumerable = server.Keys(db.Database, pattern);
                foreach (var current in enumerable)
                    Remove(current);
            }
        }
        /// <summary>
        /// Impelement edilmedi.
        /// </summary>
        /// <exception cref="NotImplementedException">throws NotImplementedException</exception>
        public object GetIfNewer(string key, object version)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Cache' in tamamını temizler.
        /// </summary>
        public void Clear()
        {
            var endPoints = redis.GetEndPoints();

            foreach (var endpoint in endPoints)
            {
                var server = redis.GetServer(endpoint);

                var enumerable = server.Keys(db.Database);

                foreach (var current in enumerable)
                    Remove(current);
            }
        }
    }
}