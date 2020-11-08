using Common.Constant;
using Common.Factory;
using Common.Interface;
using Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ChangeOrder.Models
{
    public class ECO : DObject, IDObject
    {
        public DateTime? EoDt { get; set; }
        public string Title { get; set; }
        public int? Count { get; set; }
        public string EPart_No { get; set; }
        public int? Oem_Lib_OID { get; set; }
        public string Oem_Lib_OID_KorNm { get; set; }
        public int? Car_Lib_OID { get; set; }
        public string Car_Lib_OID_KorNm { get; set; }
        public int? Pms_OID { get; set; }
        public string Pms_OID_KorNm { get; set; }
        public string EoType { get; set; }
        public string EoType_Nm
        {
            get
            {
                string EoType_Nm = "";
                if (EoType == EoConstant.TYPE_CUSTOMER) EoType_Nm= EoConstant.TYPE_CUSTOMER_KorNm;
                else if (EoType == EoConstant.TYPE_HOUSE) EoType_Nm = EoConstant.TYPE_HOUSE_KorNm;
                return EoType_Nm;
            }
        }
        public string EoProType { get; set; }
        public string EoID { get; set; }
        public string EoID_Nm
        {
            get
            {
                switch (this.EoID)
                {
                    case "N": return "N(" + EoConstant.ID_N + ")";
                    case "D": return "D(" + EoConstant.ID_D + ")";
                    case "R": return "R(" + EoConstant.ID_R + ")";
                    default: return "";
                }
            }
        }
        public string IsService { get; set; }
        public DateTime? DrwDt { get; set; }
        public string DesignCost { get; set; }
        public string WtChange { get; set; }
        public string IsMold { get; set; }
        public string DieQt { get; set; }
        public string DieCost { get; set; }
        public string EoClass { get; set; }
        public string EoClass_Nm
        {
            get
            {
                string EoClass_Nm = "";
                if (EoClass == EoConstant.CLASS_EMERGENCY) EoClass_Nm = EoConstant.CLASS_EMERGENCY + "(" + EoConstant.CLASS_EMERGENCY_KorNm + ")";
                else if (EoClass == EoConstant.CLASS_MPR) EoClass_Nm = EoConstant.CLASS_MPR + "(" + EoConstant.CLASS_MPR_KorNm + ")";
                else if (EoClass == EoConstant.CLASS_MINOR) EoClass_Nm = EoConstant.CLASS_MINOR + "(" + EoConstant.CLASS_MINOR_KorNm + ")";
                else if (EoClass == EoConstant.CLASS_MAJOR) EoClass_Nm = EoConstant.CLASS_MAJOR + "(" + EoConstant.CLASS_MAJOR_KorNm + ")";
                return EoClass_Nm;
            }
        }
        public string Eo_InvClr { get; set; }
        public string Eo_InvClr_Nm
        {
            get
            {
                string Eo_InvClr_Nm = "";
                if (Eo_InvClr == EoConstant.INV_NONE) Eo_InvClr_Nm = EoConstant.INV_NONE + "(" + EoConstant.INV_NONE_KorNm + ")";
                else if (Eo_InvClr == EoConstant.INV_USE) Eo_InvClr_Nm = EoConstant.INV_USE + "(" + EoConstant.INV_USE_KorNm + ")";
                else if (Eo_InvClr == EoConstant.INV_SCRAP) Eo_InvClr_Nm = EoConstant.INV_SCRAP + "(" + EoConstant.INV_SCRAP_KorNm + ")";
                else if (Eo_InvClr == EoConstant.INV_REWORK) Eo_InvClr_Nm = EoConstant.INV_REWORK + "(" + EoConstant.INV_REWORK_KorNm + ")";
                return Eo_InvClr_Nm;
            }
        }
    public string Eo_Fault { get; set; }
        public string Eo_Fault_Nm
        {
            get
            {
                string Eo_Fault_Nm = "";
                if (Eo_Fault == EoConstant.FAULT_COMP) Eo_Fault_Nm = EoConstant.FAULT_COMP_KorNm;
                else if (Eo_Fault == EoConstant.FAULT_CUST) Eo_Fault_Nm = EoConstant.FAULT_CUST_KorNm;
                else if (Eo_Fault == EoConstant.FAULT_EMPTY) Eo_Fault_Nm = EoConstant.FAULT_EMPTY_KorNm;
                else if (Eo_Fault == EoConstant.FAULT_SELF) Eo_Fault_Nm = EoConstant.FAULT_SELF_KorNm;
                return Eo_Fault_Nm;
            }
        }
        public string TeamNm { get; set; }
        public string ApprovDt { get; set; }
        public string Oem_Eo { get; set; }
        public DateTime? OemDt { get; set; }
        public int? OemUs { get; set; }
        public string OemUsNm { get; set; }
        public DateTime? Oem_RecDt { get; set; }
        public string Effective { get; set; }
        public string Statement { get; set; }

        public string Eo_Reason { get; set; }

        public string Eo_Reason_Nm
        {
            get
            {
                if (this.Eo_Reason != null && this.Eo_Reason.Count() > 0)
                {
                    string[] arrReason = this.Eo_Reason.IndexOf(',') > -1 ? this.Eo_Reason.Split(',') : new string[] { this.Eo_Reason };
                    string retrunVal = "";
                    foreach (string arrVal in arrReason)
                    {
                        switch (arrVal)
                        {
                            case "SR": retrunVal += Common.Constant.EoConstant.SR + ","; break;
                            case "PI": retrunVal += Common.Constant.EoConstant.PI + ","; break;
                            case "IC": retrunVal += Common.Constant.EoConstant.IC + ","; break;
                            case "CS": retrunVal += Common.Constant.EoConstant.CS + ","; break;
                            case "CR": retrunVal += Common.Constant.EoConstant.CR + ","; break;
                            case "QI": retrunVal += Common.Constant.EoConstant.QI + ","; break;
                            case "ST": retrunVal += Common.Constant.EoConstant.ST + ","; break;
                            case "IR": retrunVal += Common.Constant.EoConstant.IR + ","; break;
                            case "WD": retrunVal += Common.Constant.EoConstant.WD + ","; break;
                            case "LO": retrunVal += Common.Constant.EoConstant.LO + ","; break;
                            case "RA": retrunVal += Common.Constant.EoConstant.RA + ","; break;
                            case "GR": retrunVal += Common.Constant.EoConstant.GR + ","; break;
                            default: retrunVal += ""; break;
                        }
                    }
                    return retrunVal;
                }
                else
                {
                    return "";
                }
            }
        }


    }

    public static class ECORepository
    {
        public static List<ECO> SelChangeOrder(ECO _param)
        {
            _param.Type = EoConstant.TYPE_CHANGE_ORDER;
            List<ECO> lECO = DaoFactory.GetList<ECO>("ChangeOrder.SelChangeOrder", _param);
            lECO.ForEach(obj =>
            {
                obj.Oem_Lib_OID_KorNm = LibraryRepository.SelLibraryObject(new Library { OID = obj.Oem_Lib_OID }).KorNm;
                obj.Car_Lib_OID_KorNm = LibraryRepository.SelLibraryObject(new Library { OID = obj.Car_Lib_OID }).KorNm;
                obj.Pms_OID_KorNm = LibraryRepository.SelLibraryObject(new Library { OID = obj.Pms_OID }).KorNm;

                obj.BPolicy = BPolicyRepository.SelBPolicy(new BPolicy { Type = obj.Type, OID = obj.BPolicyOID }).First();
            });
            return lECO;
        }

        public static ECO SelChangeOrderObject(ECO _param)
        {
            _param.Type = EoConstant.TYPE_CHANGE_ORDER;
            ECO lECO = DaoFactory.GetData<ECO>("ChangeOrder.SelChangeOrder", _param);

            lECO.Oem_Lib_OID_KorNm = LibraryRepository.SelLibraryObject(new Library { OID = lECO.Oem_Lib_OID }).KorNm;
            lECO.Car_Lib_OID_KorNm = LibraryRepository.SelLibraryObject(new Library { OID = lECO.Car_Lib_OID }).KorNm;
            lECO.Pms_OID_KorNm = LibraryRepository.SelLibraryObject(new Library { OID = lECO.Pms_OID }).KorNm;

            lECO.BPolicy = BPolicyRepository.SelBPolicy(new BPolicy { Type = lECO.Type, OID = lECO.BPolicyOID }).First();
            return lECO;
        }
        #region 설계변경 수정
        public static ECO UdtChangeOrderObject(ECO _param)
        {
            DaoFactory.SetUpdate("ChangeOrder.UdtChangeOrder", _param);

            return _param;
        }
        #endregion
    }
}
