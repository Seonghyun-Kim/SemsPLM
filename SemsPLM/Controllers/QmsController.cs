using Common.Constant;
using Common.Factory;
using Common.Models;
using Common.Models.File;
using OfficeOpenXml;
using Pms.Models;
using Qms.Models;
using SemsPLM.Filter;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Web;
using System.Web.Mvc;

namespace SemsPLM.Controllers
{
    [AuthorizeFilter]
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

        public ActionResult InfoOpenIssue(int? OID)
        {
            if (OID == null) { throw new Exception(""); }

            OpenIssue _OpenIssue = OpenIssueRepository.SelOpenIssue(new OpenIssue() { OID = OID });
            ViewBag.OpenIssue = _OpenIssue;
            ViewBag.Status = BPolicyRepository.SelBPolicy(new BPolicy { Type = QmsConstant.TYPE_OPEN_ISSUE });
            ViewBag.ItemStatus = BPolicyRepository.SelBPolicy(new BPolicy { Type = QmsConstant.TYPE_OPEN_ISSUE_ITEM });

            var items = OpenIssueItemRepository.SelOpenIssueItems(new OpenIssueItem() { OpenIssueOID = OID });
            ViewBag.OpenIssueItemCnt = items == null ? 0 : items.Count();

            if (items == null)
            {
                ViewBag.ItemRatio = null;
            }
            else if (items.Count() > 0)
            {
                int FinishCnt = _OpenIssue.CompleatedCnt;

                ViewBag.ItemRatio = (FinishCnt / items.Count() * 100).ToString() + "%";
            }

            return View();
        }

        public ActionResult EditOpenIssue(OpenIssue _param)
        {
            ViewBag.OpenIssueOID = _param.OID;
            return View("Dialog/dlgEditOpenIssue", _param);
        }

        public JsonResult SelOpenIssue(OpenIssue _param)
        {
            return Json(OpenIssueRepository.SelOpenIssue(_param));
        }

        public JsonResult SelOpenIssues(OpenIssue _param)
        {
            return Json(OpenIssueRepository.SelOpenIssues(_param));
        }

