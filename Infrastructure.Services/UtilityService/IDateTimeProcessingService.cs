using System;
using System.ServiceModel;
using Infrastructure.Core.Extensions;
using Infrastructure.Core.Helpers;

namespace Infrastructure.Services.UtilityService
{
    [ServiceContract]
    public interface IDateTimeProcessingService
    {
        [OperationContract]
        long DateDiff(DateTimeHelper.DateInterval interval, DateTime dt1, DateTime dt2);
        [OperationContract]
        long DateDiff(DateTimeHelper.DateInterval interval, DateTime dt1, DateTime dt2, DayOfWeek eFirstDayOfWeek);
        /// <summary>
        /// İki tarih arasındaki süreleri hesaplar ve yil, ay, gün olarak geri döndürür.
        /// </summary>
        /// <param name="tarih1">Başlangıç Tarihi</param>
        /// <param name="tarih2">Bitiş Tarihi</param>
        /// <returns></returns>
        [OperationContract]
        DateTimeHelper.TarihFarkSureler DateDiff(DateTime tarih1, DateTime tarih2);
    }
}