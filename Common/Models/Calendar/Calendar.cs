using Common.Factory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Models
{
    public class Calendar : DObject
    {
        public int? WorkingDay { get; set; }

        public int? DefaultHoliday { get; set; }

        public List<CalendarDetail> CalendarDetails { get; set; }

    }

    public static class CalendarRepository
    {
        public static Calendar SelCalendar(Calendar _param)
        {
            return DaoFactory.GetData<Calendar>("Manage.SelCalendar", _param);
        }
    }
}
