using Common.Factory;
using Common.Interface;
using Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pms.Models
{
    public class CustomerSchedule
    {
        public int? OID { get; set; }
        public int? Car_Lib_OID { get; set; }
        public string Name { get; set; }
        public DateTime? StartDt { get; set; }
        public int? Ord { get; set; }
    }
    public static class CustomerScheduleRepository
    {
        public static List<CustomerSchedule> SelProjMngtCustomerSchedule(CustomerSchedule _param)
        {
            List<CustomerSchedule> result = DaoFactory.GetList<CustomerSchedule>("Pms.SelProjMngtCustomerSchedule", _param);
            return result;
        }
        
        public static List<CustomerSchedule> InsProjMngtCustomerSchedule(List<CustomerSchedule> _param, int? Car)
        {

            DaoFactory.SetDelete("Pms.DelProjMngtCustomerSchedule", new CustomerSchedule { Car_Lib_OID = Car });
            if (_param != null && _param.Count > 0)
            {
                for (var i = 0; i < _param.Count; i++)
                {
                    _param[i].Ord = i + 1;
                    DaoFactory.SetInsert("Pms.InsProjMngtCustomerSchedule", _param[i]);
                }
            }
            return _param;
        }

    }
}
