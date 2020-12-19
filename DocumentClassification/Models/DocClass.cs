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

namespace DocumentClassification.Models
{
    public class DocClass : DObject, IDObject
    {
        public int? FromOID { get; set; }
        public string Classification { get; set; }
        public string Code { get; set; }
        public string ViewUrl { get; set; }
        public string IsUse { get; set; }
        public string IsRequired { get; set; }

    }
    public static class DocClassRepository
    {
        public static List<DocClass> SelDocClass(HttpSessionStateBase Context, DocClass _param)
        {
            _param.Type = DocClassConstant.TYPE_DOCCLASS;
            List<DocClass> lDocClass = DaoFactory.GetList<DocClass>("DocClass.SelDocClass", _param);
            lDocClass.ForEach(obj =>
            {
                obj.BPolicy = BPolicyRepository.SelBPolicy(new BPolicy { Type = obj.Type, OID = obj.BPolicyOID }).First();
            });
            return lDocClass;
        }
        public static DocClass SelDocClassObject(HttpSessionStateBase Context, DocClass _param)
        {
            _param.Type = DocClassConstant.TYPE_DOCCLASS;
            DocClass lDocClass = DaoFactory.GetData<DocClass>("DocClass.SelDocClass", _param);
            lDocClass.BPolicy = BPolicyRepository.SelBPolicy(new BPolicy { Type = lDocClass.Type, OID = lDocClass.BPolicyOID }).First();

            return lDocClass;
        }

        public static List<DocClass> SelProjectDocClass(HttpSessionStateBase Context, DocClass _param)
        {
            _param.Type = DocClassConstant.TYPE_DOCCLASS;
            List<DocClass> lDocClass = DaoFactory.GetList<DocClass>("DocClass.SelProjectDocClass", _param);
            lDocClass.ForEach(obj =>
            {
                obj.BPolicy = BPolicyRepository.SelBPolicy(new BPolicy { Type = obj.Type, OID = obj.BPolicyOID }).First();
            });
            return lDocClass;
        }

    }
}
