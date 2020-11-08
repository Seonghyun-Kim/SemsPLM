using Common.Factory;
using Common.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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


        public int? Ord { get; set; }

        public string DepartmentNm { get; set; }

        public string PersonNm { get; set; }

        public int? CurrentNum { get; set; }
    }

    public static class ApprovalTaskRepository
    {
        public static List<ApprovalTask> SelInboxTasks(ApprovalTask _param)
        {
            List<ApprovalTask> lApprovalTasks = DaoFactory.GetList<ApprovalTask>("Comm.SelApprovalTask", _param);
            lApprovalTasks.ForEach(task =>
            {
                task.PersonObj = PersonRepository.SelPerson(new Person { OID = task.PersonOID });
                task.PersonNm = task.PersonObj.Name;
                task.DepartmentNm = task.PersonObj.DepartmentNm;
            });
            return lApprovalTasks; ;
        }

        public static int InsInboxTask(ApprovalTask _param)
        {
            return DaoFactory.SetInsert("Comm.InsApprovalTask", _param);

        }
    }
}
