using Common.Constant;
using Common.Factory;
using Common.Models;
using Common.Models.File;
using Pms.Models;
using Qms.Models;
using SemsPLM.Filter;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
            else if(items.Count() > 0)
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
            List<Library> occurrenceList = LibraryRepository.SelLibrary(new Library { FromOID = occurrenceKey.OID });  // 발생유형
            ViewBag.occurrenceList = occurrenceList;

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

            Library occurrenceKey = LibraryRepository.SelLibraryObject(new Library { Name = "OCCURRENCE_TYPE"});
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

            Library imputeKey = LibraryRepository.SelLibraryObject(new Library { Name = "IMPUTE" });
            List<Library> imputeList = LibraryRepository.SelLibrary(new Library { FromOID = imputeKey.OID });  // 귀책구분
            ViewBag.imputeList = imputeList;

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

                    if(pms != null)
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



                DObject dobj = new DObject();
                dobj.Type = QmsConstant.TYPE_QUICK_RESPONSE;
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
            return Json(result);
        }

        public JsonResult UdtQuickResponse(QuickResponse _param)
        {
            int result = 0;
            DirectoryInfo di = null;
            try
            {
                DaoFactory.BeginTransaction();

                DObject dobj = new DObject();
                dobj.Type = QmsConstant.TYPE_QUICK_RESPONSE;
                dobj.OID = _param.OID;
                DObjectRepository.UdtDObject(Session, dobj);

                QuickResponseRepository.UdtQuickResponse(_param);

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
        #endregion


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

                if (!(blockade.BPolicyNm == QmsConstant.POLICY_QMS_MODULE_PREPARE || blockade.BPolicyNm == QmsConstant.POLICY_QMS_MODULE_STARTED))
                {
                    throw new Exception("봉쇄조치가 진행중인관계로 일정을 수정 할 수 없습니다.");
                }

                _params.ForEach(v =>
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
                        dObject.BPolicyOID = 58; // 작성중으로 변경
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

        public ActionResult InfoMiniBlockade(int OID)
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