        public JsonResult SelOpenIssueItems(OpenIssueItem _param)
        {
            return Json(OpenIssueItemRepository.SelOpenIssueItems(_param));
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
                    ProjectOID = _param.ProjectOID,
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
                            dobj.BPolicyOID = _param.BPolicyOID;
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

        public JsonResult SaveOpenIssueItem(OpenIssue _param)
        {
            try
            {
                DaoFactory.BeginTransaction();

                // OpenIssue 리스트 항목 추가
                if (_param.OpenIssueItems != null && _param.OpenIssueItems.Count > 0)
                {

                    _param.OpenIssueItems.ForEach(item =>
                    {
                        int? ItemOID = null;
                        OpenIssueItem openIssueItem = new OpenIssueItem()
                        {
                            OpenIssueTitle = item.OpenIssueTitle,
                            RelapseInsideFl = item.RelapseInsideFl,
                            RelapseHanonFl = item.RelapseHanonFl,
                            RelapseCarFl = item.RelapseCarFl,
                            OpenIssueDetailDesc = item.OpenIssueDetailDesc,
                            OpenIssueOccurrenceDt = item.OpenIssueOccurrenceDt,
                            OpenIssueExpectedDt = item.OpenIssueExpectedDt,
                            OpenIssueCompleteDt = item.OpenIssueCompleteDt,
                            OpenIssueCloseFl = item.OpenIssueCloseFl
                        };

                        if (item.OID == null)
                        {
                            if (item.IsDel == "Y")
                            {
                                return;
                            }

                            _param.BPolicyOID = null;

                            DObject dobj = new DObject();
                            dobj.Type = QmsConstant.TYPE_OPEN_ISSUE_ITEM;
                            dobj.Name = QmsConstant.TYPE_OPEN_ISSUE_ITEM + "_" + _param.OID;
                            dobj.Description = _param.Description;
                            ItemOID = DObjectRepository.InsDObject(Session, dobj);
                            openIssueItem.OID = ItemOID;
                            OpenIssueItemRepository.InsOpenIssueItem(openIssueItem);

                            OpenIssueRelationship openIssueRelationship = new OpenIssueRelationship()
                            {
                                FromOID = _param.OID,
                                ToOID = ItemOID,
                                type = QmsConstant.TYPE_OPEN_ISSUE_ITEM,
                                CreateUs = Convert.ToInt32(Session["UserOID"])
                            };

                            OpenIssueRelationshipRepository.InsOpenIssueRelationship(openIssueRelationship);

                            OpenIssueRepository.UdtOpenIssueSuspenseCnt(_param);
                        }
                        else
                        {
                            if (item.IsDel == "Y")
                            {
                                var policys = BPolicyRepository.SelBPolicy(new BPolicy { Type = QmsConstant.TYPE_OPEN_ISSUE_ITEM });

                                policys.ForEach(v =>
                                {
                                    if (item.BPolicyOID == v.OID)
                                    {
                                        if (v.Name != QmsConstant.POLICY_OPENISSUE_ITEM_SUSPENSE)
                                        {
                                            throw new Exception("OPEN ISSUE 항목은 미결일 경우에만 삭제 할 수 있습니다.");
                                        }
                                    }
                                });

                                DObjectRepository.DelDObject(Session, item, null);
                                OpenIssueRepository.UdtOpenIssueDelSuspenseCnt(_param);
                            }
                            else
                            {
                                DObjectRepository.UdtDObject(Session, item);
                                OpenIssueItemRepository.UdtOpenIssueItem(item);
                            }
                        }
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
            Library oemKey = LibraryRepository.SelCodeLibraryObject(new Library { Code1 = "OEM" });
            List<Library> oemList = LibraryRepository.SelCodeLibrary(new Library { FromOID = oemKey.OID });
            ViewBag.oemList = oemList;

            Library occurrenceKey = LibraryRepository.SelLibraryObject(new Library { Name = "OCCURRENCE" });
            List<Library> occurrenceList = LibraryRepository.SelLibrary(new Library { FromOID = occurrenceKey.OID, IsUse = "Y" });  // 발생유형
            ViewBag.occurrenceList = occurrenceList;

            Library ItemKey = LibraryRepository.SelCodeLibraryObject(new Library { Code1 = CommonConstant.ATTRIBUTE_ITEM });
            List<Library> ItemList = LibraryRepository.SelCodeLibrary(new Library { FromOID = ItemKey.OID });  //OEM 목록
            ViewBag.ItemList = ItemList;
                      

            //ViewBag.Status = from x in BPolicyRepository.SelBPolicy(new BPolicy { Type = QmsConstant.TYPE_QUICK_RESPONSE })
            //                 where x.Name != "Disposal"
            //                 select x;

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

        public ActionResult CreateQuickResponse(int? ProjectOID)
        {
            //Library oemKey = LibraryRepository.SelLibraryObject(new Library { Name = "OEM" });
            //List<Library> oemList = LibraryRepository.SelLibrary(new Library { FromOID = oemKey.OID });  // OEM
            Library factoryKey = LibraryRepository.SelLibraryObject(new Library { Name = "FACTORY_TYPE" });
            List<Library> factoryList = LibraryRepository.SelLibrary(new Library { FromOID = factoryKey.OID });
            ViewBag.factoryList = factoryList;

            Library oemKey = LibraryRepository.SelCodeLibraryObject(new Library { Code1 = "OEM" }); // OEM
            List<Library> oemList = LibraryRepository.SelCodeLibrary(new Library { FromOID = oemKey.OID });
            ViewBag.oemList = oemList;

            //Library plantKey = LibraryRepository.SelLibraryObject(new Library { Name = "PLANT" });
            //List<Library> plantList = LibraryRepository.SelLibrary(new Library { FromOID = oemKey.OID });  // 공장 구분으로 변경해야함
            //ViewBag.plantList = plantList;

            Library occurrenceKey = LibraryRepository.SelLibraryObject(new Library { Name = "OCCURRENCE_TYPE" });
            List<Library> occurrenceList = LibraryRepository.SelLibrary(new Library { FromOID = occurrenceKey.OID, IsUse = "Y" });  // 발생유형
            ViewBag.occurrenceList = occurrenceList;

            /*Library occurrenceAreaKey = LibraryRepository.SelLibraryObject(new Library { Name = "OCCURRENCE_AREA" });
            List<Library> occurrenceAreaList = LibraryRepository.SelLibrary(new Library { FromOID = occurrenceAreaKey.OID });  // 발생처
            ViewBag.occurrenceAreaList = occurrenceAreaList;*/

            Library induceKey = LibraryRepository.SelLibraryObject(new Library { Name = "INDUCE" });
            List<Library> induceList = LibraryRepository.SelLibrary(new Library { FromOID = induceKey.OID });  // 유발공정
            ViewBag.induceList = induceList;

            Library defectDegreeKey = LibraryRepository.SelLibraryObject(new Library { Name = "DEFECT_DEGREE" });
            List<Library> defectDegreeList = LibraryRepository.SelLibrary(new Library { FromOID = defectDegreeKey.OID });  // 결함정도
            ViewBag.defectDegreeList = defectDegreeList;

            Library imputeKey = LibraryRepository.SelLibraryObject(new Library { Name = "IMPUTE_TYPE" });
            List<Library> imputeList = LibraryRepository.SelLibrary(new Library { FromOID = imputeKey.OID });  // 귀책구분
            ViewBag.imputeList = imputeList;

            Library imputeDeptKey = LibraryRepository.SelLibraryObject(new Library { Name = "IMPUTE_DEPART" });
            List<Library> imputeDeptList = LibraryRepository.SelLibrary(new Library { FromOID = imputeDeptKey.OID });  // 귀책구분
            ViewBag.imputeDeptList = imputeDeptList;

            Library correctDecisionKey = LibraryRepository.SelLibraryObject(new Library { Name = "CORRECT_DECISION" });
            List<Library> correctDecisionList = LibraryRepository.SelLibrary(new Library { FromOID = correctDecisionKey.OID });  // 시정판정
            ViewBag.correctDecisionList = correctDecisionList;
            ViewBag.ProjectOID = ProjectOID;

            Library EnrollmentTypeKey = LibraryRepository.SelLibraryObject(new Library { Name = "ENROLLMENT_TYPE" });
            List<Library> EnrollmentTypeList = LibraryRepository.SelLibrary(new Library { FromOID = EnrollmentTypeKey.OID });  // 등록구분
            ViewBag.EnrollmentTypeList = EnrollmentTypeList;

            return View();
        }

        public ActionResult InfoQuickResponse(int OID)
        {
            //Library oemKey = LibraryRepository.SelLibraryObject(new Library { Name = "OEM" });
            //List<Library> oemList = LibraryRepository.SelLibrary(new Library { FromOID = oemKey.OID });  // OEM
            Library factoryKey = LibraryRepository.SelLibraryObject(new Library { Name = "FACTORY_TYPE" });
            List<Library> factoryList = LibraryRepository.SelLibrary(new Library { FromOID = factoryKey.OID });
            ViewBag.factoryList = factoryList;

            Library oemKey = LibraryRepository.SelCodeLibraryObject(new Library { Code1 = "OEM" });
            List<Library> oemList = LibraryRepository.SelCodeLibrary(new Library { FromOID = oemKey.OID });
            ViewBag.oemList = oemList;

            //Library plantKey = LibraryRepository.SelLibraryObject(new Library { Name = "PLANT" });
            //List<Library> plantList = LibraryRepository.SelLibrary(new Library { FromOID = oemKey.OID });  // 공장 구분으로 변경해야함
            //ViewBag.plantList = plantList;

            Library occurrenceKey = LibraryRepository.SelLibraryObject(new Library { Name = "OCCURRENCE_TYPE" });
            List<Library> occurrenceList = LibraryRepository.SelLibrary(new Library { FromOID = occurrenceKey.OID, IsUse = "Y" });  // 발생유형
            ViewBag.occurrenceList = occurrenceList;

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

            Library EnrollmentTypeKey = LibraryRepository.SelLibraryObject(new Library { Name = "ENROLLMENT_TYPE" });
            List<Library> EnrollmentTypeList = LibraryRepository.SelLibrary(new Library { FromOID = EnrollmentTypeKey.OID });  // 등록구분
            ViewBag.EnrollmentTypeList = EnrollmentTypeList;

            ViewBag.QuickDetail = QuickResponseRepository.SelQuickResponse(new QuickResponse() { OID = OID });
            ViewBag.Status = BPolicyRepository.SelBPolicy(new BPolicy { Type = QmsConstant.TYPE_QUICK_RESPONSE });

            Library occurrenceAreaKey = LibraryRepository.SelLibraryObject(new Library { Name = ViewBag.QuickDetail.OccurrenceNm == "사내" ? "OCCURRENCE_AREA_INSIDE" : "OCCURRENCE_AREA_OUT" });
            List<Library> occurrenceAreaList = LibraryRepository.SelLibrary(new Library { FromOID = occurrenceAreaKey.OID });  // 발생처
            ViewBag.occurrenceAreaList = occurrenceAreaList;


            return View();
        }

        public ActionResult InfoQuickResponseDetail(int OID)
        {
            ViewBag.QuickResponse = QuickResponseRepository.SelQuickResponse(new QuickResponse() { OID = OID });

            List<QuickResponseModule> Modulelist = QuickResponseModuleRepository.SelQuickResponseModules(new QuickResponseModule() { QuickOID = OID });

            Modulelist.ForEach(v =>
            {
                if (v.ModuleType == QmsConstant.TYPE_BLOCKADE)
                {
                    ViewBag.Blockade = v;
                    ViewBag.BlockadeFl = v.ModuleFl;
                    ViewBag.BlockadeItems = BlockadeItemRepository.SelBlockadeItems(new BlockadeItem() { ModuleOID = v.OID });
                    ViewBag.BlockadeApprvalData = ApprovalRepository.SelApproval(Session, new Approval { TargetOID = v.OID });
                }
                else if (v.ModuleType == QmsConstant.TYPE_OCCURRENCE_CAUSE)
                {
                    ViewBag.OccurrenceCause = v;
                    ViewBag.OccurrenceCauseFl = v.ModuleFl;
                    List<OccurrenceCauseItem> OccurrenceCauseItems = OccurrenceCauseItemRepository.SelOccurrenceCauseItems(new OccurrenceCauseItem() { ModuleOID = v.OID });
                    OccurrenceCauseItems.ForEach(i =>
                    {
                        i.OccurrenceWhys = OccurrenceWhyRepository.SelOccurrenceWhys(new OccurrenceWhy() { CauseOID = i.OID });
                    });
                    ViewBag.OccurrenceCauseItems = OccurrenceCauseItems;
                    ViewBag.DetectCounterMeasure = DetectCounterMeasureRepository.SelDetectCounterMeasure(new DetectCounterMeasure() { ModuleOID = v.OID });
                    if (ViewBag.DetectCounterMeasure == null)
                    {
                        ViewBag.DetectCounterMeasure = new DetectCounterMeasure();
                    }
                    ViewBag.OccurrenceCauseApprvalData = ApprovalRepository.SelApproval(Session, new Approval { TargetOID = v.OID });
                }
                else if (v.ModuleType == QmsConstant.TYPE_IMPROVE_COUNTERMEASURE)
                {
                    ViewBag.ImproveCountermeasure = v;
                    ViewBag.ImproveCountermeasureFl = v.ModuleFl;
                    ViewBag.ImproveCounterMeasureItems = ImproveCounterMeasureItemRepository.SelImproveCounterMeasureItems(new ImproveCounterMeasureItem() { ModuleOID = v.OID });
                    ViewBag.ImproveCountermeasureApprvalData = ApprovalRepository.SelApproval(Session, new Approval { TargetOID = v.OID });
                }
                else if (v.ModuleType == QmsConstant.TYPE_ERROR_PRROF)
                {
                    ViewBag.ErrorProof = ErrorProofRepository.SelErrorProof(new ErrorProof() { ModuleOID = v.OID });
                    ViewBag.ErrorProofFl = v.ModuleFl;
                    ViewBag.ErrorProofApprvalData = ApprovalRepository.SelApproval(Session, new Approval { TargetOID = v.OID });
                }
                else if (v.ModuleType == QmsConstant.TYPE_LPA_UNFIT)
                {
                    ViewBag.LpaUnfit = LpaUnfitRepository.SelLpaUnfit(new LpaUnfit() { ModuleOID = v.OID });
                    ViewBag.LpaUnfitFl = v.ModuleFl;
                    ViewBag.LpaUnfitCheck = LpaUnfitCheckRepository.SelLpaUnfitChecks(new LpaUnfitCheck() { ModuleOID = v.OID });
                }
                else if (v.ModuleType == QmsConstant.TYPE_LPA_MEASURE)
                {
                    ViewBag.LpaMeasure = LpaMeasureRepository.SelLpaMeasure(new LpaMeasure() { ModuleOID = v.OID });
                    ViewBag.LpaMeasureApprvalData = ApprovalRepository.SelApproval(Session, new Approval { TargetOID = v.OID });
                }
                else if (v.ModuleType == QmsConstant.TYPE_QUICK_RESPONSE_CHECK)
                {
                    ViewBag.QmsCheck = v;
                    ViewBag.QmsCheckFl = v.ModuleFl;
                    ViewBag.QmsCheckItems = QmsCheckRepository.SelQmsChecks(new QmsCheck() { ModuleOID = v.OID });
                    ViewBag.QmsCheckApprvalData = ApprovalRepository.SelApproval(Session, new Approval { TargetOID = v.OID });
                }
                else if (v.ModuleType == QmsConstant.TYPE_STANDARD)
                {
                    ViewBag.StandardDoc = v;
                    ViewBag.StandardDocFl = v.ModuleFl;
                    ViewBag.StandardDocItem = StandardDocRepository.SelStandardDocs(new StandardDoc() { ModuleOID = v.OID });
                    ViewBag.StandardDocApprvalData = ApprovalRepository.SelApproval(Session, new Approval { TargetOID = v.OID });
                }
                else if (v.ModuleType == QmsConstant.TYPE_WORKER_EDU)
                {
                    ViewBag.WorkerEdu = WorkerEduRepository.SelWorkerEdu(new WorkerEdu() { ModuleOID = v.OID });
                    ViewBag.WorkerEduFl = v.ModuleFl;
                    ViewBag.WorkerEduApprvalData = ApprovalRepository.SelApproval(Session, new Approval { TargetOID = v.OID });
                }
            });

            return View(OID);
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

            if (_param.ProjectOID != null)
            {
                PmsProject pmsProject = PmsProjectRepository.SelPmsObject(Session, new PmsProject() { OID = _param.ProjectOID });

                List<DRelationship> relationships = DRelationshipRepository.SelRelationship(Session, new DRelationship() { FromOID = _param.ProjectOID });

                List<QuickResponse> _list = new List<QuickResponse>();

                relationships.ForEach(v =>
                {
                    QuickResponse data = list.Find(q => q.OID == v.ToOID);

                    if (data != null)
                    {
                        data.CarCode = pmsProject.Car_Lib_Nm;
                        _list.Add(data);
                    }
                });

                list = _list;
            }
            else
            {
                List<PmsProject> lPmses = PmsProjectRepository.SelPmsObjects(Session, new PmsProject());
                List<DRelationship> QuickResponseRelation = DRelationshipRepository.SelRelationship(Session, new DRelationship() { Type = QmsConstant.RELATIONSHIP_QUICK_RESPONSE });

                List<QuickResponse> _list = new List<QuickResponse>();

                list.ForEach(v =>
                {
                    DRelationship rel = QuickResponseRelation.Find(r => r.ToOID == v.OID);
                    if (rel == null) return;

                    PmsProject pms = lPmses.Find(p => p.OID == rel.FromOID);

                    if (pms != null)
                    {
                        v.CarCode = pms.Car_Lib_Nm;

                        if (_param.SearchCarCode != null)
                        {
                            if (v.CarCode.IndexOf(_param.SearchCarCode.Trim()) > -1)
                            {
                                _list.Add(v);
                            }
                        }
                    }
                });

                if (_param.SearchCarCode != null) { list = _list; }

            }

            List<QuickResponseModule> Modulelist = QuickResponseModuleRepository.SelQuickResponseModules(new QuickResponseModule());

            List<QuickResponseView> gridView = new List<QuickResponseView>();

            list.ForEach(v =>
            {
                QuickResponseView quickResponseView = new QuickResponseView(v);

                var Modules = from m in Modulelist
                              where m.QuickOID == v.OID
                              select m;

                string QuickResponseStatus = string.Empty;

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

                        if (m.BPolicyNm == QmsConstant.POLICY_QMS_MODULE_STARTED || m.BPolicyNm == QmsConstant.POLICY_QMS_MODULE_REVIEW || m.BPolicyNm == QmsConstant.POLICY_QMS_MODULE_REJECTED)
                        {
                            QuickResponseStatus = QmsConstant.TYPE_BLOCKADE_NAME;
                        }
                    }
                    else if (m.ModuleType == QmsConstant.TYPE_OCCURRENCE_CAUSE)
                    {
                        quickResponseView.ModuleOccurrenceCauseOID = m.OID;
                        quickResponseView.ModuleOccurrenceCauseStatusNm = m.BPolicyNm;
                        quickResponseView.ModuleOccurrenceCauseFl = m.ModuleFl;
                        quickResponseView.ModuleOccurrenceCauseEstEndDt = m.EstEndDt;
                        quickResponseView.ModuleOccurrenceCauseChargeUserOID = m.ChargeUserOID;
                        quickResponseView.ModuleOccurrenceCauseChargeUserNm = m.ChargeUserNm;

                        if (m.BPolicyNm == QmsConstant.POLICY_QMS_MODULE_STARTED || m.BPolicyNm == QmsConstant.POLICY_QMS_MODULE_REVIEW || m.BPolicyNm == QmsConstant.POLICY_QMS_MODULE_REJECTED)
                        {
                            QuickResponseStatus = QmsConstant.TYPE_OCCURRENCE_CAUSE_NAME;
                        }
                    }
                    else if (m.ModuleType == QmsConstant.TYPE_IMPROVE_COUNTERMEASURE)
                    {
                        quickResponseView.ModuleImproveCountermeasureOID = m.OID;
                        quickResponseView.ModuleImproveCountermeasureStatusNm = m.BPolicyNm;
                        quickResponseView.ModuleImproveCountermeasureFl = m.ModuleFl;
                        quickResponseView.ModuleImproveCountermeasureEstEndDt = m.EstEndDt;
                        quickResponseView.ModuleImproveCountermeasureChargeUserOID = m.ChargeUserOID;
                        quickResponseView.ModuleImproveCountermeasureChargeUserNm = m.ChargeUserNm;

                        if (m.BPolicyNm == QmsConstant.POLICY_QMS_MODULE_STARTED || m.BPolicyNm == QmsConstant.POLICY_QMS_MODULE_REVIEW || m.BPolicyNm == QmsConstant.POLICY_QMS_MODULE_REJECTED)
                        {
                            QuickResponseStatus = QmsConstant.TYPE_IMPROVE_COUNTERMEASURE_NAME;
                        }
                    }
                    else if (m.ModuleType == QmsConstant.TYPE_ERROR_PRROF)
                    {
                        quickResponseView.ModuleErrorProofOID = m.OID;
                        quickResponseView.ModuleErrorProofStatusNm = m.BPolicyNm;
                        quickResponseView.ModuleErrorProofFl = m.ModuleFl;
                        quickResponseView.ModuleErrorProofEstEndDt = m.EstEndDt;
                        quickResponseView.ModuleErrorProofChargeUserOID = m.ChargeUserOID;
                        quickResponseView.ModuleErrorProofChargeUserNm = m.ChargeUserNm;

                        if (m.BPolicyNm == QmsConstant.POLICY_QMS_MODULE_STARTED || m.BPolicyNm == QmsConstant.POLICY_QMS_MODULE_REVIEW || m.BPolicyNm == QmsConstant.POLICY_QMS_MODULE_REJECTED)
                        {
                            QuickResponseStatus = QmsConstant.TYPE_ERROR_PRROF_NAME;
                        }
                    }
                    else if (m.ModuleType == QmsConstant.TYPE_LPA_UNFIT)
                    {
                        quickResponseView.ModuleLpaOID = m.OID;
                        quickResponseView.ModuleLpaStatusNm = m.BPolicyNm;
                        quickResponseView.ModuleLpaFl = m.ModuleFl;
                        quickResponseView.ModuleLpaEstEndDt = m.EstEndDt;
                        quickResponseView.ModuleLpaChargeUserOID = m.ChargeUserOID;
                        quickResponseView.ModuleLpaChargeUserNm = m.ChargeUserNm;

                        if (m.BPolicyNm == QmsConstant.POLICY_QMS_MODULE_STARTED || m.BPolicyNm == QmsConstant.POLICY_QMS_MODULE_REVIEW || m.BPolicyNm == QmsConstant.POLICY_QMS_MODULE_REJECTED)
                        {
                            QuickResponseStatus = QmsConstant.TYPE_LPA_UNFIT_NAME;
                        }
                    }
                    else if (m.ModuleType == QmsConstant.TYPE_LPA_MEASURE)
                    {
                        if (m.BPolicyNm == QmsConstant.POLICY_QMS_MODULE_STARTED || m.BPolicyNm == QmsConstant.POLICY_QMS_MODULE_REVIEW || m.BPolicyNm == QmsConstant.POLICY_QMS_MODULE_REJECTED)
                        {
                            QuickResponseStatus = QmsConstant.TYPE_LPA_MEASURE_NAME;
                        }
                    }
                    else if (m.ModuleType == QmsConstant.TYPE_QUICK_RESPONSE_CHECK)
                    {
                        quickResponseView.ModuleCheckOID = m.OID;
                        quickResponseView.ModuleCheckStatusNm = m.BPolicyNm;
                        quickResponseView.ModuleCheckFl = m.ModuleFl;
                        quickResponseView.ModuleCheckEstEndDt = m.EstEndDt;
                        quickResponseView.ModuleCheckChargeUserOID = m.ChargeUserOID;
                        quickResponseView.ModuleCheckChargeUserNm = m.ChargeUserNm;

                        if (m.BPolicyNm == QmsConstant.POLICY_QMS_MODULE_STARTED || m.BPolicyNm == QmsConstant.POLICY_QMS_MODULE_REVIEW || m.BPolicyNm == QmsConstant.POLICY_QMS_MODULE_REJECTED)
                        {
                            QuickResponseStatus = QmsConstant.TYPE_QUICK_RESPONSE_CHECK_NAME;
                        }
                    }
                    else if (m.ModuleType == QmsConstant.TYPE_STANDARD)
                    {
                        quickResponseView.ModuleStandardOID = m.OID;
                        quickResponseView.ModuleStandardStatusNm = m.BPolicyNm;
                        quickResponseView.ModuleStandardFl = m.ModuleFl;
                        quickResponseView.ModuleStandardEstEndDt = m.EstEndDt;
                        quickResponseView.ModuleStandardChargeUserOID = m.ChargeUserOID;
                        quickResponseView.ModuleStandardChargeUserNm = m.ChargeUserNm;

                        if (m.BPolicyNm == QmsConstant.POLICY_QMS_MODULE_STARTED || m.BPolicyNm == QmsConstant.POLICY_QMS_MODULE_REVIEW || m.BPolicyNm == QmsConstant.POLICY_QMS_MODULE_REJECTED)
                        {
                            QuickResponseStatus = QmsConstant.TYPE_STANDARD_NAME;
                        }
                    }
                    else if (m.ModuleType == QmsConstant.TYPE_WORKER_EDU)
                    {
                        quickResponseView.ModuleWorkerEduOID = m.OID;
                        quickResponseView.ModuleWorkerEduStatusNm = m.BPolicyNm;
                        quickResponseView.ModuleWorkerEduFl = m.ModuleFl;
                        quickResponseView.ModuleWorkerEduEstEndDt = m.EstEndDt;
                        quickResponseView.ModuleWorkerEduChargeUserOID = m.ChargeUserOID;
                        quickResponseView.ModuleWorkerEduChargeUserNm = m.ChargeUserNm;

                        if (m.BPolicyNm == QmsConstant.POLICY_QMS_MODULE_STARTED || m.BPolicyNm == QmsConstant.POLICY_QMS_MODULE_REVIEW || m.BPolicyNm == QmsConstant.POLICY_QMS_MODULE_REJECTED)
                        {
                            QuickResponseStatus = QmsConstant.TYPE_WORKER_EDU_NAME;
                        }
                    }
                });

                if (v.BPolicyNm == QmsConstant.POLICY_QMS_MODULE_PREPARE)
                {
                    QuickResponseStatus = "일정관리";
                }
                else if (v.BPolicyNm == QmsConstant.POLICY_QMS_MODULE_COMPLETED)
                {
                    QuickResponseStatus = "완료";
                }

                quickResponseView.StatusNm = QuickResponseStatus;

                gridView.Add(quickResponseView);
            });

            return Json(gridView);
        }

        public JsonResult InsQuickResponse(QuickResponse _param)
        {
            int result = 0;
            DirectoryInfo di = null;
            try
            {
                DaoFactory.BeginTransaction();
                _param.Type = QmsConstant.TYPE_QUICK_RESPONSE;

                DObject dobj = new DObject();
                dobj.Type = _param.Type;
                _param.Name = DateTime.Now.Ticks.ToString(); // 채번필요
                dobj.Name = _param.Name;
                result = DObjectRepository.InsDObject(Session, dobj);

                _param.OID = result;
                int returnValue = QuickResponseRepository.InsQuickResponse(_param);

                if (_param.ProjectOID != null)
                {
                    DRelationship dQuickProject = new DRelationship();
                    dQuickProject.Type = QmsConstant.RELATIONSHIP_QUICK_RESPONSE;
                    dQuickProject.FromOID = _param.ProjectOID;
                    dQuickProject.ToOID = _param.OID;
                    DRelationshipRepository.InsDRelationshipNotOrd(Session, dQuickProject);
                }

                int SetQuickModule(string Type, string Name)
                {
                    DObject dobjModule = new DObject();
                    dobjModule.Type = Type;
                    dobjModule.Name = Name;
                    int ModuleOID = DObjectRepository.InsDObject(Session, dobjModule);
                    QuickResponseModuleRepository.InsQuickResponseModule(new QuickResponseModule() { QuickOID = result, OID = ModuleOID });

                    DRelationship dRelModule = new DRelationship();
                    dRelModule.Type = QmsConstant.RELATIONSHIP_QUICK_MODULE;
                    dRelModule.FromOID = _param.OID;
                    dRelModule.ToOID = ModuleOID;
                    DRelationshipRepository.InsDRelationshipNotOrd(Session, dRelModule);

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
                    DRelationshipRepository.InsDRelationshipNotOrd(Session, dRelModule);
                }

                // 봉쇄조치
                int blockadeOID = SetQuickModule(QmsConstant.TYPE_BLOCKADE, QmsConstant.TYPE_BLOCKADE_NAME);

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
                SetQuickModule(QmsConstant.TYPE_OCCURRENCE_CAUSE, QmsConstant.TYPE_OCCURRENCE_CAUSE_NAME);

                // 개선대책등록
                SetQuickModule(QmsConstant.TYPE_IMPROVE_COUNTERMEASURE, QmsConstant.TYPE_IMPROVE_COUNTERMEASURE_NAME);

                // ERROR PROOF
                int ErrorProofOID = SetQuickModule(QmsConstant.TYPE_ERROR_PRROF, QmsConstant.TYPE_ERROR_PRROF_NAME);

                ErrorProofRepository.InsErrorProof(new ErrorProof() { ModuleOID = ErrorProofOID });

                // LPA 부적합 등록
                int LpaUnfitOID = SetQuickModule(QmsConstant.TYPE_LPA_UNFIT, QmsConstant.TYPE_LPA_UNFIT_NAME);
                LpaUnfitRepository.InsLpaUnfit(new LpaUnfit() { ModuleOID = LpaUnfitOID });

                // LPA 부적합 대책서 
                int LpaUnMeasureOID = SetQuickModule(QmsConstant.TYPE_LPA_MEASURE, QmsConstant.TYPE_LPA_MEASURE_NAME);
                LpaMeasureRepository.InsLpaMeasure(new LpaMeasure() { ModuleOID = LpaUnMeasureOID });

                DRelationship dRelLpa = new DRelationship();
                dRelLpa.Type = QmsConstant.RELATIONSHIP_LPA;
                dRelLpa.FromOID = LpaUnfitOID;
                dRelLpa.ToOID = LpaUnMeasureOID;
                DRelationshipRepository.InsDRelationshipNotOrd(Session, dRelLpa);

                // 유효성 체크
                SetQuickModule(QmsConstant.TYPE_QUICK_RESPONSE_CHECK, QmsConstant.TYPE_QUICK_RESPONSE_CHECK_NAME);

                // 표준화&Follow-Up 조치 등록
                SetQuickModule(QmsConstant.TYPE_STANDARD, QmsConstant.TYPE_STANDARD_NAME);

                // 사용자 교육
                int WorkerEduOID = SetQuickModule(QmsConstant.TYPE_WORKER_EDU, QmsConstant.TYPE_WORKER_EDU_NAME);
                WorkerEduRepository.InsWorkerEdu(new WorkerEdu() { ModuleOID = WorkerEduOID });

                // DB작업이 끝나면 고품 사진 Temp -> Vault로 이동
                if (!string.IsNullOrEmpty(_param.PoorPicture))
                {
                    string StoragePath = System.Web.HttpContext.Current.Server.MapPath(System.Configuration.ConfigurationManager.AppSettings["FileStorage"]);
                    string imgTempPath = System.Configuration.ConfigurationManager.AppSettings["ImageTempPath"];
                    string TempPath = QmsConstant.TYPE_QUICK_RESPONSE;

                    string imgVaulePath = System.Configuration.ConfigurationManager.AppSettings["ImageValutPath"];
                    string SavePath = QmsConstant.TYPE_QUICK_RESPONSE + "\\" + _param.OID;

                    di = new DirectoryInfo(StoragePath + "/" + imgVaulePath + "/" + SavePath);

                    if (!di.Exists)
                    {
                        di.Create();
                    }

                    FileInfo FileInfo = new FileInfo(StoragePath + "\\" + imgTempPath + "\\" + TempPath + "\\" + _param.PoorPicture);

                    if (FileInfo.Exists)
                    {
                        FileInfo.MoveTo(StoragePath + "\\" + imgVaulePath + "\\" + SavePath + "\\" + _param.PoorPicture);
                    }
                }

                if (_param.Files != null)
                {
                    HttpFileRepository.InsertData(Session, _param);
                }

                if (_param.delFiles != null)
                {
                    _param.delFiles.ForEach(v =>
                    {
                        HttpFileRepository.DeleteData(Session, v);
                    });
                }

                DaoFactory.Commit();
            }
            catch (Exception ex)
            {
                DaoFactory.Rollback();
                if (di != null && !di.Exists)
                {
                    di.Delete();
                }
                return Json(new ResultJsonModel { isError = true, resultMessage = ex.Message, resultDescription = ex.ToString() });
            }
            return Json(QuickResponseRepository.SelQuickResponse(new QuickResponse() { OID = result }));
        }

        public JsonResult UdtQuickResponse(QuickResponse _param)
        {
            int result = 0;
            DirectoryInfo di = null;
            try
            {
                DaoFactory.BeginTransaction();
                _param.Type = QmsConstant.TYPE_QUICK_RESPONSE;

                DObject dobj = new DObject();
                dobj.Type = _param.Type;
                dobj.OID = _param.OID;
                DObjectRepository.UdtDObject(Session, dobj);

                QuickResponseRepository.UdtQuickResponse(_param);

                if (_param.Files != null)
                {
                    HttpFileRepository.InsertData(Session, _param);
                }

                if (_param.delFiles != null)
                {
                    _param.delFiles.ForEach(v =>
                    {
                        HttpFileRepository.DeleteData(Session, v);
                    });
                }

                // DB작업이 끝나면 고품 사진 Temp -> Vault로 이동
                if (!string.IsNullOrEmpty(_param.PoorPicture))
                {
                    string StoragePath = System.Web.HttpContext.Current.Server.MapPath(System.Configuration.ConfigurationManager.AppSettings["FileStorage"]);
                    string imgTempPath = System.Configuration.ConfigurationManager.AppSettings["ImageTempPath"];
                    string TempPath = QmsConstant.TYPE_QUICK_RESPONSE;

                    string imgVaulePath = System.Configuration.ConfigurationManager.AppSettings["ImageValutPath"];
                    string SavePath = QmsConstant.TYPE_QUICK_RESPONSE + "\\" + _param.OID;

                    di = new DirectoryInfo(StoragePath + "/" + imgVaulePath + "/" + SavePath);

                    if (!di.Exists)
                    {
                        di.Create();
                    }

                    FileInfo FileInfo = new FileInfo(StoragePath + "\\" + imgTempPath + "\\" + TempPath + "\\" + _param.PoorPicture);

                    if (FileInfo.Exists)
                    {
                        FileInfo.MoveTo(StoragePath + "\\" + imgVaulePath + "\\" + SavePath + "\\" + _param.PoorPicture);
                    }
                }

                DaoFactory.Commit();
            }
            catch (Exception ex)
            {
                DaoFactory.Rollback();

                // 폴더안에 아무 고픔사진도 없다면(생성 당시에 고품사진 없이 생성함) 폴더 삭제
                if (di != null && !di.Exists && di.GetFiles().Length > 0)
                {
                    di.Delete();
                }

                return Json(new ResultJsonModel { isError = true, resultMessage = ex.Message, resultDescription = ex.ToString() });
            }
            return Json(result);
        }

        #region -- 고품사진
        public JsonResult ImgUploadFile(int? OID)
        {
            string StoragePath = System.Web.HttpContext.Current.Server.MapPath(System.Configuration.ConfigurationManager.AppSettings["FileStorage"]);

            string imgVaulePath = "";
            string SavePath = QmsConstant.TYPE_QUICK_RESPONSE + "/";
            if (OID == null)
            {
                imgVaulePath = System.Configuration.ConfigurationManager.AppSettings["ImageTempPath"];
            }
            else
            {
                imgVaulePath = System.Configuration.ConfigurationManager.AppSettings["ImageValutPath"];
                SavePath += OID.ToString();
            }

            var fileName = "";
            if (Request.Files.Count > 0)
            {
                HttpFileCollectionBase files = Request.Files;
                HttpPostedFileBase File = files[0];
                try
                {
                    DirectoryInfo di = new DirectoryInfo(StoragePath + "/" + imgVaulePath + "/" + SavePath);

                    if (!di.Exists)
                    {
                        di.Create();
                    }

                    System.TimeSpan tspan = System.TimeSpan.FromTicks(DateTime.Now.Ticks);
                    long curTime = (long)tspan.TotalSeconds;
                    string file_nm = Path.GetFileName(File.FileName);
                    string file_format = file_nm.Substring(file_nm.LastIndexOf(".") + 1);
                    string cfile_nm = curTime.ToString() + "." + file_format;

                    FileStream fs = System.IO.File.Create(StoragePath + "/" + imgVaulePath + "/" + SavePath + "\\" + cfile_nm);
                    File.InputStream.CopyTo(fs);

                    fileName = cfile_nm;

                    fs.Close();
                    File.InputStream.Close();

                    if (OID != null)
                    {
                        QuickResponse quickResponse = new QuickResponse()
                        {
                            OID = OID,
                            PoorPicture = fileName,
                        };
                        QuickResponseRepository.UdtQuickResponse(quickResponse);
                    }
                }
                catch (Exception ex)
                {
                    return Json(new ResultJsonModel { isError = true, resultMessage = ex.Message, resultDescription = ex.ToString() });
                }
            }
            return Json(fileName);
        }

        [HttpGet]
        public ActionResult ImgFileDownload(int? OID, string fileName)
        {
            try
            {
                string StoragePath = System.Web.HttpContext.Current.Server.MapPath(System.Configuration.ConfigurationManager.AppSettings["FileStorage"]);
                string imgVaulePath = System.Configuration.ConfigurationManager.AppSettings["ImageValutPath"];
                string SavePath = QmsConstant.TYPE_QUICK_RESPONSE + "/" + OID;

                string fileFullDirectory = Path.Combine(StoragePath, imgVaulePath, SavePath, fileName);

                FileInfo fi = new FileInfo(fileFullDirectory);

                if (!fi.Exists)
                {
                    throw new Exception("파일이 존재하지않습니다.");
                }

                System.IO.Stream fileStream = new FileStream(fi.FullName, FileMode.Open); ;

                string downloadImgName = string.Format("{0}.{1}", "고품사진", fileName.Substring(fileName.LastIndexOf(".") + 1));

                if (Request.Browser.Browser == "IE" || Request.Browser.Browser == "InternetExplorer")
                {
                    return File(fileStream, MediaTypeNames.Application.Octet, HttpUtility.UrlEncode(downloadImgName, System.Text.Encoding.UTF8));
                }
                else
                {
                    return File(fileStream, MediaTypeNames.Application.Octet, downloadImgName);
                }

            }
            catch (Exception ex)
            {
                string message = ex.Message.Replace("'", "");
                return Content("<script language='javascript' type='text/javascript'>alert('" + message + "');history.back();</script>");
            }
        }
        #endregion

        [HttpGet]
        public ActionResult ExcelExportQuickResponseDetail(int OID)
        {
            QuickResponse QuickResponse = QuickResponseRepository.SelQuickResponse(new QuickResponse() { OID = OID });

            List<QuickResponseModule> Modulelist = QuickResponseModuleRepository.SelQuickResponseModules(new QuickResponseModule() { QuickOID = OID });

            QuickResponseModule Blockade = null;
            int? BlockadeFl = null;
            List<BlockadeItem> BlockadeItems = null;
            Approval BlockadeApprvalData = null;

            QuickResponseModule OccurrenceCause = null;
            int? OccurrenceCauseFl = null;
            List<OccurrenceCauseItem> OccurrenceCauseItems = null;
            DetectCounterMeasure DetectCounterMeasure = null;
            Approval OccurrenceCauseApprvalData = null;

            QuickResponseModule ImproveCountermeasure = null;
            int? ImproveCountermeasureFl = null;
            List<ImproveCounterMeasureItem> ImproveCounterMeasureItems = null;
            Approval ImproveCountermeasureApprvalData = null;

            ErrorProof ErrorProof = null;
            int? ErrorProofFl = null;
            Approval ErrorProofApprvalData = null;

            LpaUnfit LpaUnfit = null;
            int? LpaUnfitFl = null;
            List<LpaUnfitCheck> LpaUnfitCheck = null;

            LpaMeasure LpaMeasure = null;
            Approval LpaMeasureApprvalData = null;

            QuickResponseModule QmsCheck = null;
            int? QmsCheckFl = null;
            List<QmsCheck> QmsCheckItems = null;
            Approval QmsCheckApprvalData = null;

            QuickResponseModule StandardDoc = null;
            int? StandardDocFl = null;
            List<StandardDoc> StandardDocItem = null;
            Approval StandardDocApprvalData = null;

            WorkerEdu WorkerEdu = null;
            int? WorkerEduFl = null;
            Approval WorkerEduApprvalData = null;

            Modulelist.ForEach(v =>
            {
                if (v.ModuleType == QmsConstant.TYPE_BLOCKADE)
                {
                    Blockade = v;
                    BlockadeFl = v.ModuleFl;
                    BlockadeItems = BlockadeItemRepository.SelBlockadeItems(new BlockadeItem() { ModuleOID = v.OID });
                    BlockadeApprvalData = ApprovalRepository.SelApproval(Session, new Approval { TargetOID = v.OID });
                }
                else if (v.ModuleType == QmsConstant.TYPE_OCCURRENCE_CAUSE)
                {
                    OccurrenceCause = v;
                    OccurrenceCauseFl = v.ModuleFl;
                    List<OccurrenceCauseItem> _OccurrenceCauseItems = OccurrenceCauseItemRepository.SelOccurrenceCauseItems(new OccurrenceCauseItem() { ModuleOID = v.OID });
                    _OccurrenceCauseItems.ForEach(i =>
                    {
                        i.OccurrenceWhys = OccurrenceWhyRepository.SelOccurrenceWhys(new OccurrenceWhy() { CauseOID = i.OID });
                    });
                    OccurrenceCauseItems = _OccurrenceCauseItems;
                    DetectCounterMeasure = DetectCounterMeasureRepository.SelDetectCounterMeasure(new DetectCounterMeasure() { ModuleOID = v.OID });
                    if (DetectCounterMeasure == null)
                    {
                        DetectCounterMeasure = new DetectCounterMeasure();
                    }
                    OccurrenceCauseApprvalData = ApprovalRepository.SelApproval(Session, new Approval { TargetOID = v.OID });
                }
                else if (v.ModuleType == QmsConstant.TYPE_IMPROVE_COUNTERMEASURE)
                {
                    ImproveCountermeasure = v;
                    ImproveCountermeasureFl = v.ModuleFl;
                    ImproveCounterMeasureItems = ImproveCounterMeasureItemRepository.SelImproveCounterMeasureItems(new ImproveCounterMeasureItem() { ModuleOID = v.OID });
                    ImproveCountermeasureApprvalData = ApprovalRepository.SelApproval(Session, new Approval { TargetOID = v.OID });
                }
                else if (v.ModuleType == QmsConstant.TYPE_ERROR_PRROF)
                {
                    ErrorProof = ErrorProofRepository.SelErrorProof(new ErrorProof() { ModuleOID = v.OID });
                    ErrorProofFl = v.ModuleFl;
                    ErrorProofApprvalData = ApprovalRepository.SelApproval(Session, new Approval { TargetOID = v.OID });
                }
                else if (v.ModuleType == QmsConstant.TYPE_LPA_UNFIT)
                {
                    LpaUnfit = LpaUnfitRepository.SelLpaUnfit(new LpaUnfit() { ModuleOID = v.OID });
                    LpaUnfitFl = v.ModuleFl;
                    LpaUnfitCheck = LpaUnfitCheckRepository.SelLpaUnfitChecks(new LpaUnfitCheck() { ModuleOID = v.OID });
                }
                else if (v.ModuleType == QmsConstant.TYPE_LPA_MEASURE)
                {
                    LpaMeasure = LpaMeasureRepository.SelLpaMeasure(new LpaMeasure() { ModuleOID = v.OID });
                    LpaMeasureApprvalData = ApprovalRepository.SelApproval(Session, new Approval { TargetOID = v.OID });
                }
                else if (v.ModuleType == QmsConstant.TYPE_QUICK_RESPONSE_CHECK)
                {
                    QmsCheck = v;
                    QmsCheckFl = v.ModuleFl;
                    QmsCheckItems = QmsCheckRepository.SelQmsChecks(new QmsCheck() { ModuleOID = v.OID });
                    QmsCheckApprvalData = ApprovalRepository.SelApproval(Session, new Approval { TargetOID = v.OID });
                }
                else if (v.ModuleType == QmsConstant.TYPE_STANDARD)
                {
                    StandardDoc = v;
                    StandardDocFl = v.ModuleFl;
                    StandardDocItem = StandardDocRepository.SelStandardDocs(new StandardDoc() { ModuleOID = v.OID });
                    StandardDocApprvalData = ApprovalRepository.SelApproval(Session, new Approval { TargetOID = v.OID });
                }
                else if (v.ModuleType == QmsConstant.TYPE_WORKER_EDU)
                {
                    WorkerEdu = WorkerEduRepository.SelWorkerEdu(new WorkerEdu() { ModuleOID = v.OID });
                    WorkerEduFl = v.ModuleFl;
                    WorkerEduApprvalData = ApprovalRepository.SelApproval(Session, new Approval { TargetOID = v.OID });
                }
            });

            Stream DownExcelStream = null;
            string newTempFilePath = "";
            string strNewFileName = DateTime.Now.Ticks.ToString() + ".xlsx";

            //string FilePath = System.Web.HttpContext.Current.Server.MapPath(System.Configuration.ConfigurationManager.AppSettings["ExcelTemplatePath"]) + "\\" + DateTime.Now.Ticks.ToString() + ".xlsx";
            var file = new FileInfo(strNewFileName);

            using (ExcelPackage excelPackage = new ExcelPackage(file))
            {
                excelPackage.Workbook.Worksheets.Add("sheet1");
                ExcelWorksheet ws = excelPackage.Workbook.Worksheets.First();

                ws.Column(1).Width = 0.6;
                ws.Column(2).Width = 2;

                for (int iCnt = 3; iCnt <= 24; iCnt++)
                {
                    if (iCnt == 13 || iCnt == 14)
                    {
                        ws.Column(iCnt).Width = 2;
                    }
                    else
                    {
                        ws.Column(iCnt).Width = 3.9;
                    }
                }

                ws.Column(25).Width = 2;
                ws.Column(26).Width = 0.6;

                PrintExcelCell(ws.Cells[2, 3, 2, 5], "TITLE", "제목");
                PrintExcelCell(ws.Cells[2, 6, 2, 24], "VALUE", QuickResponse.Title);

                // 기본정보
                PrintExcelCell(ws.Cells[4, 3, 4, 5], "TITLE", "기본정보");
                PrintExcelCell(ws.Cells[5, 3, 5, 5], "TH", "신속대응NO");
                PrintExcelCell(ws.Cells[5, 6, 5, 13], "TD", QuickResponse.Name);
                PrintExcelCell(ws.Cells[5, 14, 5, 16], "TH", "공장구분");
                PrintExcelCell(ws.Cells[5, 17, 5, 24], "TD", QuickResponse.PlantNm);

                PrintExcelCell(ws.Cells[6, 3, 6, 5], "TH", "발생유형");
                PrintExcelCell(ws.Cells[6, 6, 6, 13], "TD", QuickResponse.OccurrenceNm);
                PrintExcelCell(ws.Cells[6, 14, 6, 16], "TH", "고객사");
                PrintExcelCell(ws.Cells[6, 17, 6, 24], "TD", QuickResponse.OemNm);

                PrintExcelCell(ws.Cells[7, 3, 7, 5], "TH", "품번/품명");
                PrintExcelCell(ws.Cells[7, 6, 7, 13], "TD", QuickResponse.PartNo + "/" + QuickResponse.PartNm);
                PrintExcelCell(ws.Cells[7, 14, 7, 16], "TH", "고객사");
                PrintExcelCell(ws.Cells[7, 17, 7, 24], "TD", QuickResponse.OemNm);

                PrintExcelCell(ws.Cells[8, 3, 8, 5], "TH", "품질담당자");
                PrintExcelCell(ws.Cells[8, 6, 8, 13], "TD", QuickResponse.WorkUserNm);
                PrintExcelCell(ws.Cells[8, 14, 8, 16], "TH", "작성일시");
                PrintExcelCell(ws.Cells[8, 17, 8, 24], "TD", QuickResponse.WriteDt == null ? "" : ((DateTime)QuickResponse.WriteDt).ToString("yyyy-MM-dd"));

                PrintExcelCell(ws.Cells[9, 3, 9, 5], "TH", "불량수량");
                PrintExcelCell(ws.Cells[9, 6, 9, 24], "TD", QuickResponse.PoorCnt.ToString());

                ws.Cells[5, 3, 9, 24].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Medium);

                // 문제요약
                PrintExcelCell(ws.Cells[11, 3, 11, 5], "TITLE", "문제요약");
                PrintExcelCell(ws.Cells[12, 3, 12, 5], "TH", "발생처");
                PrintExcelCell(ws.Cells[12, 6, 12, 13], "TD", QuickResponse.OccurrenceAreaNm);
                PrintExcelCell(ws.Cells[12, 14, 12, 16], "TH", "결함정도");
                PrintExcelCell(ws.Cells[12, 17, 12, 24], "TD", QuickResponse.DefectDegreeNm);

                PrintExcelCell(ws.Cells[13, 3, 13, 5], "TH", "유발공정");
                PrintExcelCell(ws.Cells[13, 6, 13, 13], "TD", QuickResponse.InduceNm);
                PrintExcelCell(ws.Cells[13, 14, 13, 16], "TH", "귀책구분");
                PrintExcelCell(ws.Cells[13, 17, 13, 24], "TD", QuickResponse.ImputeNm);

                PrintExcelCell(ws.Cells[14, 3, 14, 5], "TH", "귀책처");
                PrintExcelCell(ws.Cells[14, 6, 14, 13], "TD", QuickResponse.ImputeDepartmentOID != null ? QuickResponse.ImputeDepartmentNm : QuickResponse.ImputeSupplierNm);
                PrintExcelCell(ws.Cells[14, 14, 14, 16], "TH", "재발여부");
                PrintExcelCell(ws.Cells[14, 17, 14, 24], "TD", QuickResponse.RecurrenceFl == true ? "Y" : "N");

                PrintExcelCell(ws.Cells[15, 3, 15, 5], "TH", "불량내용상세");
                PrintExcelCell(ws.Cells[15, 6, 15, 24], "TD", QuickResponse.PoorDetail);

                PrintExcelCell(ws.Cells[16, 3, 16, 5], "TH", "발생일자");
                PrintExcelCell(ws.Cells[16, 6, 16, 13], "TD", QuickResponse.OccurrenceDt == null ? "" : ((DateTime)QuickResponse.OccurrenceDt).ToString("yyyy-MM-dd"));
                PrintExcelCell(ws.Cells[16, 14, 16, 16], "TH", "시정판정");
                PrintExcelCell(ws.Cells[16, 17, 16, 24], "TD", QuickResponse.CorrectDecisionNm);

                PrintExcelCell(ws.Cells[17, 3, 17, 5], "TH", "발생장소");
                PrintExcelCell(ws.Cells[17, 6, 17, 24], "TD", QuickResponse.OccurrencePlace);

                PrintExcelCell(ws.Cells[18, 3, 18, 5], "TH", "QA");
                PrintExcelCell(ws.Cells[18, 6, 18, 24], "TD", QuickResponse.Qa);

                ws.Cells[12, 3, 18, 24].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Medium);

                int iRow = 21;
                #region -- 봉쇄조치
                int StartModuleRow = 21;
                PrintExcelCell(ws.Cells[iRow, 2, iRow, 4], "MODULE", "봉쇄조치");

                iRow = ApprovPrint(ws, BlockadeApprvalData, iRow + 2);

                // 봉쇄조치
                PrintExcelCell(ws.Cells[iRow, 3, iRow, 5], "TITLE", "봉쇄조치");
                iRow++;
                int StartBlockadeItemRow = iRow;

                PrintExcelCell(ws.Cells[iRow, 3, iRow, 4], "TH", "범위");
                PrintExcelCell(ws.Cells[iRow, 5, iRow, 8], "TH", "대상범위");
                PrintExcelCell(ws.Cells[iRow, 9, iRow, 13], "TH", "조치방법");
                PrintExcelCell(ws.Cells[iRow, 14, iRow, 15], "TH", "수량");
                PrintExcelCell(ws.Cells[iRow, 16, iRow, 17], "TH", "조치부서");
                PrintExcelCell(ws.Cells[iRow, 18, iRow, 19], "TH", "담당자");
                PrintExcelCell(ws.Cells[iRow, 20, iRow, 24], "TH", "기한");
                iRow++;

                BlockadeItems.ForEach(v =>
                {
                    PrintExcelCell(ws.Cells[iRow, 3, iRow + 1, 4], "TH", v.Name);
                    PrintExcelCell(ws.Cells[iRow, 5, iRow + 1, 8], "TD", v.TargetScope);
                    PrintExcelCell(ws.Cells[iRow, 9, iRow + 1, 13], "TD", v.Act);
                    PrintExcelCell(ws.Cells[iRow, 14, iRow + 1, 15], "TD", v.TargetCnt.ToString());
                    PrintExcelCell(ws.Cells[iRow, 16, iRow + 1, 17], "TD", v.ActDepartmentNm);
                    PrintExcelCell(ws.Cells[iRow, 18, iRow + 1, 19], "TD", v.ActUserNm);
                    PrintExcelCell(ws.Cells[iRow, 20, iRow + 1, 24], "TD", (v.ActStartDt == null ? "" : ((DateTime)v.ActStartDt).ToString("yyyy-MM-dd")) + "~" + (v.ActEndDt == null ? "" : ((DateTime)v.ActEndDt).ToString("yyyy-MM-dd")));

                    iRow = iRow + 2;
                });

                ws.Cells[StartBlockadeItemRow, 3, iRow - 1, 24].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Medium);

                iRow++;

                // 시정결과
                PrintExcelCell(ws.Cells[iRow, 3, iRow, 5], "TITLE", "시정결과");
                iRow++;
                int StartCorrectiveActionRow = iRow;
                PrintExcelCell(ws.Cells[iRow, 3, iRow + 1, 4], "TH", "범위");
                PrintExcelCell(ws.Cells[iRow, 5, iRow, 8], "TH", "선별");
                PrintExcelCell(ws.Cells[iRow + 1, 5, iRow + 1, 6], "TH", "적합");
                PrintExcelCell(ws.Cells[iRow + 1, 7, iRow + 1, 8], "TH", "부적합");
                PrintExcelCell(ws.Cells[iRow, 9, iRow + 1, 10], "TH", "재작업");
                PrintExcelCell(ws.Cells[iRow, 11, iRow + 1, 12], "TH", "폐기");
                PrintExcelCell(ws.Cells[iRow, 13, iRow + 1, 15], "TH", "특채");
                PrintExcelCell(ws.Cells[iRow, 16, iRow + 1, 17], "TH", "기타");
                PrintExcelCell(ws.Cells[iRow, 18, iRow + 1, 24], "TH", "비고");
                iRow = iRow + 2;


                BlockadeItems.ForEach(v =>
                {
                    PrintExcelCell(ws.Cells[iRow, 3, iRow + 1, 4], "TH", v.Name);
                    PrintExcelCell(ws.Cells[iRow, 5, iRow + 1, 6], "TD", v.SortSuitableCnt.ToString());
                    PrintExcelCell(ws.Cells[iRow, 7, iRow + 1, 8], "TD", v.SortIncongruityCnt.ToString());
                    PrintExcelCell(ws.Cells[iRow, 9, iRow + 1, 10], "TD", v.ReworkCnt.ToString());
                    PrintExcelCell(ws.Cells[iRow, 11, iRow + 1, 12], "TD", v.DisuseCnt.ToString());
                    PrintExcelCell(ws.Cells[iRow, 13, iRow + 1, 15], "TD", v.SpecialCnt.ToString());
                    PrintExcelCell(ws.Cells[iRow, 16, iRow + 1, 17], "TD", v.EtcCnt.ToString());
                    PrintExcelCell(ws.Cells[iRow, 18, iRow + 1, 24], "TD", v.Description);

                    iRow = iRow + 2;
                });

                ws.Cells[StartCorrectiveActionRow, 3, iRow - 1, 24].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Medium);

                ws.Cells[StartModuleRow + 1, 2, iRow, 25].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Medium);
                #endregion
                iRow = iRow + 2;

