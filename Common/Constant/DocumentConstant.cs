﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Constant
{
    public class DocumentConstant
    {
        public static string TYPE_DOCUMENT = "DOCUMENT";
        public static string TABLE_DOCUMENT = "T_DDOCUMENT";
        public static string TYPE_PROJECT_DOCUMENT = "PDOC"; //프로젝트문서

        public static string TYPE_TECHNICAL_DOCUMENT = "TDOC"; //기술문서

        public static string TYPE_PROJECT_DOCUMENT_KOR = "프로젝트 문서";

        public static string TYPE_TECHNICAL_DOCUMENT_KOR = "기술 문서";

        //상태
        public static string POLICY_DOCUMENT_PREPARE = "Prepare";
        public static string POLICY_DOCUMENT_STARTED = "Started";
        public static string POLICY_DOCUMENT_COMPLETED = "Completed";
    }
}
