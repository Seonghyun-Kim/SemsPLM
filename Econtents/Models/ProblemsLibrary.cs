using Common;
using Common.Constant;
using Common.Factory;
using Common.Interface;
using Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Econtents.Models
{
    public class ProblemsLibrary : DObject, IDObject
    {
        public int? Car_Lib_OID { get; set; }
        public int? Product { get; set; }
        public int? Part { get; set; }
        public int? Occurrence { get; set; }
        public int? Stage_Occurrence { get; set; }
        public int? Failure_Type { get; set; }
        public int? Division { get; set; }
        public string Issues { get; set; }
        public string Issues_Thumbnail { get; set; }
        public string Cause { get; set; }
        public string Cause_Thumbnail { get; set; }
        public string Countermeasures { get; set; }
        public string Countermeasures_Thumbnail { get; set; }
    }

    public static class ProblemsLibraryRepository
    {
        #region ProblemsLibrary 리스트 검색
        public static List<ProblemsLibrary> SelProblemsLibrary(ProblemsLibrary _param)
        {
            _param.Type = EcontentsConstant.TYPE_ECONTENTS;
            //_param.StartCreateDt = 
            List<ProblemsLibrary> lProblemsLibrary = DaoFactory.GetList<ProblemsLibrary>("Econtents.SelProblemsLibrary", _param);
            lProblemsLibrary.ForEach(obj =>
            {
                obj.BPolicy = BPolicyRepository.SelBPolicy(new BPolicy { Type = obj.Type, OID = obj.BPolicyOID }).First();
            });
            return lProblemsLibrary;
        }
        #endregion
        #region ProblemsLibrary 오브젝 검색

        public static ProblemsLibrary SelProblemsLibraryObject(ProblemsLibrary _param)
        {
            _param.Type = EcontentsConstant.TYPE_ECONTENTS;
            ProblemsLibrary lProblemsLibrary = DaoFactory.GetData<ProblemsLibrary>("Econtents.SelProblemsLibrary", _param);

            lProblemsLibrary.BPolicy = BPolicyRepository.SelBPolicy(new BPolicy { Type = lProblemsLibrary.Type, OID = lProblemsLibrary.BPolicyOID }).First();

            return lProblemsLibrary;
        }
        #endregion
    }
}