using Common.Constant;
using Common.Factory;
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
            string action = Convert.ToString(oArgs[5]);
            try
            {
                QuickResponseModule quickResponse = QuickResponseModuleRepository.SelQuickResponseModule(new QuickResponseModule() { OID = Convert.ToInt32(oid) });

                List<QuickResponseModule> quickResponseModules = QuickResponseModuleRepository.SelQuickResponseModules(new QuickResponseModule() { QuickOID = quickResponse.QuickOID, ModuleFl = 1 });

                var NextModules = quickResponseModules.SkipWhile(v => v.OID != quickResponse.OID).Skip(1);

                if (action == CommonConstant.ACTION_PROMOTE)
                {
                    if (NextModules.Count() == 0)
                    {
                        DRelationship dRelModule = new DRelationship();
                        dRelModule.Type = QmsConstant.RELATIONSHIP_QUICK_MODULE;
                        dRelModule.ToOID = Convert.ToInt32(oid);

                        // 모든 항목이 끝났을 경우
                        DRelationship dRelQuickResponse = DaoFactory.GetData<DRelationship>("Comm.SelDRelationship", dRelModule);

                        QuickResponse quick = QuickResponseRepository.SelQuickResponse(new QuickResponse() { OID = dRelQuickResponse.FromOID });

                        quick.BPolicyOID = 55; //완료로 변경
                        DObjectRepository.UdtDObject(Context, quick);

                        QuickResponseRepository.UdtQuickResponse(new QuickResponse() { OID = quick.OID, FinishDt = DateTime.Now });
                    }
                    else
                    {
                        // 다음 항목이 있을 경우
                        QuickResponseModule nextModule = NextModules.First();

                        List<BPolicy> nextModluePolicies = BPolicyRepository.SelBPolicy(new BPolicy() { Type = nextModule.ModuleType, Name = "Started" });

                        nextModluePolicies.ForEach(v =>
                        {
                            DObjectRepository.UdtDObject(Context, new DObject() { OID = nextModule.OID, BPolicyOID = v.OID });
                        });

                        if (quickResponse.ModuleType == QmsConstant.TYPE_LPA_MEASURE)
                        {
                            QuickResponseModule LpaUnfitModule = QuickResponseModuleRepository.SelQuickResponseModule(new QuickResponseModule() { QuickOID = quickResponse.QuickOID, ModuleFl = 1, ModuleType = QmsConstant.TYPE_LPA_UNFIT });

                            List<BPolicy> LpaUnfitePolicies = BPolicyRepository.SelBPolicy(new BPolicy() { Type = QmsConstant.TYPE_LPA_UNFIT, Name = "Completed" });

                            DObjectRepository.UdtDObject(Context, new DObject() { OID = LpaUnfitModule.OID, BPolicyOID = LpaUnfitePolicies[0].OID });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            return "";
        }

        public string ActionModuleReview(object[] args)
        {
            object[] oArgs = args;
            HttpSessionStateBase Context = (HttpSessionStateBase)oArgs[0];
            int iAdmin = Convert.ToInt32(Context["UserOID"]);
            string type = Convert.ToString(oArgs[1]);
            string status = Convert.ToString(oArgs[2]);
            string oid = Convert.ToString(oArgs[3]);
            string action = Convert.ToString(oArgs[5]);
            try
            {
                if (action == CommonConstant.ACTION_PROMOTE)
                {
                    BPolicy reviewSt = BPolicyRepository.SelBPolicy(new BPolicy { Type = type, Name = QmsConstant.POLICY_QMS_MODULE_REVIEW }).First();
                    DObjectRepository.UdtDObject(Context, new DObject() { OID = Convert.ToInt32(oid), BPolicyOID = reviewSt.OID });
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
