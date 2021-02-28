using ChangeOrder.Models;
using Common;
using Common.Constant;
using Common.Factory;
using Common.Models;
using Common.Models.File;
using EBom.Models;
using SemsPLM.Filter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Web;
using System.Web.Mvc;

namespace SemsPLM.Controllers
{
    [AuthorizeFilter]
    public class EBomController : Controller
    {
        // GET: EBom
        #region -- EPart View
        public ActionResult EBomStructure()
        {
            return View();
        }
        public ActionResult CreateEPart()
        {
            Library ItemKey = LibraryRepository.SelCodeLibraryObject(new Library { Code1 = CommonConstant.ATTRIBUTE_ITEM });
            Library placeKey = LibraryRepository.SelLibraryObject(new Library { Name = CommonConstant.ATTRIBUTE_PRODUCED_PLACE });
            Library epartKey = LibraryRepository.SelLibraryObject(new Library { Name = CommonConstant.ATTRIBUTE_EPARTTYPE });
            Library oemKey = LibraryRepository.SelCodeLibraryObject(new Library { Code1 = CommonConstant.ATTRIBUTE_OEM });
            Library psizeKey = LibraryRepository.SelLibraryObject(new Library { Name = CommonConstant.ATTRIBUTE_PSIZE });

            List<Library> ItemList = LibraryRepository.SelCodeLibrary(new Library { FromOID = ItemKey.OID });  //OEM 목록
            List<Library> placeList = LibraryRepository.SelLibrary(new Library { FromOID = placeKey.OID });  //생산지 목록
            List<Library> epartList = LibraryRepository.SelLibrary(new Library { FromOID = epartKey.OID });  //제품구분 목록
            List<Library> oemList = LibraryRepository.SelCodeLibrary(new Library { FromOID = oemKey.OID });  //차종 목록
            List<Library> psizeList = LibraryRepository.SelLibrary(new Library { FromOID = psizeKey.OID });  //제품구분 목록

            ViewBag.ItemList = ItemList;
            ViewBag.placeList = placeList;
            ViewBag.epartList = epartList;
            ViewBag.oemList = oemList;
            ViewBag.psizeList = psizeList;
            return View();
        }
        public ActionResult SearchEPart()
        {
            Library ItemKey = LibraryRepository.SelCodeLibraryObject(new Library { Code1 = CommonConstant.ATTRIBUTE_ITEM });
            Library oemKey = LibraryRepository.SelCodeLibraryObject(new Library { Code1 = CommonConstant.ATTRIBUTE_OEM });

            List<Library> ItemList = LibraryRepository.SelCodeLibrary(new Library { FromOID = ItemKey.OID });  //OEM 목록
            List<Library> oemList = LibraryRepository.SelCodeLibrary(new Library { FromOID = oemKey.OID });  //차종 목록

            ViewBag.oemList = oemList;
            ViewBag.ItemList = ItemList;
            return View();
        }

        public ActionResult SearchReleaseEPart()
        {
            Library ItemKey = LibraryRepository.SelCodeLibraryObject(new Library { Code1 = CommonConstant.ATTRIBUTE_ITEM });
            Library oemKey = LibraryRepository.SelCodeLibraryObject(new Library { Code1 = CommonConstant.ATTRIBUTE_OEM });

            List<Library> ItemList = LibraryRepository.SelCodeLibrary(new Library { FromOID = ItemKey.OID });  //OEM 목록
            List<Library> oemList = LibraryRepository.SelCodeLibrary(new Library { FromOID = oemKey.OID });  //차종 목록

            ViewBag.oemList = oemList;
            ViewBag.ItemList = ItemList;
            return View();
        }

        public ActionResult InfoEPart(int OID)
        {
            ViewBag.OID = OID;
            EPart InfoEPart = EPartRepository.SelEPartObject(Session, new EPart { OID = OID });
            ViewBag.Status = BPolicyRepository.SelBPolicy(new BPolicy { Type = EBomConstant.TYPE_PART });

            Library ItemKey = LibraryRepository.SelCodeLibraryObject(new Library { Code1 = CommonConstant.ATTRIBUTE_ITEM });
            Library placeKey = LibraryRepository.SelLibraryObject(new Library { Name = CommonConstant.ATTRIBUTE_PRODUCED_PLACE });
            Library epartKey = LibraryRepository.SelLibraryObject(new Library { Name = CommonConstant.ATTRIBUTE_EPARTTYPE});
            Library oemKey = LibraryRepository.SelCodeLibraryObject(new Library { Code1 = CommonConstant.ATTRIBUTE_OEM });
            Library psizeKey = LibraryRepository.SelLibraryObject(new Library { Name = CommonConstant.ATTRIBUTE_PSIZE });

            List<Library> ItemList = LibraryRepository.SelCodeLibrary(new Library { FromOID = ItemKey.OID });  //OEM 목록
            List<Library> placeList = LibraryRepository.SelLibrary(new Library { FromOID = placeKey.OID });  //생산지 목록
            List<Library> epartList = LibraryRepository.SelLibrary(new Library { FromOID = epartKey.OID });  //제품구분 목록
            List<Library> oemList = LibraryRepository.SelCodeLibrary(new Library { FromOID = oemKey.OID });  //차종 목록
            List<Library> psizeList = LibraryRepository.SelLibrary(new Library { FromOID = psizeKey.OID });  //제품구분 목록

            ViewBag.ItemList = ItemList;
            ViewBag.placeList = placeList;
            ViewBag.epartList = epartList;
            ViewBag.oemList = oemList;
            ViewBag.psizeList = psizeList;

            return View(InfoEPart);
        }
        public ActionResult dlgSearchEBomStructure(int? OID)
        {
            Library ItemKey = LibraryRepository.SelCodeLibraryObject(new Library { Code1 = CommonConstant.ATTRIBUTE_ITEM });
            Library oemKey = LibraryRepository.SelCodeLibraryObject(new Library { Code1 = CommonConstant.ATTRIBUTE_OEM });


            List<Library> ItemList = LibraryRepository.SelCodeLibrary(new Library { FromOID = ItemKey.OID });  //OEM 목록
            List<Library> oemList = LibraryRepository.SelCodeLibrary(new Library { FromOID = oemKey.OID });  //차종 목록

            ViewBag.oemList = oemList;
            ViewBag.ItemList = ItemList;
            ViewBag.OID = OID;
            return PartialView("Dialog/dlgSearchEBomStructure");
        }

