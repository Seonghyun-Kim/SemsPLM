using Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Common.Trigger
{
    public class ApprovalTrigger
    {
        public string ActionTemplatePromote(object[] args)
        {
            object[] oArgs = args;
            HttpSessionStateBase Context = (HttpSessionStateBase)oArgs[0];
            int iAdmin = Convert.ToInt32(Context["UserOID"]);
            string type = Convert.ToString(oArgs[1]);
            string status = Convert.ToString(oArgs[2]);
            string oid = Convert.ToString(oArgs[3]);
            try
            {
                DObjectRepository.UdtDObject(Context, new DObject { OID = Convert.ToInt32(oid), Description = "11111" });
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            return "";
        }

    }
}
