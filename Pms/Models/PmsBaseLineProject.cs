using Common.Constant;
using Common.Factory;
using Common.Interface;
using Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pms.Models
{
    public class PmsBaseLineProject : PmsProject
    {
        
        public int? TargetProjectOID { get; set; }
        public int? ProjectBaseLineOID { get; set; }
        public int? ProjectOID { get; set; }
        public string ProjectNm { get; set; }

    }

    public static class PmsBaseLineProjectRepository
    {

        public static PmsBaseLineProject SelPmsBaseLIneProject(PmsBaseLineProject _param)
        {
            _param.Type = PmsConstant.TYPE_BASE_LINE_PROJECT;
            PmsBaseLineProject pmsBaseLineProject = DaoFactory.GetData<PmsBaseLineProject>("Pms.SelPmsBaseLineProject", _param);
            pmsBaseLineProject.BPolicy = BPolicyRepository.SelBPolicy(new BPolicy { Type = pmsBaseLineProject.Type, OID = pmsBaseLineProject.BPolicyOID }).First();
            return pmsBaseLineProject;
        }

        public static int InsPmsBaseLineProject(PmsBaseLineProject _param)
        {
            _param.CreateUs = 1;
            if (_param.BPolicyOID == null)
            {
                _param.BPolicyOID = BPolicyRepository.SelBPolicy(new BPolicy { Type = _param.Type }).First().OID;
            }
            return DaoFactory.SetInsert("Pms.InsPmsBaseLineProject", _param);
        }
    }
}
