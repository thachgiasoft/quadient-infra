using System;

namespace Infrastructure.Services.ValidationService
{
    public class TCKimlikNoDogrulama
    {
        #region "Sabite Mesajlar"

        string BosOlamaz = "T.C.Kimlik numarası alanı boş bırakılamaz !";
        string SadeceRakamOlmali = "T.C.Kimlik numarası sadece rakamlardan oluşabilir !";
        string EksikGirildi = "T.C.Kimlik numarası 11 hane olmalıdır !";
        string IlkHaneSifirOlamaz = "T.C.Kimlik numarasının ilk hanesi sıfır olamaz !";
        string HataliNo = "T.C.kimlik numarası doğru formatta değil !";

        #endregion

        private string mMesaj = "";
        public string Mesaj
        {
            get
            {
                return mMesaj;
            }
        }

        private readonly ITypeValidation _typeValidation;

        public TCKimlikNoDogrulama(ITypeValidation typeValidation)
        {
            _typeValidation = typeValidation;
        }

        /// <summary>
        /// Verilen bir T.C.Kimlik Numarasının doğru formatta olup olmadığının kontrolü için kullanılan methoddur
        /// </summary>
        /// <param name="pTCNo">Kontrolü yapılacak olan T.C.Kimlik Numarası</param>
        /// <returns></returns>
        public bool TCKimlikKontrol(string pTCNo)
        {
            try
            {
                if (pTCNo.Length == 0)
                {
                    mMesaj = BosOlamaz;
                    return false;
                }
                if (!_typeValidation.IsNumeric(pTCNo))
                {
                    mMesaj = SadeceRakamOlmali;
                    return false;
                }
                if (pTCNo.Length != 11)
                {
                    mMesaj = EksikGirildi;
                    return false;
                }
                int ilHane = int.Parse(pTCNo.Substring(0, 1));
                if (ilHane == 0)
                {
                    mMesaj = IlkHaneSifirOlamaz;
                    return false;
                }

                int[] TC = new int[11];

                for (int i = 0; i < 11; i++)
                {
                    string a = pTCNo[i].ToString();   //her string bir arraydir!
                    TC[i] = Convert.ToInt32(a);
                }

                int tekler = 0;
                int ciftler = 0;

                for (int k = 0; k < 9; k++)
                {
                    if (k % 2 == 0)         //yani çift ise ama bizim tek elemanımız oluyor dizinin ilk elemanı 0 olduğu için.
                        tekler += TC[k];
                    else if (k % 2 != 0)    //yani tek ise ama bizde çift eleman oluyor.
                        ciftler += TC[k];
                }

                int t1 = (tekler * 3) + ciftler;
                int c1 = (10 - (t1 % 10)) % 10;
                int t2 = c1 + ciftler;
                int t3 = (t2 * 3) + tekler;
                int c2 = (10 - (t3 % 10)) % 10;

                if (c1 == TC[9] && c2 == TC[10]) //son iki basamak kontrolü
                    return true;
                else
                {
                    mMesaj = HataliNo;
                    return false;
                }

            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
