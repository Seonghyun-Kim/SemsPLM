using Common.Constant;
using Common.Factory;
using Common.Interface;
using Common.Models;
using Common.Models.File;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Document.Models
{
    public class Doc : DObject,IDObject, IObjectFile
    {
        public int? FromOID { get; set; }
        public int? RootOID { get; set; }
        public int? ToOID { get; set; }
        public int? TaskOID { get; set; }
        public int? DocType { get; set; }
        public string Title { get; set; }
        public string Eo_No { get; set; }
        public string DocGroup { get; set; }
        public int? Doc_Lib_Lev1_OID { get; set; }
        public string Doc_Lib_Lev1_KorNm { get; set; }
        public int? Doc_Lib_Lev2_OID { get; set; }
        public string Doc_Lib_Lev2_KorNm { get; set; }
        public int? Doc_Lib_Lev3_OID { get; set; }
        public string Doc_Lib_Lev3_KorNm { get; set; }
        public string DocType_KorNm { get; set; }
        public string TaskNm { get; set; }

        public string Code { get; set; }

        public List<HttpPostedFileBase> Files { get; set; }
        public List<HttpFile> delFiles { get; set; }
    }

    public static class DocRepository
    {
        public static List<Doc> SelDoc(HttpSessionStateBase Context, Doc _param)
        {
            _param.Type = DocumentConstant.TYPE_DOCUMENT;
            List<Doc> lDocs = new List<Doc>();
            List<Doc> lDoc = DaoFactory.GetList<Doc>("Doc.SelDoc", _param);
            if (lDoc == null || lDoc.Count < 1)
            {
                return lDocs;
            }
            List<BPolicy> lBPlolicy = BPolicyRepository.SelBPolicyOIDs(new BPolicy { OIDs = lDoc.Select(sel => Convert.ToInt32(sel.BPolicyOID)).ToList() });
            List<Person> lPerson = PersonRepository.SelPersons(Context, new Person { OIDs = lDoc.Select(sel => Convert.ToInt32(sel.CreateUs)).ToList() });
            lDoc.ForEach(obj =>
            {
                //obj.BPolicy = BPolicyRepository.SelBPolicy(new BPolicy { Type = obj.Type, OID = obj.BPolicyOID }).First();
                obj.BPolicy = lBPlolicy.Find(data => data.OID == obj.BPolicyOID);
                obj.DocType_KorNm = DObjectRepository.SelDObject(Context, new DObject { OID = obj.DocType }).Name;
                //obj.CreateUsNm = PersonRepository.SelPerson(Context, new Person { OID = obj.CreateUs }).Name;
                obj.CreateUsNm = lPerson.Find(data => data.OID == obj.CreateUs).Name;

                obj.BPolicyAuths = BPolicyAuthRepository.MainAuth(Context, obj, null);
                if (obj.BPolicyAuths.FindAll(item => item.AuthNm == CommonConstant.AUTH_VIEW).Count > 0)
                {
                    lDocs.Add(obj);
                }
            });
            return lDocs;
        }

        public static Doc SelDocObject(HttpSessionStateBase Context, Doc _param)
        {
            _param.Type = DocumentConstant.TYPE_DOCUMENT;
            Doc lDoc = DaoFactory.GetData<Doc>("Doc.SelDoc", _param);
            lDoc.CreateUsNm  = PersonRepository.SelPerson(Context, new Person { OID = lDoc.CreateUs }).Name;
            lDoc.DocType_KorNm = DObjectRepository.SelDObject(Context, new DObject { OID = lDoc.DocType }).Name;
            lDoc.BPolicy = BPolicyRepository.SelBPolicy(new BPolicy { Type = lDoc.Type, OID = lDoc.BPolicyOID }).First();
            lDoc.BPolicyAuths = BPolicyAuthRepository.MainAuth(Context, lDoc, null);
            return lDoc;
        }

        #region 문서 수정
        public static Doc UdtDocObject(Doc _param)
        {
            DaoFactory.SetUpdate("Doc.UdtDoc", _param);

            return _param;
        }
        #endregion

    }
}
