using Common.Constant;
using Common.Factory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pms.Models
{
    public class PmsBaseLineProcess : PmsProcess
    {
        public int? TargetPrcessOID { get; set; }

        public int? RootBaseLineOID { get; set; }

        public int? ProcessOID { get; set; }

        public string ProcessNm { get; set; }
    }

    public static class PmsBaseLineProcessRepository
    {
        public static int InsPmsBaseLineProcess(PmsBaseLineProcess _param)
        {
            return DaoFactory.SetInsert("Pms.InsPmsBaseLineProcess", _param);
        }

        public static PmsBaseLineProcess SelPmsBaseLIneProcess(PmsBaseLineProcess _param)
        {

            PmsBaseLineProcess pmsBaseLineProcess = DaoFactory.GetData<PmsBaseLineProcess>("Pms.SelPmsBaseLineProcess", _param);
            return pmsBaseLineProcess;
        }
    }
}
