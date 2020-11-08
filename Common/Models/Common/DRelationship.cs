using Common.Factory;
using Common.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Models
{
    public class DRelationship
    {
        public int? OID { get; set; }

        public int? FromOID { get; set; }

        public IDObject FromData { get; set; }

        public int? ToOID { get; set; }

        public IDObject ToData { get; set; }

        public int? Count { get; set; }

        public string Type { get; set; }

        public int? Ord { get; set; }

        public int? Level { get; set; }

        public DateTime? CreateDt { get; set; }

        public int? CreateUs { get; set; }

        public DateTime? DeleteDt { get; set; }

        public int? DeleteUs { get; set; }
    }

    public static class DRelationshipRepository
    {

        public static int InsDRelationshipNotOrd(DRelationship _param)
        {
            _param.CreateUs = 1;
            return DaoFactory.SetInsert("Comm.InsDRelationshipNotOrd", _param);
        }

        public static int InsDRelationship(DRelationship _param)
        {
            _param.CreateUs = 1;
            return DaoFactory.SetInsert("Comm.InsDRelationship", _param);
        }

        public static int DelDRelationship(DRelationship _param)
        {
            _param.DeleteUs = 1;
            return DaoFactory.SetUpdate("Comm.DelDRelationship", _param);
        }

        public static List<DRelationship> SelRelationship(DRelationship _param)
        {
            return DaoFactory.GetList<DRelationship>("Comm.SelDRelationship", _param);
        }
        
    }
}
