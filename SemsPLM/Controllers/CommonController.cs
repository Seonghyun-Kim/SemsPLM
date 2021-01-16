using Common;
using Common.Constant;
using Common.Factory;
using Common.Models;
using Common.Models.File;
using Common.Utils;
using SemsPLM.Filter;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Web;
using System.Web.Mvc;
using Trigger;

namespace SemsPLM.Controllers
{
    [AuthorizeFilter]
    public class CommonController : Controller
    {
        // GET: Common

        #region -- Menu

        public ActionResult Menu()
        {

            ViewBag.Menu = BMenuRepository.SelBMenu();
            return PartialView("Partitial/ptMenu");
        }

        #endregion

        #region -- Person

        public ActionResult ApprovalPerson()
        {
            ViewBag.Organization = CompanyRepository.SelOrganizationWithPerson(Session, new List<string> { CommonConstant.TYPE_PERSON });
            //ViewBag.Organization = CompanyRepository.SelOrganizationWithPerson(null);
            return PartialView("Dialog/dlgApprovalPerson");
        }

        #endregion

        #region -- My Approval

        public ActionResult MyApproval()
        {
            ViewBag.Types = BDefineRepository.SelDefines(new BDefine { Type = CommonConstant.DEFINE_TYPE });
            ViewBag.Status = BPolicyRepository.SelBPolicy(new BPolicy { Type = CommonConstant.TYPE_APPROVAL_TASK });
            return View();
        }

        public JsonResult SelMyApproval(ApprovalTask _param)
        {
            List<ApprovalTask> myApproval = new List<ApprovalTask>();
            ApprovalTaskRepository.SelInboxMyTasks(Session, _param).ForEach(approv =>
            {
                approv.ApprovalBPolicy = DObjectRepository.SelDObject(Session, new DObject { OID = approv.ApprovalOID }).BPolicy;
                if (approv.BPolicy.Name == approv.ApprovalBPolicy.Name)
                {
                    myApproval.Add(approv);
                }
            });
            return Json(myApproval);
        }

