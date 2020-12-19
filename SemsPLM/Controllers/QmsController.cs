using Common.Constant;
using Common.Factory;
using Common.Models;
using Common.Models.File;
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
        public ActionResult OpenIssueList()
        {
            return View();
        }

        public ActionResult EditOpenIssue(OpenIssue _param)
        {
            ViewBag.ProjectOID = _param.ProjectOID;
            ViewBag.ProcessOID = _param.ProcessOID;
            ViewBag.OpenIssueOID = _param.OID;
            return View("Dialog/dlgEditOpenIssue", _param);
        }

        public JsonResult InsOpenIssue(OpenIssue _param, List<OpenIssueItem> openIssueItems, List<QuickResponse> quickResponses)
        {
            int resultValue = 0;
            try
            {
                DaoFactory.BeginTransaction();

                OpenIssue openIssue = new OpenIssue()
                {
                    CustomerLibOID = _param.CustomerLibOID,
                    CarLibOID = _param.CarLibOID,
                    ProductOID = _param.ProductOID,
                    ProjectOID = _param.ProjectOID,
                    ProcessOID = _param.ProcessOID,
                };

                // OpenIssue 신규작성 일 경우 OpenIssue OID 생성
                if (openIssue.OID == null)
                {
                    DObject dobj = new DObject();
                    dobj.Type = QmsConstant.TYPE_OPEN_ISSUE;
                    dobj.Name = QmsConstant.TYPE_OPEN_ISSUE;
                    openIssue.OID = DObjectRepository.InsDObject(Session, dobj);

                    resultValue = OpenIssueRepository.InsOpenIssue(openIssue);
                }

                // OpenIssue 리스트 항목 추가
                if (openIssueItems.Count > 0)
                {
                    OpenIssueRelationship openIssueRelationship = null;
                    openIssueItems.ForEach(item =>
                    {
                        OpenIssueItem openIssueItem = new OpenIssueItem()
                        {
                            RelapseInsideFl = item.RelapseInsideFl,
                            RelapseHanonFl = item.RelapseHanonFl,
                            RelapseCarFl = item.RelapseCarFl,
                            OpenIssueDetailDesc = item.OpenIssueDetailDesc,
                            OpenIssueOccurrenceDt = item.OpenIssueOccurrenceDt,
                            OpenIssueExpectedDt = item.OpenIssueExpectedDt,
                            OpenIssueCompleteDt = item.OpenIssueCompleteDt,
                            OpenIssueCloseFl = item.OpenIssueCloseFl
                        };

                        if (openIssue.OID == null)
                        {
                            DObject dobj = new DObject();
                            dobj.Type = QmsConstant.TYPE_OPEN_ISSUE_ITEM;
                            dobj.Name = QmsConstant.TYPE_OPEN_ISSUE_ITEM + "_" + openIssue.OID;
                            openIssueItem.OID = DObjectRepository.InsDObject(Session, dobj);

                            resultValue = OpenIssueItemRepository.InsOpenIssueItem(openIssueItem);
                        }

                        if (openIssueRelationship != null)
                        {
                            openIssueRelationship = null;
                        }

                        // OpenIssue 와 OpenIssue 리스트 항목 릴레이션 연결
                        openIssueRelationship = new OpenIssueRelationship()
                        {
                            FromOID = openIssue.OID,
                            ToOID = openIssueItem.OID,
                            type = QmsConstant.TYPE_OPEN_ISSUE_ITEM,
                            CreateUs = Convert.ToInt32(Session["UserOID"])
                        };

                        resultValue = OpenIssueRelationshipRepository.InsOpenIssueRelationship(openIssueRelationship);
                    });
                }

                // 신속대응 연결
                if (quickResponses.Count > 0)
                {
                    OpenIssueRelationship openIssueRelationship = null;
                    quickResponses.ForEach(item =>
                    {
                        if (openIssueRelationship != null)
                        {
                            openIssueRelationship = null;
                        }

                        openIssueRelationship = new OpenIssueRelationship()
                        {
                            FromOID = openIssue.OID,
                            ToOID = item.OID,
                            type = QmsConstant.TYPE_QUICK_RESPONSE,
                            CreateUs = Convert.ToInt32(Session["UserOID"])
                        };

                        resultValue = OpenIssueRelationshipRepository.InsOpenIssueRelationship(openIssueRelationship);
                    });
                }

                DaoFactory.Commit();
                return Json(1);
            }
            catch (Exception ex)
            {
                DaoFactory.Rollback();
                return Json(new ResultJsonModel { isError = true, resultMessage = ex.Message, resultDescription = ex.ToString() });
            }
        }

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

        public ActionResult CreateQuickResponse()
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

            return View();
        }

        public ActionResult InfoQuickResponse(int OID)
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

            ViewBag.QuickDetail = QuickResponseRepository.SelQuickResponse(new QuickResponse() { OID = OID });
            ViewBag.Status = BPolicyRepository.SelBPolicy(new BPolicy { Type = QmsConstant.TYPE_QUICK_RESPONSE });
            return View();
        }

        /*
        public ActionResult CreateQuickResponse(QuickResponse quickResponse)
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
        */

        public JsonResult SelQuickResponseGridList(QuickResponse _param)
        {
            List<QuickResponse> list = QuickResponseRepository.SelQuickResponses(_param);

            List<QuickResponseModule> Modulelist = QuickResponseModuleRepository.SelQuickResponseModules(new QuickResponseModule());

            List<QuickResponseView> gridView = new List<QuickResponseView>();

            list.ForEach(v =>
            {
                QuickResponseView quickResponseView = new QuickResponseView(v);

                var Modules = from m in Modulelist
                              where m.QuickOID == v.OID
                              select m;

                Modules.ToList().ForEach(m =>
                {
                    if (m.ModuleType == QmsConstant.TYPE_BLOCKADE)
                    {
                        quickResponseView.ModuleBlockadeOID = m.OID;
                        quickResponseView.ModuleBlockadeStatusNm = m.BPolicyNm;
                        quickResponseView.ModuleBlockadeFl = m.ModuleFl;
                        quickResponseView.ModuleBlockadeEstEndDt = m.EstEndDt;
                        quickResponseView.ModuleBlockadeChargeUserOID = m.ChargeUserOID;
                        quickResponseView.ModuleBlockadeChargeUserNm = m.ChargeUserNm;
                    }
                    else if (m.ModuleType == QmsConstant.TYPE_OCCURRENCE_CAUSE)
                    {
                        quickResponseView.ModuleOccurrenceCauseOID = m.OID;
                        quickResponseView.ModuleOccurrenceCauseStatusNm = m.BPolicyNm;
                        quickResponseView.ModuleOccurrenceCauseFl = m.ModuleFl;
                        quickResponseView.ModuleOccurrenceCauseEstEndDt = m.EstEndDt;
                        quickResponseView.ModuleOccurrenceCauseChargeUserOID = m.ChargeUserOID;
                        quickResponseView.ModuleOccurrenceCauseChargeUserNm = m.ChargeUserNm;
                    }
                    else if (m.ModuleType == QmsConstant.TYPE_IMPROVE_COUNTERMEASURE)
                    {
                        quickResponseView.ModuleImproveCountermeasureOID = m.OID;
                        quickResponseView.ModuleImproveCountermeasureStatusNm = m.BPolicyNm;
                        quickResponseView.ModuleImproveCountermeasureFl = m.ModuleFl;
                        quickResponseView.ModuleImproveCountermeasureEstEndDt = m.EstEndDt;
                        quickResponseView.ModuleImproveCountermeasureChargeUserOID = m.ChargeUserOID;
                        quickResponseView.ModuleImproveCountermeasureChargeUserNm = m.ChargeUserNm;
                    }
                    else if (m.ModuleType == QmsConstant.TYPE_ERROR_PRROF)
                    {
                        quickResponseView.ModuleErrorProofOID = m.OID;
                        quickResponseView.ModuleErrorProofStatusNm = m.BPolicyNm;
                        quickResponseView.ModuleErrorProofFl = m.ModuleFl;
                        quickResponseView.ModuleErrorProofEstEndDt = m.EstEndDt;
                        quickResponseView.ModuleErrorProofChargeUserOID = m.ChargeUserOID;
                        quickResponseView.ModuleErrorProofChargeUserNm = m.ChargeUserNm;
                    }
                    else if (m.ModuleType == QmsConstant.TYPE_LPA_UNFIT)
                    {
                        quickResponseView.ModuleLpaOID = m.OID;
                        quickResponseView.ModuleLpaStatusNm = m.BPolicyNm;
                        quickResponseView.ModuleLpaFl = m.ModuleFl;
                        quickResponseView.ModuleLpaEstEndDt = m.EstEndDt;
                        quickResponseView.ModuleLpaChargeUserOID = m.ChargeUserOID;
                        quickResponseView.ModuleLpaChargeUserNm = m.ChargeUserNm;
                    }
                    else if (m.ModuleType == QmsConstant.TYPE_QUICK_RESPONSE_CHECK)
                    {
                        quickResponseView.ModuleCheckOID = m.OID;
                        quickResponseView.ModuleCheckStatusNm = m.BPolicyNm;
                        quickResponseView.ModuleCheckFl = m.ModuleFl;
                        quickResponseView.ModuleCheckEstEndDt = m.EstEndDt;
                        quickResponseView.ModuleCheckChargeUserOID = m.ChargeUserOID;
                        quickResponseView.ModuleCheckChargeUserNm = m.ChargeUserNm;
                    }
                    else if (m.ModuleType == QmsConstant.TYPE_STANDARD)
                    {
                        quickResponseView.ModuleStandardOID = m.OID;
                        quickResponseView.ModuleStandardStatusNm = m.BPolicyNm;
                        quickResponseView.ModuleStandardFl = m.ModuleFl;
                        quickResponseView.ModuleStandardEstEndDt = m.EstEndDt;
                        quickResponseView.ModuleStandardChargeUserOID = m.ChargeUserOID;
                        quickResponseView.ModuleStandardChargeUserNm = m.ChargeUserNm;
                    }
                    else if (m.ModuleType == QmsConstant.TYPE_WORKER_EDU)
                    {
                        quickResponseView.ModuleWorkerEduOID = m.OID;
                        quickResponseView.ModuleWorkerEduStatusNm = m.BPolicyNm;
                        quickResponseView.ModuleWorkerEduFl = m.ModuleFl;
                        quickResponseView.ModuleWorkerEduEstEndDt = m.EstEndDt;
                        quickResponseView.ModuleWorkerEduChargeUserOID = m.ChargeUserOID;
                        quickResponseView.ModuleWorkerEduChargeUserNm = m.ChargeUserNm;
                    }
                });


                gridView.Add(quickResponseView);
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
                result = DObjectRepository.InsDObject(Session, dobj);

                _param.OID = result;
                int returnValue = QuickResponseRepository.InsQuickResponse(_param);


                int SetQuickModule(string Type)
                {
                    DObject dobjModule = new DObject();
                    dobjModule.Type = Type;
                    dobjModule.Name = Type;
                    int ModuleOID = DObjectRepository.InsDObject(Session, dobjModule);
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
                    int ItemOID = DObjectRepository.InsDObject(Session, dobjModule);

                    BlockadeItemRepository.InsBlockadeItem(new BlockadeItem() { ModuleOID = ModuleOID, OID = ItemOID });

                    DRelationship dRelModule = new DRelationship();
                    dRelModule.Type = QmsConstant.RELATIONSHIP_QUICK_MODULE;
                    dRelModule.FromOID = ModuleOID;
                    dRelModule.ToOID = ItemOID;
                    DRelationshipRepository.InsDRelationshipNotOrd(dRelModule);
                }

                // 봉쇄조치
                int blockadeOID = SetQuickModule(QmsConstant.TYPE_BLOCKADE);

                if (_param.BlockadeMaterialFl == true)
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
                int ErrorProofOID = SetQuickModule(QmsConstant.TYPE_ERROR_PRROF);

                ErrorProofRepository.InsErrorProof(new ErrorProof() { ModuleOID = ErrorProofOID });

                // LPA 부적합 등록
                int LpaUnfitOID = SetQuickModule(QmsConstant.TYPE_LPA_UNFIT);
                LpaUnfitRepository.InsLpaUnfit(new LpaUnfit() { ModuleOID = LpaUnfitOID });

                // LPA 부적합 대책서 
                int LpaUnMeasureOID = SetQuickModule(QmsConstant.TYPE_LPA_MEASURE);
                LpaMeasureRepository.InsLpaMeasure(new LpaMeasure() { ModuleOID = LpaUnMeasureOID });

                DRelationship dRelLpa = new DRelationship();
                dRelLpa.Type = QmsConstant.RELATIONSHIP_LPA;
                dRelLpa.FromOID = LpaUnfitOID;
                dRelLpa.ToOID = LpaUnMeasureOID;
                DRelationshipRepository.InsDRelationshipNotOrd(dRelLpa);

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
                DObjectRepository.UdtDObject(Session, dobj);

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
            if (quickResponse == null || quickResponse.OID == null)
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
                    ViewBag.ErrorProof = v;
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

                QuickResponseModule blockade = QuickResponseModuleRepository.SelQuickResponseModule(new QuickResponseModule() { QuickOID = _params[0].QuickOID, ModuleType = QmsConstant.TYPE_BLOCKADE });
              
                if(!(blockade.BPolicyNm == QmsConstant.POLICY_QMS_MODULE_PREPARE || blockade.BPolicyNm == QmsConstant.POLICY_QMS_MODULE_STARTED))
                {
                    throw new Exception("봉쇄조치가 진행중인관계로 일정을 수정 할 수 없습니다.");
                }

                _params.ForEach(v =>
                {                    
                    QuickResponseModuleRepository.UdtQuickResponseModule(v);

                    DObject dObject = DObjectRepository.SelDObject(Session, v);

                    if(dObject.Type == QmsConstant.TYPE_LPA_UNFIT)
                    {
                        QuickResponseModule LpaMeasureModule = QuickResponseModuleRepository.SelQuickResponseModule(new QuickResponseModule() { QuickOID = v.QuickOID, ModuleType = QmsConstant.TYPE_LPA_MEASURE });

                        LpaMeasureModule.ModuleFl = v.ModuleFl;

                        QuickResponseModuleRepository.UdtQuickResponseModule(LpaMeasureModule);
                    }

                    if (dObject.Type == QmsConstant.TYPE_BLOCKADE && dObject.BPolicyOID == 57 && v.EstEndDt != null && v.ChargeUserOID != null)
                    {
                        dObject.BPolicyOID = 58; // 작성중으로 변경
                        DObjectRepository.UdtDObject(Session, dObject);
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

        #region -- 봉쇄조치 

        public ActionResult InfoBlockade(int OID)
        {
            QuickResponseModule Module = QuickResponseModuleRepository.SelQuickResponseModule(new QuickResponseModule { OID = OID });
            ViewBag.Blockade = Module;
            ViewBag.BlockadeItems = BlockadeItemRepository.SelBlockadeItems(new BlockadeItem() { ModuleOID = OID });
            ViewBag.QuickDetail = QuickResponseRepository.SelQuickResponse(new QuickResponse() { OID = Module.QuickOID });
            ViewBag.Status = BPolicyRepository.SelBPolicy(new BPolicy { Type = QmsConstant.TYPE_BLOCKADE });
            return View();
        }

        public ActionResult EditQuickBlockade(QuickResponseModule _param)
        {
            ViewBag.QuickOID = _param.QuickOID;
            return View("Dialog/dlgEditQuickBlockade", BlockadeItemRepository.SelBlockadeItems(new BlockadeItem() { ModuleOID = _param.OID }));
        }

        public JsonResult SaveQuickBlockade(Blockade param)
        {
            try
            {
                DaoFactory.BeginTransaction();
                param.Type = QmsConstant.TYPE_BLOCKADE;
                param.BlockadeItems.ForEach(v =>
                {
                    DObjectRepository.UdtDObject(Session, v);
                    BlockadeItemRepository.UdtBlockadeItem(v);
                });

                if (param.Files != null)
                {
                    HttpFileRepository.InsertData(Session, param);
                }

                if (param.delFiles != null)
                {
                    param.delFiles.ForEach(v =>
                    {
                        HttpFileRepository.DeleteData(Session, v);
                    });
                }

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

        public ActionResult InfoOccurrenceCause(int OID)
        {
            QuickResponseModule Module = QuickResponseModuleRepository.SelQuickResponseModule(new QuickResponseModule { OID = OID });
            ViewBag.OccurrenceCause = Module;
            ViewBag.QuickDetail = QuickResponseRepository.SelQuickResponse(new QuickResponse() { OID = Module.QuickOID });

            Library occurrenceCauseKey = LibraryRepository.SelLibraryObject(new Library { Name = "OCCURRENCE_CAUSE" });
            List<Library> occurrenceCauseList = LibraryRepository.SelLibrary(new Library { FromOID = occurrenceCauseKey.OID });  // 검사유형
            ViewBag.OccurrenceCauseLibList = occurrenceCauseList;

            List<OccurrenceCauseItem> OccurrenceCauseItems = OccurrenceCauseItemRepository.SelOccurrenceCauseItems(new OccurrenceCauseItem() { ModuleOID = OID });
            OccurrenceCauseItems.ForEach(v =>
            {
                v.OccurrenceWhys = OccurrenceWhyRepository.SelOccurrenceWhys(new OccurrenceWhy() { CauseOID = v.OID });
            });
            ViewBag.OccurrenceCauseItems = OccurrenceCauseItems;

            DetectCounterMeasure detectCounterMeasure = DetectCounterMeasureRepository.SelDetectCounterMeasure(new DetectCounterMeasure() { ModuleOID = OID });
            ViewBag.DetectCounterMeasure = detectCounterMeasure == null ? new DetectCounterMeasure() { ModuleOID = OID } : detectCounterMeasure;
            ViewBag.Status = BPolicyRepository.SelBPolicy(new BPolicy { Type = QmsConstant.TYPE_OCCURRENCE_CAUSE });

            return View();
        }

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

        public JsonResult SaveQuickOccurrenceCause(OccurrenceCause param)
        {
            try
            {
                DaoFactory.BeginTransaction();
                param.Type = QmsConstant.TYPE_OCCURRENCE_CAUSE;
                param.OccurrenceCauseItems.ForEach(v =>
                {
                    int? CauseOID = v.OID;
                    if (v.OID == null)
                    {
                        DObject dobj = new DObject();
                        dobj.Type = QmsConstant.TYPE_OCCURRENCE_CAUSE_ITEM;
                        dobj.Name = QmsConstant.TYPE_OCCURRENCE_CAUSE_ITEM;
                        v.OID = DObjectRepository.InsDObject(Session, dobj);
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
                            w.OID = DObjectRepository.InsDObject(Session, dobj);
                            w.CauseOID = v.OID;

                            OccurrenceWhyRepository.InsOccurrenceWhy(w);
                        }
                        else
                        {
                            if (w.IsRemove == "Y")
                            {
                                w.DeleteUs = 73;
                                DObjectRepository.DelDObject(Session, w);
                            }
                            else
                            {
                                w.ModifyUs = 73;
                                OccurrenceWhyRepository.UdtOccurrenceWhy(w);
                                DObjectRepository.UdtDObject(Session, w);
                            }
                        }
                    });
                });

                DetectCounterMeasureRepository.UdtDetectCounterMeasure(param.DetectCounterMeasure);

                if (param.Files != null)
                {
                    HttpFileRepository.InsertData(Session, param);
                }

                if (param.delFiles != null)
                {
                    param.delFiles.ForEach(v =>
                    {
                        HttpFileRepository.DeleteData(Session, v);
                    });
                }

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
        public ActionResult InfoImproveCounterMeasure(int OID)
        {
            QuickResponseModule Module = QuickResponseModuleRepository.SelQuickResponseModule(new QuickResponseModule { OID = OID });
            ViewBag.ImproveCounterMeasure = Module;
            ViewBag.QuickDetail = QuickResponseRepository.SelQuickResponse(new QuickResponse() { OID = Module.QuickOID });
            ViewBag.ImproveCounterMeasureItems = ImproveCounterMeasureItemRepository.SelImproveCounterMeasureItems(new ImproveCounterMeasureItem() { ModuleOID = OID });
            ViewBag.Status = BPolicyRepository.SelBPolicy(new BPolicy { Type = QmsConstant.TYPE_IMPROVE_COUNTERMEASURE });
            return View();
        }

        public ActionResult EditQuickImproveCounterMeasure(QuickResponseModule _param)
        {
            ViewBag.QuickOID = _param.QuickOID;
            ViewBag.ModuleOID = _param.OID;

            ViewBag.ImproveCounterMeasures = ImproveCounterMeasureItemRepository.SelImproveCounterMeasureItems(new ImproveCounterMeasureItem() { ModuleOID = _param.OID });

            return View("Dialog/dlgEditQuickImproveCounterMeasure");
        }

        public JsonResult SaveQuickImproveCounterMeasure(ImproveCounterMeasure param)
        {
            try
            {
                DaoFactory.BeginTransaction();
                param.Type = QmsConstant.TYPE_IMPROVE_COUNTERMEASURE;
                param.ImproveCounterMeasureItems.ForEach(v =>
                {
                    if (v.OID == null)
                    {
                        DObject dobj = new DObject();
                        dobj.Type = QmsConstant.TYPE_IMPROVE_COUNTERMEASURE_ITEM;
                        dobj.Name = QmsConstant.TYPE_IMPROVE_COUNTERMEASURE_ITEM;
                        v.OID = DObjectRepository.InsDObject(Session, dobj);

                        ImproveCounterMeasureItemRepository.InsImproveCounterMeasureItem(v);
                    }
                    else
                    {
                        if (v.IsRemove == "Y")
                        {
                            v.DeleteUs = 73;
                            DObjectRepository.DelDObject(Session, v);
                        }
                        else
                        {
                            v.ModifyUs = 73;
                            ImproveCounterMeasureItemRepository.UdtImproveCounterMeasureItem(v);
                            DObjectRepository.UdtDObject(Session, v);
                        }
                    }
                });

                if (param.Files != null)
                {
                    HttpFileRepository.InsertData(Session, param);
                }

                if (param.delFiles != null)
                {
                    param.delFiles.ForEach(v =>
                    {
                        HttpFileRepository.DeleteData(Session, v);
                    });
                }

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

        public ActionResult InfoErrorProof(int OID)
        {
            QuickResponseModule Module = QuickResponseModuleRepository.SelQuickResponseModule(new QuickResponseModule { OID = OID });
            ErrorProof errorproof = ErrorProofRepository.SelErrorProof(new ErrorProof() { ModuleOID = OID });
            if (errorproof == null)
            {
                errorproof = new ErrorProof();
                errorproof.OID = OID;
                errorproof.ModuleOID = OID;
            }
            ViewBag.ErrorProof = errorproof;
            ViewBag.QuickDetail = QuickResponseRepository.SelQuickResponse(new QuickResponse() { OID = Module.QuickOID });
            ViewBag.Status = BPolicyRepository.SelBPolicy(new BPolicy { Type = QmsConstant.TYPE_ERROR_PRROF });
            return View();
        }
        /// <summary>
        /// 2020.11.15
        /// 작업자 교육 등록
        /// </summary>
        /// <returns></returns>
        public ActionResult EditErrorProof(ErrorProof _param)
        {
            ViewBag.QuickOID = _param.QuickOID;
            ViewBag.ModuleOID = _param.OID;

            return View("Dialog/dlgEditErrorProof", _param);
        }

        public JsonResult SaveQuickErrorProof(ErrorProof param)
        {
            try
            {
                DaoFactory.BeginTransaction();
                param.Type = QmsConstant.TYPE_ERROR_PRROF;

                ErrorProof errorproof = ErrorProofRepository.SelErrorProof(new ErrorProof() { ModuleOID = param.OID });

                DObjectRepository.UdtDObject(Session, param);
                if (errorproof == null)
                {
                    ErrorProofRepository.InsErrorProof(param);
                }
                else
                {
                    ErrorProofRepository.UdtErrorProof(param);
                }

                if (param.Files != null)
                {
                    HttpFileRepository.InsertData(Session, param);
                }

                if (param.delFiles != null)
                {
                    param.delFiles.ForEach(v =>
                    {
                        HttpFileRepository.DeleteData(Session, v);
                    });
                }

                DaoFactory.Commit();
                return Json("1");
            }
            catch (Exception ex)
            {
                DaoFactory.Rollback();
                return Json(new ResultJsonModel { isError = true, resultMessage = ex.Message, resultDescription = ex.ToString() });
            }
        }

        public JsonResult InsErrorProof(ErrorProof _param)
        {
            int returnValue = 0;
            try
            {
                DaoFactory.BeginTransaction();

                ErrorProof ErrorProof = new ErrorProof()
                {
                    ModuleOID = _param.ModuleOID,
                    EstDt = _param.EstDt,
                    ActDt = _param.ActDt,
                    CheckDetail = _param.CheckDetail,
                    CheckUserOID = _param.CheckUserOID,
                };

                returnValue = ErrorProofRepository.InsErrorProof(ErrorProof);

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
        public ActionResult InfoLpaUnfit(int OID)
        {
            QuickResponseModule Module = QuickResponseModuleRepository.SelQuickResponseModule(new QuickResponseModule { OID = OID });
            LpaUnfit lpaUnfit = LpaUnfitRepository.SelLpaUnfit(new LpaUnfit() { ModuleOID = OID });
            if (lpaUnfit == null)
            {
                lpaUnfit = new LpaUnfit();
                lpaUnfit.OID = OID;
                lpaUnfit.ModuleOID = OID;
            }

            List<LpaUnfitCheck> lpaUnfitCheck = LpaUnfitCheckRepository.SelLpaUnfitChecks(new LpaUnfitCheck() { ModuleOID = OID });

            List<DRelationship> relLpa = DRelationshipRepository.SelRelationship(new DRelationship() { Type = QmsConstant.RELATIONSHIP_LPA, FromOID = OID });

            ViewBag.LpaUnfit = lpaUnfit;
            ViewBag.LpaUnfitCheck = lpaUnfitCheck;
            ViewBag.QuickDetail = QuickResponseRepository.SelQuickResponse(new QuickResponse() { OID = Module.QuickOID });
            ViewBag.Status = BPolicyRepository.SelBPolicy(new BPolicy { Type = QmsConstant.TYPE_LPA_UNFIT });
            ViewBag.CurrentSt = Module.BPolicyNm;
            relLpa.ForEach(v => { ViewBag.LpaMeasureOID = v.ToOID; });

            // 콤보박스용
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

            return View();
        }

        public JsonResult SaveQuickLpaUnfit(LpaUnfit param)
        {
            try
            {
                DaoFactory.BeginTransaction();
                param.Type = QmsConstant.TYPE_LPA_UNFIT;

                LpaUnfit lpaUnfit = LpaUnfitRepository.SelLpaUnfit(new LpaUnfit() { ModuleOID = param.OID });

                DObjectRepository.UdtDObject(Session, param);
                int? OID = null;
                if (lpaUnfit == null)
                {
                    OID = LpaUnfitRepository.InsLpaUnfit(param);
                }
                else
                {
                    OID = param.OID;
                    LpaUnfitRepository.UdtLpaUnfit(param);
                }

                param.LpaUnfitChecks.ForEach(v =>
                {
                    if (v.OID == null)
                    {
                        DObject dobj = new DObject();
                        dobj.Type = QmsConstant.TYPE_LPA_UNFIT_CHECK_ITEM;
                        dobj.Name = QmsConstant.TYPE_LPA_UNFIT_CHECK_ITEM;
                        v.OID = DObjectRepository.InsDObject(Session, dobj);

                        LpaUnfitCheckRepository.InsLpaUnfitCheck(v);
                    }
                    else
                    {
                        if (v.IsRemove == "Y")
                        {
                            v.DeleteUs = 73;
                            DObjectRepository.DelDObject(Session, v);
                        }
                        else
                        {
                            v.ModifyUs = 73;
                            LpaUnfitCheckRepository.UdtLpaUnfitCheck(v);
                            DObjectRepository.UdtDObject(Session, v);
                        }
                    }
                });

                if (param.Files != null)
                {
                    HttpFileRepository.InsertData(Session, param);
                }

                if (param.delFiles != null)
                {
                    param.delFiles.ForEach(v =>
                    {
                        HttpFileRepository.DeleteData(Session, v);
                    });
                }

                DaoFactory.Commit();
                return Json("1");
            }
            catch (Exception ex)
            {
                DaoFactory.Rollback();
                return Json(new ResultJsonModel { isError = true, resultMessage = ex.Message, resultDescription = ex.ToString() });
            }
        }

        public JsonResult ConfirmQuickLpaUnfit(LpaUnfit param)
        {
            try
            {
                DaoFactory.BeginTransaction();

                if(param.ModuleOID == null) { throw new Exception("잘못된 호출입니다."); }

                DObjectRepository.UdtDObject(Session, new DObject() { OID = param.ModuleOID, BPolicyOID = 79 });

                QuickResponseModule quickResponse = QuickResponseModuleRepository.SelQuickResponseModule(new QuickResponseModule() { OID = param.ModuleOID });

                QuickResponseModule LpaMeasureModule = QuickResponseModuleRepository.SelQuickResponseModule(new QuickResponseModule() { QuickOID = quickResponse.QuickOID, ModuleFl = 1, ModuleType = QmsConstant.TYPE_LPA_MEASURE });

                List<BPolicy> nextModluePolicies = BPolicyRepository.SelBPolicy(new BPolicy() { Type = QmsConstant.TYPE_LPA_MEASURE, Name = "Started" });

                DObjectRepository.UdtDObject(Session, new DObject() { OID = LpaMeasureModule.OID, BPolicyOID = nextModluePolicies[0].OID });

                DaoFactory.Commit();

                return Json(param.ModuleOID);
            }
            catch(Exception ex)
            {
                DaoFactory.Rollback();
                return Json(new ResultJsonModel { isError = true, resultMessage = ex.Message, resultDescription = ex.ToString() });
            }
        }

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
                    //OID = _param.LpaUnfitCheck.OID,
                    //ModuleOID = lpaUnfit.ModuleOID,
                    //CheckPoin = _param.LpaUnfitCheck.CheckPoin
                };

                // LPA 부적합현황 지적사항 등록
                if (lpaUnfitCheck.OID == null)
                {
                    DObject dobj = new DObject();
                    dobj.Type = QmsConstant.TYPE_LPA_UNFIT_CHECK_ITEM;
                    dobj.Name = "LPA_UNFIT_CHECK_ITEM_" + lpaUnfit.ModuleOID;
                    lpaUnfitCheck.OID = DObjectRepository.InsDObject(Session, dobj);

                    resultValue = LpaUnfitCheckRepository.InsLpaUnfitCheck(lpaUnfitCheck);
                }

                DaoFactory.Commit();
                return Json(resultValue);
            }
            catch (Exception ex)
            {
                DaoFactory.Rollback();
                return Json(new ResultJsonModel { isError = true, resultMessage = ex.Message, resultDescription = ex.ToString() });
            }
        }
        #endregion

        #region -- LPA 대책서 
        public ActionResult InfoLpaMeasure(int OID)
        {
            QuickResponseModule Module = QuickResponseModuleRepository.SelQuickResponseModule(new QuickResponseModule { OID = OID });
            LpaMeasure lpaMeasure = LpaMeasureRepository.SelLpaMeasure(new LpaMeasure() { ModuleOID = OID });
            if (lpaMeasure == null)
            {
                lpaMeasure = new LpaMeasure();
                lpaMeasure.OID = OID;
                lpaMeasure.ModuleOID = OID;
            }

            List<DRelationship> relLpa = DRelationshipRepository.SelRelationship(new DRelationship() { Type = QmsConstant.RELATIONSHIP_LPA, ToOID = OID });

            int? LpaUnfitOID = null;
            relLpa.ForEach(v => { LpaUnfitOID = v.FromOID; });
            List<LpaUnfitCheck> lpaUnfitCheck = LpaUnfitCheckRepository.SelLpaUnfitChecks(new LpaUnfitCheck() { ModuleOID = LpaUnfitOID });

            ViewBag.lpaMeasure = lpaMeasure;
            ViewBag.LpaUnfitCheck = lpaUnfitCheck;
            ViewBag.QuickDetail = QuickResponseRepository.SelQuickResponse(new QuickResponse() { OID = Module.QuickOID });
            ViewBag.Status = BPolicyRepository.SelBPolicy(new BPolicy { Type = QmsConstant.TYPE_LPA_MEASURE });
            ViewBag.CurrentSt = Module.BPolicyNm;
            ViewBag.LpaUnfitOID = LpaUnfitOID;

            return View();
        }

        public JsonResult SaveQuickLpaMeasure(LpaMeasure param)
        {
            try
            {
                DaoFactory.BeginTransaction();
                param.Type = QmsConstant.TYPE_LPA_MEASURE;

                LpaMeasure lpaMeasure = LpaMeasureRepository.SelLpaMeasure(new LpaMeasure() { ModuleOID = param.OID });

                DObjectRepository.UdtDObject(Session, param);
                int? OID = null;
                if (lpaMeasure == null)
                {
                    OID = LpaMeasureRepository.InsLpaMeasure(param);
                }
                else
                {
                    OID = param.OID;
                    LpaMeasureRepository.UdtLpaMeasure(param);
                }

                param.LpaUnfitChecks.ForEach(v =>
                {
                    v.ModifyUs = 73;
                    LpaUnfitCheckRepository.UdtLpaUnfitCheck(v);
                    DObjectRepository.UdtDObject(Session, v);
                });

                if (param.Files != null)
                {
                    HttpFileRepository.InsertData(Session, param);
                }

                if (param.delFiles != null)
                {
                    param.delFiles.ForEach(v =>
                    {
                        HttpFileRepository.DeleteData(Session, v);
                    });
                }

                DaoFactory.Commit();
                return Json("1");
            }
            catch (Exception ex)
            {
                DaoFactory.Rollback();
                return Json(new ResultJsonModel { isError = true, resultMessage = ex.Message, resultDescription = ex.ToString() });
            }
        }

        public ActionResult EditLPAMeasure(LpaMeasure _param)
        {
            ViewBag.QuickOID = _param.QuickOID;
            ViewBag.ModuleOID = _param.OID;

            List<QuickResponseModule> list = QuickResponseModuleRepository.SelQuickResponseModules(new QuickResponseModule() { QuickOID = _param.QuickOID });

            QuickResponseModule lpaUnfitModule = (from x in list
                                                  where x.Type == QmsConstant.TYPE_LPA_UNFIT
                                                  select x
                                                 ).FirstOrDefault();
            if (lpaUnfitModule != null)
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

        #region -- 화면
        /// <summary>
        /// 2020.11.15
        /// 유효성 검증 등록 화면
        /// </summary>
        /// <returns></returns>
        public ActionResult EditQuickValidation(QmsCheck _param)
        {
            ViewBag.QuickOID = _param.QuickOID;
            ViewBag.ModuleOID = _param.OID;
            QuickResponseModule qmsCheckModule = QuickResponseModuleRepository.SelQuickResponseModule(new QuickResponseModule { QuickOID = _param.QuickOID, OID = _param.OID });
            QmsCheck qmsCheck = new QmsCheck()
            {
                QuickOID = qmsCheckModule.QuickOID,
                OID = qmsCheckModule.OID,
                ModuleOID = qmsCheckModule.OID
            };

            return View("Dialog/dlgEditQuickValidation", qmsCheck);
        }

        /// <summary>
        /// 2020.12.13
        /// 유효성 검증 상세화면
        /// </summary>
        /// <returns></returns>
        public ActionResult InfoQuickValidation(int? OID)
        {
            QuickResponseModule Module = QuickResponseModuleRepository.SelQuickResponseModule(new QuickResponseModule { OID = OID });
            ViewBag.QmsCheck = Module;

            ViewBag.QmsCheckItems = QmsCheckRepository.SelQmsChecks(new QmsCheck() { ModuleOID = OID });
            ViewBag.QuickDetail = QuickResponseRepository.SelQuickResponse(new QuickResponse() { OID = Module.QuickOID });
            ViewBag.Status = BPolicyRepository.SelBPolicy(new BPolicy { Type = QmsConstant.TYPE_QUICK_RESPONSE_CHECK });
            ViewBag.CurrentSt = Module.BPolicyNm;
            return View();
        }
        #endregion

        #region -- 등록, 수정, 삭제, 조회
        /// <summary>
        /// 2020.11.21
        /// 유효성 검증 등록
        /// </summary>
        /// <param name="_param"></param>
        /// <returns></returns>
        public JsonResult InsQuickValidation(QmsCheck _param)
        {
            try
            {
                DaoFactory.BeginTransaction();

                foreach (QmsCheck qmsCheck in _param.QmsCheckList)
                {
                    if (qmsCheck.OID == null)
                    {
                        DObject dobj = new DObject();
                        dobj.Type = QmsConstant.TYPE_QUICK_RESPONSE_CHECK_ITEM;
                        dobj.Name = QmsConstant.TYPE_QUICK_RESPONSE_CHECK_ITEM + "_" + qmsCheck.ModuleOID;
                        qmsCheck.OID = DObjectRepository.InsDObject(Session, dobj);

                        QmsCheckRepository.InsQmsCheck(qmsCheck);
                    }
                }

                DaoFactory.Commit();

                return Json(1);
            }
            catch (Exception ex)
            {
                DaoFactory.Rollback();
                return Json(new ResultJsonModel { isError = true, resultMessage = ex.Message, resultDescription = ex.ToString() });
            }
        }

        /// <summary>
        /// 2020.12.13
        /// 유효성 검증 수정
        /// </summary>
        /// <param name="_param"></param>
        /// <returns></returns>
        public JsonResult UdtQuickValidation(QmsCheck _param)
        {
            try
            {
                DaoFactory.BeginTransaction();

                foreach (QmsCheck qmsCheck in _param.QmsCheckList)
                {
                    if (qmsCheck.OID == null)
                    {
                        throw new Exception("잘못된 호출");
                    }

                    QmsCheckRepository.UdtQmsCheck(qmsCheck);
                }

                DaoFactory.Commit();

                return Json(1);
            }
            catch (Exception ex)
            {
                DaoFactory.Rollback();
                return Json(new ResultJsonModel { isError = true, resultMessage = ex.Message, resultDescription = ex.ToString() });
            }
        }

        #endregion

        #endregion

        #region -- 표준화

        #region -- 화면
        /// <summary>
        /// 2020.11.15
        /// 표준화 등록 화면
        /// </summary>
        /// <returns></returns>
        public ActionResult EditStandardFollowUp(StandardDoc _param)
        {
            ViewBag.QuickOID = _param.QuickOID;
            ViewBag.ModuleOID = _param.OID;
            QuickResponseModule standardDocModule = QuickResponseModuleRepository.SelQuickResponseModule(new QuickResponseModule { QuickOID = _param.QuickOID, OID = _param.OID });
            StandardDoc standardDoc = new StandardDoc()
            {
                QuickOID = standardDocModule.QuickOID,
                OID = standardDocModule.OID,
                ModuleOID = standardDocModule.OID
            };

            return View("Dialog/dlgEditStandardFollowUp", standardDoc);
        }

        /// <summary>
        /// 2020.12.13
        /// 표준화 상세화면
        /// PFMEA, Drawing, ManagePlan, WorkStd, Inspect 문서타입이 정해지면 추가 작업 진행 예정
        /// </summary>
        /// <returns></returns>
        public ActionResult InfoStandardFollowUp(int? OID)
        {
            QuickResponseModule Module = QuickResponseModuleRepository.SelQuickResponseModule(new QuickResponseModule { OID = OID });
            StandardDoc standardDoc = StandardDocRepository.SelStandardDoc(new StandardDoc() { ModuleOID = OID });
            if (standardDoc == null)
            {
                standardDoc = new StandardDoc();
                standardDoc.OID = OID;
                standardDoc.ModuleOID = OID;
            }

            /*ViewBag.StandardDocDetail = StandardDocRepository.SelStandardDocs(new StandardDoc() { ModuleOID = OID });*/
            // TEST
            List<StandardDoc> StandardDocs = new List<StandardDoc>();
            for (int i = 0; i < 5; i++)
            {
                StandardDocs.Add(new StandardDoc()
                {
                    OID = i,
                    ModuleOID = 100,
                    DocOID = 100 + i,
                    DocNm = "TEST",
                    DocSummary = "TEST",
                    DocCompleteDt = DateTime.Now,
                });
            }

            ViewBag.StandardDoc = standardDoc;
            ViewBag.StandardDocDetail = StandardDocs.ToList();
            ViewBag.QuickDetail = QuickResponseRepository.SelQuickResponse(new QuickResponse() { OID = Module.QuickOID });
            ViewBag.Status = BPolicyRepository.SelBPolicy(new BPolicy { Type = QmsConstant.TYPE_STANDARD });
            ViewBag.CurrentSt = Module.BPolicyNm;

            return View();
        }
        #endregion

        #region -- 등록, 수정, 삭제, 조회
        public JsonResult InsStandardFollowUp(StandardDoc _param)
        {
            int ModuleOID = 0;
            int returnValue = 0;
            try
            {
                DaoFactory.BeginTransaction();

                foreach (StandardDoc standardDoc in _param.StandardFollowUpList)
                {
                    if (standardDoc.OID == null)
                    {
                        DObject dobj = new DObject();
                        dobj.Type = QmsConstant.TYPE_STANDARD_DOC_ITEM;
                        dobj.Name = QmsConstant.TYPE_STANDARD_DOC_ITEM + "_" + standardDoc.ModuleOID;
                        standardDoc.OID = DObjectRepository.InsDObject(Session, dobj);

                        returnValue = StandardDocRepository.InsStandardDoc(standardDoc);
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

        public JsonResult UdtStandardFollowUp(StandardDoc _param)
        {
            try
            {
                DaoFactory.BeginTransaction();

                foreach (StandardDoc standardDoc in _param.StandardFollowUpList)
                {
                    if (standardDoc.OID == null)
                    {
                        throw new Exception("잘못된 호출");
                    }

                    StandardDocRepository.UdtStandardDoc(standardDoc);
                }

                DaoFactory.Commit();

                return Json(1);
            }
            catch (Exception ex)
            {
                DaoFactory.Rollback();
                return Json(new ResultJsonModel { isError = true, resultMessage = ex.Message, resultDescription = ex.ToString() });
            }
        }
        #endregion

        #endregion

        #region -- 교육

        #region -- 화면
        /// <summary>
        /// 2020.12.12
        /// 작업자 교육 리스트
        /// </summary>
        /// <returns></returns>
        public ActionResult WorkerEducationList()
        {
            return View();
        }

        /// <summary>
        /// 2020.11.15
        /// 작업자 교육 등록
        /// </summary>
        /// <returns></returns>
        public ActionResult EditWorkerEducation(WorkerEdu _param)
        {
            ViewBag.QuickOID = _param.QuickOID;
            ViewBag.ModuleOID = _param.OID;
            QuickResponseModule workerEduModule = QuickResponseModuleRepository.SelQuickResponseModule(new QuickResponseModule { QuickOID = _param.QuickOID, OID = _param.OID });
            WorkerEdu workerEdu = new WorkerEdu()
            {
                QuickOID = workerEduModule.QuickOID,
                OID = workerEduModule.OID,
                ModuleOID = workerEduModule.OID
            };

            return View("Dialog/dlgEditWorkerEducation", workerEdu);
        }

        /// <summary>
        /// 2020.12.12
        /// 작업자 교육 상세화면
        /// </summary>
        /// <returns></returns>
        public ActionResult InfoWorkerEducation(int? OID)
        {
            QuickResponseModule Module = QuickResponseModuleRepository.SelQuickResponseModule(new QuickResponseModule { OID = OID });
            WorkerEdu workerEdu = WorkerEduRepository.SelWorkerEdu(new WorkerEdu() { ModuleOID = OID });
            if (workerEdu == null)
            {
                workerEdu = new WorkerEdu();
                workerEdu.OID = OID;
                workerEdu.ModuleOID = OID;
            }

            ViewBag.WorkerEdu = workerEdu;
            ViewBag.QuickDetail = QuickResponseRepository.SelQuickResponse(new QuickResponse() { OID = Module.QuickOID });
            ViewBag.Status = BPolicyRepository.SelBPolicy(new BPolicy { Type = QmsConstant.TYPE_WORKER_EDU });
            ViewBag.CurrentSt = Module.BPolicyNm;
            return View();
        }
        #endregion

        #region -- 등록, 수정, 삭제, 조회
        public JsonResult InsWorkerEducation(WorkerEdu param)
        {
            try
            {
                DaoFactory.BeginTransaction();
                param.Type = QmsConstant.TYPE_WORKER_EDU;
                int returnValue = WorkerEduRepository.InsWorkerEdu(param);

                if (param.Files != null)
                {
                    HttpFileRepository.InsertData(Session, param);
                }

                if (param.delFiles != null)
                {
                    param.delFiles.ForEach(v =>
                    {
                        HttpFileRepository.DeleteData(Session, v);
                    });
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

        public JsonResult UdtWorkerEducation(WorkerEdu param)
        {
            try
            {
                DaoFactory.BeginTransaction();
                param.Type = QmsConstant.TYPE_WORKER_EDU;
                int returnValue = WorkerEduRepository.UdtWorkerEdu(param);

                if (param.Files != null)
                {
                    HttpFileRepository.InsertData(Session, param);
                }

                if (param.delFiles != null)
                {
                    param.delFiles.ForEach(v =>
                    {
                        HttpFileRepository.DeleteData(Session, v);
                    });
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

        public JsonResult SelWorkerEducationGridList(WorkerEdu searchModel, int pagesize, int pagenum)
        {
            try
            {
                List<WorkerEdu> workerEdus = WorkerEduRepository.SelWorkerEdus(searchModel);

                var total = workerEdus.Count();
                workerEdus = workerEdus.Skip(pagesize * pagenum).Take(pagesize).Cast<WorkerEdu>().ToList();

                var result = new
                {
                    TotalRows = total,
                    Rows = workerEdus
                };

                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new ResultJsonModel { isError = true, resultMessage = ex.Message, resultDescription = ex.ToString() });
            }
        }

        #endregion

        #endregion
    }
}