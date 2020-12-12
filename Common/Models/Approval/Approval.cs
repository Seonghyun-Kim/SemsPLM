using Common.Factory;
using Common.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Models
{
    public class Approval : DObject, IDObject
    {
        public int? TargetOID { get; set; }

        public int? ApprovalCount { get; set; }

        public string Comment { get; set; }

        public int? CurrentNum { get; set; }

        public List<ApprovalStep> InboxStep { get; set; }

        public List<ApprovalComment> InboxCommnet { get; set; }
    }

    public static class ApprovalRepository
    {
        public static Approval SelApprovalNonStep(Approval _param)
        {
            Approval tmpApproval = DaoFactory.GetData<Approval>("Comm.SelApproval", _param);
            return tmpApproval;
        }

        public static Approval SelApproval(Approval _param)
        {
            Approval tmpApproval = ApprovalRepository.SelApprovalNonStep(_param);
            if (tmpApproval != null)
            {
                tmpApproval.InboxStep = ApprovalStepRepository.SelApprovalSteps(new ApprovalStep { ApprovalOID = tmpApproval.OID });
                tmpApproval.InboxCommnet = ApprovalCommentRepository.SelApprovalComment(new ApprovalComment { ApprovalOID = tmpApproval.OID });
            }
            return tmpApproval;
        }

        public static List<Approval> SelApprovals(Approval _param)
        {
            List<Approval> tmpApprovals = DaoFactory.GetList<Approval>("Comm.SelApproval", _param);
            tmpApprovals.ForEach(app =>
            {
                app.InboxStep = ApprovalStepRepository.SelApprovalSteps(new ApprovalStep { ApprovalOID = app.OID });
                app.InboxCommnet = ApprovalCommentRepository.SelApprovalComment(new ApprovalComment { ApprovalOID = app.OID });
            });
            return tmpApprovals;
        }

        public static int InsApproval(Approval _param)
        {
            return DaoFactory.SetInsert("Comm.InsApproval", _param);
        }

        public static int UdtApproval(Approval _param)
        {
            return DaoFactory.SetUpdate("Comm.UdtApproval", _param);
        }
    }
}
