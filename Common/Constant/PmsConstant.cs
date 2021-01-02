using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Constant
{
    public class PmsConstant
    {
        public static string MODULE_PMS = "PMS";

        public static string TYPE_PROJECT = "PROJECT";
        public static string TYPE_PROJECT_TEMP = "PROJECT_TEMP";
        public static string TYPE_GATE = "GATE";
        public static string TYPE_PHASE = "PHASE";
        public static string TYPE_TASK = "TASK";
        public static string TYPE_ISSUE_PROJECT = "ISSUE_PROJECT";
        public static string TYPE_ISSUE_TASK = "ISSUE_TASK";
        public static string TYPE_RISK = "RISK";
        public static string TYPE_BASE_LINE_PROJECT = "BASE_LINE_PROJECT";
        public static string TYPE_GATEVIEW_METTING = "METTING";
        public static string TYPE_GATEVIEW_CHECKLIST = "CHECKLIST";
        public static string TYPE_RELIABILITY = "RELIABILITY";
        public static string TYPE_RELIABILITY_REPORT = "RELIABILITY_REPORT";
        public static string TYPE_TOTAL = "TOTAL";

        //이슈 체크박스 타입
        public static string ATTRIBUTE_ISSUE_SPEC = "SPEC";
        public static string ATTRIBUTE_ISSUE_4M = "4M";
        public static string ATTRIBUTE_ISSUE_QUALITY = "QUALITY";
        public static string ATTRIBUTE_ISSUE_ETC = "ETC";

        public static string ATTRIBUTE_ISSUE_SPECNm = "사양";
        public static string ATTRIBUTE_ISSUE_4MNm = "4M";
        public static string ATTRIBUTE_ISSUE_QUALITYNm = "품질";
        public static string ATTRIBUTE_ISSUE_ETCNm = "기타";

        public static string ATTRIBUTE_OEM = "OEM";
        public static string ATTRIBUTE_CAR = "CAR";

        public static string RELATIONSHIP_WBS = "WBS";
        public static string RELATIONSHIP_MEMBER = "MEMBER";
        public static string RELATIONSHIP_ISSUE = "ISSUE";
        public static string RELATIONSHIP_BASELINE = "BASELINE";
        public static string RELATIONSHIP_GATEVIEW_METTING = "METTING";
        public static string RELATIONSHIP_GATEVIEW_CHECKLIST = "CHECKLIST";
        public static string RELATIONSHIP_DOC_CLASS = "DOC_CLASS";
        public static string RELATIONSHIP_DOC_MASTER = "DOC_MASTER";
        public static string RELATIONSHIP_PROJECT_EPART = "PROJECT_EPART";

        public static string ROLE_PM = "PM";
        public static string ROLE_PE = "PE";
        public static string ROLE_GUEST = "GUEST";

        public static string TABLE_PROJECT = "T_DPMS_PROJECT";
        public static string TABLE_PROCESS = "T_DPMS_PROCESS";
        public static string TABLE_ISSUE = "T_DPMS_ISSUE";
        public static string TABLE_RELIABILITY = "T_DPMS_RELIABILITY_FROM";
        public static string TABLE_RELIABILITY_REPORT = "T_DPMS_RELIABILITY_REPORT";

        //기본기간
        public static int INIT_DURATION = 1;
        public static int INIT_COMPLETE = 0;

        public static int PREPARE = 5;
        public static int DELAY = 3;
        public static string WARNING_COLOR = "#fbd46d";
        public static string DELAY_COLOR = "#c70039";

        //상태
        public static string POLICY_PROJECT_PREPARE = "Prepare";
        public static string POLICY_PROJECT_STARTED = "Started";
        public static string POLICY_PROJECT_PAUSED = "Paused";
        public static string POLICY_PROJECT_COMPLETED = "Completed";
        public static string POLICY_PROJECT_DISPOSAL = "Disposal";

        public static string POLICY_PROJECT_TEMP_EXIST = "Exist";
        public static string POLICY_PROJECT_TEMP_DISPOSAL = "Disposal";

        public static string POLICY_PROCESS_PREPARE = "Prepare";
        public static string POLICY_PROCESS_STARTED = "Started";
        public static string POLICY_PROCESS_PAUSED = "Paused";
        public static string POLICY_PROCESS_COMPLETED = "Completed";

        public static string POLICY_ISSUE_PROJECT_PREPARE = "Prepare";
        public static string POLICY_ISSUE_PROJECT_STARTED = "Started";
        public static string POLICY_ISSUE_PROJECT_COMPLETED = "Completed";

        public static string POLICY_ISSUE_TASK_PREPARE = "Prepare";
        public static string POLICY_ISSUE_TASK_BEFORE_STARTED = "BeforeStarted";
        public static string POLICY_ISSUE_TASK_STARTED = "Started";
        public static string POLICY_ISSUE_TASK_BEFORE_COMPLETED = "BeforeCompleted";
        public static string POLICY_ISSUE_TASK_REJECT = "Reject";
        public static string POLICY_ISSUE_TASK_COMPLETED = "Completed";


        //ACTION
        public static string ACTION_NEW = "N";
        public static string ACTION_ADD = "A";
        public static string ACTION_ADD_NM = "ADD";
        public static string ACTION_UPDATE = "U";
        public static string ACTION_DELETE = "D";
        public static string ACTION_DELETE_NM = "DEL";
        public static string ACTION_LEFT = "L";
        public static string ACTION_RIGHT = "R";
        public static string ACTION_NONE = "NONE";
        public static string ACTION_MODIFY = "MODIFY";
        

        // Option
        public static List<string> DISEDITABLE { get => new List<string> { "ObjName", "Dependency", "EstEndDt", "EstDuration" }; }
        public static List<string> FLOWEDITABLE { get => new List<string> { "ObjName", "Dependency", "EstStartDt", "EstEndDt", "EstDuration", "Description" }; }

        //ICON
        public static int DEFAULT_ICONSIZE = 20;
        public static string ICON_CARTYPE = "./images/cartype.png";
    }
}
