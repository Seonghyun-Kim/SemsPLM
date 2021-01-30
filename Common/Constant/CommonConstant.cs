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

        public static string TYPE_NOTICE = "NOTICE";

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
        public static int SINGLE_MAX_NUMBER = 34;
        public static string REVISION_PREFIX = "";

        public static string ACTION_YES = "Y";
        public static string ACTION_NO = "N";

        //Library
        public static string TYPE_LIBRARY = "LIBRARY";
        public static string TABLE_LIBRARY = "T_DLIBRARY";

        //Icon
        public static int DEFAULT_ICONSIZE = 20;
        public static string ICON_COMPANY = "./images/comp.png";
        public static string ICON_DEPARTMENT = "./images/depart.png";
        public static string ICON_PERSON = "./images/user.png";
        public static string ICON_DOCUMENT = "./images/doctype.png";
        public static string ICON_DOCUMENT_DETAIL = "./images/docdetail.png";

        public static string TEXT_ENCRYPT_KEY = "SemsText";
        public static string FILE_ENCRYPT_KEY = "SemsFile";

        //Auth Division
        public static string AUTH_SYSTEM = "SYSTEM";
        public static string AUTH_CUSTOM = "CUSTOM";
        public static string AUTH_DIV_OWNER = "OWNER";
        public static string AUTH_DIV_ROLE = "ROLE";
        public static string AUTH_DIV_USER = "USER";
        public static string AUTH_DIV_PUBLIC = "PUBLIC";
        public static string AUTH_DIV_MANAGER = "MANAGER";

        //Auth
        public static string AUTH_VIEW = "View";
        public static string AUTH_MODIFY = "Modify";
        public static string AUTH_DELETE = "Delete";
        public static string AUTH_DOWNLOAD = "Download";
        public static string AUTH_PROMOTE = "Promote";
        public static string AUTH_RELATION = "Relation";

        //Attribute
        public static string ATTRIBUTE_ITEM = "ITEM";
        public static string ATTRIBUTE_OEM = "OEM";
        public static string ATTRIBUTE_CAR = "CAR";
        public static string ATTRIBUTE_CUSTOMER = "CUSTOMER";
        public static string ATTRIBUTE_REASONCHANGE = "REASONCHANGE";
        public static string ATTRIBUTE_TDOC = "TDOC";
        public static string ATTRIBUTE_PDOC = "PDOC";
        public static string ATTRIBUTE_PRODUCED_PLACE = "PRODUCED_PLACE";
        public static string ATTRIBUTE_EPARTTYPE = "EPARTTYPE";
        public static string ATTRIBUTE_PSIZE = "PSIZE";
        public static string ATTRIBUTE_CARTYPE = "CARTYPE";
        public static string ATTRIBUTE_DEVSTEP = "DEVSTEP";
        public static string ATTRIBUTE_TESTITEMLIST = "TESTITEMLIST";
        public static string ATTRIBUTE_PROGRESS = "PROGRESS";


        //Doc Class
        public static string ATTRIBUTE_DOCUMENT = "일반문서";
        public static string ATTRIBUTE_PROJECT_DOCUMENT = "프로젝트 문서";
    }
}
