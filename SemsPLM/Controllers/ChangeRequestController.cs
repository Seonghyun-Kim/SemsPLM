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
            Library oemKey = LibraryRepository.SelLibraryObject(new Library { Name = "OEM" });
            List<Library> oemList = LibraryRepository.SelLibrary(new Library { FromOID = oemKey.OID });  
            ViewBag.oemList = oemList;

            return View();
        }

        public ActionResult SearchChangeRequest()
        {
            Library oemKey = LibraryRepository.SelLibraryObject(new Library { Name = "OEM" });
            List<Library> oemList = LibraryRepository.SelLibrary(new Library { FromOID = oemKey.OID });  //OEM 목록
            ViewBag.oemList = oemList;
            return View();
        }

        public ActionResult InfoChangeRequest(int OID)
        {
            ECR ECRDetail = ECRRepository.SelChangeRequestObject(new ECR { OID = OID });
            ViewBag.ECRDetail = ECRDetail;
            ViewBag.Status = BPolicyRepository.SelBPolicy(new BPolicy { Type = EoConstant.TYPE_CHANGE_REQUEST });
            return View();
        }

        #region 변경요청 등록
        public JsonResult InsertChangeRequest(ECR _param)
        {
            int resultOid = 0;
            try
            {
                
                DaoFactory.BeginTransaction();
                
                var YYYY = DateTime.Now.ToString("yyyy");
                var MM = DateTime.Now.ToString("MM");

                var YY = YYYY.Substring(2, 2);

                DObject dobj = new DObject();
                dobj.Type = EoConstant.TYPE_CHANGE_REQUEST;
                dobj.TableNm = EoConstant.TABLE_CHANGE_REQUEST;

                var selName = "ECR-" + YY + MM;

                var LateName = ECRRepository.SelChangeRequest(new ECR { Name = selName });

                if(LateName.Count == 0)
                {
                    dobj.Name = "ECR-" + YY + MM + "-001";
                }
                else
                {
                    int NUM = Convert.ToInt32(LateName.Last().Name.Substring(9, 3)) + 1;
                    dobj.Name = "ECR-" + YY + MM + "-" + string.Format("{0:D3}", NUM);
                }

                dobj.Description = _param.Description;
                resultOid = DObjectRepository.InsDObject(dobj);


                _param.OID = resultOid;
                DaoFactory.SetInsert("ChangeRequest.InsChangeRequest", _param);
        
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
            List<ECR> lECO = ECRRepository.SelChangeRequest(_param);
            return Json(lECO);
        }
        #endregion

        #region 변경요청 수정
        public JsonResult UdtEcrObj (ECR _param)
        {
            DObjectRepository.UdtDObject(_param);
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
                            EORepository.InsEOContents(obj);
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
            return Json(lEO);
        }
        #endregion

    }
}