        public JsonResult PromoteApprovalTask(ApprovalTask _param)
        {
            try
            {
                DaoFactory.BeginTransaction();
                TriggerUtil.StatusPromote(Session, false, CommonConstant.TYPE_APPROVAL_TASK, Convert.ToString(_param.BPolicyOID), Convert.ToInt32(_param.OID), Convert.ToInt32(_param.OID), _param.ActionType, "");
                ApprovalTaskRepository.UdtInboxTask(new ApprovalTask { ActionType = _param.ActionType, Comment = _param.Comment, OID =  _param.OID});

                if (_param.ActionType == CommonConstant.ACTION_REJECT)
                {
                    Approval tmpApproval = ApprovalRepository.SelApprovalNonStep(Session, new Approval { OID = _param.ApprovalOID });
                    string returnVal = TriggerUtil.StatusPromote(Session, false, CommonConstant.TYPE_APPROVAL, Convert.ToString(tmpApproval.BPolicyOID), Convert.ToInt32(tmpApproval.OID), Convert.ToInt32(tmpApproval.OID), _param.ActionType, "");
                    if (returnVal != null && returnVal.Length > 0)
                    {
                        throw new Exception(returnVal);
                    }

                    returnVal = "";
                    DObject targetDobj = DObjectRepository.SelDObject(Session, new DObject { OID = tmpApproval.TargetOID });
                    returnVal = TriggerUtil.StatusPromote(Session, false, targetDobj.Type, Convert.ToString(targetDobj.BPolicyOID), Convert.ToInt32(targetDobj.OID), Convert.ToInt32(targetDobj.OID), _param.ActionType, "");
                    if (returnVal != null && returnVal.Length > 0)
                    {
                        throw new Exception(returnVal);
                    }
                    DaoFactory.Commit();
                    return Json(CommonConstant.RETURN_SUCCESS);
                }

                bool allSuccess = true;
                ApprovalStep tmpApprovalStep = ApprovalStepRepository.SelApprovalStep(Session, new ApprovalStep { OID = _param.StepOID });
                tmpApprovalStep.InboxTask.ForEach(task =>
                {
                    if (task.BPolicy.Name != CommonConstant.POLICY_APPROVAL_COMPLETED)
                    {
                        allSuccess = false;
                    }
                });

                if (allSuccess)
                {
                   Approval tmpApproval = ApprovalRepository.SelApprovalNonStep(Session, new Approval { OID = _param.ApprovalOID });
                   int cntCurrent = Convert.ToInt32(tmpApproval.CurrentNum);
                   cntCurrent += 1;
                   if (tmpApproval.ApprovalCount >= cntCurrent)
                   {
                        string returnVal = "";
                        ApprovalRepository.UdtApproval(Session, new Approval { CurrentNum = cntCurrent, OID = tmpApproval.OID });
                        ApprovalRepository.SelApproval(Session, new Approval { OID = tmpApproval.OID }).InboxStep.Find(step => step.Ord == cntCurrent).InboxTask.ForEach(task =>
                        {
                            returnVal = "";
                            returnVal = TriggerUtil.StatusPromote(Session, false, CommonConstant.TYPE_APPROVAL_TASK, Convert.ToString(task.BPolicyOID), Convert.ToInt32(task.OID), Convert.ToInt32(task.OID), _param.ActionType, "");
                            if (returnVal != null && returnVal.Length > 0)
                            {
                                throw new Exception(returnVal);
                            }
                        });
                   }
                   else if (tmpApproval.ApprovalCount < cntCurrent)
                   {
                        string returnVal = "";
                        ApprovalRepository.UdtApproval(Session, new Approval { CurrentNum = cntCurrent, OID = tmpApproval.OID });
                        returnVal = TriggerUtil.StatusPromote(Session, false, CommonConstant.TYPE_APPROVAL, Convert.ToString(tmpApproval.BPolicyOID), Convert.ToInt32(tmpApproval.OID), Convert.ToInt32(tmpApproval.OID), _param.ActionType, "");
                        if (returnVal != null && returnVal.Length > 0)
                        {
                            throw new Exception(returnVal);
                        }

                        returnVal = "";
                        DObject targetDobj = DObjectRepository.SelDObject(Session, new DObject { OID = tmpApproval.TargetOID });
                        returnVal = TriggerUtil.StatusPromote(Session, false, targetDobj.Type, Convert.ToString(targetDobj.BPolicyOID), Convert.ToInt32(targetDobj.OID), Convert.ToInt32(targetDobj.OID), _param.ActionType, "");
                        if (returnVal != null && returnVal.Length > 0)
                        {
                            throw new Exception(returnVal);
                        }
                    }
                }
                DaoFactory.Commit();
            }
            catch(Exception ex)
            {
                DaoFactory.Rollback();
                return Json(new ResultJsonModel { isError = true, resultMessage = ex.Message, resultDescription = ex.ToString() });
            }
            return Json(CommonConstant.RETURN_SUCCESS);
        }

        public JsonResult PromoteObjectTask(string Type, string OID, string RootOID, string Status, string GoStatusOID, string Action, string Comment)
        {
            try
            {
                DaoFactory.BeginTransaction();

                string returnVal = "";
                DObject targetDobj = DObjectRepository.SelDObject(Session, new DObject { OID = Convert.ToInt32(OID) });
                returnVal = TriggerUtil.StatusObjectPromote(Session, false, targetDobj.Type, Convert.ToString(targetDobj.BPolicyOID), GoStatusOID, Convert.ToInt32(targetDobj.OID), (RootOID == null ? Convert.ToInt32(targetDobj.OID) : Convert.ToInt32(RootOID)), Action, "");
                if (returnVal != null && returnVal.Length > 0)
                {
                    throw new Exception(returnVal);
                }
                DaoFactory.Commit();
            }
            catch (Exception ex)
            {
                DaoFactory.Rollback();
                return Json(new ResultJsonModel { isError = true, resultMessage = ex.Message, resultDescription = ex.ToString() });
            }
            return Json(CommonConstant.RETURN_SUCCESS);
        }

        #endregion

        #region -- Approval

