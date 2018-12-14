using System;
using Infrastructure.Core.Helpers;

namespace Infrastructure.Services.UtilityService
{
    public class DateTimeProcessingService : IDateTimeProcessingService
    {
        public long DateDiff(DateTimeHelper.DateInterval interval, DateTime dt1, DateTime dt2)
        {
            return DateTimeHelper.DateDiff(interval, dt1, dt2);
        }

        public long DateDiff(DateTimeHelper.DateInterval interval, DateTime dt1, DateTime dt2, DayOfWeek eFirstDayOfWeek)
        {
            return DateTimeHelper.DateDiff(interval, dt1, dt2, eFirstDayOfWeek);
        }

        public DateTimeHelper.TarihFarkSureler DateDiff(DateTime tarih1, DateTime tarih2)
        {
            return DateTimeHelper.DateDiff(tarih1, tarih2);
        }
    }
}
