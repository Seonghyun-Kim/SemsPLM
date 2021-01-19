using Common.Constant;
using Common.Factory;
using Common.Interface;
using Common.Models;
using Common.Models.File;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace ChangeRequest.Models
{
    public class ECR : DObject, IDObject, IObjectFile
    {
        public string ItemGroup { get; set; } //품목그룹
        public string Title { get; set; } //제목
        public DateTime? DesignChangeDt { get; set; }

        public DateTime? StartDesignChangeDt { get; set; }
        public DateTime? EndDesignChangeDt { get; set; }

        public int? ReasonChangeRequest { get; set; }
        public string Compatible { get; set; }
        public string IsMold { get; set; }
        public string ReasonRegistration { get; set; }
        public string Security { get; set; }
        public string Changes { get; set; }
        public string Stock { get; set; }
        public string Memo { get; set; }
        public string IsAttached { get; set; }
        public DateTime? SelRequestDt { get; set; }
        public string SelPartNo { get; set; }
        public DateTime? EBom { get; set; }
        public string DevMP { get; set; }
        public string CarType { get; set; }
        public string Status { get; set; }

        public string DevMPNm
        {
            get
            {
                if (this.DevMP == EoConstant.TYPE_DEV)
                {
                    return EoConstant.TYPE_DEV_KorNm;
                }
                else if (this.DevMP == EoConstant.TYPE_MP)
                {
                    return EoConstant.TYPE_MP_KorNm;
                }
                else
                {
                    return "";
                }

            }
        }
        public string IsMoldNm
        {
            get
            {
                if (this.IsMold == EoConstant.TYPE_IRRELEVANT)
                {
                    return EoConstant.TYPE_IRRELEVANT_KorNm;
                }
                else if (this.IsMold == EoConstant.TYPE_YES)
                {
                    return EoConstant.TYPE_YES_KorNm;
                }
                else if (this.IsMold == EoConstant.TYPE_NEW)
                {
                    return EoConstant.TYPE_NEWDEV_KorNm;
                }
                else
                {
                    return "";
                }

            }
        }
        public string ReasonRegistrationNm
        {
            get
            {
                if (this.ReasonRegistration == EoConstant.TYPE_ECO)
                {
                    return EoConstant.TYPE_ECO_KorNm;
                }
                else if (this.ReasonRegistration == EoConstant.TYPE_NEW)
                {
                    return EoConstant.TYPE_NEW_KorNm;
                }
                else
                {
                    return "";
                }

            }
        }
        public string SecurityNm
        {
            get
            {
                if (this.Security == EoConstant.TYPE_SECURITY)
                {
                    return EoConstant.TYPE_SECURITY_KorNm;
                }
                else if (this.Security == EoConstant.TYPE_IMPORTANT)
                {
                    return EoConstant.TYPE_IMPORTANT_KorNm;
                }
                else if (this.Security == EoConstant.TYPE_LAW)
                {
                    return EoConstant.TYPE_LAW_KorNm;
                }
                else if (this.Security == EoConstant.TYPE_COMMON)
                {
                    return EoConstant.TYPE_COMMON_KorNm;
                }
                else
                {
                    return "";
                }

            }
        }
        public string StockNm
        {
            get
            {
                if (this.Stock == EoConstant.TYPE_IRRELEVANT)
                {
                    return EoConstant.TYPE_IRRELEVANT_KorNm;
                }
                else if (this.Stock == EoConstant.TYPE_EXHAUST)
                {
                    return EoConstant.TYPE_EXHAUST_KorNm;
                }
                else if (this.Stock == EoConstant.TYPE_REWORK)
                {
                    return EoConstant.TYPE_REWORK_KorNm;
                }
                else if (this.Stock == EoConstant.TYPE_DISPOSAL)
                {
                    return EoConstant.TYPE_DISPOSAL_KorNm;
                }
                else
                {
                    return "";
                }

            }
        }
        public string CompatibleNm
        {
            get
            {
                if (this.Compatible == EoConstant.TYPE_YES)
                {
                    return EoConstant.TYPE_YES_KorNm;
                }
                else if (this.Compatible == EoConstant.TYPE_NO)
                {
                    return EoConstant.TYPE_NO_KorNm;
                }
                else if (this.Compatible == EoConstant.TYPE_SAME)
                {
                    return EoConstant.TYPE_SAME_KorNm;
                }
                else
                {
                    return "";
                }

            }
        }
        public string IsAttachedNm
        {
            get
            {
                if (this.IsAttached == EoConstant.TYPE_YES)
                {
                    return EoConstant.TYPE_YES_KorNm;
                }
                else if (this.IsAttached == EoConstant.TYPE_NO)
                {
                    return EoConstant.TYPE_NO_KorNm;
                }
                else
                {
                    return "";
                }

            }
        }
        public string ChangesNm
        {
            get
            {
                if (this.Changes != null && this.Changes.Count() > 0)
                {
                    string[] arrReason = this.Changes.IndexOf(',') > -1 ? this.Changes.Split(',') : new string[] { this.Changes };
                    string retrunVal = "";
                    foreach (string arrVal in arrReason)
                    {
                        switch (arrVal)
                        {
                            case "DRW": retrunVal += Common.Constant.EoConstant.TYPE_DRW_KorNm + ", "; break;
                            case "BOM": retrunVal += Common.Constant.EoConstant.TYPE_BOM_KorNm + ", "; break;
                            case "PRODUCE": retrunVal += Common.Constant.EoConstant.TYPE_PRODUCE_KorNm + ", "; break;
                            case "NEW": retrunVal += Common.Constant.EoConstant.TYPE_NEWSPEC_KorNm + ", "; break;
                            case "ADD": retrunVal += Common.Constant.EoConstant.TYPE_ADD_KorNm + ", "; break;

                            default: retrunVal += ""; break;
                        }
                    }
                    return retrunVal.Substring(0, retrunVal.LastIndexOf(", "));
                }
                else
                {
                    return "";
                }
            }
        }

        public string ReasonChangeRequestNm { get; set; }


        public int? RootOID { get; set; }
        public int? ToOID { get; set; }
        public List<HttpPostedFileBase> Files { get; set; }
        public List<HttpFile> delFiles { get; set; }
    }
    public static class ECRRepository
    {
        public static List<ECR> SelChangeRequest(HttpSessionStateBase Context, ECR _param)
        {
            _param.Type = EoConstant.TYPE_CHANGE_REQUEST;
            List<ECR> lECR = DaoFactory.GetList<ECR>("ChangeRequest.SelChangeRequest", _param);
            List<ECR> ViewEPart = new List<ECR>();

            lECR.ForEach(obj =>
            {
                obj.BPolicy = BPolicyRepository.SelBPolicy(new BPolicy { Type = obj.Type, OID = obj.BPolicyOID }).First();
                obj.CreateUsNm = PersonRepository.SelPerson(Context, new Person { OID = obj.CreateUs }).Name;
                obj.BPolicyAuths = BPolicyAuthRepository.MainAuth(Context, obj, null);

                if (obj.ReasonChangeRequest != null)
                {
                    obj.ReasonChangeRequestNm = LibraryRepository.SelLibraryObject(new Library { OID = obj.ReasonChangeRequest }).KorNm;
                }

                if (obj.BPolicyAuths.FindAll(item => item.AuthNm == CommonConstant.AUTH_VIEW).Count > 0)
                {
                    if(_param.Status != null)
                    {
                        if(obj.BPolicy.Name == EoConstant.POLICY_EO_COMPLETED)
                        {
                            ViewEPart.Add(obj);
                        }
                    }
                    else
                    {
                        if (obj.BPolicy.Name != EoConstant.POLICY_EO_COMPLETED)
                        {
                            ViewEPart.Add(obj);
                        }
                    }
                }
            });
            return ViewEPart;
        }
        public static ECR SelChangeRequestObject(HttpSessionStateBase Context, ECR _param)
        {
            _param.Type = EoConstant.TYPE_CHANGE_REQUEST;
            ECR lECR = DaoFactory.GetData<ECR>("ChangeRequest.SelChangeRequest", _param);

            lECR.BPolicy = BPolicyRepository.SelBPolicy(new BPolicy { Type = lECR.Type, OID = lECR.BPolicyOID }).First();
            lECR.CreateUsNm = PersonRepository.SelPerson(Context, new Person { OID = lECR.CreateUs }).Name;
            lECR.BPolicyAuths = BPolicyAuthRepository.MainAuth(Context, lECR, null);

            if (lECR.ReasonChangeRequest != null)
            {
                lECR.ReasonChangeRequestNm = LibraryRepository.SelLibraryObject(new Library { OID = lECR.ReasonChangeRequest }).KorNm;
            }
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
