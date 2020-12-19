using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Constant;
using Common.Factory;
using Common.Models;

namespace EBom.Models
{
    public class EBOM : DRelationship
    {
        public List<EBOM> Children { get; set; }
        public List<EBOM> Parents { get; set; }

        public List<string> diseditable { get; set; }

        public string ObjName { get; set; }
        public string Description { get; set; }
        public int? BPolicyOID { get; set; }
        public BPolicy BPolicy { get; set; }
        public List<BPolicyAuth> BPolicyAuths { get; set; }
        public string ObjRevision { get; set; }
        public string ObjTdmxOID { get; set; }
        public int? ObjIsLatest { get; set; }
        public string ObjTitle { get; set; }
        public string ObjRep_Part_No { get; set; }
        public string ObjRep_Part_No2 { get; set; }
        public string ObjEo_No { get; set; }
        public DateTime? ObjEo_No_ApplyDt { get; set; }
        public string ObjEo_No_History { get; set; }
        public string ObjEtc { get; set; }
        public int? ObjApprovOID { get; set; }
        public int? ObjEPartType { get; set; }
        public string ObjEPartTypeNm { get; set; }
        public string ObjSel_Eo { get; set; }
        public DateTime? ObjSel_Eo_Dt { get; set; }
        public string ObjSpec { get; set; }
        public string ObjSurface { get; set; }
        public int? ObjOem_Lib_OID { get; set; }
        public int? ObjCar_Lib_OID { get; set; }
        public int? ObjPms_OID { get; set; }
        public int? ObjProd_Lib_Lev1_OID { get; set; }
        public int? ObjProd_Lib_Lev2_OID { get; set; }
        public int? ObjProd_Lib_Lev3_OID { get; set; }
        public string ObjDivision { get; set; }
        public int?   ObjITEM_No { get; set; }
        public string ObjITEM_NoNm { get; set; } //ITEM_NO
        public int?   ObjITEM_Middle { get; set; }
        public string ObjITEM_MiddleNm { get; set; }
        public int?   ObjProduction_Place { get; set; }
        public string ObjProduction_PlaceNm { get; set; }
        public int?   ObjBlock_No { get; set; }
        public string ObjBlock_NoNm { get; set; } //BLOCK_NONm
        public string ObjSerial { get; set; }
        public string ObjSel_Revision { get; set; }
        public int? ObjMaterial_OID { get; set; }
        public string ObjMaterial_Nm { get; set; }



        public string ObjCar_Lib_Nm { get; set; }

        public string ObjThumbnail { get; set; }

        public string Action { get; set; }
        public int? OldOID { get; set; }
        public int? RootOID { get; set; }
        public int? LToOID { get; set; }
        public int? RToOID { get; set; }

        public int? TimeOID { get; set; }
    }

    public static class EBomRepository
    {
        #region EBom Add
        public static int AddAction(EBOM _param)
        {
            int OID = DaoFactory.SetInsert("EBom.InsEBomStructure", _param);
            return OID;
        }
        #endregion

        #region EBom Delete
        public static int DeleteAction(EBOM _param)
        {
            int OID = DaoFactory.SetDelete("EBom.delEBomStructure", _param);
            return OID;
        }
        #endregion

        #region EBom Ru
        public static int RuAction(EBOM _param)
        {
            int OID = DaoFactory.SetUpdate("EBom.RuEBomStructure", _param);
            return OID;
        }
        #endregion

        #region EBom Udt
        public static int UdtAction(EBOM _param)
        {
            int OID = DaoFactory.SetUpdate("EBom.UdtEBomStructure", _param);
            return OID;
        }
        #endregion

    }
}
