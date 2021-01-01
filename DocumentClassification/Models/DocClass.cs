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

        public int? RootOID { get; set; }
        public int? ToOID { get; set; }
        public string ProjectNm { get; set; }
        public string DocClassNm { get; set; }
        public string TaskNm { get; set; }

        public List<DocClass> Children { get; set; }

        public string RelOID { get; set; }
        public string RelType { get; set; }
        public string expanded { get; set; }
        public string Files { get; set; }
        public string DocToOID { get; set; }
        public string DocClassOID { get; set; }
        public string DocClassPID { get; set; }
        public string DocClassPIDNm { get; set; }
        public string DocName { get; set; }
        public string DocOID { get; set; }
        public string DocNo { get; set; }
        public string DocNoNm { get; set; }
        public string DocSt { get; set; }
        public string DocStNm { get; set; }
        public string DocRev { get; set; }
        public string DatabaseFl { get; set; }
        public string LinkOID { get; set; }
        public string EditUrl { get; set; }
        public string PMSViewUrl { get; set; }
        public string PMSEditUrl { get; set; }
        public string UseFl { get; set; }

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

        public static List<JqTreeModel> SelDocClassTree(HttpSessionStateBase Context, string Type)
        {
            DocClass Document = DocClassRepository.SelDocClassObject(Context, new DocClass { Name = Type });
            List<JqTreeModel> jqTreeModelList = new List<JqTreeModel>();
            JqTreeModel jqTreeModel = new JqTreeModel();
            jqTreeModel.id = Document.OID;
            jqTreeModel.label = Document.Name;
            jqTreeModel.icon = CommonConstant.ICON_DOCUMENT;
            jqTreeModel.iconsize = CommonConstant.DEFAULT_ICONSIZE;
            jqTreeModel.expanded = true;
            jqTreeModel.type = Type;
            jqTreeModel.items = new List<JqTreeModel>();

            List<DocClass> docTypeList = DocClassRepository.SelDocClass(Context, new DocClass { FromOID = Document.OID });

            docTypeList.ForEach(item =>
            {
                JqTreeModel innerJqTreeModel = new JqTreeModel();
                innerJqTreeModel.id = item.OID;
                innerJqTreeModel.label = item.Name;
                innerJqTreeModel.icon = CommonConstant.ICON_DOCUMENT_DETAIL;
                innerJqTreeModel.iconsize = PmsConstant.DEFAULT_ICONSIZE;
                innerJqTreeModel.expanded = true;
                innerJqTreeModel.type = DocClassConstant.TYPE_DOCCLASS;
                jqTreeModel.items.Add(innerJqTreeModel);
            });
            jqTreeModelList.Add(jqTreeModel);

            return jqTreeModelList;
        }

    }
}
