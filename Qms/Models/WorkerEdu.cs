using Common.Factory;
using Common.Interface;
using Common.Models.File;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Qms.Models
{
    public class WorkerEdu : QuickResponseModule, IDObject, IObjectFile
    {
        public int? ModuleOID { get; set; }
        // 교육내용 
        public string EduDetail { get; set; }

        // 교육계획
        public string EduPlan { get; set; }

        // 교육일자 
        public DateTime? EduDt { get; set; }

        // 강사(담당자) 
        public int? EduUserOID { get; set; }

        public string EduUserNm { get; set; }

        #region -- Search & View

        public string EduSDt { get; set; }
        public string EduEDt { get; set; }

        #endregion

        #region -- File
        public List<HttpPostedFileBase> Files { get; set; }

        public List<HttpFile> delFiles { get; set; }

        #endregion
    }

    public static class WorkerEduRepository
    {
        public static WorkerEdu SelWorkerEdu(WorkerEdu _param)
        {
            return DaoFactory.GetData<WorkerEdu>("Qms.SelWorkerEdu", _param);
        }

        public static List<WorkerEdu> SelWorkerEdus(WorkerEdu _param)
        {
            return DaoFactory.GetList<WorkerEdu>("Qms.SelWorkerEdu", _param);
        }

        public static int InsWorkerEdu(WorkerEdu _param)
        {
            return DaoFactory.SetInsert("Qms.InsWorkerEdu", _param);
        }

        public static int UdtWorkerEdu(WorkerEdu _param)
        {
            return DaoFactory.SetUpdate("Qms.UdtWorkerEdu", _param);
        }
    }
}