        public ActionResult Approval(Approval _param)
        {
            ViewBag.Organization = CompanyRepository.SelOrganizationWithPerson(Session, new List<string> { CommonConstant.TYPE_PERSON });
            if (_param.TargetOID != null)
            {
                ViewBag.TargetOID = _param.TargetOID;
            }
            ViewBag.SelApproval = ApprovalRepository.SelSaveApprovalsNonStep(Session, new Approval{Type = Common.Constant.CommonConstant.TYPE_SAVE_APPROVAL });
            return PartialView("Dialog/dlgApproval");
        }

        public JsonResult SelApproval(Approval _param)
        {
            if(_param.OID == null)
            {
                return Json(ApprovalRepository.SelApprovals(Session, _param));
            }
            return Json(null);
        }

        public JsonResult InsApproval(Approval _param)
        {
            int result = 0;
            try
            {
                DaoFactory.BeginTransaction();
                
                DObject dobj = new DObject();
                if (_param.TargetOID == null)
                {
                    dobj.Type = CommonConstant.TYPE_SAVE_APPROVAL;
                    dobj.Name = _param.Name;
                }
                else
                {
                    dobj.Type = CommonConstant.TYPE_APPROVAL;
                    string strApprovalPrefix = dobj.Type + "-" + DateTime.Now.ToString("yyyyMMdd") + "-";
                    dobj.Name = strApprovalPrefix + SemsUtil.MakeSeq(DObjectRepository.SelNameSeq(Session, new DObject { Type = CommonConstant.TYPE_APPROVAL, Name = strApprovalPrefix + "%" }), "000");
                    dobj.Description = _param.Description;
                }
                dobj.TableNm = CommonConstant.TABLE_APPROVAL;
                result = DObjectRepository.InsDObject(Session, dobj);

                _param.OID = result;
                _param.ApprovalCount = _param.InboxStep.FindAll(step => step.ApprovalType.Equals(CommonConstant.TYPE_APPROVAL_APPROV) || step.ApprovalType.Equals(CommonConstant.TYPE_APPROVAL_AGREE)).Count;
                ApprovalRepository.InsApproval(Session, _param);

                List<int> lPromoteOID = new List<int>();
                int index = 0;
                _param.InboxStep.ForEach(step =>
                {
                    step.ApprovalOID = result;
                    int stepResult = ApprovalStepRepository.InsApprovalStep(Session, step);
                    
                    step.InboxTask.ForEach(task =>
                    {
                        if (dobj != null)
                        {
                            dobj = null;
                        }
                        dobj = new DObject();
                        dobj.Type = CommonConstant.TYPE_APPROVAL_TASK;
                        dobj.TableNm = CommonConstant.TABLE_APPROVAL_TASK;
                        string strApprovalPrefix = dobj.Type + "-" + DateTime.Now.ToString("yyyyMMdd") + "-";
                        dobj.Name = strApprovalPrefix + SemsUtil.MakeSeq(DObjectRepository.SelNameSeq(Session, new DObject { Type = CommonConstant.TYPE_APPROVAL_TASK, Name = strApprovalPrefix + "%" }), "000");
                        int taskResult = DObjectRepository.InsDObject(Session, dobj);

                        task.ApprovalOID = result;
                        task.StepOID = stepResult;
                        task.OID = taskResult;
                        ApprovalTaskRepository.InsInboxTask(task);

                        if (index == 0)
                        {
                            lPromoteOID.Add(taskResult);
                        }
                    });
                    index++;
                });

                if (_param.TargetOID != null)
                {
                    if((_param.AutoStatus == null || Convert.ToBoolean(_param.AutoStatus))){
                        DObject targetDobj = DObjectRepository.SelDObject(Session, new DObject { OID = _param.TargetOID });
                        TriggerUtil.StatusPromote(Session, false, targetDobj.Type, Convert.ToString(targetDobj.BPolicyOID), Convert.ToInt32(targetDobj.OID), Convert.ToInt32(targetDobj.OID), CommonConstant.ACTION_PROMOTE, null);
                    }

                    if (lPromoteOID != null && lPromoteOID.Count > 0)
                    {
                        lPromoteOID.ForEach(promoteOID =>
                        {
                            if (dobj != null)
                            {
                                dobj = null;
                            }
                            dobj = DObjectRepository.SelDObject(Session, new DObject { OID = promoteOID });
                            TriggerUtil.StatusPromote(Session, false, dobj.Type, Convert.ToString(dobj.BPolicyOID), Convert.ToInt32(dobj.OID), Convert.ToInt32(dobj.OID), CommonConstant.ACTION_PROMOTE, null);
                        });

                        DObject approvDobj = DObjectRepository.SelDObject(Session, new DObject { OID = result });
                        TriggerUtil.StatusPromote(Session, false, approvDobj.Type, Convert.ToString(approvDobj.BPolicyOID), Convert.ToInt32(approvDobj.OID), Convert.ToInt32(approvDobj.OID), CommonConstant.ACTION_PROMOTE, null);
                    }
                    ApprovalRepository.UdtApproval(Session, new Approval { OID = result, CurrentNum = 1 });
                }
                
                DaoFactory.Commit();
            }
            catch(Exception ex)
            {
                DaoFactory.Rollback();
                return Json(new ResultJsonModel { isError = true, resultMessage = ex.Message, resultDescription = ex.ToString() });
            }
            return Json(result);
        }

