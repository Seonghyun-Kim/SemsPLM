using Common.Constant;
using Common.Models;
using Pms.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Pms.Auth
{
    public static class PmsAuth
    {
        public static List<BPolicyAuth> RoleAuth(HttpSessionStateBase Context, DObject dobj)
        {
            List<BDefine> lRoles = BDefineRepository.SelDefines(new BDefine { Type = CommonConstant.TYPE_ROLE, Module = PmsConstant.MODULE_PMS });
            List<PmsRelationship> members = PmsRelationshipRepository.SelPmsRelationship(Context, new PmsRelationship { Type = PmsConstant.RELATIONSHIP_MEMBER, FromOID = dobj.OID, ToOID = Convert.ToInt32(Context["UserOID"]) });
            if (members.Count > 0)
            {
                return BPolicyAuthRepository.SelBPolicyAuths(new BPolicyAuth { Type = dobj.Type, PolicyOID = dobj.BPolicyOID, AuthTargetDiv = CommonConstant.TYPE_ROLE, AuthTargetOID = members.First().RoleOID, AuthDiv = CommonConstant.AUTH_SYSTEM });
            }
            return new List<BPolicyAuth>();

        }
    }
}
