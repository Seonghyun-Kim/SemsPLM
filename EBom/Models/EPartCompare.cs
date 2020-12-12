using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EBom.Models
{
    public class EPartCompare : EBOM
    {
        public List<EPartCompare> CompareChildren { get; set; }
        public int? LOId { get; set; }
        public string LName { get; set; }
        public string LType { get; set; }
        public int? LId { get; set; }
        public int? LOrd { get; set; }
        public int? LToOld { get; set; }
        public string LRevision { get; set; }
        public int? LCar_Lib_OID { get; set; }
        public string LCar_Lib_NM { get; set; }
        public string LThumbnail { get; set; }


        public int? ROId { get; set; }
        public string RName { get; set; }
        public string RType { get; set; }
        public int? RId { get; set; }
        public int? ROrd { get; set; }
        public int? RToOld { get; set; }
        public string RRevision { get; set; }
        public int? RCar_Lib_OID { get; set; }
        public string RCar_Lib_NM { get; set; }
        public string RThumbnail { get; set; }

    }
}
