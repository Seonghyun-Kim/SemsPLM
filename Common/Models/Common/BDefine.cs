using Common.Factory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Models
{
    public class BDefine
    {
        public int? OID { get; set; }

        public string Name { get; set; }

        public string Type { get; set; }

        public string Module { get; set; }

        public int? Ord { get; set; }

        public string Description { get; set; }

        public string Link { get; set; }
    }

    public static class BDefineRepository
    {
        public static BDefine SelDefine(BDefine _param)
        {
            return DaoFactory.GetData<BDefine>("Comm.SelBDefine", _param);
        }

        public static List<BDefine> SelDefines(BDefine _param)
        {
            return DaoFactory.GetList<BDefine>("Comm.SelBDefine", _param);
        }

    }
}
