using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Constant
{
    public class QmsConstant
    {
        public static string RELATIONSHIP_QUICK_RESPONSE { get { return "QUICK_RESPONSE"; } }

        public static string TYPE_OPEN_ISSUE { get { return "OPEN_ISSUE"; } }

        public static string TYPE_QUICK_RESPONSE { get { return "QUICK_RESPONSE"; } }

        // 봉쇄조치
        public static string TYPE_BLOCKADE { get { return "BLOCKADE"; } }

        public static string TYPE_BLOCKADE_NAME { get { return "봉쇄조치"; } }

        // 원인분석
        public static string TYPE_OCCURRENCE_CAUSE { get { return "OCCURRENCE_CAUSE"; } }

        public static string TYPE_OCCURRENCE_CAUSE_NAME { get { return "원인분석"; } }

        // 개선대책등록
        public static string TYPE_IMPROVE_COUNTERMEASURE { get { return "IMPROVE_COUNTERMEASURE"; } }

        public static string TYPE_IMPROVE_COUNTERMEASURE_NAME { get { return "개선대책"; } }

        // ERROR PROOF
        public static string TYPE_ERROR_PRROF { get { return "ERROR_PRROF"; } }

        public static string TYPE_ERROR_PRROF_NAME { get { return "ERROR_PRROF"; } }

        // LPA 부적합 등록
        public static string TYPE_LPA_UNFIT { get { return "LPA_UNFIT"; } }

        public static string TYPE_LPA_UNFIT_NAME { get { return "LPA 감사"; } }

        // LPA 부적합 대책서 
        public static string TYPE_LPA_MEASURE { get { return "LPA_MEASURE"; } }

        public static string TYPE_LPA_MEASURE_NAME { get { return "LPA 대책서작성"; } }

        // 유효성 체크
        public static string TYPE_QUICK_RESPONSE_CHECK { get { return "QUICK_RESPONSE_CHECK"; } }

        public static string TYPE_QUICK_RESPONSE_CHECK_NAME { get { return "유효성 검증"; } }

        // 표준화&Follow-Up 조치 등록
        public static string TYPE_STANDARD { get { return "STANDARD_FOLLOW"; } }

        public static string TYPE_STANDARD_NAME { get { return "표준 F/U"; } }


        //public static string TYPE_STANDARD_DOC { get { return "STANDARD_DOC"; } }

        // 교육
        public static string TYPE_WORKER_EDU { get { return "WORKER_EDU"; } }

        public static string TYPE_WORKER_EDU_NAME { get { return "작업자 교육"; } }


        public static string TYPE_BLOCKADE_ITEM { get { return "BLOCKADE_ITEM"; } }

        public static string TYPE_BLOCKADE_ITEM_MATERIAL { get { return "BLOCKADE_ITEM_MATERIAL"; } }

        public static string TYPE_BLOCKADE_ITEM_OUT_PRODUCT { get { return "BLOCKADE_ITEM_OUT_PRODUCT"; } }

        public static string TYPE_BLOCKADE_ITEM_PROCESS_PRODUCT { get { return "BLOCKADE_ITEM_PROCESS_PRODUCT"; } }

        public static string TYPE_BLOCKADE_ITEM_FINISH_PRODUCT { get { return "BLOCKADE_ITEM_FINISH_PRODUCT"; } }

        public static string TYPE_BLOCKADE_ITEM_STORAGE_PRODUCT { get { return "BLOCKADE_ITEM_STORAGE_PRODUCT"; } }

        public static string TYPE_BLOCKADE_ITEM_SHIP_PRODUCT { get { return "BLOCKADE_ITEM_SHIP_PRODUCT"; } }

        public static string NAME_BLOCKADE_ITEM_MATERIAL { get { return "원재료"; } }

        public static string NAME_BLOCKADE_ITEM_OUT_PRODUCT { get { return "외주품"; } }

        public static string NAME_BLOCKADE_ITEM_PROCESS_PRODUCT { get { return "공정품"; } }

        public static string NAME_BLOCKADE_ITEM_FINISH_PRODUCT { get { return "완성품"; } }

        public static string NAME_BLOCKADE_ITEM_STORAGE_PRODUCT { get { return "창고재고"; } }

        public static string NAME_BLOCKADE_ITEM_SHIP_PRODUCT { get { return "고객출하"; } }



        public static string TYPE_OCCURRENCE_CAUSE_ITEM { get { return "OCCURRENCE_CAUSE_ITEM"; } }

        public static string TYPE_WHY { get { return "WHY"; } }

        public static string TYPE_IMPROVE_COUNTERMEASURE_ITEM { get { return "IMPROVE_COUNTERMEASURE"; } }

        public static string TYPE_LPA_UNFIT_CHECK_ITEM { get { return "LPA_UNFIT_CHECK_ITEM"; } }

        public static string TYPE_QUICK_RESPONSE_CHECK_ITEM { get { return "QUICK_RESPONSE_CHECK_ITEM"; } }

        public static string TYPE_STANDARD_DOC_ITEM { get { return "STANDARD_DOC_ITEM "; } }

        public static string RELATIONSHIP_QUICK_MODULE { get { return "QUICK_MODULE"; } }

        public static string RELATIONSHIP_LPA { get { return "LPA_UNFIT_MEASURE"; } }

        public static string TYPE_OPEN_ISSUE_ITEM { get { return "OPEN_ISSUE_ITEM"; } }


        public static string POLICY_QMS_MODULE_PREPARE = "Prepare";
        public static string POLICY_QMS_MODULE_STARTED = "Started";
        public static string POLICY_QMS_MODULE_REVIEW = "Review";
        public static string POLICY_QMS_MODULE_COMPLETED = "Completed";
        public static string POLICY_QMS_MODULE_REJECTED = "Rejected";


        public static string POLICY_OPENISSUE_STARTED = "Started";
        public static string POLICY_OPENISSUE_COMPLETED = "Completed";

        public static string POLICY_OPENISSUE_ITEM_SUSPENSE = "Suspense";
        public static string POLICY_OPENISSUE_ITEM_UNCOMMITTED = "Uncommitted";
        public static string POLICY_OPENISSUE_ITEM_COMPLETED = "Compleated";

    }
}
