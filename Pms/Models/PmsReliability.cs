using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Common.Constant;
using Common.Factory;
using Common.Interface;
using Common.Models;
using Common.Models.File;
using Pms.Interface;

namespace Pms.Models
{
    public class PmsReliability : DObject, IDObject
    {
        public int? FromOID { get; set; }
        public int? RootOID { get; set; }
        public string RequiredSchedule { get; set; }
        public int DevStep { get; set; }
        public string TestStandard { get; set; }
        public string RegNum { get; set; }
        public string PartNo { get; set; }
        public string CarType { get; set; }
        public DateTime TestMethodDt { get; set; }
        public string NewVer { get; set; }
        public string HWVer { get; set; }
        public string SWVer { get; set; }
        public string CANVer { get; set; }
        public string TestApplyVer { get; set; }
        public string TestCarType { get; set; }
        public string TestPurpose { get; set; }
        public string TestContents { get; set; }
        public string SampleQuantity { get; set; }
        public string TestStandardContents { get; set; }
        public string Requirements { get; set; }
    }
    public class TestItemList : DObject, IDObject
    {
        public string FromOID { get; set; }
        public string KorNm { get; set; }
        public string TestStandardNo { get; set; }
        public string TestCondition { get; set; }
        public string Sample { get; set; }
        public string ETC { get; set; }
    }


    public static class PmsReliabilityRepository
    {
        public static int InsPmsReliability(HttpSessionStateBase Context, PmsReliability _param)
        {
            return DaoFactory.SetInsert("Pms.InsPmsReliability", _param);
        }

        public static List<PmsReliability> SelPmsReliability(HttpSessionStateBase Context, PmsReliability _param)
        {
            List<PmsReliability> PmsReliability = DaoFactory.GetList<PmsReliability>("Pms.SelPmsReliability", _param);
            PmsReliability.ForEach(obj =>
            {
                obj.BPolicy = BPolicyRepository.SelBPolicy(new BPolicy { Type = obj.Type, OID = obj.BPolicyOID }).First();
            });
            return PmsReliability;
        }

        public static PmsReliability SelPmsReliabilityObject(HttpSessionStateBase Context, PmsReliability _param)
        {
            _param.Type = Common.Constant.PmsConstant.TYPE_RELIABILITY;
            PmsReliability PmsReliability = DaoFactory.GetData<PmsReliability>("Pms.SelPmsReliability", _param);
            PmsReliability.CreateUsNm = PersonRepository.SelPerson(Context, new Person { OID = PmsReliability.CreateUs }).Name;
            PmsReliability.BPolicy = BPolicyRepository.SelBPolicy(new BPolicy { Type = PmsReliability.Type, OID = PmsReliability.BPolicyOID }).First();
            PmsReliability.BPolicyAuths = BPolicyAuthRepository.MainAuth(Context, PmsReliability, null);
            
            return PmsReliability;
        }

        

    }
}
