using Common.Constant;
using Common.Factory;
using Common.Interface;
using Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Document.Models
{
    public class Doc : DObject,IDObject
    {
        public string DocType { get; set; }
        public string Title { get; set; }
        public string Eo_No { get; set; }
        public int? Doc_Lib_Lev1_OID { get; set; }
        public string Doc_Lib_Lev1_KorNm { get; set; }
        public int? Doc_Lib_Lev2_OID { get; set; }
        public string Doc_Lib_Lev2_KorNm { get; set; }
        public int? Doc_Lib_Lev3_OID { get; set; }
        public string Doc_Lib_Lev3_KorNm { get; set; }
        public string DocType_KorNm
        {
            get
            {
               if(this.DocType == DocumentContant.TYPE_PROJECT_DOCUMENT)
                {
                    return DocumentContant.TYPE_PROJECT_DOCUMENT_KOR;
                }else if(this.DocType == DocumentContant.TYPE_TECHNICAL_DOCUMENT)
                {
                    return DocumentContant.TYPE_TECHNICAL_DOCUMENT_KOR;
                }
                else
                {
                    return "";
                }
            }
        }


    }

    public static class DocRepository
    {
        public static List<Doc> SelDoc(Doc _param)
        {
            _param.Type = DocumentContant.TYPE_DOCUMENT;
            List<Doc> lDoc = DaoFactory.GetList<Doc>("Doc.SelDoc", _param);
            lDoc.ForEach(obj =>
            {
                obj.BPolicy = BPolicyRepository.SelBPolicy(new BPolicy { Type = obj.Type, OID = obj.BPolicyOID }).First();
            });
            return lDoc;
        }

        public static Doc SelDocObject(Doc _param)
        {
            _param.Type = DocumentContant.TYPE_DOCUMENT;
            Doc lDoc = DaoFactory.GetData<Doc>("Doc.SelDoc", _param);

            lDoc.BPolicy = BPolicyRepository.SelBPolicy(new BPolicy { Type = lDoc.Type, OID = lDoc.BPolicyOID }).First();
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
