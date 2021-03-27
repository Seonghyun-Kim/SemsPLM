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
using SemsPLM.Filter;
using Common.Models.File;

namespace SemsPLM.Controllers
{
    [AuthorizeFilter]
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

        public ActionResult SearchReleaseProblemsLibrary()
        {
            Library oemKey = LibraryRepository.SelCodeLibraryObject(new Library { Code1 = CommonConstant.ATTRIBUTE_OEM });
            List<Library> oemList = LibraryRepository.SelCodeLibrary(new Library { FromOID = oemKey.OID });  //차종 목록
            ViewBag.oemList = oemList;
            return View();
        }

        public ActionResult InfoMiniProblemsLibrary(int OID)
        {
            ProblemsLibrary ProblemsLibraryDetail = ProblemsLibraryRepository.SelProblemsLibraryObject(Session, new ProblemsLibrary { OID = OID });
            ViewBag.OID = ProblemsLibraryDetail.OID;
            ViewBag.ProblemsLibraryDetail = ProblemsLibraryDetail;
            ViewBag.Status = BPolicyRepository.SelBPolicy(new BPolicy { Type = EcontentsConstant.TYPE_PROBLEMS_LIBRARY });

            Library oemKey = LibraryRepository.SelCodeLibraryObject(new Library { Code1 = CommonConstant.ATTRIBUTE_OEM });
            List<Library> oemList = LibraryRepository.SelCodeLibrary(new Library { FromOID = oemKey.OID });

            ViewBag.oemList = oemList;

            return PartialView("InfoProblemsLibrary");
        }


        public ActionResult InfoProblemsLibrary(int OID)
        {
            ProblemsLibrary ProblemsLibraryDetail = ProblemsLibraryRepository.SelProblemsLibraryObject(Session, new ProblemsLibrary { OID = OID });
            ViewBag.OID = ProblemsLibraryDetail.OID;
            ViewBag.ProblemsLibraryDetail = ProblemsLibraryDetail;
            ViewBag.Status = BPolicyRepository.SelBPolicy(new BPolicy { Type = EcontentsConstant.TYPE_PROBLEMS_LIBRARY });

            Library oemKey = LibraryRepository.SelCodeLibraryObject(new Library { Code1 = CommonConstant.ATTRIBUTE_OEM });
            List<Library> oemList = LibraryRepository.SelCodeLibrary(new Library { FromOID = oemKey.OID });

            ViewBag.oemList = oemList;

            return View();
        }

        #region ProblemsLibrary 검색
        public JsonResult SelProblemsLibrary(ProblemsLibrary _param)
        {
            List<ProblemsLibrary> ProblemsLibrary = ProblemsLibraryRepository.SelProblemsLibrary(Session, _param);
            return Json(ProblemsLibrary);
        }
        #endregion

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

        public ActionResult InfoMiniOptimalDesign(int OID)
        {
            OptimalDesign OptimalDesignDetail = OptimalDesignRepository.SelOptimalDesignObject(Session, new OptimalDesign { OID = OID });
            ViewBag.OID = OptimalDesignDetail.OID;
            ViewBag.OptimalDesignDetail = OptimalDesignDetail;
            ViewBag.Status = BPolicyRepository.SelBPolicy(new BPolicy { Type = EcontentsConstant.TYPE_OPTIMAL_DESIGN });

            Library oemKey = LibraryRepository.SelCodeLibraryObject(new Library { Code1 = CommonConstant.ATTRIBUTE_OEM });
            List<Library> oemList = LibraryRepository.SelCodeLibrary(new Library { FromOID = oemKey.OID });

            ViewBag.oemList = oemList;

            return PartialView("InfoOptimalDesign");
        }

        public ActionResult SearchReleaseOptimalDesign()
        {
            Library oemKey = LibraryRepository.SelCodeLibraryObject(new Library { Code1 = CommonConstant.ATTRIBUTE_OEM });
            List<Library> oemList = LibraryRepository.SelCodeLibrary(new Library { FromOID = oemKey.OID });  //차종 목록
            ViewBag.oemList = oemList;
            return View();
        }

