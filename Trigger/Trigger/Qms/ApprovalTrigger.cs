using Common.Models;
using Qms.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Qms.Trigger
{
    public class ApprovalTrigger
    {
        public string ActionNextModulePromote(object[] args)
        {
            object[] oArgs = args;
            HttpSessionStateBase Context = (HttpSessionStateBase)oArgs[0];
            int iAdmin = Convert.ToInt32(Context["UserOID"]);
            string type = Convert.ToString(oArgs[1]);
            string status = Convert.ToString(oArgs[2]);
            string oid = Convert.ToString(oArgs[3]);
            try
            {
                QuickResponseModule quickResponse = QuickResponseModuleRepository.SelQuickResponseModule(new QuickResponseModule() { OID = Convert.ToInt32(oid) });

                List<QuickResponseModule> quickResponseModules = QuickResponseModuleRepository.SelQuickResponseModules(new QuickResponseModule() { QuickOID = quickResponse.QuickOID, ModuleFl = 1 });

                QuickResponseModule nextModule = quickResponseModules.SkipWhile(v => v.OID != quickResponse.OID).Skip(1).First();

                if(nextModule == null)
                {

                }
                else
                {
                    List<BPolicy> nextModluePolicies = BPolicyRepository.SelBPolicy(new BPolicy() { Type = nextModule.ModuleType, Name = "Started" });

                    nextModluePolicies.ForEach(v =>
                    {
                        DObjectRepository.UdtDObject(Context, new DObject() { OID = nextModule.OID, BPolicyOID = v.OID });
                    });

                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            return "";
        }

    }
}
