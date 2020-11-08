using Common.Factory;
using Common.Interface;
using Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qms.Models
{
    public class ImproveCounterMeasure : DObject, IDObject
    {     
        // 개선대책 ModuleOID 
        public int ModuleOID { get; set; }

        // 근본원인 
        public string RootCause { get; set; }

        // 개선대책 
        public string ImproveCountermeasure { get; set; }

        // 처리일자 
        public DateTime? ProcessDt { get; set; }
    }

    public static class ImproveCounterMeasureRepository
    {
        public static ImproveCounterMeasure SelImproveCounterMeasure(ImproveCounterMeasure _param)
        {
            return DaoFactory.GetData<ImproveCounterMeasure>("Qms.SelImproveCounterMeasure", _param);
        }

        public static List<ImproveCounterMeasure> SelImproveCounterMeasures(ImproveCounterMeasure _param)
        {
            return DaoFactory.GetList<ImproveCounterMeasure>("Qms.SelImproveCounterMeasure", _param);
        }

        public static int InsImproveCounterMeasure(ImproveCounterMeasure _param)
        {
            _param.CreateUs = 1;
            return DaoFactory.SetInsert("Qms.InsImproveCounterMeasure", _param);
        }

        public static int UdtImproveCounterMeasure(ImproveCounterMeasure _param)
        {
            _param.ModifyUs = 1;
            return DaoFactory.SetUpdate("Qms.UdtImproveCounterMeasure", _param);
        }

        public static int DelImproveCounterMeasure(ImproveCounterMeasure _param)
        {
            _param.DeleteUs = 1;
            return DaoFactory.SetUpdate("Qms.DelImproveCounterMeasure", _param);
        }

    }
}
