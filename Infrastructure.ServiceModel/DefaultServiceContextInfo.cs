using System;
using System.Xml.Serialization;

namespace Infrastructure.ServiceModel
{
    [XmlInclude(typeof(ServiceContextInfo))]
   
    public class DefaultServiceContextInfo : ServiceContextInfo
    {
    }
}
