using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Models
{
    public class ResultJsonModel
    {
        public bool isError { get; set; }
        public string resultMessage { get; set; }
        public string resultDescription { get; set; }
        public string resultStackTrace { get; set; }
    }
}
