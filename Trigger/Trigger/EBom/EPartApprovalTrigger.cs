using Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EBom.Models;
using System.Web;

namespace EBom.Trigger
{
    public class EPartApprovalTrigger
    {
        public string ChildApproval(object[] args)
        {
            object[] oArgs = args;
            HttpSessionStateBase Context = (HttpSessionStateBase)oArgs[0];
            string type = Convert.ToString(oArgs[1]);
            string status = Convert.ToString(oArgs[2]);
            string oid = Convert.ToString(oArgs[3]);
            try
            {
                List<EPart> Child = EPartRepository.SelRootChildList(new EPart { FromOID = Convert.ToInt32(oid) });
                foreach(EPart Obj in Child)
                {
                    DObjectRepository.UdtDObject(Context, new DObject { OID = Obj.OID, BPolicyOID = 42 });
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
