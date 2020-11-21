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

        public PartialViewResult QuickResponseSummary(QuickResponse quickResponse)
        {
            if (quickResponse == null || quickResponse.OID == null)
            {
                throw new Exception("잘못된 호출입니다.");
            }
            return PartialView("Partitial/QuickResponseSummary", QuickResponseRepository.SelQuickResponse(quickResponse));
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


                int SetQuickModule(string Type)
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

                    return ModuleOID;
                }

                void SetBloackadeItem(int ModuleOID, string type, string name)
                {
                    DObject dobjModule = new DObject();
                    dobjModule.Type = type;
                    dobjModule.Name = name;
                    int ItemOID = DObjectRepository.InsDObject(dobjModule);

                    BlockadeItemRepository.InsBlockadeItem(new BlockadeItem() { ModuleOID = ModuleOID, OID = ItemOID });
                    
                    DRelationship dRelModule = new DRelationship();
                    dRelModule.Type = QmsConstant.RELATIONSHIP_QUICK_MODULE;
                    dRelModule.FromOID = ModuleOID;
                    dRelModule.ToOID = ItemOID;
                    DRelationshipRepository.InsDRelationshipNotOrd(dRelModule);
                }

                // 봉쇄조치
                int blockadeOID = SetQuickModule(QmsConstant.TYPE_BLOCKADE);

                if(_param.BlockadeMaterialFl == true)
                {
                    SetBloackadeItem(blockadeOID, QmsConstant.TYPE_BLOCKADE_ITEM_MATERIAL, QmsConstant.NAME_BLOCKADE_ITEM_MATERIAL);
                }

                if (_param.BlockadeOutProductFl == true)
                {
                    SetBloackadeItem(blockadeOID, QmsConstant.TYPE_BLOCKADE_ITEM_OUT_PRODUCT, QmsConstant.NAME_BLOCKADE_ITEM_OUT_PRODUCT);
                }

                if (_param.BlockadeProcessProductFl == true)
                {
                    SetBloackadeItem(blockadeOID, QmsConstant.TYPE_BLOCKADE_ITEM_PROCESS_PRODUCT, QmsConstant.NAME_BLOCKADE_ITEM_PROCESS_PRODUCT);
                }

                if (_param.BlockadeFinishProductFl == true)
                {
                    SetBloackadeItem(blockadeOID, QmsConstant.TYPE_BLOCKADE_ITEM_FINISH_PRODUCT, QmsConstant.NAME_BLOCKADE_ITEM_FINISH_PRODUCT);
                }

                if (_param.BlockadeStorageProductFl == true)
                {
                    SetBloackadeItem(blockadeOID, QmsConstant.TYPE_BLOCKADE_ITEM_STORAGE_PRODUCT, QmsConstant.NAME_BLOCKADE_ITEM_STORAGE_PRODUCT);
                }

                if (_param.BlockadeShipProductFl == true)
                {
                    SetBloackadeItem(blockadeOID, QmsConstant.TYPE_BLOCKADE_ITEM_SHIP_PRODUCT, QmsConstant.NAME_BLOCKADE_ITEM_SHIP_PRODUCT);
                }
                               
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
        public ActionResult EditQuickResponsePlan(QuickResponse quickResponse)
        {
            if(quickResponse == null || quickResponse.OID == null)
            {
                throw new Exception("잘못된 호출입니다.");
            }

            List<QuickResponseModule> list = QuickResponseModuleRepository.SelQuickResponseModules(new QuickResponseModule() { QuickOID = quickResponse.OID });

            list.ForEach(v =>
            {               
                if (v.ModuleType == QmsConstant.TYPE_BLOCKADE)
                {
                    ViewBag.Blockade = v;
                }
                else if (v.ModuleType == QmsConstant.TYPE_OCCURRENCE_CAUSE)
                {
                    ViewBag.OccurrenceCause = v;
                }
                else if (v.ModuleType == QmsConstant.TYPE_IMPROVE_COUNTERMEASURE)
                {
                    ViewBag.ImproveCountermeasure = v;
                }
                else if (v.ModuleType == QmsConstant.TYPE_ERROR_PRROF)
                {
                    ViewBag.ErrorPrrof = v;
                }
                else if (v.ModuleType == QmsConstant.TYPE_LPA_UNFIT)
                {
                    ViewBag.Lpa = v;
                }
                else if (v.ModuleType == QmsConstant.TYPE_QUICK_RESPONSE_CHECK)
                {
                    ViewBag.Check = v;
                }
                else if (v.ModuleType == QmsConstant.TYPE_STANDARD)
                {
                    ViewBag.Standard = v;
                }
                else if (v.ModuleType == QmsConstant.TYPE_WORKER_EDU)
                {
                    ViewBag.WorkerEdu = v;
                }
            });

            return View("Dialog/dlgEditQuickResponsePlan", QuickResponseRepository.SelQuickResponse(quickResponse));
        }

        public JsonResult SaveQuickResponseModule(List<QuickResponseModule> _params)
        {
            try
            {
                DaoFactory.BeginTransaction();

                _params.ForEach(v =>
                {
                    QuickResponseModuleRepository.UdtQuickResponseModule(v);
                });

                DaoFactory.Commit();
                return Json("1");
            }
            catch(Exception ex)
            {
                DaoFactory.Rollback();
                return Json(new ResultJsonModel { isError = true, resultMessage = ex.Message, resultDescription = ex.ToString() });
            }
           
        }
        #endregion

        #region -- 봉쇄조치 
        public ActionResult EditQuickBlockade(QuickResponseModule _param)
        {
            ViewBag.QuickOID = _param.QuickOID;
            return View("Dialog/dlgEditQuickBlockade", BlockadeItemRepository.SelBlockadeItems(new BlockadeItem() { ModuleOID = _param.OID }));
        }

        public JsonResult SaveQuickBlockade(List<BlockadeItem> _params)
        {
            try
            {
                DaoFactory.BeginTransaction();

                _params.ForEach(v =>
                {
                    DObjectRepository.UdtDObject(v);
                    BlockadeItemRepository.UdtBlockadeItem(v);
                });

                DaoFactory.Commit();
                return Json("1");
            }
            catch (Exception ex)
            {
                DaoFactory.Rollback();
                return Json(new ResultJsonModel { isError = true, resultMessage = ex.Message, resultDescription = ex.ToString() });
            }

        }

        #endregion

        #region -- 발생원인분석 
        public ActionResult EditQuickOccurrenceCause(QuickResponseModule _param)
        {
            ViewBag.QuickOID = _param.QuickOID;
            ViewBag.ModuleOID = _param.OID;

            Library occurrenceCauseKey = LibraryRepository.SelLibraryObject(new Library { Name = "OCCURRENCE_CAUSE" });
            List<Library> occurrenceCauseList = LibraryRepository.SelLibrary(new Library { FromOID = occurrenceCauseKey.OID });  // 검사유형
            ViewBag.occurrenceCauseList = occurrenceCauseList;

            return View("Dialog/dlgEditQuickOccurrenceCause");
        }
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
        public ActionResult QuickMyProcessDocumentList()
        {
            return View();
        }

        /// <summary>
        /// 2020.11.15
        /// 유효성 검증 등록
        /// </summary>
        /// <returns></returns>
        public ActionResult EditQuickValidation()
        {
            return View("Dialog/dlgEditQuickValidation");
        }

        #endregion

        #region -- 표준화

        #endregion

        #region -- 교육
        /// <summary>
        /// 2020.11.15
        /// 작업자 교육 등록
        /// </summary>
        /// <returns></returns>
        public ActionResult EditWorkerEducation(WorkerEdu workerEdu)
        {
            return View("Dialog/dlgEditWorkerEducation", (workerEdu.OID == null ? workerEdu : WorkerEduRepository.SelWorkerEdu(workerEdu)));
        }

        public JsonResult InsWorkerEducation(WorkerEdu _param)
        {
            int ModuleOID = 0;
            try
            {
                DaoFactory.BeginTransaction();

                DObject dobj = new DObject();
                dobj.Type = QmsConstant.TYPE_WORKER_EDU;
                dobj.Name = QmsConstant.TYPE_WORKER_EDU;
                ModuleOID = DObjectRepository.InsDObject(dobj);

                if(ModuleOID == 0)
                {
                    throw new Exception("작업자교육을 등록 할 수가 없습니다.");
                }

                _param.ModuleOID = ModuleOID;
                int returnValue = WorkerEduRepository.InsWorkerEdu(_param);

                DaoFactory.Commit();

                return Json(ModuleOID);
            }
            catch(Exception ex)
            {
                DaoFactory.Rollback();
                return Json(new ResultJsonModel { isError = true, resultMessage = ex.Message, resultDescription = ex.ToString() });
            }
        }
        #endregion
    }
}