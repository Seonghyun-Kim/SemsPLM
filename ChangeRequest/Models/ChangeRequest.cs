using Common.Constant;
using Common.Factory;
using Common.Interface;
using Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChangeRequest.Models
{
    public class ECR : DObject, IDObject
    {
        public string Title { get; set; }
        public int? Count { get; set; }
        public string EPart_No { get; set; }
        public int? Oem_Lib_OID { get; set; }
        public int? Car_Lib_OID { get; set; }
        public int? Pms_OID { get; set; }
        public string EoType { get; set; }
        public string EoProType { get; set; }
        public string EoID { get; set; }
        public string IsService { get; set; }
        public string DesignCost { get; set; }
        public string WtChange { get; set; }
        public string IsMold { get; set; }
        public string DieQt { get; set; }
        public string DieCost { get; set; }
        public string EoClass { get; set; }
        public string Eo_InvClr { get; set; }
        public string ApprovDt { get; set; }
        public string Oem_Eo { get; set; }
        public DateTime? OemDt { get; set; }
        public string OemUs { get; set; }
        public DateTime? Oem_RecDt { get; set; }
        public string Effective { get; set; }
        public string Statement { get; set; }
        public string Eo_Reason { get; set; }
        public string Oem_Lib_Nm { get; set; }
        public string Car_Lib_Nm { get; set; }
        public string Pms_Nm { get; set; }

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
    public static class ECRRepository
    {
        public static List<ECR> SelChangeRequest(ECR _param)
        {
            _param.Type = EoConstant.TYPE_CHANGE_REQUEST;
            List<ECR> lECR = DaoFactory.GetList<ECR>("ChangeRequest.SelChangeRequest", _param);
            lECR.ForEach(obj =>
            {
                obj.Oem_Lib_Nm = LibraryRepository.SelLibraryObject(new Library { OID = obj.Oem_Lib_OID }).KorNm;
                obj.Car_Lib_Nm = LibraryRepository.SelLibraryObject(new Library { OID = obj.Car_Lib_OID }).KorNm;
                obj.Pms_Nm = LibraryRepository.SelLibraryObject(new Library { OID = obj.Pms_OID }).KorNm;

                obj.BPolicy = BPolicyRepository.SelBPolicy(new BPolicy { Type = obj.Type, OID = obj.BPolicyOID }).First();
            });
            return lECR;
        }
        public static ECR SelChangeRequestObject(ECR _param)
        {
            _param.Type = EoConstant.TYPE_CHANGE_REQUEST;
            ECR lECR = DaoFactory.GetData<ECR>("ChangeRequest.SelChangeRequest", _param);

            lECR.Oem_Lib_Nm = LibraryRepository.SelLibraryObject(new Library { OID = lECR.Oem_Lib_OID }).KorNm;
            lECR.Car_Lib_Nm = LibraryRepository.SelLibraryObject(new Library { OID = lECR.Car_Lib_OID }).KorNm;
            lECR.Pms_Nm = LibraryRepository.SelLibraryObject(new Library { OID = lECR.Pms_OID }).KorNm;

            lECR.BPolicy = BPolicyRepository.SelBPolicy(new BPolicy { Type = lECR.Type, OID = lECR.BPolicyOID }).First();

            return lECR;
        }
        public static ECR UdtChangeRequest(ECR _param)
        {
            _param.Type = EoConstant.TYPE_CHANGE_REQUEST;
            DaoFactory.SetUpdate("ChangeRequest.UdtChangeRequest", _param);

            return _param;
        }


        
    }


        
}
