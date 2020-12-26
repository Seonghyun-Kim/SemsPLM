using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Common.Factory;
using Common.Interface;
using Common.Models;
using Common.Models.File;

namespace Pms.Models
{
    public class PmsGateMetting : DObject, IDObject, IObjectFile
    {
        public int? RootOID { get; set; }
        public int? FromOID { get; set; }
        public int? MettingOID { get; set; }
        public List<HttpPostedFileBase> Files { get; set; }
        public List<HttpFile> delFiles { get; set; }
    }
    public static class PmsGateMettingRepository
    {
        public static List<PmsGateMetting> SelPmsGateMetting(HttpSessionStateBase Context, PmsGateMetting _param)
        {
            _param.Type = Common.Constant.PmsConstant.TYPE_GATEVIEW_METTING;
            List<PmsGateMetting> PmsGateMetting = DaoFactory.GetList<PmsGateMetting>("Pms.SelPmsGateMetting", _param);
            PmsGateMetting.ForEach(item =>
            {
                item.BPolicy = BPolicyRepository.SelBPolicy(new BPolicy { Type = item.Type, OID = item.BPolicyOID }).First();
            });
            return PmsGateMetting;
        }

        public static PmsGateMetting SelPmsGateMettingObject(HttpSessionStateBase Context, PmsGateMetting _param)
        {
            _param.Type = Common.Constant.PmsConstant.TYPE_GATEVIEW_METTING;
            PmsGateMetting PmsGateMetting = DaoFactory.GetData<PmsGateMetting>("Pms.SelPmsGateMetting", _param);
            PmsGateMetting.BPolicy = BPolicyRepository.SelBPolicy(new BPolicy { Type = PmsGateMetting.Type, OID = PmsGateMetting.BPolicyOID }).First();
            return PmsGateMetting;
        }
        
    }
}
