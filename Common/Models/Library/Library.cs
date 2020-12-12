using Common.Constant;
using Common.Factory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Models
{
    public class Library : DObject
    {
        public string KorNm { get; set; }    //한글명
        public int? FromOID { get; set; }     //부모 OID
        public string IsUse { get; set; }    //사용여부
        public int? Ord { get; set; }         //순서
        public string isRequired { get; set; } //필수여부
        public string Code1 { get; set; } //코드1
        public string Code2 { get; set; } //코드2
    }

    public static class LibraryRepository
    {
        public static int updateLibrary(Library _param)
        {
            return DaoFactory.SetUpdate("Library.updateLibrary", _param);
        }
        public static List<Library> SelLibrary(Library _param)
        {
            _param.Type = CommonConstant.TYPE_LIBRARY;
            List<Library> lLibrary = DaoFactory.GetList<Library>("Library.SelLibrary", _param);
            lLibrary.ForEach(obj =>
            {
                obj.BPolicy = BPolicyRepository.SelBPolicy(new BPolicy { Type = obj.Type, OID = obj.BPolicyOID }).First();
            });
            return lLibrary;
        }

        public static Library SelLibraryObject(Library _param)
        {
            _param.Type = CommonConstant.TYPE_LIBRARY;
            Library Library = DaoFactory.GetData<Library>("Library.SelLibrary", _param);
            //Library.BPolicy = BPolicyRepository.SelBPolicy(new BPolicy { Type = Library.Type, OID = Library.BPolicyOID }).First();
            return Library;
        }

        public static int updateCodeLibrary(Library _param)
        {
            return DaoFactory.SetUpdate("Library.updateCodeLibrary", _param);
        }
        public static int deleteCodeLibrary(Library _param)
        {
            return DaoFactory.SetUpdate("Library.deleteCodeLibrary", _param);
        }
        public static List<Library> SelCodeLibrary(Library _param)
        {
            List<Library> lLibrary = DaoFactory.GetList<Library>("Library.SelCodeLibrary", _param);
            return lLibrary;
        }

        public static Library SelCodeLibraryObject(Library _param)
        {
            Library Library = DaoFactory.GetData<Library>("Library.SelCodeLibrary", _param);
            return Library;
        }
        public static List<Library> SelCodeLibraryChild(Library _param)
        {
            Library temp = DaoFactory.GetData<Library>("Library.SelCodeLibrary", _param);

            List<Library> lLibrary = DaoFactory.GetList<Library>("Library.SelCodeLibrary", new Library {FromOID = temp.OID });
            return lLibrary;
        }

    }
}
