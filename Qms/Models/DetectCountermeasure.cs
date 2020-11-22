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
    public class DetectCounterMeasure : QuickResponseModule, IDObject
    {
        public int? ModuleOID { get { return OID; } set { OID = value; } }
        // 검출장소-제조공정 
        public bool DetectM { get; set; }

        // 제조공정 상세내용 
        public string DetectMDetail { get; set; }

        // 검출장소-출하단계 
        public bool DetectS { get; set; }

        // 제조공정 상세내용 
        public string DetectSDetail { get; set; }

        // 검출장소-품질 
        public bool DetectQ { get; set; }

        // 제조공정 상세내용 
        public string DetectQDetail { get; set; }

        // 검출장소-기타 
        public bool DetectE { get; set; }

        // 제조공정 상세내용 
        public string DetectEDetail { get; set; }

        // 유출원인1 
        public string LeakCause1 { get; set; }

        // 유출원인2 
        public string LeakCause2 { get; set; }

        // 유출원인3 
        public string LeakCause3 { get; set; }

        // 검출일 
        public DateTime? DetectDt { get; set; }

        // 검출대책 
        public string Measure { get; set; }
    }

    public static class DetectCounterMeasureRepository
    {
        public static DetectCounterMeasure SelDetectCounterMeasure(DetectCounterMeasure _param)
        {
            return DaoFactory.GetData<DetectCounterMeasure>("Qms.SelDetectCounterMeasure", _param);
        }

        public static List<DetectCounterMeasure> SelDetectCounterMeasures(DetectCounterMeasure _param)
        {
            return DaoFactory.GetList<DetectCounterMeasure>("Qms.SelDetectCounterMeasure", _param);
        }

        public static int UdtDetectCounterMeasure(DetectCounterMeasure _param)
        {
            return DaoFactory.SetUpdate("Qms.UdtDetectCounterMeasure", _param);
        }
    }
}
