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
    public class OccurrenceCauseItem : DObject, IDObject
    {
        // 발생원인OID 
        public int? ModuleOID { get; set; }

        // 발생원인 
        public int OccurrenceCauseLibOID { get; set; }

        public string OccurrenceCauseLibText { get; set; }

        public List<OccurrenceWhy> OccurrenceWhys { get; set; }

    }

    public static class OccurrenceCauseItemRepository
    {
        public static OccurrenceCauseItem SelOccurrenceCauseItem(OccurrenceCauseItem _param)
        {
            return DaoFactory.GetData<OccurrenceCauseItem>("Qms.SelOccurrenceCauseItem", _param);
        }

        public static List<OccurrenceCauseItem> SelOccurrenceCauseItems(OccurrenceCauseItem _param)
        {
            return DaoFactory.GetList<OccurrenceCauseItem>("Qms.SelOccurrenceCauseItem", _param);
        }

        public static int InsOccurrenceCauseItem(OccurrenceCauseItem _param)
        {
            return DaoFactory.SetInsert("Qms.InsOccurrenceCauseItem", _param);
        }

        public static int UdtOccurrenceCauseItem(OccurrenceCauseItem _param)
        {
            return DaoFactory.SetUpdate("Qms.UdtOccurrenceCauseItem", _param);
        }

        public static int DelOccurrenceCauseItem(OccurrenceCauseItem _param)
        {
            return DaoFactory.SetUpdate("Qms.DelOccurrenceCauseItem", _param);
        }
    }

    public class OccurrenceWhy : DObject, IDObject
    {
        // 발생원인 OID 
        public int? CauseOID { get; set; }

        // 발생원인 내역 
        public string OccurrenceCauseDetail { get; set; }

        public string IsRemove { get; set; }

    }

    public static class OccurrenceWhyRepository
    {
        public static OccurrenceWhy SelOccurrenceWhy(OccurrenceWhy _param)
        {
            return DaoFactory.GetData<OccurrenceWhy>("Qms.SelOccurrenceWhy", _param);
        }

        public static List<OccurrenceWhy> SelOccurrenceWhys(OccurrenceWhy _param)
        {
            return DaoFactory.GetList<OccurrenceWhy>("Qms.SelOccurrenceWhy", _param);
        }

        public static int InsOccurrenceWhy(OccurrenceWhy _param)
        {
            return DaoFactory.SetInsert("Qms.InsOccurrenceWhy", _param);
        }

        public static int UdtOccurrenceWhy(OccurrenceWhy _param)
        {
            return DaoFactory.SetUpdate("Qms.UdtOccurrenceWhy", _param);
        }

        public static int DelOccurrenceWhy(OccurrenceWhy _param)
        {
            return DaoFactory.SetUpdate("Qms.DelOccurrenceWhy", _param);
        }
    }

}
