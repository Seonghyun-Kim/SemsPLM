using Common.Constant;
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
    public class QuickResponse : DObject, IDObject
    {
        // 공장구분 
        public int? PlantLibOID { get; set; }

        public string PlantNm { get; set; }

        // 발생유형 
        public int? OccurrenceLibOID { get; set; }

        public string OccurrenceNm { get; set; }

        // 품번 
        public int? PartOID { get; set; }

        public string PartNo { get; set; }

        public string PartNm { get; set; }

        public string CarCode { get; set; }

        public string PartGrpNm { get; set; }

        // LOT NO 
        public string LotNo { get; set; }

        // 고객사 
        public int? OemLibOID { get; set; }

        public string OemNm { get; set; }

        // 작업자 
        public int? WorkUserOID { get; set; }

        public string WorkUserNm { get; set; }
        

        // 작성일자 
        public DateTime? WriteDt { get; set; }

        // 불량수량 
        public int PoorCnt { get; set; }

        public string Title { get; set; }

        // 발생처 
        public int? OccurrenceAreaLibOID { get; set; }

        public string OccurrenceAreaNm { get; set; }

        // 유발공정 
        public int? InduceLibOID { get; set; }

        public string InduceNm { get; set; }

        // 결함정도 
        public int? DefectDegreeLibOID { get; set; }

        public string DefectDegreeNm { get; set; }

        // 귀책구분 
        public int? ImputeLibOID { get; set; }

        public string ImputeNm { get; set; }

        // 귀책처(자체) 
        public int? ImputeDepartmentOID { get; set; }

        public string ImputeDepartmentNm { get; set; }

        // 귀책처(협력사) 
        public int? ImputeSupplierOID { get; set; }

        public string ImputeSupplierNm { get; set; }

        // 요약
        public string Summary { get; set; }

        // 불량내용상세 
        public string PoorDetail { get; set; }

        // 재발여부 
        public bool RecurrenceFl { get; set; }

        // 발생장소 
        public string OccurrencePlace { get; set; }

        // 발생일자 
        public DateTime? OccurrenceDt { get; set; }

        // 시정판정 
        public int? CorrectDecisionLibOID { get; set; }

        public string CorrectDecisionNm { get; set; }

        // 대책서회신요구일자 
        public DateTime? MeasureResponseDt { get; set; }

        // QA(의견) 
        public string Qa { get; set; }

        // 봉쇄조치-원재료 
        public bool? BlockadeMaterialFl { get; set; }

        // 봉쇄조치-외주품 
        public bool? BlockadeOutProductFl { get; set; }

        // 봉쇄조치-공정품 
        public bool? BlockadeProcessProductFl { get; set; }

        // 봉쇄조치-완성품 
        public bool? BlockadeFinishProductFl { get; set; }

        // 봉쇄조치-창고재고 
        public bool? BlockadeStorageProductFl { get; set; }

        // 봉쇄조치-고객출하 
        public bool? BlockadeShipProductFl { get; set; }

        // 일정계획담당자 
        public int? PlanUserOID { get; set; }

        public string PlanUserNm { get; set; }

        // 고품사진 
        public string PoorPicture { get; set; }
    }

    public class QuickResponseView
    {
        public int? OID { get; set; }

        public string Name { get; set; }

        public DateTime? OccurrenceDt { get; set; }

        public string OccurrenceNm { get; set; }
        
        public string PartNm { get; set; }

        public string CarCode { get; set; }

        public string PartGrpNm { get; set; }

        public string Title { get; set; }

        public string CreateUsNm { get; set; }

        public string PlanUserNm { get; set; }


        public int? ModuleBlockadeOID { get; set; }

        public string ModuleBlockadeStatusNm { get; set; }

        public bool ModuleBlockadeFl { get; set; }

        public int? ModuleBlockadeChargeUserOID { get; set; }

        public string ModuleBlockadeChargeUserNm { get; set; }

        public DateTime? ModuleBlockadeEstEndDt { get; set; }


        public int? ModuleOccurrenceCauseOID { get; set; }

        public string ModuleOccurrenceCauseStatusNm { get; set; }

        public bool ModuleOccurrenceCauseFl { get; set; }

        public int? ModuleOccurrenceCauseChargeUserOID { get; set; }

        public string ModuleOccurrenceCauseChargeUserNm { get; set; }

        public DateTime? ModuleOccurrenceCauseEstEndDt { get; set; }


        public int? ModuleImproveCountermeasureOID { get; set; }

        public string ModuleImproveCountermeasureStatusNm { get; set; }

        public bool ModuleImproveCountermeasureFl { get; set; }

        public int? ModuleImproveCountermeasureChargeUserOID { get; set; }

        public string ModuleImproveCountermeasureChargeUserNm { get; set; }

        public DateTime? ModuleImproveCountermeasureEstEndDt { get; set; }


        public int? ModuleErrorPrrofOID { get; set; }

        public string ModuleErrorPrrofStatusNm { get; set; }

        public bool ModuleErrorPrrofFl { get; set; }

        public int? ModuleErrorPrrofChargeUserOID { get; set; }

        public string ModuleErrorPrrofChargeUserNm { get; set; }

        public DateTime? ModuleErrorPrrofEstEndDt { get; set; }


        public int? ModuleLpaOID { get; set; }

        public string ModuleLpaStatusNm { get; set; }

        public bool ModuleLpaFl { get; set; }

        public int? ModuleLpaChargeUserOID { get; set; }

        public string ModuleLpaChargeUserNm { get; set; }

        public DateTime? ModuleLpaEstEndDt { get; set; }


        public int? ModuleCheckOID { get; set; }

        public string ModuleCheckStatusNm { get; set; }

        public bool ModuleCheckFl { get; set; }

        public int? ModuleCheckChargeUserOID { get; set; }

        public string ModuleCheckChargeUserNm { get; set; }

        public DateTime? ModuleCheckEstEndDt { get; set; }


        public int? ModuleStandardOID { get; set; }

        public string ModuleStandardStatusNm { get; set; }

        public bool ModuleStandardFl { get; set; }

        public int? ModuleStandardChargeUserOID { get; set; }

        public string ModuleStandardChargeUserNm { get; set; }

        public DateTime? ModuleStandardEstEndDt { get; set; }


        public int? ModuleWorkerEduOID { get; set; }

        public string ModuleWorkerEduStatusNm { get; set; }

        public bool ModuleWorkerEduFl { get; set; }

        public int? ModuleWorkerEduChargeUserOID { get; set; }

        public string ModuleWorkerEduChargeUserNm { get; set; }

        public DateTime? ModuleWorkerEduEstEndDt { get; set; }

        public QuickResponseView(QuickResponse response)
        {
            this.OID = response.OID;
            this.Name = response.Name;
            this.OccurrenceDt = response.OccurrenceDt;
            this.OccurrenceNm = response.OccurrenceNm;
            this.PartNm = response.PartNm;
            this.CarCode = response.CarCode;
            this.PartGrpNm = response.PartGrpNm;
            this.Title = response.Title;
            this.CreateUsNm = response.CreateUsNm;
            this.PlanUserNm = response.PlanUserNm;
        }
    }

    public static class QuickResponseRepository
    {
        public static QuickResponse SelQuickResponse(QuickResponse _param)
        {
            _param.Type = QmsConstant.TYPE_QUICK_RESPONSE;
            QuickResponse quickResponse = DaoFactory.GetData<QuickResponse>("Qms.SelQuickResponse", _param);

            quickResponse.BPolicy = BPolicyRepository.SelBPolicy(new BPolicy { Type = QmsConstant.TYPE_QUICK_RESPONSE, OID = quickResponse.BPolicyOID }).First();
            return quickResponse;
        }

        public static List<QuickResponse> SelQuickResponses(QuickResponse _param)
        {
            return DaoFactory.GetList<QuickResponse>("Qms.SelQuickResponse", _param);
        }

        public static int InsQuickResponse(QuickResponse _param)
        {
            _param.CreateUs = 1;
            return DaoFactory.SetInsert("Qms.InsQuickResponse", _param);
        }

        public static int UdtQuickResponse(QuickResponse _param)
        {
            _param.ModifyUs = 1;
            return DaoFactory.SetUpdate("Qms.UdtQuickResponse", _param);
        }

        public static int DelQuickResponse(QuickResponse _param)
        {
            _param.DeleteUs = 1;
            return DaoFactory.SetUpdate("Qms.DelQuickResponse", _param);
        }

    }

    public class QuickResponseModule : DObject, IDObject
    {
        // 신속대응 OID 
        public int? QuickOID { get; set; }

        // 모듈 사용유무 
        public bool ModuleFl { get; set; }

        public string ModuleType { get; set; }

        // 완료예정일 
        public DateTime? EstEndDt { get; set; }

        // 처리담당자 
        public int? ChargeUserOID { get; set; }

        public string ChargeUserNm { get; set; }
    }

    public static class QuickResponseModuleRepository
    {
        public static QuickResponseModule SelQuickResponseModule(QuickResponseModule _param)
        {
            return DaoFactory.GetData<QuickResponseModule>("Qms.SelQuickResponseModule", _param);
        }

        public static List<QuickResponseModule> SelQuickResponseModules(QuickResponseModule _param)
        {
            return DaoFactory.GetList<QuickResponseModule>("Qms.SelQuickResponseModule", _param);
        }

        public static int InsQuickResponseModule(QuickResponseModule _param)
        {
            _param.CreateUs = 1;
            return DaoFactory.SetInsert("Qms.InsQuickResponseModule", _param);
        }

        public static int UdtQuickResponseModule(QuickResponseModule _param)
        {
            _param.ModifyUs = 1;
            return DaoFactory.SetUpdate("Qms.UdtQuickResponseModule", _param);
        }
    }
}
