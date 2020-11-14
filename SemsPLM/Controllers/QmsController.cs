using Common.Constant;
using Common.Factory;
using Common.Models;
using Qms.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SemsPLM.Controllers
{
    public class QmsController : Controller
    {
        // GET: Qms
        public ActionResult Index()
        {
            return View();
        }

        #region -- 오픈이슈 
        #endregion

        #region -- 신속대응 등록 & 조회 
        public ActionResult QuickResponseList()
        {
            return View();
        }

        public ActionResult EditQuickResponse(QuickResponse quickResponse)
        {
            Library oemKey = LibraryRepository.SelLibraryObject(new Library { Name = "OEM" });
            List<Library> oemList = LibraryRepository.SelLibrary(new Library { FromOID = oemKey.OID });  // OEM
            ViewBag.oemList = oemList;

            //Library plantKey = LibraryRepository.SelLibraryObject(new Library { Name = "PLANT" });
            //List<Library> plantList = LibraryRepository.SelLibrary(new Library { FromOID = oemKey.OID });  // 공장 구분으로 변경해야함
            //ViewBag.plantList = plantList;

            Library occurrenceKey = LibraryRepository.SelLibraryObject(new Library { Name = "OCCURRENCE" });
            List<Library> occurrenceList = LibraryRepository.SelLibrary(new Library { FromOID = occurrenceKey.OID });  // 검사유형
            ViewBag.occurrenceList = occurrenceList;

            Library occurrenceAreaKey = LibraryRepository.SelLibraryObject(new Library { Name = "OCCURRENCE_AREA" });
            List<Library> occurrenceAreaList = LibraryRepository.SelLibrary(new Library { FromOID = occurrenceAreaKey.OID });  // 발생처
            ViewBag.occurrenceAreaList = occurrenceAreaList;

            Library induceKey = LibraryRepository.SelLibraryObject(new Library { Name = "INDUCE" });
            List<Library> induceList = LibraryRepository.SelLibrary(new Library { FromOID = induceKey.OID });  // 유발공정
            ViewBag.induceList = induceList;

            Library defectDegreeKey = LibraryRepository.SelLibraryObject(new Library { Name = "DEFECT_DEGREE" });
            List<Library> defectDegreeList = LibraryRepository.SelLibrary(new Library { FromOID = defectDegreeKey.OID });  // 결함정도
            ViewBag.defectDegreeList = defectDegreeList;

            Library imputeKey = LibraryRepository.SelLibraryObject(new Library { Name = "IMPUTE" });
            List<Library> imputeList = LibraryRepository.SelLibrary(new Library { FromOID = imputeKey.OID });  // 귀책구분
            ViewBag.imputeList = imputeList;

            Library correctDecisionKey = LibraryRepository.SelLibraryObject(new Library { Name = "CORRECT_DECISION" });
            List<Library> correctDecisionList = LibraryRepository.SelLibrary(new Library { FromOID = correctDecisionKey.OID });  // 시정판정
            ViewBag.correctDecisionList = correctDecisionList;
 


            return View("Dialog/dlgEditQuickResponse",  (quickResponse.OID == null ? quickResponse : QuickResponseRepository.SelQuickResponse(quickResponse)));
        }

        public JsonResult SelQuickResponseGridList(QuickResponse _param)
        {
            List<QuickResponse> list = QuickResponseRepository.SelQuickResponses(_param);

            List<QuickResponseView> gridView = new List<QuickResponseView>();

            list.ForEach(v =>
            {
                gridView.Add(new QuickResponseView(v));
            });
            
            return Json(gridView);
        }

        public JsonResult InsQuickResponse(QuickResponse _param)
        {
            int result = 0;
            try
            {
                DaoFactory.BeginTransaction();

                DObject dobj = new DObject();
                dobj.Type = QmsConstant.TYPE_QUICK_RESPONSE;
                _param.Name = DateTime.Now.Ticks.ToString(); // 채번필요
                dobj.Name = _param.Name;
                result = DObjectRepository.InsDObject(dobj);

                _param.OID = result;
                int returnValue = QuickResponseRepository.InsQuickResponse(_param);


                void SetQuickModule(string Type)
                {
                    DObject dobjModule = new DObject();
                    dobjModule.Type = Type;
                    dobjModule.Name = Type;
                    int ModuleOID = DObjectRepository.InsDObject(dobjModule);
                    QuickResponseModuleRepository.InsQuickResponseModule(new QuickResponseModule() { QuickOID = result, OID = ModuleOID });

                    DRelationship dRelModule = new DRelationship();
                    dRelModule.Type = QmsConstant.RELATIONSHIP_QUICK_MODULE;
                    dRelModule.FromOID = _param.OID;
                    dRelModule.ToOID = ModuleOID;
                    DRelationshipRepository.InsDRelationshipNotOrd(dRelModule);
                }

                // 봉쇄조치
                SetQuickModule(QmsConstant.TYPE_BLOCKADE);

                // 원인분석
                SetQuickModule(QmsConstant.TYPE_OCCURRENCE_CAUSE);

                // 개선대책등록
                SetQuickModule(QmsConstant.TYPE_IMPROVE_COUNTERMEASURE);

                // ERROR PROOF
                SetQuickModule(QmsConstant.TYPE_ERROR_PRROF);

                // LPA 부적합 등록
                SetQuickModule(QmsConstant.TYPE_LPA_UNFIT);

                // LPA 부적합 대책서 
                SetQuickModule(QmsConstant.TYPE_LPA_MEASURE);

                // 유효성 체크
                SetQuickModule(QmsConstant.TYPE_QUICK_RESPONSE_CHECK);

                // 표준화&Follow-Up 조치 등록
                SetQuickModule(QmsConstant.TYPE_STANDARD);

                // 사용자 교육
                SetQuickModule(QmsConstant.TYPE_WORKER_EDU);

                DaoFactory.Commit();
            }
            catch (Exception ex)
            {
                DaoFactory.Rollback();
                return Json(new ResultJsonModel { isError = true, resultMessage = ex.Message, resultDescription = ex.ToString() });
            }
            return Json(result);
        }

        public JsonResult UdtQuickResponse(QuickResponse _param)
        {
            int result = 0;
            try
            {
                DaoFactory.BeginTransaction();

                DObject dobj = new DObject();
                dobj.Type = QmsConstant.TYPE_QUICK_RESPONSE;
                dobj.OID = _param.OID;            
                DObjectRepository.UdtDObject(dobj);

                QuickResponseRepository.UdtQuickResponse(_param);
                DaoFactory.Commit();
            }
            catch (Exception ex)
            {
                DaoFactory.Rollback();
                return Json(new ResultJsonModel { isError = true, resultMessage = ex.Message, resultDescription = ex.ToString() });
            }
            return Json(result);
        }

        #endregion

        #region -- 신속대응 일정관리 
        #endregion

        #region -- 봉쇄조치 
        #endregion

        #region -- 발생원인분석 
        #endregion

        #region -- 개선대책 
        #endregion

        #region -- Error Proof  
        #endregion

        #region -- LPA 부적합현황 
        #endregion

        #region -- LPA 대책서 
        #endregion

        #region -- 유효성 검증 
        #endregion

        #region -- 표준화
        #endregion

        #region -- 교육
        #endregion
    }
}