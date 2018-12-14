using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.ServiceModel
{
    /// <summary>
    /// Bu info distributed mimarilerde calismalarda load balancerdan onceki sunucunun bilgilerini icerir
    /// </summary>
    [Serializable]
    public class ClientComputerInfo
    {
        /// <summary>
        /// istek yapan (web iis vs) sunucu adi
        /// </summary>
        public string ClientComputerName { get; set; }
        /// <summary>
        /// istek yapan (web iis vs) sunucu ip adresi
        /// </summary>
        public string ClientComputerIpAddress { get; set; }
    }
}
