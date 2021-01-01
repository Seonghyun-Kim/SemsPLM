using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pms.Models
{
    public class PmsClass
    {
        public string Name { get; set; }

        public List<PmsProject> Children { get; set; }

        public string expanded { get => "true"; }
    }
}
