using Common.Factory;
using Common.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Common.Models
{
    public class ApprovalStep : DObject, IDObject
    {
        public int? ApprovalOID { get; set; }

        public int? Ord { get; set; }

        public string ApprovalType { get; set; }

        public List<ApprovalTask> InboxTask { get; set; }

    }

    public static class ApprovalStepRepository
    {
        public static ApprovalStep SelApprovalStep(HttpSessionStateBase Context, ApprovalStep _param)
        {
            ApprovalStep tmpApprovalStep = DaoFactory.GetData<ApprovalStep>("Comm.SelApprovalStep", _param);
            tmpApprovalStep.InboxTask = ApprovalTaskRepository.SelInboxTasks(Context, new ApprovalTask { StepOID = tmpApprovalStep.OID });
            return tmpApprovalStep;
        }

        public static List<ApprovalStep> SelApprovalSteps(HttpSessionStateBase Context, ApprovalStep _param)
        {
            List<ApprovalStep> tmpApprovalSteps = DaoFactory.GetList<ApprovalStep>("Comm.SelApprovalStep", _param);
            tmpApprovalSteps.ForEach(step =>
            {
                step.InboxTask = ApprovalTaskRepository.SelInboxTasks(Context, new ApprovalTask { StepOID = step.OID });
            });
            return tmpApprovalSteps;
        }

        public static int InsApprovalStep(HttpSessionStateBase Context, ApprovalStep _param)
        {
            return DaoFactory.SetInsert("Comm.InsApprovalStep", _param);
        }

    }
}
