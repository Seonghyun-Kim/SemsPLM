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

namespace Pms.Models
{
    public class PmsIssue : DObject, IDObject, IObjectFile
    {
        public int? RootOID { get; set; }
        public int? FromOID { get; set; }
        public string ProjectNm { get; set; }
        public string Oem_Lib_Nm { get; set; }
        public string Car_Lib_Nm { get; set; }
        public string TaskNm { get; set; }
        public int? Importance { get; set; }
        public DateTime? EstFinDt { get; set; }
        public int? Manager_OID { get; set; }
        public string ManagerNm { get; set; }
        public string Contents { get; set; }
        public DateTime? FinDt { get; set; }
        public string IsApprovalRequired { get; set; }
        public string IssueType { get; set; }
        public int? TaskOID { get; set; }
        public string IssueTypeNm
        {
            get
            {
                if (this.IssueType != null && this.IssueType.Count() > 0)
                {
                    string[] arrReason = this.IssueType.IndexOf(',') > -1 ? this.IssueType.Split(',') : new string[] { this.IssueType };
                    string retrunVal = "";
                    foreach (string arrVal in arrReason)
                    {
                        switch (arrVal)
                        {
                            case "SPEC": retrunVal += Common.Constant.PmsConstant.ATTRIBUTE_ISSUE_SPECNm + ", "; break;
                            case "4M": retrunVal += Common.Constant.PmsConstant.ATTRIBUTE_ISSUE_4MNm + ", "; break;
                            case "QUALITY": retrunVal += Common.Constant.PmsConstant.ATTRIBUTE_ISSUE_QUALITYNm + ", "; break;
                            case "ETC": retrunVal += Common.Constant.PmsConstant.ATTRIBUTE_ISSUE_ETCNm + ", "; break;


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
        public string[] IssueTypeList
        {
            get
            {
                if (this.IssueType != null && this.IssueType.Count() > 0)
                {
                    string[] arrReason = this.IssueType.IndexOf(',') > -1 ? this.IssueType.Split(',') : new string[] { this.IssueType };

                    return arrReason;
                }
                else
                {
                    return null;
                }
            }
        }
        public string ImportanceNm
        {
            get
            {
                if (this.Importance != null && this.Importance > 0)
                {
                    string returnVal = "";
                    switch (Importance)
                    {
                        case 3: returnVal = "상"; break;
                        case 2: returnVal = "중"; break;
                        case 1: returnVal = "하"; break;
                        default: returnVal = ""; break;
                    }
                    return returnVal;
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
    public static class PmsIssueRepository
    {

        public static int InsPmsIssue(HttpSessionStateBase Context, PmsIssue _param)
        {
            return DaoFactory.SetInsert("Pms.InsPmsIssue", _param);
        }

        public static PmsIssue SelIssue(HttpSessionStateBase Context, PmsIssue _param)
        {
            PmsIssue pmsIssue = DaoFactory.GetData<PmsIssue>("Pms.SelIssue", _param);
            if (pmsIssue != null)
            {
                pmsIssue.BPolicy = BPolicyRepository.SelBPolicy(new BPolicy { Type = pmsIssue.Type, OID = pmsIssue.BPolicyOID }).First();
                pmsIssue.CreateUsNm = PersonRepository.SelPerson(Context, new Person { OID = pmsIssue.CreateUs }).Name;
            }
            return pmsIssue;
        }
        public static PmsIssue UdtIssue(HttpSessionStateBase Context, PmsIssue _param)
        {
            DaoFactory.SetUpdate("Pms.UdtIssue", _param);

            return _param;
        }

        public static List<PmsIssue> SelIssueOIDs(HttpSessionStateBase Context, PmsIssue _param)
        {
            List<PmsIssue> pmsIssue = DaoFactory.GetList<PmsIssue>("Pms.SelIssue", _param);
            if (pmsIssue == null || pmsIssue.Count < 1)
            {
                return new List<PmsIssue>();
            }
            List<int> iPersonOIDs = pmsIssue.Select(data => Convert.ToInt32(data.CreateUs)).ToList();
            List<BPolicy> lBPolicy = BPolicyRepository.SelBPolicyOIDs(new BPolicy { OIDs = pmsIssue.Select(sel => Convert.ToInt32(sel.BPolicyOID)).ToList() });
            List<Person> lPerson = PersonRepository.SelPersons(Context, new Person { OIDs = iPersonOIDs });
            pmsIssue.ForEach(issue =>
            {
                issue.BPolicy = lBPolicy.Find(bpolicy => bpolicy.OID == issue.BPolicyOID);
                issue.CreateUsNm = lPerson.Find(person => person.OID == issue.CreateUs).Name;

            });
            return pmsIssue;
        }
    }
}
