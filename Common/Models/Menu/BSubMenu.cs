using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Models
{
    public class BSubMenu
    {
        public int? OID { get; set; }

        public int? MenuOID { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public int? Ord { get; set; }

        public string Link { get; set; }

        public int? IsUse { get; set; }
    }
}