        #region -- 김창수 결재선 저장 인원 검색
        public JsonResult SelApprovalPersonHistory(DObject _param)
        {
            List<ApprovalTask> displayTask = new List<ApprovalTask>();
            displayTask = ApprovalTaskRepository.SelInboxTasks(Session, new ApprovalTask { ApprovalOID = _param.OID });
            return Json(displayTask);
        }
        #endregion

        #region -- 김창수 결재선 저장 후 불러오기
        public JsonResult SelSaveApprovalsLoad(Approval _param)
        {
            if (_param.OID == null)
            {
                return Json(ApprovalRepository.SelSaveApprovalsNonStep(Session, new Approval { Type = Common.Constant.CommonConstant.TYPE_SAVE_APPROVAL }));
            }
            return Json(null);
        }
        #endregion

        #endregion

        #region -- Approval History

        public ActionResult ApprovalHistory(DObject _param)
        {
            ViewBag.TargetOID = _param.OID;
            return PartialView("Partitial/ptApprovalHistory");
        }

        public JsonResult SelApprovalHistory(DObject _param)
        {
            List<ApprovalTask> displayTask = new List<ApprovalTask>();
            Approval approval = ApprovalRepository.SelApproval(Session, new Approval { TargetOID = _param.OID });
            if (approval != null)
            {
                approval.InboxStep.ForEach(step =>
                {
                    step.InboxTask.ForEach(task =>
                    {
                        task.Ord = step.Ord;
                        task.CurrentNum = approval.CurrentNum;
                        displayTask.Add(task);
                    });
                });
            }
            return Json(displayTask);
        }

        #endregion

        #region -- Approval Content
        public ActionResult ApprovalContent(ApprovalTask _param)
        {
            ViewBag.ApprvalData = ApprovalRepository.SelApproval(Session, new Approval { OID = _param.ApprovalOID });
            ViewBag.ApprovalTaskData = _param;
            return PartialView("Dialog/dlgApprovalContent");
        }

        public JsonResult InsApprovalComment(ApprovalComment _param)
        {
            ApprovalCommentRepository.InsApprovalComment(Session, _param);
            return Json(ApprovalCommentRepository.SelApprovalComment(Session, new ApprovalComment { ApprovalOID = _param.ApprovalOID } ));
        }

        #endregion

        #region -- Image

