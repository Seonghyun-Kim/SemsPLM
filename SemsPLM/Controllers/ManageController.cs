using Common;
using Common.Constant;
using Common.Factory;
using Common.Models;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Web;
using System.Web.Mvc;

namespace SemsPLM.Controllers
{
    public class ManageController : Controller
    {
        // GET: Manage
        public ActionResult Index()
        {
            return View();
        }


        #region -- Module : Calendar

        public ActionResult CalendarManage()
        {
            return View();
        }

        public PartialViewResult DetailCalendar()
        {
            return PartialView("Partitial/ptDetailCalendar");
        }

        public ActionResult EditCalendarInfo()
        {
            return PartialView("Dialog/dlgEditCalendar");
        }

        public JsonResult InsertCalendar(Calendar _param)
        {
            int result = 0;
            try
            {
                DaoFactory.BeginTransaction();
                DObject dObj = new DObject();
                dObj.Name = _param.Name;
                dObj.Type = CommonConstant.TYPE_CALENDAR;
                dObj.TableNm = CommonConstant.TABLE_CALENDAR;
                dObj.TdmxOID = DObjectRepository.SelTdmxOID(new DObject { Type = dObj.Type });
                dObj.IsLatest = 1;
                int dOid = DObjectRepository.InsDObject(dObj);

                _param.OID = dOid;
                DaoFactory.SetInsert("Manage.InsCalendar", _param);

                List<CalendarDetail> holidayDefault = DaoFactory.GetList<CalendarDetail>("Manage.SelCalendarHolidayDefailt", null);
                holidayDefault.ForEach(item =>
                {
                    item.CalendarOID = dOid;
                    DaoFactory.SetInsert("Manage.InsCalendarDetail", item);
                });
                DaoFactory.Commit();
            }
            catch (Exception ex)
            {
                DaoFactory.Rollback();
                 return Json(new ResultJsonModel { isError = true, resultMessage = ex.Message, resultDescription = ex.ToString() });
            }
            return Json(result);
        }

        public JsonResult SelectCalendar()
        {
            List<Calendar> lCalendar = DaoFactory.GetList<Calendar>("Manage.SelCalendar", new Calendar { Type = CommonConstant.TYPE_CALENDAR });
            lCalendar.ForEach(obj =>
            {
                obj.BPolicy = BPolicyRepository.SelBPolicy(new BPolicy { Type = obj.Type, OID = obj.BPolicyOID }).First();
            });
            return Json(lCalendar);    
        }

