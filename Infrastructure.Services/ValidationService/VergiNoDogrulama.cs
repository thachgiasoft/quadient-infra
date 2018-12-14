using System.Collections.Generic;
using System.Linq;

namespace Infrastructure.Services.ValidationService
{
    public class VergiNoDogrulama
    {
        #region "Sabit Mesajlar"

        private string SadeceRakamOlmali = "Vergi numarası sadece rakamlardan oluşabilir !";
        private string EksikGirildi = "Vergi numarası 10 hane olmalıdır !";
        //string IlkHaneSifirOlamaz = "Vergi numarasının ilk hanesi sıfır olamaz !";
        private string HataliNo = "Vergi numarası doğru formatta değil !";

        #endregion

        //public VergiNoDogrulama Instance
        //{
        //    get { return new VergiNoDogrulama(); }
        //}

        private readonly ITypeValidation _typeValidation;
        public VergiNoDogrulama(ITypeValidation typeValidation)
        {
            _typeValidation = typeValidation;
        }

        private string mMesaj = "";
        public string Mesaj
        {
            get
            {
                return mMesaj;
            }
        }

        public bool VergiNoDogrula(string sVergiNo)
        {
            int digitCount = sVergiNo.Length;
            if (sVergiNo.Length == 0)
            {
                mMesaj = EksikGirildi;
                return false;
            }
            //TODO burasi dusunulecek !
            if (!_typeValidation.IsNumeric(sVergiNo))
            {
                mMesaj = SadeceRakamOlmali;
                return false;
            }

            if (digitCount != 10)
            {
                mMesaj = EksikGirildi;
                return false;
            }
            long vergiNo = long.Parse(sVergiNo);
            int checkDigit = 0;
            var digits = new List<int>();
            for (int i = 0; i < digitCount; i++)
            {
                if (i == 0)
                {
                    checkDigit = (int)(vergiNo % 10);
                }
                else
                {
                    digits.Add((int)(vergiNo % 10));
                }
                vergiNo /= 10;
            }
            //eger 0 azalmssa
            for (int i = 0; i < digitCount - digits.Count; i++)
                digits.Add(0);
            for (int i = 1; i < 10; i++)
            {
                digits[i - 1] = (digits[i - 1] + i) % 10;
            }
            for (int i = 1; i < 10; i++)
            {
                digits[i - 1] = (digits[i - 1] * Power(2, i));
            }
            for (int i = 0; i < digits.Count; i++)
            {
                var digitTemp = digits[i];
                int digitSum = 0;
                while (digitTemp != 0)
                {
                    digitSum += digitTemp % 10;
                    digitTemp /= 10;
                }
                if (digitSum > 9)
                {
                    digitSum = digitSum % 9;
                    if (digitSum == 0)
                        digitSum = 9;
                }
                digits[i] = digitSum;
            }
            int total = digits.Sum();
            int greaterNumber = 0;
            if (total % 10 == 0)
                greaterNumber = total;
            else
                greaterNumber = ((total / 10) + 1) * 10;
            if (greaterNumber - total == checkDigit)
                return true;

            mMesaj = HataliNo;
            return false;
        }

        private int Power(int a, int b)
        {
            int f = 1;
            for (int i = 1; i <= b; i++)
                f *= a;
            return f;
        }
    }
}
