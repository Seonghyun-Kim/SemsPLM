using Common.Factory;
using Common.Interface;
using Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qms.Models
{
    public class QmsCheck : DObject, IDObject
    {
        // 신속대응 OID
        public int? QuickOID { get; set; }

        public int? ModuleOID { get; set; }
        // N차 
        public int? Cnt { get; set; }

        // 검증시작일
        public DateTime? CheckSt { get; set; }

        // 검증완료일
        public DateTime? CheckEt { get; set; }

        // 검증일 
        public DateTime? CheckDt { get; set; }

        // 담당자
        public int? CheckUserOID { get; set; }

        // 판정 
        public int? CheckFl { get; set; }

        // 검증내용요약 
        public string CheckDetail { get; set; }

        // 종료여부 
        public int? FinishFl { get; set; }

        // 종료사유 
        public string FinishDetail { get; set; }

        #region -- 저장용
        // 1차, 2차, 3차 유효성 검증 등록
        public List<QmsCheck> QmsCheckList { get; set; }
        #endregion

        #region -- Search & View
        public string CheckUserNm { get; set; }
        #endregion
    }

    public static class QmsCheckRepository
    {
        public static QmsCheck SelQmsCheck(QmsCheck _param)
        {
            return DaoFactory.GetData<QmsCheck>("Qms.SelQmsCheck", _param);
        }

        public static List<QmsCheck> SelQmsChecks(QmsCheck _param)
        {
            return DaoFactory.GetList<QmsCheck>("Qms.SelQmsCheck", _param);
        }

        public static int InsQmsCheck(QmsCheck _param)
        {
            return DaoFactory.SetInsert("Qms.InsQmsCheck", _param);
        }

        public static int UdtQmsCheck(QmsCheck _param)
        {
            return DaoFactory.SetUpdate("Qms.UdtQmsCheck", _param);
        }

    }
}