        #region EPart Tree추가 창 검색
        public ActionResult dlgSetSearchEBomStructure(int? OID)
        {
            Library ItemKey = LibraryRepository.SelCodeLibraryObject(new Library { Code1 = CommonConstant.ATTRIBUTE_ITEM });
            Library oemKey = LibraryRepository.SelCodeLibraryObject(new Library { Code1 = CommonConstant.ATTRIBUTE_OEM });


            List<Library> ItemList = LibraryRepository.SelCodeLibrary(new Library { FromOID = ItemKey.OID });  //OEM 목록
            List<Library> oemList = LibraryRepository.SelCodeLibrary(new Library { FromOID = oemKey.OID });  //차종 목록

            ViewBag.oemList = oemList;
            ViewBag.ItemList = ItemList;
            ViewBag.OID = OID;
            return PartialView("Dialog/dlgSetSearchEBomStructure");
        }
        #endregion

        public ActionResult CompareEPart()
        {
            return View();
        }

        public ActionResult dlgSearchEPart()
        {
            Library ItemKey = LibraryRepository.SelCodeLibraryObject(new Library { Code1 = CommonConstant.ATTRIBUTE_ITEM });
            Library oemKey = LibraryRepository.SelCodeLibraryObject(new Library { Code1 = CommonConstant.ATTRIBUTE_OEM });


            List<Library> ItemList = LibraryRepository.SelCodeLibrary(new Library { FromOID = ItemKey.OID });  //OEM 목록
            List<Library> oemList = LibraryRepository.SelCodeLibrary(new Library { FromOID = oemKey.OID });  //차종 목록

            ViewBag.oemList = oemList;
            ViewBag.ItemList = ItemList;
            return PartialView("Dialog/dlgSearchEPart");
        }
        public ActionResult dlgCreateEPart(string Division)
        {
            Library ItemKey = LibraryRepository.SelCodeLibraryObject(new Library { Code1 = CommonConstant.ATTRIBUTE_ITEM});
            Library placeKey = LibraryRepository.SelLibraryObject(new Library { Name = CommonConstant.ATTRIBUTE_PRODUCED_PLACE });
            Library epartKey = LibraryRepository.SelLibraryObject(new Library { Name = CommonConstant.ATTRIBUTE_EPARTTYPE });
            Library oemKey = LibraryRepository.SelCodeLibraryObject(new Library { Code1 = CommonConstant.ATTRIBUTE_OEM });
            Library psizeKey = LibraryRepository.SelLibraryObject(new Library { Name = CommonConstant.ATTRIBUTE_PSIZE });

            List<Library> ItemList = LibraryRepository.SelCodeLibrary(new Library { FromOID = ItemKey.OID });  //OEM 목록
            List<Library> placeList = LibraryRepository.SelLibrary(new Library { FromOID = placeKey.OID });  //생산지 목록
            List<Library> epartList = LibraryRepository.SelLibrary(new Library { FromOID = epartKey.OID });  //제품구분 목록
            List<Library> oemList = LibraryRepository.SelCodeLibrary(new Library { FromOID = oemKey.OID });  //차종 목록
            List<Library> psizeList = LibraryRepository.SelLibrary(new Library { FromOID = psizeKey.OID });  //제품구분 목록

            ViewBag.ItemList = ItemList;
            ViewBag.placeList = placeList;
            ViewBag.epartList = epartList;
            ViewBag.oemList = oemList;
            ViewBag.psizeList = psizeList;

            if(Division != null)
            {
                ViewBag.Division = Division;
            }
            else
            {
                ViewBag.Division = "";
            }

            return PartialView("Dialog/dlgCreateEPart");
        }

        #region 조립도이고 Assy인 제품 검색
        public ActionResult dlgSearchEPartAssy(string RootOID)
        {
            Library ItemKey = LibraryRepository.SelCodeLibraryObject(new Library { Code1 = CommonConstant.ATTRIBUTE_ITEM });
            Library oemKey = LibraryRepository.SelCodeLibraryObject(new Library { Code1 = CommonConstant.ATTRIBUTE_OEM });

            List<Library> ItemList = LibraryRepository.SelCodeLibrary(new Library { FromOID = ItemKey.OID });  //OEM 목록
            List<Library> oemList = LibraryRepository.SelCodeLibrary(new Library { FromOID = oemKey.OID });  //차종 목록

            Library EPartTypeKey = LibraryRepository.SelLibraryObject(new Library { Name = EBomConstant.PART_TYPE });
            List<Library> EPartTypeList = LibraryRepository.SelLibrary(new Library { FromOID = EPartTypeKey.OID });

            Library AssyKey = new Library();
            EPartTypeList.ForEach(item => {
                if(item.KorNm == EBomConstant.PART_TYPE_ASSY)
                {
                    AssyKey = item;
                    return;
                }
            });

            ViewBag.EPartTypeAssy = AssyKey.OID;
            ViewBag.RootOID = RootOID;
            ViewBag.oemList = oemList;
            ViewBag.ItemList = ItemList;
            return PartialView("Dialog/dlgSearchEPartAssy");
        }
        #endregion

        #region EPart 최상위 검색 페이지
        public ActionResult dlgSearchTopEPart(int? OID, string Division)
        {
            ViewBag.OID = OID;
            ViewBag.Division = Division;
            return PartialView("Dialog/dlgSearchTopEPart");
        }
        #endregion

        #endregion

        #region EPart 검색
        public JsonResult SelEPart(EPart _param)
        {
            List<EPart> Epart = EPartRepository.SelEPart(Session, _param);
            return Json(Epart);
        }
        #endregion

