using System;

namespace Infrastructure.Core.Helpers
{
    public class DateTimeHelper
    {
        public enum DateInterval
        {
            Day,
            DayOfYear,
            Hour,
            Minute,
            Month,
            Quarter,
            Second,
            Weekday,
            WeekOfYear,
            Year
        }

        public struct TarihFarkSureler
        {
            public int Ay { get; set; }
            public int Gun { get; set; }
            public int Yil { get; set; }
        }

        public static long DateDiff(DateInterval interval, DateTime dt1, DateTime dt2)
        {
            return DateDiff(interval, dt1, dt2, System.Globalization.DateTimeFormatInfo.CurrentInfo.FirstDayOfWeek);
        }

        public static long DateDiff(DateInterval interval, DateTime dt1, DateTime dt2, DayOfWeek eFirstDayOfWeek)
        {
            if (interval == DateInterval.Year) return dt2.Year - dt1.Year;
            if (interval == DateInterval.Month) return (dt2.Month - dt1.Month) + (12 * (dt2.Year - dt1.Year));

            TimeSpan ts = dt2 - dt1;

            if (interval == DateInterval.Day || interval == DateInterval.DayOfYear) return Round(ts.TotalDays);
            if (interval == DateInterval.Hour) return Round(ts.TotalHours);
            if (interval == DateInterval.Minute) return Round(ts.TotalMinutes);
            if (interval == DateInterval.Second) return Round(ts.TotalSeconds);
            if (interval == DateInterval.Weekday) return Round(ts.TotalDays / 7.0);

            if (interval == DateInterval.WeekOfYear)
            {
                while (dt2.DayOfWeek != eFirstDayOfWeek)
                    dt2 = dt2.AddDays(-1);
                while (dt1.DayOfWeek != eFirstDayOfWeek)
                    dt1 = dt1.AddDays(-1);
                ts = dt2 - dt1;
                return Round(ts.TotalDays / 7.0);
            }

            if (interval == DateInterval.Quarter)
            {
                double d1Quarter = GetQuarter(dt1.Month);
                double d2Quarter = GetQuarter(dt2.Month);
                double d1 = d2Quarter - d1Quarter;
                double d2 = (4 * (dt2.Year - dt1.Year));
                return Round(d1 + d2);
            }
            return 0;
        }

        private static int GetQuarter(int nMonth)
        {
            if (nMonth <= 3) return 1;
            if (nMonth <= 6) return 2;
            if (nMonth <= 9) return 3;
            return 4;
        }

        private static long Round(double dVal)
        {
            if (dVal >= 0) return (long)Math.Floor(dVal);
            return (long)Math.Ceiling(dVal);
        }

        /// <summary>
        /// İki tarih arasındaki süreleri hesaplar ve yil, ay, gün olarak geri döndürür.
        /// </summary>
        /// <param name="tarih1">Başlangıç Tarihi</param>
        /// <param name="tarih2">Bitiş Tarihi</param>
        /// <returns></returns>
        public static TarihFarkSureler DateDiff(DateTime tarih1, DateTime tarih2)
        {
            int gun = 0;
            int ay = 0;
            int yil = 0;

            int tempGun = 0;
            int tempAy = 0;
            int tempYil = 0;

            if (tarih2.Day < tarih1.Day)
            {
                tempGun = tarih2.Day + 30;
            }
            else
            {
                tempGun = tarih2.Day;
            }
            gun = tempGun - tarih1.Day;

            if ((tarih2.Month <= tarih1.Month) && tarih2.Day < tarih1.Day)
            {
                tempAy = tarih2.Month + 11;
            }
            else if (tarih2.Month < tarih1.Month)
            {
                tempAy = tarih2.Month + 12;
            }
            else if ((tarih2.Month > tarih1.Month) && (tarih2.Day < tarih1.Day))
            {
                tempAy = tarih2.Month - 1;
            }
            else
            {
                tempAy = tarih2.Month;
            }
            ay = tempAy - tarih1.Month;

            if ((tarih2.Month < tarih1.Month) || ((tarih2.Day < tarih1.Day) && tarih2.AddMonths(-1).Month < tarih1.Month))
            {
                tempYil = tarih2.Year - 1;
            }
            else
            {
                tempYil = tarih2.Year;
            }
            yil = tempYil - tarih1.Year;

            TarihFarkSureler sureler = new TarihFarkSureler();
            sureler.Ay = ay;
            sureler.Yil = yil;
            sureler.Gun = gun;
            return sureler;
        }
        public static string GetPrettyDate(DateTime d, string dakikadanAz = "Şimdi", string ikiDakikadanAz = "1 dakika önce",
            string saattenAz = "dakika önce", string ikiSaattenAz = "1 saat önce", string gundenAz = "saat önce", string dun = "Dün", string haftadanAz = "gün önce", string aydanAz = "hafta önce")
        {
            // 1.
            // Get time span elapsed since the date.
            TimeSpan s = DateTime.Now.Subtract(d);

            // 2.
            // Get total number of days elapsed.
            int dayDiff = (int)s.TotalDays;

            // 3.
            // Get total number of seconds elapsed.
            int secDiff = (int)s.TotalSeconds;

            // 4.
            // Don't allow out of range values.
            if (dayDiff < 0 || dayDiff >= 31)
            {
                return null;
            }

            // 5.
            // Handle same-day times.
            if (dayDiff == 0)
            {
                // A.
                // Less than one minute ago.
                if (secDiff < 60)
                {
                    return dakikadanAz;
                }
                // B.
                // Less than 2 minutes ago.
                if (secDiff < 120)
                {
                    return ikiDakikadanAz;
                }
                // C.
                // Less than one hour ago.
                if (secDiff < 3600)
                {
                    return string.Format("{0} {1}",
                        Math.Floor((double)secDiff / 60), saattenAz);
                }
                // D.
                // Less than 2 hours ago.
                if (secDiff < 7200)
                {
                    return ikiSaattenAz;
                }
                // E.
                // Less than one day ago.
                if (secDiff < 86400)
                {
                    return string.Format("{0} {1}",
                        Math.Floor((double)secDiff / 3600), gundenAz);
                }
            }
            // 6.
            // Handle previous days.
            if (dayDiff == 1)
            {
                return dun;
            }
            if (dayDiff < 7)
            {
                return string.Format("{0} {1}", dayDiff, haftadanAz);
            }
            if (dayDiff < 31)
            {
                return string.Format("{0} {1}",
                Math.Ceiling((double)dayDiff / 7), aydanAz);
            }
            return d.ToString();
        }
    }
}
