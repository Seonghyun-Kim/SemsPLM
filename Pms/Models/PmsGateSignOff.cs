using Common.Factory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Pms.Models
{
    public class PmsGateSignOff
    {
        public int? OID { get; set; }

        public string Forecast { get; set; }

        public string WrCreateStore { get; set; }

        public string CalendarEtc { get; set; }

        public string NonCompleteIssue { get; set; }

        public string SummaryEtc { get; set; }

        public DateTime? CreateDt { get; set; }

        public int? CreateUs { get; set; }

        public DateTime? ModifyDt { get; set; }

        public int? ModifyUs { get; set; }

    }
    public static class PmsGateSignOffRepository
    {
        public static PmsGateSignOff SelPmsGateSignOff(HttpSessionStateBase Context, PmsGateSignOff _param)
        {
            PmsGateSignOff lSignOff = DaoFactory.GetData<PmsGateSignOff>("Pms.SelPmsGateSignOff", _param);

            return lSignOff;
        }

        public static PmsGateSignOff UdtChangeOrderObject(PmsGateSignOff _param)
        {
            DaoFactory.SetUpdate("Pms.UdtPmsGateSignOff", _param);

            return _param;
        }

    }

}
