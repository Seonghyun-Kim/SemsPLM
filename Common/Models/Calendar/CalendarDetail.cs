using Common.Factory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Models
{
    public class CalendarDetail
    {
        public int? CalendarOID { get; set; }

        public int? CalendarDetailOID { get; set; }

        public int? Year { get; set; }

        public int? Month { get; set; }

        public int? Day { get; set; }

        public string FullDate
        {
            get
            {
                return "#" + this.Year + "-" + string.Format("{0:D2}", this.Month) + "-" + string.Format("{0:D2}", this.Day) + "#";
            }
        }

        public string Title { get; set; }

        public string Contents { get; set; }

        public int? IsHoliday { get; set; }

    }

    public static class CalendarDetailRepository
    {
        public static List<CalendarDetail> SelCalendarDetails(CalendarDetail _param)
        {
            return DaoFactory.GetList<CalendarDetail>("Manage.SelCalenderDetail", _param);
        }
    }
}
