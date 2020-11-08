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
        public static string TYPE_ISSUE = "ISSUE";
        public static string TYPE_RISK = "RISK";
        public static string TYPE_BASE_LINE_PROJECT = "BASE_LINE_PROJECT";
        public static string TYPE_GATEVIEW_METTING = "METTING";
        public static string TYPE_GATEVIEW_CHECKLIST = "CHECKLIST";

        public static string RELATIONSHIP_WBS = "WBS";
        public static string RELATIONSHIP_MEMBER = "MEMBER";
        public static string RELATIONSHIP_ISSUE_RISK = "ISSUE/RISK";
        public static string RELATIONSHIP_BASELINE = "BASELINE";
        public static string RELATIONSHIP_GATEVIEW_METTING = "METTING";
        public static string RELATIONSHIP_GATEVIEW_CHECKLIST = "CHECKLIST";

        public static string ROLE_PM = "PM";
        public static string ROLE_PE = "PE";
        public static string ROLE_GUEST = "GUEST";

        public static string TABLE_PROJECT = "T_DPMS_PROJECT";
        public static string TABLE_PROCESS = "T_DPMS_PROCESS";

        //기본기간
        public static int INIT_DURATION = 1;
        public static int INIT_COMPLETE = 0;

        public static int DELAY = 3;

        //상태
        public static string POLICY_PROJECT_PREPARE = "Prepare";
        public static string POLICY_PROJECT_STARTED = "Started";
        public static string POLICY_PROJECT_PAUSED = "Paused";
        public static string POLICY_PROJECT_COMPLETED = "Completed";
        public static string POLICY_PROJECT_DISPOSAL = "Disposal";

        public static string POLICY_PROCESS_PREPARE = "Prepare";
        public static string POLICY_PROCESS_STARTED = "Started";
        public static string POLICY_PROCESS_PAUSED = "Paused";
        public static string POLICY_PROCESS_COMPLETED = "Completed";

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
    }
}
