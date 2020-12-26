using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
}
