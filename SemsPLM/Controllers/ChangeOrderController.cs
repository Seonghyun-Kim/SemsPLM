using Common.Constant;
using Common.Factory;
using Common.Models;
using System;
using ChangeOrder.Models;
using System.Collections.Generic;
using System.Web.Mvc;
using EBom.Models;
using System.Linq;
using ChangeRequest.Models;
using Common.Models.File;
using SemsPLM.Filter;

namespace SemsPLM.Controllers
{
    [AuthorizeFilter]
    public class ChangeOrderController : Controller
    {
        // GET: ChangeOrder
        public ActionResult InfoMiniChangeOrder(int OID)
        {
            ECO ECODetail = ECORepository.SelChangeOrderObject(Session, new ECO { OID = OID });
            Library reasonKey = LibraryRepository.SelLibraryObject(new Library { Name = CommonConstant.ATTRIBUTE_REASONCHANGE });
            List<Library> reasonList = LibraryRepository.SelLibrary(new Library { FromOID = reasonKey.OID });  //변경 사유 부분
            List<EO> SelassessList = EORepository.SelEOContentsOID(new EO { RootOID = ECODetail.OID, Type = EoConstant.TYPE_ASSESSLIST });  //설계변경의 체크리스트 마스터OID검색
            List<Library> originList = new List<Library>();
            SelassessList.ForEach(item =>
            {
                Library pdata = LibraryRepository.SelAssessLibraryObject(new Library { OID = item.ToOID }); //마스터 검색
                EO AssessManager = EORepository.SelEOContentsObject(new EO { FromOID = item.ToOID, RootOID = ECODetail.OID, Type = EoConstant.TYPE_ASSESSLIST_MANAGER });
                if (AssessManager != null)
                {
                    pdata.ToOID = AssessManager.ToOID;
                    pdata.ManagerNm = PersonRepository.SelPerson(Session, new Person { OID = AssessManager.ToOID }).Name;
                }
                pdata.Cdata = LibraryRepository.SelAssessLibraryChild(new Library { FromOID = pdata.OID }); //마스터 자녀들검색
                originList.Add(pdata);

                List<EO> checkData = EORepository.SelEOContentsOID(new EO { FromOID = item.ToOID, RootOID = ECODetail.OID, Type = EoConstant.TYPE_ASSESSLIST_CHILD });

                item.CheckData = checkData;  //체크리스트 검색
            });

            Approval approv = ApprovalRepository.SelApprovalNonStep(Session, new Approval { TargetOID = Convert.ToInt32(OID) });
            Approval tempApprov = ApprovalRepository.SelApproval(Session, new Approval { TargetOID = Convert.ToInt32(OID) });
            string ApprovFlag = CommonConstant.ACTION_NO;
            if (tempApprov.ApprovalCount < tempApprov.CurrentNum){}
            else
            {
                List<ApprovalTask> curApprov = tempApprov.InboxStep[Convert.ToInt32(tempApprov.CurrentNum - 1)].InboxTask;

                if (curApprov != null)
                {
                    curApprov.ForEach(app =>
                    {
                        if ((Convert.ToInt32(Session["UserOID"]) == app.PersonOID))
                        {
                            ApprovFlag = CommonConstant.ACTION_YES;
                        }
                    });
                }
            }

            if (approv != null)
            {
                ViewBag.ApprovStatus = DObjectRepository.SelDObject(Session, new DObject { OID = approv.OID }).BPolicy.Name;
            }
            else
            {
                ViewBag.ApprovStatus = null;
            }
            ViewBag.ApprovFlag = ApprovFlag;
            ViewBag.reasonList = reasonList;
            ViewBag.SelassessList = SelassessList;
            ViewBag.originList = originList;
            ViewBag.ECODetail = ECODetail;
            ViewBag.Status = BPolicyRepository.SelBPolicy(new BPolicy { Type = EoConstant.TYPE_CHANGE_ORDER });

            return PartialView("InfoChangeOrder");
        }
        public JsonResult SelChangeOrderAssessList(int OID)
        {
            List<EO> SelassessList = EORepository.SelEOContentsOID(new EO { RootOID = OID, Type = EoConstant.TYPE_ASSESSLIST });  //설계변경의 체크리스트 마스터OID검색
            List<Library> originList = new List<Library>();
            SelassessList.ForEach(item =>
            {
                Library pdata = LibraryRepository.SelAssessLibraryObject(new Library { OID = item.ToOID }); //마스터 검색
                EO AssessManager = EORepository.SelEOContentsObject(new EO { FromOID = item.ToOID, RootOID = OID, Type = EoConstant.TYPE_ASSESSLIST_MANAGER });
                if (AssessManager != null)
                {
                    pdata.ToOID = AssessManager.ToOID;
                    pdata.ManagerNm = PersonRepository.SelPerson(Session, new Person { OID = AssessManager.ToOID }).Name;
                }
                pdata.Cdata = LibraryRepository.SelAssessLibraryChild(new Library { FromOID = pdata.OID }); //마스터 자녀들검색
                originList.Add(pdata);

                List<EO> checkData = EORepository.SelEOContentsOID(new EO { FromOID = item.ToOID, RootOID = OID, Type = EoConstant.TYPE_ASSESSLIST_CHILD });

                item.CheckData = checkData;  //체크리스트 검색
            });

            ViewBag.SelassessList = SelassessList;
            ViewBag.originList = originList;
            var result = new { SelassessList = SelassessList, originList = originList };
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public ActionResult CreateChangeOrder()
        {
            Library reasonKey = LibraryRepository.SelLibraryObject(new Library { Name = CommonConstant.ATTRIBUTE_REASONCHANGE });
            List<Library> reasonList = LibraryRepository.SelLibrary(new Library { FromOID = reasonKey.OID });  //변경 사유 부분
            List<Library> assessList = LibraryRepository.SelAssessLibraryLatest(null);
            assessList.ForEach(item =>
            {
                List<Library> child = LibraryRepository.SelAssessLibraryChild(new Library { FromOID = item.OID });

                item.Cdata = child;
            });

            ViewBag.reasonList = reasonList;
            ViewBag.assessList = assessList;
            return View();
        }

        public ActionResult SearchChangeOrder()
        {
            Library reasonKey = LibraryRepository.SelLibraryObject(new Library { Name = CommonConstant.ATTRIBUTE_REASONCHANGE });
            List<Library> reasonList = LibraryRepository.SelLibrary(new Library { FromOID = reasonKey.OID });  //변경 사유 부분
            ViewBag.BPolicies = BPolicyRepository.SelBPolicy(new BPolicy { Type = EoConstant.TYPE_CHANGE_ORDER });
            ViewBag.reasonList = reasonList;
            return View();
        }
        public ActionResult SearchReleaseChangeOrder()
        {
            Library reasonKey = LibraryRepository.SelLibraryObject(new Library { Name = CommonConstant.ATTRIBUTE_REASONCHANGE });
            List<Library> reasonList = LibraryRepository.SelLibrary(new Library { FromOID = reasonKey.OID });  //변경 사유 부분
            ViewBag.BPolicies = BPolicyRepository.SelBPolicy(new BPolicy { Type = EoConstant.TYPE_CHANGE_ORDER });
            ViewBag.reasonList = reasonList;
            return View();
        }
        public ActionResult InfoChangeOrder(int OID)
        {
            ECO ECODetail = ECORepository.SelChangeOrderObject(Session, new ECO { OID = OID });
            Library reasonKey = LibraryRepository.SelLibraryObject(new Library { Name = CommonConstant.ATTRIBUTE_REASONCHANGE });
            List<Library> reasonList = LibraryRepository.SelLibrary(new Library { FromOID = reasonKey.OID });  //변경 사유 부분
            List<EO> SelassessList = EORepository.SelEOContentsOID(new EO { RootOID = ECODetail.OID, Type = EoConstant.TYPE_ASSESSLIST });  //설계변경의 체크리스트 마스터OID검색
            List<Library> originList = new List<Library>();
            SelassessList.ForEach(item =>
            {
                Library pdata = LibraryRepository.SelAssessLibraryObject(new Library { OID = item.ToOID }); //마스터 검색
                EO AssessManager = EORepository.SelEOContentsObject(new EO { FromOID = item.ToOID, RootOID = ECODetail.OID, Type = EoConstant.TYPE_ASSESSLIST_MANAGER });
                if(AssessManager != null)
                {
                    pdata.ToOID = AssessManager.ToOID;
                    pdata.ManagerNm = PersonRepository.SelPerson(Session,new Person { OID = AssessManager.ToOID }).Name;
                }
                pdata.Cdata = LibraryRepository.SelAssessLibraryChild(new Library { FromOID = pdata.OID }); //마스터 자녀들검색
                originList.Add(pdata);

                List<EO> checkData = EORepository.SelEOContentsOID(new EO { FromOID = item.ToOID,RootOID =ECODetail.OID, Type = EoConstant.TYPE_ASSESSLIST_CHILD });

                item.CheckData = checkData;  //체크리스트 검색
            });
            Approval approv = ApprovalRepository.SelApprovalNonStep(Session, new Approval { TargetOID = Convert.ToInt32(OID) });
            if (approv != null)
            {
                ViewBag.ApprovStatus = DObjectRepository.SelDObject(Session, new DObject { OID = approv.OID }).BPolicy.Name;
            }
            else
            {
                ViewBag.ApprovStatus = null;
            }
            ViewBag.reasonList = reasonList;
            ViewBag.SelassessList = SelassessList;
            ViewBag.originList = originList;
            ViewBag.ECODetail = ECODetail;
            ViewBag.Status = BPolicyRepository.SelBPolicy(new BPolicy { Type = EoConstant.TYPE_CHANGE_ORDER });

            return View();
        }

        public ActionResult dlgSearchEPart(string Type)
        {
            Library ItemKey = LibraryRepository.SelCodeLibraryObject(new Library { Code1 = CommonConstant.ATTRIBUTE_ITEM });
            Library oemKey = LibraryRepository.SelCodeLibraryObject(new Library { Code1 = CommonConstant.ATTRIBUTE_OEM });


            List<Library> ItemList = LibraryRepository.SelCodeLibrary(new Library { FromOID = ItemKey.OID });  //OEM 목록
            List<Library> oemList = LibraryRepository.SelCodeLibrary(new Library { FromOID = oemKey.OID });  //차종 목록
            List<int> partOIDList = EORepository.partOIDList(new EO { Type = Type });
            ViewBag.oemList = oemList;
            ViewBag.ItemList = ItemList;
            ViewBag.partOIDList = partOIDList;
            return PartialView("Dialog/dlgSearchEPart");
        }

        public ActionResult dlgSearchECR(string Type)
        {
            return PartialView("Dialog/dlgSearchECR");
        }

        public PartialViewResult dlgSelectAssessManager()
        {
            ViewBag.Organization = CompanyRepository.SelOrganizationWithPerson(Session, new List<string> { CommonConstant.TYPE_PERSON });
            return PartialView("Dialog/dlgSelectAssessManager");
        }

        public ActionResult dlgEBomStructure(int OID, string Name)
        {
            ViewBag.OID = OID;
            ViewBag.Name = Name;
            return PartialView("Dialog/dlgEBomStructure");
        }

        #region -- Module : ChangeOder

        #region 설계변경 등록
        public JsonResult InsertChangeOrder(ECO _param,List<List<Library>> _assessList, List<EO> _eo)
        {
            int resultOid = 0;
            try
            {
                DaoFactory.BeginTransaction();

                DObject dobj = new DObject();

                var YYYY = DateTime.Now.ToString("yyyy");
                var MM = DateTime.Now.ToString("MM");
                var dd = DateTime.Now.ToString("dd");
                var selName = "WRO" + YYYY + MM + dd + "-001";
                var NewName = "WRO" + YYYY + MM + dd;

                var LateName = ECORepository.SelChangeOrder(Session, new ECO { Name = NewName });

                if (LateName.Count == 0)
                {
                    dobj.Name = selName;
                }
                else
                {
                    int NUM = Convert.ToInt32(LateName.Last().Name.Substring(12, 3)) + 1;
                    dobj.Name = NewName + "-" + string.Format("{0:D3}", NUM);
                }

                dobj.Type = EoConstant.TYPE_CHANGE_ORDER;
                dobj.TableNm = EoConstant.TABLE_CHANGE_ORDER;
                dobj.Description = _param.Description;
                resultOid = DObjectRepository.InsDObject(Session, dobj); //오브젝트 등록
                _param.Type = dobj.Type;
                _param.OID = resultOid;
                DaoFactory.SetInsert("ChangeOrder.InsChangeOrder", _param); //설계변경 등록

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
                EO data = null;
                for (var i = 0; i < _assessList.Count; i++)
                {
                    for (var j = 0; j < _assessList[i].Count; j++)
                    {
                        if (j == 0) //평가표 마스터 등록
                        {
                           if(data!= null)
                            {
                                data = null;
                            }
                            data = new EO();
                            data.RootOID = resultOid;
                            data.FromOID = resultOid;
                            data.ToOID = _assessList[i][0].OID;
                            data.Type = EoConstant.TYPE_ASSESSLIST;
                            EORepository.InsEOContents(Session,data);
                            if(_assessList[i][0].ToOID != null)
                            {
                                if (data != null)
                                {
                                    data = null;
                                }
                                data = new EO();
                                data.RootOID = resultOid;
                                data.FromOID = _assessList[i][0].OID; 
                                data.ToOID = _assessList[i][0].ToOID;
                                data.Type = EoConstant.TYPE_ASSESSLIST_MANAGER;
                                EORepository.InsEOContents(Session, data);
                            }
                        }
                        else //평가표 체크리스트 등록
                        {
                            if (data != null)
                            {
                                data = null;
                            }
                            data = new EO();
                            data.RootOID = resultOid;
                            data.FromOID = _assessList[i][0].OID;
                            data.ToOID = _assessList[i][j].OID;
                            data.Type = EoConstant.TYPE_ASSESSLIST_CHILD;
                            EORepository.InsEOContents(Session,data);
                        }
                    }
                }
                if (_eo != null && _eo.Count > 0)
                {
                    _eo.ForEach(eoData =>
                   {
                       if (eoData != null)
                       {
                           eoData.RootOID = resultOid;
                           EORepository.InsEOContents(Session, eoData);
                       }

                   });
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

        #region 설계변경 검색
        public JsonResult SelChangeOrder(ECO _param)
        {
            List<ECO> lECO = ECORepository.SelChangeOrder(Session, _param);
            return Json(lECO);
        }
        #endregion

        #region 설계변경 EONO 중복검색
        public JsonResult SelChangeOrderCheck(ECO _param)
        {
            _param.Type = EoConstant.TYPE_CHANGE_ORDER;
            ECO lECO = DaoFactory.GetData<ECO>("ChangeOrder.SelChangeOrderCheck", _param);
            if(lECO == null)
            {
                return Json(0);
            }
            return Json(1);
        }
        #endregion

        #region 설계변경 평가표 수정
        public JsonResult UpdateECOAssess(List<List<Library>> _assessList, EO delData)
        {
            try
            {
                DaoFactory.BeginTransaction();
                if (delData != null && delData.Type != null && delData.RootOID != null)
                {
                    EORepository.delEOContents(delData);
                }
                if (_assessList != null && _assessList.Count > 0)
                {
                    EO data = null;
                    for (var i = 0; i < _assessList.Count; i++)
                    {
                        for (var j = 0; j < _assessList[i].Count; j++)
                        {
                            if (j == 0) //평가표 마스터 등록
                            {
                                if (data != null)
                                {
                                    data = null;
                                }
                                data = new EO();
                                data.RootOID = delData.RootOID;
                                data.FromOID = delData.RootOID;
                                data.ToOID = _assessList[i][0].OID;
                                data.Type = EoConstant.TYPE_ASSESSLIST;
                                EORepository.InsEOContents(Session,data);
                                if (_assessList[i][0].ToOID != null)
                                {
                                    if (data != null)
                                    {
                                        data = null;
                                    }
                                    data = new EO();
                                    data.RootOID = delData.RootOID;
                                    data.FromOID = _assessList[i][0].OID;
                                    data.ToOID = _assessList[i][0].ToOID;
                                    data.Type = EoConstant.TYPE_ASSESSLIST_MANAGER;
                                    EORepository.InsEOContents(Session, data);
                                }

                            }
                            else //평가표 체크리스트 등록
                            {
                                if (data != null)
                                {
                                    data = null;
                                }
                                data = new EO();
                                data.RootOID = delData.RootOID;
                                data.FromOID = _assessList[i][0].OID;
                                data.ToOID = _assessList[i][j].OID;
                                data.Type = EoConstant.TYPE_ASSESSLIST_CHILD;
                                EORepository.InsEOContents(Session,data);
                            }
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
            return Json(0);

        }
        #endregion

        #region 설계변경리스트 등록
        public JsonResult InsECOContents(List<EO> _param,EO delData)
         {
            int resultOid = 0;
            try
            {
                DaoFactory.BeginTransaction();
                if (delData != null && delData.Type != null&&delData.RootOID!=null)
                {
                    EORepository.delEOContents(delData);
                }
                //dobj.TdmxOID = DObjectRepository.SelTdmxOID(new DObject { Type = DocumentContant.TYPE_DOCUMENT });
                if (_param != null && _param.Count > 0)
                {
                    _param.ForEach(obj =>
                  {
                      if (obj != null)
                      {
                          EORepository.InsEOContents(Session,obj);
                      }
                  });
                }

                //_param.DocType = _param.DocType;
                //_param.Title = _param.Title;
                //_param.Eo_No = _param.Eo_No;
               

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

        #region 설계변경리스트 검색
        public JsonResult SelEOContents(EO _param)
        {
            List<EO> lEO = EORepository.SelEOContentsOID(_param);
            List<EPart> lEPart = new List<EPart>();
            if (_param.Type == Common.Constant.EoConstant.TYPE_EBOM_LIST)
            {

                lEO.ForEach(obj =>
                {
                    if (obj != null)
                    {
                        EPart eobj = EPartRepository.SelEPartObject(Session, new EPart { OID = obj.ToOID });
                        if (eobj != null)
                        {
                            eobj.RootOID = _param.RootOID;
                            eobj.Type = _param.Type;
                            eobj.ToOID = eobj.OID;
                            lEPart.Add(eobj);
                        }
                    }
                });
                return Json(lEPart);
            }
            else
            {
                return Json(lEO);
            }

        }
        #endregion

        #region 설계변경리스트 삭제
        public JsonResult DelEOContents(List<EO> _param)
        {
            int resultOid = 0;
            try
            {
                DaoFactory.BeginTransaction();
                if (_param != null && _param.Count > 0)
                {
                    _param.ForEach(obj =>
                    {
                        if (obj != null)
                        {
                            EORepository.delEOContents(obj);
                        }
                    });
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

        #region 설계변경 수정
        public JsonResult UdtChangeOrder(ECO _param)
        {
            int result = 0;
            try
            {
                DaoFactory.BeginTransaction();

                DObjectRepository.UdtDObject(Session, _param);
                ECORepository.UdtChangeOrderObject(_param);

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
        #endregion

        #region 연관ECR 검색
        public JsonResult SelECRRelation(EO _param)
        {
            List<EO> lEO = EORepository.SelEOContentsOID(_param);

            List<ECR> lECR = EORepository.SelECRRelation(lEO);
            return Json(lECR);
        }
        #endregion

        #endregion
    }
}