using System;

namespace Infrastructure.JobScheduling
{
    public sealed class WeeklySchedule : ScheduleBase
    {
        public DayOfWeek[] HaftalikCalismaGunleri { get; set; }
    }
}
