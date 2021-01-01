using Common;
using Common.Constant;
using Common.Factory;
using Common.Models;
using DocumentClassification.Models;
using SemsPLM.Filter;
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
    [AuthorizeFilter]
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
                dObj.TdmxOID = DObjectRepository.SelTdmxOID(Session, new DObject { Type = dObj.Type });
                dObj.IsLatest = 1;
                int dOid = DObjectRepository.InsDObject(Session, dObj);

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
                    DObjectRepository.UdtLatestDObject(Session, new DObject { OID = calendar.OID });
                    DObject dObj = new DObject();
                    dObj.Name = calendar.Name;
                    dObj.Type = CommonConstant.TYPE_CALENDAR;
                    dObj.TableNm = CommonConstant.TABLE_CALENDAR;
                    dObj.TdmxOID = calendar.TdmxOID;
                    result = DObjectRepository.InsDObject(Session, dObj);

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
            catch (Exception ex)
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
            ViewBag.Organization = CompanyRepository.SelOrganization(Session);
            return View();
        }

        #endregion

        #region -- Module : Department

        public ActionResult EditDepartment(JqTreeModel _param)
        {
            ViewBag.id = _param.id;
            if (_param.id != null)
            {
                DObject dobj = DObjectRepository.SelDObjects(Session, new DObject { Type = CommonConstant.TYPE_DEPARTMENT, OID = _param.id }).First();
                ViewBag.label = dobj.Name;
                ViewBag.value = dobj.Description;
                ViewBag.CreateDt = dobj.CreateDt;
                ViewBag.CreateUsNm = PersonRepository.SelPerson(Session, new Person { OID =dobj.CreateUs}).Name;

            }
            else
            {
                ViewBag.label = "";
                ViewBag.value = "";
                ViewBag.CreateDt = Convert.ToString(DateTime.Now.ToString("yyyy-MM-dd"));
                ViewBag.CreateUsNm = Session["UserNm"];
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
                result = DRelationshipRepository.DelDRelationship(Session, dRel);

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
                result = DObjectRepository.InsDObject(Session, dobj);

                DRelationship dRel = new DRelationship();
                dRel.Type = CommonConstant.RELATIONSHIP_DEPARTMENT;
                dRel.FromOID = _param.parentId;
                dRel.ToOID = result;
                DRelationshipRepository.InsDRelationshipNotOrd(Session, dRel);

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

        #region -- Module : Person

        public ActionResult EditPerson(Person _param)
        {
            if (_param.OID != null)
            {
                Person person = PersonRepository.SelPerson(Session, new Person { OID = _param.OID });
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
                dobj.CreateUs = Convert.ToInt32(Session["UserOID"]);
                result = DObjectRepository.InsDObject(Session, dobj);

                _param.OID = result;
                int returnValue = PersonRepository.InsPerson(Session, _param);
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
                DObjectRepository.UdtDObject(Session, dobj);

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
                lPerson = PersonRepository.SelPersons(Session, _param);
            }
            else
            {
                DObject dobj = DObjectRepository.SelDObject(Session, new DObject { OID = _param.DepartmentOID });
                if (dobj.Type.Equals(CommonConstant.TYPE_COMPANY))
                {
                    lPerson = PersonRepository.SelPersons(Session, new Person { });
                }
                else
                {
                    lPerson = PersonRepository.SelPersons(Session, _param);
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
                DObjectRepository.DelDObject(Session, dobj);

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
                result = DObjectRepository.UdtDObject(Session, _param);
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
                resultOid = DObjectRepository.InsDObject(Session, dobj);

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
                DObjectRepository.DelDObject(Session, _param);
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
                _param.CreateUs = Convert.ToInt32(Session["UserOID"]);
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
                _param.DeleteUs = Convert.ToInt32(Session["UserOID"]);
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

        #region -- Module : AssessListLibrary
        public ActionResult AssessListLibrary()
        {
            return View();
        }

        public ActionResult SelAssessLibrary(Library _param)
        {
            List<Library> lLibrary = LibraryRepository.SelAssessLibraryLatest(_param);
            lLibrary.ForEach(item =>
            {
                List<Library> child = LibraryRepository.SelAssessLibraryChild(new Library { FromOID = item.OID });

                item.Cdata = child;
            });


            return Json(lLibrary);
        }

        public JsonResult updateAssessLibrary(List<Library> _param)
        {
            int resultOid = 0;
            int defaultCOrd = 1; //자식 ord
            string revision = "";
            try
            {
                DaoFactory.BeginTransaction();
                // DObjectRepository.UdtLatestDObject(new DObject { OID = _param.OID });
                //  revision = SemsUtil.MakeMajorRevisonUp(_param[0].Cdata[0].Revision); //리비전업
                for (var i = 0; i < _param.Count; i++)
                {
                    if (_param[0].isParentMove == "Y") //부모 순서변경했을경우
                    {
                        if (_param[i].isChange == "Y") //부모 리비전 상승했을경우
                        {
                            LibraryRepository.UpdateAssessIsLatest(_param[i]); //isLatest = 0으로 변경
                            _param[i].Revision = SemsUtil.MakeMajorRevisonUp(_param[i].Revision);
                            _param[i].Ord = i + 1;//부모순서 변경
                            _param[i].CreateUs = Convert.ToInt32(Session["UserOID"]);
                            resultOid = DaoFactory.SetInsert("Library.InsAssessParent", _param[i]);
                            defaultCOrd = 1;
                            if (_param[i].Cdata != null)
                            {
                                _param[i].Cdata.ForEach(child =>
                            {
                                child.FromOID = resultOid;
                                child.Ord = defaultCOrd;
                                DaoFactory.SetInsert("Library.InsAssessChild", child);
                                defaultCOrd++;
                            });
                            }
                        }
                        else if (_param[i].isMove == "Y") //리비전 미변경, 자식 순서변경
                        {
                            _param[i].Ord = i + 1;
                            DaoFactory.SetUpdate("Library.UpdateAssessParentOrd", _param[i]); //부모순서 변경

                            defaultCOrd = 1;
                            if (_param[i].Cdata != null)
                            {
                                _param[i].Cdata.ForEach(child =>
                            {
                                child.FromOID = resultOid;
                                child.Ord = defaultCOrd;
                                DaoFactory.SetUpdate("Library.UpdateAssessChildOrd", child);
                                defaultCOrd++;
                            });
                            }
                        }
                        else if (_param[i].isDelete == "Y") //삭제여부 판단
                        {
                            _param[i].DeleteUs = Convert.ToInt32(Session["UserOID"]);
                            DaoFactory.SetUpdate("Library.deleteAssessLibrary", _param[i]); //부모삭제
                        }
                        else //리비전 미변경 자식 미변경
                        {
                            _param[i].Ord = i + 1;
                            DaoFactory.SetUpdate("Library.UpdateAssessParentOrd", _param[i]); //부모순서 변경
                        }
                    }
                    else //부모순서 변경안했을경우
                    {
                        if (_param[i].isChange == "Y") //부모 리비전 상승했을경우
                        {
                            LibraryRepository.UpdateAssessIsLatest(_param[i]); //isLatest = 0으로 변경
                            _param[i].Revision = SemsUtil.MakeMajorRevisonUp(_param[i].Revision);
                            _param[i].Ord = i + 1;
                            _param[i].CreateUs = Convert.ToInt32(Session["UserOID"]);
                            resultOid = DaoFactory.SetInsert("Library.InsAssessParent", _param[i]);
                            defaultCOrd = 1;
                            if (_param[i].Cdata != null)
                            {
                                _param[i].Cdata.ForEach(child =>
                            {
                                child.FromOID = resultOid;
                                child.Ord = defaultCOrd;
                                DaoFactory.SetInsert("Library.InsAssessChild", child);
                                defaultCOrd++;
                            });
                            }
                        }
                        else if (_param[i].isMove == "Y") //리비전 미변경, 자식 순서변경
                        {
                            defaultCOrd = 1;
                            if (_param[i].Cdata != null)
                            {
                                _param[i].Cdata.ForEach(child =>
                                {
                                    child.FromOID = resultOid;
                                    child.Ord = defaultCOrd;
                                    DaoFactory.SetUpdate("Library.UpdateAssessChildOrd", child);
                                    defaultCOrd++;
                                });
                            }
                        }
                        else if (_param[i].isDelete == "Y") //삭제여부 판단
                        {
                            _param[i].DeleteUs = Convert.ToInt32(Session["UserOID"]);
                            DaoFactory.SetUpdate("Library.deleteAssessLibrary", _param[i]); //부모삭제
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

        #region -- Module : Auth

        public ActionResult AuthManage()
        {
            return View();
        }

        public ActionResult EditRole(Role _param)
        {
            if (_param.OID != null)
            {
                ViewBag.Role = DObjectRepository.SelDObject(Session, new DObject { Type = CommonConstant.TYPE_ROLE, OID = _param.OID });
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
                DRelationshipRepository.SelRelationship(Session, new DRelationship { Type = CommonConstant.RELATIONSHIP_ROLE, FromOID = _param.FromOID }).ForEach(role =>
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
                if (DObjectRepository.SelDObject(Session, dobj) != null)
                {
                    throw new Exception("Role 중복입니다.");
                }
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

        #endregion

        #region -- Module : 문서분류체계
        public ActionResult DocumentClassification()
        {
            return View();
        }

        #region -- 문서분류체계 수정 등록창
        public ActionResult dlgDocumentClassIntegrate()
        {
            return PartialView("Dialog/dlgDocumentClassIntegrate");
        }
        #endregion

        #region -- 문서분류체계 검색
        public JsonResult SelDocumentClassification(DocClass _param)
        {
            return Json(DocClassRepository.SelDocClass(Session, _param));
        }
        #endregion

        #region -- 문서분류체계 검색
        public JsonResult SelProjectDocumentClassification(DocClass _param)
        {
            DocClass Document = new DocClass();
            if (_param.Type == Common.Constant.DocumentConstant.TYPE_DOCUMENT)
            {
                Document = DocClassRepository.SelDocClassObject(Session, new DocClass { Name = CommonConstant.ATTRIBUTE_DOCUMENT });
            }
            else
            {
                Document = DocClassRepository.SelDocClassObject(Session, new DocClass { Name = CommonConstant.ATTRIBUTE_PROJECT_DOCUMENT });
            }
            _param.OID = Document.OID;
            _param.FromOID = Document.OID;

            return Json(DocClassRepository.SelProjectDocClass(Session, _param));
        }
        #endregion

        #region -- 문서분류체계 등록
        public JsonResult InsDocumentClassification(DocClass _param)
        {
            int resultOID = 0;
            try
            {
                DaoFactory.BeginTransaction();

                DObject dobj = new DObject();
                dobj.Type = DocClassConstant.TYPE_DOCCLASS;
                dobj.TableNm = DocClassConstant.TABLE_DOCCLASS;
                dobj.Name = _param.Name;
                dobj.Description = _param.Description;
                resultOID = DObjectRepository.InsDObject(Session, dobj);

                _param.OID = resultOID;
                DaoFactory.SetInsert("DocClass.InsDocClass", _param);

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

        #region -- 문서분류체계 수정
        public JsonResult UdtDocumentClassification(DocClass _param)
        {
            int resultOID = 0;
            try
            {
                DaoFactory.BeginTransaction();
                DObjectRepository.UdtDObject(Session, _param);
                DaoFactory.SetUpdate("DocClass.UdtDocClass", _param);
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

        #region -- 문서분류체계 삭제
        public JsonResult DelDocumentClassification(DocClass _param)
        {
            int resultOID = 0;
            try
            {
                DaoFactory.BeginTransaction();
                DObjectRepository.DelDObject(Session, _param);
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

        #endregion
    }
}