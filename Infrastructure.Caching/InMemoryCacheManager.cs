using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.ServiceModel;
using System.Text.RegularExpressions;
using Infrastructure.Core.Caching;

namespace Infrastructure.Caching
{
    /// <summary>
    /// Memory cache yönetim sınıfı.
    /// </summary>
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    public class InMemoryCacheManager : ICacheManager
    {
        /// <summary>
        /// Memory Cache nesnesi.
        /// </summary>
        protected ObjectCache Cache
        {
            get
            {
                return MemoryCache.Default;
            }
        }

        /// <summary>
        /// Cache client' ların listelendiği dictionary nesnesi.
        /// </summary>
        public ConcurrentDictionary<string, CacheClient> CacheClients { get { return _cacheClients; } }
        public event EventHandler<ObjectPutEventArgs> ObjectPut;
        private readonly ConcurrentDictionary<string, CacheClient> _cacheClients;
        public InMemoryCacheManager()
        {
            _cacheClients = new ConcurrentDictionary<string, CacheClient>();
        }

        /// <summary>
        /// Belirtilen anahtar için cache deki değeri verilen tipte döndürür.
        /// </summary>
        /// <typeparam name="T">Cache' lenmiş nesnenin türü.</typeparam>
        /// <param name="key">Cache' den istenilen nesnenin anahtar değeri.</param>
        /// <returns>Belirtilen anahtarla ilişkilendirilmiş değeri verilen tipte döndürür.</returns>
        public T Get<T>(string key) where T : class
        {
            return (T)Cache[key];
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
            if (data == null)
                return;

            Cache.Set(new CacheItem(key, data), null);
        }

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
            if (data == null)
                return;

            var policy = new CacheItemPolicy { SlidingExpiration = cacheTime };
            Cache.Set(new CacheItem(key, data), policy);
        }


        /// <summary>
        /// Belirtilen anahtar değer ile daha önce kaydedilmiş bir değerin olup olmadığı bilgisini verir.
        /// </summary>
        /// <param name="key">Cache' e kaydedilmiş değerin anahtar değeri.</param>
        /// <returns>Belirtilen ahantar değer ile daha önce kaydedilmiş bir değerin olup olmadığı bilgisini döndürür.</returns>
        public bool IsSet(string key)
        {
            return (Cache.Contains(key));
        }

        /// <summary>
        /// Belirtilen ahantar değer için kaydedilmiş veriyi Cache' den siler.
        /// </summary>
        /// <param name="key">Silinmek istenen verinin anahtar değeri.</param>
        public void Remove(string key)
        {
            Cache.Remove(key);
        }


        /// <summary>
        /// Anahtar değeri belli bir pattern da olan verileri cache den siler.
        /// </summary>
        /// <param name="pattern">Silinmek istenen verinin anahtar değerinin syntax yapısı.</param>
        public void RemoveByPattern(string pattern)
        {
            var regex = new Regex(pattern, RegexOptions.Singleline | RegexOptions.Compiled | RegexOptions.IgnoreCase);
            var keysToRemove = (from item in Cache where regex.IsMatch(item.Key) select item.Key).ToList();
            foreach (var key in keysToRemove)
            {
                Remove(key);
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
            foreach (var item in Cache)
                Remove(item.Key);
        }
    }
}