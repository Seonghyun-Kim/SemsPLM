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
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult CreateProblemsLibrary()
        {
            return View();
        }
        public ActionResult SearchProblemsLibrary()
        {
            Library carKey = LibraryRepository.SelCodeLibraryObject(new Library { Code1 = "CARTYPE" });
            List<Library> carList = LibraryRepository.SelCodeLibrary(new Library { FromOID = carKey.OID });  //차종 목록
            ViewBag.carList = carList;
            return View();
        }
        public ActionResult InfoProblemsLibrary(int OID)
        {
            ProblemsLibrary ProblemsLibraryDetail = ProblemsLibraryRepository.SelProblemsLibraryObject(new ProblemsLibrary { OID = OID });
            ViewBag.OID = ProblemsLibraryDetail.OID;
            ViewBag.ProblemsLibraryDetail = ProblemsLibraryDetail;
            ViewBag.Status = BPolicyRepository.SelBPolicy(new BPolicy { Type = EcontentsConstant.TYPE_ECONTENTS });
            return View(ProblemsLibraryDetail);
        }
        public ActionResult CreateOptimalDesign()
        {
            return View();
        }
        public ActionResult SearchOptimalDesign()
        {
            return View();
        }

        #region ProblemsLibrary 검색
        public JsonResult SelProblemsLibrary(ProblemsLibrary _param)
        {
            List<ProblemsLibrary> ProblemsLibrary = ProblemsLibraryRepository.SelProblemsLibrary(_param);
            return Json(ProblemsLibrary);
        }
        #endregion


        public JsonResult InsProblemsLibrary(ProblemsLibrary _param)
        {
            int resultOid = 0;

            try
            {
                DaoFactory.BeginTransaction();

                DObject dobj = new DObject();
                dobj.Type = EcontentsConstant.TYPE_ECONTENTS;
                dobj.TableNm = EcontentsConstant.TABLE_PROBLEMS_LIBRARY;
                dobj.Name = _param.Name;
                dobj.Description = _param.Description;
                
                resultOid = DObjectRepository.InsDObject(dobj);

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
        
    }
}