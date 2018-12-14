using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.JobScheduling
{
    public abstract class ScheduledJobBase
    {
        public Guid IslemKey { get; set; }
        public string JobName { get; set; }
        public string GroupName { get; set; }
        public string ServiceBaseEndPointKey { get; set; }

        public byte[] FonksiyonData { get; set; }
        public string FonksiyonDataEncoded
        {
            get
            {
                return FonksiyonData == null ? null : Convert.ToBase64String(FonksiyonData);
            }
        }
        public HeaderData HeaderData { get; set; }
        public string HeaderDataXmlString { get; set; }

        //Serializer icin parametresiz ctor gerekli
        protected ScheduledJobBase()
        {
            IslemKey = Guid.NewGuid();
            JobName = string.Empty;
            GroupName = string.Empty;
            FonksiyonData = null;
            HeaderData = null;
        }

        protected ScheduledJobBase(string kuyrukAdi, string grupAdi, byte[] fonksiyonData, Guid islemKey)
            : this(kuyrukAdi, grupAdi, fonksiyonData, null, islemKey)
        {

        }

        protected ScheduledJobBase(string kuyrukAdi, string grupAdi, byte[] fonksiyonData, HeaderData headerData, Guid islemKey)
        {
            IslemKey = islemKey;
            JobName = kuyrukAdi;
            GroupName = grupAdi;
            FonksiyonData = fonksiyonData;
            HeaderData = headerData;
        }

        protected ScheduledJobBase(string kuyrukAdi, string grupAdi, string serviceBaseEndPointKey, byte[] fonksiyonData, HeaderData headerData, Guid islemKey)
        {
            IslemKey = islemKey;
            JobName = kuyrukAdi;
            GroupName = grupAdi;
            ServiceBaseEndPointKey = serviceBaseEndPointKey;
            FonksiyonData = fonksiyonData;
            HeaderData = headerData;
        }

        protected ScheduledJobBase(string kuyrukAdi, string grupAdi, string serviceBaseEndPointKey, byte[] fonksiyonData, string headerData, Guid islemKey)
        {
            IslemKey = islemKey;
            JobName = kuyrukAdi;
            GroupName = grupAdi;
            ServiceBaseEndPointKey = serviceBaseEndPointKey;
            FonksiyonData = fonksiyonData;
            HeaderDataXmlString = headerData;
        }
    }
}
