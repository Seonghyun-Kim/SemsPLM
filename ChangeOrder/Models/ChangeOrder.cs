using Common.Constant;
using Common.Factory;
using Common.Interface;
using Common.Models;
using Common.Models.File;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace ChangeOrder.Models
{
    public class ECO : DObject, IDObject, IObjectFile
    {
        public string ItemGroup { get; set; }
        public string Title { get; set; }
        public DateTime? DesignChangeDt { get; set; }
        public int ReasonChange { get; set; }
        public string ReasonChangeNm { get; set; }
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
        public string CarType { get; set; }
        public int? RootOID { get; set; }
        public int? ToOID { get; set; }
        public string DevMP { get; set; }
        public DateTime? StartDesignChangeDt { get; set; }
        public DateTime? EndDesignChangeDt { get; set; }
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

        public List<HttpPostedFileBase> Files { get; set; }

        public List<HttpFile> delFiles { get; set; }


    }

    public static class ECORepository
    {
        public static List<ECO> SelChangeOrder(HttpSessionStateBase Context, ECO _param)
        {
            _param.Type = EoConstant.TYPE_CHANGE_ORDER;
            List<ECO> lECOs = new List<ECO>();
            List<ECO> lECO = DaoFactory.GetList<ECO>("ChangeOrder.SelChangeOrder", _param);
            lECO.ForEach(obj =>
            {
                obj.ReasonChangeNm = LibraryRepository.SelLibraryObject(new Library { OID = obj.ReasonChange }).KorNm;
                obj.CreateUsNm = PersonRepository.SelPerson(Context, new Person { OID = obj.CreateUs }).Name;
                obj.BPolicy = BPolicyRepository.SelBPolicy(new BPolicy { Type = obj.Type, OID = obj.BPolicyOID }).First();
                obj.BPolicyAuths = BPolicyAuthRepository.MainAuth(Context, obj,null);
                if (obj.BPolicyAuths.FindAll(item => item.AuthNm == CommonConstant.AUTH_VIEW).Count > 0)
                {
                    lECOs.Add(obj);
                }
            });
            return lECOs;
        }

        public static ECO SelChangeOrderObject(HttpSessionStateBase Context, ECO _param)
        {
            _param.Type = EoConstant.TYPE_CHANGE_ORDER;
            ECO lECO = DaoFactory.GetData<ECO>("ChangeOrder.SelChangeOrder", _param);

            lECO.ReasonChangeNm = LibraryRepository.SelLibraryObject(new Library { OID = lECO.ReasonChange }).KorNm;
            lECO.CreateUsNm = PersonRepository.SelPerson(Context, new Person { OID = lECO.CreateUs }).Name;
            lECO.BPolicy = BPolicyRepository.SelBPolicy(new BPolicy { Type = lECO.Type, OID = lECO.BPolicyOID }).First();
            lECO.BPolicyAuths = BPolicyAuthRepository.MainAuth(Context, lECO, null);
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
