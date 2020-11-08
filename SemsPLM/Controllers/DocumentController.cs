using Common.Constant;
using Common.Factory;
using Common.Models;
using Document.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SemsPLM.Controllers
{
    public class DocumentController : Controller
    {
        // GET: Document
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult SearchDocument()
        {
            return View();
        }

        public ActionResult InfoDocument(int OID)
        {
            Doc docDetail = DocRepository.SelDocObject(new Doc { OID = OID });
            if (docDetail.Doc_Lib_Lev1_OID != null)
            {
                docDetail.Doc_Lib_Lev1_KorNm = LibraryRepository.SelLibraryObject(new Library { OID = docDetail.Doc_Lib_Lev1_OID }).KorNm;

            }
            if (docDetail.Doc_Lib_Lev2_OID != null)
            {
                docDetail.Doc_Lib_Lev2_KorNm = LibraryRepository.SelLibraryObject(new Library { OID = docDetail.Doc_Lib_Lev2_OID }).KorNm;

            }
            if (docDetail.Doc_Lib_Lev3_OID != null)
            {
                docDetail.Doc_Lib_Lev3_KorNm = LibraryRepository.SelLibraryObject(new Library { OID = docDetail.Doc_Lib_Lev3_OID }).KorNm;

            }
            ViewBag.docDetail = docDetail;
            ViewBag.Status = BPolicyRepository.SelBPolicy(new BPolicy { Type = DocumentContant.TYPE_DOCUMENT });
            return View();
        }

        public ActionResult CreateDocument()
        {
            Library oemKey = LibraryRepository.SelLibraryObject(new Library { Name = "OEM" }); 
            List<Library> oemList = LibraryRepository.SelLibrary(new Library { FromOID= oemKey.OID});  //OEM 목록
            ViewBag.oemList = oemList;

            Library tdocKey = LibraryRepository.SelLibraryObject(new Library { Name = "TDOC" });
            List<Library> tdocList = LibraryRepository.SelLibrary(new Library { FromOID = tdocKey.OID }); //TDOC 목록
            ViewBag.tdocList = tdocList;

            Library pdocKey = LibraryRepository.SelLibraryObject(new Library { Name = "PDOC" });
            List<Library> pdocList = LibraryRepository.SelLibrary(new Library { FromOID = pdocKey.OID }); //PDOC 목록
            ViewBag.pdocList = pdocList;

            return View();
        }
        #region -- Module : Document
        #region 문서 등록
        public JsonResult InsertDocument(Doc _param)
        {
            int resultOid = 0;
            try
            {
                DaoFactory.BeginTransaction();
            //    DObjectRepository.UdtLatestDObject(new DObject { OID = _param.OID });

                DObject dobj = new DObject();
                dobj.Type = DocumentContant.TYPE_DOCUMENT;
                dobj.TableNm = DocumentContant.TABLE_DOCUMENT;
                dobj.Name = _param.Name;
                dobj.Description = _param.Description;
                //dobj.TdmxOID = DObjectRepository.SelTdmxOID(new DObject { Type = DocumentContant.TYPE_DOCUMENT });
                resultOid = DObjectRepository.InsDObject(dobj);

                _param.OID = resultOid;
                //_param.DocType = _param.DocType;
                //_param.Title = _param.Title;
                //_param.Eo_No = _param.Eo_No;
                DaoFactory.SetInsert("Doc.InsDoc", _param);

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

        #region 문서검색
        public JsonResult SelDoc(Doc _param)
        {
            List<Doc> lDoc = DocRepository.SelDoc(_param);
            return Json(lDoc);
        }
        #endregion

        #region 문서 수정
        public JsonResult UdtDocument(Doc _param)
        {
            int result = 0;
            try
            {
                DaoFactory.BeginTransaction();

                DObjectRepository.UdtDObject(_param);
                DocRepository.UdtDocObject(_param);

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