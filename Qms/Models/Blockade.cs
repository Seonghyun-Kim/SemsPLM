﻿using Common.Models.File;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Qms.Models
{
    public class Blockade : QuickResponseModule, IObjectFile
    {
        public List<BlockadeItem> BlockadeItems { get; set; }

        public List<HttpPostedFileBase> Files { get; set; }

        public List<HttpFile> delFiles { get; set; }
    }
}
