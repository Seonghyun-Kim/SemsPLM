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
    public class ImproveCounterMeasureItem : DObject, IDObject
    {     
        // 개선대책 ModuleOID 
        public int? ModuleOID { get; set; }

        // 근본원인 
        public string RootCause { get; set; }

        // 개선대책 
        public string ImproveCountermeasure { get; set; }

        // 처리일자 
        public DateTime? ProcessDt { get; set; }

        public string IsRemove { get; set; }
    }

    public static class ImproveCounterMeasureItemRepository
    {
        public static ImproveCounterMeasureItem SelImproveCounterMeasureItem(ImproveCounterMeasureItem _param)
        {
            return DaoFactory.GetData<ImproveCounterMeasureItem>("Qms.SelImproveCounterMeasureItem", _param);
        }

        public static List<ImproveCounterMeasureItem> SelImproveCounterMeasureItems(ImproveCounterMeasureItem _param)
        {
            return DaoFactory.GetList<ImproveCounterMeasureItem>("Qms.SelImproveCounterMeasureItem", _param);
        }

        public static int InsImproveCounterMeasureItem(ImproveCounterMeasureItem _param)
        {
            return DaoFactory.SetInsert("Qms.InsImproveCounterMeasureItem", _param);
        }

        public static int UdtImproveCounterMeasureItem(ImproveCounterMeasureItem _param)
        {
            return DaoFactory.SetUpdate("Qms.UdtImproveCounterMeasureItem", _param);
        }

        public static int DelImproveCounterMeasureItem(ImproveCounterMeasureItem _param)
        {
            return DaoFactory.SetUpdate("Qms.DelImproveCounterMeasure", _param);
        }

    }
}
