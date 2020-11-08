using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Models
{
    public class BPolicyAuth
    {
        public int? OID { get; set; }

        public int? PolicyOID { get; set; }

        public int? AuthTargetOID { get; set; }

        public string AuthTargetDiv { get; set; }

        public string AuthNm { get; set; }

        public string AuthTitle { get; set; }

        public string AuthDiv { get; set; }

        public int? AuthObjectOID { get; set; }
    }
}
