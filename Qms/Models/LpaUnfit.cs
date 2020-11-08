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
    public class LpaUnfit : QuickResponseModule, IDObject
    {// 레이어 
        public int LayerLibOID { get; set; }

        // 감사주기 
        public int AuditLibOID { get; set; }

        // 그룹군 
        public int LpaGrpLibOID { get; set; }

        // 사용구분 
        public int LpaUseLibOID { get; set; }

        // 확인공정 
        public int LpaCheckProcessLibOID { get; set; }

        // 심사자(LPA 담당자) 
        public int? LpaCheckUserOID { get; set; }

        // LPA 점검일자 
        public DateTime? LpaCheckDt { get; set; }

        // 설비명 
        public string EquipmentNm { get; set; }

        // 품번/품명 
        public int? PartOID { get; set; }

        // 담당자 
        public int? LpaUserOID { get; set; }

        // 완료요청일 
        public DateTime? FinishRequestDt { get; set; }

        // 대책서 담당자 
        public int? MeasureUserOID { get; set; }

    }

    public static class LpaUnfitRepository
    {
        public static LpaUnfit SelLpaUnfit(LpaUnfit _param)
        {
            return DaoFactory.GetData<LpaUnfit>("Qms.SelLpaUnfit", _param);
        }

        public static List<LpaUnfit> SelLpaUnfits(LpaUnfit _param)
        {
            return DaoFactory.GetList<LpaUnfit>("Qms.SelLpaUnfit", _param);
        }

        public static int InsLpaUnfit(LpaUnfit _param)
        {
            _param.CreateUs = 1;
            return DaoFactory.SetInsert("Qms.InsLpaUnfit", _param);
        }

        public static int UdtLpaUnfit(LpaUnfit _param)
        {
            _param.ModifyUs = 1;
            return DaoFactory.SetUpdate("Qms.UdtLpaUnfit", _param);
        }

    }

    public class LpaUnfitCheck : DObject, IDObject
    {      
        // 지적사항 내용 
        public string CheckPoin { get; set; }

        // 원인분석 
        public string CauseAnalysis { get; set; }

        // 개선대책 
        public string ImproveCountermeasure { get; set; }

    }

    public static class LpaUnfitCheckRepository
    {
        public static LpaUnfitCheck SelLpaUnfitCheck(LpaUnfitCheck _param)
        {
            return DaoFactory.GetData<LpaUnfitCheck>("Qms.SelLpaUnfitCheck", _param);
        }

        public static List<LpaUnfitCheck> SelLpaUnfitChecks(LpaUnfitCheck _param)
        {
            return DaoFactory.GetList<LpaUnfitCheck>("Qms.SelLpaUnfitCheck", _param);
        }

        public static int InsLpaUnfitCheck(LpaUnfitCheck _param)
        {
            _param.CreateUs = 1;
            return DaoFactory.SetInsert("Qms.InsLpaUnfitCheck", _param);
        }

        public static int UdtLpaUnfitCheck(LpaUnfitCheck _param)
        {
            _param.ModifyUs = 1;
            return DaoFactory.SetUpdate("Qms.UdtLpaUnfitCheck", _param);
        }

        public static int DelLpaUnfitCheck(LpaUnfitCheck _param)
        {
            _param.DeleteUs = 1;
            return DaoFactory.SetUpdate("Qms.DelLpaUnfitCheck", _param);
        }
    }
}
