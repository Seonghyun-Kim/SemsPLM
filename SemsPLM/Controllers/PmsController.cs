﻿using Common;
using Common.Constant;
using Common.Factory;
using Common.Models;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using Pms;
using Pms.Interface;
using Pms.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Services.Description;

namespace SemsPLM.Controllers
{
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

        #region -- Project

        public ActionResult SearchProject()
        {
            return View();
        }

        public ActionResult CreateProject()
        {
            return PartialView("Dialog/dlgCreateProject");
        }

        public ActionResult InfoProject(string OID)
        {
            ViewBag.OID = OID;
            ViewBag.GanttUrl = "/Pms/DetailGanttView?OID=" + OID;
            ViewBag.Detail = PmsProjectRepository.SelPmsObject(new PmsProject { OID = Convert.ToInt32(OID) });
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
                resultOid = DObjectRepository.InsDObject(dobj);

                _param.OID = resultOid;
                _param.Complete = PmsConstant.INIT_COMPLETE;
                _param.EstStartDt = _param.BaseDt;
                _param.EstDuration = PmsConstant.INIT_DURATION;
                _param.EstEndDt = PmsUtils.CalculateFutureDate(Convert.ToDateTime(_param.BaseDt), Convert.ToInt32(_param.EstDuration), Convert.ToInt32(_param.WorkingDay), lHoliday);
                PmsProjectRepository.InsPmsProject(_param);

                relDobj = new PmsRelationship();
                relDobj.Type = PmsConstant.RELATIONSHIP_MEMBER;
                relDobj.FromOID = resultOid;
                relDobj.ToOID = 75;
                relDobj.RoleOID = BDefineRepository.SelDefine(new BDefine { Type = CommonConstant.DEFINE_ROLE, Module = PmsConstant.MODULE_PMS, Name = PmsConstant.ROLE_PM }).OID;
                relDobj.RootOID = resultOid;
                PmsRelationshipRepository.InsPmsRelationship(relDobj);

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
            List<PmsProject> lPmses = PmsProjectRepository.SelPmsObjects(_param);
            return Json(lPmses);
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
            ViewBag.Detail = PmsProjectRepository.SelPmsObject(new PmsProject { Type = PmsConstant.TYPE_PROJECT_TEMP, OID = Convert.ToInt32(OID), IsTemplate = PmsConstant.TYPE_PROJECT_TEMP });
            ViewBag.Holiday = string.Join(",", CalendarDetailRepository.SelCalendarDetails(new CalendarDetail { CalendarOID = Convert.ToInt32(ViewBag.Detail.CalendarOID) }).Select(value => value.FullDate.ToString()).ToArray());
            ViewBag.Status = BPolicyRepository.SelBPolicy(new BPolicy { Type = PmsConstant.TYPE_PROJECT_TEMP });
            ViewBag.Role = BDefineRepository.SelDefines(new BDefine { Type = CommonConstant.DEFINE_ROLE, Module = PmsConstant.MODULE_PMS });
            return View();
        }

        #endregion

        #region -- Process

