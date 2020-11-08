using Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Constant
{
    public class EBomConstant
    {
        public static string TYPE_PART = "PART";

        public static string TABLE_PART = "T_DEPART";

        public static string PART_TYPE_ASSY = "A";
        public static string PART_TYPE_SASSY = "S";
        public static string PART_TYPE_DETAIL = "D";

        public static List<string> DISEDITABLE
        {
            get => new List<string> { 
            "OID"
            ,"FromOID"
            ,"ToOID"
            ,"Level"
            ,"ObjName"
            ,"ObjTitle"
            ,"ObjRep_Part_No"
            ,"ObjRep_Part_No2"
            ,"ObjEo_No"
            ,"ObjEPartType"
            ,"ObjThumbnail"
            ,"ObjOem_Lib_OID"
            ,"ObjCar_Lib_OID"
            ,"ObjPms_OID"
            ,"ObjOem_Lib_NM"
            ,"ObjCar_Lib_NM"
            ,"ObjPms_NM" };
        }
        public static List<string> FLOWEDITABLE
        {
            get => new List<string> {
            "OID"
            ,"FromOID"
            ,"ToOID"
            ,"Level"
            ,"ObjTitle"
            ,"ObjRep_Part_No"
            ,"ObjRep_Part_No2"
            ,"ObjEo_No"
            ,"ObjEPartType"
            ,"ObjThumbnail"
            ,"ObjOem_Lib_OID"
            ,"ObjCar_Lib_OID"
            ,"ObjPms_OID"
            ,"ObjOem_Lib_NM"
            ,"ObjCar_Lib_NM"
            ,"ObjPms_NM" };
        }
    }
}