        public JsonResult ImgUploadFile(string OID)
        {
            string VAULT_PATH = Server.MapPath("~/images/Thumbnail/");
            var fileName = "";
            if (Request.Files.Count > 0)
            {
                HttpFileCollectionBase files = Request.Files;
                HttpPostedFileBase File = files[0];
                try
                {
                    DirectoryInfo di = new DirectoryInfo(VAULT_PATH);

                    if (!di.Exists)
                    {
                        di.Create();
                    }

                    System.TimeSpan tspan = System.TimeSpan.FromTicks(DateTime.Now.Ticks);
                    long curTime = (long)tspan.TotalSeconds;
                    string file_nm = Path.GetFileName(File.FileName);
                    string file_format = file_nm.Substring(file_nm.LastIndexOf(".") + 1);
                    string cfile_nm = curTime.ToString() + "." + file_format;

                    FileStream fs = System.IO.File.Create(VAULT_PATH + "\\" + cfile_nm);
                    File.InputStream.CopyTo(fs);

                    fileName = cfile_nm;

                    fs.Close();
                    File.InputStream.Close();

                    if (OID != null && OID.Length > 0) {
                        DObjectRepository.UdtDObject(Session, new DObject { OID = Convert.ToInt32(OID), Thumbnail = fileName });
                    }
                }
                catch (Exception ex)
                {
                    return Json(new ResultJsonModel { isError = true, resultMessage = ex.Message, resultDescription = ex.ToString() });
                }
            }
            return Json(fileName);
        }

        public JsonResult TmpUploadFile(string OID)
        {
            string VAULT_PATH = Server.MapPath("~/TmpFile/");
            Dictionary<string, string> dFiles = new Dictionary<string, string>();
            if (Request.Files.Count > 0)
            {
                HttpFileCollectionBase files = Request.Files;
                HttpPostedFileBase File = files[0];
                try
                {
                    DirectoryInfo di = new DirectoryInfo(VAULT_PATH);

                    if (!di.Exists)
                    {
                        di.Create();
                    }

                    System.TimeSpan tspan = System.TimeSpan.FromTicks(DateTime.Now.Ticks);
                    long curTime = (long)tspan.TotalSeconds;
                    string file_nm = Path.GetFileName(File.FileName);
                    string file_format = file_nm.Substring(file_nm.LastIndexOf(".") + 1);
                    string cfile_nm = curTime.ToString() + "." + file_format;

                    FileStream fs = System.IO.File.Create(VAULT_PATH + "\\" + cfile_nm);
                    File.InputStream.CopyTo(fs);

                    dFiles.Add("Origin", File.FileName);
                    dFiles.Add("Export", cfile_nm);

                    fs.Close();
                    File.InputStream.Close();

                }
                catch (Exception ex)
                {
                    return Json(new ResultJsonModel { isError = true, resultMessage = ex.Message, resultDescription = ex.ToString() });
                }
            }
            return Json(dFiles);
        }

        #endregion

        #region -- File 
        public JsonResult GetFileList(HttpFile httpFile)
        {
            try
            {
                return Json(HttpFileRepository.SelFiles(Session, httpFile));
            }
            catch (Exception ex)
            {
                return Json(new ResultJsonModel { isError = true, resultMessage = ex.Message, resultDescription = ex.ToString() });
            }

        }

        [HttpGet]
        public ActionResult CommonFileDownload(HttpFile fileModel)
        {
            try
            {
                HttpFile downFile = HttpFileRepository.SelFile(Session, fileModel);

                if (downFile == null || downFile.FileOID == null)
                {
                    throw new Exception("잘못된 호출입니다.");
                }

                System.IO.Stream fileStream = SemsValut.GetFileStream(downFile);
                //new ActionLog(downFile, eActionType.DOWNLOAD, null).InsertData();

                if (Request.Browser.Browser == "IE" || Request.Browser.Browser == "InternetExplorer")
                {
                    return File(fileStream, MediaTypeNames.Application.Octet, HttpUtility.UrlEncode(downFile.OrgNm, System.Text.Encoding.UTF8));
                }
                else
                {
                    return File(fileStream, MediaTypeNames.Application.Octet, downFile.OrgNm);
                }

            }
            catch (Exception ex)
            {
                string message = ex.Message.Replace("'", "");
                return Content("<script language='javascript' type='text/javascript'>alert('" + message + "');history.back();</script>");
            }
        }

        public JsonResult CommonFilePath(HttpFile fileModel)
        {
            try
            {
                HttpFile downFile = HttpFileRepository.SelFile(Session, fileModel);

                if (downFile == null || downFile.FileOID == null)
                {
                    throw new Exception("잘못된 호출입니다.");
                }

                string filePath = SemsValut.GetFileString(downFile);
                return Json(filePath.Replace("\\", "/"));
            }
            catch (Exception ex)
            {
                return Json(new ResultJsonModel { isError = true, resultMessage = ex.Message, resultDescription = ex.ToString() });
            }
        }
        #endregion

