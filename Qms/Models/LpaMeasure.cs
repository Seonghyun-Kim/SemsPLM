using Common.Factory;
using Common.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qms.Models
{
    public  class LpaMeasure : QuickResponseModule, IDObject
    {
        // 검토원인상세 
        public string CauseAnalysis { get; set; }

        // 재발방지대책 
        public string ImproveCountermeasure { get; set; }

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
            _param.CreateUs = 1;
            return DaoFactory.SetInsert("Qms.InsLpaMeasure", _param);
        }

        public static int UdtLpaMeasure(LpaMeasure _param)
        {
            _param.ModifyUs = 1;
            return DaoFactory.SetUpdate("Qms.UdtLpaMeasure", _param);
        }
    }
}
