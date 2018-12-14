using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Infrastructure.ServiceModel
{
    [Serializable()]
    [XmlRoot(ElementName = "")]
    public abstract class ServiceContextInfo
    {
        [XmlElement()]
        public object UserId { get; set; }
        [XmlElement()]
        public string ClientIp { get; set; }
    }
}
