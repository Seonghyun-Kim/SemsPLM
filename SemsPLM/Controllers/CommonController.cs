using Common;
using Common.Constant;
using Common.Factory;
using Common.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SemsPLM.Controllers
{
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
            ViewBag.Organization = CompanyRepository.SelOrganizationWithPerson(new List<string> { CommonConstant.TYPE_PERSON });
            //ViewBag.Organization = CompanyRepository.SelOrganizationWithPerson(null);
            return PartialView("Dialog/dlgApprovalPerson");
        }

        #endregion

        #region -- Approval

        public ActionResult Approval(Approval _param)
        {
            ViewBag.Organization = CompanyRepository.SelOrganizationWithPerson(new List<string> { CommonConstant.TYPE_PERSON });
            if (_param.TargetOID != null)
            {
                ViewBag.TargetOID = _param.TargetOID;
            }
            return PartialView("Dialog/dlgApproval");
        }

        public JsonResult SelApproval(Approval _param)
        {
            if(_param.OID == null)
            {
                return Json(ApprovalRepository.SelApprovals(_param));
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
                    dobj.Name = strApprovalPrefix + SemsUtil.MakeSeq(DObjectRepository.SelNameSeq(new DObject { Type = CommonConstant.TYPE_APPROVAL, Name = strApprovalPrefix + "%" }), "000");
                }
                dobj.TableNm = CommonConstant.TABLE_APPROVAL;
                result = DObjectRepository.InsDObject(dobj);

                _param.OID = result;
                _param.ApprovalCount = _param.InboxStep.FindAll(step => step.ApprovalType.Equals(CommonConstant.TYPE_APPROVAL_APPROV) || step.ApprovalType.Equals(CommonConstant.TYPE_APPROVAL_AGREE)).Count;
                ApprovalRepository.InsApproval(_param);

                List<int> lPromoteOID = new List<int>();
                int index = 0;
                _param.InboxStep.ForEach(step =>
                {
                    step.ApprovalOID = result;
                    int stepResult = ApprovalStepRepository.InsApprovalStep(step);
                    
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
                        dobj.Name = strApprovalPrefix + SemsUtil.MakeSeq(DObjectRepository.SelNameSeq(new DObject { Type = CommonConstant.TYPE_APPROVAL_TASK, Name = strApprovalPrefix + "%" }), "000");
                        int taskResult = DObjectRepository.InsDObject(dobj);

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
                DaoFactory.Commit();

                DaoFactory.BeginTransaction();
                DObject targetDobj = DObjectRepository.SelDObject(new DObject { OID = _param.TargetOID });
                DObjectRepository.StatusPromote(false, targetDobj.Type, Convert.ToString(targetDobj.BPolicyOID), Convert.ToInt32(targetDobj.OID), Convert.ToInt32(targetDobj.OID), CommonConstant.ACTION_PROMOTE, null);
                if (lPromoteOID != null && lPromoteOID.Count > 0)
                {
                    lPromoteOID.ForEach(promoteOID =>
                    {
                        if (dobj != null)
                        {
                            dobj = null;
                        }
                        dobj = DObjectRepository.SelDObject(new DObject { OID = promoteOID });
                        DObjectRepository.StatusPromote(false, dobj.Type, Convert.ToString(dobj.BPolicyOID), Convert.ToInt32(dobj.OID), Convert.ToInt32(dobj.OID), CommonConstant.ACTION_PROMOTE, null);
                    });
                }
                ApprovalRepository.UdtApproval(new Approval { OID = result, CurrentNum = 1 });
                DaoFactory.Commit();
            }
            catch(Exception ex)
            {
                DaoFactory.Rollback();
                return Json(new ResultJsonModel { isError = true, resultMessage = ex.Message, resultDescription = ex.ToString() });
            }
            return Json(result);
        }

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
            Approval approval = ApprovalRepository.SelApproval(new Approval { TargetOID = _param.OID });
            approval.InboxStep.ForEach(step =>
            {
                step.InboxTask.ForEach(task =>
                {
                    task.Ord = step.Ord;
                    task.CurrentNum = approval.CurrentNum;
                    displayTask.Add(task);
                });
            });
            return Json(displayTask);
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
                        DObjectRepository.UdtDObject(new DObject { OID = Convert.ToInt32(OID), Thumbnail = fileName });
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

    }
}