                #region -- 원인분석
                StartModuleRow = iRow;
                PrintExcelCell(ws.Cells[iRow, 2, iRow, 4], "MODULE", "원인분석");

                iRow = ApprovPrint(ws, OccurrenceCauseApprvalData, iRow + 2);

                // 원인분석
                PrintExcelCell(ws.Cells[iRow, 3, iRow, 5], "TITLE", "원인분석");
                iRow++;
                int StartOccurrenceCauseRow = iRow;

                PrintExcelCell(ws.Cells[iRow, 3, iRow, 5], "TH", "발생원인");
                PrintExcelCell(ws.Cells[iRow, 6, iRow, 24], "TH", "발생원인내역");
                iRow++;

                OccurrenceCauseItems.ForEach(v =>
                {
                    if (v.OccurrenceWhys.Count() == 0) { return; }

                    PrintExcelCell(ws.Cells[iRow, 3, iRow + v.OccurrenceWhys.Count() - 1, 5], "TH", v.OccurrenceCauseLibText);

                    int Cnt = 1;

                    v.OccurrenceWhys.ForEach(w =>
                    {
                        PrintExcelCell(ws.Cells[iRow, 6, iRow, 8], "TD", "WHY " + Cnt.ToString());
                        PrintExcelCell(ws.Cells[iRow, 9, iRow, 24], "TD", w.OccurrenceCauseDetail);
                        iRow++;
                        Cnt++;
                    });
                });

