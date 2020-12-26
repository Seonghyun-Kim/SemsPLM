using Common.Constant;
using Common.Factory;
using Common.Models;
using Common.Models.File;
using Document.Models;
using DocumentClassification.Models;
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
            DocClass Document = DocClassRepository.SelDocClassObject(Session, new DocClass { Name = CommonConstant.ATTRIBUTE_DOCUMENT });
            List<DocClass> docTypeList = DocClassRepository.SelDocClass(Session,new DocClass { FromOID = Document.OID});
            ViewBag.docTypeList = docTypeList;
            
            return View();
        }
        public JsonResult SelDocClassTree()
        {
            return Json(DocClassRepository.SelDocClassTree(Session, CommonConstant.ATTRIBUTE_DOCUMENT));
        }
        public ActionResult InfoDocument(int OID)
        {
            Doc docDetail = DocRepository.SelDocObject(Session, new Doc { OID = OID });
            ViewBag.docDetail = docDetail;
            ViewBag.Status = BPolicyRepository.SelBPolicy(new BPolicy { Type = DocumentConstant.TYPE_DOCUMENT });
            return View();
        }

        public ActionResult CreateDocument()
        {

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
                dobj.Type = DocumentConstant.TYPE_DOCUMENT;
                dobj.TableNm = DocumentConstant.TABLE_DOCUMENT;
                dobj.Description = _param.Description;
                //dobj.TdmxOID = DObjectRepository.SelTdmxOID(new DObject { Type = DocumentContant.TYPE_DOCUMENT });
                var YYYY = DateTime.Now.ToString("yyyy");
                var MM = DateTime.Now.ToString("MM");
                var dd = DateTime.Now.ToString("dd");
                var selName = "DOC" + YYYY + MM + dd + "-001";
                var NewName = "DOC" + YYYY + MM + dd;

                var LateName = DocRepository.SelDoc(Session,new Doc { Name = NewName });

                if (LateName.Count == 0)
                {
                    dobj.Name = selName;
                }
                else
                {
                    int NUM = Convert.ToInt32(LateName.Last().Name.Substring(12, 3)) + 1;
                    dobj.Name = NewName + "-" + string.Format("{0:D3}", NUM);
                }

                resultOid = DObjectRepository.InsDObject(Session, dobj);
                _param.OID = resultOid;
                _param.Type = dobj.Type;
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
                _param.OID = resultOid;
                _param.DocGroup = DocumentConstant.TYPE_DOCUMENT;
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
            List<Doc> lDoc = DocRepository.SelDoc(Session,_param);
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

                DObjectRepository.UdtDObject(Session, _param);
                DocRepository.UdtDocObject(_param);

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

        

        #endregion

    }
}