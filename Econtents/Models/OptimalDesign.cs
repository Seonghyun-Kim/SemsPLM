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
    public class OptimalDesign : DObject, IDObject
    {
        public int? Oem_Lib_OID { get; set; }
        public int? Car_Lib_OID { get; set; }

        public string Oem_Lib_Nm { get; set; }
        public string Car_Lib_Nm { get; set; }

        public string Product { get; set; }
        public string Division { get; set; }
        public int? ReflectedNum { get; set; }
        public int? NotReflectedNum { get; set; }
    }
    public class OptimalDesignItem : DObject, IDObject
    {
        public int? FromOID { get; set; }
        public int? Problems_Lib_OID { get; set; }
        public string Reflected { get; set; }
        public int? Oem_Lib_OID { get; set; }
        public int? Car_Lib_OID { get; set; }
        public string Product { get; set; }
        public string Part { get; set; }

        public string Problems_Lib_Name { get; set; }
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
        public string Oem_Lib_Nm { get; set; }
        public string Car_Lib_Nm { get; set; }
    }

    public static class OptimalDesignRepository
    {
        #region OptimalDesign 리스트 검색
        public static List<OptimalDesign> SelOptimalDesign(HttpSessionStateBase Context, OptimalDesign _param)
        {
            _param.Type = EcontentsConstant.TYPE_OPTIMAL_DESIGN;
            List<OptimalDesign> lOptimalDesign = DaoFactory.GetList<OptimalDesign>("Econtents.SelOptimalDesign", _param);
            lOptimalDesign.ForEach(obj =>
            {
                obj.CreateUsNm = PersonRepository.SelPerson(Context, new Person { OID = obj.CreateUs }).Name;
                obj.BPolicy = BPolicyRepository.SelBPolicy(new BPolicy { Type = obj.Type, OID = obj.BPolicyOID }).First();
                obj.BPolicyNm = obj.BPolicy.Name;
                obj.BPolicyAuths = BPolicyAuthRepository.MainAuth(Context, obj, null);
                if (obj.Oem_Lib_OID != null)
                {
                    obj.Oem_Lib_Nm = LibraryRepository.SelCodeLibraryObject(new Library { OID = obj.Oem_Lib_OID }).KorNm;
                }
                if (obj.Car_Lib_OID != null)
                {
                    obj.Car_Lib_Nm = LibraryRepository.SelCodeLibraryObject(new Library { OID = obj.Car_Lib_OID }).KorNm;
                }
            });
            return lOptimalDesign;
        }
        #endregion

        #region OptimalDesign 오브젝트 검색
        public static OptimalDesign SelOptimalDesignObject(HttpSessionStateBase Context, OptimalDesign _param)
        {
            _param.Type = EcontentsConstant.TYPE_OPTIMAL_DESIGN;
            OptimalDesign lOptimalDesign = DaoFactory.GetData<OptimalDesign>("Econtents.SelOptimalDesign", _param);
            lOptimalDesign.CreateUsNm = PersonRepository.SelPerson(Context, new Person { OID = lOptimalDesign.CreateUs }).Name;
            lOptimalDesign.BPolicy = BPolicyRepository.SelBPolicy(new BPolicy { Type = lOptimalDesign.Type, OID = lOptimalDesign.BPolicyOID }).First();
            lOptimalDesign.BPolicyNm = lOptimalDesign.BPolicy.Name;
            lOptimalDesign.BPolicyAuths = BPolicyAuthRepository.MainAuth(Context, lOptimalDesign, null);
            if (lOptimalDesign.Oem_Lib_OID != null)
            {
                lOptimalDesign.Oem_Lib_Nm = LibraryRepository.SelCodeLibraryObject(new Library { OID = lOptimalDesign.Oem_Lib_OID }).KorNm;
            }
            if (lOptimalDesign.Car_Lib_OID != null)
            {
                lOptimalDesign.Car_Lib_Nm = LibraryRepository.SelCodeLibraryObject(new Library { OID = lOptimalDesign.Car_Lib_OID }).KorNm;
            }

            return lOptimalDesign;
        }
        #endregion

        #region OptimalDesign 리스트 검색
        public static List<OptimalDesignItem> SelOptimalDesignItem(HttpSessionStateBase Context, OptimalDesignItem _param)
        {
            _param.Type = EcontentsConstant.TYPE_OPTIMAL_DESIGN_ITEM;
            List<OptimalDesignItem> lOptimalDesignItem = DaoFactory.GetList<OptimalDesignItem>("Econtents.SelOptimalDesignItem", _param);

            ProblemsLibrary ProblemsLibraryDetail = new ProblemsLibrary();
            lOptimalDesignItem.ForEach(obj =>
            {
                ProblemsLibraryDetail = ProblemsLibraryRepository.SelProblemsLibraryObject(Context, new ProblemsLibrary { OID = obj.Problems_Lib_OID });

                obj.Problems_Lib_OID = ProblemsLibraryDetail.OID;
                obj.Problems_Lib_Name = ProblemsLibraryDetail.Name;
                obj.Oem_Lib_OID = ProblemsLibraryDetail.OEM_Lib_OID;
                obj.Car_Lib_OID = ProblemsLibraryDetail.Car_Lib_OID;
                obj.Product = ProblemsLibraryDetail.Product;
                obj.Part = ProblemsLibraryDetail.Part;
                obj.Occurrence = ProblemsLibraryDetail.Occurrence;
                obj.Stage_Occurrence = ProblemsLibraryDetail.Stage_Occurrence;
                obj.Failure_Type = ProblemsLibraryDetail.Failure_Type;
                obj.Division = ProblemsLibraryDetail.Division;
                obj.Issues = ProblemsLibraryDetail.Issues;
                obj.Cause = ProblemsLibraryDetail.Cause;
                obj.Countermeasures = ProblemsLibraryDetail.Countermeasures;

                obj.Oem_Lib_Nm = LibraryRepository.SelCodeLibraryObject(new Library { OID = obj.Oem_Lib_OID }).KorNm;
                obj.Car_Lib_Nm = LibraryRepository.SelCodeLibraryObject(new Library { OID = obj.Car_Lib_OID }).KorNm;
            });

            return lOptimalDesignItem;
        }
        #endregion

        

    }
}
