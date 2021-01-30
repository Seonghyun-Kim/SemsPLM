﻿using Common.Constant;
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
    public class QuickResponse : DObject, IDObject, IObjectFile
    {
        // 공장구분 
        public int? PlantLibOID { get; set; }

        public string PlantNm { get; set; }

        public int? ProjectOID { get; set; }

        // 발생유형 
        public int? OccurrenceLibOID { get; set; }

        public string OccurrenceNm { get; set; }

        // 품목
        public int? ItemLibOID { get; set; }
        public string ItemLibNm { get; set; }
        
        public string ItemNm { get; set; }
        // 품번 
        public int? PartOID { get; set; }

        public string PartNo { get; set; }

        public string PartNm { get; set; }

        public string CarCode { get; set; }

        public string PartGrpNm { get; set; }

        // 발생Lot 
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

        // 양품사진 
        public string GoodPicture { get; set; }

        public DateTime? FinishDt { get; set; }

        // 등록구분
        public int? EnrollmentLibOID { get; set; }

        public string EnrollmentLibNm { get; set; }

        #region -- Search용
        //발생유형
        public string SearchOccurrenceLibOID { get; set; }

        //발생일자
        public string SearchOccurrenceSDt { get; set; }

        public string SearchOccurrenceEDt { get; set; }

        // 작성일
        public string SearchCreateSDt { get; set; }

        public string SearchCreateEDt { get; set; }

        // 상태
        public int? SearchStatusOID { get; set; }

        public string SearchOemNm { get; set; }

        //차종 코드
        public string SearchCarCode { get; set; }

        public string SearchPartNo { get; set; }
        public string SearchPartNm { get; set; }

        public string SearchCreateUsNm { get; set; }

        public string SearchPlanUsNm { get; set; }

        // 품목 OID
        public int? SearchItemLibOID { get; set; }
        #endregion

        #region -- File
        public List<HttpPostedFileBase> Files { get; set; }

        public List<HttpFile> delFiles { get; set; }

        #endregion
    }

    public class QuickResponseView
    {
        public int? OID { get; set; }

        public string Name { get; set; }

        public DateTime? OccurrenceDt { get; set; }

        // 발생유형 
        public int? OccurrenceLibOID { get; set; }
        public string OccurrenceNm { get; set; }
        
        public string PartNm { get; set; }

        public string CarCode { get; set; }

        public string PartGrpNm { get; set; }

        public string Title { get; set; }

        public string CreateUsNm { get; set; }

        public int? PlanUserOID { get; set; }

        public string PlanUserNm { get; set; }

        // 발생Lot 
        public string LotNo { get; set; }

        // 고객사 
        public int? OemLibOID { get; set; }

        public string OemNm { get; set; }

        // 작업자 
        public int? WorkUserOID { get; set; }

        public string WorkUserNm { get; set; }

        // 불량수량 
        public int PoorCnt { get; set; }

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

        // 시정판정 
        public int? CorrectDecisionLibOID { get; set; }

        public string CorrectDecisionNm { get; set; }


        public int? ModuleBlockadeOID { get; set; }

        public string ModuleBlockadeStatusNm { get; set; }

        public int? ModuleBlockadeFl { get; set; }

        public int? ModuleBlockadeChargeUserOID { get; set; }

        public string ModuleBlockadeChargeUserNm { get; set; }

        public DateTime? ModuleBlockadeEstEndDt { get; set; }


        public int? ModuleOccurrenceCauseOID { get; set; }

        public string ModuleOccurrenceCauseStatusNm { get; set; }

        public int? ModuleOccurrenceCauseFl { get; set; }

        public int? ModuleOccurrenceCauseChargeUserOID { get; set; }

        public string ModuleOccurrenceCauseChargeUserNm { get; set; }

        public DateTime? ModuleOccurrenceCauseEstEndDt { get; set; }


        public int? ModuleImproveCountermeasureOID { get; set; }

        public string ModuleImproveCountermeasureStatusNm { get; set; }

        public int? ModuleImproveCountermeasureFl { get; set; }

        public int? ModuleImproveCountermeasureChargeUserOID { get; set; }

        public string ModuleImproveCountermeasureChargeUserNm { get; set; }

        public DateTime? ModuleImproveCountermeasureEstEndDt { get; set; }


        public int? ModuleErrorProofOID { get; set; }

        public string ModuleErrorProofStatusNm { get; set; }

        public int? ModuleErrorProofFl { get; set; }

        public int? ModuleErrorProofChargeUserOID { get; set; }

        public string ModuleErrorProofChargeUserNm { get; set; }

        public DateTime? ModuleErrorProofEstEndDt { get; set; }


        public int? ModuleLpaOID { get; set; }

        public string ModuleLpaStatusNm { get; set; }

        public int? ModuleLpaFl { get; set; }

        public int? ModuleLpaChargeUserOID { get; set; }

        public string ModuleLpaChargeUserNm { get; set; }

        public DateTime? ModuleLpaEstEndDt { get; set; }


        public int? ModuleCheckOID { get; set; }

        public string ModuleCheckStatusNm { get; set; }

        public int? ModuleCheckFl { get; set; }

        public int? ModuleCheckChargeUserOID { get; set; }

        public string ModuleCheckChargeUserNm { get; set; }

        public DateTime? ModuleCheckEstEndDt { get; set; }


        public int? ModuleStandardOID { get; set; }

        public string ModuleStandardStatusNm { get; set; }

        public int? ModuleStandardFl { get; set; }

        public int? ModuleStandardChargeUserOID { get; set; }

        public string ModuleStandardChargeUserNm { get; set; }

        public DateTime? ModuleStandardEstEndDt { get; set; }


        public int? ModuleWorkerEduOID { get; set; }

        public string ModuleWorkerEduStatusNm { get; set; }

        public int? ModuleWorkerEduFl { get; set; }

        public int? ModuleWorkerEduChargeUserOID { get; set; }

        public string ModuleWorkerEduChargeUserNm { get; set; }

        public DateTime? ModuleWorkerEduEstEndDt { get; set; }

        public string StatusNm { get; set; }

        public DateTime? FinishDt { get; set; }

        // 등록구분
        public int? EnrollmentLibOID { get; set; }

        public string EnrollmentLibNm { get; set; }

        public DateTime? CreateDt { get; set; }

        // 품목
        public int? ItemLibOID { get; set; }
        public string ItemLibNm { get; set; }
        public string ItemNm { get; set; }
        public QuickResponseView(QuickResponse response)
        {
            this.OID = response.OID;
            this.Name = response.Name;
            this.OccurrenceDt = response.OccurrenceDt;
            this.OccurrenceNm = response.OccurrenceNm;
            this.PoorCnt = response.PoorCnt;
            this.PartNm = response.PartNm;
            this.CarCode = response.CarCode;
            this.PartGrpNm = response.PartGrpNm;
            this.OemLibOID = response.OemLibOID;
            this.OemNm = response.OemNm;
            this.OccurrenceLibOID = response.OccurrenceLibOID;
            this.OccurrenceNm = response.OccurrenceNm;
            this.OccurrenceAreaLibOID = response.OccurrenceAreaLibOID;
            this.OccurrenceAreaNm = response.OccurrenceAreaNm;
            this.OccurrencePlace = response.OccurrencePlace;
            this.LotNo = response.LotNo;
            this.WorkUserNm = response.WorkUserNm;
            this.EnrollmentLibOID = response.EnrollmentLibOID;
            this.EnrollmentLibNm = response.EnrollmentLibNm;
            this.ImputeLibOID = response.ImputeLibOID;
            this.ImputeNm = response.ImputeNm;
            this.ItemLibOID = response.ItemLibOID;
            this.ItemLibNm = response.ItemLibNm;
            this.CreateDt = response.CreateDt;
            this.Title = response.Title;
            this.CreateUsNm = response.CreateUsNm;
            this.PlanUserOID = response.PlanUserOID;
            this.PlanUserNm = response.PlanUserNm;
            this.FinishDt = response.FinishDt;
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
            return DaoFactory.SetInsert("Qms.InsQuickResponse", _param);
        }

        public static int UdtQuickResponse(QuickResponse _param)
        {
            return DaoFactory.SetUpdate("Qms.UdtQuickResponse", _param);
        }

        public static int DelQuickResponse(QuickResponse _param)
        {
            return DaoFactory.SetUpdate("Qms.DelQuickResponse", _param);
        }

    }

    public class QuickResponseModule : DObject, IDObject
    {
        // 신속대응 OID 
        public int? QuickOID { get; set; }

        // 모듈 사용유무 
        public int? ModuleFl { get; set; }

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
            return DaoFactory.SetInsert("Qms.InsQuickResponseModule", _param);
        }

        public static int UdtQuickResponseModule(QuickResponseModule _param)
        {
            return DaoFactory.SetUpdate("Qms.UdtQuickResponseModule", _param);
        }
    }
}
