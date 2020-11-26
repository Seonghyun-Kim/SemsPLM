using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Constant
{
    public class EoConstant
    {
        public static string TYPE_CHANGE_ORDER = "CHANGE_ORDER";
        public static string TYPE_CHANGE_ORDER_KorNm = "설계변경";
        public static string TABLE_CHANGE_ORDER = "T_DCHANGE_ORDER";

        public static string TYPE_EBOM_LIST = "EBOM_LIST";
        public static string TYPE_EBOM_STATEMENT = "EBOM_STATEMENT";
        public static string TYPE_MBOM_LIST = "MBOM_LIST";
        public static string TYPE_MBOM_STATEMENT = "MBOM_STATEMENT";


        public static string TYPE_CHANGE_REQUEST = "CHANGE_REQUEST";
        public static string TYPE_CHANGE_REQUEST_KorNm = "변경요청";
        public static string TABLE_CHANGE_REQUEST = "T_DCHANGE_REQUEST";

        public static string TYPE_ECR_STATEMENT = "ECR_STATEMENT"; //변경요청내역
        public static string TYPE_ECO_RELATION = "ECO_RELATION"; //연관 ECO

        //EO 타입
        public static string TYPE_CUSTOMER_KorNm = "대여도(Customer)";
        public static string TYPE_HOUSE_KorNm = "승인도(In House)";

        public static string TYPE_CUSTOMER = "CUSTOMER";
        public static string TYPE_HOUSE = "HOUSE";

        //EO FAULT 귀책사유
        public static string FAULT_CUST_KorNm = "고객사";
        public static string FAULT_COMP_KorNm = "협력사";
        public static string FAULT_SELF_KorNm = "자체";
        public static string FAULT_EMPTY_KorNm = "내용없음";

        public static string FAULT_CUST = "CUST";
        public static string FAULT_COMP = "COMP";
        public static string FAULT_SELF = "SELP";
        public static string FAULT_EMPTY = "EMPTY";

        //EO 처리유형
        public static string PRO_M = "Migration";
        public static string PRO_N = "New";
        public static string PRO_R = "Revision";

        //EO ID 
        public static string ID_N = "정규사양";
        public static string ID_D = "임시사양";
        public static string ID_R = "기각사양";


        //EO CLASS 
        public static string CLASS_EMERGENCY_KorNm = "긴급";
        public static string CLASS_MPR_KorNm = "필수";
        public static string CLASS_MAJOR_KorNm = "일반";
        public static string CLASS_MINOR_KorNm = "정정";

        public static string CLASS_EMERGENCY = "EMERGENCY";
        public static string CLASS_MPR = "MPR";
        public static string CLASS_MAJOR = "MAJOR";
        public static string CLASS_MINOR = "MINOR";


        //재고처리
        public static string INV_NONE_KorNm = "구형재고무관";
        public static string INV_USE_KorNm = "재고소진";
        public static string INV_SCRAP_KorNm = "폐기";
        public static string INV_REWORK_KorNm = "수정";

        public static string INV_NONE = "NONE";
        public static string INV_USE = "USE";
        public static string INV_SCRAP = "SCRAP";
        public static string INV_REWORK = "REWORK";

        //시작 리비전
        public static string INIT_REIVISION = "R00";

        //기술 변경 사유
        public static string SR = "SR(법규관련)";
        public static string PI = "PI(작업성향상)";
        public static string IC = "IC(경쟁력 강화)";
        public static string CS = "CS(SPEC변경)";
        public static string CR = "CR(원가관련)";
        public static string QI = "QI(품질향상)";
        public static string ST = "ST(표준화)";
        public static string IR = "IR(초도출도)";
        public static string WD = "WD(중량감소)";
        public static string LO = "LO(국산화)";
        public static string RA = "RA(사양정리)";
        public static string GR = "GR(기타)";


    }
}