        #region EPart 등록
        public JsonResult InsEPart(EPart _param)
        {
            int resultOid = 0;
            DObject dobj = new DObject();
            try
            {
                DaoFactory.BeginTransaction();
                var check = CreateEPartChk(_param);
                if (check == 1)
                {
                    DaoFactory.Rollback();
                    return Json(new ResultJsonModel { isError = true, resultMessage = "품번이 이미 존재합니다.", resultDescription = "" });
                }

                if(_param.Division != Common.Constant.EBomConstant.DIV_STANDARD)
                {
                    int SelRev = SemsUtil.SingleCompare(_param.Sel_Revision);
                    if (SelRev < 0)
                    {
                        DaoFactory.Rollback();
                        return Json(new ResultJsonModel { isError = true, resultMessage = "고객사 리비전은 숫자 또는 문자만 가능 합니다", resultDescription = "고객사 리비전은 숫자 또는 문자만 가능 합니다" });
                    }

                    var CheckName = _param.Name.Substring(0, (_param.Name.Length - 1));
                    var CheckDiv = CreateEPartChk(new EPart { Name = CheckName, Type = EBomConstant.TYPE_PART, Division = _param.Division });
                    if (CheckDiv == 1)
                    {
                        DaoFactory.Rollback();
                        return Json(new ResultJsonModel { isError = true, resultMessage = "품번이 이미 존재합니다.", resultDescription = "" });
                    }
                }
                
                dobj.Type = EBomConstant.TYPE_PART;
                dobj.TableNm = EBomConstant.TABLE_PART;
                dobj.Name = _param.Name;
                dobj.Description = _param.Description;
                dobj.Thumbnail = _param.Thumbnail;
                resultOid = DObjectRepository.InsDObject(Session, dobj);

                _param.Type = EBomConstant.TYPE_PART;
                _param.OID = resultOid;

                DaoFactory.SetInsert("EBom.InsEPart", _param);

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
            return Json(resultOid);
        }
        #endregion

        #region EPart 정전개
        public JsonResult SelectEBom(EPart _param)
        {
            EBOM lBom = EPartRepository.getListEbom(Session, 0, Convert.ToInt32(_param.OID));
            return Json(lBom);
        }
        #endregion

        #region EPart 역전개
        public JsonResult SelectReverseEBom(EPart _param)
        {
            EBOM lBom = EPartRepository.getListReverseEbom(Session, 0, Convert.ToInt32(_param.OID));
            return Json(lBom);
        }
        #endregion

        #region EPart 중복체크
        public int CreateEPartChk(EPart _param)
        {
            int result = 0;
            EPart Epart = EPartRepository.ChkEPart(_param);
            if(Epart != null)
            {
                result = 1;
            }
            return result;
        }
        #endregion

        #region EPart 수정
        public JsonResult UdtEPartObj(EPart _param)
        {
            int result = 0;
            try
            {
                DaoFactory.BeginTransaction();
                List<EPart> EpartName = EPartRepository.SelEPart(Session, new EPart { Name = _param.Name });

                if (EpartName != null && EpartName.Count > 0 && _param.OID != EpartName[0].OID)
                {
                    DaoFactory.Rollback();
                    return Json(new ResultJsonModel { isError = true, resultMessage = "이미 존재하는 품번 입니다.", resultDescription = "이미 존재하는 품번 입니다." });
                }

                if (_param.Division != EBomConstant.DIV_STANDARD)
                {
                    int SelRev = SemsUtil.SingleCompare(_param.Sel_Revision);
                    if (SelRev < 0)
                    {
                        DaoFactory.Rollback();
                        return Json(new ResultJsonModel { isError = true, resultMessage = "고객사 리비전은 숫자 또는 문자만 가능 합니다", resultDescription = "고객사 리비전은 숫자 또는 문자만 가능 합니다" });
                    }

                    List<EPart> Tdmxlist = EPartRepository.SelEPart(Session, new EPart { TdmxOID = _param.TdmxOID });
                    if (_param.Revision != "00")
                    {
                        if (SemsUtil.SingleCompare(Tdmxlist[0].Sel_Revision) > SelRev)
                        {
                            DaoFactory.Rollback();
                            return Json(new ResultJsonModel { isError = true, resultMessage = "이전 고객사 리비전은 사용 할 수 없습니다", resultDescription = "이전 고객사 리비전은 사용 할 수 없습니다" });
                        }
                    }
                }

                DObjectRepository.UdtDObject(Session, _param);

                EPartRepository.UdtEPartObject(_param);

                if(_param.Thumbnail == null || _param.Thumbnail == "")
                {
                    DObjectRepository.DelThumbnailDObject(Session, _param);
                }
                
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
            List<EBOM> lBom = EPartRepository.getListEbomAddChild(Session, 0, _param.Name, _param);
            return Json(lBom);
        }
        #endregion

        #region EBOM 편집 저장
        public JsonResult EditStructure(List<EBOM> _param)
        {
            int resultOID = 0;

            Dictionary<int, int> applyoid = new Dictionary<int, int>();         //EPART 가 개정된 oid
            Dictionary<int, int> relationoid = new Dictionary<int, int>();      //구조가 개정 되면서 변경 된것

            EBOM NewRel = null;

            if (_param == null)
            {
                return Json(1);
            }

            try
            {
                DaoFactory.BeginTransaction();

                var RevExist = _param.FindAll(v => v.Action == "REV");

                if (RevExist.Count > 0)
                {
                    foreach (EBOM Data in _param)
                    {
                        if (Data.Action == "D")
                        {
                            EBomRepository.DeleteAction(Session, Data);
                        }
                    }
                    _param.RemoveAll(VALUE => VALUE.Action == "D");

                    foreach (EBOM RevData in RevExist)
                    {
                        if (resultOID != 0)
                        {
                            resultOID = 0;
                        }

                        var Data = EPartRepository.SelEPartObject(Session, new EPart { OID = RevData.ToOID });

                        if (Data.Division == Common.Constant.EBomConstant.DIV_SINGLE)
                        {
                            Data.Sel_Revision = SemsUtil.SingleMakeMajorRevisonUp(Data.Sel_Revision);
                            string SelRev = Data.Name.Substring(0, (Data.Name.Length - 1));
                            Data.Name = SelRev + Data.Sel_Revision;
                        }

                        resultOID = DObjectRepository.ReviseDObject(Session, new DObject { OID = RevData.ToOID, Name = Data.Name });

                        HttpFileRepository.ReviseFiles(Session, new HttpFile { OID = RevData.ToOID }, resultOID);

                        Data.OID = resultOID;

                        DaoFactory.SetInsert("EBom.InsEPart", Data);

                        applyoid.Add(Convert.ToInt32(RevData.ToOID), resultOID);
                    }

                    foreach (EBOM RevRelData in RevExist)
                    {
                        if (!relationoid.ContainsKey(Convert.ToInt32(RevRelData.OID)))
                        {
                            NewRel = new EBOM();
                            if (applyoid.ContainsKey(Convert.ToInt32(RevRelData.FromOID)))
                            {
                                NewRel.FromOID = applyoid[Convert.ToInt32(RevRelData.FromOID)];
                            }
                            else
                            {
                                NewRel.FromOID = RevRelData.FromOID;
                            }
                            if (applyoid.ContainsKey(Convert.ToInt32(RevRelData.ToOID)))
                            {
                                NewRel.ToOID = applyoid[Convert.ToInt32(RevRelData.ToOID)];
                            }
                            else
                            {
                                NewRel.ToOID = RevRelData.ToOID;
                            }
                            NewRel.Count = RevRelData.Count;
                            NewRel.Ord = RevRelData.Ord;
                            

                            if(applyoid.ContainsKey(Convert.ToInt32(RevRelData.FromOID)) && applyoid.ContainsKey(Convert.ToInt32(RevRelData.ToOID)))
                            {
                                int NewRelOID = EBomRepository.AddAction(Session, NewRel);
                                relationoid.Add(Convert.ToInt32(RevRelData.OID), NewRelOID);
                            }
                            else
                            {
                                //테스트 해야함
                                NewRel.OldOID = RevRelData.OID;
                                int NewRelOID = EBomRepository.ReAction(Session, NewRel);

                                List<EBOM> SelChlidEBom = DaoFactory.GetList<EBOM>("EBom.SelEBom", new EBOM { FromOID = RevRelData.ToOID });

                                SelChlidEBom.ForEach(val => {
                                    val.FromOID = NewRel.ToOID;
                                    EBomRepository.AddAction(Session, val);
                                });
                            }
                            
                        }
                    }
                    _param.RemoveAll(VALUE => VALUE.Action == "REV");

                    foreach (EBOM Data in _param)
                    {
                        if (Data.Action == "A")
                        {
                            if (applyoid.ContainsKey(Convert.ToInt32(Data.FromOID)))
                            {
                                NewRel = new EBOM();
                                NewRel.FromOID = applyoid[Convert.ToInt32(Data.FromOID)];
                                if (applyoid.ContainsKey(Convert.ToInt32(Data.ToOID)))
                                {
                                    NewRel.ToOID = applyoid[Convert.ToInt32(Data.ToOID)];
                                }
                                else
                                {
                                    NewRel.ToOID = Data.ToOID;
                                }
                                NewRel.Count = Data.Count;
                                NewRel.Ord = Data.Ord;
                                EBomRepository.AddAction(Session, NewRel);
                            }
                            else
                            {
                                EBomRepository.AddAction(Session, Data);
                            }
                        }
                    }
                    _param.RemoveAll(VALUE => VALUE.Action == "A");


                    foreach (EBOM Data in _param)
                    {
                        if (Data.Action == "RE")
                        {
                            if (applyoid.ContainsKey(Convert.ToInt32(Data.FromOID)))
                            {
                                NewRel = new EBOM();
                                NewRel.FromOID = applyoid[Convert.ToInt32(Data.FromOID)];
                                if (applyoid.ContainsKey(Convert.ToInt32(Data.ToOID)))
                                {
                                    NewRel.ToOID = applyoid[Convert.ToInt32(Data.ToOID)];
                                }
                                NewRel.Count = Data.Count;
                                NewRel.Ord = Data.Ord;
                                EBomRepository.AddAction(Session, NewRel);
                            }
                            else
                            {
                                EBomRepository.ReAction(Session, Data);
                            }
                        }
                    }
                    _param.RemoveAll(VALUE => VALUE.Action == "RE");

                    foreach (EBOM Data in _param)
                    {
                        if (Data.Action == "U")
                        {
                            if (relationoid.ContainsKey(Convert.ToInt32(Data.OID)))
                            {
                                Data.OID = relationoid[Convert.ToInt32(Data.OID)];
                            }
                            EBomRepository.UdtAction(Session, Data);
                        }
                    }
                    _param.RemoveAll(VALUE => VALUE.Action == "U");


                }
                else
                {
                    foreach (EBOM Data in _param)
                    {
                        if (Data.Action == "D")
                        {
                            EBomRepository.DeleteAction(Session, Data);
                        }
                    }
                    _param.RemoveAll(VALUE => VALUE.Action == "D");

                    foreach (EBOM Data in _param)
                    {
                        if (Data.Action == "A")
                        {
                            EBomRepository.AddAction(Session, Data);
                        }
                    }
                    _param.RemoveAll(VALUE => VALUE.Action == "A");

                    foreach (EBOM Data in _param)
                    {
                        if (Data.Action == "RE")
                        {
                            EBomRepository.ReAction(Session, Data);
                        }
                    }
                    _param.RemoveAll(VALUE => VALUE.Action == "RE");

                    foreach (EBOM Data in _param)
                    {
                        if (Data.Action == "U")
                        {
                            EBomRepository.UdtAction(Session, Data);
                        }
                    }
                    _param.RemoveAll(VALUE => VALUE.Action == "U");
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

        #region EPart Compare
        public JsonResult EPartCompare(int? LOID, int? ROID)
        {
            int level = 0;
            List<EBOM> CompareList = new List<EBOM>();
        
            EPart LPataData = EPartRepository.SelEPartObject(Session, new EPart { OID = LOID });
            EPart RPataData = EPartRepository.SelEPartObject(Session, new EPart { OID = ROID });

            List<EBOM> LList = new List<EBOM>();
            List<EBOM> RList = new List<EBOM>();

            List<EBOM> LEPartList = EPartRepository.SelRootChildEBomList(new EBOM { FromOID = Convert.ToInt32(LOID) });

            EBOM LEPartObj = new EBOM();
            LEPartObj.Level = 0;
            LEPartObj.FromOID = 0;
            LEPartObj.ToOID = LOID;
            LEPartList.Insert(0, LEPartObj);

            List<EBOM> REPartList = EPartRepository.SelRootChildEBomList(new EBOM { FromOID = Convert.ToInt32(ROID) });

            EBOM REPartObj = new EBOM();
            REPartObj.Level = 0;
            REPartObj.FromOID = 0;
            REPartObj.ToOID = LOID;
            REPartList.Insert(0, REPartObj);

            //리스트 만들기
            LEPartList.ForEach(item =>
            {
                int compareIndex = CompareList.FindIndex(innerItem => { return ((EBOM)innerItem).FromOID == item.FromOID && ((EBOM)innerItem).ToOID == item.ToOID; });
                if (compareIndex < 0)
                {
                    int RListIndex = REPartList.FindIndex(innerItem =>
                    {
                        return ((EBOM)innerItem).Ord == item.Ord && ((EBOM)innerItem).Level == item.Level;
                    });
                    if (RListIndex > -1)
                    {
                        item.Action = PmsConstant.ACTION_ADD;
                        item.Ord = REPartList[RListIndex].Ord;
                        item.RToOID = REPartList[RListIndex].ToOID;
                        CompareList.Add(item);
                        REPartList.RemoveAt(RListIndex);
                    }
                    else if (RListIndex < 0)
                    {
                        item.Action = PmsConstant.ACTION_LEFT;
                        CompareList.Add(item);
                    }
                }
            });
            REPartList.ForEach(item =>
            {
                item.Action = PmsConstant.ACTION_RIGHT;
                item.LToOID = item.ToOID;
                CompareList.Add(item);
            });
            //return Json(0);
            return Json(getListCompareEPartStructure(level, LOID, ROID, LPataData, RPataData, CompareList));
        }

        public EPartCompare getListCompareEPartStructure(int Lev, int? LOID, int? ROID, EPart LPartData, EPart RPataData, List<EBOM> CompareList)
        {
            List<EBOM> CheckCompareList = CompareList;

            EPartCompare getCompareStructure = new EPartCompare();
            getCompareStructure.Level = Lev;
            getCompareStructure.LOId = LOID;
            getCompareStructure.ToOID = LOID;
            getCompareStructure.RootOID = LOID;
            getCompareStructure.LName = LPartData.Name;
            getCompareStructure.LType = EBomConstant.TYPE_PART;
            getCompareStructure.LToOld = LOID;
            getCompareStructure.LRevision = LPartData.Revision;
            //getCompareStructure.LId = null;

            getCompareStructure.RName = RPataData.Name;
            getCompareStructure.RType = EBomConstant.TYPE_PART;
            getCompareStructure.RToOld = ROID;
            getCompareStructure.RRevision = RPataData.Revision;
            //getCompareStructure.RId = null;

            getCompareEPartStructure(getCompareStructure, LOID, ROID, CompareList, CheckCompareList);

            return getCompareStructure;
        }

        public void getCompareEPartStructure(EPartCompare getCompareStructure, int? LOID, int? ROID, List<EBOM> CompareList, List<EBOM> CheckCompareList)
        {
            getCompareStructure.CompareChildren = new List<EPartCompare>();

            List<EBOM> lRelation = CompareList.FindAll(item => { return (((EBOM)item).FromOID == getCompareStructure.ToOID) || (item.FromOID == getCompareStructure.RToOld) || (item.FromOID == getCompareStructure.LToOld); }).OrderBy(x => ((EBOM)x).Ord).ToList();
            
            for (var j = 0; j < lRelation.Count; j++)
            {
                EPartCompare tmpCompareItem = new EPartCompare();
                tmpCompareItem.Level = getCompareStructure.Level + 1;
                tmpCompareItem.FromOID = lRelation[j].FromOID;
                tmpCompareItem.ToOID = lRelation[j].ToOID;
                tmpCompareItem.LToOld = lRelation[j].LToOID;
                tmpCompareItem.RToOld = lRelation[j].RToOID;
                tmpCompareItem.Action = lRelation[j].Action;
                if (tmpCompareItem.Action.Equals(PmsConstant.ACTION_ADD))
                {
                    tmpCompareItem.Action = PmsConstant.ACTION_NONE;

                    EPart LDetail = EPartRepository.SelEPartObject(Session, new EPart { OID = tmpCompareItem.ToOID });
                    tmpCompareItem.LOId = LDetail.OID;
                    tmpCompareItem.LName = LDetail.Name;
                    tmpCompareItem.LType = LDetail.Type;
                    //tmpCompareItem.LId = LDetail.OID;
                    tmpCompareItem.LOrd = lRelation[j].Ord;
                    tmpCompareItem.LRevision = LDetail.Revision;
                    tmpCompareItem.LCar_Lib_OID = LDetail.Car_Lib_OID;
                    tmpCompareItem.LCar_Lib_NM = LDetail.Car_Lib_Nm;
                    tmpCompareItem.LThumbnail = LDetail.Thumbnail;


                    EPart RDetail = EPartRepository.SelEPartObject(Session, new EPart { OID = tmpCompareItem.RToOld });
                    tmpCompareItem.ROId = RDetail.OID;
                    tmpCompareItem.RName = RDetail.Name;
                    tmpCompareItem.RType = RDetail.Type;
                    //tmpCompareItem.RId = RDetail.OID;
                    tmpCompareItem.ROrd = lRelation[j].Ord;
                    tmpCompareItem.RRevision = RDetail.Revision;
                    tmpCompareItem.RCar_Lib_OID = RDetail.Car_Lib_OID;
                    tmpCompareItem.RCar_Lib_NM = RDetail.Car_Lib_Nm;
                    tmpCompareItem.RThumbnail = RDetail.Thumbnail;

                    if (!tmpCompareItem.LOId.Equals(tmpCompareItem.ROId))
                    {
                        tmpCompareItem.Action = PmsConstant.ACTION_MODIFY;

                        if (!tmpCompareItem.LName.Equals(tmpCompareItem.RName))
                        {
                            tmpCompareItem.Action = tmpCompareItem.Action + "|" + "Name";
                        }

                        if (!tmpCompareItem.LOrd.Equals(tmpCompareItem.ROrd))
                        {
                            tmpCompareItem.Action = tmpCompareItem.Action + "|" + "Ord";
                        }
                    }
                }
                else if (tmpCompareItem.Action.Equals(PmsConstant.ACTION_LEFT))
                {
                    tmpCompareItem.Action = PmsConstant.ACTION_ADD_NM;
                    EPart LDetail = EPartRepository.SelEPartObject(Session, new EPart { OID = tmpCompareItem.ToOID });
                    tmpCompareItem.LOId = LDetail.OID;
                    tmpCompareItem.LName = LDetail.Name;
                    tmpCompareItem.LType = LDetail.Type;
                    //tmpCompareItem.LId = LDetail.OID;
                    tmpCompareItem.LOrd = lRelation[j].Ord;
                    tmpCompareItem.LRevision = LDetail.Revision;
                    tmpCompareItem.LCar_Lib_OID = LDetail.Car_Lib_OID;
                    tmpCompareItem.LCar_Lib_NM = LDetail.Car_Lib_Nm;
                    tmpCompareItem.LThumbnail = LDetail.Thumbnail;
                }
                else if (tmpCompareItem.Action.Equals(PmsConstant.ACTION_RIGHT))
                {
                    tmpCompareItem.Action = PmsConstant.ACTION_DELETE_NM;

                    EPart RDetail = new EPart();
                    if (tmpCompareItem.RToOld != null)
                    {
                        RDetail = EPartRepository.SelEPartObject(Session, new EPart { OID = tmpCompareItem.RToOld });
                    }
                    else
                    {
                        RDetail = EPartRepository.SelEPartObject(Session, new EPart { OID = tmpCompareItem.ToOID });
                    }
                    tmpCompareItem.ROId = RDetail.OID;
                    tmpCompareItem.RName = RDetail.Name;
                    tmpCompareItem.RType = RDetail.Type;
                    //tmpCompareItem.RId = RDetail.OID;
                    tmpCompareItem.ROrd = lRelation[j].Ord;
                    tmpCompareItem.RRevision = RDetail.Revision;
                    tmpCompareItem.RCar_Lib_OID = RDetail.Car_Lib_OID;
                    tmpCompareItem.RCar_Lib_NM = RDetail.Car_Lib_Nm;
                    tmpCompareItem.RThumbnail = RDetail.Thumbnail;
                }

                getCompareStructure.CompareChildren.Add(tmpCompareItem);
                CheckCompareList.RemoveAll(data => data.Equals(lRelation[j]));
                getCompareEPartStructure(tmpCompareItem, LOID, ROID, CompareList, CheckCompareList);
                //List<EBOM> A = CompareList.FindAll(item => { return ((EBOM)item).FromOID == tmpCompareItem.ToOID; }).OrderBy(x => ((EBOM)x).Ord).ToList();
            }
        }


        public void getCheckCompareEPartStructure(EPartCompare getCompareStructure, int? LOID, int? ROID, List<EBOM> CompareList, List<EBOM> CheckCompareList)
        {
            getCompareStructure.CompareChildren = new List<EPartCompare>();

            List<EBOM> lRelation = CompareList.FindAll(item => { return ((EBOM)item).FromOID == getCompareStructure.RToOld && item.LToOID == null; }).OrderBy(x => ((EBOM)x).Ord).ToList();

            for (var j = 0; j < lRelation.Count; j++)
            {
                EPartCompare tmpCompareItem = new EPartCompare();
                tmpCompareItem.Level = getCompareStructure.Level + 1;
                tmpCompareItem.FromOID = lRelation[j].FromOID;
                tmpCompareItem.ToOID = lRelation[j].ToOID;
                tmpCompareItem.LToOld = lRelation[j].LToOID;
                tmpCompareItem.RToOld = lRelation[j].RToOID;
                tmpCompareItem.Action = lRelation[j].Action;
                if (tmpCompareItem.Action.Equals(PmsConstant.ACTION_ADD))
                {
                    tmpCompareItem.Action = PmsConstant.ACTION_NONE;

                    EPart LDetail = EPartRepository.SelEPartObject(Session, new EPart { OID = tmpCompareItem.ToOID });
                    tmpCompareItem.LOId = LDetail.OID;
                    tmpCompareItem.LName = LDetail.Name;
                    tmpCompareItem.LType = LDetail.Type;
                    //tmpCompareItem.LId = LDetail.OID;
                    tmpCompareItem.LOrd = lRelation[j].Ord;


                    EPart RDetail = EPartRepository.SelEPartObject(Session, new EPart { OID = tmpCompareItem.RToOld });
                    tmpCompareItem.ROId = RDetail.OID;
                    tmpCompareItem.RName = RDetail.Name;
                    tmpCompareItem.RType = RDetail.Type;
                    //tmpCompareItem.RId = RDetail.OID;
                    tmpCompareItem.ROrd = lRelation[j].Ord;

                    if (!tmpCompareItem.LOId.Equals(tmpCompareItem.ROId))
                    {
                        tmpCompareItem.Action = PmsConstant.ACTION_MODIFY;

                        if (!tmpCompareItem.LName.Equals(tmpCompareItem.RName))
                        {
                            tmpCompareItem.Action = tmpCompareItem.Action + "|" + "Name";
                        }

                        if (!tmpCompareItem.LOrd.Equals(tmpCompareItem.ROrd))
                        {
                            tmpCompareItem.Action = tmpCompareItem.Action + "|" + "Ord";
                        }
                    }
                }
                else if (tmpCompareItem.Action.Equals(PmsConstant.ACTION_LEFT))
                {
                    tmpCompareItem.Action = PmsConstant.ACTION_ADD_NM;
                    EPart LDetail = EPartRepository.SelEPartObject(Session, new EPart { OID = tmpCompareItem.ToOID });
                    tmpCompareItem.LOId = LDetail.OID;
                    tmpCompareItem.LName = LDetail.Name;
                    tmpCompareItem.LType = LDetail.Type;
                    //tmpCompareItem.LId = LDetail.OID;
                    tmpCompareItem.LOrd = lRelation[j].Ord;
                }
                else if (tmpCompareItem.Action.Equals(PmsConstant.ACTION_RIGHT))
                {
                    tmpCompareItem.Action = PmsConstant.ACTION_DELETE_NM;
                    EPart RDetail = EPartRepository.SelEPartObject(Session, new EPart { OID = tmpCompareItem.RToOld });
                    tmpCompareItem.ROId = RDetail.OID;
                    tmpCompareItem.RName = RDetail.Name;
                    tmpCompareItem.RType = RDetail.Type;
                    //tmpCompareItem.RId = RDetail.OID;
                    tmpCompareItem.ROrd = lRelation[j].Ord;
                }

                getCompareStructure.CompareChildren.Add(tmpCompareItem);
                CheckCompareList.RemoveAll(data => data.Equals(lRelation[j]));
                getCheckCompareEPartStructure(tmpCompareItem, LOID, ROID, CompareList, CheckCompareList);
                //List<EBOM> A = CompareList.FindAll(item => { return ((EBOM)item).FromOID == tmpCompareItem.ToOID; }).OrderBy(x => ((EBOM)x).Ord).ToList();
            }
        }
        #endregion

        #region 자식 기준 최상위 EPART 리스트
        public JsonResult SelChildTopParentList(EBOM _param)
        {
            List<EPart> EPartList = EPartRepository.SelChildTopParentList(Session, _param);
            return Json(EPartList);
        }
        #endregion

        #region 역전개 개정
        public JsonResult ReverseStructure(List<EPart> _param, int? RootOID, int? NewDataOID)
        {
            Dictionary<int, int> applyoid = new Dictionary<int, int>();         //EPART 가 개정된 oid
            Dictionary<int, int> relationoid = new Dictionary<int, int>();      //구조가 개정 되면서 변경 된것

            int NewFromOID = 0;
            int NewToOID = 0;
            int NewRelOID = 0;

            try
            {
                DaoFactory.BeginTransaction();

                //var RootData = EPartRepository.SelEPartObject(Session, new EPart { OID = RootOID });
                //if (RootData.Division == Common.Constant.EBomConstant.DIV_STANDARD)
                //{
                //
                //}

                List<EBOM> AllStructure = new List<EBOM>();
                List<EBOM> RootChildList = null;

                _param.ForEach(v => {
                    if(RootChildList != null)
                    {
                        RootChildList = null;
                    }
                    RootChildList = EPartRepository.SelRootChildEBomList(new EBOM { FromOID = v.OID });  //전체구조
                    AllStructure.AddRange(RootChildList);
                });
                                

                List<EBOM> EditStructure = EPartRepository.ReverseStructure(Session, _param, RootOID);                          //수정해야할 구조

                EPart FromData = null;
                List<EPart> FromTdmxData = null;

                EPart ToData = null;
                List<EPart> ToTdmxData = null;

                EditStructure.ForEach(v =>
                {
                    if (FromData != null) {
                        FromData = null;
                    }
                    FromData = EPartRepository.SelEPartObject(Session, new EPart { OID = v.FromOID });
                    if (FromTdmxData != null)
                    {
                        FromTdmxData = null;
                    }
                    FromTdmxData = EPartRepository.SelEPart(Session, new EPart { TdmxOID = FromData.TdmxOID });
                    FromTdmxData = FromTdmxData.OrderByDescending(revVal => revVal.Revision).ToList();

                    if (!applyoid.ContainsKey(Convert.ToInt32(FromData.OID)))
                    {
                        if (NewDataOID != null && RootOID == FromData.OID)
                        {
                            applyoid.Add(Convert.ToInt32(RootOID), Convert.ToInt32(NewDataOID));
                        }
                        else
                        {
                            if (FromData.OID == FromTdmxData[0].OID)
                            {
                                if (FromData.Division == Common.Constant.EBomConstant.DIV_SINGLE)
                                {
                                    FromData.Sel_Revision = SemsUtil.SingleMakeMajorRevisonUp(FromData.Sel_Revision);
                                    string SelRev = FromData.Name.Substring(0, (FromData.Name.Length - 1));
                                    FromData.Name = SelRev + FromData.Sel_Revision;
                                }
                                NewFromOID = DObjectRepository.ReviseDObject(Session, new DObject { OID = FromData.OID, Name = FromData.Name });

                                HttpFileRepository.ReviseFiles(Session, new HttpFile { OID = FromData.OID }, NewFromOID);

                                FromData.OID = NewFromOID;
                                DaoFactory.SetInsert("EBom.InsEPart", FromData);
                            }
                            else
                            {
                                NewFromOID = Convert.ToInt32(FromTdmxData[0].OID);
                            }
                            applyoid.Add(Convert.ToInt32(v.FromOID), NewFromOID);
                        }
                    }

                    if (ToData != null)
                    {
                        ToData = null;
                    }
                    ToData = EPartRepository.SelEPartObject(Session, new EPart { OID = v.ToOID });
                    if (ToTdmxData != null)
                    {
                        ToTdmxData = null;
                    }
                    ToTdmxData = EPartRepository.SelEPart(Session, new EPart { TdmxOID = ToData.TdmxOID });
                    ToTdmxData = ToTdmxData.OrderByDescending(revVal => revVal.Revision).ToList();

                    if (!applyoid.ContainsKey(Convert.ToInt32(ToData.OID)))
                    {
                        if (NewDataOID != null && RootOID == ToData.OID)
                        {
                            applyoid.Add(Convert.ToInt32(RootOID), Convert.ToInt32(NewDataOID));
                        }
                        else
                        {
                            if (ToData.OID == ToTdmxData[0].OID)
                            {
                                if (ToData.Division == Common.Constant.EBomConstant.DIV_SINGLE)
                                {
                                    ToData.Sel_Revision = SemsUtil.SingleMakeMajorRevisonUp(ToData.Sel_Revision);
                                    string SelRev = ToData.Name.Substring(0, (ToData.Name.Length - 1));
                                    ToData.Name = SelRev + ToData.Sel_Revision;
                                }
                                NewToOID = DObjectRepository.ReviseDObject(Session, new DObject { OID = ToData.OID, Name = ToData.Name });

                                HttpFileRepository.ReviseFiles(Session, new HttpFile { OID = ToData.OID }, NewToOID);

                                ToData.OID = NewToOID;
                                DaoFactory.SetInsert("EBom.InsEPart", ToData);
                            }
                            else
                            {
                                NewToOID = Convert.ToInt32(ToTdmxData[0].OID);
                            }
                            applyoid.Add(Convert.ToInt32(v.ToOID), NewToOID);
                        }
                    }
                });

                AllStructure.ForEach(v =>
                {
                    EBOM NewRel = new EBOM();

                    if (applyoid.ContainsKey(Convert.ToInt32(v.FromOID)))
                    {
                        NewRel.FromOID = applyoid[Convert.ToInt32(v.FromOID)];
                    }
                    else
                    {
                        NewRel.FromOID = v.FromOID;
                    }

                    if (applyoid.ContainsKey(Convert.ToInt32(v.ToOID)))
                    {
                        NewRel.ToOID = applyoid[Convert.ToInt32(v.ToOID)];
                    }
                    else
                    {
                        NewRel.ToOID = v.ToOID;
                    }

                    EBOM SelParentEBom = DaoFactory.GetData<EBOM>("EBom.SelEBom", NewRel);
                    if(SelParentEBom != null)
                    {
                        return;
                        //relationoid.Add(Convert.ToInt32(v.OID), Convert.ToInt32(SelParentEBom.OID));
                    }
                    else
                    {
                        if (!relationoid.ContainsKey(Convert.ToInt32(v.OID)))
                        {
                            if (applyoid.ContainsKey(Convert.ToInt32(v.FromOID)))
                            {
                                NewRel.Count = v.Count;
                                NewRel.Ord = v.Ord;
                                NewRelOID = EBomRepository.AddAction(Session, NewRel);
                            }
                            relationoid.Add(Convert.ToInt32(v.OID), NewRelOID);
                        }
                    }
                });

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

        #region 정전개 에서 개정
        public JsonResult EPartStructureUpdate(EBOM _param)
        {
            int? resultOID = 0;
            EPart Data = new EPart();
            try
            {
                DaoFactory.BeginTransaction();
                Data = EPartRepository.SelEPartObject(Session, new EPart { OID = _param.ToOID });

                var TdmxData = EPartRepository.SelEPart(Session, new EPart { TdmxOID = Data.TdmxOID });
                TdmxData = TdmxData.OrderByDescending(revVal => revVal.Revision).ToList();
                if (TdmxData[0].OID != Data.OID)
                {
                    DaoFactory.Rollback();
                    return Json(new ResultJsonModel { isError = true, resultMessage = "이미 개정 된 품번이 존재합니다", resultDescription = "이미 개정 된 품번이 존재합니다" });
                }

                if (Data.Division == Common.Constant.EBomConstant.DIV_SINGLE)
                {
                    Data.Sel_Revision = SemsUtil.SingleMakeMajorRevisonUp(Data.Sel_Revision);
                    string SelRev = Data.Name.Substring(0, (Data.Name.Length - 1));
                    Data.Name = SelRev + Data.Sel_Revision;
                }

                resultOID = DObjectRepository.ReviseDObject(Session, new DObject { OID = _param.ToOID, Name = Data.Name });

                HttpFileRepository.ReviseFiles(Session, new HttpFile { OID = _param.ToOID }, resultOID);

                Data.OID = resultOID;
               
                DaoFactory.SetInsert("EBom.InsEPart", Data);

                if (_param.OID != null)
                {
                    EBOM SelParentEBom = DaoFactory.GetData<EBOM>("EBom.SelEBom", new EBOM { OID = _param.OID });
                    List<EBOM> SelChlidEBom = DaoFactory.GetList<EBOM>("EBom.SelEBom", new EBOM { FromOID = _param.ToOID });

                    EBOM NewData = new EBOM();

                    NewData.OldOID = SelParentEBom.OID;
                    NewData.FromOID = SelParentEBom.FromOID;
                    NewData.ToOID = resultOID;
                    NewData.Count = SelParentEBom.Count;
                    NewData.Ord = SelParentEBom.Ord;

                    EBomRepository.ReAction(Session, NewData);

                    SelChlidEBom.ForEach(v =>
                    {
                        v.FromOID = resultOID;
                        EBomRepository.AddAction(Session, v);
                    });
                }
                else if (_param.ToOID != null)
                {
                    List<EBOM> SelChlidEBom = DaoFactory.GetList<EBOM>("EBom.SelEBom", new EBOM { FromOID = _param.ToOID });

                    SelChlidEBom.ForEach(v =>
                    {
                        v.FromOID = resultOID;
                        EBomRepository.AddAction(Session, v);
                    });
                }

                DaoFactory.Commit();
            }
            catch (Exception ex)
            {
                DaoFactory.Rollback();
                return Json(new ResultJsonModel { isError = true, resultMessage = ex.Message, resultDescription = ex.ToString() });
            }
            return Json(Data);
        }
        #endregion

        #region 자신 기준 이전 리비전 품번 리스트 검색
        public JsonResult SelEPartRevPrevious(EPart _param)
        {
            var Data = DObjectRepository.SelDObject(Session, new DObject { OID = Convert.ToInt32(_param.OID) });
            List<DObject> Tdmx = DObjectRepository.SelDObjects(Session, new DObject { TdmxOID = Data.TdmxOID });
            List<DObject> appyStruct = new List<DObject>();

            Tdmx.ForEach(Obj =>
            {
                List<DObject> callStruct = new List<DObject>();
                callStruct = Tdmx.FindAll(value => value.TdmxOID == Obj.TdmxOID && value.Revision != Obj.Revision);

                foreach (DObject appy in callStruct)
                {
                    if (appyStruct.FindIndex(value => value.TdmxOID == appy.TdmxOID && value.OID > appy.OID) < 0)
                    {
                        appyStruct.Add(appy);
                    }
                }

                appyStruct = appyStruct.OrderByDescending(revVal => revVal.Revision).ToList();

            });

            foreach (DObject i in appyStruct)
            {
                Tdmx.RemoveAll(value => value.OID == i.OID);
            }

            return Json(Tdmx);
        }
        #endregion

        #region 연관된 설계변경 검색
        public JsonResult SelRelationECO(EPart _param)
        {
            EO lEO = EORepository.SelEOContentsObject(new EO { ToOID = _param.OID, Type = Common.Constant.EoConstant.TYPE_EBOM_LIST });
            List<ECO> ECODetail = new List<ECO>();
            if (lEO != null)
            {
                ECODetail = ECORepository.SelChangeOrder(Session, new ECO { OID = lEO.RootOID });
            }

            return Json(ECODetail);
        }
        #endregion

        #region 정전개 개정
        public JsonResult SelTempEPart(EPart _param)
        {
            EPart TempData = EPartRepository.SelTempEPartObject(Session, _param);
            if(TempData != null)
            {
                return Json(0);
            }

            EPart RootData = EPartRepository.SelEPartObject(Session, _param);

            List<EPart> TdmxData = EPartRepository.SelEPart(Session, new EPart { TdmxOID = RootData.TdmxOID });
            if(RootData.OID != TdmxData[0].OID)
            {
                return Json(0);
            }

            RootData.Revision = SemsUtil.MakeMajorRevisonUp(RootData.Revision);

            if (RootData.Division == Common.Constant.EBomConstant.DIV_SINGLE)
            {
                RootData.Sel_Revision = SemsUtil.SingleMakeMajorRevisonUp(RootData.Sel_Revision);
                string SelRev = RootData.Name.Substring(0, (RootData.Name.Length - 1));
                RootData.Name = SelRev + RootData.Sel_Revision;
            }
            return Json(RootData);
        }
        #endregion

        #region CopyToBom
        public JsonResult CopyToBom(EPart _param)
        {
            try
            {
                DaoFactory.BeginTransaction();

                List<EBOM> SelOldEBom = DaoFactory.GetList<EBOM>("EBom.SelEBom", new EBOM { FromOID = _param.OldOID });

                SelOldEBom.ForEach(v =>
                {
                    v.FromOID = _param.OID;
                    EBomRepository.AddAction(Session, v);
                });

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


        #region 하위에 붙을때 구조상 재귀 오류가 있을 수 있는 품번 체크
        public JsonResult CheckRecursionError(EBOM _param)
        {
            var result = 0;
            try
            {
                DaoFactory.BeginTransaction();

                //OID로 부모 OID 전부 찾기
                //SelAllParentStructure
                EBOM Parent = new EBOM();
                EBOM Child = new EBOM();


                List<EBOM> SelAllParentStructure = DaoFactory.GetList<EBOM>("EBom.SelAllParentStructure", new EBOM { ToOID = _param.FromOID });
                Parent.FromOID = _param.FromOID;
                SelAllParentStructure.Add(Parent);

                List<EPart> ParentData = new List<EPart>();
                List<EPart> ChildData = new List<EPart>();

                SelAllParentStructure.ForEach(v =>
                {
                    ParentData.Add(EPartRepository.SelEPartObject(Session, new EPart { OID = v.FromOID }));
                });


                //OID로 자식 OID 전부 찾기
                //SelRootChildEBomList
                List<EBOM> SelRootChildEBomList = DaoFactory.GetList<EBOM>("EBom.SelRootChildEBomList", new EBOM { FromOID = _param.ToOID });
                Child.FromOID = _param.ToOID;
                SelRootChildEBomList.Add(Child);

                SelRootChildEBomList.ForEach(v => {
                    ChildData.Add(EPartRepository.SelEPartObject(Session, new EPart { OID = v.FromOID }));
                });

                ChildData.ForEach(item => {
                    if (ParentData.FindAll(v => v.TdmxOID == item.TdmxOID).Count > 0)
                    {
                        result = 1;
                        return;
                    }
                });


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

    }
}