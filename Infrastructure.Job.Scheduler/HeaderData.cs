using System;
using System.Xml.Serialization;
using Infrastructure.Core.Helpers;

namespace Infrastructure.JobScheduling
{
    [Serializable]
    public sealed class HeaderData
    {
        public string HeaderName { get; set; }
        public string HeaderNamespace { get; set; }
        public string DataAsXmlString { get; set; }
        [XmlIgnore]
        public object Data { get; set; }

        public string ToXml()
        {
            DataAsXmlString = Data.XmlSerializeToString();
            Data = null;
            return this.XmlSerializeToString();
        }
    }
}
