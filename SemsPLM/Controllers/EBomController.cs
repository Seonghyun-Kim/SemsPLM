using Common;
using Common.Constant;
using Common.Factory;
using Common.Models;
using EBom.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Web;
using System.Web.Mvc;

namespace SemsPLM.Controllers
{
    public class EBomController : Controller
    {
        // GET: EBom
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult EBomStructure()
        {
            return View();
        }
        public ActionResult CreateEPart()
        {
            Library oemKey = LibraryRepository.SelLibraryObject(new Library { Name = "OEM" });
            Library psizeKey = LibraryRepository.SelLibraryObject(new Library { Name = "PSIZE" });
            Library prodstrKey = LibraryRepository.SelLibraryObject(new Library { Name = "PROD_STRUCTURE" });
            
            List<Library> oemList = LibraryRepository.SelLibrary(new Library { FromOID = oemKey.OID });  //OEM 목록
            List<Library> psizeList = LibraryRepository.SelLibrary(new Library { FromOID = psizeKey.OID });
            List<Library> prodstrList = LibraryRepository.SelLibrary(new Library { FromOID = prodstrKey.OID });

            ViewBag.oemList = oemList;
            ViewBag.psizeList = psizeList;
            ViewBag.prodstrList = prodstrList;

            return View();
        }
        public ActionResult SearchEPart()
        {
            Library oemKey = LibraryRepository.SelLibraryObject(new Library { Name = "OEM" });
            List<Library> oemList = LibraryRepository.SelLibrary(new Library { FromOID = oemKey.OID });  //OEM 목록
            ViewBag.oemList = oemList;
            return View();
        }
        public ActionResult InfoEPart(int OID)
        {
            ViewBag.OID = OID;
            EPart InfoEPart = EPartRepository.SelEPartObject(new EPart { OID = OID });

            Library MaterialKey = LibraryRepository.SelLibraryObject(new Library { Name = "PSIZE" });
            List<Library> MaterialList = LibraryRepository.SelLibrary(new Library { FromOID = MaterialKey.OID });
            ViewBag.MaterialList = MaterialList;

            Library prodstrKey = LibraryRepository.SelLibraryObject(new Library { Name = "PROD_STRUCTURE" });
            List<Library> prodstrList = LibraryRepository.SelLibrary(new Library { FromOID = prodstrKey.OID });
            ViewBag.prodstrList = prodstrList;

            if (InfoEPart.Prod_Lib_Lev2_OID != null && InfoEPart.Prod_Lib_Lev2_OID > 0)
            {
                ViewBag.PROD_LIBLv2 = LibraryRepository.SelLibrary(new Library { FromOID = InfoEPart.Prod_Lib_Lev1_OID });
            }
            if (InfoEPart.Prod_Lib_Lev3_OID != null && InfoEPart.Prod_Lib_Lev3_OID > 0)
            {
                ViewBag.PROD_LIBLv3 = LibraryRepository.SelLibrary(new Library { FromOID = InfoEPart.Prod_Lib_Lev2_OID });
            }

            ViewBag.Status = BPolicyRepository.SelBPolicy(new BPolicy { Type = EBomConstant.TYPE_PART });

            return View(InfoEPart);
        }
        public ActionResult dlgSearchEBomStructure(int? OID)
        {
            Library oemKey = LibraryRepository.SelLibraryObject(new Library { Name = "OEM" });
            List<Library> oemList = LibraryRepository.SelLibrary(new Library { FromOID = oemKey.OID });  //OEM 목록
            ViewBag.oemList = oemList;
            ViewBag.OID = OID;
            return PartialView("Dialog/dlgSearchEBomStructure");
        }

        public ActionResult CompareEPart()
        {
            return View();
        }

        public ActionResult dlgSearchEPart()
        {
            return PartialView("Dialog/dlgSearchEPart");
        }
        

