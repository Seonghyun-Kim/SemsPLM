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
    public class ErrorProof : QuickResponseModule, IDObject, IObjectFile
    {
        // 모듈 OID 
        public int? ModuleOID { get; set; }

        // 예정일자 
        public DateTime EstDt { get; set; }

        // 처리일자 
        public DateTime ActDt { get; set; }

        // 점검내용 
        public string CheckDetail { get; set; }

        // 점검당담자 
        public int? CheckUserOID { get; set; }

        public string CheckUserNm { get; set; }

        public List<HttpPostedFileBase> Files { get; set; }

        public List<HttpFile> delFiles { get; set; }

    }

    public static class ErrorProofRepository
    {
        public static ErrorProof SelErrorProof(ErrorProof _param)
        {
            return DaoFactory.GetData<ErrorProof>("Qms.SelErrorProof", _param);
        }

        public static List<ErrorProof> SelErrorProofs(ErrorProof _param)
        {
            return DaoFactory.GetList<ErrorProof>("Qms.SelErrorProof", _param);
        }

        public static int InsErrorProof(ErrorProof _param)
        {
            _param.CreateUs = 1;
            return DaoFactory.SetInsert("Qms.InsErrorProof", _param);
        }

        public static int UdtErrorProof(ErrorProof _param)
        {
            _param.ModifyUs = 1;
            return DaoFactory.SetUpdate("Qms.UdtErrorProof", _param);
        }

    }
}
