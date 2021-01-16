using Common.Constant;
using Common.Models;
using Common.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Common.Trigger
{
    public class ApprovalTrigger
    {
        public string ActionTemplatePromote(object[] args)
        {
            object[] oArgs = args;
            HttpSessionStateBase Context = (HttpSessionStateBase)oArgs[0];
            int iAdmin = Convert.ToInt32(Context["UserOID"]);
            string type = Convert.ToString(oArgs[1]);
            string status = Convert.ToString(oArgs[2]);
            string oid = Convert.ToString(oArgs[3]);
            try
            {
                DObjectRepository.UdtDObject(Context, new DObject { OID = Convert.ToInt32(oid), Description = "" });
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            return "";
        }

        public string ActionApprvalTaskStartedMailPromote(object[] args)
        {
            object[] oArgs = args;
            HttpSessionStateBase Context = (HttpSessionStateBase)oArgs[0];
            int iAdmin = Convert.ToInt32(Context["UserOID"]);
            string type = Convert.ToString(oArgs[1]);
            string status = Convert.ToString(oArgs[2]);
            string oid = Convert.ToString(oArgs[3]);
            try
            {
                List<ApprovalTask> lApprovalTask = ApprovalTaskRepository.SelInboxTasks(Context, new ApprovalTask { Type = CommonConstant.TYPE_APPROVAL_TASK, OID = Convert.ToInt32(oid) });
                SemsSmtp smtp = new SemsSmtp();

                lApprovalTask.ForEach(apprvTask =>
                {
                    ApprovalMail apprvMail = new ApprovalMail(Context, apprvTask);
                    smtp.SetMailInfo(apprvMail);
                });
                smtp.SendMail();
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            return "";
        }

    }
}
