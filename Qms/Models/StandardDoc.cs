using Common.Factory;
using Common.Interface;
using Common.Models;
using Common.Models.File;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Qms.Models
{
    public class StandardDoc : DObject, IDObject, IObjectFile
    {
        // 신속대응 OID
        public int? QuickOID { get; set; }
        public int? ModuleOID { get; set; }

        // 문서타입 
        public int? DocType { get; set; }

        // 문서 OID 
        public int? DocOID { get; set; }

        // 반영내용 
        public string DocSummary { get; set; }

        public DateTime? DocCompleteDt { get; set; }

        public int? DocFl { get; set; }

        public List<StandardDoc> StandardFollowUpList { get; set; }

        #region -- Search & View
        public string DocClassNm { get; set; }
        public string DocNm { get; set; }

        public string DocFileNm { get; set; }
        #endregion

        #region -- File
        public List<HttpPostedFileBase> Files { get; set; }

        public List<HttpFile> delFiles { get; set; }

        #endregion
    }

    public static class StandardDocRepository
    {
        public static StandardDoc SelStandardDoc(StandardDoc _param)
        {
            return DaoFactory.GetData<StandardDoc>("Qms.SelStandardDoc", _param);
        }

        public static List<StandardDoc> SelStandardDocs(StandardDoc _param)
        {
            return DaoFactory.GetList<StandardDoc>("Qms.SelStandardDoc", _param);
        }

        public static int InsStandardDoc(StandardDoc _param)
        {
            return DaoFactory.SetInsert("Qms.InsStandardDoc", _param);
        }

        public static int UdtStandardDoc(StandardDoc _param)
        {
            return DaoFactory.SetUpdate("Qms.UdtStandardDoc", _param);
        }
    }

    public class HorizontalDeployment : QuickResponseModule, IDObject
    {
        // 수평전개 플래그 
        public bool HorizontalFl { get; set; }

        // 수평전개사항 
        public string Horizontal { get; set; }

        // 수평전개사항 코멘트 
        public string HorizontalComent { get; set; }
    }

    public static class HorizontalDeploymentRepository
    {
        public static HorizontalDeployment SelHorizontalDeployment(HorizontalDeployment _param)
        {
            return DaoFactory.GetData<HorizontalDeployment>("Qms.SelHorizontalDeployment", _param);
        }

        public static List<HorizontalDeployment> SelHorizontalDeployments(HorizontalDeployment _param)
        {
            return DaoFactory.GetList<HorizontalDeployment>("Qms.SelHorizontalDeployment", _param);
        }

        public static int InsHorizontalDeployment(HorizontalDeployment _param)
        {
            return DaoFactory.SetInsert("Qms.InsHorizontalDeployment", _param);
        }

        public static int UdtHorizontalDeployment(HorizontalDeployment _param)
        {
            return DaoFactory.SetUpdate("Qms.UdtHorizontalDeployment", _param);
        }
    }
}
