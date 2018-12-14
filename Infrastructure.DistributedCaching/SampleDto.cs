using System;


namespace Infrastructure.DistributedCaching
{
    [Serializable]
    public class SampleDto 
    {
        public SampleDto()
        {
            TestGuid = Guid.NewGuid();
        }

        public int BirimKey { get; set; }

        public int? UstBirimKey { get; set; }

        public string BirimAdi { get; set; }

        public string BirimAdiArama { get; set; }

        public int? CografiKey { get; set; }

        public string YazismaKod { get; set; }

        public int BirimTipNo { get; set; }

        public byte KaynakTipNo { get; set; }

        public int? BagliBirimKey { get; set; }

        public int? MerkezBirimKey { get; set; }

        public short RaporSiraKodu { get; set; }

        public string Aktifmi { get; set; }

        public byte GenelGorunurlukTipi { get; set; }

        public int DetayBirimTipNo { get; set; }

        public DateTime GTarihi { get; set; }

        public string SaymanlikKodu { get; set; }

        public Guid TestGuid { get; set; }

        public Guid? TestGuidNull { get; set; }

        #region Filtreleme sýrasýnda eklenenler

        public string ValuePath { get; set; }

        public string TextPath { get; set; }

        public string TipNoPath { get; set; }

        public string DetayTipNoPath { get; set; }

        public int NodeLevel { get; set; }

        public byte GorunurlukTipi { get; set; }
        #endregion

    }
}