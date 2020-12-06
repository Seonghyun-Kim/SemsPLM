using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Constant
{
    public class CommonConstant
    {
        public static int RETURN_SUCCESS = 1;

        public static string DEFINE_ROLE = "ROLE";
        public static string DEFINE_TYPE = "TYPE";

        public static string TYPE_CALENDAR = "CALENDAR";
        public static string TABLE_CALENDAR = "T_DCALENDAR";

        public static string TYPE_COMPANY = "COMPANY";
        public static string TYPE_DEPARTMENT = "DEPARTMENT";
        public static string TYPE_PERSON = "PERSON";
        public static string TYPE_APPROVAL = "APPROVAL";
        public static string TYPE_APPROVAL_STEP = "APPROVAL_STEP";
        public static string TYPE_SAVE_APPROVAL = "SAVE_APPROVAL";
        public static string TYPE_APPROVAL_TASK = "APPROVAL_TASK";
        public static string TYPE_APPROVAL_APPROV = "APPROV";
        public static string TYPE_APPROVAL_AGREE = "AGREE";
        public static string TYPE_APPROVAL_DIST = "DIST";
        public static string TYPE_ROLE = "ROLE";

        public static string RELATIONSHIP_DEPARTMENT = "DEPARTMENT";
        public static string RELATIONSHIP_ROLE = "ROLE";

        public static string POLICY_APPROVAL_PREPARE = "Prepare";
        public static string POLICY_APPROVAL_STARTED = "Started";
        public static string POLICY_APPROVAL_COMPLETED = "Completed";
        public static string POLICY_APPROVAL_REJECTED = "Rejected";
        public static string POLICY_APPROVAL_TASK_PREPARE = "Prepare";
        public static string POLICY_APPROVAL_TASK_STARTED = "Started";
        public static string POLICY_APPROVAL_TASK_COMPLETED = "Completed";
        public static string POLICY_APPROVAL_TASK_REJECTED = "Rejected";

        public static string TABLE_APPROVAL = "T_DAPPROVAL";
        public static string TABLE_APPROVAL_TASK = "T_DAPPROVAL_TASK";

        //System
        public static string POLICY_TRIGGER_CLASS = "CLASS";
        public static string POLICY_TRIGGER_FUNCTION = "FUNCTION";

        public static string ACTION_PROMOTE = "P";
        public static string ACTION_REJECT = "R";

        public static string INIT_REVISION = "00";
        public static int MAX_NUMBER = 1155;
        public static string REVISION_PREFIX = "";

        //Library
        public static string TYPE_LIBRARY = "LIBRARY";
        public static string TABLE_LIBRARY = "T_DLIBRARY";

        //Icon
        public static int DEFAULT_ICONSIZE = 20;
        public static string ICON_COMPANY = "./images/comp.png";
        public static string ICON_DEPARTMENT = "./images/depart.png";
        public static string ICON_PERSON = "./images/user.png";

        public static string TEXT_ENCRYPT_KEY = "SemsText";
        public static string FILE_ENCRYPT_KEY = "SemsFile";
    }
}
