using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Core.Caching
{
    /// <summary>
    /// CacheClient sınıfı.
    /// </summary>
    [Serializable]
    public class CacheClient
    {
        /// <summary>
        /// CacheClient isim bilgisi.
        /// </summary>
        public string CacheClientName { get; set; }
        /// <summary>
        /// Anahtar bilgisi.
        /// </summary>
        public string CacheKey { get; set; }
    }
}
