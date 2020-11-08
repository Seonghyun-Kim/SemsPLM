using Common;
using Common.Constant;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pms
{
    public class PmsUtils
    {
        public static DateTime CalculateFutureDate(DateTime fromDate, int numberofWorkDays, int workingDay, ICollection<DateTime> holidays)
        {
            var futureDate = fromDate;
            if (workingDay == 5)
            {
                for (var i = PmsConstant.INIT_DURATION; i < numberofWorkDays; i++)
                {
                    if (futureDate.DayOfWeek == DayOfWeek.Saturday
                       || futureDate.DayOfWeek == DayOfWeek.Sunday
                       || (holidays != null && holidays.Contains(futureDate)))
                    {
                        futureDate = futureDate.AddDays(1);
                        numberofWorkDays++;
                    }
                    else
                    {
                        futureDate = futureDate.AddDays(1);
                    }
                    while (futureDate.DayOfWeek == DayOfWeek.Saturday
                        || futureDate.DayOfWeek == DayOfWeek.Sunday
                        || (holidays != null && holidays.Contains(futureDate)))
                    {
                        futureDate = futureDate.AddDays(1);
                    }
                }
            }
            else if (workingDay == 6)
            {
                for (var i = PmsConstant.INIT_DURATION; i < numberofWorkDays; i++)
                {
                    if (futureDate.DayOfWeek == DayOfWeek.Sunday
                       || (holidays != null && holidays.Contains(futureDate)))
                    {
                        futureDate = futureDate.AddDays(1);
                        numberofWorkDays++;
                    }
                    else
                    {
                        futureDate = futureDate.AddDays(1);
                    }
                    while (futureDate.DayOfWeek == DayOfWeek.Sunday
                        || (holidays != null && holidays.Contains(futureDate)))
                    {
                        futureDate = futureDate.AddDays(1);
                    }
                }
            }
            return futureDate;
        }

        public static int CalculateFutureDuration(DateTime fromDate, DateTime toDate, int workingDay, ICollection<DateTime> holidays)
        {
            int iBetWeenDayCnt = PmsConstant.INIT_DURATION;

            DateTime tmp;
            int i = 0;
            while (true)
            {
                tmp = fromDate.AddDays(i);
                if (workingDay == 5)
                {
                    if (tmp.DayOfWeek == DayOfWeek.Saturday
                          || tmp.DayOfWeek == DayOfWeek.Sunday
                          || (holidays != null && holidays.Contains(tmp))) { }
                    else
                    {
                        iBetWeenDayCnt++;
                    }

                    TimeSpan betWeen = toDate - tmp;
                    if (betWeen.Days <= 0)
                    {
                        break;
                    }
                    i++;
                }
                else if (workingDay == 6)
                {
                    if (tmp.DayOfWeek == DayOfWeek.Saturday
                          || (holidays != null && holidays.Contains(tmp))) { }
                    else
                    {
                        iBetWeenDayCnt++;
                    }

                    TimeSpan betWeen = toDate - tmp;
                    if (betWeen.Days <= 0)
                    {
                        break;
                    }
                    i++;
                }
            }
            return iBetWeenDayCnt;
        }

        public static int CalculateGapFutureDuration(DateTime fromDate, DateTime toDate, int workingDay, ICollection<DateTime> holidays)
        {
            int iBetWeenDayCnt = 0;

            DateTime tmp;
            int i = 0;
            while (true)
            {
                tmp = fromDate.AddDays(i);
                if (workingDay == 5)
                {
                    if (tmp.DayOfWeek == DayOfWeek.Saturday
                          || tmp.DayOfWeek == DayOfWeek.Sunday
                          || (holidays != null && holidays.Contains(tmp))) { }
                    else
                    {
                        iBetWeenDayCnt++;
                    }

                    TimeSpan betWeen = toDate - tmp;
                    if (betWeen.Days <= 0)
                    {
                        break;
                    }
                    i++;
                }
                else if (workingDay == 6)
                {
                    if (tmp.DayOfWeek == DayOfWeek.Saturday
                          || (holidays != null && holidays.Contains(tmp))) { }
                    else
                    {
                        iBetWeenDayCnt++;
                    }

                    TimeSpan betWeen = toDate - tmp;
                    if (betWeen.Days <= 0)
                    {
                        break;
                    }
                    i++;
                }
            }
            return iBetWeenDayCnt;
        }

        public static int CalculateDelay(DateTime _estEndDt, DateTime _actEndDt, int _workingDay, List<DateTime> _lHoliday)
        {
            int delay = 0;
            delay = PmsUtils.CalculateGapFutureDuration(Convert.ToDateTime(string.Format("{0:yyyy-MM-dd}", _estEndDt)), Convert.ToDateTime(string.Format("{0:yyyy-MM-dd}", _actEndDt)), _workingDay, _lHoliday);
            return delay;
        }
    }
}
