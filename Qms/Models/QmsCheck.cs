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
        // N차 
        public int? Cnt { get; set; }

        // 검증시작일
        public DateTime? ConfirmSt { get; set; }

        // 검증완료일
        public DateTime? ConfirmEt { get; set; }

        // 검증일 
        public DateTime? CheckDt { get; set; }

        // 판정 
        public bool? CheckFl { get; set; }

        // 검증내용요약 
        public string CheckDetail { get; set; }

        // 종료여부 
        public bool? FinishFl { get; set; }

        // 종료사유 
        public string FinishDetail { get; set; }
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
            _param.CreateUs = 1;
            return DaoFactory.SetInsert("Qms.InsQmsCheck", _param);
        }

        public static int UdtQmsCheck(QmsCheck _param)
        {
            _param.ModifyUs = 1;
            return DaoFactory.SetUpdate("Qms.UdtQmsCheck", _param);
        }

    }
}
