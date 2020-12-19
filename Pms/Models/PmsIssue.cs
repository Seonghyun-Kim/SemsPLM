﻿using Common.Factory;
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
        public string TaskNm { get; set; }
        public int? Importance { get; set; }
        public DateTime? EstFinDt { get; set; }
        public int? Manager_OID { get; set; }
        public string ManagerNm { get; set; }
        public string Contents { get; set; }
        public string IssueType { get; set; }

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
            pmsIssue.BPolicy = BPolicyRepository.SelBPolicy(new BPolicy { Type = pmsIssue.Type, OID = pmsIssue.BPolicyOID }).First();
            pmsIssue.CreateUsNm = PersonRepository.SelPerson(Context, new Person { OID = pmsIssue.CreateUs }).Name;
            return pmsIssue;
        }
        public static PmsIssue UdtIssue(HttpSessionStateBase Context, PmsIssue _param)
        {
            DaoFactory.SetUpdate("Pms.UdtIssue", _param);

            return _param;
        }
    }
}
