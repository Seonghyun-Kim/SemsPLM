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
    public class ApprovalComment : DObject, IDObject
    {
        public int? ApprovalOID { get; set; }

        public string Comment { get; set; }

        public int? Ord { get; set; }

    }

    public static class ApprovalCommentRepository
    {
        public static List<ApprovalComment> SelApprovalComment(HttpSessionStateBase Context, ApprovalComment _param)
        {
            List<ApprovalComment> lTmpApprovalComment = DaoFactory.GetList<ApprovalComment>("Comm.SelApprovalComment", _param);
            lTmpApprovalComment.ForEach(approvComment =>
            {
                approvComment.CreateUsNm = PersonRepository.SelPerson(Context, new Person { OID = approvComment.CreateUs }).Name;
            });
            return lTmpApprovalComment;
        }

        public static int InsApprovalComment(ApprovalComment _param)
        {
            return DaoFactory.SetInsert("Comm.InsApprovalComment", _param);
        }
    }
}
