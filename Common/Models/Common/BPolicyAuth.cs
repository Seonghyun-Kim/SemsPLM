using Common.Constant;
using Common.Factory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Common.Models
{
    public class BPolicyAuth
    {
        public int? OID { get; set; }

        public int? PolicyOID { get; set; }

        public int? AuthTargetOID { get; set; }

        public string AuthTargetDiv { get; set; }

        public string AuthNm { get; set; }

        public string AuthTitle { get; set; }

        public string AuthDiv { get; set; }

        public int? AuthObjectOID { get; set; }


        //ETC
        public string Type { get; set; }

        public string Name { get; set; }

    }

    public static class BPolicyAuthRepository
    {

        public static List<BPolicyAuth> SelBPolicyAuths(BPolicyAuth _param)
        {
            return DaoFactory.GetList<BPolicyAuth>("Comm.SelBPolicyAuth", _param);
        }


        public static List<BPolicyAuth> MainAuth(HttpSessionStateBase Context, DObject dobj)
        {
            List<BPolicyAuth> mainAuth = new List<BPolicyAuth>();
            mainAuth.AddRange(OwnerAuth(Context, dobj));
            mainAuth.AddRange(PublicAuth(Context, dobj));
            return mainAuth;
        }

        public static List<BPolicyAuth> OwnerAuth(HttpSessionStateBase Context, DObject dobj)
        {   
            return dobj.CreateUs == Convert.ToInt32(Context["UserOID"]) ? BPolicyAuthRepository.SelBPolicyAuths(new BPolicyAuth { Type = dobj.Type, PolicyOID = dobj.BPolicyOID, AuthTargetDiv = CommonConstant.AUTH_DIV_OWNER, AuthDiv = CommonConstant.AUTH_SYSTEM }) : new List<BPolicyAuth>();
        }

        public static List<BPolicyAuth> PublicAuth(HttpSessionStateBase Context, DObject dobj)
        {
            return BPolicyAuthRepository.SelBPolicyAuths(new BPolicyAuth { Type = dobj.Type, PolicyOID = dobj.BPolicyOID, AuthTargetDiv = CommonConstant.AUTH_DIV_PUBLIC, AuthDiv = CommonConstant.AUTH_SYSTEM });
        }
    }
}