        #region -- Notice
        public ActionResult EditNotice(string OID)
        {
            DObject tmpNotice = DObjectRepository.SelDObject(Session, new DObject { OID = Convert.ToInt32(OID) });
            if (tmpNotice != null)
            {
                tmpNotice.CreateUsNm = PersonRepository.SelPerson(Session, new Person { OID = tmpNotice.CreateUs }).Name;
            }
            ViewBag.OID = OID;
            ViewBag.Detail = tmpNotice;
            return PartialView("Dialog/dlgEditNotice");
        }

        public ActionResult InsNotice(DObject _param)
        {
            int result = 0;
            try
            {
                DaoFactory.BeginTransaction();

                DObject dobj = new DObject();
                dobj.Type = CommonConstant.TYPE_NOTICE;
                dobj.Name = _param.Name;
                dobj.CreateUs = Convert.ToInt32(Session["UserOID"]);
                dobj.Description = _param.Description;
                result = DObjectRepository.InsDObject(Session, dobj);

                DaoFactory.Commit();
            }
            catch (Exception ex)
            {
                DaoFactory.Rollback();
                return Json(new ResultJsonModel { isError = true, resultMessage = ex.Message, resultDescription = ex.ToString() });
            }
            return Json(result);
        }

        public ActionResult SelNotice(DObject _param)
        {
            List<DObject> dobj = DObjectRepository.SelDObjects(Session, new DObject { Type = CommonConstant.TYPE_NOTICE, OID = _param.OID });
            if(dobj.Count > 0)
            {
                dobj.ForEach(obj =>
                {
                    obj.CreateUsNm = PersonRepository.SelPerson(Session, new Person { OID = obj.CreateUs }).Name;
                });
            }
            return Json(dobj);
        }

        public JsonResult UdtNotice(DObject _param)
        {
            int result = 0;
            try
            {
                DaoFactory.BeginTransaction();

                DObject dobj = new DObject();
                dobj.Type = CommonConstant.TYPE_NOTICE;
                dobj.Name = _param.Name;
                dobj.OID = _param.OID;
                dobj.Description = _param.Description;
                result = DObjectRepository.UdtDObject(Session, dobj);

                DaoFactory.Commit();
            }
            catch (Exception ex)
            {
                DaoFactory.Rollback();
                return Json(new ResultJsonModel { isError = true, resultMessage = ex.Message, resultDescription = ex.ToString() });

            }
            return Json(result);
        }

        public JsonResult DelNotice(DObject _param)
        {
            try
            {
                DaoFactory.BeginTransaction();
                DObjectRepository.DelDObject(Session, _param, null);
                DaoFactory.Commit();
            }
            catch (Exception ex)
            {
                DaoFactory.Rollback();
                return Json(new ResultJsonModel { isError = true, resultMessage = ex.Message, resultDescription = ex.ToString() });
            }
            return Json(0);
        }

        #endregion

        #region -- Approval Dashboard

