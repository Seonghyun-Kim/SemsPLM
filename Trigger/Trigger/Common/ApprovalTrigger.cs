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

        public string ActionApprovalCompletedMailPromote(object[] args)
        {
            object[] oArgs = args;
            HttpSessionStateBase Context = (HttpSessionStateBase)oArgs[0];
            int iAdmin = Convert.ToInt32(Context["UserOID"]);
            string type = Convert.ToString(oArgs[1]);
            string status = Convert.ToString(oArgs[2]);
            string oid = Convert.ToString(oArgs[3]);
            string action = Convert.ToString(oArgs[5]);
            try
            {
                if (action.Equals(CommonConstant.ACTION_PROMOTE))
                {
                    Approval approv = ApprovalRepository.SelApprovalNonStep(Context, new Approval { OID = Convert.ToInt32(oid) });
                    List<ApprovalTask> lApprovalTask = ApprovalTaskRepository.SelInboxTasks(Context, new ApprovalTask { Type = CommonConstant.TYPE_APPROVAL_TASK, ApprovalOID = Convert.ToInt32(oid) });
                    SemsSmtp smtp = new SemsSmtp();
                    lApprovalTask.ForEach(apprvTask =>
                    {
                        ApprovalCompleteMail apprvMail = new ApprovalCompleteMail(Context, approv, apprvTask.PersonObj);
                        smtp.SetMailInfo(apprvMail);
                    });
                    smtp.SendMail();
                }
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
            string action = Convert.ToString(oArgs[5]);
            try
            {
                List<ApprovalTask> lApprovalTask = ApprovalTaskRepository.SelInboxTasks(Context, new ApprovalTask { Type = CommonConstant.TYPE_APPROVAL_TASK, OID = Convert.ToInt32(oid) });
                SemsSmtp smtp = new SemsSmtp();

                lApprovalTask.ForEach(apprvTask =>
                {
                    ApprovalTaskMail apprvMail = new ApprovalTaskMail(Context, apprvTask);
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

        public string ActionApprovalTaskCompletedMailPromote(object[] args)
        {
            object[] oArgs = args;
            HttpSessionStateBase Context = (HttpSessionStateBase)oArgs[0];
            int iAdmin = Convert.ToInt32(Context["UserOID"]);
            string type = Convert.ToString(oArgs[1]);
            string status = Convert.ToString(oArgs[2]);
            string oid = Convert.ToString(oArgs[3]);
            string action = Convert.ToString(oArgs[5]);
            try
            {
                List<ApprovalTask> lApprovalTask = ApprovalTaskRepository.SelInboxTasks(Context, new ApprovalTask { Type = CommonConstant.TYPE_APPROVAL_TASK, OID = Convert.ToInt32(oid) });
                SemsSmtp smtp = new SemsSmtp();

                if (action.Equals(CommonConstant.ACTION_PROMOTE))
                {

                }
                else if (action.Equals(CommonConstant.ACTION_REJECT))
                {
                    ApprovalTaskRejectMail apprvMail = new ApprovalTaskRejectMail(Context, lApprovalTask.First());
                    smtp.SetMailInfo(apprvMail);
                }
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
