using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.JobScheduling
{
    public sealed class ScheduledJob : ScheduledJobBase
    {
        public ScheduledJob()
            : base()
        { }

        public ScheduledJob(string kuyrukAdi, string grupAdi, byte[] fonksiyonData)
            : this(kuyrukAdi, grupAdi, fonksiyonData, Guid.NewGuid())
        { }

        public ScheduledJob(string kuyrukAdi, string grupAdi, byte[] fonksiyonData, HeaderData headerData)
            : this(kuyrukAdi, grupAdi, fonksiyonData, headerData, Guid.NewGuid())
        { }

        public ScheduledJob(string kuyrukAdi, string grupAdi, string serviceBaseEndPointKey, byte[] fonksiyonData, HeaderData headerData)
            : this(kuyrukAdi, grupAdi, serviceBaseEndPointKey, fonksiyonData, headerData, Guid.NewGuid())
        { }

        public ScheduledJob(string kuyrukAdi, string grupAdi, string serviceBaseEndPointKey, byte[] fonksiyonData, string headerData)
            : this(kuyrukAdi, grupAdi, serviceBaseEndPointKey, fonksiyonData, headerData, Guid.NewGuid())
        { }

        public ScheduledJob(string kuyrukAdi, string grupAdi, byte[] fonksiyonData, Guid islemKey)
            : base(kuyrukAdi, grupAdi, fonksiyonData, islemKey)
        { }

        public ScheduledJob(string kuyrukAdi, string grupAdi, byte[] fonksiyonData, HeaderData headerData, Guid islemKey)
            : base(kuyrukAdi, grupAdi, fonksiyonData, headerData, islemKey)
        { }

        public ScheduledJob(string kuyrukAdi, string grupAdi, string serviceBaseEndPointKey, byte[] fonksiyonData, HeaderData headerData, Guid islemKey)
            : base(kuyrukAdi, grupAdi, serviceBaseEndPointKey, fonksiyonData, headerData, islemKey)
        { }

        public ScheduledJob(string kuyrukAdi, string grupAdi, string serviceBaseEndPointKey, byte[] fonksiyonData, string headerData, Guid islemKey)
            : base(kuyrukAdi, grupAdi, serviceBaseEndPointKey, fonksiyonData, headerData, islemKey)
        { }

        public Guid? OnKosulIslemKey { get; set; }
    }
}
