using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pms.Models
{
    public class PmsCompare : PmsBaseLineRelationship
    {
        public List<PmsCompare> CompareChildren { get; set; }

        public int? ProjectOID { get; set; }

        public string LName { get; set; }

        public string LType { get; set; }

        public int? LId { get; set; }

        public string LDependency { get; set; }

        public int LEstDuration { get; set; }

        public DateTime? LEstStartDt { get; set; }

        public DateTime? LEstEndDt { get; set; }

        public int LActDuration { get; set; }

        public DateTime? LActStartDt { get; set; }

        public DateTime? LActEndDt { get; set; }

        public int? LUserOID { get; set; }

        public string LUserNm { get; set; }

        public string LDeptNm { get; set; }

        public int? LWorkingDay { get; set; }

        public int? LCalendarOID { get; set; }

        public List<PmsRelationship> LMembers { get; set; }

        public int? LOrd { get; set; }

        public string RName { get; set; }

        public string RType { get; set; }

        public string RProjectStNm { get; set; }

        public int? RId { get; set; }

        public string RDependency { get; set; }

        public int REstDuration { get; set; }

        public DateTime? REstStartDt { get; set; }

        public DateTime? REstEndDt { get; set; }

        public int RActDuration { get; set; }

        public DateTime? RActStartDt { get; set; }

        public DateTime? RActEndDt { get; set; }

        public int? RUserOID { get; set; }

        public string RUserNm { get; set; }

        public string RDeptNm { get; set; }

        public int? RWorkingDay { get; set; }

        public int? RCalendarOID { get; set; }

        public List<PmsRelationship> RMembers { get; set; }

        public int? ROrd { get; set; }
    }
}
