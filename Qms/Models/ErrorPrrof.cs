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
    public class ErrorPrrof : QuickResponseModule, IDObject
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
        public int CheckUserOID { get; set; }

    }

    public static class ErrorPrrofRepository
    {
        public static ErrorPrrof SelErrorPrrof(ErrorPrrof _param)
        {
            return DaoFactory.GetData<ErrorPrrof>("Qms.SelErrorPrrof", _param);
        }

        public static List<ErrorPrrof> SelErrorPrrofs(ErrorPrrof _param)
        {
            return DaoFactory.GetList<ErrorPrrof>("Qms.SelErrorPrrof", _param);
        }

        public static int InsErrorPrrof(ErrorPrrof _param)
        {
            _param.CreateUs = 1;
            return DaoFactory.SetInsert("Qms.InsErrorPrrof", _param);
        }

        public static int UdtErrorPrrof(ErrorPrrof _param)
        {
            _param.ModifyUs = 1;
            return DaoFactory.SetUpdate("Qms.UdtErrorPrrof", _param);
        }

    }
}
