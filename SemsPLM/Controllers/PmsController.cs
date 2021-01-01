using Common;
using Common.Constant;
using Common.Factory;
using Common.Models;
using Common.Models.File;
using Document.Models;
using DocumentClassification.Models;
using EBom.Models;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using Pms;
using Pms.Auth;
using Pms.Interface;
using Pms.Models;
using SemsPLM.Filter;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Services.Description;
using Trigger;

namespace SemsPLM.Controllers
{
    [AuthorizeFilter]
    public class PmsController : Controller
    {
        public PartialViewResult CreateProjectContent(string Mode)
        {
            string partialUrl = "";
            switch (Mode)
            {
                case "Calendar":
                    partialUrl = "Dialog/dlgCreateProjectContentCalendar";
                    break;
                case "Base":
                    partialUrl = "Dialog/dlgCreateProjectContentBase";
                    break;
                case "Template":
                    partialUrl = "Dialog/dlgCreateProjectContentTemplate";
                    break;
                case "ProdecessorProject":
                    partialUrl = "Dialog/dlgCreateProjectContentProdecessor";
                    break;
            }
            return PartialView(partialUrl);
        }

        #region -- Approval

        public ActionResult InfoMiniProject(string OID)
        {
            DObject dobj = DObjectRepository.SelDObject(Session, new DObject { OID = Convert.ToInt32(OID) });
            if (dobj.Type != PmsConstant.TYPE_PROJECT)
            {
                PmsRelationship parentRelationship = PmsRelationshipRepository.SelPmsRelationship(Session, new PmsRelationship { Type = PmsConstant.RELATIONSHIP_WBS, ToOID = Convert.ToInt32(OID) }).First();
                OID = Convert.ToString(parentRelationship.RootOID);
            }
            ViewBag.OID = OID;
            ViewBag.Detail = PmsProjectRepository.SelPmsObject(Session, new PmsProject { OID = Convert.ToInt32(OID) });
            ViewBag.GanttUrl = "/Pms/DetailGanttView?OID=" + OID;
            ViewBag.Holiday = string.Join(",", CalendarDetailRepository.SelCalendarDetails(new CalendarDetail { CalendarOID = Convert.ToInt32(ViewBag.Detail.CalendarOID) }).Select(value => value.FullDate.ToString()).ToArray());
            ViewBag.Status = BPolicyRepository.SelBPolicy(new BPolicy { Type = PmsConstant.TYPE_PROJECT });
            ViewBag.Role = BDefineRepository.SelDefines(new BDefine { Type = CommonConstant.DEFINE_ROLE, Module = PmsConstant.MODULE_PMS });
            return PartialView("InfoProject");
        }

        public ActionResult InfoMiniProcess(string OID)
        {
            PmsRelationship parentRelationship = PmsRelationshipRepository.SelPmsRelationship(Session, new PmsRelationship { Type = PmsConstant.RELATIONSHIP_WBS, ToOID = Convert.ToInt32(OID) }).First();
            ViewBag.ProjectOID = parentRelationship.RootOID;
            ViewBag.ProjectDetail = PmsProjectRepository.SelPmsObject(Session, new PmsProject { OID = Convert.ToInt32(parentRelationship.RootOID) });
            ViewBag.OID = OID;
            ViewBag.Detail = PmsProcessRepository.SelPmsProcess(Session, new PmsProcess { OID = Convert.ToInt32(OID) });
            ViewBag.Holiday = string.Join(",", CalendarDetailRepository.SelCalendarDetails(new CalendarDetail { CalendarOID = Convert.ToInt32(ViewBag.ProjectDetail.CalendarOID) }).Select(value => value.FullDate.ToString()).ToArray());
            ViewBag.Status = BPolicyRepository.SelBPolicy(new BPolicy { Type = ViewBag.Detail.ProcessType });
            ViewBag.Role = BDefineRepository.SelDefines(new BDefine { Type = CommonConstant.DEFINE_ROLE, Module = PmsConstant.MODULE_PMS });
            ViewBag.ParentType = DObjectRepository.SelDObject(Session, new DObject { OID = Convert.ToInt32(parentRelationship.FromOID) }).Type;
            Approval approv = ApprovalRepository.SelApprovalNonStep(Session, new Approval { TargetOID = Convert.ToInt32(OID) });
            if (approv != null)
            {
                ViewBag.ApprovStatus = DObjectRepository.SelDObject(Session, new DObject { OID = approv.OID }).BPolicy.Name;
            }
            else
            {
                ViewBag.ApprovStatus = null;
            }
            return PartialView("InfoProcess");
        }

        #endregion

        #region -- Project

        public JsonResult SelOemCarData()
        {
            return Json(PmsProjectRepository.SelTotalProjMngt(Session, ""));
        }

        public ActionResult SearchProject()
        {
            return View();
        }

        public ActionResult CreateProject()
        {
            Library ItemKey = LibraryRepository.SelCodeLibraryObject(new Library { Code1 = CommonConstant.ATTRIBUTE_ITEM });
            Library placeKey = LibraryRepository.SelLibraryObject(new Library { Name = CommonConstant.ATTRIBUTE_PRODUCED_PLACE  });
            Library epartKey = LibraryRepository.SelLibraryObject(new Library { Name = CommonConstant.ATTRIBUTE_EPARTTYPE });
            Library oemKey = LibraryRepository.SelCodeLibraryObject(new Library { Code1 = CommonConstant.ATTRIBUTE_OEM });
            Library custKey = LibraryRepository.SelLibraryObject(new Library { Name = CommonConstant.ATTRIBUTE_CUSTOMER });

            List<Library> ItemList = LibraryRepository.SelCodeLibrary(new Library { FromOID = ItemKey.OID });  //item 목록
            List<Library> placeList = LibraryRepository.SelLibrary(new Library { FromOID = placeKey.OID });  //생산지 목록
            List<Library> epartList = LibraryRepository.SelLibrary(new Library { FromOID = epartKey.OID });  //제품구분 목록
            List<Library> oemList = LibraryRepository.SelCodeLibrary(new Library { FromOID = oemKey.OID });  //oem 목록
            List<Library> custList = LibraryRepository.SelLibrary(new Library { FromOID = custKey.OID });  //고객사목록


            ViewBag.ItemList = ItemList;
            ViewBag.placeList = placeList;
            ViewBag.epartList = epartList;
            ViewBag.oemList = oemList;
            ViewBag.custList = custList;
            return PartialView("Dialog/dlgCreateProject");
        }
        public ActionResult ModifyProject(string OID)
        {
            PmsProject tmpProj = PmsProjectRepository.SelPmsObject(Session, new PmsProject { OID = Convert.ToInt32(OID) });
            if (tmpProj.BPolicyAuths.FindIndex(auth => auth.AuthNm == CommonConstant.AUTH_MODIFY) < 0)
            {
                return PartialView();
            }
            Library custKey = LibraryRepository.SelLibraryObject(new Library { Name = CommonConstant.ATTRIBUTE_CUSTOMER });
            List<Library> custList = LibraryRepository.SelLibrary(new Library { FromOID = custKey.OID });  //고객사목록

            ViewBag.OID = OID;
            ViewBag.custList = custList;
            ViewBag.Detail = tmpProj;
            return PartialView("Dialog/dlgModifyProject");
        }
        public ActionResult InfoProject(string OID)
        {
            ViewBag.OID = OID;
            ViewBag.GanttUrl = "/Pms/DetailGanttView?OID=" + OID;
            ViewBag.Detail = PmsProjectRepository.SelPmsObject(Session, new PmsProject { OID = Convert.ToInt32(OID) });
            ViewBag.Holiday = string.Join(",", CalendarDetailRepository.SelCalendarDetails(new CalendarDetail { CalendarOID = Convert.ToInt32(ViewBag.Detail.CalendarOID) }).Select(value => value.FullDate.ToString()).ToArray());
            ViewBag.Status = BPolicyRepository.SelBPolicy(new BPolicy { Type = PmsConstant.TYPE_PROJECT });
            ViewBag.Role = BDefineRepository.SelDefines(new BDefine { Type = CommonConstant.DEFINE_ROLE, Module = PmsConstant.MODULE_PMS });
            return View();
        }

        public JsonResult InsProject(PmsProject _param)
        {
            int resultOid = 0;
            List<DateTime> lHoliday = CalendarDetailRepository.SelCalendarDetails(new CalendarDetail { CalendarOID = Convert.ToInt32(_param.CalendarOID), IsHoliday = 1 }).Select(val => DateTime.Parse(val.Year + "-" + val.Month + "-" + val.Day)).ToList();
            DObject dobj = null;
            PmsRelationship relDobj = null;
            try
            {
                DaoFactory.BeginTransaction();

                dobj = new DObject();
                //dobj.Type = PmsConstant.TYPE_PROJECT;
                if (_param.Type == null)
                {
                    dobj.Type = PmsConstant.TYPE_PROJECT;
                }
                else
                {
                    dobj.Type = _param.Type;
                }
                dobj.TableNm = PmsConstant.TABLE_PROJECT;
                dobj.Name = _param.Name;
                dobj.Description = _param.Description;
                resultOid = DObjectRepository.InsDObject(Session, dobj);

                _param.OID = resultOid;
                _param.Complete = PmsConstant.INIT_COMPLETE;
                _param.EstStartDt = _param.BaseDt;
                _param.EstDuration = PmsConstant.INIT_DURATION;
                _param.EstEndDt = PmsUtils.CalculateFutureDate(Convert.ToDateTime(_param.BaseDt), Convert.ToInt32(_param.EstDuration), Convert.ToInt32(_param.WorkingDay), lHoliday);
                PmsProjectRepository.InsPmsProject(Session, _param);

                relDobj = new PmsRelationship();
                relDobj.Type = PmsConstant.RELATIONSHIP_MEMBER;
                relDobj.FromOID = resultOid;
                relDobj.ToOID = Convert.ToInt32(Session["UserOID"]);
                relDobj.RoleOID = BDefineRepository.SelDefine(new BDefine { Type = CommonConstant.DEFINE_ROLE, Module = PmsConstant.MODULE_PMS, Name = PmsConstant.ROLE_PM }).OID;
                relDobj.RootOID = resultOid;
                PmsRelationshipRepository.InsPmsRelationship(Session, relDobj);

                if (_param.TemplateOID != null && _param.TemplateOID > 0)
                {
                    Dictionary<int, int> mapperOid = new Dictionary<int, int>();
                    PmsProject tmpProj = PmsProjectRepository.SelPmsObject(Session, new PmsProject { Type = PmsConstant.TYPE_PROJECT_TEMP, IsTemplate = PmsConstant.TYPE_PROJECT_TEMP, OID = Convert.ToInt32(_param.TemplateOID) });
                    PmsRelationship tmpPmsRelationship = null;

                    if (_param.TemplateContent.IndexOf(PmsConstant.RELATIONSHIP_WBS) > -1)
                    {
                        int targetOid = 0;
                        DObject tmpDobj = null;
                        PmsProject cProj = null;
                        PmsProcess tmpProc = null;
                        
                        List<PmsRelationship> lWbs = PmsRelationshipRepository.GetProjWbsLIst(Session, Convert.ToString(tmpProj.OID));
                        lWbs.ForEach(wbs =>
                        {
                            targetOid = 0;
                            if (tmpDobj != null)
                            {
                                tmpDobj = null;
                            }
                            if (tmpProc != null)
                            {
                                tmpProc = null;
                            }

                            if (wbs.ObjType == PmsConstant.TYPE_PROJECT_TEMP)
                            {
                                cProj = PmsProjectRepository.SelPmsObject(Session, new PmsProject { OID = resultOid });
                                PmsProjectRepository.UdtPmsProject(Session, 
                                    new PmsProject { 
                                        EstDuration = tmpProj.EstDuration,
                                        EstEndDt = PmsUtils.CalculateFutureDate(Convert.ToDateTime(cProj.EstStartDt), Convert.ToInt32(tmpProj.EstDuration), Convert.ToInt32(cProj.WorkingDay), lHoliday), 
                                        OID = cProj.OID });
                                mapperOid.Add(Convert.ToInt32(tmpProj.OID), resultOid);
                                return;
                            }

                            tmpDobj = new DObject();
                            tmpDobj.Type = wbs.ObjType;
                            tmpDobj.TableNm = PmsConstant.TABLE_PROCESS;
                            tmpDobj.Name = wbs.ObjName;
                            targetOid = DObjectRepository.InsDObject(Session, tmpDobj);

                            if (targetOid > 0)
                            {
                                int EstGap = PmsUtils.CalculateGapFutureDuration(Convert.ToDateTime(string.Format("{0:yyyy-MM-dd}", tmpProj.EstStartDt)), Convert.ToDateTime(string.Format("{0:yyyy-MM-dd}", wbs.EstStartDt)), Convert.ToInt32(cProj.WorkingDay), lHoliday);

                                tmpProc = new PmsProcess();
                                tmpProc.OID = targetOid;
                                tmpProc.ProcessType = wbs.ObjType;
                                tmpProc.Id = wbs.Id;
                                tmpProc.Dependency = wbs.Dependency;
                                tmpProc.EstDuration = wbs.EstDuration;
                                tmpProc.EstStartDt = PmsUtils.CalculateFutureDate(Convert.ToDateTime(string.Format("{0:yyyy-MM-dd}", cProj.EstStartDt)), EstGap, Convert.ToInt32(cProj.WorkingDay), lHoliday);
                                tmpProc.EstEndDt = PmsUtils.CalculateFutureDate(Convert.ToDateTime(string.Format("{0:yyyy-MM-dd}", tmpProc.EstStartDt)), Convert.ToInt32(wbs.EstDuration), Convert.ToInt32(cProj.WorkingDay), lHoliday);
                                tmpProc.Level = wbs.Level;
                                tmpProc.Complete = PmsConstant.INIT_COMPLETE;
                                tmpProc.No = wbs.No;
                                PmsProcessRepository.InsPmsProcess(Session, tmpProc);
                                mapperOid.Add(Convert.ToInt32(wbs.ToOID), targetOid);
                            }
                        });

                        lWbs.ForEach(wbs =>
                        {
                            if (tmpPmsRelationship != null)
                            {
                                tmpPmsRelationship = null;
                            }
                            if (wbs.ObjType == PmsConstant.TYPE_PROJECT_TEMP)
                            {
                                return;
                            }
                            tmpPmsRelationship = new PmsRelationship();
                            tmpPmsRelationship.RootOID = resultOid;
                            tmpPmsRelationship.FromOID = mapperOid[Convert.ToInt32(wbs.FromOID)];
                            tmpPmsRelationship.ToOID = mapperOid[Convert.ToInt32(wbs.ToOID)];
                            tmpPmsRelationship.Ord = wbs.Ord;
                            tmpPmsRelationship.Type = wbs.Type;
                            PmsRelationshipRepository.InsPmsRelationship(Session, tmpPmsRelationship);
                        });
                    }

                    if (_param.TemplateContent.IndexOf(PmsConstant.RELATIONSHIP_MEMBER) > -1)
                    {
                        List<PmsRelationship> lMember = PmsRelationshipRepository.SelPmsRelationship(Session, new PmsRelationship { Type = PmsConstant.RELATIONSHIP_MEMBER, RootOID = tmpProj.OID });
                        lMember.ForEach(item =>
                        {
                            if (item.RoleOID == relDobj.RoleOID)
                            {
                                return;
                            }

                            if (tmpPmsRelationship != null)
                            {
                                tmpPmsRelationship = null;
                            }
                            tmpPmsRelationship = new PmsRelationship();
                            tmpPmsRelationship.RootOID = resultOid;
                            tmpPmsRelationship.FromOID = mapperOid[Convert.ToInt32(item.FromOID)];
                            tmpPmsRelationship.ToOID = Convert.ToInt32(item.ToOID);
                            tmpPmsRelationship.Ord = item.Ord;
                            tmpPmsRelationship.Type = item.Type;
                            tmpPmsRelationship.RoleOID = item.RoleOID;
                            PmsRelationshipRepository.InsPmsRelationship(Session, tmpPmsRelationship);
                        });
                    }

                    if (_param.TemplateContent.IndexOf(PmsConstant.RELATIONSHIP_DOC_MASTER) > -1)
                    {
                        List<PmsRelationship> lDocMaster = PmsRelationshipRepository.SelPmsRelationship(Session, new PmsRelationship { Type = PmsConstant.RELATIONSHIP_DOC_MASTER, RootOID = tmpProj.OID });
                        lDocMaster.ForEach(item =>
                        {
                            if (tmpPmsRelationship != null)
                            {
                                tmpPmsRelationship = null;
                            }
                            tmpPmsRelationship = new PmsRelationship();
                            tmpPmsRelationship.RootOID = resultOid;
                            tmpPmsRelationship.FromOID = mapperOid[Convert.ToInt32(item.FromOID)];
                            tmpPmsRelationship.ToOID = Convert.ToInt32(item.ToOID);
                            tmpPmsRelationship.Ord = item.Ord;
                            tmpPmsRelationship.Type = item.Type;
                            PmsRelationshipRepository.InsPmsRelationship(Session, tmpPmsRelationship);
                        });
                    }
                }
                DaoFactory.Commit();
            }
            catch (Exception ex)
            {
                DaoFactory.Rollback();
                return Json(new ResultJsonModel { isError = true, resultMessage = ex.Message, resultDescription = ex.ToString() });
            }
            finally
            {
                dobj = null;
                relDobj = null;
            }
            return Json(resultOid);
        }