                ws.Cells[StartOccurrenceCauseRow, 3, iRow - 1, 24].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Medium);

                iRow++;

                // 검출대책
                PrintExcelCell(ws.Cells[iRow, 3, iRow, 5], "TITLE", "검출대책");
                iRow++;
                int StartDetectCounterMeasureRow = iRow;

                PrintExcelCell(ws.Cells[iRow, 3, iRow, 5], "TH", "검출장소");
                PrintExcelCell(ws.Cells[iRow, 6, iRow, 8], "TH", "YES/NO");
                PrintExcelCell(ws.Cells[iRow, 9, iRow, 24], "TH", "해당공정 내용 (상세위치, 작업자, 검사원 등)");
                iRow++;

                PrintExcelCell(ws.Cells[iRow, 3, iRow, 5], "TH", "제조공정");
                PrintExcelCell(ws.Cells[iRow, 6, iRow, 8], "TD", DetectCounterMeasure.DetectM == true ? "Yes" : "No");
                PrintExcelCell(ws.Cells[iRow, 9, iRow, 24], "TD", DetectCounterMeasure.DetectMDetail);
                iRow++;

                PrintExcelCell(ws.Cells[iRow, 3, iRow, 5], "TH", "품질");
                PrintExcelCell(ws.Cells[iRow, 6, iRow, 8], "TD", DetectCounterMeasure.DetectS == true ? "Yes" : "No");
                PrintExcelCell(ws.Cells[iRow, 9, iRow, 24], "TD", DetectCounterMeasure.DetectSDetail);
                iRow++;

                PrintExcelCell(ws.Cells[iRow, 3, iRow, 5], "TH", "출하단계");
                PrintExcelCell(ws.Cells[iRow, 6, iRow, 8], "TD", DetectCounterMeasure.DetectQ == true ? "Yes" : "No");
                PrintExcelCell(ws.Cells[iRow, 9, iRow, 24], "TD", DetectCounterMeasure.DetectQDetail);
                iRow++;

                PrintExcelCell(ws.Cells[iRow, 3, iRow, 5], "TH", "기타");
                PrintExcelCell(ws.Cells[iRow, 6, iRow, 8], "TD", DetectCounterMeasure.DetectE == true ? "Yes" : "No");
                PrintExcelCell(ws.Cells[iRow, 9, iRow, 24], "TD", DetectCounterMeasure.DetectEDetail);
                iRow++;

                PrintExcelCell(ws.Cells[iRow, 3, iRow, 5], "TH", "유출원인 1");
                PrintExcelCell(ws.Cells[iRow, 6, iRow, 24], "TD", DetectCounterMeasure.LeakCause1);
                iRow++;

                PrintExcelCell(ws.Cells[iRow, 3, iRow, 5], "TH", "유출원인 2");
                PrintExcelCell(ws.Cells[iRow, 6, iRow, 24], "TD", DetectCounterMeasure.LeakCause2);
                iRow++;

                PrintExcelCell(ws.Cells[iRow, 3, iRow, 5], "TH", "유출원인 3");
                PrintExcelCell(ws.Cells[iRow, 6, iRow, 24], "TD", DetectCounterMeasure.LeakCause3);
                iRow++;

                PrintExcelCell(ws.Cells[iRow, 3, iRow, 5], "TH", "검출일");
                PrintExcelCell(ws.Cells[iRow, 6, iRow, 24], "TD", DetectCounterMeasure.DetectDt == null ? "" : ((DateTime)DetectCounterMeasure.DetectDt).ToString("yyyy-MM-dd"));
                iRow++;

                PrintExcelCell(ws.Cells[iRow, 3, iRow, 5], "TH", "검출대책");
                PrintExcelCell(ws.Cells[iRow, 6, iRow, 24], "TD", DetectCounterMeasure.Measure);
                iRow++;

                ws.Cells[StartDetectCounterMeasureRow, 3, iRow - 1, 24].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Medium);
                ws.Cells[StartModuleRow + 1, 2, iRow, 25].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Medium);
                #endregion
                iRow = iRow + 2;

                #region -- 개선대책
                StartModuleRow = iRow;
                PrintExcelCell(ws.Cells[iRow, 2, iRow, 4], "MODULE", "개선대책");

                iRow = ApprovPrint(ws, ImproveCountermeasureApprvalData, iRow + 2);

                // 개선대책
                PrintExcelCell(ws.Cells[iRow, 3, iRow, 5], "TITLE", "개선대책");
                iRow++;
                int StartImproveCountermeasureRow = iRow;

                PrintExcelCell(ws.Cells[iRow, 3, iRow, 11], "TH", "근본원인");
                PrintExcelCell(ws.Cells[iRow, 12, iRow, 21], "TH", "개선대책");
                PrintExcelCell(ws.Cells[iRow, 22, iRow, 24], "TH", "처리일자");
                iRow++;

                ImproveCounterMeasureItems.ForEach(v =>
                {
                    PrintExcelCell(ws.Cells[iRow, 3, iRow, 11], "TD", v.RootCause);
                    PrintExcelCell(ws.Cells[iRow, 12, iRow, 21], "TD", v.ImproveCountermeasure);
                    PrintExcelCell(ws.Cells[iRow, 22, iRow, 24], "TD", v.ProcessDt == null ? "" : ((DateTime)v.ProcessDt).ToString("yyyy-MM-dd"));
                    iRow++;
                });

                ws.Cells[StartImproveCountermeasureRow, 3, iRow - 1, 24].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Medium);
                ws.Cells[StartModuleRow + 1, 2, iRow, 25].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Medium);
                #endregion

                iRow = iRow + 2;

                #region -- Error Prrof
                if (ErrorProofFl == 1)
                {
                    StartModuleRow = iRow;
                    PrintExcelCell(ws.Cells[iRow, 2, iRow, 4], "MODULE", "Error Prrof");

                    iRow = ApprovPrint(ws, ImproveCountermeasureApprvalData, iRow + 2);

                    PrintExcelCell(ws.Cells[iRow, 3, iRow, 5], "TITLE", "개선대책");
                    iRow++;
                    int StartErrorPrrofRow = iRow;

                    PrintExcelCell(ws.Cells[iRow, 3, iRow, 5], "TH", "예정일자");
                    PrintExcelCell(ws.Cells[iRow, 6, iRow, 13], "TD", ErrorProof.EstDt == null ? "" : ((DateTime)ErrorProof.EstDt).ToString("yyyy-MM-dd"));
                    PrintExcelCell(ws.Cells[iRow, 14, iRow, 16], "TH", "처리일자");
                    PrintExcelCell(ws.Cells[iRow, 17, iRow, 24], "TD", ErrorProof.ActDt == null ? "" : ((DateTime)ErrorProof.ActDt).ToString("yyyy-MM-dd"));
                    iRow++;

                    PrintExcelCell(ws.Cells[iRow, 3, iRow, 5], "TH", "점검내용");
                    PrintExcelCell(ws.Cells[iRow, 6, iRow, 24], "TD", ErrorProof.CheckDetail);
                    iRow++;

                    PrintExcelCell(ws.Cells[iRow, 3, iRow, 5], "TH", "점검담당자");
                    PrintExcelCell(ws.Cells[iRow, 6, iRow, 24], "TD", ErrorProof.CheckUserNm);
                    iRow++;

                    ws.Cells[StartErrorPrrofRow, 3, iRow - 1, 24].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Medium);
                    ws.Cells[StartModuleRow + 1, 2, iRow, 25].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Medium);
                }
                #endregion

                iRow = iRow + 2;

                #region -- LPA
                if (LpaUnfitFl == 1)
                {
                    StartModuleRow = iRow;
                    PrintExcelCell(ws.Cells[iRow, 2, iRow, 6], "MODULE", "LPA 부적합현황");

                    iRow = ApprovPrint(ws, LpaMeasureApprvalData, iRow + 2);

                    PrintExcelCell(ws.Cells[iRow, 3, iRow, 7], "TITLE", "LPA 부적합현황");
                    iRow++;
                    int StartLpaUnfitRow = iRow;

                    PrintExcelCell(ws.Cells[iRow, 3, iRow, 5], "TH", "Layer");
                    PrintExcelCell(ws.Cells[iRow, 6, iRow, 13], "TD", LpaUnfit.LayerLibNm);
                    PrintExcelCell(ws.Cells[iRow, 14, iRow, 16], "TH", "감사주기");
                    PrintExcelCell(ws.Cells[iRow, 17, iRow, 24], "TD", LpaUnfit.AuditLibNm);
                    iRow++;

                    PrintExcelCell(ws.Cells[iRow, 3, iRow, 5], "TH", "그룹군");
                    PrintExcelCell(ws.Cells[iRow, 6, iRow, 13], "TD", LpaUnfit.LpaGrpLibNm);
                    PrintExcelCell(ws.Cells[iRow, 14, iRow, 16], "TH", "사용구분");
                    PrintExcelCell(ws.Cells[iRow, 17, iRow, 24], "TD", LpaUnfit.LpaUseLibNm);
                    iRow++;

                    PrintExcelCell(ws.Cells[iRow, 3, iRow, 5], "TH", "심사자");
                    PrintExcelCell(ws.Cells[iRow, 6, iRow, 13], "TD", LpaUnfit.LpaCheckUserNm);
                    PrintExcelCell(ws.Cells[iRow, 14, iRow, 16], "TH", "확인공정");
                    PrintExcelCell(ws.Cells[iRow, 17, iRow, 24], "TD", LpaUnfit.LpaCheckProcessLibNm);
                    iRow++;

                    PrintExcelCell(ws.Cells[iRow, 3, iRow, 5], "TH", "심사일자");
                    PrintExcelCell(ws.Cells[iRow, 6, iRow, 13], "TD", LpaUnfit.LpaCheckDt == null ? "" : ((DateTime)LpaUnfit.LpaCheckDt).ToString("yyyy-MM-dd"));
                    PrintExcelCell(ws.Cells[iRow, 14, iRow, 16], "TH", "설비명");
                    PrintExcelCell(ws.Cells[iRow, 17, iRow, 24], "TD", LpaUnfit.EquipmentNm);
                    iRow++;

                    PrintExcelCell(ws.Cells[iRow, 3, iRow, 5], "TH", "품번/품명");
                    PrintExcelCell(ws.Cells[iRow, 6, iRow, 24], "TD", LpaUnfit.PartCd + "/" + LpaUnfit.PartNm);
                    iRow++;

                    int LpaUnfitCheckItemCount = LpaUnfitCheck.Count();
                    if (LpaUnfitCheckItemCount == 0)
                    {
                        PrintExcelCell(ws.Cells[iRow, 3, iRow, 5], "TH", "지적사항");
                        PrintExcelCell(ws.Cells[iRow, 6, iRow, 24], "TD", "");
                        iRow++;
                    }
                    else
                    {
                        PrintExcelCell(ws.Cells[iRow, 3, iRow + (LpaUnfitCheckItemCount - 1), 5], "TH", "지적사항");

                        int iNum = 1;
                        LpaUnfitCheck.ForEach(v =>
                        {
                            PrintExcelCell(ws.Cells[iRow, 6, iRow, 6], "TD", iNum.ToString());
                            PrintExcelCell(ws.Cells[iRow, 7, iRow, 24], "TD", v.CheckPoin);
                            iNum++;
                            iRow++;
                        });
                    }

                    PrintExcelCell(ws.Cells[iRow, 3, iRow, 5], "TH", "LPA 담당자");
                    PrintExcelCell(ws.Cells[iRow, 6, iRow, 13], "TD", LpaUnfit.LpaUserNm);
                    PrintExcelCell(ws.Cells[iRow, 14, iRow, 16], "TH", "대책서담당자");
                    PrintExcelCell(ws.Cells[iRow, 17, iRow, 24], "TD", LpaUnfit.MeasureUserNm);
                    iRow++;

                    PrintExcelCell(ws.Cells[iRow, 3, iRow, 5], "TH", "완료예정일");
                    PrintExcelCell(ws.Cells[iRow, 6, iRow, 24], "TD", LpaUnfit.FinishRequestDt == null ? "" : ((DateTime)LpaUnfit.FinishRequestDt).ToString("yyyy-MM-dd"));
                    iRow++;

                    ws.Cells[StartLpaUnfitRow, 3, iRow - 1, 24].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Medium);

                    iRow = iRow + 2;

                    PrintExcelCell(ws.Cells[iRow, 3, iRow, 7], "TITLE", "LPA 대책서작성");
                    iRow++;
                    int StartLpaMeasureRow = iRow;

                    PrintExcelCell(ws.Cells[iRow, 3, iRow + LpaUnfitCheckItemCount, 5], "TH", "지적사항");
                    PrintExcelCell(ws.Cells[iRow, 6, iRow, 6], "TH", "NO");
                    PrintExcelCell(ws.Cells[iRow, 7, iRow, 11], "TH", "지적사항");
                    PrintExcelCell(ws.Cells[iRow, 12, iRow, 18], "TH", "원인분석");
                    PrintExcelCell(ws.Cells[iRow, 19, iRow, 24], "TH", "개선대책");

                    iRow++;
                    int iLpaUnfitCheck = 0;
                    LpaUnfitCheck.ForEach(v =>
                    {
                        PrintExcelCell(ws.Cells[iRow, 6, iRow, 6], "TD", iLpaUnfitCheck.ToString());
                        PrintExcelCell(ws.Cells[iRow, 7, iRow, 11], "TD", v.CheckPoin);
                        PrintExcelCell(ws.Cells[iRow, 12, iRow, 18], "TD", v.CauseAnalysis);
                        PrintExcelCell(ws.Cells[iRow, 19, iRow, 24], "TD", v.ImproveCountermeasure);
                        iLpaUnfitCheck++;
                        iRow++;
                    });

                    PrintExcelCell(ws.Cells[iRow, 3, iRow, 5], "TH", "검토원인상세");
                    PrintExcelCell(ws.Cells[iRow, 6, iRow, 24], "TD", LpaMeasure.CauseAnalysis);
                    iRow++;

                    PrintExcelCell(ws.Cells[iRow, 3, iRow, 5], "TH", "재발방지대책");
                    PrintExcelCell(ws.Cells[iRow, 6, iRow, 24], "TD", LpaMeasure.ImproveCountermeasure);
                    iRow++;

                    ws.Cells[StartLpaMeasureRow, 3, iRow - 1, 24].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Medium);
                    ws.Cells[StartModuleRow + 1, 2, iRow, 25].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Medium);
                }
                #endregion

                iRow = iRow + 2;

                #region -- 유효성검증
                if (QmsCheckFl == 1)
                {
                    StartModuleRow = iRow;
                    PrintExcelCell(ws.Cells[iRow, 2, iRow, 5], "MODULE", "유효성 검증");

                    iRow = ApprovPrint(ws, QmsCheckApprvalData, iRow + 2);

                    PrintExcelCell(ws.Cells[iRow, 3, iRow, 5], "TITLE", "유효성 검증");
                    iRow++;
                    int StartCheckRow = iRow;

                    PrintExcelCell(ws.Cells[iRow, 3, iRow, 5], "TH", "구분");
                    PrintExcelCell(ws.Cells[iRow, 6, iRow, 11], "TH", "1차 유효성검증");
                    PrintExcelCell(ws.Cells[iRow, 12, iRow, 18], "TH", "2차 유효성검증");
                    PrintExcelCell(ws.Cells[iRow, 19, iRow, 24], "TH", "3차 유효성검증");

                    PrintExcelCell(ws.Cells[iRow + 1, 3, iRow + 1, 5], "TH", "검증완료일");
                    PrintExcelCell(ws.Cells[iRow + 2, 3, iRow + 2, 5], "TH", "담당자");
                    PrintExcelCell(ws.Cells[iRow + 3, 3, iRow + 3, 5], "TH", "검증일");
                    PrintExcelCell(ws.Cells[iRow + 4, 3, iRow + 4, 5], "TH", "판정");
                    PrintExcelCell(ws.Cells[iRow + 5, 3, iRow + 5, 5], "TH", "검증내용요약");
                    PrintExcelCell(ws.Cells[iRow + 6, 3, iRow + 6, 5], "TH", "종료여부");
                    PrintExcelCell(ws.Cells[iRow + 7, 3, iRow + 7, 5], "TH", "종료사유");

                    int iItemCnt = 0;
                    QmsCheckItems.ForEach(v =>
                    {
                        int StartCheckCol = 0;
                        int EndCheckCol = 0;
                        if (iItemCnt == 0)
                        {
                            StartCheckCol = 6;
                            EndCheckCol = 11;
                        }
                        else if (iItemCnt == 1)
                        {
                            StartCheckCol = 12;
                            EndCheckCol = 18;
                        }
                        else if (iItemCnt == 2)
                        {
                            StartCheckCol = 19;
                            EndCheckCol = 24;
                        }
                        else
                        {
                            return;
                        }

                        PrintExcelCell(ws.Cells[iRow + 1, StartCheckCol, iRow + 1, EndCheckCol], "TD", v.CheckEt == null ? "" : ((DateTime)v.CheckEt).ToString("yyyy-MM-dd"));
                        PrintExcelCell(ws.Cells[iRow + 2, StartCheckCol, iRow + 2, EndCheckCol], "TD", v.CheckUserNm);
                        PrintExcelCell(ws.Cells[iRow + 3, StartCheckCol, iRow + 3, EndCheckCol], "TD", v.CheckDt == null ? "" : ((DateTime)v.CheckDt).ToString("yyyy-MM-dd"));
                        PrintExcelCell(ws.Cells[iRow + 4, StartCheckCol, iRow + 4, EndCheckCol], "TD", v.CheckFl == 1 ? "Y" : "N");
                        PrintExcelCell(ws.Cells[iRow + 5, StartCheckCol, iRow + 5, EndCheckCol], "TD", v.CheckDetail);
                        PrintExcelCell(ws.Cells[iRow + 6, StartCheckCol, iRow + 6, EndCheckCol], "TD", v.FinishFl == 1 ? "Y" : "N");
                        PrintExcelCell(ws.Cells[iRow + 7, StartCheckCol, iRow + 7, EndCheckCol], "TD", v.FinishDetail);
                        iItemCnt++;
                    });

                    if (iItemCnt < 3)
                    {
                        for (int x = iItemCnt; x < 3; x++)
                        {
                            int StartCheckCol = 0;
                            int EndCheckCol = 0;
                            if (x == 0)
                            {
                                StartCheckCol = 6;
                                EndCheckCol = 11;
                            }
                            else if (x == 1)
                            {
                                StartCheckCol = 12;
                                EndCheckCol = 18;
                            }
                            else if (x == 2)
                            {
                                StartCheckCol = 19;
                                EndCheckCol = 24;
                            }

                            PrintExcelCell(ws.Cells[iRow + 1, StartCheckCol, iRow + 7, EndCheckCol], "TD", "");
                        }
                    }

                    iRow = iRow + 8;

                    ws.Cells[StartCheckRow, 3, iRow - 1, 24].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Medium);
                    ws.Cells[StartModuleRow + 1, 2, iRow, 25].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Medium);

                }
                #endregion

                iRow = iRow + 2;

                #region -- 표준화 F/U
                if (StandardDocFl == 1)
                {
                    StartModuleRow = iRow;
                    PrintExcelCell(ws.Cells[iRow, 2, iRow, 4], "MODULE", "표준화 F/U");

                    iRow = ApprovPrint(ws, StandardDocApprvalData, iRow + 2);

                    PrintExcelCell(ws.Cells[iRow, 3, iRow, 5], "TITLE", "표준화 F/U");
                    iRow++;
                    int StartStandardRow = iRow;

                    PrintExcelCell(ws.Cells[iRow, 3, iRow, 5], "TH", "문서타입");
                    PrintExcelCell(ws.Cells[iRow, 6, iRow, 12], "TH", "문서명");
                    PrintExcelCell(ws.Cells[iRow, 13, iRow, 21], "TH", "반영내용");
                    PrintExcelCell(ws.Cells[iRow, 22, iRow, 24], "TH", "완료일");
                    iRow++;

                    ws.Cells[StartStandardRow, 3, iRow - 1, 24].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Medium);
                    ws.Cells[StartModuleRow + 1, 2, iRow, 25].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Medium);
                }
                #endregion

                iRow = iRow + 2;

                #region -- 사용자교육
                if (WorkerEduFl == 1)
                {
                    StartModuleRow = iRow;
                    PrintExcelCell(ws.Cells[iRow, 2, iRow, 4], "MODULE", "사용자 교육");

                    iRow = ApprovPrint(ws, WorkerEduApprvalData, iRow + 2);

                    PrintExcelCell(ws.Cells[iRow, 3, iRow, 5], "TITLE", "사용자 교육");
                    iRow++;
                    int StartEduRow = iRow;

                    PrintExcelCell(ws.Cells[iRow, 3, iRow, 5], "TH", "교육일자");
                    PrintExcelCell(ws.Cells[iRow, 6, iRow, 13], "TD", WorkerEdu.EduDt == null ? "" : ((DateTime)WorkerEdu.EduDt).ToString("yyyy-MM-dd"));
                    PrintExcelCell(ws.Cells[iRow, 14, iRow, 16], "TH", "담당자");
                    PrintExcelCell(ws.Cells[iRow, 17, iRow, 24], "TD", WorkerEdu.EduUserNm);
                    iRow++;

                    PrintExcelCell(ws.Cells[iRow, 3, iRow, 5], "TH", "교육내용");
                    PrintExcelCell(ws.Cells[iRow, 6, iRow, 24], "TD", WorkerEdu.EduDetail);
                    iRow++;

                    PrintExcelCell(ws.Cells[iRow, 3, iRow, 5], "TH", "교육계획");
                    PrintExcelCell(ws.Cells[iRow, 6, iRow, 24], "TD", WorkerEdu.EduPlan);
                    iRow++;

                    ws.Cells[StartEduRow, 3, iRow - 1, 24].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Medium);
                    ws.Cells[StartModuleRow + 1, 2, iRow, 25].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Medium);
                }
                #endregion

                newTempFilePath = SaveTempExcelFile(excelPackage, strNewFileName);
                excelPackage.Dispose();
                ws.Dispose();
            }


            DownExcelStream = FileStream(Path.Combine(newTempFilePath));

            if (Request.Browser.Browser == "IE" || Request.Browser.Browser == "InternetExplorer")
            {
                return File(DownExcelStream, MediaTypeNames.Application.Octet, HttpUtility.UrlEncode(string.Format("신속대응[{0}]_{1}.xlsx", QuickResponse.Title, DateTime.Now.ToString("yyyy-MM-dd")), System.Text.Encoding.UTF8));
            }
            else
            {
                return File(DownExcelStream, MediaTypeNames.Application.Octet, HttpUtility.UrlEncode(string.Format("신속대응[{0}]_{1}.xlsx", QuickResponse.Title, DateTime.Now.ToString("yyyy-MM-dd"))));
            }
        }

        private int ApprovPrint(ExcelWorksheet ws, Approval approv, int iRow)
        {
            if (approv == null) { return iRow; }
            int AppvCol = 23;
            ws.Row(iRow - 1).Height = 2.3;
            List<ApprovalStep> approvalSteps = approv.InboxStep.OrderByDescending(d => d.OID).ToList<ApprovalStep>();

            approvalSteps.ForEach(v =>
            {
                if (AppvCol == 2) { return; }
                if (v.ApprovalType == Common.Constant.CommonConstant.TYPE_APPROVAL_DIST) { return; }

                ApprovalTask TaskData = v.InboxTask[0];

                PrintExcelCell(ws.Cells[iRow, AppvCol, iRow, AppvCol + 1], "APPV_TEXT", TaskData.PersonObj.Name);
                if (TaskData.BPolicy.Name == Common.Constant.CommonConstant.POLICY_APPROVAL_TASK_COMPLETED)
                {
                    PrintExcelCell(ws.Cells[iRow + 1, AppvCol, iRow + 1, AppvCol + 1], "APPV_TEXT", "승인");
                    PrintExcelCell(ws.Cells[iRow + 2, AppvCol, iRow + 2, AppvCol + 1], "APPV_DATE", ((DateTime)TaskData.ApprovalDt).ToString("yyyy-MM-dd"));
                }
                else if (TaskData.BPolicy.Name == Common.Constant.CommonConstant.POLICY_APPROVAL_REJECTED)
                {
                    PrintExcelCell(ws.Cells[iRow + 1, AppvCol, iRow + 1, AppvCol + 1], "APPV_TEXT", "반려");
                    PrintExcelCell(ws.Cells[iRow + 2, AppvCol, iRow + 2, AppvCol + 1], "APPV_DATE", ((DateTime)TaskData.ApprovalDt).ToString("yyyy-MM-dd"));
                }
                else
                {
                    PrintExcelCell(ws.Cells[iRow + 1, AppvCol, iRow + 1, AppvCol + 1], "TD", "-");
                    PrintExcelCell(ws.Cells[iRow + 2, AppvCol, iRow + 2, AppvCol + 1], "APPV_DATE", "-");
                }

                AppvCol = AppvCol - 2;

            });

            PrintExcelCell(ws.Cells[iRow, AppvCol, iRow + 2, AppvCol + 1], "APPV_TEXT", "결재");

            return iRow + 3;
        }


        private void PrintExcelCell(ExcelRange range, string Type, string Text)
        {
            range.Merge = true;
            range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
            range.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;

            if (Type == "TITLE")
            {
                range.Style.Font.Size = 9;
                range.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Medium);
                range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
            }
            else if (Type == "VALUE")
            {
                range.Style.Font.Size = 9;
                range.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Medium);
                //range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                //range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
            }
            else if (Type == "TH")
            {
                range.Style.Font.Size = 9;
                range.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.WhiteSmoke);
            }
            else if (Type == "TD")
            {
                range.Style.Font.Size = 8;
                range.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
            }
            else if (Type == "MODULE")
            {
                range.Style.Font.Size = 9;
                range.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Medium);
                range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.PapayaWhip);
            }
            else if (Type == "APPV_TEXT")
            {
                range.Style.Font.Size = 7;
                //range.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
            }
            else if (Type == "APPV_DATE")
            {
                range.Style.Font.Size = 7;
                //range.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
            }

            range.Value = Text;
        }

        public static Stream FileStream(string fileFullPath)
        {
            try
            {
                var fs = new FileStream(fileFullPath, FileMode.Open);
                return fs;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string SaveTempExcelFile(ExcelPackage excel, string fileName)
        {
            try
            {
                string StoragePath = System.Web.HttpContext.Current.Server.MapPath(System.Configuration.ConfigurationManager.AppSettings["FileStorage"]);
                string TempPath = System.Configuration.ConfigurationManager.AppSettings["TempPath"];

                DirectoryInfo di = new DirectoryInfo(StoragePath + "/" + TempPath + "/");

                if (!di.Exists) { di.Create(); }

                FileInfo uploadFile = new FileInfo(Path.Combine(StoragePath, TempPath, "", fileName));

                string ext = uploadFile.Extension;
                string purefileName = uploadFile.Name.Substring(0, uploadFile.Name.Length - ext.Length);

                FileInfo fi = new FileInfo(di.FullName + "/" + fileName);

                if (fi.Exists)
                {
                    /*중복되지 않은 파일명 생성*/
                    int cnt = 1;
                    string newFileName = "";
                    string pureName = purefileName;

                    while (fi.Exists)
                    {
                        newFileName = pureName + " (" + (cnt++) + ")" + ext;
                        fi = new FileInfo(StoragePath + "/" + TempPath + "/" + fileName);
                    }
                }

                excel.SaveAs(fi);

                return StoragePath + "/" + TempPath + "/" + fileName;

            }
            catch (Exception ex)
            {
                throw ex;
            }
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

        public JsonResult SaveQuickResponseModule(QuickResponse _param)
        {
            try
            {
                DaoFactory.BeginTransaction();

                QuickResponseModule blockade = QuickResponseModuleRepository.SelQuickResponseModule(new QuickResponseModule() { QuickOID = _param.OID, ModuleType = QmsConstant.TYPE_BLOCKADE });

                if (!(blockade.BPolicyNm == QmsConstant.POLICY_QMS_MODULE_PREPARE || blockade.BPolicyNm == QmsConstant.POLICY_QMS_MODULE_STARTED))
                {
                    throw new Exception("봉쇄조치가 진행중인관계로 일정을 수정 할 수 없습니다.");
                }

                _param.Modules.ForEach(v =>
                {
                    QuickResponseModuleRepository.UdtQuickResponseModule(v);

                    DObject dObject = DObjectRepository.SelDObject(Session, v);


                    if (dObject.Type == QmsConstant.TYPE_LPA_UNFIT)
                    {
                        QuickResponseModule LpaMeasureModule = QuickResponseModuleRepository.SelQuickResponseModule(new QuickResponseModule() { QuickOID = v.QuickOID, ModuleType = QmsConstant.TYPE_LPA_MEASURE });

                        LpaMeasureModule.ModuleFl = v.ModuleFl;

                        QuickResponseModuleRepository.UdtQuickResponseModule(LpaMeasureModule);
                    }

                    if (dObject.Type == QmsConstant.TYPE_BLOCKADE && dObject.BPolicyOID == 57 && v.EstEndDt != null && v.ChargeUserOID != null)
                    {
                        dObject.BPolicyOID =  58; // 작성중으로 변경
                        DObjectRepository.UdtDObject(Session, dObject);

                        DRelationship dRelModule = new DRelationship();
                        dRelModule.Type = QmsConstant.RELATIONSHIP_QUICK_MODULE;
                        dRelModule.ToOID = dObject.OID;
                        DRelationship dRelQuickResponse = DaoFactory.GetData<DRelationship>("Comm.SelDRelationship", dRelModule);

                        QuickResponse quick = QuickResponseRepository.SelQuickResponse(new Qms.Models.QuickResponse() { OID = dRelQuickResponse.FromOID });

                        quick.BPolicyOID = 54; //검토중으로 변경
                        DObjectRepository.UdtDObject(Session, quick);
                    }
                });

                if (_param.Files != null)
                {
                    HttpFileRepository.InsertData(Session, _param);
                }

                if (_param.delFiles != null)
                {
                    _param.delFiles.ForEach(v =>
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

        public PartialViewResult InfoMiniBlockade(int OID)
        {
            QuickResponseModule Module = QuickResponseModuleRepository.SelQuickResponseModule(new QuickResponseModule { OID = OID });
            ViewBag.Blockade = Module;
            ViewBag.BlockadeItems = BlockadeItemRepository.SelBlockadeItems(new BlockadeItem() { ModuleOID = OID });
            ViewBag.QuickDetail = QuickResponseRepository.SelQuickResponse(new QuickResponse() { OID = Module.QuickOID });
            ViewBag.Status = BPolicyRepository.SelBPolicy(new BPolicy { Type = QmsConstant.TYPE_BLOCKADE });
            return PartialView("InfoBlockade");
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

        public ActionResult InfoMiniOccurrenceCause(int OID)
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

            return PartialView("InfoOccurrenceCause");
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

                    if (v.OccurrenceWhys != null)
                    {
                        v.OccurrenceWhys.ForEach(w =>
                        {
                            if (w.OID == null)
                            {
                                if (w.IsRemove == "Y")
                                {
                                    return;
                                }

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
                                    DObjectRepository.DelDObject(Session, w, null);
                                }
                                else
                                {
                                    OccurrenceWhyRepository.UdtOccurrenceWhy(w);
                                    DObjectRepository.UdtDObject(Session, w);
                                }
                            }
                        });
                    }
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

        public ActionResult InfoMiniImproveCounterMeasure(int OID)
        {
            QuickResponseModule Module = QuickResponseModuleRepository.SelQuickResponseModule(new QuickResponseModule { OID = OID });
            ViewBag.ImproveCounterMeasure = Module;
            ViewBag.QuickDetail = QuickResponseRepository.SelQuickResponse(new QuickResponse() { OID = Module.QuickOID });
            ViewBag.ImproveCounterMeasureItems = ImproveCounterMeasureItemRepository.SelImproveCounterMeasureItems(new ImproveCounterMeasureItem() { ModuleOID = OID });
            ViewBag.Status = BPolicyRepository.SelBPolicy(new BPolicy { Type = QmsConstant.TYPE_IMPROVE_COUNTERMEASURE });
            return PartialView("InfoImproveCounterMeasure");
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
                            DObjectRepository.DelDObject(Session, v, null);
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
                errorproof = new ErrorProof()
                {
                    OID = OID,
                    ModuleOID = OID,
                    BPolicyOID = Module.BPolicyOID,
                    BPolicyNm = Module.BPolicyNm
                };
            }
            ViewBag.ErrorProof = errorproof;
            ViewBag.QuickDetail = QuickResponseRepository.SelQuickResponse(new QuickResponse() { OID = Module.QuickOID });
            ViewBag.Status = BPolicyRepository.SelBPolicy(new BPolicy { Type = QmsConstant.TYPE_ERROR_PRROF });
            return View();
        }

        public ActionResult InfoMiniErrorProof(int OID)
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
            return PartialView("InfoErrorProof");
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

            List<DRelationship> relLpa = DRelationshipRepository.SelRelationship(Session, new DRelationship() { Type = QmsConstant.RELATIONSHIP_LPA, FromOID = OID });

            ViewBag.LpaUnfit = lpaUnfit;
            ViewBag.LpaUnfitCheck = lpaUnfitCheck;
            ViewBag.QuickDetail = QuickResponseRepository.SelQuickResponse(new QuickResponse() { OID = Module.QuickOID });
            ViewBag.Status = BPolicyRepository.SelBPolicy(new BPolicy { Type = QmsConstant.TYPE_LPA_UNFIT });

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
                        if (v.IsRemove == "Y")
                        {
                            return;
                        }
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
                            DObjectRepository.DelDObject(Session, v, null);
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

                if (param.ModuleOID == null) { throw new Exception("잘못된 호출입니다."); }

                DObjectRepository.UdtDObject(Session, new DObject() { OID = param.ModuleOID, BPolicyOID = 79 });

                QuickResponseModule quickResponse = QuickResponseModuleRepository.SelQuickResponseModule(new QuickResponseModule() { OID = param.ModuleOID });

                QuickResponseModule LpaMeasureModule = QuickResponseModuleRepository.SelQuickResponseModule(new QuickResponseModule() { QuickOID = quickResponse.QuickOID, ModuleFl = 1, ModuleType = QmsConstant.TYPE_LPA_MEASURE });

                List<BPolicy> nextModluePolicies = BPolicyRepository.SelBPolicy(new BPolicy() { Type = QmsConstant.TYPE_LPA_MEASURE, Name = "Started" });

                DObjectRepository.UdtDObject(Session, new DObject() { OID = LpaMeasureModule.OID, BPolicyOID = nextModluePolicies[0].OID });

                DaoFactory.Commit();

                return Json(param.ModuleOID);
            }
            catch (Exception ex)
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

            List<DRelationship> relLpa = DRelationshipRepository.SelRelationship(Session, new DRelationship() { Type = QmsConstant.RELATIONSHIP_LPA, ToOID = OID });

            int? LpaUnfitOID = null;
            relLpa.ForEach(v => { LpaUnfitOID = v.FromOID; });
            LpaUnfit lpaUnfit = LpaUnfitRepository.SelLpaUnfit(new LpaUnfit() { ModuleOID = LpaUnfitOID });
            List<LpaUnfitCheck> lpaUnfitCheck = LpaUnfitCheckRepository.SelLpaUnfitChecks(new LpaUnfitCheck() { ModuleOID = LpaUnfitOID });

            ViewBag.lpaMeasure = lpaMeasure;
            ViewBag.LpaUnfitCheck = lpaUnfitCheck;
            ViewBag.QuickDetail = QuickResponseRepository.SelQuickResponse(new QuickResponse() { OID = Module.QuickOID });
            ViewBag.Status = BPolicyRepository.SelBPolicy(new BPolicy { Type = QmsConstant.TYPE_LPA_MEASURE });
            ViewBag.LpaUnfitOID = LpaUnfitOID;
            ViewBag.MeasureUserOID = lpaUnfit.MeasureUserOID;

            return View();
        }

        public ActionResult InfoMiniLpaMeasure(int OID)
        {
            QuickResponseModule Module = QuickResponseModuleRepository.SelQuickResponseModule(new QuickResponseModule { OID = OID });
            LpaMeasure lpaMeasure = LpaMeasureRepository.SelLpaMeasure(new LpaMeasure() { ModuleOID = OID });
            if (lpaMeasure == null)
            {
                lpaMeasure = new LpaMeasure();
                lpaMeasure.OID = OID;
                lpaMeasure.ModuleOID = OID;
            }

            List<DRelationship> relLpa = DRelationshipRepository.SelRelationship(Session, new DRelationship() { Type = QmsConstant.RELATIONSHIP_LPA, ToOID = OID });

            int? LpaUnfitOID = null;
            relLpa.ForEach(v => { LpaUnfitOID = v.FromOID; });
            LpaUnfit lpaUnfit = LpaUnfitRepository.SelLpaUnfit(new LpaUnfit() { ModuleOID = LpaUnfitOID });
            List<LpaUnfitCheck> lpaUnfitCheck = LpaUnfitCheckRepository.SelLpaUnfitChecks(new LpaUnfitCheck() { ModuleOID = LpaUnfitOID });

            ViewBag.lpaMeasure = lpaMeasure;
            ViewBag.LpaUnfitCheck = lpaUnfitCheck;
            ViewBag.QuickDetail = QuickResponseRepository.SelQuickResponse(new QuickResponse() { OID = Module.QuickOID });
            ViewBag.Status = BPolicyRepository.SelBPolicy(new BPolicy { Type = QmsConstant.TYPE_LPA_MEASURE });
            ViewBag.LpaUnfitOID = LpaUnfitOID;
            ViewBag.MeasureUserOID = lpaUnfit.MeasureUserOID;

            return PartialView("InfoLpaMeasure");
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
            List<Approval> approvals = ApprovalRepository.SelApprovals(Session, new Approval() { TargetOID = Module.OID });
            if (approvals != null && approvals.Count > 0)
            {
                ViewBag.ApprovalCnt = approvals.Count();
            }

            return View();
        }

        public ActionResult InfoMiniQuickValidation(int? OID)
        {
            QuickResponseModule Module = QuickResponseModuleRepository.SelQuickResponseModule(new QuickResponseModule { OID = OID });
            ViewBag.QmsCheck = Module;

            ViewBag.QmsCheckItems = QmsCheckRepository.SelQmsChecks(new QmsCheck() { ModuleOID = OID });
            ViewBag.QuickDetail = QuickResponseRepository.SelQuickResponse(new QuickResponse() { OID = Module.QuickOID });
            ViewBag.Status = BPolicyRepository.SelBPolicy(new BPolicy { Type = QmsConstant.TYPE_QUICK_RESPONSE_CHECK });
            List<Approval> approvals = ApprovalRepository.SelApprovals(Session, new Approval() { TargetOID = Module.OID });
            if (approvals != null && approvals.Count > 0)
            {
                ViewBag.ApprovalCnt = approvals.Count();
            }

            return PartialView("InfoQuickValidation");
        }
        #endregion

        #region -- 등록, 수정, 삭제, 조회

        /// <summary>
        /// 2020.12.13
        /// 유효성 검증 등록, 수정
        /// </summary>
        /// <param name="_param"></param>
        /// <returns></returns>
        public JsonResult SaveQuickValidation(QmsCheck _param)
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
                    else
                    {
                        QmsCheckRepository.UdtQmsCheck(qmsCheck);
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
                standardDoc = new StandardDoc()
                {
                    OID = OID,
                    ModuleOID = OID,
                    BPolicyOID = Module.BPolicyOID,
                    BPolicyNm = Module.BPolicyNm
                };
            }

            /*ViewBag.StandardDocDetail = StandardDocRepository.SelStandardDocs(new StandardDoc() { ModuleOID = OID });*/
            // TEST
            // Module은 모듈로 
            // ITEM 은 따로 뺴야함
            // 김성현.
            ViewBag.StandardDoc = Module;
            ViewBag.StandardDocItem = StandardDocRepository.SelStandardDocs(new StandardDoc() { ModuleOID = OID });
            ViewBag.QuickDetail = QuickResponseRepository.SelQuickResponse(new QuickResponse() { OID = Module.QuickOID });
            ViewBag.Status = BPolicyRepository.SelBPolicy(new BPolicy { Type = QmsConstant.TYPE_STANDARD });

            return View();
        }
        public ActionResult InfoMiniStandardFollowUp(int? OID)
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
            // Module은 모듈로 
            // ITEM 은 따로 뺴야함
            // 김성현.
            ViewBag.StandardDoc = Module;
            ViewBag.StandardDocItem = StandardDocRepository.SelStandardDocs(new StandardDoc() { ModuleOID = OID });
            ViewBag.QuickDetail = QuickResponseRepository.SelQuickResponse(new QuickResponse() { OID = Module.QuickOID });
            ViewBag.Status = BPolicyRepository.SelBPolicy(new BPolicy { Type = QmsConstant.TYPE_STANDARD });

            return PartialView("InfoMiniStandardFollowUp");
        }
        #endregion

        #region -- 등록, 수정, 삭제, 조회
        public JsonResult SaveStandardFollowUp(StandardDoc _param)
        {
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

                        StandardDocRepository.InsStandardDoc(standardDoc);
                    }
                    else
                    {
                        StandardDocRepository.UdtStandardDoc(standardDoc);
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
                workerEdu = new WorkerEdu()
                {
                    OID = Module.OID,
                    ModuleOID = Module.OID,
                    BPolicyOID = Module.BPolicyOID,
                    BPolicyNm = Module.BPolicyNm
                };
            }

            ViewBag.WorkerEdu = workerEdu;
            ViewBag.QuickDetail = QuickResponseRepository.SelQuickResponse(new QuickResponse() { OID = Module.QuickOID });
            ViewBag.Status = BPolicyRepository.SelBPolicy(new BPolicy { Type = QmsConstant.TYPE_WORKER_EDU });

            return View();
        }

        public ActionResult InfoMiniWorkerEducation(int? OID)
        {
            QuickResponseModule Module = QuickResponseModuleRepository.SelQuickResponseModule(new QuickResponseModule { OID = OID });
            WorkerEdu workerEdu = WorkerEduRepository.SelWorkerEdu(new WorkerEdu() { ModuleOID = OID });
            if (workerEdu == null)
            {
                workerEdu = new WorkerEdu();
                workerEdu.OID = Module.OID;
                workerEdu.ModuleOID = Module.OID;
            }

            ViewBag.WorkerEdu = workerEdu;
            ViewBag.QuickDetail = QuickResponseRepository.SelQuickResponse(new QuickResponse() { OID = Module.QuickOID });
            ViewBag.Status = BPolicyRepository.SelBPolicy(new BPolicy { Type = QmsConstant.TYPE_WORKER_EDU });

            return PartialView("InfoWorkerEducation");
        }
        #endregion

        #region -- 등록, 수정, 삭제, 조회

        /// <summary>
        /// 작업자 교육 등록, 수정
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public JsonResult SaveWorkerEducation(WorkerEdu param)
        {
            int returnValue = 0;
            try
            {
                DaoFactory.BeginTransaction();
                param.Type = QmsConstant.TYPE_WORKER_EDU;

                WorkerEdu workerEdu = new WorkerEdu()
                {
                    ModuleOID = param.ModuleOID,
                    EduDetail = param.EduDetail,
                    EduPlan = param.EduPlan,
                    EduDt = param.EduDt,
                    EduUserOID = param.EduUserOID
                };

                if (WorkerEduRepository.SelWorkerEdu(new WorkerEdu() { ModuleOID = workerEdu.ModuleOID }) == null)
                {
                    returnValue = WorkerEduRepository.InsWorkerEdu(workerEdu);
                }
                else
                {
                    returnValue = WorkerEduRepository.UdtWorkerEdu(workerEdu);
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