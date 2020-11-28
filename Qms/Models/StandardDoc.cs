using Common.Factory;
using Common.Interface;
using Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qms.Models
{
    public class StandardDoc : DObject, IDObject
    {
        // 신속대응 OID
        public int? QuickOID { get; set; }
        public int? ModuleOID { get; set; }

        // 문서타입 
        public int DocType { get; set; }

        // 문서 OID 
        public int DocOID { get; set; }

        // 반영내용 
        public string DocSummary { get; set; }

        public string DocCompleteDt { get; set; }

        public List<StandardDoc> StandardFollowUpList { get; set; }
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
            _param.CreateUs = 1;
            return DaoFactory.SetInsert("Qms.InsStandardDoc", _param);
        }

        public static int UdtStandardDoc(StandardDoc _param)
        {
            _param.ModifyUs = 1;
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
            _param.CreateUs = 1;
            return DaoFactory.SetInsert("Qms.InsHorizontalDeployment", _param);
        }

        public static int UdtHorizontalDeployment(HorizontalDeployment _param)
        {
            _param.ModifyUs = 1;
            return DaoFactory.SetUpdate("Qms.UdtHorizontalDeployment", _param);
        }
    }
}
