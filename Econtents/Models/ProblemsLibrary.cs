using Common;
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

namespace Econtents.Models
{
    public class ProblemsLibrary : DObject, IDObject, IObjectFile
    {
        public int? OEM_Lib_OID { get; set; }
        public int? Car_Lib_OID { get; set; }
        public string Product { get; set; }
        public string Part { get; set; }
        public string Occurrence { get; set; }
        public string Stage_Occurrence { get; set; }
        public string Failure_Type { get; set; }
        public string Division { get; set; }
        public string Issues { get; set; }
        public string Issues_Thumbnail { get; set; }
        public string Cause { get; set; }
        public string Cause_Thumbnail { get; set; }
        public string Countermeasures { get; set; }
        public string Countermeasures_Thumbnail { get; set; }

        public string OEM_Lib_Nm { get; set; }
        public string Car_Lib_Nm { get; set; }

        public List<HttpPostedFileBase> Files { get; set; }
        public List<HttpFile> delFiles { get; set; }
    }

    public static class ProblemsLibraryRepository
    {

        #region ProblemsLibrary 리스트 검색
        public static List<ProblemsLibrary> SelProblemsLibrary(HttpSessionStateBase Context, ProblemsLibrary _param)
        {
            _param.Type = EcontentsConstant.TYPE_PROBLEMS_LIBRARY;
            List<ProblemsLibrary> lProblemsLibrary = DaoFactory.GetList<ProblemsLibrary>("Econtents.SelProblemsLibrary", _param);
            lProblemsLibrary.ForEach(obj =>
            {
                obj.CreateUsNm = PersonRepository.SelPerson(Context, new Person { OID = obj.CreateUs }).Name;
                obj.BPolicy = BPolicyRepository.SelBPolicy(new BPolicy { Type = obj.Type, OID = obj.BPolicyOID }).First();
                obj.BPolicyNm = obj.BPolicy.Name;
                obj.BPolicyAuths = BPolicyAuthRepository.MainAuth(Context, obj, null);
                if (obj.OEM_Lib_OID != null)
                {
                    obj.OEM_Lib_Nm = LibraryRepository.SelCodeLibraryObject(new Library { OID = obj.OEM_Lib_OID }).KorNm;
                }
                if (obj.Car_Lib_OID != null)
                {
                    obj.Car_Lib_Nm = LibraryRepository.SelCodeLibraryObject(new Library { OID = obj.Car_Lib_OID }).KorNm;
                }
            }); 
            return lProblemsLibrary;
        }
        #endregion
        #region ProblemsLibrary 오브젝 검색

        public static ProblemsLibrary SelProblemsLibraryObject(HttpSessionStateBase Context, ProblemsLibrary _param)
        {
            _param.Type = EcontentsConstant.TYPE_PROBLEMS_LIBRARY;
            ProblemsLibrary lProblemsLibrary = DaoFactory.GetData<ProblemsLibrary>("Econtents.SelProblemsLibrary", _param);

            lProblemsLibrary.CreateUsNm = PersonRepository.SelPerson(Context, new Person { OID = lProblemsLibrary.CreateUs }).Name;
            lProblemsLibrary.BPolicy = BPolicyRepository.SelBPolicy(new BPolicy { Type = lProblemsLibrary.Type, OID = lProblemsLibrary.BPolicyOID }).First();
            lProblemsLibrary.BPolicyNm = lProblemsLibrary.BPolicy.Name;
            lProblemsLibrary.BPolicyAuths = BPolicyAuthRepository.MainAuth(Context, lProblemsLibrary, null);
            if (lProblemsLibrary.OEM_Lib_OID != null)
            {
                lProblemsLibrary.OEM_Lib_Nm = LibraryRepository.SelCodeLibraryObject(new Library { OID = lProblemsLibrary.OEM_Lib_OID }).KorNm;
            }
            if (lProblemsLibrary.Car_Lib_OID != null)
            {
                lProblemsLibrary.Car_Lib_Nm = LibraryRepository.SelCodeLibraryObject(new Library { OID = lProblemsLibrary.Car_Lib_OID }).KorNm;
            }

            return lProblemsLibrary;
        }
        #endregion

        
        #region ProblemsLibrary 오브젝 수정

        public static int UdtProblemsLibrary(HttpSessionStateBase Context, ProblemsLibrary _param)
        {
            _param.Type = EcontentsConstant.TYPE_PROBLEMS_LIBRARY;
            int Udt = DaoFactory.SetUpdate("Econtents.UdtProblemsLibrary", _param);
            return Udt;
        }
        #endregion
    }
}