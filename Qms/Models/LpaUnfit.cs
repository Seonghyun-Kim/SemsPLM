using Common.Factory;
using Common.Interface;
using Common.Models;
using Common.Models.File;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Qms.Models
{
    public class LpaUnfit : QuickResponseModule, IDObject, IObjectFile
    {
        public int? ModuleOID { get; set; }
        // 레이어 
        public int LayerLibOID { get; set; }

        public string LayerLibNm { get; set; }
        // 감사주기 
        public int AuditLibOID { get; set; }

        public string AuditLibNm { get; set; }
        // 그룹군 
        public int LpaGrpLibOID { get; set; }

        public string LpaGrpLibNm { get; set; }
        // 사용구분 
        public int LpaUseLibOID { get; set; }
        public string LpaUseLibNm { get; set; }

        // 확인공정 
        public int LpaCheckProcessLibOID { get; set; }
        public string LpaCheckProcessLibNm { get; set; }

        // 점검라인
        public string CheckLine { get; set; }

        // 심사자(LPA 담당자) 
        public int? LpaCheckUserOID { get; set; }

        public string LpaCheckUserNm { get; set; }

        // LPA 점검일자 
        public DateTime? LpaCheckDt { get; set; }

        // 설비명 
        public string EquipmentNm { get; set; }

        // 품번/품명 
        public int? PartOID { get; set; }

        public string PartCd { get; set; }

        public string PartNm { get; set; }

        // 담당자 
        public int? LpaUserOID { get; set; }

        public string LpaUserNm { get; set; }

        // 완료요청일 
        public DateTime? FinishRequestDt { get; set; }

        // 대책서 담당자 
        public int? MeasureUserOID { get; set; }

        public string MeasureUserNm { get; set; }

        public List<HttpPostedFileBase> Files { get; set; }

        public List<HttpFile> delFiles { get; set; }

        public List<LpaUnfitCheck> LpaUnfitChecks { get; set; }

        #region
        // 현재 Object 상태
        public string CurrentNm { get; set; }
        public int? CurrentStOID { get; set; }
        public string CurrentStNm { get; set; }
        public int? CurrentStOrd { get; set; }

        #endregion
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
            return DaoFactory.SetInsert("Qms.InsLpaUnfit", _param);
        }

        public static int UdtLpaUnfit(LpaUnfit _param)
        {
            return DaoFactory.SetUpdate("Qms.UdtLpaUnfit", _param);
        }

    }

    public class LpaUnfitCheck : DObject, IDObject
    {
        // LPA 부적합현황 OID
        public int? ModuleOID { get; set; }

        // 지적사항 내용 
        public string CheckPoin { get; set; }

        // 원인분석 
        public string CauseAnalysis { get; set; }

        // 개선대책 
        public string ImproveCountermeasure { get; set; }

        // 개선대책 담당자
        public int? ImproveCountermeasureUserOID { get; set; }

        public string ImproveCountermeasureUserNm { get; set; }

        public string IsRemove { get; set; }

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
            return DaoFactory.SetInsert("Qms.InsLpaUnfitCheck", _param);
        }

        public static int UdtLpaUnfitCheck(LpaUnfitCheck _param)
        {
            return DaoFactory.SetUpdate("Qms.UdtLpaUnfitCheck", _param);
        }

        public static int DelLpaUnfitCheck(LpaUnfitCheck _param)
        {
            return DaoFactory.SetUpdate("Qms.DelLpaUnfitCheck", _param);
        }
    }
}
