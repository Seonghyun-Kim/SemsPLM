using Common.Constant;
using Common.Models;
using Pms.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Pms.Trigger
{
    public class IssueTrigger
    {
        public string CheckIssuePromote(object[] args)
        {
            object[] oArgs = args;
            HttpSessionStateBase Context = (HttpSessionStateBase)oArgs[0];
            int iUser = Convert.ToInt32(Context["UserOID"]);
            string type = Convert.ToString(oArgs[1]);
            string status = Convert.ToString(oArgs[2]);
            string oid = Convert.ToString(oArgs[3]);
            try
            {
                PmsIssue issue = PmsIssueRepository.SelIssue(Context, new PmsIssue { OID = Convert.ToInt32(oid) });
                if (issue.BPolicy.Name == PmsConstant.POLICY_ISSUE_COMPLETED)
                {
                    issue.FinDt = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                    PmsIssueRepository.UdtIssue(Context, issue);
                }

            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            return "";
        }
    }

}