        public JsonResult SelProjects(PmsProject _param)
        {
            List<PmsProject> lPmses = PmsProjectRepository.SelPmsObjects(Session, _param);
            return Json(lPmses);
        }
        public JsonResult SelProject(PmsProject _param)
        {
            PmsProject PmsInfo = PmsProjectRepository.SelPmsObject(Session, _param);
            return Json(PmsInfo);
        }
        public JsonResult UdtProject(PmsProject _param)
        {
            int result = 0;
            try
            {
                DaoFactory.BeginTransaction();

                DObjectRepository.UdtDObject(Session, _param);
                PmsProjectRepository.UdtPmsProject(Session, _param);

                DaoFactory.Commit();

            }
            catch (Exception ex)
            {
                DaoFactory.Rollback();
                return Json(new ResultJsonModel { isError = true, resultMessage = ex.Message, resultDescription = ex.ToString() });
            }
            return Json(result);
        }

        public JsonResult InsProjectEPartRelation(PmsRelationship _param)
        {
            int result = 0;
            try
            {
                DaoFactory.BeginTransaction();
                _param.Type = Common.Constant.PmsConstant.RELATIONSHIP_PROJECT_EPART;
                PmsRelationshipRepository.InsPmsRelationship(Session, _param);
                DaoFactory.Commit();
            }
            catch (Exception ex)
            {
                DaoFactory.Rollback();
                return Json(new ResultJsonModel { isError = true, resultMessage = ex.Message, resultDescription = ex.ToString() });
            }
            return Json(result);
        }

        public JsonResult SelProjectEPartRelation(PmsRelationship _param)
        {
            _param.Type = Common.Constant.PmsConstant.RELATIONSHIP_PROJECT_EPART;
            List<PmsRelationship> result = PmsRelationshipRepository.SelPmsRelationship(Session, _param);
            EPart InfoEPart = null;
            result.ForEach(item => {
                if(InfoEPart != null)
                {
                    InfoEPart = null;
                }
                InfoEPart = EPartRepository.SelEPartObject(Session, new EPart { OID = item.ToOID });
                item.ObjName = InfoEPart.Name;
                item.DocRev = InfoEPart.Revision;
                item.CreateDt = InfoEPart.CreateDt;
                item.CreateUsNm = InfoEPart.CreateUsNm;
            });


            return Json(result);
        }

