using ChangeOrder.Models;
using ChangeRequest.Models;
using Common.Constant;
using Common.Factory;
using Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SemsPLM.Controllers
{
    public class ChangeRequestController : Controller
    {
        // GET: ChangeRequest
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult CreateChangeRequest()
        {
            Library ECRreasonKey = LibraryRepository.SelLibraryObject(new Library { Name = "ReasonChange" });
            List<Library> ECRreasonList = LibraryRepository.SelLibrary(new Library { FromOID = ECRreasonKey.OID });
            ViewBag.ECRreasonList = ECRreasonList;
            return View();
        }

        public ActionResult SearchChangeRequest()
        {
            //Library oemKey = LibraryRepository.SelLibraryObject(new Library { Name = "OEM" });
            //List<Library> oemList = LibraryRepository.SelLibrary(new Library { FromOID = oemKey.OID });  //OEM 목록
            //ViewBag.oemList = oemList;
            return View();
        }

        public ActionResult InfoChangeRequest(int OID)
        {
            ECR ECRDetail = ECRRepository.SelChangeRequestObject(new ECR { OID = OID });
            ViewBag.ECRDetail = ECRDetail;
            ViewBag.Status = BPolicyRepository.SelBPolicy(new BPolicy { Type = EoConstant.TYPE_CHANGE_REQUEST });

            Library ECRreasonKey = LibraryRepository.SelLibraryObject(new Library { Name = "ReasonChange" });
            List<Library> ECRreasonList = LibraryRepository.SelLibrary(new Library { FromOID = ECRreasonKey.OID });
            ViewBag.ECRreasonList = ECRreasonList;


            return View();
        }
        public ActionResult dlgSearchECO()
        {
            Library oemKey = LibraryRepository.SelCodeLibraryObject(new Library { Code1 = "OEM" });
            List<Library> oemList = LibraryRepository.SelCodeLibrary(new Library { FromOID = oemKey.OID });  //OEM 목록
            ViewBag.oemList = oemList;
            return PartialView("Dialog/dlgSearchECO");
        }

        #region 변경요청 등록
        public JsonResult InsertChangeRequest(ECR _param, List<EO> ECR_STATEMENT, List<EO> ECRRelatedData)
        {
            int resultOid = 0;
            try
            {
                
                DaoFactory.BeginTransaction();
                
                var YYYY = DateTime.Now.ToString("yyyy");
                var MM = DateTime.Now.ToString("MM");
                var dd = DateTime.Now.ToString("dd");

                DObject dobj = new DObject();
                dobj.Type = EoConstant.TYPE_CHANGE_REQUEST;
                dobj.TableNm = EoConstant.TABLE_CHANGE_REQUEST;

                var selName = "WR" + YYYY + MM +  dd + "-001";
                var NewName = "WR" + YYYY + MM +  dd;

                var LateName = ECRRepository.SelChangeRequest(new ECR { Name = NewName });

                if(LateName.Count == 0)
                {
                    dobj.Name = selName;
                }
                else
                {
                    int NUM = Convert.ToInt32(LateName.Last().Name.Substring(11, 3)) + 1;
                    dobj.Name = NewName + "-" + string.Format("{0:D3}", NUM);
                }

                dobj.Description = _param.Description;
                resultOid = DObjectRepository.InsDObject(Session, dobj);


                _param.OID = resultOid;
                DaoFactory.SetInsert("ChangeRequest.InsChangeRequest", _param);

                if (ECR_STATEMENT != null && ECR_STATEMENT.Count > 0)
                {
                    ECR_STATEMENT.ForEach(obj =>
                    {
                        if (obj != null)
                        {
                            obj.RootOID = resultOid;
                            EORepository.InsEOContents(Session,obj);
                        }
                    });
                }

                if (ECRRelatedData != null && ECRRelatedData.Count > 0)
                {
                    ECRRelatedData.ForEach(obj =>
                    {
                        if (obj != null)
                        {
                            EO RelatedData = new EO();
                            RelatedData.RootOID = resultOid;
                            RelatedData.ToOID = obj.OID;
                            RelatedData.Type = Common.Constant.EoConstant.TYPE_ECO_RELATION;
                            EORepository.InsEOContents(Session,RelatedData);
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

        #region 변경요청 검색
        public JsonResult SelChangeRequest(ECR _param)
        {
            List<ECR> lECR = ECRRepository.SelChangeRequest(_param);
            return Json(lECR);
        }
        #endregion

        #region 변경요청 수정
        public JsonResult UdtEcrObj (ECR _param)
        {
            DObjectRepository.UdtDObject(Session, _param);
            ECRRepository.UdtChangeRequest(_param);
            return Json(0);
        }
        #endregion

        #region 변경요청 변경요청내역 등록
        public JsonResult InsECRContents(List<EO> _param, EO delData)
        {
            int resultOid = 0;
            try
            {
                DaoFactory.BeginTransaction();
                if (delData != null && delData.Type != null && delData.RootOID != null)
                {
                    EORepository.delEOContents(delData);
                }

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

        #region 변경요청 변경요청내역 검색
        public JsonResult SelEOContents(EO _param)
        {
            List<EO> lEO = EORepository.SelEOContentsOID(_param);
            List<ECO> lECO = new List<ECO>();
            if (_param.Type == Common.Constant.EoConstant.TYPE_ECO_RELATION)
            {
                lEO.ForEach(obj =>
                {
                    if (obj != null)
                    {
                        ECO eobj = ECORepository.SelChangeOrderObject(new ECO { OID = obj.ToOID });
                        //eobj.RootOID = _param.RootOID;
                        //eobj.Type = _param.Type;
                        //eobj.ToOID = eobj.OID;
                        //eobj.OID = obj.OID;
                        lECO.Add(eobj);
                    }
                });
                return Json(lECO);
            }
            return Json(lEO);
        }
        #endregion

        #region 변경요청 연관 ECO
        public JsonResult InsECRRelationContents(int? RootOID, List<EO> _param)
        {
            int resultOid = 0;
            try
            {
                DaoFactory.BeginTransaction();

                EO lEO = new EO();
                if (_param != null && _param.Count > 0)
                {
                    _param.ForEach(obj =>
                    {
                        if (obj != null)
                        {
                            lEO.RootOID = RootOID;
                            lEO.ToOID = obj.OID;
                            lEO.Type = Common.Constant.EoConstant.TYPE_ECO_RELATION;
                            EORepository.InsEOContents(Session,lEO);
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

        #region 변경요청 연관 ECO 삭제
        public JsonResult DelEOContents(EO Param, List<ECO> DelData)
        {
            int resultOid = 0;
            try
            {
                DaoFactory.BeginTransaction();

                EO lEO = new EO();
                if (DelData != null && DelData.Count > 0)
                {
                    DelData.ForEach(obj =>
                    {
                        if (obj != null)
                        {
                            lEO.RootOID = Param.RootOID;
                            lEO.ToOID = obj.OID;
                            lEO.Type = Param.Type;
                            EORepository.delEOContents(lEO);
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
    }
}