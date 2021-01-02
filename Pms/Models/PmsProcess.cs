using Common.Constant;
using Common.Factory;
using Common.Interface;
using Common.Models;
using Pms.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Pms.Models
{
    public class PmsProcess : DObject, IDObject
    {
        public string ProcessType { get; set; }

        public int? Id { get; set; }

        public string Dependency { get; set; }

        public DateTime? EstStartDt { get; set; }

        public DateTime? EstEndDt { get; set; }

        public int? EstDuration { get; set; }

        public DateTime? ActStartDt { get; set; }

        public DateTime? ActEndDt { get; set; }

        public int? ActDuration { get; set; }

        public int? Level { get; set; }

        public int? Complete { get; set; }

        public string No { get; set; }

        //System
        public int? RootOID { get; set; }

        public string ApprovStatus { get; set; }
    }

    public static class PmsProcessRepository
    {

        public static int InsPmsProcess(HttpSessionStateBase Context, PmsProcess _param)
        {
            return DaoFactory.SetInsert("Pms.InsPmsProcess", _param);
        }

        public static int UdtPmsProcess(HttpSessionStateBase Context, PmsProcess _param)
        {
            return DaoFactory.SetUpdate("Pms.UdtPmsProcess", _param);
        }

        public static PmsProcess SelPmsProcess(HttpSessionStateBase Context, PmsProcess _param)
        {
            _param.Type = _param.ProcessType;
            PmsProcess pmsProcess = DaoFactory.GetData<PmsProcess>("Pms.SelPmsProcess", _param);
            pmsProcess.BPolicy = BPolicyRepository.SelBPolicy(new BPolicy { Type = pmsProcess.ProcessType, OID = pmsProcess.BPolicyOID }).First();
            pmsProcess.BPolicyAuths = BPolicyAuthRepository.MainAuth(Context, pmsProcess, PmsAuth.RoleAuth(Context, pmsProcess));
            return pmsProcess;
        }

    }
}
