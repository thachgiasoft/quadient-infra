using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.Core.Infrastructure;
using Infrastructure.Core.TypeFinders;

namespace Infrastructure.Core.Caching
{

    /// <summary>
    /// Tüm cache manager lar bu interface i implement etmek zorundadır.
    /// </summary>
    [ServiceContract]
    public interface ICacheManager
    {
        /// <summary>
        /// Cache client' ların listelendiği dictionary nesnesi.
        /// </summary>
        ConcurrentDictionary<string, CacheClient> CacheClients { get; }
        /// <summary>
        /// Object put event
        /// </summary>
        event EventHandler<ObjectPutEventArgs> ObjectPut;


        /// <summary>
        /// Belirtilen anahtar için cache deki değeri verilen tipte döndürür.
        /// </summary>
        /// <typeparam name="T">Cache' lenmiş nesnenin türü.</typeparam>
        /// <param name="key">Cache' den istenilen nesnenin anahtar değeri.</param>
        /// <returns>Belirtilen anahtarla ilişkilendirilmiş değeri verilen tipte döndürür.</returns>
        T Get<T>(string key) where T : class;

        /// <summary>
        /// Belirtilen anahtar için cache deki değeri object türünde döndürür.
        /// </summary>
        /// <param name="key">İstenilen değerin anahtar değeri.</param>
        /// <returns>Belirtilen anahtarla ilişkilendirilmiş değeri object türünde döndürür.</returns>
        [OperationContract(Name = "GetObject")]
        object Get(string key);

        /// <summary>
        /// Object türünde verilen değeri belirtilen anahtar ile ilişkili olarak Cache' e kaydeder.
        /// </summary>
        /// <param name="key">Cache' e kaydedilecek değerin anahtar değeri.</param>
        /// <param name="data">Cache' e kaydedilecek değer.</param>
        [OperationContract]
        void Set(string key, object data);


        /// <summary>
        /// Object türünde verilen değeri (data) belirtilen anahtar ile ilişkili olarak Cache' e belirtilen süre boyunca saklanacak şekilde kaydeder.
        /// </summary>
        /// <param name="key">Cache' e kaydedilecek değerin anahtar değeri.</param>
        /// <param name="data">Cache' e kaydedilecek değer.</param>
        /// <param name="cacheTime">Cache' e kaydedilecek değerin Cache' de tutulma süresi.</param>

        [OperationContract(Name = "SetWithTimeSpan")]
        void Set(string key, object data, TimeSpan cacheTime);

        /// <summary>
        /// Belirtilen anahtar değer ile daha önce kaydedilmiş bir değerin olup olmadığı bilgisini verir.
        /// </summary>
        /// <param name="key">Cache' e kaydedilmiş değerin anahtar değeri.</param>
        /// <returns>Belirtilen ahantar değer ile daha önce kaydedilmiş bir değerin olup olmadığı bilgisini döndürür.</returns>
        [OperationContract]
        bool IsSet(string key);

        /// <summary>
        /// Belirtilen ahantar değer için kaydedilmiş veriyi Cache' den siler.
        /// </summary>
        /// <param name="key">Silinmek istenen verinin anahtar değeri.</param>
        [OperationContract]
        void Remove(string key);

        /// <summary>
        /// Anahtar değeri belli bir pattern da olan verileri cache den siler.
        /// </summary>
        /// <param name="pattern">Silinmek istenen verinin anahtar değerinin syntax yapısı.</param>
        [OperationContract]
        void RemoveByPattern(string pattern);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="version"></param>
        /// <returns></returns>
        [OperationContract]
        object GetIfNewer(string key, object version);

        /// <summary>
        /// Cache' in tamamını temizler.
        /// </summary>
        [OperationContract]
        void Clear();

    }

}

