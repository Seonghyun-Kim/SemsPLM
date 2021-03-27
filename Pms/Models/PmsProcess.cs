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

        public string RootNm { get; set; }

        public string RootOEM { get; set; }

        public string RootCarType { get; set; }

        public string RootItem { get; set; }

        public int? Delay { get; set; }

        public string ApprovStatus { get; set; }
        public int? ITEM_No { get; set; }
        public int? Oem_Lib_OID { get; set; }
        public int? Car_Lib_OID { get; set; }
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
            if (pmsProcess != null)
            {
                pmsProcess.BPolicy = BPolicyRepository.SelBPolicy(new BPolicy { Type = pmsProcess.ProcessType, OID = pmsProcess.BPolicyOID }).First();
                pmsProcess.BPolicyAuths = BPolicyAuthRepository.MainAuth(Context, pmsProcess, PmsAuth.RoleAuth(Context, pmsProcess));
            }
            return pmsProcess;
        }

        public static List<PmsProcess> SelPmsProcessOIDs(HttpSessionStateBase Context, PmsProcess _param)
        {
            List<PmsProcess> pmsProcesses = DaoFactory.GetList<PmsProcess>("Pms.SelPmsProcess", _param);
            if (pmsProcesses == null || pmsProcesses.Count < 1)
            {
                return new List<PmsProcess>();
            }
            List<BPolicy> procBPolicies = BPolicyRepository.SelBPolicyOIDs(new BPolicy { OIDs = pmsProcesses.Select(sel => Convert.ToInt32(sel.BPolicyOID)).ToList() });
            pmsProcesses.ForEach(proc =>
            {
                //proc.BPolicy = BPolicyRepository.SelBPolicy(new BPolicy { Type = proc.ProcessType, OID = proc.BPolicyOID }).First();
                proc.BPolicy = procBPolicies.Find(data => data.OID == proc.BPolicyOID);
                proc.BPolicyAuths = BPolicyAuthRepository.MainAuth(Context, proc, PmsAuth.RoleAuth(Context, proc));
            });
            return pmsProcesses;
        }

    }
}
