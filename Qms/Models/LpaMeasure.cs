using Common.Factory;
using Common.Interface;
using Common.Models.File;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Qms.Models
{
    public  class LpaMeasure : QuickResponseModule, IDObject, IObjectFile
    {
        public int? ModuleOID { get; set; }

        // 검토원인상세 
        public string CauseAnalysis { get; set; }

        // 재발방지대책 
        public string ImproveCountermeasure { get; set; }

        public List<HttpPostedFileBase> Files { get; set; }

        public List<HttpFile> delFiles { get; set; }

        public List<LpaUnfitCheck> LpaUnfitChecks { get; set; }


    }

    public static class LpaMeasureRepository
    {
        public static LpaMeasure SelLpaMeasure(LpaMeasure _param)
        {
            return DaoFactory.GetData<LpaMeasure>("Qms.SelLpaMeasure", _param);
        }

        public static List<LpaMeasure> SelLpaMeasures(LpaMeasure _param)
        {
            return DaoFactory.GetList<LpaMeasure>("Qms.SelLpaMeasure", _param);
        }

        public static int InsLpaMeasure(LpaMeasure _param)
        {
            return DaoFactory.SetInsert("Qms.InsLpaMeasure", _param);
        }

        public static int UdtLpaMeasure(LpaMeasure _param)
        {
            return DaoFactory.SetUpdate("Qms.UdtLpaMeasure", _param);
        }
    }
}
