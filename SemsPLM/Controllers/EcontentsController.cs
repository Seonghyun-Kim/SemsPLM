using Common;
using Common.Constant;
using Common.Factory;
using Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Econtents.Models;

namespace SemsPLM.Controllers
{
    public class EcontentsController : Controller
    {
        // GET: Econtents
        #region ProblemsLibrary View
        public ActionResult CreateProblemsLibrary()
        {
            Library oemKey = LibraryRepository.SelCodeLibraryObject(new Library { Code1 = CommonConstant.ATTRIBUTE_OEM });
            List<Library> oemList = LibraryRepository.SelCodeLibrary(new Library { FromOID = oemKey.OID });  //차종 목록
            ViewBag.oemList = oemList;
            return View();
        }
        public ActionResult SearchProblemsLibrary()
        {
            Library oemKey = LibraryRepository.SelCodeLibraryObject(new Library { Code1 = CommonConstant.ATTRIBUTE_OEM });
            List<Library> oemList = LibraryRepository.SelCodeLibrary(new Library { FromOID = oemKey.OID });  //차종 목록
            ViewBag.oemList = oemList;
            return View();
        }
        public ActionResult InfoProblemsLibrary(int OID)
        {
            ProblemsLibrary ProblemsLibraryDetail = ProblemsLibraryRepository.SelProblemsLibraryObject(new ProblemsLibrary { OID = OID });
            ViewBag.OID = ProblemsLibraryDetail.OID;
            ViewBag.ProblemsLibraryDetail = ProblemsLibraryDetail;
            ViewBag.Status = BPolicyRepository.SelBPolicy(new BPolicy { Type = EcontentsConstant.TYPE_PROBLEMS_LIBRARY });
            return View(ProblemsLibraryDetail);
        }
        #endregion

        #region OptimalDesign View
        public ActionResult CreateOptimalDesign()
        {
            Library oemKey = LibraryRepository.SelCodeLibraryObject(new Library { Code1 = CommonConstant.ATTRIBUTE_OEM });
            List<Library> oemList = LibraryRepository.SelCodeLibrary(new Library { FromOID = oemKey.OID });  //차종 목록
            ViewBag.oemList = oemList;
            return View();
        }
        public ActionResult SearchOptimalDesign()
        {
            Library oemKey = LibraryRepository.SelCodeLibraryObject(new Library { Code1 = CommonConstant.ATTRIBUTE_OEM });
            List<Library> oemList = LibraryRepository.SelCodeLibrary(new Library { FromOID = oemKey.OID });  //차종 목록
            ViewBag.oemList = oemList;
            return View();
        }
        #endregion

        #region ProblemsLibrary 검색
        public JsonResult SelProblemsLibrary(ProblemsLibrary _param)
        {
            List<ProblemsLibrary> ProblemsLibrary = ProblemsLibraryRepository.SelProblemsLibrary(_param);
            return Json(ProblemsLibrary);
        }
        #endregion

        #region ProblemsLibrary 등록
        public JsonResult InsProblemsLibrary(ProblemsLibrary _param)
        {
            int resultOid = 0;

            try
            {
                DaoFactory.BeginTransaction();

                List<ProblemsLibrary> ProblemsLibrary = ProblemsLibraryRepository.SelProblemsLibrary(_param);

                DObject dobj = new DObject();
                dobj.Type = EcontentsConstant.TYPE_PROBLEMS_LIBRARY;
                dobj.TableNm = EcontentsConstant.TABLE_PROBLEMS_LIBRARY;
                dobj.Name = (ProblemsLibrary.Count + 1).ToString();
                dobj.Description = _param.Description;
                
                resultOid = DObjectRepository.InsDObject(Session, dobj);

                _param.OID = resultOid;
                DaoFactory.SetInsert("Econtents.InsProblemsLibrary", _param);

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