        public ActionResult InfoOptimalDesign(int OID)
        {
            OptimalDesign OptimalDesignDetail = OptimalDesignRepository.SelOptimalDesignObject(Session, new OptimalDesign { OID = OID });
            ViewBag.OID = OptimalDesignDetail.OID;
            ViewBag.OptimalDesignDetail = OptimalDesignDetail;
            ViewBag.Status = BPolicyRepository.SelBPolicy(new BPolicy { Type = EcontentsConstant.TYPE_OPTIMAL_DESIGN });

            Library oemKey = LibraryRepository.SelCodeLibraryObject(new Library { Code1 = CommonConstant.ATTRIBUTE_OEM });
            List<Library> oemList = LibraryRepository.SelCodeLibrary(new Library { FromOID = oemKey.OID });

            ViewBag.oemList = oemList;
            return View();
        }
        #endregion



        #region ProblemsLibrary 등록
        public JsonResult InsProblemsLibrary(ProblemsLibrary _param)
        {
            int resultOid = 0;

            try
            {
                DaoFactory.BeginTransaction();

                List<ProblemsLibrary> ProblemsLibrary = ProblemsLibraryRepository.SelProblemsLibrary(Session, _param);

                DObject dobj = new DObject();
                dobj.Type = EcontentsConstant.TYPE_PROBLEMS_LIBRARY;
                dobj.TableNm = EcontentsConstant.TABLE_PROBLEMS_LIBRARY;
                dobj.Name = (ProblemsLibrary.Count + 1).ToString();
                dobj.Description = _param.Description;
                
                resultOid = DObjectRepository.InsDObject(Session, dobj);

                _param.OID = resultOid;
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

        #region ProblemsLibrary 업데이트
        public JsonResult UdtProblemsLibrary(ProblemsLibrary _param)
        {
            int Udt = 0;

            try
            {
                DaoFactory.BeginTransaction();

                DObjectRepository.UdtDObject(Session, _param);

                Udt = ProblemsLibraryRepository.UdtProblemsLibrary(Session, _param);

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
            return Json(Udt);
        }
        #endregion


        #region OptimalDesign 검색
        public JsonResult SelOptimalDesign(OptimalDesign _param)
        {
            List<OptimalDesign> OptimalDesign = OptimalDesignRepository.SelOptimalDesign(Session, _param);
            return Json(OptimalDesign);
        }
        #endregion

        #region OptimalDesign 등록
        public JsonResult InsOptimalDesign(OptimalDesign _param, List<OptimalDesignItem> _items)
        {
            int resultOid = 0;

            try
            {
                DaoFactory.BeginTransaction();

                List<OptimalDesign> OptimalDesign = OptimalDesignRepository.SelOptimalDesign(Session, _param);
              
                DObject dobj = new DObject();
                dobj.Type = EcontentsConstant.TYPE_OPTIMAL_DESIGN;
                dobj.TableNm = EcontentsConstant.TABLE_OPTIMAL_DESIGN;
                dobj.Name = (OptimalDesign.Count + 1).ToString();

                resultOid = DObjectRepository.InsDObject(Session, dobj);
              
                _param.OID = resultOid;
             
                DaoFactory.SetInsert("Econtents.InsOptimalDesign", _param);

                _items.ForEach(v => {
                    OptimalDesignItem Item = new OptimalDesignItem();
                    Item.FromOID = resultOid;
                    Item.Problems_Lib_OID = v.OID;
                    Item.Reflected = v.Reflected;
                    Item.Description = v.Description;

                    DaoFactory.SetInsert("Econtents.InsOptimalDesignItem", Item);
                });

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

        #region OptimalDesign 검색
        public JsonResult SelOptimalDesignItem(OptimalDesignItem _param)
        {
            List<OptimalDesignItem> SelOptimalDesignItem = OptimalDesignRepository.SelOptimalDesignItem(Session, _param);
            return Json(SelOptimalDesignItem);
        }
        #endregion
        


    }
}