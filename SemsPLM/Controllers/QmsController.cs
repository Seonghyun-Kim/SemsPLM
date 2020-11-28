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
            ViewBag.occurrenceCauseLibList = occurrenceCauseList;

            List<OccurrenceCauseItem> OccurrenceCauseItems = OccurrenceCauseItemRepository.SelOccurrenceCauseItems(new OccurrenceCauseItem() { ModuleOID = _param.OID });
            OccurrenceCauseItems.ForEach(v =>
            {
                v.OccurrenceWhys = OccurrenceWhyRepository.SelOccurrenceWhys(new OccurrenceWhy() { CauseOID = v.OID });
            });
            ViewBag.OccurrenceCauseItems = OccurrenceCauseItems;
            
            DetectCounterMeasure detectCounterMeasure = DetectCounterMeasureRepository.SelDetectCounterMeasure(new DetectCounterMeasure() { ModuleOID = _param.OID });
            ViewBag.DetectCounterMeasure = detectCounterMeasure == null ? new DetectCounterMeasure() { ModuleOID = _param.OID } : detectCounterMeasure;

            return View("Dialog/dlgEditQuickOccurrenceCause");
        }

        public JsonResult SaveQuickOccurrenceCause(List<OccurrenceCauseItem> occurrenceCauses, DetectCounterMeasure measure)
        {
            try
            {
                DaoFactory.BeginTransaction();

                occurrenceCauses.ForEach(v =>
                {
                    int? CauseOID = v.OID;
                    if (v.OID == null)
                    {
                        DObject dobj = new DObject();
                        dobj.Type = QmsConstant.TYPE_OCCURRENCE_CAUSE_ITEM;
                        dobj.Name = QmsConstant.TYPE_OCCURRENCE_CAUSE_ITEM;
                        v.OID = DObjectRepository.InsDObject(dobj);
                        CauseOID = v.OID;
                        OccurrenceCauseItemRepository.InsOccurrenceCauseItem(v);
                    }
                    else
                    {
                        OccurrenceCauseItemRepository.UdtOccurrenceCauseItem(v);
                    }

                    //int WhyCnt = 1;
                    v.OccurrenceWhys.ForEach(w =>
                    {
                        if (w.OID == null)
                        {
                            DObject dobj = new DObject();
                            dobj.Type = QmsConstant.TYPE_WHY;
                            dobj.Name = "WHY";
                            w.OID = DObjectRepository.InsDObject(dobj);
                            w.CauseOID = v.OID;

                            OccurrenceWhyRepository.InsOccurrenceWhy(w);
                        }
                        else
                        {
                            if(w.IsRemove == "Y")
                            {      
                                w.DeleteUs = 73;
                                DObjectRepository.DelDObject(w);
                            }
                            else
                            {
                                w.ModifyUs = 73;
                                OccurrenceWhyRepository.UdtOccurrenceWhy(w);
                                DObjectRepository.UdtDObject(w);
                            }
                        }
                    });
                });

                DetectCounterMeasureRepository.UdtDetectCounterMeasure(measure);

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

        #region -- 개선대책 
        public ActionResult EditQuickImproveCounterMeasure(QuickResponseModule _param)
        {
            ViewBag.QuickOID = _param.QuickOID;
            ViewBag.ModuleOID = _param.OID;

            ViewBag.ImproveCounterMeasures = ImproveCounterMeasureRepository.SelImproveCounterMeasures(new ImproveCounterMeasure() { ModuleOID = _param.OID });

            return View("Dialog/dlgEditQuickImproveCounterMeasure");
        }

        public JsonResult SavetQuickImproveCounterMeasure(List<ImproveCounterMeasure> _params)
        {
            try
            {
                DaoFactory.BeginTransaction();

                _params.ForEach(v =>
                {
                    if(v.OID == null)
                    {
                        DObject dobj = new DObject();
                        dobj.Type = QmsConstant.TYPE_IMPROVE_COUNTERMEASURE_ITEM;
                        dobj.Name = "WHY";
                        v.OID = DObjectRepository.InsDObject(dobj);
                       
                        ImproveCounterMeasureRepository.InsImproveCounterMeasure(v);
                    }
                    else
                    {
                        if (v.IsRemove == "Y")
                        {
                            v.DeleteUs = 73;
                            DObjectRepository.DelDObject(v);
                        }
                        else
                        {
                            v.ModifyUs = 73;
                            ImproveCounterMeasureRepository.UdtImproveCounterMeasure(v);
                            DObjectRepository.UdtDObject(v);
                        }
                    }
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

        #region -- Error Proof  
        /// <summary>
        /// 2020.11.15
        /// 작업자 교육 등록
        /// </summary>
        /// <returns></returns>
        public ActionResult EditErrorProof(ErrorPrrof _param)
        {
            ViewBag.QuickOID = _param.QuickOID;
            ViewBag.ModuleOID = _param.OID;

            return View("Dialog/dlgEditErrorProof", _param);
        }

        public JsonResult InsErrorProof(ErrorPrrof _param)
        {
            int returnValue = 0;
            try
            {
                DaoFactory.BeginTransaction();

                ErrorPrrof errorPrrof = new ErrorPrrof()
                {
                    ModuleOID = _param.ModuleOID,
                    EstDt = _param.EstDt,
                    ActDt = _param.ActDt,
                    CheckDetail = _param.CheckDetail,
                    CheckUserOID = _param.CheckUserOID,
                };

                returnValue = ErrorPrrofRepository.InsErrorPrrof(errorPrrof);

                DaoFactory.Commit();

                return Json(returnValue);
            }
            catch (Exception ex)
            {
                DaoFactory.Rollback();
                return Json(new ResultJsonModel { isError = true, resultMessage = ex.Message, resultDescription = ex.ToString() });
            }
        }
        #endregion

        #region -- LPA 부적합현황 
        public ActionResult EditLPAIncongruity(LpaUnfit _param)
        {
            ViewBag.QuickOID = _param.QuickOID;
            ViewBag.ModuleOID = _param.OID;

            Library LayerLibKey = LibraryRepository.SelLibraryObject(new Library { Name = "LAYER" });
            List<Library> LayerLibList = LibraryRepository.SelLibrary(new Library { FromOID = LayerLibKey.OID });  // Layer
            ViewBag.LayerLibList = LayerLibList;

            Library AuditLibKey = LibraryRepository.SelLibraryObject(new Library { Name = "AUDIT" });
            List<Library> AuditLibList = LibraryRepository.SelLibrary(new Library { FromOID = AuditLibKey.OID });  // 감사주기
            ViewBag.AuditLibList = AuditLibList;

            Library LpaGrpLibKey = LibraryRepository.SelLibraryObject(new Library { Name = "LPA_GROUP" });
            List<Library> LpaGrpLibList = LibraryRepository.SelLibrary(new Library { FromOID = LpaGrpLibKey.OID });  // 그룹군
            ViewBag.LpaGrpLibList = LpaGrpLibList;

            Library LpaUseLibKey = LibraryRepository.SelLibraryObject(new Library { Name = "LPA_USE" });
            List<Library> LpaUseLibList = LibraryRepository.SelLibrary(new Library { FromOID = LpaUseLibKey.OID });  // 사용구분
            ViewBag.LpaUseLibList = LpaUseLibList;

            Library LpaCheckProcessLibKey = LibraryRepository.SelLibraryObject(new Library { Name = "LPA_CHECK_PROCESS" });
            List<Library> LpaCheckProcessLibList = LibraryRepository.SelLibrary(new Library { FromOID = LpaCheckProcessLibKey.OID });  // 확인공정
            ViewBag.LpaCheckProcessLibList = LpaCheckProcessLibList;

            return View("Dialog/dlgEditLPAIncongruity", _param);
        }

        public JsonResult InsLPAIncongruity(LpaUnfit _param)
        {
            int? resultValue = null;
            try
            {
                DaoFactory.BeginTransaction();

                LpaUnfit lpaUnfit = new LpaUnfit()
                {
                    QuickOID = _param.QuickOID,
                    ModuleOID = _param.ModuleOID,
                    LayerLibOID = _param.LayerLibOID,
                    AuditLibOID = _param.AuditLibOID,
                    LpaGrpLibOID = _param.LpaGrpLibOID,
                    LpaUseLibOID = _param.LpaUseLibOID,
                    LpaCheckProcessLibOID = _param.LpaCheckProcessLibOID,
                    LpaCheckUserOID = _param.LpaCheckUserOID,
                    LpaCheckDt = _param.LpaCheckDt,
                    EquipmentNm = _param.EquipmentNm,
                    PartOID = _param.PartOID,
                    LpaUserOID = _param.LpaUserOID,
                    FinishRequestDt = _param.FinishRequestDt,
                    MeasureUserOID = _param.MeasureUserOID
                };

                LpaUnfitRepository.InsLpaUnfit(lpaUnfit);

                // LPA 부적합현황 지적사항
                LpaUnfitCheck lpaUnfitCheck = new LpaUnfitCheck()
                {
                    OID = _param.LpaUnfitCheck.OID,
                    ModuleOID = lpaUnfit.ModuleOID,
                    CheckPoin = _param.LpaUnfitCheck.CheckPoin
                };

                // LPA 부적합현황 지적사항 등록
                if (lpaUnfitCheck.OID == null)
                {
                    DObject dobj = new DObject();
                    dobj.Type = QmsConstant.TYPE_LPA_UNFIT_CHECK_ITEM;
                    dobj.Name = "LPA_UNFIT_CHECK_ITEM_"+ lpaUnfit.ModuleOID;
                    lpaUnfitCheck.OID = DObjectRepository.InsDObject(dobj);

                    resultValue = LpaUnfitCheckRepository.InsLpaUnfitCheck(lpaUnfitCheck);
                }

                DaoFactory.Commit();
                return Json(resultValue);
            }
            catch(Exception ex)
            {
                DaoFactory.Rollback();
                return Json(new ResultJsonModel { isError = true, resultMessage = ex.Message, resultDescription = ex.ToString() });
            }
        }
        #endregion

        #region -- LPA 대책서 
        public ActionResult EditLPAMeasure(LpaMeasure _param)
        {
            ViewBag.QuickOID = _param.QuickOID;
            ViewBag.ModuleOID = _param.OID;

            List<QuickResponseModule> list = QuickResponseModuleRepository.SelQuickResponseModules(new QuickResponseModule() { QuickOID = _param.QuickOID });

            QuickResponseModule lpaUnfitModule = (from x in list
                                                 where x.Type == QmsConstant.TYPE_LPA_UNFIT
                                                 select x
                                                 ).FirstOrDefault();
            if(lpaUnfitModule != null)
            {
                ViewBag.LpaUnfitDetail = LpaUnfitRepository.SelLpaUnfit(new LpaUnfit() { ModuleOID = lpaUnfitModule.OID });
            }

            return View("Dialog/dlgEditLPAMeasure", _param);
        }

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
        public ActionResult EditQuickValidation(QmsCheck _param)
        {
            ViewBag.QuickOID = _param.QuickOID;
            ViewBag.ModuleOID = _param.OID;

            return View("Dialog/dlgEditQuickValidation", _param);
        }

        /// <summary>
        /// 2020.11.21
        /// 유효성 검증 저장
        /// </summary>
        /// <param name="_param"></param>
        /// <returns></returns>
        public JsonResult InsQuickValidation(QmsCheck _param)
        {
            int ModuleOID = 0;
            int returnValue = 0;
            try
            {
                DaoFactory.BeginTransaction();

                foreach (QmsCheck qmsCheck in _param.QmsCheckList)
                {
                    /*qmsCheck.OID = qmsCheck.Cnt;
                    qmsCheck.ModuleOID = qmsCheck.Cnt;*/

                    if (qmsCheck.OID == null)
                    {
                        returnValue = QmsCheckRepository.InsQmsCheck(qmsCheck);
                    }
                    else
                    {
                        QmsCheckRepository.UdtQmsCheck(qmsCheck);
                    }
                }

                DaoFactory.Commit();

                return Json(returnValue);
            }
            catch (Exception ex)
            {
                DaoFactory.Rollback();
                return Json(new ResultJsonModel { isError = true, resultMessage = ex.Message, resultDescription = ex.ToString() });
            }
        }

        #endregion

        #region -- 표준화
        /// <summary>
        /// 2020.11.15
        /// 작업자 교육 등록
        /// </summary>
        /// <returns></returns>
        public ActionResult EditStandardFollowUp(StandardDoc _param)
        {
            ViewBag.QuickOID = _param.QuickOID;
            ViewBag.ModuleOID = _param.OID;

            return View("Dialog/dlgEditStandardFollowUp", _param);
        }

        public JsonResult InsStandardFollowUp(StandardDoc _param)
        {
            int ModuleOID = 0;
            int returnValue = 0;
            try
            {
                DaoFactory.BeginTransaction();

                foreach (StandardDoc standardDoc in _param.StandardFollowUpList)
                {
                    /*qmsCheck.OID = qmsCheck.Cnt;
                    qmsCheck.ModuleOID = qmsCheck.Cnt;*/

                    if (standardDoc.OID == null)
                    {
                        returnValue = StandardDocRepository.InsStandardDoc(_param);
                    }
                    else
                    {
                        StandardDocRepository.UdtStandardDoc(_param);
                    }
                }

                DaoFactory.Commit();

                return Json(ModuleOID);
            }
            catch (Exception ex)
            {
                DaoFactory.Rollback();
                return Json(new ResultJsonModel { isError = true, resultMessage = ex.Message, resultDescription = ex.ToString() });
            }
        }
        #endregion

        #region -- 교육
        /// <summary>
        /// 2020.11.15
        /// 작업자 교육 등록
        /// </summary>
        /// <returns></returns>
        public ActionResult EditWorkerEducation(WorkerEdu _param)
        {
            ViewBag.QuickOID = _param.QuickOID;
            ViewBag.ModuleOID = _param.OID;

            return View("Dialog/dlgEditWorkerEducation", _param);
        }

        public JsonResult InsWorkerEducation(WorkerEdu _param)
        {
            try
            {
                DaoFactory.BeginTransaction();

                int returnValue = WorkerEduRepository.InsWorkerEdu(_param);

                DaoFactory.Commit();

                return Json(returnValue);
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