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
    public class ApprovalTask : DObject, IDObject
    {
        public int? ApprovalOID { get; set; }

        public int? StepOID { get; set; }

        public int?  PersonOID { get; set; }

        public Person PersonObj { get; set; }

        public string ApprovalType { get; set; }

        public string Comment { get; set; }

        public DateTime? ApprovalDt { get; set; }

        public string ActionType { get; set; }


        public int? Ord { get; set; }

        public string DepartmentNm { get; set; }

        public string PersonNm { get; set; }

        public int? CurrentNum { get; set; }

        public int? DocOID { get; set; }

        public string DocType { get; set; }

        public string DocNm { get; set; }

        public int? DocCreateUs { get; set; }

        public string DocCreateNm { get; set; }

        public BPolicy ApprovalBPolicy { get; set; }

    }

    public static class ApprovalTaskRepository
    {
        public static List<ApprovalTask> SelInboxTasks(HttpSessionStateBase Context, ApprovalTask _param)
        {
            List<ApprovalTask> lApprovalTasks = DaoFactory.GetList<ApprovalTask>("Comm.SelApprovalTask", _param);
            lApprovalTasks.ForEach(task =>
            {
                task.BPolicy = BPolicyRepository.SelBPolicy(new BPolicy { OID = task.BPolicyOID }).First();
                task.PersonObj = PersonRepository.SelPerson(Context, new Person { OID = task.PersonOID });
                task.PersonNm = task.PersonObj.Name;
                task.DepartmentNm = task.PersonObj.DepartmentNm;
            });
            return lApprovalTasks; ;
        }

        public static int InsInboxTask(ApprovalTask _param)
        {
            return DaoFactory.SetInsert("Comm.InsApprovalTask", _param);
        }

        public static int UdtInboxTask(ApprovalTask _param)
        {
            return DaoFactory.SetUpdate("Comm.UdtInboxTask", _param);
        }
    }
}
