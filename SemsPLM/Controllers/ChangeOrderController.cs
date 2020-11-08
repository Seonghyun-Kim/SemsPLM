using Common.Constant;
using Common.Factory;
using Common.Models;
using System;
using ChangeOrder.Models;
using System.Collections.Generic;
using System.Web.Mvc;
using EBom.Models;

namespace SemsPLM.Controllers
{
    public class ChangeOrderController : Controller
    {
        // GET: ChangeOrder
        public ActionResult CreateChangeOrder()
        {
            Library oemKey = LibraryRepository.SelLibraryObject(new Library { Name = "OEM" });
            List<Library> oemList = LibraryRepository.SelLibrary(new Library { FromOID = oemKey.OID });  //OEM 목록
            ViewBag.oemList = oemList;

            return View();
        }

        public ActionResult SearchChangeOrder()
        {
            Library oemKey = LibraryRepository.SelLibraryObject(new Library { Name = "OEM" });
            List<Library> oemList = LibraryRepository.SelLibrary(new Library { FromOID = oemKey.OID });  //OEM 목록
            ViewBag.oemList = oemList;
            return View();
        }

        public ActionResult InfoChangeOrder(int OID)
        {
            ECO ECODetail = ECORepository.SelChangeOrderObject(new ECO { OID = OID });
            ViewBag.ECODetail = ECODetail;
            ViewBag.Status = BPolicyRepository.SelBPolicy(new BPolicy { Type = EoConstant.TYPE_CHANGE_ORDER });
            return View();
        }

        public ActionResult dlgSearchEPart(string Type)
        {
            Library oemKey = LibraryRepository.SelLibraryObject(new Library { Name = "OEM" });
            List<Library> oemList = LibraryRepository.SelLibrary(new Library { FromOID = oemKey.OID });  //OEM 목록
            List<int> partOIDList = EORepository.partOIDList(new EO { Type = Type });
            ViewBag.partOIDList = partOIDList;
            ViewBag.oemList = oemList;
            return PartialView("Dialog/dlgSearchEPart");
        }


        #region -- Module : ChangeOder

        #region 설계변경 등록
        public JsonResult InsertChangeOrder(ECO _param)
        {
            int resultOid = 0;
            try
            {
                DaoFactory.BeginTransaction();
         //       DObjectRepository.UdtLatestDObject(new DObject { OID = _param.OID });

                DObject dobj = new DObject();
                dobj.Type = EoConstant.TYPE_CHANGE_ORDER;
                dobj.TableNm = EoConstant.TABLE_CHANGE_ORDER;
                dobj.Name = _param.Name;
                dobj.Description = _param.Description;
                //dobj.TdmxOID = DObjectRepository.SelTdmxOID(new DObject { Type = DocumentContant.TYPE_DOCUMENT });
                resultOid = DObjectRepository.InsDObject(dobj);

                _param.OID = resultOid;
                //_param.DocType = _param.DocType;
                //_param.Title = _param.Title;
                //_param.Eo_No = _param.Eo_No;
                DaoFactory.SetInsert("ChangeOrder.InsChangeOrder", _param);

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
            List<ECO> lECO = ECORepository.SelChangeOrder(_param);
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
                          EORepository.InsEOContents(obj);
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
            if (_param.Type == Common.Constant.EoConstant.TYPE_EBOM_LIST || _param.Type == Common.Constant.EoConstant.TYPE_MBOM_LIST)
            {
                
                lEO.ForEach(obj =>
                {
                    if (obj != null)
                    {
                        EPart eobj = EPartRepository.SelEPartObject(new EPart { OID = obj.ToOID });
                        eobj.RootOID = _param.RootOID;
                        eobj.Type = _param.Type;
                        eobj.ToOID = eobj.OID;
                        lEPart.Add(eobj);
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

                DObjectRepository.UdtDObject(_param);
                ECORepository.UdtChangeOrderObject(_param);

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
    }
}