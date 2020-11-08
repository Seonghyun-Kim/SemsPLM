using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Models
{
    public class JqTreeModel
    {
        
        public int? parentId { get; set; }

        public int? id { get; set; }

        public string label { get; set; }

        public string value { get; set; }

        public string icon { get; set; }

        public int? iconsize { get; set; }

        public Boolean expanded { get; set; }

        public Boolean selected { get; set; }

        public List<JqTreeModel> items { get; set; }

        //Custom Attr
        public string type { get; set; }

        public List<string> checkitemtypes { get; set; }

    }
}
