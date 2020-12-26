using Common.Constant;
using Common.Models;
using Pms.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Pms.Trigger
{
    public class ProjectTrigger
    {

        public string CheckStartedProjectPromote(object[] args)
        {
            object[] oArgs = args;
            HttpSessionStateBase Context = (HttpSessionStateBase)oArgs[0];
            int iUser = Convert.ToInt32(Context["UserOID"]);
            string type = Convert.ToString(oArgs[1]);
            string status = Convert.ToString(oArgs[2]);
            string oid = Convert.ToString(oArgs[3]);
            try
            {
                List<PmsRelationship> lProjWbs = PmsRelationshipRepository.GetProjWbsLIst(Context, oid);
                if(lProjWbs.FindAll(wbs => { return wbs.ObjType == PmsConstant.TYPE_TASK && wbs.Members.Count < 1; }).Count > 0)
                {
                    return "Member를 할당해 주세요.";
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            return "";
        }

        public string ActionStartedProjectPromote(object[] args)
        {
            object[] oArgs = args;
            HttpSessionStateBase Context = (HttpSessionStateBase)oArgs[0];
            int iUser = Convert.ToInt32(Context["UserOID"]);
            string type = Convert.ToString(oArgs[1]);
            string status = Convert.ToString(oArgs[2]);
            string oid = Convert.ToString(oArgs[3]);
            try
            {
                DObjectRepository.UdtDObject(Context, new DObject { OID = Convert.ToInt32(oid) });
                PmsProjectRepository.UdtPmsProject(Context, new PmsProject { OID = Convert.ToInt32(oid), ActStartDt = DateTime.Now });
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            return "";
        }

    }
}