        public ActionResult InfoProcess(string ProjectOID, string OID)
        {
            ViewBag.ProjectOID = ProjectOID;
            ViewBag.ProjectDetail = PmsProjectRepository.SelPmsObject(new PmsProject { OID = Convert.ToInt32(ProjectOID) });
            ViewBag.OID = OID;
            ViewBag.Detail = PmsProcessRepository.SelPmsProcess(new PmsProcess { OID = Convert.ToInt32(OID) });
            ViewBag.Holiday = string.Join(",", CalendarDetailRepository.SelCalendarDetails(new CalendarDetail { CalendarOID = Convert.ToInt32(ViewBag.ProjectDetail.CalendarOID) }).Select(value => value.FullDate.ToString()).ToArray());
            ViewBag.Status = BPolicyRepository.SelBPolicy(new BPolicy { Type = ViewBag.Detail.ProcessType });
            ViewBag.Role = BDefineRepository.SelDefines(new BDefine { Type = CommonConstant.DEFINE_ROLE, Module = PmsConstant.MODULE_PMS });
            return View();
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
                    PmsRelationshipRepository.DelPmsRelaionship(new PmsRelationship { Type = PmsConstant.RELATIONSHIP_WBS, OID = item.OID });
                });

                _params.FindAll(filter => filter.Action != PmsConstant.ACTION_DELETE).ForEach(item =>
                {
                    tmpOid = 0;
                    if (item.Action == null || item.Action.Length < 1)
                    {
                        if (item.ObjType.Equals(PmsConstant.TYPE_PROJECT))
                        {
                            if (dobj != null)
                            {
                                dobj = null;
                            }
                            dobj = new DObject();
                            dobj.OID = item.ToOID;
                            dobj.Name = item.ObjName;
                            dobj.Description = item.Description;
                            DObjectRepository.UdtDObject(dobj);

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
                            PmsProjectRepository.UdtPmsProject(tmpPmsProject);
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
                            DObjectRepository.UdtDObject(dobj);

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
                            PmsProcessRepository.UdtPmsProcess(tmpPmsProcess);

                            if (tmpPmsRelationship != null)
                            {
                                tmpPmsRelationship = null;
                            }
                            tmpPmsRelationship = new PmsRelationship();
                            tmpPmsRelationship.OID = item.OID;
                            tmpPmsRelationship.Ord = indexOrd;
                            PmsRelationshipRepository.UdtPmsRelationship(tmpPmsRelationship);

                            if (item.Members != null && item.Members.Count > 0)
                            {
                                PmsRelationshipRepository.DelPmsRelaionship(new PmsRelationship { FromOID = item.ToOID, Type = PmsConstant.RELATIONSHIP_MEMBER });
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
                                    PmsRelationshipRepository.InsPmsRelationship(tmpPmsRelationship);
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
                            tmpOid = DObjectRepository.InsDObject(dobj);
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
                                PmsProcessRepository.InsPmsProcess(tmpPmsProcess);
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
                                int tmpResult = PmsRelationshipRepository.InsPmsRelationship(tmpPmsRelationship);
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
                                PmsRelationshipRepository.UdtPmsRelationship(tmpPmsRelationship);

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
                                        PmsRelationshipRepository.InsPmsRelationship(tmpPmsRelationship);
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
            PmsProject proj = PmsProjectRepository.SelPmsObject(new PmsProject { OID = _param.OID });
            return Json(PmsRelationshipRepository.GetObjWbsStructure(Level, Convert.ToInt32(proj.OID), proj));
        }

        #endregion

        #region -- Member
        
        public bool CheckMembers(PmsRelationship param)
        {
            bool bResult = true;
            List<PmsRelationship> lPmsMembers = PmsRelationshipRepository.SelPmsRelationship(new PmsRelationship { Type = PmsConstant.RELATIONSHIP_MEMBER, RootOID = param.RootOID });
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
            List<PmsRelationship> lMember = PmsRelationshipRepository.SelPmsRelationship(_param);
            Person tmpPerson = null;
            lMember.ForEach(member =>
            {
                member.RoleOIDNm = BDefineRepository.SelDefine(new BDefine { OID = member.RoleOID }).Description;
                if (tmpPerson != null)
                {
                    tmpPerson = null;
                }
                tmpPerson = PersonRepository.SelPerson(new Person { OID = member.ToOID });
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
                    PmsRelationshipRepository.DelPmsRelaionship(item);
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
                            PmsRelationshipRepository.UdtPmsRelationship(pmsRelationship);
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
                                PmsRelationshipRepository.InsPmsRelationship(pmsRelationship);
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
            PmsRelationshipRepository.SelPmsRelationship(new PmsRelationship { Type = PmsConstant.RELATIONSHIP_WBS, FromOID = Convert.ToInt32(ProjectOID) }).ForEach(item =>
            {
                PmsProcess tmpProcess = PmsProcessRepository.SelPmsProcess(new PmsProcess { OID = item.ToOID });
                if (tmpProcess.Type.Equals(PmsConstant.TYPE_GATE))
                {
                    lGate.Add(tmpProcess);
                }
            });
            ViewBag.lGateView = lGate;
            return PartialView("Partitial/ptDetailGateView");
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
            List<PmsRelationship> lPmsRelaionship = PmsRelationshipRepository.SelPmsRelationship(new PmsRelationship { Type = PmsConstant.RELATIONSHIP_BASELINE, FromOID = Convert.ToInt32(ProjectOID) });
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
                dobj.Name = baseLineNmPrefix + SemsUtil.MakeSeq(DObjectRepository.SelNameSeq(new DObject { Type = PmsConstant.TYPE_BASE_LINE_PROJECT, Name = baseLineNmPrefix + "%" }), "000");
                dobj.TableNm = PmsConstant.TABLE_PROJECT;
                resultOID = DObjectRepository.InsDObject(dobj);
                PmsBaseLineProjectRepository.InsPmsBaseLineProject(new PmsBaseLineProject { TargetProjectOID = Convert.ToInt32(ProjectOID), ProjectBaseLineOID = resultOID  });

                relDobj = new PmsRelationship();
                relDobj.Type = PmsConstant.RELATIONSHIP_BASELINE;
                relDobj.FromOID = Convert.ToInt32(ProjectOID);
                relDobj.ToOID = resultOID;
                relDobj.RootOID = Convert.ToInt32(ProjectOID);
                PmsRelationshipRepository.InsPmsRelationship(relDobj);

               
                List<PmsRelationship> lWbs = PmsRelationshipRepository.SelPmsRelationship(new PmsRelationship { Type = PmsConstant.RELATIONSHIP_WBS, RootOID = Convert.ToInt32(ProjectOID) });
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
                    lPmsMemberRelationship = PmsRelationshipRepository.SelPmsRelationship(new PmsRelationship { Type = PmsConstant.RELATIONSHIP_MEMBER, FromOID = value });
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
            PmsProject LProject = PmsProjectRepository.SelPmsObject(new PmsProject { OID = Convert.ToInt32(ProjectOID) });
            PmsBaseLineProject RProject = PmsBaseLineProjectRepository.SelPmsBaseLIneProject(new PmsBaseLineProject { OID = Convert.ToInt32(ProjectBaseLineOID) });
            int FromOID = Convert.ToInt32(ProjectOID);
            int RootBaseLineOID = Convert.ToInt32(ProjectBaseLineOID);

            List<IPmsRelationship> CompareRelation = new List<IPmsRelationship>();
            List<PmsRelationship> lProjectRelationship = PmsRelationshipRepository.SelPmsRelationship(new PmsRelationship { RootOID = FromOID, Type = PmsConstant.RELATIONSHIP_WBS });
            List<PmsBaseLineRelationship> lProjectBaseLineRelationship = PmsBaseLineRelationshipRepository.SelPmsBaseLineRelationship(new PmsBaseLineRelationship { RootOID = FromOID, RootBaseLineOID = RootBaseLineOID, Type = PmsConstant.RELATIONSHIP_WBS });

            lProjectRelationship.ForEach(item =>
            {
                int compareIndex = CompareRelation.FindIndex(innerItem => { return  ((PmsRelationship)innerItem).FromOID == item.FromOID && ((PmsRelationship)innerItem).ToOID == item.ToOID; });
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
                    PmsProcess LDetail = PmsProcessRepository.SelPmsProcess(new PmsProcess { OID = tmpCompareItem.ToOID });
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
                    PmsProcess LDetail = PmsProcessRepository.SelPmsProcess(new PmsProcess { OID = tmpCompareItem.ToOID });
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
            PmsProject pmsProject = PmsProjectRepository.SelPmsObject(new PmsProject { OID = Convert.ToInt32(OID) });            
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

            PmsProject pmsProject = PmsProjectRepository.SelPmsObject(new PmsProject { OID = Convert.ToInt32(OID) });
            if (pmsProject.BPolicy.Name == PmsConstant.POLICY_PROJECT_PREPARE || pmsProject.BPolicy.Name == PmsConstant.POLICY_PROJECT_PAUSED)
            {
                ganttData.Add("canWrite", true);
            }
            else
            {
                ganttData.Add("canWrite", false);
            }

            ganttData.Add("tasks",  PmsRelationshipRepository.GetLDGanttWbs(OID));
            List<Person> lPerson = new List<Person>();
            PmsRelationshipRepository.SelPmsRelationship(new PmsRelationship { FromOID = pmsProject.OID, Type = PmsConstant.RELATIONSHIP_MEMBER }).ForEach(member =>
            {
                lPerson.Add(PersonRepository.SelPerson(new Person { OID = member.ToOID }));
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
                    PmsProjectRepository.UdtPmsProject(new PmsProject { EstDuration = Convert.ToInt32(item["duration"]), EstStartDt = Convert.ToDateTime(item["start"]), EstEndDt = Convert.ToDateTime(item["end"]), OID = Convert.ToInt32(ProjectOID) });
                }
                else
                {
                    ProcessOID = "";
                    ProcessOID = Convert.ToString(item["oid"]);
                    PmsProcessRepository.UdtPmsProcess(new PmsProcess { EstDuration = Convert.ToInt32(item["duration"]), EstStartDt = Convert.ToDateTime(item["start"]), EstEndDt = Convert.ToDateTime(item["end"]), Dependency = Convert.ToString(item["depends"]), OID = Convert.ToInt32(ProcessOID) });
                }
            });
            return Json(PmsRelationshipRepository.GetLDGanttWbs(ProjectOID));
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

                PmsProject pdobj = PmsProjectRepository.SelPmsObject(new PmsProject { OID = Convert.ToInt32(OID) });
                List<DateTime> lHoliday = CalendarDetailRepository.SelCalendarDetails(new CalendarDetail { CalendarOID = pdobj.CalendarOID, IsHoliday = 1 }).Select(val => DateTime.Parse(val.Year + "-" + val.Month + "-" + val.Day)).ToList();


                int workingDay = Convert.ToInt32(pdobj.WorkingDay);
                int rootOID = Convert.ToInt32(pdobj.OID);

                excelMapList.ForEach(excelMap =>
                {
                    PmsRelationship pmsRel = new PmsRelationship();
                   if(excelMap["개요 수준"].Equals("1"))
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
                        pmsRel.EstEndDt = PmsUtils.CalculateFutureDate( Convert.ToDateTime(pmsRel.EstStartDt), Convert.ToInt32(pmsRel.EstDuration), Convert.ToInt32(pmsRel.WorkingDay), lHoliday);
                        if (displayRelationship[Convert.ToInt32(pmsRel.Level - 1)].Children == null)
                        {
                            displayRelationship[Convert.ToInt32(pmsRel.Level - 1)].Children = new List<PmsRelationship>();
                        }

                        displayRelationship[Convert.ToInt32(pmsRel.Level - 1)].Children.Add(pmsRel);
                        if(displayRelationship.ContainsKey(Convert.ToInt32(pmsRel.Level))){
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
            for(int colIdx = 0; colIdx < numberOfCells; colIdx++)
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
            for(int rowId = startRowRead; rowId <= numberOfRows; rowId++)
            {
                if (row != null)
                {
                    row = null;
                }

                row = sheet.GetRow(rowId);
                Dictionary<string, string> rowMap = new Dictionary<string, string>();
                
                for(int colId = 0; colId < numberOfCells; colId++)
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
    }
}