        public JsonResult InsertCalenderDetail(Calendar _param)
        {
            int result = 0;
            try
            {
                DaoFactory.BeginTransaction();

                Calendar calendar = DaoFactory.GetData<Calendar>("Manage.SelCalendar", new Calendar { Type = CommonConstant.TYPE_CALENDAR, OID = _param.OID });
                List<CalendarDetail> lCalendarDetails = DaoFactory.GetList<CalendarDetail>("Manage.SelCalenderDetail", new CalendarDetail { CalendarOID = calendar.OID });
                if (lCalendarDetails != null && lCalendarDetails.Count > 0)
                {
                    DObjectRepository.UdtLatestDObject(new DObject { OID = calendar.OID });
                    DObject dObj = new DObject();
                    dObj.Name = calendar.Name;
                    dObj.Type = CommonConstant.TYPE_CALENDAR;
                    dObj.TableNm = CommonConstant.TABLE_CALENDAR;
                    dObj.TdmxOID = calendar.TdmxOID;
                    result = DObjectRepository.InsDObject(dObj);

                    Calendar tmpCalendar = new Calendar();
                    tmpCalendar.OID = result;
                    tmpCalendar.WorkingDay = calendar.WorkingDay;
                    DaoFactory.SetInsert("Manage.InsCalendar", tmpCalendar);
                }
                else
                {
                    result = Convert.ToInt32(calendar.OID);
                }

                if (_param.CalendarDetails != null && _param.CalendarDetails.Count > 0)
                {
                    _param.CalendarDetails.ForEach(item =>
                    {
                        item.CalendarOID = result;
                        DaoFactory.SetInsert("Manage.InsCalendarDetail", item);
                    });
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

        public JsonResult SelectCalenderDetail(CalendarDetail _param)
        {
            List<CalendarDetail> lCalendarDetails = DaoFactory.GetList<CalendarDetail>("Manage.SelCalenderDetail", new CalendarDetail { CalendarOID = _param.CalendarOID });
            return Json(lCalendarDetails);
        }

        #endregion

        #region -- Module : User

        public ActionResult UserManage()
        {
            ViewBag.Organization = CompanyRepository.SelOrganization();
            return View();
        }

        #endregion

        #region -- Module : Department

        public ActionResult EditDepartment(JqTreeModel _param)
        {
            ViewBag.id = _param.id;
            if (_param.id != null)
            {
                DObject dobj = DObjectRepository.SelDObjects(new DObject { Type = CommonConstant.TYPE_DEPARTMENT, OID = _param.id }).First();
                ViewBag.label = dobj.Name;
                ViewBag.value = dobj.Description;
            }
            else
            {
                ViewBag.label = "";
                ViewBag.value = "";
            }
            return PartialView("Dialog/dlgEditDepartment");
        }

        public JsonResult DelDepartment(JqTreeModel _param)
        {
            int result = 0;
            try
            {
                DaoFactory.BeginTransaction();

                DRelationship dRel = new DRelationship();
                dRel.Type = CommonConstant.RELATIONSHIP_DEPARTMENT;
                dRel.FromOID = _param.parentId;
                dRel.ToOID = _param.id;
                result = DRelationshipRepository.DelDRelationship(dRel);

                DaoFactory.Commit();
            }
            catch (Exception ex)
            {
                DaoFactory.Rollback();
                return Json(new ResultJsonModel { isError = true, resultMessage = ex.Message, resultDescription = ex.ToString() });

            }
            return Json(result);
        }

        public JsonResult UtpDepartment(JqTreeModel _param)
        {
            int result = 0;
            try
            {
                DaoFactory.BeginTransaction();

                DObject dobj = new DObject();
                dobj.Type = CommonConstant.TYPE_DEPARTMENT;
                dobj.OID = _param.id;
                dobj.Name = _param.label;
                dobj.Description = _param.value;
                result = DObjectRepository.UdtDObject(dobj);

                DaoFactory.Commit();
            }
            catch (Exception ex)
            {
                DaoFactory.Rollback();
                return Json(new ResultJsonModel { isError = true, resultMessage = ex.Message, resultDescription = ex.ToString() });

            }
            return Json(result);
        }

        public JsonResult InsDepartment(JqTreeModel _param)
        {
            int result = 0;
            try
            {
                DaoFactory.BeginTransaction();

                DObject dobj = new DObject();
                dobj.Type = CommonConstant.TYPE_DEPARTMENT;
                dobj.Name = _param.label;
                dobj.Description = _param.value;
                result = DObjectRepository.InsDObject(dobj);

                DRelationship dRel = new DRelationship();
                dRel.Type = CommonConstant.RELATIONSHIP_DEPARTMENT;
                dRel.FromOID = _param.parentId;
                dRel.ToOID = result;
                DRelationshipRepository.InsDRelationshipNotOrd(dRel);

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

        #region -- Module : Person

        public ActionResult EditPerson(Person _param)
        {
            if (_param.OID != null)
            {
                Person person = PersonRepository.SelPerson(new Person { OID = _param.OID });
                ViewBag.DepartmentOID = person.DepartmentOID;
                ViewBag.DepartmentNm = person.DepartmentNm;
                ViewBag.OID = person.OID;
                ViewBag.Person = person;
            }
            else
            {
                ViewBag.DepartmentOID = _param.DepartmentOID;
                ViewBag.DepartmentNm = _param.DepartmentNm;
            }
            return PartialView("Dialog/dlgEditPerson");
        }

        public JsonResult InsPerson(Person _param)
        {
            int result = 0;
            try
            {
                DaoFactory.BeginTransaction();

                DObject dobj = new DObject();
                dobj.Type = CommonConstant.TYPE_PERSON;
                dobj.Name = _param.Name;
                result = DObjectRepository.InsDObject(dobj);

                _param.OID = result;
                int returnValue = PersonRepository.InsPerson(_param);
                if (returnValue < 0)
                {
                    throw new Exception("ID가 중복입니다.");
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

        public JsonResult UtpPerson(Person _param)
        {
            int result = 0;
            try
            {
                DaoFactory.BeginTransaction();

                DObject dobj = new DObject();
                dobj.Type = CommonConstant.TYPE_PERSON;
                dobj.Name = _param.Name;
                dobj.OID = _param.OID;
                dobj.Thumbnail = _param.Thumbnail;
                DObjectRepository.UdtDObject(dobj);
                
                PersonRepository.UtpPerson(_param);
                DaoFactory.Commit();
            }
            catch (Exception ex)
            {
                DaoFactory.Rollback();
                return Json(new ResultJsonModel { isError = true, resultMessage = ex.Message, resultDescription = ex.ToString() });
            }
            return Json(result);
        }

        public JsonResult UdtPwPerson(Person _param)
        {
            int result = 0;
            try
            {
                DaoFactory.BeginTransaction();
                PersonRepository.UtpPwPerson(_param);
                DaoFactory.Commit();
            }
            catch (Exception ex)
            {
                DaoFactory.Rollback();
                return Json(new ResultJsonModel { isError = true, resultMessage = ex.Message, resultDescription = ex.ToString() });
            }
            return Json(result);
        }

        public JsonResult SelPersons(Person _param)
        {
            List<Person> lPerson = null;
            if (_param.DepartmentOID == null || _param.DepartmentOID < 0)
            {
                lPerson = PersonRepository.SelPersons(_param);
            }
            else
            {
                DObject dobj = DObjectRepository.SelDObject(new DObject { OID = _param.DepartmentOID });
                if (dobj.Type.Equals(CommonConstant.TYPE_COMPANY))
                {
                    lPerson = PersonRepository.SelPersons(new Person { });
                }
                else
                {
                    lPerson = PersonRepository.SelPersons(_param);
                }
            }
            return Json(lPerson);
        }

        public JsonResult DelPerson(Person _param)
        {
            int result = 0;
            try
            {
                DaoFactory.BeginTransaction();

                DObject dobj = new DObject();
                dobj.Type = CommonConstant.TYPE_PERSON;
                dobj.OID = _param.OID;
                DObjectRepository.DelDObject(dobj);

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

        #region -- Module : Library
        public ActionResult LibraryManage()
        {
            return View();
        }

        public JsonResult SelLibrary(Library _param)
        {
            List<Library> lLibrary = LibraryRepository.SelLibrary(_param);
            return Json(lLibrary);
        }
        public JsonResult SelLibraryObject(Library _param)
        {
            Library Library = LibraryRepository.SelLibraryObject(_param);
            return Json(Library);
        }
        public JsonResult updateLibrary(Library _param)
        {
            int result = 0;
            try
            {
                DaoFactory.BeginTransaction();
                result= DObjectRepository.UdtDObject(_param);
                LibraryRepository.updateLibrary(_param);
                DaoFactory.Commit();
            }
            catch (Exception ex)
            {

                DaoFactory.Rollback();
                return Json(new ResultJsonModel { isError = true, resultMessage = ex.Message, resultDescription = ex.ToString() });
            }
            return Json(result);
        }
        public ActionResult dlgLibraryManip()
        {
            return PartialView("Dialog/dlgLibraryManip");
        }
        public JsonResult InsertLibrary(Library _param)
        {
            int resultOid = 0;
            try
            {
                DaoFactory.BeginTransaction();
               // DObjectRepository.UdtLatestDObject(new DObject { OID = _param.OID });

                DObject dobj = new DObject();
                dobj.Type = CommonConstant.TYPE_LIBRARY;
                dobj.TableNm = CommonConstant.TABLE_LIBRARY;
                dobj.Name = _param.Name;
                dobj.Description = _param.Description;
                resultOid = DObjectRepository.InsDObject(dobj);

                _param.OID = resultOid;
                //_param.DocType = _param.DocType;
                //_param.Title = _param.Title;
                //_param.Eo_No = _param.Eo_No;
                DaoFactory.SetInsert("Library.InsLibrary", _param);

                DaoFactory.Commit();
            }
            catch (Exception ex)
            {
                DaoFactory.Rollback();
                return Json(new ResultJsonModel { isError = true, resultMessage = ex.Message, resultDescription = ex.ToString() });
            }
            return Json(resultOid);
        }

        public JsonResult delLibrary(Library _param)
        {
            try
            {
                DaoFactory.BeginTransaction();
                DObjectRepository.DelDObject(_param);
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

        #region -- Module : CodeLibrary

        public ActionResult CodeLibrary()
        {
            return View();
        }

        public JsonResult SelCodeLibrary(Library _param)
        {
            List<Library> lLibrary = LibraryRepository.SelCodeLibrary(_param);
            return Json(lLibrary);
        }
        public JsonResult SelCodeLibraryChild(Library _param)
        {
            List<Library> lLibrary = LibraryRepository.SelCodeLibraryChild(_param);
            return Json(lLibrary);
        }
        public JsonResult SelCodeLibraryObject(Library _param)
        {
            Library Library = LibraryRepository.SelCodeLibraryObject(_param);
            return Json(Library);
        }

        public ActionResult dlgCodeLibraryManip()
        {
            return PartialView("Dialog/dlgCodeLibraryManip");
        }

        public JsonResult InsertCodeLibrary(Library _param)
        {
            int resultOid = 0;
            try
            {
                DaoFactory.BeginTransaction();
                // DObjectRepository.UdtLatestDObject(new DObject { OID = _param.OID });

                DaoFactory.SetInsert("Library.InsCodeLibrary", _param);

                DaoFactory.Commit();
            }
            catch (Exception ex)
            {
                DaoFactory.Rollback();
                return Json(new ResultJsonModel { isError = true, resultMessage = ex.Message, resultDescription = ex.ToString() });
            }
            return Json(resultOid);
        }

        public JsonResult delCodeLibrary(Library _param)
        {
            try
            {
                DaoFactory.BeginTransaction();
                LibraryRepository.deleteCodeLibrary(_param);
                DaoFactory.Commit();
            }
            catch (Exception ex)
            {

                DaoFactory.Rollback();
                return Json(new ResultJsonModel { isError = true, resultMessage = ex.Message, resultDescription = ex.ToString() });
            }
            return Json(0);
        }

        public JsonResult updateCodeLibrary(Library _param)
        {
            int result = 0;
            try
            {
                DaoFactory.BeginTransaction();
                LibraryRepository.updateCodeLibrary(_param);
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

        #region -- Module : Auth

        public ActionResult AuthManage()
        {
            return View();
        }

        public ActionResult EditRole(Role _param)
        {
            if (_param.OID != null)
            {
                ViewBag.Role = DObjectRepository.SelDObject(new DObject { Type = CommonConstant.TYPE_ROLE, OID = _param.OID });
            }
            else
            {
                ViewBag.Role = null;
            }
            return PartialView("Dialog/dlgEditRole");
        }

        public JsonResult SelRole(DRelationship _param)
        {
            List<Role> lRoles = RoleRepository.SelRoles(new Role { });
            if (_param.FromOID != null)
            {
                DRelationshipRepository.SelRelationship(new DRelationship { Type = CommonConstant.RELATIONSHIP_ROLE, FromOID = _param.FromOID }).ForEach(role =>
                {
                    lRoles.FindAll(innerRole => innerRole.OID == role.ToOID).ForEach(filterRole =>
                    {
                        filterRole.IsChecked = true;
                    });
                });
            }
            return Json(lRoles);
        }

        public JsonResult InsRole(Role _param)
        {
            int result = 0;
            try
            {
                DaoFactory.BeginTransaction();

                DObject dobj = new DObject();
                dobj.Type = CommonConstant.TYPE_ROLE;
                dobj.Name = _param.Name;
                if (DObjectRepository.SelDObject(dobj) != null)
                {
                    throw new Exception("Role 중복입니다.");
                }
                result = DObjectRepository.InsDObject(dobj);
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
    }
}