        #region EPart 검색
        public JsonResult SelEPart(EPart _param)
        {
            List<EPart> Epart = EPartRepository.SelEPart(_param);
            return Json(Epart);
        }
        #endregion

        #region EPart 등록
        public JsonResult InsEPart(EPart _param)
        {
            int resultOid = 0;
            
            try
            {
                DaoFactory.BeginTransaction();
                //DObjectRepository.UdtLatestDObject(new DObject { OID = _param.OID });

                DObject dobj = new DObject();
                dobj.Type = EBomConstant.TYPE_PART;
                dobj.TableNm = EBomConstant.TABLE_PART;
                dobj.Name = _param.Name;
                dobj.Description = _param.Description;

                resultOid = DObjectRepository.InsDObject(dobj);

                _param.OID = resultOid;

                //_param.Title              = _param.Title;
                //_param.Rep_Part_No        = _param.Rep_Part_No;
                //_param.Rep_Part_No2       = _param.Rep_Part_No2;
                //_param.Eo_No              = _param.Eo_No;
                //_param.Eo_No_ApplyDate    = _param.Eo_No_ApplyDate;
                //_param.Eo_No_History      = _param.Eo_No_History;
                //_param.Etc                = _param.Etc;
                //_param.ApprovOID          = _param.ApprovOID;
                //_param.EPartType          = _param.EPartType;
                //_param.Sel_Eo             = _param.Sel_Eo;
                //_param.Sel_Eo_Date        = _param.Sel_Eo_Date;
                //_param.Spec               = _param.Spec;
                //_param.Surface            = _param.Surface;

                DaoFactory.SetInsert("EBom.InsEPart", _param);

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

        #region EPart 정전개
        public JsonResult SelectEBom(EPart _param)
        {
            EBOM lBom = EPartRepository.getListEbom(0, Convert.ToInt32(_param.OID));
            return Json(lBom);
        }
        #endregion

        #region EPart 역전개
        public JsonResult SelectReverseEBom(EPart _param)
        {
            EBOM lBom = EPartRepository.getListReverseEbom(0, Convert.ToInt32(_param.OID));
            return Json(lBom);
        }
        #endregion

        #region EPart 중복체크
        public JsonResult CreateEPartChk(EPart _param)
        {
            int result = 0;
            EPart Epart = EPartRepository.ChkEPart(_param);
            if(Epart != null)
            {
                result = 1;
            }
            return Json(result);
        }
        #endregion

        #region EPart 수정
        public JsonResult UdtEPartObj(EPart _param)
        {
            int result = 0;
            DObjectRepository.UdtDObject(_param);
            EPartRepository.UdtEPartObject(_param);


            return Json(result);
        }
        //UdtDObject
        #endregion


        #region EPart 하위 리스트
        public JsonResult SelRootChildList(EPart _param)
        {
            List<EPart> Epart = EPartRepository.SelRootChildList(_param);
            return Json(Epart);
        }
        #endregion

        #region EBOM 편집 추가
        public JsonResult SelectEBomAddChild(EPart _param)
        {
            List<EBOM> lBom = EPartRepository.getListEbomAddChild(0, _param.Name, _param);
            return Json(lBom);
        }
        #endregion

        #region EBOM 편집 저장
        public JsonResult EditStructure(List<EBOM> _param)
        {
            if(_param == null)
            {
                return Json(1);
            }

            foreach(EBOM Data in _param)
            {
                if(Data.Action == "A")
                {
                    EBomRepository.AddAction(Data);
                }
            }
            _param.RemoveAll(VALUE => VALUE.Action == "A");

            foreach (EBOM Data in _param)
            {
                if (Data.Action == "RU")
                {
                    EBomRepository.RuAction(Data);
                }
            }
            _param.RemoveAll(VALUE => VALUE.Action == "RU");

            foreach (EBOM Data in _param)
            {
                if (Data.Action == "D")
                {
                    EBomRepository.DeleteAction(Data);
                }
            }
            return Json(0);
        }
        #endregion
        



    }
}