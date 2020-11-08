using Common.Constant;
using Common.Factory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Models
{
    public class BPolicy
    {
        public int? OID { get; set; }

        public string Type { get; set; }

        public string Name { get; set; }

        public int? StatusOID { get; set; }

        public string StatusNm { get; set; }

        public string CheckProgram { get; set; }

        public string ActionProgram { get; set; }

        public int? StatusOrd { get; set; }

        public string IsRevision { get; set; }

        public string BeforeActionOID { get; set; }

        public string NextActionOID { get; set; }
    }

    public static class BPolicyRepository
    {
        public static List<BPolicy> SelBPolicy(BPolicy _param)
        {
            return DaoFactory.GetList<BPolicy>("Comm.SelBPolicy", _param).OrderBy(d => d.StatusOID).ToList();
        } 

        public static List<Dictionary<string, string>> SelCheckProgram(BPolicy _param)
        {
            BPolicy policy = DaoFactory.GetData<BPolicy>("Comm.SelBPolicy", _param);
            if (policy.CheckProgram == null || policy.CheckProgram.Length < 1)
            {
                return null;
            }

            List<Dictionary<string, string>> lDCheckProgram = new List<Dictionary<string, string>>();
            policy.CheckProgram.Split(',').ToList().ForEach(prog =>
            {
                Dictionary<string, string> tmp = new Dictionary<string, string>();
                string[] splitProg = prog.Split(':');
                tmp.Add(CommonConstant.POLICY_TRIGGER_CLASS, splitProg[0]);
                tmp.Add(CommonConstant.POLICY_TRIGGER_FUNCTION, splitProg[1]);
                lDCheckProgram.Add(tmp);
            });
            return lDCheckProgram;
        }

        public static List<Dictionary<string, string>> SelActionProgram(BPolicy _param) 
        {
            BPolicy policy = DaoFactory.GetData<BPolicy>("Comm.SelBPolicy", _param);
            if (policy.ActionProgram == null || policy.ActionProgram.Length < 1)
            {
                return null;
            }

            List<Dictionary<string, string>> lDActionProgram = new List<Dictionary<string, string>>();
            policy.ActionProgram.Split(',').ToList().ForEach(prog =>
            {
                Dictionary<string, string> tmp = new Dictionary<string, string>();
                string[] splitProg = prog.Split(':');
                tmp.Add(CommonConstant.POLICY_TRIGGER_CLASS, splitProg[0]);
                tmp.Add(CommonConstant.POLICY_TRIGGER_FUNCTION, splitProg[1]);
                lDActionProgram.Add(tmp);
            });
            return lDActionProgram;
        }
    }
}