        public JsonResult DelProjectEPartRelation(int? OID)
        {
            int result = 0;
            try
            {
                DaoFactory.BeginTransaction();
                PmsRelationship _param = new PmsRelationship();
                _param.OID = OID;
                _param.Type = Common.Constant.PmsConstant.RELATIONSHIP_PROJECT_EPART;
                PmsRelationshipRepository.DelPmsRelaionship(Session, _param);
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

        #region -- Project Template

        public ActionResult SearchTemplateProject()
        {
            return View();
        }

        public ActionResult CreateTmpProject()
        {
            return PartialView("Dialog/dlgCreateTmpProject");
        }

        public ActionResult InfoTmpProject(string OID)
        {
            ViewBag.OID = OID;
            ViewBag.GanttUrl = "/Pms/DetailGanttView?OID=" + OID;
            ViewBag.Detail = PmsProjectRepository.SelPmsObject(Session, new PmsProject { Type = PmsConstant.TYPE_PROJECT_TEMP, OID = Convert.ToInt32(OID), IsTemplate = PmsConstant.TYPE_PROJECT_TEMP });
            ViewBag.Holiday = string.Join(",", CalendarDetailRepository.SelCalendarDetails(new CalendarDetail { CalendarOID = Convert.ToInt32(ViewBag.Detail.CalendarOID) }).Select(value => value.FullDate.ToString()).ToArray());
            ViewBag.Status = BPolicyRepository.SelBPolicy(new BPolicy { Type = PmsConstant.TYPE_PROJECT_TEMP });
            ViewBag.Role = BDefineRepository.SelDefines(new BDefine { Type = CommonConstant.DEFINE_ROLE, Module = PmsConstant.MODULE_PMS });
            return View();
        }

        public ActionResult InfoTmpProcess(string ProjectOID, string OID)
        {
            ViewBag.ProjectOID = ProjectOID;
            ViewBag.ProjectDetail = PmsProjectRepository.SelPmsObject(Session, new PmsProject { Type = PmsConstant.TYPE_PROJECT_TEMP, OID = Convert.ToInt32(ProjectOID), IsTemplate = PmsConstant.TYPE_PROJECT_TEMP });
            ViewBag.OID = OID;
            ViewBag.Detail = PmsProcessRepository.SelPmsProcess(Session, new PmsProcess { OID = Convert.ToInt32(OID) });
            ViewBag.Holiday = string.Join(",", CalendarDetailRepository.SelCalendarDetails(new CalendarDetail { CalendarOID = Convert.ToInt32(ViewBag.ProjectDetail.CalendarOID) }).Select(value => value.FullDate.ToString()).ToArray());
            ViewBag.Status = BPolicyRepository.SelBPolicy(new BPolicy { Type = ViewBag.Detail.ProcessType });
            ViewBag.Role = BDefineRepository.SelDefines(new BDefine { Type = CommonConstant.DEFINE_ROLE, Module = PmsConstant.MODULE_PMS });
            return View();
        }

        #endregion

        #region -- Process

        public ActionResult InfoProcess(string ProjectOID, string OID)
        {
            ViewBag.ProjectOID = ProjectOID;
            ViewBag.ProjectDetail = PmsProjectRepository.SelPmsObject(Session, new PmsProject { OID = Convert.ToInt32(ProjectOID) });
            ViewBag.OID = OID;
            ViewBag.Detail = PmsProcessRepository.SelPmsProcess(Session, new PmsProcess { OID = Convert.ToInt32(OID) });
            ViewBag.Holiday = string.Join(",", CalendarDetailRepository.SelCalendarDetails(new CalendarDetail { CalendarOID = Convert.ToInt32(ViewBag.ProjectDetail.CalendarOID) }).Select(value => value.FullDate.ToString()).ToArray());
            ViewBag.Status = BPolicyRepository.SelBPolicy(new BPolicy { Type = ViewBag.Detail.ProcessType });
            ViewBag.Role = BDefineRepository.SelDefines(new BDefine { Type = CommonConstant.DEFINE_ROLE, Module = PmsConstant.MODULE_PMS });
            Approval approv = ApprovalRepository.SelApprovalNonStep(Session, new Approval { TargetOID = Convert.ToInt32(OID) });
            if (approv != null)
            {
                ViewBag.ApprovStatus = DObjectRepository.SelDObject(Session, new DObject { OID = approv.OID }).BPolicy.Name;
            }
            else
            {
                ViewBag.ApprovStatus = null;
            }

            PmsRelationship parentRelationship = PmsRelationshipRepository.SelPmsRelationship(Session, new PmsRelationship { Type = PmsConstant.RELATIONSHIP_WBS, ToOID = Convert.ToInt32(OID) }).First();
            ViewBag.ParentType = DObjectRepository.SelDObject(Session, new DObject { OID = Convert.ToInt32(parentRelationship.FromOID) }).Type;
            return View();
        }

        public JsonResult ApprovProcess(string OID)
        {
            List<PmsRelationship> lPmsProc = PmsRelationshipRepository.GetProcWbsLIst(Session, OID);
            if(lPmsProc.FindAll(item => item.ToOID != Convert.ToInt32(OID) && item.ObjStNm != PmsConstant.POLICY_PROCESS_COMPLETED).Count > 0)
            {
                return Json(new ResultJsonModel { isError = true, resultMessage = "완료되지 않은 Task가 존재합니다.", resultDescription = "완료되지 않은 Task가 존재합니다." });
            }
            return Json(1);
        }

        #endregion

        #region -- Wbs

        public JsonResult InsWbs(List<PmsRelationship> _params)
        {
            DObject dobj = null;
            PmsProject tmpPmsProject = null;
            PmsProcess tmpPmsProcess = null;
            PmsRelationship tmpPmsRelationship = null;
            int iProjectOID = 0;
            int tmpOid = 0, indexOrd = 0;
            try
            {
                DaoFactory.BeginTransaction();
                Dictionary<int, int> newKey = new Dictionary<int, int>();
                List<BDefine> lRoles = BDefineRepository.SelDefines(new BDefine { Type = CommonConstant.DEFINE_ROLE, Module = PmsConstant.MODULE_PMS });
                _params.FindAll(filter => filter.Action == PmsConstant.ACTION_DELETE).ForEach(item =>
                {
                    PmsRelationshipRepository.DelPmsRelaionship(Session, new PmsRelationship { Type = PmsConstant.RELATIONSHIP_WBS, OID = item.OID });
                });

                _params.FindAll(filter => filter.Action != PmsConstant.ACTION_DELETE).ForEach(item =>
                {
                    tmpOid = 0;
                    if (item.Action == null || item.Action.Length < 1)
                    {
                        if (item.ObjType.Equals(PmsConstant.TYPE_PROJECT) || item.ObjType.Equals(PmsConstant.TYPE_PROJECT_TEMP))
                        {
                            if (dobj != null)
                            {
                                dobj = null;
                            }
                            dobj = new DObject();
                            dobj.OID = item.ToOID;
                            dobj.Name = item.ObjName;
                            dobj.Description = item.Description;
                            DObjectRepository.UdtDObject(Session, dobj);

                            iProjectOID = Convert.ToInt32(dobj.OID);

                            if (tmpPmsProject != null)
                            {
                                tmpPmsProject = null;
                            }
                            tmpPmsProject = new PmsProject();
                            tmpPmsProject.OID = dobj.OID;
                            tmpPmsProject.EstDuration = item.EstDuration;
                            tmpPmsProject.EstStartDt = item.EstStartDt;
                            tmpPmsProject.EstEndDt = item.EstEndDt;
                            PmsProjectRepository.UdtPmsProject(Session, tmpPmsProject);
                        }
                        else
                        {
                            if (dobj != null)
                            {
                                dobj = null;
                            }
                            dobj = new DObject();
                            dobj.OID = item.ToOID;
                            dobj.Name = item.ObjName;
                            dobj.Description = item.Description;
                            DObjectRepository.UdtDObject(Session, dobj);

                            if (tmpPmsProcess != null)
                            {
                                tmpPmsProcess = null;
                            }
                            tmpPmsProcess = new PmsProcess();
                            tmpPmsProcess.OID = dobj.OID;
                            tmpPmsProcess.Dependency = item.Dependency;
                            tmpPmsProcess.EstDuration = item.EstDuration;
                            tmpPmsProcess.EstStartDt = item.EstStartDt;
                            tmpPmsProcess.EstEndDt = item.EstEndDt;
                            tmpPmsProcess.No = item.No;
                            PmsProcessRepository.UdtPmsProcess(Session, tmpPmsProcess);

                            if (tmpPmsRelationship != null)
                            {
                                tmpPmsRelationship = null;
                            }
                            tmpPmsRelationship = new PmsRelationship();
                            tmpPmsRelationship.OID = item.OID;
                            tmpPmsRelationship.Ord = indexOrd;
                            PmsRelationshipRepository.UdtPmsRelationship(Session, tmpPmsRelationship);

                            if (item.Members != null && item.Members.Count > 0)
                            {
                                PmsRelationshipRepository.DelPmsRelaionship(Session, new PmsRelationship { FromOID = item.ToOID, Type = PmsConstant.RELATIONSHIP_MEMBER });
                                item.Members.ForEach(member =>
                                {
                                    if (tmpPmsRelationship != null)
                                    {
                                        tmpPmsRelationship = null;
                                    }

                                    tmpPmsRelationship = new PmsRelationship();
                                    tmpPmsRelationship.FromOID = tmpPmsProcess.OID;
                                    tmpPmsRelationship.ToOID = member.ToOID;
                                    tmpPmsRelationship.RootOID = iProjectOID;
                                    tmpPmsRelationship.RoleOID = Convert.ToInt32(lRoles.Find(role => { return role.Name == PmsConstant.ROLE_PE; }).OID);
                                    tmpPmsRelationship.Type = PmsConstant.RELATIONSHIP_MEMBER;
                                    PmsRelationshipRepository.InsPmsRelationship(Session, tmpPmsRelationship);
                                });
                            }
                        }
                    }
                    else
                    {
                        if (item.Action.Equals(PmsConstant.ACTION_NEW))
                        {
                            if (dobj != null)
                            {
                                dobj = null;
                            }
                            dobj = new DObject();
                            dobj.Type = item.ObjType;
                            dobj.Name = item.ObjName;
                            dobj.Description = item.Description;
                            dobj.TableNm = PmsConstant.TABLE_PROCESS;
                            tmpOid = DObjectRepository.InsDObject(Session, dobj);
                            if (tmpOid > 0)
                            {
                                if (tmpPmsProcess != null)
                                {
                                    tmpPmsProcess = null;
                                }
                                tmpPmsProcess = new PmsProcess();
                                tmpPmsProcess.OID = tmpOid;
                                tmpPmsProcess.ProcessType = item.ObjType;
                                tmpPmsProcess.Id = item.Id;
                                tmpPmsProcess.Dependency = item.Dependency;
                                tmpPmsProcess.EstStartDt = item.EstStartDt;
                                tmpPmsProcess.EstEndDt = item.EstEndDt;
                                tmpPmsProcess.EstDuration = item.EstDuration;
                                tmpPmsProcess.Level = item.Level;
                                tmpPmsProcess.Complete = item.Complete;
                                tmpPmsProcess.No = item.No;
                                PmsProcessRepository.InsPmsProcess(Session, tmpPmsProcess);
                                newKey.Add(Convert.ToInt32(item.ToOID), tmpOid);

                                if (tmpPmsRelationship != null)
                                {
                                    tmpPmsRelationship = null;
                                }
                                tmpPmsRelationship = new PmsRelationship();
                                tmpPmsRelationship.Type = PmsConstant.RELATIONSHIP_WBS;
                                if (newKey.ContainsKey(Convert.ToInt32(item.FromOID)))
                                {
                                    tmpPmsRelationship.FromOID = newKey[Convert.ToInt32(item.FromOID)];
                                }
                                else
                                {
                                    tmpPmsRelationship.FromOID = Convert.ToInt32(item.FromOID);
                                }
                                tmpPmsRelationship.ToOID = tmpOid;
                                tmpPmsRelationship.Ord = indexOrd;
                                tmpPmsRelationship.RootOID = item.RootOID;
                                int tmpResult = PmsRelationshipRepository.InsPmsRelationship(Session, tmpPmsRelationship);
                                if (tmpResult < 0)
                                {
                                    throw new Exception("중복된 데이터가 존재합니다.");
                                }

                                if (tmpPmsRelationship != null)
                                {
                                    tmpPmsRelationship = null;
                                }
                                tmpPmsRelationship = new PmsRelationship();
                                tmpPmsRelationship.OID = item.OID;
                                tmpPmsRelationship.Ord = indexOrd;
                                PmsRelationshipRepository.UdtPmsRelationship(Session, tmpPmsRelationship);

                                if (item.Members != null && item.Members.Count > 0)
                                {
                                    item.Members.ForEach(member =>
                                    {
                                        if (tmpPmsRelationship != null)
                                        {
                                            tmpPmsRelationship = null;
                                        }

                                        tmpPmsRelationship = new PmsRelationship();
                                        tmpPmsRelationship.FromOID = tmpPmsProcess.OID;
                                        tmpPmsRelationship.ToOID = member.ToOID;
                                        tmpPmsRelationship.RootOID = iProjectOID;
                                        tmpPmsRelationship.RoleOID = Convert.ToInt32(lRoles.Find(role => { return role.Name == PmsConstant.ROLE_PE; }).OID);
                                        tmpPmsRelationship.Type = PmsConstant.RELATIONSHIP_MEMBER;
                                        PmsRelationshipRepository.InsPmsRelationship(Session, tmpPmsRelationship);
                                    });
                                }
                            }
                        }
                    }
                    indexOrd++;
                });
                DaoFactory.Commit();
            }
            catch (Exception ex)
            {
                DaoFactory.Rollback();
                return Json(new ResultJsonModel { isError = true, resultMessage = ex.Message, resultDescription = ex.ToString() });
            }

            return Json(0);
        }

        public JsonResult SelWbs(PmsProject _param)
        {
            int Level = 0;
            PmsProject proj = PmsProjectRepository.SelPmsObject(Session, _param);
            return Json(PmsRelationshipRepository.GetProjWbsStructure(Session, Level, Convert.ToInt32(proj.OID), proj));
        }

        public JsonResult SelProcessWbs(PmsProcess _param)
        {
            int Level = 0;
            PmsProcess proc = PmsProcessRepository.SelPmsProcess(Session, new PmsProcess { OID = _param.OID });
            proc.RootOID = _param.RootOID;
            return Json(PmsRelationshipRepository.GetProcWbsStructure(Session, Level, proc));
        }

        #endregion

        #region -- Member

        public bool CheckMembers(PmsRelationship param)
        {
            bool bResult = true;
            List<PmsRelationship> lPmsMembers = PmsRelationshipRepository.SelPmsRelationship(Session, new PmsRelationship { Type = PmsConstant.RELATIONSHIP_MEMBER, RootOID = param.RootOID });
            List<PmsRelationship> tmpPmsMembers = lPmsMembers.FindAll(item => { return item.FromOID != param.RootOID; });
            tmpPmsMembers.ForEach(item =>
            {
                if (item.Action == null)
                {
                    if (item.ToOID == param.ToOID && item.RoleOID != param.RoleOID)
                    {
                        bResult = false;
                    }
                }
                else
                {
                    if (item.ToOID == param.ToOID)
                    {
                        bResult = false;
                    }
                }
            });
            return bResult;
        }

        public JsonResult SelMembers(PmsRelationship _param)
        {
            _param.Type = PmsConstant.RELATIONSHIP_MEMBER;
            List<PmsRelationship> lMember = PmsRelationshipRepository.SelPmsRelationship(Session, _param);
            Person tmpPerson = null;
            lMember.ForEach(member =>
            {
                member.RoleOIDNm = BDefineRepository.SelDefine(new BDefine { OID = member.RoleOID }).Description;
                if (tmpPerson != null)
                {
                    tmpPerson = null;
                }
                tmpPerson = PersonRepository.SelPerson(Session, new Person { OID = member.ToOID });
                member.PersonNm = tmpPerson.Name;
                member.DepartmentNm = tmpPerson.DepartmentNm;
                member.Thumbnail = tmpPerson.Thumbnail;
            });
            return Json(lMember);
        }

        public JsonResult InsertMembers(List<PmsRelationship> _params)
        {
            PmsRelationship pmsRelationship = null;
            try
            {
                int indexOrd = 0;
                DaoFactory.BeginTransaction();
                List<PmsRelationship> delParams = _params.FindAll(item => { return item.Action != null && item.Action == PmsConstant.ACTION_DELETE; });
                delParams.ForEach(item =>
                {
                    if (!CheckMembers(item))
                    {
                        throw new Exception("Task의 담당자로 지정되어 삭제할 수 없습니다.");
                    }

                    item.Type = PmsConstant.RELATIONSHIP_MEMBER;
                    PmsRelationshipRepository.DelPmsRelaionship(Session, item);
                });

                _params.ForEach(param =>
                {
                    if (param.Action == null || param.Action.Length < 0)
                    {
                        if (param.OID != null && param.OID > 0)
                        {
                            if (!CheckMembers(param))
                            {
                                throw new Exception(param.PersonNm + " Task의 담당자로 지정되어 변경할 수 없습니다.");
                            }

                            if (pmsRelationship != null)
                            {
                                pmsRelationship = null;
                            }
                            pmsRelationship = new PmsRelationship();
                            pmsRelationship.Type = PmsConstant.RELATIONSHIP_MEMBER;
                            pmsRelationship.RoleOID = param.RoleOID;
                            pmsRelationship.OID = param.OID;
                            PmsRelationshipRepository.UdtPmsRelationship(Session, pmsRelationship);
                        }
                    }
                    else
                    {
                        if (param.OID != null && param.OID > 0) { }
                        else
                        {
                            if (param.Action == PmsConstant.ACTION_ADD)
                            {
                                if (pmsRelationship != null)
                                {
                                    pmsRelationship = null;
                                }
                                pmsRelationship = new PmsRelationship();
                                pmsRelationship.RootOID = param.RootOID;
                                pmsRelationship.FromOID = param.FromOID;
                                pmsRelationship.ToOID = param.ToOID;
                                pmsRelationship.Type = PmsConstant.RELATIONSHIP_MEMBER;
                                pmsRelationship.RoleOID = param.RoleOID;
                                pmsRelationship.Ord = indexOrd;
                                pmsRelationship.Description = param.Description;
                                PmsRelationshipRepository.InsPmsRelationship(Session, pmsRelationship);
                            }
                        }
                    }
                    indexOrd++;
                });

                DaoFactory.Commit();
            }
            catch (Exception ex)
            {
                DaoFactory.Rollback();
                return Json(new ResultJsonModel { isError = true, resultMessage = ex.Message, resultDescription = ex.ToString() });
            }
            return Json(1);
        }

        public PartialViewResult AddModifyMemeber(string ProjectOID, string ProcessOID)
        {
            ViewBag.ProjectOID = ProjectOID;
            ViewBag.ProcessOID = ProcessOID;
            ViewBag.Role = BDefineRepository.SelDefines(new BDefine { Type = CommonConstant.DEFINE_ROLE, Module = PmsConstant.MODULE_PMS });
            return PartialView("Dialog/dlgAddModifyMemberInProcess");
        }

        #endregion

        #region -- GateView

        public PartialViewResult DetailGateView(string ProjectOID)
        {
            ViewBag.ProjectOID = ProjectOID;
            List<PmsProcess> lGate = new List<PmsProcess>();
            PmsRelationshipRepository.SelPmsRelationship(Session, new PmsRelationship { Type = PmsConstant.RELATIONSHIP_WBS, FromOID = Convert.ToInt32(ProjectOID) }).ForEach(item =>
            {
                PmsProcess tmpProcess = PmsProcessRepository.SelPmsProcess(Session, new PmsProcess { OID = item.ToOID });

                if (tmpProcess.Type.Equals(PmsConstant.TYPE_GATE))
                {
                    Approval approv = ApprovalRepository.SelApprovalNonStep(Session, new Approval { TargetOID = Convert.ToInt32(tmpProcess.OID) });
                    if (approv != null)
                    {
                        tmpProcess.ApprovStatus = DObjectRepository.SelDObject(Session, new DObject { OID = approv.OID }).BPolicy.Name;
                    }
                    lGate.Add(tmpProcess);
                }
            });
            ViewBag.lGateView = lGate;
            return PartialView("Partitial/ptDetailGateView");
        }

        public ActionResult GateSignOffReport(string ProjectOID, string ProcessOID)
        {
            PmsProject proj = PmsProjectRepository.SelPmsObject(Session, new PmsProject { OID = Convert.ToInt32(ProjectOID) });
            PmsProcess proc = PmsProcessRepository.SelPmsProcess(Session, new PmsProcess { OID = Convert.ToInt32(ProcessOID) });
            Approval approv = ApprovalRepository.SelApprovalNonStep(Session, new Approval { TargetOID = Convert.ToInt32(ProcessOID) });
            if (approv != null)
            {
                ViewBag.ApprovStatus = DObjectRepository.SelDObject(Session, new DObject { OID = approv.OID }).BPolicy.Name;
            }
            else
            {
                ViewBag.ApprovStatus = null;
            }
            List<CustomerSchedule> customerSchedule = CustomerScheduleRepository.SelProjMngtCustomerSchedule(new CustomerSchedule { Car_Lib_OID = proj.Car_Lib_OID });
            PmsGateSignOff signOff = PmsGateSignOffRepository.SelPmsGateSignOff(Session, new PmsGateSignOff { OID = Convert.ToInt32(ProcessOID) });
            if(signOff == null)
            {
                signOff = new PmsGateSignOff();
            }
            List<PmsRelationship> lGettingGate = new List<PmsRelationship>();
            List<PmsRelationship> lProjRelationship = PmsRelationshipRepository.GetProjWbsLIst(Session, ProjectOID);
            List<PmsRelationship> lGateRelationship = lProjRelationship.FindAll(rel => rel.ObjType == PmsConstant.TYPE_GATE);
            int gatePosition = lGateRelationship.FindIndex(gate => gate.ToOID == Convert.ToInt32(ProcessOID));
            for (int index = 0, size = gatePosition; index <= size; index++)
            {
                lGettingGate.Add(lGateRelationship[index]);
            }

            List<PmsIssue> lPmsIssue = new List<PmsIssue>();
            List<PmsRelationship> Relation = new List<PmsRelationship>();
            Relation = PmsRelationshipRepository.SelPmsRelationship(Session, new PmsRelationship { RootOID = Convert.ToInt32(ProjectOID), Type = PmsConstant.RELATIONSHIP_WBS });
            int _procIdx = Convert.ToInt32(Relation.Find(val => val.ToOID == Convert.ToInt32(ProcessOID)).Ord);
            List<int> lProcessOID = new List<int>();
            PmsProcess tmpProcess = new PmsProcess();
            bool bGate = false;
            Relation.ForEach(item =>
            {
                if (tmpProcess != null)
                {
                    tmpProcess = null;
                }
                tmpProcess = PmsProcessRepository.SelPmsProcess(Session, new PmsProcess { OID = item.ToOID });
                item.Type = tmpProcess.Type;
                if (bGate)
                {
                    return;
                }

                if (item.Type.Equals(PmsConstant.TYPE_GATE))
                {
                    if (item.Ord == _procIdx)
                    {
                        bGate = true;
                    }
                    else
                    {
                        lProcessOID.Clear();
                    }
                }
                else
                {
                    lProcessOID.Add(Convert.ToInt32(item.ToOID));
                }
            });
            Relation = null;
            int TotIssueCnt = 0;
            int CompIssueCnt = 0;
            int NonCompIssueCnt = 0;
            PmsIssue Issue = new PmsIssue();
            lProcessOID.ForEach(item =>
            {
                Relation = PmsRelationshipRepository.SelPmsRelationship(Session, new PmsRelationship { RootOID = Convert.ToInt32(ProjectOID), FromOID = item, Type = PmsConstant.RELATIONSHIP_ISSUE });
                if (Relation != null)
                {
                    
                    Relation.ForEach(data =>
                    {
                        TotIssueCnt++;
                        if (Issue!= null)
                        {
                            Issue = null;
                        }
                        Issue = PmsIssueRepository.SelIssue(Session, new PmsIssue { OID = data.ToOID });
                        if(Issue.BPolicy.Name == PmsConstant.POLICY_ISSUE_PROJECT_COMPLETED)
                        {
                            CompIssueCnt++;
                        }
                        else
                        {
                            NonCompIssueCnt++;
                        }
                        if (data.RootOID != data.FromOID)
                        {
                            Issue.TaskNm = PmsProcessRepository.SelPmsProcess(Session, new PmsProcess { OID = Convert.ToInt32(data.FromOID) }).Name;
                        }
                        Issue.ProjectNm = PmsProcessRepository.SelPmsProcess(Session, new PmsProcess { OID = Convert.ToInt32(data.RootOID) }).Name;
                        Issue.CreateUsNm = PersonRepository.SelPerson(Session, new Person { OID = data.CreateUs }).Name;

                        lPmsIssue.Add(Issue);
                    });
                }
            });
            ViewBag.signOff = signOff;
            ViewBag.TotIssueCnt = TotIssueCnt;
            ViewBag.CompIssueCnt = CompIssueCnt;
            ViewBag.NonCompIssueCnt = NonCompIssueCnt;
            ViewBag.InfoIssue = lPmsIssue;
            ViewBag.InfoProj = proj;
            ViewBag.Holiday = string.Join(",", CalendarDetailRepository.SelCalendarDetails(new CalendarDetail { CalendarOID = Convert.ToInt32(proj.CalendarOID) }).Select(value => value.FullDate.ToString()).ToArray());
            ViewBag.InfoProc = proc;
            ViewBag.OemSchedule = customerSchedule;
            ViewBag.InfoGate = lGettingGate;
            return PartialView("Dialog/dlgGateSignOffReport");
        }
        public JsonResult InsGateSignOff(PmsGateSignOff param)
        {
            int result = 0;
            try
            {
                DaoFactory.BeginTransaction();
                PmsGateSignOff checkData = PmsGateSignOffRepository.SelPmsGateSignOff(Session,new PmsGateSignOff{OID =param.OID });
                if(checkData != null)
                {
                    param.ModifyUs = Convert.ToInt32(Session["UserOID"]);
                    PmsGateSignOffRepository.UdtChangeOrderObject(param);
                }
                else
                {
                    param.CreateUs = Convert.ToInt32(Session["UserOID"]);
                    DaoFactory.SetInsert("Pms.InsPmsGateSignOff", param);
                }
                DaoFactory.Commit();
            }
            catch (Exception ex)
            {
                DaoFactory.Rollback();
                return Json(new ResultJsonModel { isError = true, resultMessage = ex.Message, resultDescription = ex.ToString() });
            }
            return Json(result);
        }

        public PartialViewResult DetailGateViewMeeting(string ProjectOID, string ProcessOID)
        {
            ViewBag.ProjectOID = ProjectOID;
            ViewBag.ProcessOID = ProcessOID;
            ViewBag.lGateViewDetail = PmsProcessRepository.SelPmsProcess(Session, new PmsProcess { OID = Convert.ToInt32(ProcessOID) });
            return PartialView("Dialog/dlgDetailGateViewMetting");
        }

        public PartialViewResult DetailGateViewMettingContent(string ProjectOID, string ProcessOID, string MettingOID)
        {
            ViewBag.ProjectOID = ProjectOID;
            ViewBag.ProcessOID = ProcessOID;

            PmsProcess pmsProcessObj = PmsProcessRepository.SelPmsProcess(Session, new PmsProcess { OID = Convert.ToInt32(ProcessOID) });

            ViewBag.ProcessDetail = pmsProcessObj;
            PmsGateMetting detail = null;
            if (MettingOID != null && MettingOID.Length > 0)
            {
                detail = PmsGateMettingRepository.SelPmsGateMettingObject(Session, new PmsGateMetting { OID = Convert.ToInt32(MettingOID) });
                detail.CreateUsNm = PersonRepository.SelPerson(Session, new Person { OID = detail.CreateUs }).Name;
                
            }
            Approval approv = ApprovalRepository.SelApprovalNonStep(Session, new Approval { TargetOID = Convert.ToInt32(ProcessOID) });
            if (approv != null)
            {
                ViewBag.ApprovStatus = DObjectRepository.SelDObject(Session, new DObject { OID = approv.OID }).BPolicy.Name;
            }
            else
            {
                ViewBag.ApprovStatus = null;
            }
            ViewBag.MettingDetail = detail;

            return PartialView("Dialog/dlgDetailGateViewMettingContent");
        }

        public JsonResult InsPmsGateMetting(PmsGateMetting param)
        {
            int resultOID = 0;
            List<HttpFile> List = new List<HttpFile>();
            try
            {
                DaoFactory.BeginTransaction();

                List<PmsGateMetting> num = PmsGateMettingRepository.SelPmsGateMetting(Session, new PmsGateMetting { OID = null });

                List<HttpFile> File = HttpFileRepository.SelFiles(Session, new HttpFile { OID = param.MettingOID });

                param.Type = PmsConstant.TYPE_GATEVIEW_METTING;
                param.Name = Convert.ToString(num.Count + 1);

                resultOID = DObjectRepository.InsDObject(Session, param);

                if (param.delFiles != null)
                {
                    param.delFiles.ForEach(v =>
                    {
                        File.ForEach(value => {
                            if (value.OID == v.OID)
                            {
                                List.Add(value);
                            }
                        });
                    });
                }

                if(File != null)
                {
                    File.RemoveAll(List.Contains);

                    File.ForEach(value => {
                        value.OID = Convert.ToInt32(resultOID);
                        value.FileOID = null;
                        value.CreateUs = Convert.ToInt32(Session["UserOID"]);
                        DaoFactory.SetInsert("Comm.InsFile", value);
                    });
                }
                
                if (param.Files != null)
                {
                    HttpFileRepository.InsertData(Session, param);
                }

                PmsRelationship Data = new PmsRelationship();

                Data.Type = PmsConstant.RELATIONSHIP_GATEVIEW_METTING;
                Data.RootOID = param.RootOID; //프로젝트 OID
                Data.FromOID = param.FromOID; //프로젝트 OID 또는 템플릿 OID 
                Data.ToOID = resultOID;

                PmsRelationshipRepository.InsPmsRelationship(Session, Data);

                DaoFactory.Commit();
            }
            catch (Exception ex)
            {
                DaoFactory.Rollback();
                return Json(new ResultJsonModel { isError = true, resultMessage = ex.Message, resultDescription = ex.ToString() });
            }
            return Json(resultOID);
        }
        public JsonResult SelPmsGateMettingList(PmsRelationship param)
        {
            List<PmsRelationship> PmsGateMettingList = PmsRelationshipRepository.SelPmsRelationship(Session, param);
            PmsGateMettingList.ForEach(item => {
                item.ObjDescription = PmsGateMettingRepository.SelPmsGateMettingObject(Session, new PmsGateMetting { OID = item.ToOID }).Description;
                item.CreateUsNm = PersonRepository.SelPerson(Session, new Person { OID = item.CreateUs }).Name;
            });
            PmsGateMettingList = PmsGateMettingList.OrderByDescending(x => x.CreateDt).ToList();

            return Json(PmsGateMettingList);
        }

        public PartialViewResult DetailGateViewDeliveres(string ProjectOID, string ProcessOID)
        {
            ViewBag.ProjectOID = ProjectOID;
            ViewBag.ProcessOID = ProcessOID;
            ViewBag.lGateViewDetail = PmsProcessRepository.SelPmsProcess(Session, new PmsProcess { OID = Convert.ToInt32(ProcessOID) });
            return PartialView("Dialog/dlgDetailGateViewDeliveres");
        }
        public JsonResult SelectGateDeliveries(string ProjectOID, string ProcessOID)
        {
            List<DocClass> lPmsDocument = new List<DocClass>();
            List<PmsRelationship> Relation = new List<PmsRelationship>();
            List<PmsRelationship> ReturnRelation = new List<PmsRelationship>();
            Relation = PmsRelationshipRepository.SelPmsRelationship(Session,new PmsRelationship { RootOID = Convert.ToInt32(ProjectOID), Type = PmsConstant.RELATIONSHIP_WBS });
            int _procIdx = Convert.ToInt32(Relation.Find(val => val.ToOID == Convert.ToInt32(ProcessOID)).Ord);
            List<int> lProcessOID = new List<int>();
            PmsProcess tmpProcess = new PmsProcess();
            bool bGate = false;
            Relation.ForEach(item =>
            {
                if(tmpProcess != null)
                {
                    tmpProcess = null;
                }
                 tmpProcess = PmsProcessRepository.SelPmsProcess(Session, new PmsProcess { OID = item.ToOID });
                item.Type = tmpProcess.Type;
                if (bGate)
                {
                    return;
                }

                if (item.Type.Equals(PmsConstant.TYPE_GATE))
                {
                    if (item.Ord == _procIdx)
                    {
                        bGate = true;
                    }
                    else
                    {
                        lProcessOID.Clear();
                    }
                }
                else
                {
                    lProcessOID.Add(Convert.ToInt32(item.ToOID));
                }
            });
            Relation = null;
            
            DocClass DocClass = new DocClass();
            Doc childDoc = new Doc();

            lProcessOID.ForEach(item =>
            {
                Relation = PmsRelationshipRepository.SelPmsRelationship(Session, new PmsRelationship {RootOID= Convert.ToInt32(ProjectOID), FromOID =item, Type = PmsConstant.RELATIONSHIP_DOC_MASTER });
                if (Relation != null)
                {
                    Relation.ForEach(data =>
                    {
                        DocClass = DocClassRepository.SelDocClassObject(Session, new DocClass { OID = data.ToOID });
                        if(data.RootOID != data.FromOID)
                        {
                            data.TaskNm = PmsProcessRepository.SelPmsProcess(Session, new PmsProcess { OID = Convert.ToInt32(data.FromOID) }).Name;
                        }
                        data.ProjectNm = PmsProcessRepository.SelPmsProcess(Session, new PmsProcess { OID = Convert.ToInt32(data.RootOID) }).Name;
                        data.DocClassNm = DocClass.Name;
                        data.ViewUrl = DocClass.ViewUrl;
                        data.CreateUsNm = PersonRepository.SelPerson(Session, new Person { OID = data.CreateUs }).Name;
                        data.Children = PmsRelationshipRepository.SelPmsRelationship(Session, new PmsRelationship { FromOID = data.ToOID, Type = PmsConstant.RELATIONSHIP_DOC_CLASS, TaskOID = data.FromOID });
                        if (data.Children != null)
                        {
                            data.Children.ForEach(cdata =>
                            {
                                if(data.ViewUrl == null)
                                {
                                    childDoc = DocRepository.SelDocObject(Session, new Doc { OID = cdata.ToOID });
                                    if (cdata.RootOID != cdata.FromOID)
                                    {
                                        cdata.TaskNm = data.TaskNm;
                                    }
                                    cdata.ProjectNm = data.ProjectNm;
                                    cdata.CreateUsNm = childDoc.CreateUsNm;
                                    cdata.DocNm = childDoc.Title;
                                    cdata.DocRev = childDoc.Revision;
                                    cdata.DocStNm = childDoc.BPolicy.StatusNm;
                                }
                                else
                                {
                                    PmsReliability Reliability = PmsReliabilityRepository.SelPmsReliabilityObject(Session, new PmsReliability { OID = cdata.ToOID });
                                    if (cdata.RootOID != cdata.FromOID)
                                    {
                                        cdata.TaskNm = data.TaskNm;
                                    }
                                    cdata.ProjectNm = data.ProjectNm;
                                    cdata.DocNm = Reliability.Name;
                                    cdata.DocRev = Reliability.Revision;
                                    cdata.DocStNm = Reliability.BPolicy.StatusNm;
                                    cdata.CreateDt = Reliability.CreateDt;
                                    cdata.CreateUsNm = Reliability.CreateUsNm;
                                    cdata.ViewUrl = data.ViewUrl;
                                }
                            });
                            
                        }
                        ReturnRelation.Add(data);
                    });              
                }
               
            });
            return Json(ReturnRelation);
        }

        public ActionResult DetailGateCheckList(string ProjectOID, string ProcessOID)
        {
            ViewBag.ProjectOID = ProjectOID;
            ViewBag.ProcessOID = ProcessOID;
            ViewBag.lGateViewDetail = PmsProcessRepository.SelPmsProcess(Session, new PmsProcess { OID = Convert.ToInt32(ProcessOID) });
            Approval approv = ApprovalRepository.SelApprovalNonStep(Session, new Approval { TargetOID = Convert.ToInt32(ProcessOID) });
            if (approv != null)
            {
                ViewBag.ApprovStatus = DObjectRepository.SelDObject(Session, new DObject { OID = approv.OID }).BPolicy.Name;
            }
            else
            {
                ViewBag.ApprovStatus = null;
            }
            return PartialView("Dialog/dlgDetailGateCheckList");
        }

        public JsonResult SelGateCheckList(string ProjectOID, string ProcessOID)
        {
            List<PmsRelationship> lProjRelationship = PmsRelationshipRepository.GetProjWbsLIst(Session, ProjectOID);
            List<PmsRelationship> lGateRelationship = lProjRelationship.FindAll(rel => rel.ObjType == PmsConstant.TYPE_GATE);
            int gatePosition = lGateRelationship.FindIndex(gate => gate.ToOID == Convert.ToInt32(ProcessOID));
            List<int> betweenGate = new List<int>();
            if (gatePosition > 0)
            {
                betweenGate.Add(Convert.ToInt32(lGateRelationship[gatePosition-1].ToOID));
            }
            betweenGate.Add(Convert.ToInt32(lGateRelationship[gatePosition].ToOID));


            bool bAdd = false;
            if (betweenGate.Count == 1)
            {
                bAdd = true;
            }
            List<PmsRelationship> gettingWbs = new List<PmsRelationship>();
            List<PmsRelationship> Relation = new List<PmsRelationship>();
            lProjRelationship.ForEach(wbs =>
            {
                if (wbs.ObjType == PmsConstant.TYPE_TASK || wbs.ObjType == PmsConstant.TYPE_GATE) {
                    if (betweenGate.Count == 1)
                    {
                        if (bAdd)
                        {
                            gettingWbs.Add(wbs);
                            Relation =PmsRelationshipRepository.SelPmsRelationship(Session, new PmsRelationship { RootOID = wbs.RootOID,FromOID =wbs.ToOID, Type = PmsConstant.RELATIONSHIP_DOC_MASTER });
                            Relation.ForEach(cdata => {
                                cdata.Children = PmsRelationshipRepository.SelPmsRelationship(Session, new PmsRelationship { FromOID = cdata.ToOID, Type = PmsConstant.RELATIONSHIP_DOC_CLASS, TaskOID = cdata.FromOID });
                                if(cdata.Children.Count > 0)
                                {
                                    cdata.Count = cdata.Children.Count;
                                }
                                else
                                {
                                    cdata.Count = 0;
                                }
                                cdata.ObjType = cdata.Type;
                                cdata.ObjName = DocClassRepository.SelDocClassObject(Session, new DocClass { OID = cdata.ToOID }).Name;
                                gettingWbs.Add(cdata);
                            });

                        }

                        if (wbs.ToOID == betweenGate[0])
                        {
                            bAdd = false;
                        }
                    }
                    else
                    {
                        if (bAdd)
                        {
                            gettingWbs.Add(wbs);
                            Relation = PmsRelationshipRepository.SelPmsRelationship(Session, new PmsRelationship { RootOID = wbs.RootOID, FromOID = wbs.ToOID, Type = PmsConstant.RELATIONSHIP_DOC_MASTER, TaskOID = wbs.ToOID });
                            Relation.ForEach(cdata => {
                                cdata.Children = PmsRelationshipRepository.SelPmsRelationship(Session, new PmsRelationship { FromOID = cdata.ToOID, Type = PmsConstant.RELATIONSHIP_DOC_CLASS, TaskOID = cdata.FromOID });
                                if (cdata.Children.Count > 0)
                                {
                                    cdata.Count = cdata.Children.Count;
                                }
                                else
                                {
                                    cdata.Count = 0;
                                }
                                cdata.ObjType = cdata.Type;
                                cdata.ObjName = DocClassRepository.SelDocClassObject(Session, new DocClass { OID = cdata.ToOID }).Name;
                                gettingWbs.Add(cdata);
                            });
                        }

                        if (wbs.ToOID == betweenGate[0])
                        {
                            bAdd = true;
                        }
                        else if (wbs.ToOID == betweenGate[1])
                        {
                            bAdd = false;
                        }
                    }
                }
            });
            return Json(gettingWbs);
        }

        public JsonResult UdtGateCheckListEtc(List<PmsRelationship> _param)
        {
            int result = 0;
            try
            {
                DaoFactory.BeginTransaction();
                if (_param.Count > 0)
                {
                    _param.ForEach(data =>
                    {
                        PmsRelationshipRepository.UdtPmsRelationship(Session, data);
                    });
                }        
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

        #region -- BaseLine

        public ActionResult DetailBaseLineProject(string ProjectOID, string ProjectBaseLineOID)
        {
            ViewBag.ProjectOID = ProjectOID;
            ViewBag.ProjectBaseLineOID = ProjectBaseLineOID;
            return PartialView("Dialog/dlgDetailBaseLineProject");
        }

        public JsonResult SelectBaseLineWbs(string ProjectOID, string ProjectBaseLineOID)
        {
            PmsBaseLineProject pmsBaseLineProject = PmsBaseLineProjectRepository.SelPmsBaseLIneProject(new PmsBaseLineProject { OID = Convert.ToInt32(ProjectBaseLineOID) });
            int FromOID = Convert.ToInt32(ProjectOID);
            int RootBaseLineOID = Convert.ToInt32(ProjectBaseLineOID);
            return Json(PmsBaseLineRelationshipRepository.getListBaseLineWbsStructure(0, FromOID, RootBaseLineOID, pmsBaseLineProject));
        }

        public JsonResult SelBaseLineProject(string ProjectOID)
        {
            List<PmsBaseLineProject> lPmsBaseLineProject = new List<PmsBaseLineProject>();
            List<PmsRelationship> lPmsRelaionship = PmsRelationshipRepository.SelPmsRelationship(Session, new PmsRelationship { Type = PmsConstant.RELATIONSHIP_BASELINE, FromOID = Convert.ToInt32(ProjectOID) });
            lPmsRelaionship.ForEach(rel =>
            {
                lPmsBaseLineProject.Add(PmsBaseLineProjectRepository.SelPmsBaseLIneProject(new PmsBaseLineProject { OID = rel.ToOID }));
            });
            return Json(lPmsBaseLineProject);
        }

        public JsonResult InsBaseLineProject(string ProjectOID)
        {
            int resultOID = 0;
            string baseLineNmPrefix = DateTime.Now.ToString("yyyyMMdd") + "-";

            DObject dobj = null;
            PmsRelationship relDobj = null;
            try
            {
                DaoFactory.BeginTransaction();
                dobj = new DObject();
                dobj.Type = PmsConstant.TYPE_BASE_LINE_PROJECT;
                dobj.Name = baseLineNmPrefix + SemsUtil.MakeSeq(DObjectRepository.SelNameSeq(Session, new DObject { Type = PmsConstant.TYPE_BASE_LINE_PROJECT, Name = baseLineNmPrefix + "%" }), "000");
                dobj.TableNm = PmsConstant.TABLE_PROJECT;
                resultOID = DObjectRepository.InsDObject(Session, dobj);
                PmsBaseLineProjectRepository.InsPmsBaseLineProject(Session, new PmsBaseLineProject { TargetProjectOID = Convert.ToInt32(ProjectOID), ProjectBaseLineOID = resultOID });

                relDobj = new PmsRelationship();
                relDobj.Type = PmsConstant.RELATIONSHIP_BASELINE;
                relDobj.FromOID = Convert.ToInt32(ProjectOID);
                relDobj.ToOID = resultOID;
                relDobj.RootOID = Convert.ToInt32(ProjectOID);
                PmsRelationshipRepository.InsPmsRelationship(Session, relDobj);


                List<PmsRelationship> lWbs = PmsRelationshipRepository.SelPmsRelationship(Session, new PmsRelationship { Type = PmsConstant.RELATIONSHIP_WBS, RootOID = Convert.ToInt32(ProjectOID) });
                List<int> lProcessOID = new List<int>();
                lWbs.ForEach(wbs =>
                {
                    if (wbs.FromOID != Convert.ToInt32(ProjectOID) && !lProcessOID.Contains(Convert.ToInt32(wbs.FromOID)))
                    {
                        lProcessOID.Add(Convert.ToInt32(wbs.FromOID));
                    }

                    if (!lProcessOID.Contains(Convert.ToInt32(wbs.ToOID)))
                    {
                        lProcessOID.Add(Convert.ToInt32(wbs.ToOID));
                    }
                    PmsBaseLineRelationshipRepository.InsPmsBaseLineRelationship(new PmsBaseLineRelationship { RootBaseLineOID = resultOID, BaseData = wbs });
                });

                List<PmsRelationship> lPmsMemberRelationship = null;
                lProcessOID.ForEach(value =>
                {
                    PmsBaseLineProcessRepository.InsPmsBaseLineProcess(new PmsBaseLineProcess { RootBaseLineOID = resultOID, TargetPrcessOID = value });

                    if (lPmsMemberRelationship != null)
                    {
                        lPmsMemberRelationship = null;
                    }
                    lPmsMemberRelationship = PmsRelationshipRepository.SelPmsRelationship(Session, new PmsRelationship { Type = PmsConstant.RELATIONSHIP_MEMBER, FromOID = value });
                    lPmsMemberRelationship.ForEach(member =>
                    {
                        PmsBaseLineRelationshipRepository.InsPmsBaseLineRelationship(new PmsBaseLineRelationship { RootBaseLineOID = resultOID, BaseData = member });
                    });
                });

                DaoFactory.Commit();
            }
            catch (Exception ex)
            {
                DaoFactory.Rollback();
                return Json(new ResultJsonModel { isError = true, resultMessage = ex.Message, resultDescription = ex.ToString() });
            }
            finally
            {
                dobj = null;
                relDobj = null;
            }
            return Json(resultOID);
        }

        public PartialViewResult CompareBaseLIneProject(string ProjectOID, string ProjectBaseLineOID)
        {
            ViewBag.ProjectOID = ProjectOID;
            ViewBag.ProjectBaseLineOID = ProjectBaseLineOID;
            return PartialView("Dialog/dlgCompareBaseLineProject");
        }

        public JsonResult SelectCompareWbs(string ProjectOID, string ProjectBaseLineOID)
        {
            int level = 0;
            PmsProject LProject = PmsProjectRepository.SelPmsObject(Session, new PmsProject { OID = Convert.ToInt32(ProjectOID) });
            PmsBaseLineProject RProject = PmsBaseLineProjectRepository.SelPmsBaseLIneProject(new PmsBaseLineProject { OID = Convert.ToInt32(ProjectBaseLineOID) });
            int FromOID = Convert.ToInt32(ProjectOID);
            int RootBaseLineOID = Convert.ToInt32(ProjectBaseLineOID);

            List<IPmsRelationship> CompareRelation = new List<IPmsRelationship>();
            List<PmsRelationship> lProjectRelationship = PmsRelationshipRepository.SelPmsRelationship(Session, new PmsRelationship { RootOID = FromOID, Type = PmsConstant.RELATIONSHIP_WBS });
            List<PmsBaseLineRelationship> lProjectBaseLineRelationship = PmsBaseLineRelationshipRepository.SelPmsBaseLineRelationship(new PmsBaseLineRelationship { RootOID = FromOID, RootBaseLineOID = RootBaseLineOID, Type = PmsConstant.RELATIONSHIP_WBS });

            lProjectRelationship.ForEach(item =>
            {
                int compareIndex = CompareRelation.FindIndex(innerItem => { return ((PmsRelationship)innerItem).FromOID == item.FromOID && ((PmsRelationship)innerItem).ToOID == item.ToOID; });
                if (compareIndex < 0)
                {
                    int baseLineIndex = lProjectBaseLineRelationship.FindIndex(innerItem =>
                    {
                        return ((PmsBaseLineRelationship)innerItem).FromOID == item.FromOID && ((PmsBaseLineRelationship)innerItem).ToOID == item.ToOID;
                    });
                    if (baseLineIndex > -1)
                    {
                        item.Action = PmsConstant.ACTION_ADD;
                        item.BaseLineOrd = lProjectBaseLineRelationship[baseLineIndex].Ord;
                        CompareRelation.Add(item);
                        lProjectBaseLineRelationship.RemoveAt(baseLineIndex);
                    }
                    else if (baseLineIndex < 0)
                    {
                        item.Action = PmsConstant.ACTION_LEFT;
                        CompareRelation.Add(item);
                    }
                }
            });
            lProjectBaseLineRelationship.ForEach(item =>
            {
                item.Action = PmsConstant.ACTION_RIGHT;
                CompareRelation.Add(item);
            });
            return Json(getListCompareWbsStructure(level, FromOID, RootBaseLineOID, LProject, RProject, CompareRelation));
        }

        public PmsCompare getListCompareWbsStructure(int _level, int _FromOID, int _RootBaseLineOID, PmsProject _LProject, PmsBaseLineProject _RProject, List<IPmsRelationship> _MergeRelations)
        {
            PmsCompare getCompareStructure = new PmsCompare();
            getCompareStructure.Level = _level;
            getCompareStructure.Level = _level;
            getCompareStructure.RootBaseLineOID = _RootBaseLineOID;
            getCompareStructure.ToOID = _FromOID;
            getCompareStructure.RootOID = _FromOID;
            getCompareStructure.LName = _LProject.Name;
            getCompareStructure.LType = PmsConstant.TYPE_PROJECT;
            getCompareStructure.LEstDuration = Convert.ToInt32(_LProject.EstDuration);
            getCompareStructure.LEstStartDt = _LProject.EstStartDt;
            getCompareStructure.LEstEndDt = _LProject.EstEndDt;
            getCompareStructure.LActDuration = Convert.ToInt32(_LProject.ActDuration);
            getCompareStructure.LActStartDt = _LProject.ActStartDt;
            getCompareStructure.LActEndDt = _LProject.ActEndDt;
            getCompareStructure.LId = null;
            getCompareStructure.LWorkingDay = _LProject.WorkingDay;
            getCompareStructure.LCalendarOID = _LProject.CalendarOID;

            getCompareStructure.RName = _RProject.Name;
            getCompareStructure.RType = PmsConstant.TYPE_PROJECT;
            getCompareStructure.REstDuration = Convert.ToInt32(_RProject.EstDuration);
            getCompareStructure.REstStartDt = _RProject.EstStartDt;
            getCompareStructure.REstEndDt = _RProject.EstEndDt;
            getCompareStructure.RActDuration = Convert.ToInt32(_RProject.ActDuration);
            getCompareStructure.RActStartDt = _RProject.ActStartDt;
            getCompareStructure.RActEndDt = _RProject.ActEndDt;
            getCompareStructure.RId = null;
            getCompareStructure.RWorkingDay = _RProject.WorkingDay;
            getCompareStructure.RCalendarOID = _RProject.CalendarOID;
            getCompareWbsStructure(getCompareStructure, _FromOID, _RootBaseLineOID, _MergeRelations);
            return getCompareStructure;
        }

        public void getCompareWbsStructure(PmsCompare _compareObj, int _projOID, int _rootBaseLineOID, List<IPmsRelationship> _MergeRelations)
        {
            _compareObj.ProjectOID = _projOID;
            _compareObj.RootBaseLineOID = _rootBaseLineOID;
            _compareObj.CompareChildren = new List<PmsCompare>();
            List<IPmsRelationship> lRelation = _MergeRelations.FindAll(item => { return ((PmsRelationship)item).FromOID == _compareObj.ToOID; }).OrderBy(x => ((PmsRelationship)x).Ord).ToList();
            lRelation.ForEach(item =>
            {
                PmsRelationship tmpRelationitem = (PmsRelationship)item;

                PmsCompare tmpCompareItem = new PmsCompare();
                tmpCompareItem.Level = _compareObj.Level + 1;
                tmpCompareItem.FromOID = tmpRelationitem.FromOID;
                tmpCompareItem.ToOID = tmpRelationitem.ToOID;
                if (tmpRelationitem.Action.Equals(PmsConstant.ACTION_ADD))
                {
                    tmpCompareItem.Action = PmsConstant.ACTION_NONE;
                    PmsProcess LDetail = PmsProcessRepository.SelPmsProcess(Session, new PmsProcess { OID = tmpCompareItem.ToOID });
                    tmpCompareItem.LName = LDetail.Name;
                    tmpCompareItem.LType = LDetail.Type;
                    tmpCompareItem.LEstDuration = Convert.ToInt32(LDetail.EstDuration);
                    tmpCompareItem.LEstStartDt = LDetail.EstStartDt;
                    tmpCompareItem.LEstEndDt = LDetail.EstEndDt;
                    tmpCompareItem.LActDuration = Convert.ToInt32(LDetail.ActDuration);
                    tmpCompareItem.LActStartDt = LDetail.ActStartDt;
                    tmpCompareItem.LActEndDt = LDetail.ActEndDt;
                    tmpCompareItem.LId = LDetail.Id;
                    tmpCompareItem.LDependency = LDetail.Dependency;
                    tmpCompareItem.LOrd = tmpRelationitem.Ord;

                    PmsBaseLineProcess RDetail = PmsBaseLineProcessRepository.SelPmsBaseLIneProcess(new PmsBaseLineProcess { ProcessOID = tmpCompareItem.ToOID, RootBaseLineOID = _rootBaseLineOID });
                    tmpCompareItem.RName = RDetail.ProcessNm;
                    tmpCompareItem.RType = RDetail.Type;
                    tmpCompareItem.REstDuration = Convert.ToInt32(RDetail.EstDuration);
                    tmpCompareItem.REstStartDt = RDetail.EstStartDt;
                    tmpCompareItem.REstEndDt = RDetail.EstEndDt;
                    tmpCompareItem.RActDuration = Convert.ToInt32(RDetail.ActDuration);
                    tmpCompareItem.RActStartDt = RDetail.ActStartDt;
                    tmpCompareItem.RActEndDt = RDetail.ActEndDt;
                    tmpCompareItem.RId = RDetail.Id;
                    tmpCompareItem.RDependency = RDetail.Dependency;
                    tmpCompareItem.ROrd = tmpRelationitem.BaseLineOrd;

                    if (!tmpCompareItem.LName.Equals(tmpCompareItem.RName)
                    || !tmpCompareItem.LEstDuration.Equals(tmpCompareItem.REstDuration)
                    || !tmpCompareItem.LEstStartDt.Equals(tmpCompareItem.REstStartDt)
                    || !tmpCompareItem.LEstEndDt.Equals(tmpCompareItem.REstEndDt)
                    || (tmpCompareItem.LDependency != null && !tmpCompareItem.LDependency.Equals(tmpCompareItem.LDependency))
                    || !tmpCompareItem.LOrd.Equals(tmpCompareItem.ROrd))
                    {
                        tmpCompareItem.Action = PmsConstant.ACTION_MODIFY;

                        if (!tmpCompareItem.LName.Equals(tmpCompareItem.RName))
                        {
                            tmpCompareItem.Action = tmpCompareItem.Action + "|" + "Name";
                        }

                        if (!tmpCompareItem.LEstDuration.Equals(tmpCompareItem.REstDuration))
                        {
                            tmpCompareItem.Action = tmpCompareItem.Action + "|" + "Duration";
                        }

                        if (!tmpCompareItem.LEstStartDt.Equals(tmpCompareItem.REstStartDt))
                        {
                            tmpCompareItem.Action = tmpCompareItem.Action + "|" + "StartDt";
                        }

                        if (!tmpCompareItem.LEstEndDt.Equals(tmpCompareItem.REstEndDt))
                        {
                            tmpCompareItem.Action = tmpCompareItem.Action + "|" + "EndDt";
                        }

                        if (!tmpCompareItem.LOrd.Equals(tmpCompareItem.ROrd))
                        {
                            tmpCompareItem.Action = tmpCompareItem.Action + "|" + "Ord";
                        }
                    }
                }
                else if (tmpRelationitem.Action.Equals(PmsConstant.ACTION_LEFT))
                {
                    tmpCompareItem.Action = PmsConstant.ACTION_ADD_NM;
                    PmsProcess LDetail = PmsProcessRepository.SelPmsProcess(Session, new PmsProcess { OID = tmpCompareItem.ToOID });
                    tmpCompareItem.LName = LDetail.Name;
                    tmpCompareItem.LType = LDetail.Type;
                    tmpCompareItem.LEstDuration = Convert.ToInt32(LDetail.EstDuration);
                    tmpCompareItem.LEstStartDt = LDetail.EstStartDt;
                    tmpCompareItem.LEstEndDt = LDetail.EstEndDt;
                    tmpCompareItem.LActDuration = Convert.ToInt32(LDetail.ActDuration);
                    tmpCompareItem.LActStartDt = LDetail.ActStartDt;
                    tmpCompareItem.LActEndDt = LDetail.ActEndDt;
                    tmpCompareItem.LId = LDetail.Id;
                    tmpCompareItem.LDependency = LDetail.Dependency;
                    tmpCompareItem.LOrd = tmpRelationitem.Ord;
                }
                else if (tmpRelationitem.Action.Equals(PmsConstant.ACTION_RIGHT))
                {
                    tmpCompareItem.Action = PmsConstant.ACTION_DELETE_NM;
                    PmsBaseLineProcess RDetail = PmsBaseLineProcessRepository.SelPmsBaseLIneProcess(new PmsBaseLineProcess { ProcessOID = tmpCompareItem.ToOID, RootBaseLineOID = _rootBaseLineOID });
                    tmpCompareItem.RName = RDetail.ProcessNm;
                    tmpCompareItem.RType = RDetail.Type;
                    tmpCompareItem.REstDuration = Convert.ToInt32(RDetail.EstDuration);
                    tmpCompareItem.REstStartDt = RDetail.EstStartDt;
                    tmpCompareItem.REstEndDt = RDetail.EstEndDt;
                    tmpCompareItem.RActDuration = Convert.ToInt32(RDetail.ActDuration);
                    tmpCompareItem.RActStartDt = RDetail.ActStartDt;
                    tmpCompareItem.RActEndDt = RDetail.ActEndDt;
                    tmpCompareItem.RId = RDetail.Id;
                    tmpCompareItem.RDependency = RDetail.Dependency;
                    tmpCompareItem.ROrd = tmpRelationitem.Ord;
                }
                _compareObj.CompareChildren.Add(tmpCompareItem);
                getCompareWbsStructure(tmpCompareItem, _projOID, _rootBaseLineOID, _MergeRelations);
            });
        }

        #endregion

        #region -- Gantt

        public ActionResult DetailGanttView(string OID)
        {
            ViewBag.ProjectOID = OID;
            PmsProject pmsProject = PmsProjectRepository.SelPmsObject(Session, new PmsProject { OID = Convert.ToInt32(OID) });
            ViewBag.Holiday = string.Join(",", CalendarDetailRepository.SelCalendarDetails(new CalendarDetail { CalendarOID = pmsProject.CalendarOID }).Select(value => value.FullDate.ToString()).ToArray());
            ViewBag.WorkingDay = pmsProject.WorkingDay;
            return View("Partitial/ptDetailGanttChartView");
        }

        public JsonResult DetailGanttData(string OID)
        {

            Dictionary<string, object> ganttData = new Dictionary<string, object>();
            ganttData.Add("selectedRow", 0);
            ganttData.Add("canDelete", false);
            ganttData.Add("canWriteOnParent", false);
            ganttData.Add("canAdd", false);

            PmsProject pmsProject = PmsProjectRepository.SelPmsObject(Session, new PmsProject { OID = Convert.ToInt32(OID) });
            if (pmsProject.BPolicy.Name == PmsConstant.POLICY_PROJECT_PREPARE || pmsProject.BPolicy.Name == PmsConstant.POLICY_PROJECT_PAUSED || pmsProject.BPolicy.Name == PmsConstant.POLICY_PROJECT_TEMP_EXIST)
            {
                ganttData.Add("canWrite", true);
            }
            else
            {
                ganttData.Add("canWrite", false);
            }

            ganttData.Add("tasks", PmsRelationshipRepository.GetLDGanttWbs(Session, OID));
            List<Person> lPerson = new List<Person>();
            PmsRelationshipRepository.SelPmsRelationship(Session, new PmsRelationship { FromOID = pmsProject.OID, Type = PmsConstant.RELATIONSHIP_MEMBER }).ForEach(member =>
            {
                lPerson.Add(PersonRepository.SelPerson(Session, new Person { OID = member.ToOID }));
            });
            ganttData.Add("resources", lPerson);
            return Json(ganttData);
        }

        public JsonResult UpdateGanttData(List<Dictionary<string, object>> _params)
        {
            List<Dictionary<string, object>> cashParam = _params;
            string ProjectOID = "";
            string ProcessOID = "";
            cashParam.ForEach(item =>
            {
                if (ProjectOID.Length < 1)
                {
                    ProjectOID = Convert.ToString(item["oid"]);
                    PmsProjectRepository.UdtPmsProject(Session, new PmsProject { EstDuration = Convert.ToInt32(item["duration"]), EstStartDt = Convert.ToDateTime(item["start"]), EstEndDt = Convert.ToDateTime(item["end"]), OID = Convert.ToInt32(ProjectOID) });
                }
                else
                {
                    ProcessOID = "";
                    ProcessOID = Convert.ToString(item["oid"]);
                    PmsProcessRepository.UdtPmsProcess(Session, new PmsProcess { EstDuration = Convert.ToInt32(item["duration"]), EstStartDt = Convert.ToDateTime(item["start"]), EstEndDt = Convert.ToDateTime(item["end"]), Dependency = Convert.ToString(item["depends"]), OID = Convert.ToInt32(ProcessOID) });
                }
            });
            return Json(PmsRelationshipRepository.GetLDGanttWbs(Session, ProjectOID));
        }
        #endregion

        #region -- Resource Dashboard

        public ActionResult ResourceDashboard()
        {
            ViewBag.Status = BPolicyRepository.SelBPolicy(new BPolicy { Type = PmsConstant.TYPE_PROJECT });
            return View();
        }

        #endregion

        #region -- Import Excel WBS

        public JsonResult ImportExcelWbs(string OID, string ExcelFile)
        {
            XSSFWorkbook workBook;
            ISheet sheet;

            List<PmsRelationship> tmpRelationship = new List<PmsRelationship>();
            Dictionary<int, PmsRelationship> displayRelationship = new Dictionary<int, PmsRelationship>();

            try
            {

                string templateFilePath = HttpContext.Server.MapPath("~/TmpFile/" + ExcelFile);
                using (var fs = new FileStream(templateFilePath, FileMode.Open, FileAccess.Read))
                {
                    workBook = new XSSFWorkbook(fs);
                }
                sheet = workBook.GetSheetAt(0);
                List<Dictionary<string, string>> excelMapList = getReadExcelMapList(workBook, sheet, 1);

                PmsProject pdobj = PmsProjectRepository.SelPmsObject(Session, new PmsProject { OID = Convert.ToInt32(OID) });
                List<DateTime> lHoliday = CalendarDetailRepository.SelCalendarDetails(new CalendarDetail { CalendarOID = pdobj.CalendarOID, IsHoliday = 1 }).Select(val => DateTime.Parse(val.Year + "-" + val.Month + "-" + val.Day)).ToList();


                int workingDay = Convert.ToInt32(pdobj.WorkingDay);
                int rootOID = Convert.ToInt32(pdobj.OID);

                excelMapList.ForEach(excelMap =>
                {
                    PmsRelationship pmsRel = new PmsRelationship();
                    if (excelMap["개요 수준"].Equals("1"))
                    {
                        pmsRel.Level = Convert.ToInt32(excelMap["개요 수준"]);
                        pmsRel.ToOID = rootOID;
                        pmsRel.RootOID = rootOID;
                        pmsRel.ObjName = pdobj.Name;
                        pmsRel.ObjType = pdobj.Type;
                        pmsRel.EstStartDt = Convert.ToDateTime(string.Format("{0:yyyy-MM-dd}", excelMap["시작"].Substring(0, excelMap["시작"].IndexOf(' '))));
                        pmsRel.ObjSt = pdobj.BPolicyOID;
                        pmsRel.ObjStNm = pdobj.BPolicy.StatusNm;
                        pmsRel.WorkingDay = workingDay;
                        pmsRel.Id = Convert.ToInt32(excelMap["ID"]);
                        pmsRel.Dependency = null;
                        displayRelationship.Add(Convert.ToInt32(excelMap["개요 수준"]), pmsRel);
                    }
                    else
                    {
                        pmsRel.Action = PmsConstant.ACTION_NEW;
                        pmsRel.Id = Convert.ToInt32(excelMap["ID"]);
                        pmsRel.Level = Convert.ToInt32(excelMap["개요 수준"]);
                        pmsRel.ToOID = (rootOID + pmsRel.Id);
                        pmsRel.RootOID = rootOID;
                        pmsRel.FromOID = displayRelationship[Convert.ToInt32(pmsRel.Level - 1)].ToOID;
                        pmsRel.ObjName = excelMap["이름"];
                        pmsRel.ObjType = PmsConstant.TYPE_TASK;
                        pmsRel.WorkingDay = workingDay;
                        pmsRel.Dependency = excelMap["선행 작업"];
                        pmsRel.EstStartDt = Convert.ToDateTime(string.Format("{0:yyyy-MM-dd}", excelMap["시작"].Substring(0, excelMap["시작"].IndexOf(' '))));
                        pmsRel.EstDuration = Convert.ToInt32(excelMap["기간"].Substring(0, excelMap["기간"].IndexOf(' ')));
                        pmsRel.EstEndDt = PmsUtils.CalculateFutureDate(Convert.ToDateTime(pmsRel.EstStartDt), Convert.ToInt32(pmsRel.EstDuration), Convert.ToInt32(pmsRel.WorkingDay), lHoliday);
                        if (displayRelationship[Convert.ToInt32(pmsRel.Level - 1)].Children == null)
                        {
                            displayRelationship[Convert.ToInt32(pmsRel.Level - 1)].Children = new List<PmsRelationship>();
                        }

                        displayRelationship[Convert.ToInt32(pmsRel.Level - 1)].Children.Add(pmsRel);
                        if (displayRelationship.ContainsKey(Convert.ToInt32(pmsRel.Level)))
                        {
                            displayRelationship[Convert.ToInt32(pmsRel.Level - 1)].EstEndDt = displayRelationship[Convert.ToInt32(pmsRel.Level - 1)].Children.Select(item => item.EstEndDt).ToList().Max();
                            displayRelationship.Remove(Convert.ToInt32(pmsRel.Level));
                        }
                        displayRelationship.Add(Convert.ToInt32(pmsRel.Level), pmsRel);
                    }
                    tmpRelationship.Add(pmsRel);
                });
                displayRelationship.Clear();

                tmpRelationship.ForEach(item =>
                {
                    if (item.Level == 1)
                    {
                        displayRelationship.Add(Convert.ToInt32(item.Level), item);
                    }
                    else
                    {
                        PmsRelationship tmpRel = tmpRelationship.Find(tmpItem => Convert.ToString(tmpItem.Id).Equals(item.Dependency));
                        if (tmpRel != null)
                        {
                            item.EstStartDt = PmsUtils.CalculateFutureDate(Convert.ToDateTime(tmpRel.EstEndDt), 2, Convert.ToInt32(item.WorkingDay), lHoliday);
                            item.EstEndDt = PmsUtils.CalculateFutureDate(Convert.ToDateTime(item.EstStartDt), Convert.ToInt32(item.EstDuration), Convert.ToInt32(item.WorkingDay), lHoliday);
                        }

                        if (displayRelationship.ContainsKey(Convert.ToInt32(item.Level)))
                        {
                            displayRelationship[Convert.ToInt32(item.Level - 1)].EstEndDt = displayRelationship[Convert.ToInt32(item.Level - 1)].Children.Select(tmpItem => tmpItem.EstEndDt).ToList().Max();
                            displayRelationship[Convert.ToInt32(item.Level - 1)].EstDuration = PmsUtils.CalculateFutureDuration(Convert.ToDateTime(displayRelationship[Convert.ToInt32(item.Level - 1)].EstStartDt),
                                                                                                                                Convert.ToDateTime(displayRelationship[Convert.ToInt32(item.Level - 1)].EstEndDt),
                                                                                                                                Convert.ToInt32(displayRelationship[Convert.ToInt32(item.Level - 1)].WorkingDay),
                                                                                                                                lHoliday);
                            displayRelationship.Remove(Convert.ToInt32(item.Level));
                        }
                        if (item.Children != null && item.Children.Count > 0)
                        {
                            item.ObjType = PmsConstant.TYPE_PHASE;
                        }
                        displayRelationship.Add(Convert.ToInt32(item.Level), item);
                    }
                });

            }
            catch (Exception ex)
            {
                return Json(new ResultJsonModel { isError = true, resultMessage = ex.Message, resultDescription = ex.ToString() });
            }

            return Json(displayRelationship[1]);
        }

        public List<Dictionary<string, string>> getReadExcelMapList(XSSFWorkbook workBook, ISheet sheet, int startRowRead)
        {
            List<Dictionary<string, string>> readExcelMapList = new List<Dictionary<string, string>>();
            IRow firstRow = sheet.GetRow(0);
            int numberOfCells = firstRow.LastCellNum;
            string[] headerList = new string[numberOfCells];
            ICell cell = null;
            for (int colIdx = 0; colIdx < numberOfCells; colIdx++)
            {
                if (cell != null)
                {
                    cell = null;
                }
                cell = firstRow.GetCell(colIdx);
                headerList[colIdx] = cell.StringCellValue.Trim();
            }

            IRow row = null;
            int numberOfRows = sheet.LastRowNum;
            for (int rowId = startRowRead; rowId <= numberOfRows; rowId++)
            {
                if (row != null)
                {
                    row = null;
                }

                row = sheet.GetRow(rowId);
                Dictionary<string, string> rowMap = new Dictionary<string, string>();

                for (int colId = 0; colId < numberOfCells; colId++)
                {
                    if (cell != null)
                    {
                        cell = null;
                    }
                    cell = row.GetCell(colId);
                    if (cell != null)
                    {
                        //cell.SetCellType(CellType.String);
                        rowMap.Add(headerList[colId], cell.StringCellValue);
                    }
                    else
                    {
                        rowMap.Add(headerList[colId], "");
                    }
                }
                readExcelMapList.Add(rowMap);
            }

            return readExcelMapList;
        }

        #endregion

        #region -- Total Project Management

        #region 통합 프로젝트
        public ActionResult TotalProjMngt()
        {
            //ViewBag.TotalProjMngt = PmsProjectRepository.SelTotalProjMngt(Session, "Total");
            return View();
        }
        #endregion

        #region 통합 프로젝트 검색
        public JsonResult SelTotalProjMngt()
        {
            return Json(PmsProjectRepository.SelTotalProjMngt(Session, Common.Constant.PmsConstant.TYPE_TOTAL));
        }
        #endregion

        #region 고객대일정 템플릿 불러오기
        public ActionResult dlgCustomerScheduleTemplateLoad()
        {
            return PartialView("Dialog/dlgCustomerScheduleTemplateLoad");
        }
        #endregion

        #region 통합 프로젝트 고객사 차종 프로젝트 검색
        public JsonResult SelProjMngtCustomerSchedule(CustomerSchedule _param)
        {
            List<CustomerSchedule> result = CustomerScheduleRepository.SelProjMngtCustomerSchedule(_param);
            return Json(result);
        }
        #endregion

        #region 통합 프로젝트 고객 대일정 등록
        public JsonResult InsProjMngtCustomerSchedule(List<CustomerSchedule> _param, int? Car)
        {
            CustomerScheduleRepository.InsProjMngtCustomerSchedule(_param, Car);
            return Json(0);
        }
        #endregion

        #endregion

        #region -- Customer Schedule Template

        #region 고객대일정 템플릿
        public ActionResult CustomerScheduleTemplate()
        {
            return View();
        }
        #endregion


        public ActionResult SelCustomerScheduleTemplate(Library _param)
        {
            List<Library> ITemplate = LibraryRepository.SelCustomerScheduleTemplate(_param);
            ITemplate.ForEach(item =>
            {
                List<Library> child = LibraryRepository.SelCustomerScheduleTemplateChild(new Library { FromOID = item.OID });

                item.Cdata = child;
            });


            return Json(ITemplate);
        }

        public JsonResult updatCustomerScheduleTemplate(List<Library> _param)
        {
            int resultOid = 0;
            int defaultCOrd = 1; //자식 ord
            try
            {
                DaoFactory.BeginTransaction();
                // DObjectRepository.UdtLatestDObject(new DObject { OID = _param.OID });
                //  revision = SemsUtil.MakeMajorRevisonUp(_param[0].Cdata[0].Revision); //리비전업
                LibraryRepository.delCustomerScheduleTemplateSub(null);
                for (var i = 0; i < _param.Count; i++)
                {

                    if (_param[i].isParentMove == "Y") //부모 순서변경했을경우
                    {
                        if (_param[i].isChange == "Y") //부모가 변경됬을경우
                        {
                            if (_param[i].OID != null)
                            {
                                _param[i].Ord = i + 1;//부모순서 변경
                                DaoFactory.SetUpdate("Library.UpdateCustomerScheduleTemplate", _param[i]);
                            }
                            else
                            {
                                _param[i].Ord = i + 1;//부모순서 변경
                                _param[i].OID = DaoFactory.SetInsert("Library.InCustomerScheduleTemplate", _param[i]);
                            }
                        }
                        defaultCOrd = 1;

                        if (_param[i].Cdata != null)
                        {
                            _param[i].Cdata.ForEach(child =>
                            {
                                child.FromOID = _param[i].OID;
                                child.Ord = defaultCOrd;
                                DaoFactory.SetInsert("Library.InCustomerScheduleTemplateSub", child);
                                defaultCOrd++;
                            });
                        }
                    }
                    else //부모순서 변경안했을경우
                    {
                        if (_param[i].OID != null)
                        {
                            _param[i].Ord = i + 1;//부모순서 변경
                            DaoFactory.SetUpdate("Library.UpdateCustomerScheduleTemplate", _param[i]);
                        }
                        else
                        {
                            _param[i].Ord = i + 1;//부모순서 변경
                            _param[i].OID = DaoFactory.SetInsert("Library.InCustomerScheduleTemplate", _param[i]);
                        }
                        defaultCOrd = 1;

                        if (_param[i].Cdata != null)
                        {
                            _param[i].Cdata.ForEach(child =>
                            {
                                child.FromOID = _param[i].OID;
                                child.Ord = defaultCOrd;
                                DaoFactory.SetInsert("Library.InCustomerScheduleTemplateSub", child);
                                defaultCOrd++;
                            });
                        }
                    }
                }
                DaoFactory.Commit();
            }
            catch (Exception ex)
            {
                DaoFactory.Rollback();
                return Json(new ResultJsonModel { isError = true, resultMessage = ex.Message, resultDescription = ex.ToString() });
            }
            return Json(resultOid);
        }

        #endregion

        #region -- Pm Dashboard

        public ActionResult PmDashboard()
        {
            return View();
        }

        #endregion

        #region -- Person Dashboard

        public ActionResult PersonDashboard()
        {
            return View();
        }

        #endregion

        #region -- My Task

        public ActionResult InfoMyTask()
        {
            ViewBag.BPolicies = BPolicyRepository.SelBPolicy(new BPolicy { Type = PmsConstant.TYPE_TASK });
            return View();
        }

        public JsonResult SelPmsClass()
        {
            int iUser = Convert.ToInt32(Session["UserOID"]);
            List<PmsClass> lPmsClass = new List<PmsClass>();
            List<PmsProject> lPmsProj = new List<PmsProject>();
            List<BDefine> lBDefine = BDefineRepository.SelDefines(new BDefine { Module = PmsConstant.MODULE_PMS, Type = CommonConstant.TYPE_ROLE });
            int guestOID = Convert.ToInt32(lBDefine.Find(def => def.Name == PmsConstant.ROLE_GUEST).OID);
            List<PmsRelationship> lMemeber = PmsRelationshipRepository.SelPmsRelationship(Session, new PmsRelationship { Type = PmsConstant.RELATIONSHIP_MEMBER, ToOID = iUser }).FindAll(member => member.RoleOID != guestOID);
            List<int> CompleteOID = new List<int>();

            DObject dobj = null;
            lMemeber.ForEach(item =>
            {
                if (dobj != null)
                {
                    dobj = null;
                }
                if (!CompleteOID.Contains(Convert.ToInt32(item.RootOID)))
                {
                    dobj = DObjectRepository.SelDObject(Session, new DObject { OID = item.RootOID });
                    if (dobj != null)
                    {
                        if (dobj.Type == PmsConstant.TYPE_PROJECT)
                        {
                            CompleteOID.Add(Convert.ToInt32(item.RootOID));
                            lPmsProj.Add(PmsProjectRepository.SelPmsObject(Session, new PmsProject { OID = item.RootOID }));
                        }
                    }
                }
            });

            if (lPmsProj.Count > 0)
            {
                List<BPolicy> lBPolicy = BPolicyRepository.SelBPolicy(new BPolicy { Type = PmsConstant.TYPE_PROJECT });
                lBPolicy.ForEach(bpolicy =>
                {
                    if (bpolicy.Name == PmsConstant.POLICY_PROJECT_STARTED)
                    {
                        PmsClass pClass = new PmsClass();
                        pClass.Name = bpolicy.StatusNm;
                        pClass.Children = lPmsProj.FindAll(proj => proj.BPolicyOID == bpolicy.OID);
                        lPmsClass.Add(pClass);
                    }
                });
            }
            return Json(lPmsClass);
        }

        public JsonResult SelMyTask(string ProjectOID)
        {
            int iUser = Convert.ToInt32(Session["UserOID"]);
            List<Dictionary<string, object>> lResult = new List<Dictionary<string, object>>();
            List<BDefine> lBDefine = BDefineRepository.SelDefines(new BDefine { Module = PmsConstant.MODULE_PMS, Type = CommonConstant.TYPE_ROLE });
            int guestOID = Convert.ToInt32(lBDefine.Find(def => def.Name == PmsConstant.ROLE_GUEST).OID);
            List<PmsRelationship> lMemeber = PmsRelationshipRepository.SelPmsRelationship(Session, new PmsRelationship { Type = PmsConstant.RELATIONSHIP_MEMBER, ToOID = iUser }).FindAll(member => member.RoleOID != guestOID);
            List<PmsRelationship> lProjMember = null;
            if (ProjectOID != null && ProjectOID.Length > 0)
            {
                lProjMember = lMemeber.FindAll(item => item.RoleOID != guestOID && item.RootOID == Convert.ToInt32(ProjectOID));
            }
            else
            {
                lProjMember = lMemeber.FindAll(item => item.RoleOID != guestOID);
            }

            List<BPolicy> lBPolicy = BPolicyRepository.SelBPolicy(new BPolicy { Type = PmsConstant.TYPE_TASK });
            List<int> CompleteOID = new List<int>();

            DObject dobj = null;
            DObject pDobj = null;
            PmsProject proj = null;
            PmsProcess proc = null;
            List<DateTime> lHoliday = null;
            lProjMember.ForEach(item =>
            {
                if (!CompleteOID.Contains(Convert.ToInt32(item.FromOID)))
                {
                    if (dobj != null)
                    {
                        dobj = null;
                    }
                    dobj = DObjectRepository.SelDObject(Session, new DObject { OID = item.FromOID });
                    if (dobj != null && dobj.Type == PmsConstant.TYPE_TASK)
                    {
                        if (pDobj != null)
                        {
                            pDobj = null;
                        }
                        if (proj != null)
                        {
                            proj = null;
                        }
                        if (proc != null)
                        {
                            proc = null;
                        }
                        if (lHoliday != null)
                        {
                            lHoliday = null;
                        }

                        pDobj = DObjectRepository.SelDObject(Session, new DObject { OID = item.RootOID });
                        if (pDobj == null || pDobj.Type != PmsConstant.TYPE_PROJECT)
                        {
                            return;
                        }
                        if (pDobj.BPolicy.Name != PmsConstant.POLICY_PROCESS_STARTED)
                        {
                            return;
                        }
                        proj = PmsProjectRepository.SelPmsObject(Session, new PmsProject { OID = pDobj.OID });
                        lHoliday = CalendarDetailRepository.SelCalendarDetails(new CalendarDetail { CalendarOID = Convert.ToInt32(proj.CalendarOID), IsHoliday = 1 }).Select(val => DateTime.Parse(val.Year + "-" + val.Month + "-" + val.Day)).ToList();
                        proc = PmsProcessRepository.SelPmsProcess(Session, new PmsProcess { OID = dobj.OID });

                        Dictionary<string, object> resultData = new Dictionary<string, object>();
                        resultData.Add("id", proc.OID);
                        resultData.Add("ProcessOID", proc.OID);
                        resultData.Add("label", proc.Name);
                        resultData.Add("ProcessNm", proc.Name);
                        resultData.Add("state", proc.BPolicyOID);
                        resultData.Add("ProcessSt", proc.BPolicyOID);
                        resultData.Add("ProcessStNm", proc.BPolicy.Name);
                        resultData.Add("EstStartDt", proc.EstStartDt != null ? string.Format("{0:yyyy-MM-dd}", proc.EstStartDt) : "");
                        resultData.Add("EstEndDt", proc.EstEndDt != null ? string.Format("{0:yyyy-MM-dd}", proc.EstEndDt) : "");
                        resultData.Add("ActStartDt", proc.ActStartDt != null ? string.Format("{0:yyyy-MM-dd}", proc.ActStartDt) : "");
                        resultData.Add("ActEndDt", proc.ActEndDt != null ? string.Format("{0:yyyy-MM-dd}", proc.ActEndDt) : "");

                        int gap = 0;
                        if (proc.ActEndDt != null)
                        {
                            
                            gap = PmsUtils.CalculateDelay(Convert.ToDateTime(proc.EstEndDt), Convert.ToDateTime(proc.ActEndDt), Convert.ToInt32(proj.WorkingDay), lHoliday);
                        }
                        else
                        {
                            gap = PmsUtils.CalculateDelay(Convert.ToDateTime(proc.EstEndDt), Convert.ToDateTime(string.Format("{0:yyyy-MM-dd}", DateTime.Now)), Convert.ToInt32(proj.WorkingDay), lHoliday);
                        }

                        if (gap > 1 && gap <= PmsConstant.DELAY)
                        {
                            resultData.Add("hex", PmsConstant.WARNING_COLOR);
                        }
                        else if (gap > PmsConstant.DELAY)
                        {
                            resultData.Add("hex", PmsConstant.DELAY_COLOR);
                        }
                        else
                        {
                            resultData.Add("hex", "");
                        }
                       
                        resultData.Add("content", proj.Name + "|" + resultData["EstStartDt"] + "|" + resultData["EstEndDt"] + "|" + resultData["ActStartDt"] + "|" + resultData["ActEndDt"]);
                        resultData.Add("resourceId", proj.OID);
                        lResult.Add(resultData);
                        CompleteOID.Add(Convert.ToInt32(proc.OID));
                    }
                }
            });
            return Json(lResult);
        }

        #endregion

        #region -- Issue

        public ActionResult CreateIssue(PmsIssue _param)
        {
            ViewBag.Detail = _param;
            return PartialView("Dialog/dlgCreateIssue");
        }

        public ActionResult InfoIssue(PmsIssue _param)
        {
            PmsIssue issue = PmsIssueRepository.SelIssue(Session, _param);
            issue.BPolicyAuths = BPolicyAuthRepository.MainAuth(Session, issue, PmsAuth.ManagerAuth(Session, issue));
            issue.BPolicyAuths.AddRange(PmsAuth.IssuePmAuth(Session, issue));
            if (issue.Manager_OID != null)
            {
                issue.ManagerNm = PersonRepository.SelPerson(Session, new Person { OID = issue.Manager_OID }).Name;
            }
            ViewBag.Detail = issue;
            ViewBag.Detail.RootOID = _param.RootOID;
            ViewBag.Detail.TaskNm = _param.TaskNm;
            ViewBag.Detail.ProjectNm = _param.ProjectNm;
            ViewBag.BPolicy = BPolicyRepository.SelBPolicy(new BPolicy { Type = ViewBag.Detail.Type });
            return PartialView("Dialog/dlgInfoIssue");
        }

        public JsonResult InsIssue(PmsIssue _param)
        {
            int resultOid = 0;
            DObject dobj = null;
            PmsRelationship relDobj = null;
            try
            {
                DaoFactory.BeginTransaction();

                dobj = new DObject();
                //dobj.Type = PmsConstant.TYPE_PROJECT;
                if (_param.Type == null)
                {
                    return Json(new ResultJsonModel { isError = true, resultMessage = "필수값이 빠졌습니다." });
                }
                else
                {
                    dobj.Type = _param.Type;
                }
                dobj.TableNm = PmsConstant.TABLE_ISSUE;
                dobj.Name = _param.Name;
                dobj.Description = _param.Description;
                resultOid = DObjectRepository.InsDObject(Session, dobj);

                _param.OID = resultOid;
                if(_param.Manager_OID != null)
                {
                    _param.BPolicy = BPolicyRepository.SelBPolicy(new BPolicy { Type = _param.Type }).First();
                    TriggerUtil.StatusPromote(Session, false, _param.Type, Convert.ToString(_param.BPolicy.OID), Convert.ToInt32(_param.OID), Convert.ToInt32(_param.OID),CommonConstant.ACTION_PROMOTE, null);
                }
                PmsIssueRepository.InsPmsIssue(Session, _param);

                relDobj = new PmsRelationship();
                relDobj.Type = PmsConstant.RELATIONSHIP_ISSUE;
                relDobj.FromOID = _param.FromOID;
                relDobj.ToOID = resultOid;
                relDobj.RootOID = _param.RootOID;
                PmsRelationshipRepository.InsPmsRelationship(Session, relDobj);

                DaoFactory.Commit();
            }
            catch (Exception ex)
            {
                DaoFactory.Rollback();
                return Json(new ResultJsonModel { isError = true, resultMessage = ex.Message, resultDescription = ex.ToString() });
            }
            finally
            {
                dobj = null;
                relDobj = null;
            }
            return Json(resultOid);
        }

        public JsonResult SelIssue(PmsRelationship _param)
        {
            _param.Type = PmsConstant.RELATIONSHIP_ISSUE;
            PmsProject PjojData = PmsProjectRepository.SelPmsObject(Session, new PmsProject { OID = Convert.ToInt32(_param.RootOID), Type = (_param.ObjType != null ? _param.ObjType : null), IsTemplate = (_param.ObjType != null ? _param.ObjType : null) });
            List<PmsRelationship> lIssue = PmsRelationshipRepository.SelPmsRelationship(Session, _param);
            List<PmsRelationship> lProjectList = PmsRelationshipRepository.GetProjWbsLIst(Session, Convert.ToString(PjojData.OID));
            List<PmsIssue> IssueData = new List<PmsIssue>();
            List<PmsRelationship> checkDel = new List<PmsRelationship>();
            PmsIssue tmpIssue = null;
            lIssue.ForEach(robj =>
            {
                if (lProjectList.FindAll(item => item.ToOID == robj.FromOID).Count > 0)
                {
                    if (tmpIssue != null)
                    {
                        tmpIssue = null;
                    }
                    checkDel = PmsRelationshipRepository.SelPmsRelationship(Session, new PmsRelationship { ToOID = robj.FromOID, RootOID = _param.RootOID, Type = PmsConstant.RELATIONSHIP_WBS });
                    if (checkDel.Count < 1)
                    {
                        return;
                    }
                    tmpIssue = PmsIssueRepository.SelIssue(Session, new PmsIssue { OID = robj.ToOID });
                    if (robj.RootOID != robj.FromOID)
                    {
                        tmpIssue.TaskNm = PmsProcessRepository.SelPmsProcess(Session, new PmsProcess { OID = Convert.ToInt32(robj.FromOID) }).Name;
                    }
                    tmpIssue.ProjectNm = PjojData.Name;
                    IssueData.Add(tmpIssue);
                }
            });
            return Json(IssueData);
        }

        public JsonResult UdtIssue(PmsIssue _param)
        {
            int result = 0;
            try
            {
                DaoFactory.BeginTransaction();

                DObjectRepository.UdtDObject(Session, _param);
                PmsIssueRepository.UdtIssue(Session, _param);
                if (_param.Manager_OID != null)
                {
                    if (_param.BPolicyNm == PmsConstant.POLICY_ISSUE_TASK_PREPARE)
                    {
                        _param.BPolicy = BPolicyRepository.SelBPolicy(new BPolicy { Type = _param.Type }).First();
                        TriggerUtil.StatusPromote(Session, false, _param.Type, Convert.ToString(_param.BPolicy.OID), Convert.ToInt32(_param.OID), Convert.ToInt32(_param.OID), CommonConstant.ACTION_PROMOTE, null);
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
                return Json(new ResultJsonModel { isError = true, resultMessage = ex.Message, resultDescription = ex.ToString() });
            }
            return Json(result);
        }

        public PartialViewResult dlgSelectIssueManager(string ProjectOID)
        {
            ViewBag.ProjectOID = ProjectOID;
            ViewBag.Role = BDefineRepository.SelDefines(new BDefine { Type = CommonConstant.DEFINE_ROLE, Module = PmsConstant.MODULE_PMS });
            return PartialView("Dialog/dlgSelectIssueManager");
        }

        public JsonResult ProcessIssue(ApprovalTask _param)
        {
            int result = 0;
            try
            {
                DaoFactory.BeginTransaction();
                TriggerUtil.StatusPromote(Session, false, _param.Type, Convert.ToString(_param.BPolicyOID), Convert.ToInt32(_param.OID), Convert.ToInt32(_param.OID), _param.ApprovalType, null);

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

        #region -- deliveries
        public ActionResult dlgDocClassDeliveries(PmsRelationship deliveriesparam)
        {
            ViewBag.Detail = deliveriesparam;
            return PartialView("Dialog/dlgDocClassDeliveries");
        }

        #region -- 프로젝트에 등록 되어있는 문서분류체계 검색
        public JsonResult SelPmsDocumentClassification(PmsRelationship _param)
        {
            List<PmsRelationship> SelPmsDocumentClassification = PmsRelationshipRepository.SelPmsDeliveriesRelationship(Session, 0, _param);
            return Json(SelPmsDocumentClassification);
        }
        #endregion

        #region -- 프로젝트에 문서분류체계 등록
        public JsonResult InsProjectDocumentClassification(List<PmsRelationship> _param)
        {
            int resultOID = 0;
            try
            {
                DaoFactory.BeginTransaction();
                if (_param != null)
                {
                    _param.ForEach(obj =>
                    {
                        obj.Type = PmsConstant.RELATIONSHIP_DOC_MASTER;
                        PmsRelationshipRepository.InsPmsRelationship(Session, obj);
                    });
                }


                DaoFactory.Commit();
            }
            catch (Exception ex)
            {
                DaoFactory.Rollback();
                return Json(new ResultJsonModel { isError = true, resultMessage = ex.Message, resultDescription = ex.ToString() });
            }
            return Json(resultOID);
        }
        #endregion

        #region -- 프로젝트에 문서분류체계 삭제
        public JsonResult DelProjectDocumentClassification(PmsRelationship _param)
        {
            try
            {
                DaoFactory.BeginTransaction();
                PmsRelationshipRepository.DelPmsRelaionship(Session, _param);
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

        #endregion

        #region -- 산출물 Action 신뢰성 의뢰서
        public ActionResult dlgReliabilityForm(string RootOID, string FromOID)
        {
            Library DevStepKey = LibraryRepository.SelLibraryObject(new Library { Name = CommonConstant.ATTRIBUTE_DEVSTEP });
            List<Library> DevStepList = LibraryRepository.SelLibrary(new Library { FromOID = DevStepKey.OID });
            ViewBag.DevStepList = DevStepList;

            return PartialView("Dialog/dlgReliabilityForm");
        }

        #region -- 산출물 Action 신뢰성 의뢰서 등록
        public JsonResult InsReliability(PmsReliability _param, List<TestItemList> _ItemListParam)
        {
            DObject dobj = new DObject();
            int resultOID = 0;
            try
            {
                DaoFactory.BeginTransaction();

                var YYYY = DateTime.Now.ToString("yyyy");
                var MM = DateTime.Now.ToString("MM");
                var dd = DateTime.Now.ToString("dd");

                dobj.Type = PmsConstant.TYPE_RELIABILITY;
                dobj.TableNm = PmsConstant.TABLE_RELIABILITY;

                var selName = "DR" + YYYY + MM + dd + "-001";
                var NewName = "DR" + YYYY + MM + dd;

                var LateName = PmsReliabilityRepository.SelPmsReliability(Session, new PmsReliability { Name = NewName });

                if (LateName.Count == 0)
                {
                    dobj.Name = selName;
                }
                else
                {
                    int NUM = Convert.ToInt32(LateName.Last().Name.Substring(11, 3)) + 1;
                    dobj.Name = NewName + "-" + string.Format("{0:D3}", NUM);
                }

                dobj.Description = _param.Description;
                resultOID = DObjectRepository.InsDObject(Session, dobj);

                _param.OID = resultOID;
                PmsReliabilityRepository.InsPmsReliability(Session, _param);

                PmsRelationship PmsRelations = new PmsRelationship();
                PmsRelations.Type = PmsConstant.RELATIONSHIP_DOC_CLASS;
                PmsRelations.RootOID = _param.RootOID; //프로젝트 OID
                PmsRelations.FromOID = _param.FromOID; 
                PmsRelations.TaskOID = _param.TaskOID;
                PmsRelations.ToOID = resultOID; //ToOID 분류

                PmsRelationshipRepository.InsPmsRelationship(Session, PmsRelations);

                PmsReliabilityRepository.InsPmsReliabilityItemList(Session, _ItemListParam, resultOID);
                

                DaoFactory.Commit();
            }
            catch (Exception ex)
            {
                DaoFactory.Rollback();
                return Json(new ResultJsonModel { isError = true, resultMessage = ex.Message, resultDescription = ex.ToString() });
            }
            return Json(resultOID);
        }
        #endregion

        #region -- 산출물 Action 신뢰성 의뢰서 시험항목 리스트 불러오기
        public JsonResult SelTestItemList(PmsReliability _param)
        {
            Library TestItemKey = LibraryRepository.SelCodeLibraryObject(new Library { Code1 = CommonConstant.ATTRIBUTE_TESTITEMLIST });
            List<Library> LibTestItemList = LibraryRepository.SelCodeLibrary(new Library { FromOID = TestItemKey.OID });

            List<TestItemList> TestItem = new List<TestItemList>();
            
            LibTestItemList.ForEach(Item => {
                TestItemList TestItemObj = new TestItemList();
                TestItemObj.TestItemLib_OID = Item.OID;
                TestItemObj.TestItemNm = Item.KorNm;

                TestItem.Add(TestItemObj);
            });
            return Json(TestItem);
        }
        #endregion

        #region -- 산출물 Action 신뢰성 의뢰서 상세
        public ActionResult dlgInfoReliabilityForm(string OID)
        {
            Library DevStepKey = LibraryRepository.SelLibraryObject(new Library { Name = CommonConstant.ATTRIBUTE_DEVSTEP });
            List<Library> DevStepList = LibraryRepository.SelLibrary(new Library { FromOID = DevStepKey.OID });
            ViewBag.DevStepList = DevStepList;

            Library TestItemKey = LibraryRepository.SelCodeLibraryObject(new Library { Code1 = CommonConstant.ATTRIBUTE_TESTITEMLIST });
            
            ViewBag.Detail =  PmsReliabilityRepository.SelPmsReliabilityObject(Session, new PmsReliability { OID = Convert.ToInt32(OID) });

            return PartialView("Dialog/dlgInfoReliabilityForm");
        }
        #endregion

        #region -- 산출물 Action 신뢰성 의뢰서 상세 시험항목 리스트
        public JsonResult SelInfoTestItemList(TestItemList _param)
        {
            List<TestItemList> TestItem = PmsReliabilityRepository.SelPmsReliabilityItemList(Session, _param);
            return Json(TestItem);
        }
        #endregion

        #region -- 산출물 Action 신뢰성 의뢰서 업데이트
        public JsonResult UdtReliability(PmsReliability _param, List<TestItemList> _ItemListParam)
        {
            int result = 0;
            try
            {
                DaoFactory.BeginTransaction();

                DObjectRepository.UdtDObject(Session, _param);

                PmsReliabilityRepository.UdtPmsReliability(Session, _param);

                DaoFactory.SetDelete("Pms.delTestItemList", new TestItemList { FromOID = _param.OID });

                PmsReliabilityRepository.InsPmsReliabilityItemList(Session, _ItemListParam, _param.OID);


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

        #region -- 산출물 Action 신뢰성 결과서
        public ActionResult dlgSearchReliabilityReport()
        {
            Library DevStepKey = LibraryRepository.SelLibraryObject(new Library { Name = CommonConstant.ATTRIBUTE_DEVSTEP });
            List<Library> DevStepList = LibraryRepository.SelLibrary(new Library { FromOID = DevStepKey.OID });
            ViewBag.DevStepList = DevStepList;
            return PartialView("Dialog/dlgSearchReliabilityReport");
        }
        #endregion

        #region -- 산출물 Action 신뢰성 결과서 검색
        public JsonResult SelReliabilityReport(PmsReliabilityReport _param)
        {
            List<PmsReliabilityReport> result = new List<PmsReliabilityReport>();
            try
            {
                DaoFactory.BeginTransaction();
                result =  PmsReliabilityReportRepository.SelPmsReliabilityReport(Session, _param);
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

        #region -- 산출물 Action 신뢰성 결과서 연결
        public JsonResult InsRelationReliabilityReport(PmsRelationship _param)
        {
            int result = 0;
            try
            {
                DaoFactory.BeginTransaction();
                _param.Type = Common.Constant.PmsConstant.RELATIONSHIP_DOC_CLASS;
                PmsRelationshipRepository.InsPmsRelationship(Session, _param);
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

        #region -- 산출물 Action 신뢰성 결과서 상세
        public ActionResult dlgInfoReliabilityReport(string OID)
        {
            Library DevStepKey = LibraryRepository.SelLibraryObject(new Library { Name = CommonConstant.ATTRIBUTE_DEVSTEP });
            List<Library> DevStepList = LibraryRepository.SelLibrary(new Library { FromOID = DevStepKey.OID });
            ViewBag.DevStepList = DevStepList;

            Library ProgressKey = LibraryRepository.SelLibraryObject(new Library { Name = CommonConstant.ATTRIBUTE_PROGRESS });
            List<Library> ProgressList = LibraryRepository.SelLibrary(new Library { FromOID = ProgressKey.OID });
            ViewBag.ProgressList = ProgressList;

            ViewBag.Detail = PmsReliabilityReportRepository.SelPmsReliabilityReportObject(Session, new PmsReliabilityReport { OID = Convert.ToInt32(OID) });
            return PartialView("Dialog/dlgInfoReliabilityReport");
        }
        #endregion

        #region -- 산출물 Action 신뢰성 결과서 상세 시험항목 리스트
        public JsonResult SelInfoReportTestItemList(ReportTestItemList _param)
        {
            List<ReportTestItemList> TestItem = PmsReliabilityReportRepository.SelPmsReliabilityReportItemList(Session, _param);
            return Json(TestItem);
        }
        #endregion

        #region -- 산출물 Action 신뢰성 결과서 업데이트
        public JsonResult UdtReliabilityReport(PmsReliabilityReport _param, List<ReportTestItemList> _ReportItemListParam)
        {
            int result = 0;
            try
            {
                DaoFactory.BeginTransaction();

                DObjectRepository.UdtDObject(Session, _param);
                PmsReliabilityReportRepository.UdtPmsReliabilityReport(Session, _param);
                DaoFactory.SetDelete("Pms.delReportTestItemList", new TestItemList { FromOID = _param.OID });
                PmsReliabilityReportRepository.InsPmsReliabilityReportItemList(Session, _ReportItemListParam, _param.OID);

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

        #endregion

        #region -- Pms Document
        public ActionResult CreateDocument(Doc _param)
        {
            ViewBag.Detail = _param;
            return PartialView("Dialog/dlgCreateDocument");
        }

        public ActionResult InfoDocument(Doc _param)
        {

            ViewBag.Detail = DocRepository.SelDocObject(Session,new Doc{OID = _param.OID });
            return PartialView("Dialog/dlgInfoDocument");
        }

        #region 문서 등록
        public JsonResult InsertPmsDocument(Doc _param)
        {
            int resultOid = 0;
            try
            {
                DaoFactory.BeginTransaction();
                //    DObjectRepository.UdtLatestDObject(new DObject { OID = _param.OID });
                PmsRelationship relDobj = null;
                DObject dobj = new DObject();
                dobj.Type = DocumentConstant.TYPE_DOCUMENT;
                dobj.TableNm = DocumentConstant.TABLE_DOCUMENT;
                dobj.Description = _param.Description;
                //dobj.TdmxOID = DObjectRepository.SelTdmxOID(new DObject { Type = DocumentContant.TYPE_DOCUMENT });
                var YYYY = DateTime.Now.ToString("yyyy");
                var MM = DateTime.Now.ToString("MM");
                var dd = DateTime.Now.ToString("dd");
                var selName = "DOC" + YYYY + MM + dd + "-001";
                var NewName = "DOC" + YYYY + MM + dd;

                var LateName = DocRepository.SelDoc(Session, new Doc { Name = NewName });

                if (LateName.Count == 0)
                {
                    dobj.Name = selName;
                }
                else
                {
                    int NUM = Convert.ToInt32(LateName.Last().Name.Substring(12, 3)) + 1;
                    dobj.Name = NewName + "-" + string.Format("{0:D3}", NUM);
                }

                resultOid = DObjectRepository.InsDObject(Session, dobj);
                _param.OID = resultOid;
                _param.Type = dobj.Type;
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
                _param.OID = resultOid;
                _param.DocGroup = DocClassConstant.TYPE_DOCCLASS;
                //_param.DocType = _param.DocType;
                //_param.Title = _param.Title;
                //_param.Eo_No = _param.Eo_No;
                DaoFactory.SetInsert("Doc.InsDoc", _param);

                relDobj = new PmsRelationship();
                relDobj.Type = PmsConstant.RELATIONSHIP_DOC_CLASS;
                relDobj.FromOID = _param.FromOID;
                relDobj.ToOID = resultOid;
                relDobj.RootOID = _param.RootOID;
                relDobj.TaskOID = _param.TaskOID;
                PmsRelationshipRepository.InsPmsRelationship(Session, relDobj);

                DaoFactory.Commit();
            }
            catch (Exception ex)
            {
                DaoFactory.Rollback();
                return Json(new ResultJsonModel { isError = true, resultMessage = ex.Message, resultDescription = ex.ToString() });
            }
            return Json(resultOid);
        }
        #endregion

        #endregion

    }
}