        public JsonResult ApprvalDashboard()
        {
            Dictionary<string, object> lResult = new Dictionary<string, object>();
            int startedCount = 0, weekCompleteCount = 0, prepareCount = 0, rejectCount = 0;
            int iUser = Convert.ToInt32(Session["UserOID"]);
            List<BPolicy> lBPolicy = BPolicyRepository.SelBPolicy(new BPolicy { Type = CommonConstant.TYPE_APPROVAL });

            List<Approval> lApproval = ApprovalRepository.SelApprovalsNonStep(Session, new Approval { Type = CommonConstant.TYPE_APPROVAL, CreateUs = iUser });
            int startedOID = Convert.ToInt32(lBPolicy.Find(bpolicy => bpolicy.Name == CommonConstant.POLICY_APPROVAL_STARTED).OID);
            int rejectOID = Convert.ToInt32(lBPolicy.Find(bpolicy => bpolicy.Name == CommonConstant.POLICY_APPROVAL_REJECTED).OID);
            int completeOID = Convert.ToInt32(lBPolicy.Find(bpolicy => bpolicy.Name == CommonConstant.POLICY_APPROVAL_COMPLETED).OID);
            int weekNum = Pms.PmsUtils.CalculateWeekNumber(Convert.ToDateTime(string.Format("{0:yyyy-MM-dd}", DateTime.Now)));

            lApproval.ForEach(approv =>
            {
                if (approv.BPolicyOID == startedOID)
                {
                    startedCount++;
                }
                else if (approv.BPolicyOID == rejectOID)
                {
                    rejectCount++;
                }
                else if (approv.BPolicyOID == completeOID)
                {
                    int innerWeekNum = Pms.PmsUtils.CalculateWeekNumber(Convert.ToDateTime(string.Format("{0:yyyy-MM-dd}", approv.ModifyDt)));
                    if (weekNum == innerWeekNum)
                    {
                        weekCompleteCount++;
                    }
                }
            });

            List<BPolicy> lBPolicyTask = BPolicyRepository.SelBPolicy(new BPolicy { Type = CommonConstant.TYPE_APPROVAL_TASK });
            int taskStartedOID = Convert.ToInt32(lBPolicyTask.Find(bpolicy => bpolicy.Name == CommonConstant.POLICY_APPROVAL_TASK_STARTED).OID);
            ApprovalTaskRepository.SelInboxMyTasks(Session, new ApprovalTask { BPolicyOID = taskStartedOID, PersonOID = iUser }).ForEach(approv =>
            {
                prepareCount++;
            });

            lResult.Add("started", startedCount);
            lResult.Add("weekComplete", weekCompleteCount);
            lResult.Add("prepare", prepareCount);
            lResult.Add("reject", rejectCount);
            return Json(lResult);
        }

        public JsonResult ApprvalListDashboard(Approval _param)
        {
            List<Approval> myApproval = new List<Approval>();
            int iUser = Convert.ToInt32(Session["UserOID"]);
            List<BPolicy> lBPolicy = BPolicyRepository.SelBPolicy(new BPolicy { Type = CommonConstant.TYPE_APPROVAL });
            int statusOID = Convert.ToInt32(lBPolicy.Find(bpolicy => bpolicy.Name == _param.BPolicyNm).OID);
            List<Approval> lApproval = ApprovalRepository.SelApprovalsNonStep(Session, new Approval { Type = CommonConstant.TYPE_APPROVAL, CreateUs = iUser, BPolicyOID = statusOID });
            int weekNum = Pms.PmsUtils.CalculateWeekNumber(Convert.ToDateTime(string.Format("{0:yyyy-MM-dd}", DateTime.Now)));

            List<BDefine> lBDefine = BDefineRepository.SelDefines(new BDefine { Type = CommonConstant.DEFINE_TYPE });
            DObject dobj = null;
            lApproval.ForEach(approv =>
            {
                if (dobj != null)
                {
                    dobj = null;
                }
                
                dobj = DObjectRepository.SelDObject(Session, new DObject { OID = approv.TargetOID });
                if (dobj == null)
                {
                    return;
                }
                approv.DocType = lBDefine.Find(define => define.Name == dobj.Type).Description;
                approv.DocNm = dobj.Name;
                approv.DocCreateUs = dobj.CreateUs;
                approv.DocCreateNm = PersonRepository.SelPerson(Session, new Person { OID = dobj.CreateUs }).Name;
                approv.DocBpolicyNm = dobj.BPolicy.StatusNm;

                int innerWeekNum = -1;
                if (_param.BPolicyNm == CommonConstant.POLICY_APPROVAL_COMPLETED)
                {
                    innerWeekNum = Pms.PmsUtils.CalculateWeekNumber(Convert.ToDateTime(string.Format("{0:yyyy-MM-dd}", approv.ModifyDt)));
                    if (innerWeekNum == weekNum)
                    {
                        myApproval.Add(approv);
                    }
                }
                else
                {
                    myApproval.Add(approv);
                }

            });
            return Json(myApproval);
        }


        #endregion
    }
}