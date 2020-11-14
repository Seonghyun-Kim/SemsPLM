using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Constant
{
    public class QmsConstant
    {
        public static string TYPE_OPEN_ISSUE { get { return "OPEN_ISSUE"; } }

        public static string TYPE_QUICK_RESPONSE { get { return "QUICK_RESPONSE"; } }

        // 봉쇄조치
        public static string TYPE_BLOCKADE { get { return "BLOCKADE"; } }

        // 원인분석
        public static string TYPE_OCCURRENCE_CAUSE { get { return "OCCURRENCE_CAUSE"; } }

        // 개선대책등록
        public static string TYPE_IMPROVE_COUNTERMEASURE { get { return "IMPROVE_COUNTERMEASURE"; } }

        // ERROR PROOF
        public static string TYPE_ERROR_PRROF { get { return "ERROR_PRROF"; } }

        // LPA 부적합 등록
        public static string TYPE_LPA_UNFIT { get { return "LPA_UNFIT"; } }

        // LPA 부적합 대책서 
        public static string TYPE_LPA_MEASURE { get { return "LPA_MEASURE"; } }

        // 유효성 체크
        public static string TYPE_QUICK_RESPONSE_CHECK { get { return "QUICK_RESPONSE_CHECK"; } }

        // 표준화&Follow-Up 조치 등록
        public static string TYPE_STANDARD { get { return "STANDARD_FOLLOW"; } }


        //public static string TYPE_STANDARD_DOC { get { return "STANDARD_DOC"; } }

        // 교육
        public static string TYPE_WORKER_EDU { get { return "WORKER_EDU"; } }

        public static string TYPE_BLOCKADE_ITEM { get { return "BLOCKADE_ITEM"; } }

        public static string TYPE_OCCURRENCE_CAUSE_ITEM { get { return "OCCURRENCE_CAUSE_ITEM"; } }

        public static string TYPE_WHY { get { return "WHY"; } }

        public static string IMPROVE_COUNTERMEASURE_ITEM { get { return "IMPROVE_COUNTERMEASURE"; } }

        public static string TYPE_LPA_UNFIT_CHECK_ITEM { get { return "LPA_UNFIT_CHECK_ITEM"; } }

        public static string TYPE_QUICK_RESPONSE_CHECK_ITEM { get { return "QUICK_RESPONSE_CHECK_ITEM"; } }

        public static string TYPE_STANDARD_DOC_ITEM { get { return "STANDARD_DOC_ITEM "; } }

        public static string RELATIONSHIP_QUICK_MODULE { get { return "QUICK_MODULE"; } }
    }
}
