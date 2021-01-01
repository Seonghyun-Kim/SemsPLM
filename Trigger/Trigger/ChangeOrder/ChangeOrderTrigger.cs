using ChangeOrder.Models;
using Common.Constant;
using Common.Models;
using EBom.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Trigger;

namespace ChangeOrder.Trigger
{
   public class ChangeOrderTrigger
    {
        public string ActionChangeOrderPromote(object[] args)
        {
            object[] oArgs = args;
            HttpSessionStateBase Context = (HttpSessionStateBase)oArgs[0];
            string type = Convert.ToString(oArgs[1]);
            string status = Convert.ToString(oArgs[2]);
            string oid = Convert.ToString(oArgs[3]);
            string PromoteType = Convert.ToString(oArgs[5]);
            try
            {
                List<EO> lEO = EORepository.SelEOContentsOID(new EO { RootOID = Convert.ToInt32(oid), Type = EoConstant.TYPE_EBOM_LIST }); //결제올린 설계변경의 ebomlist검색
                if (lEO != null && lEO.Count > 0)
                {
                    EPart eobj = null;
                    lEO.ForEach(obj =>
                    {
                        if (eobj != null)
                        {
                            eobj = null;
                        }
                        eobj = EPartRepository.SelEPartObject(Context, new EPart { OID = obj.ToOID });
                        TriggerUtil.StatusPromote(Context, false, EBomConstant.TYPE_PART, Convert.ToString(eobj.BPolicyOID), Convert.ToInt32(eobj.OID), Convert.ToInt32(eobj.OID), PromoteType, null);

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
