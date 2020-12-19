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
        #region -- EPart View
        public ActionResult EBomStructure()
        {
            return View();
        }
        public ActionResult CreateEPart()
        {
            Library ItemKey = LibraryRepository.SelCodeLibraryObject(new Library { Code1 = "ITEM" });
            Library placeKey = LibraryRepository.SelLibraryObject(new Library { Name = "PRODUCED_PLACE" });
            Library epartKey = LibraryRepository.SelLibraryObject(new Library { Name = "EPARTTYPE" });
            Library oemKey = LibraryRepository.SelCodeLibraryObject(new Library { Code1 = "OEM" });
            Library psizeKey = LibraryRepository.SelLibraryObject(new Library { Name = "PSIZE" });

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
            Library ItemKey = LibraryRepository.SelCodeLibraryObject(new Library { Code1 = "ITEM" });
            Library oemKey = LibraryRepository.SelCodeLibraryObject(new Library { Code1 = "OEM" });


            List<Library> ItemList = LibraryRepository.SelCodeLibrary(new Library { FromOID = ItemKey.OID });  //OEM 목록
            List<Library> oemList = LibraryRepository.SelCodeLibrary(new Library { FromOID = oemKey.OID });  //차종 목록

            ViewBag.oemList = oemList;
            ViewBag.ItemList = ItemList;
            return View();
        }
        public ActionResult InfoEPart(int OID)
        {
            ViewBag.OID = OID;
            EPart InfoEPart = EPartRepository.SelEPartObject(new EPart { OID = OID });
            ViewBag.Status = BPolicyRepository.SelBPolicy(new BPolicy { Type = EBomConstant.TYPE_PART });

            Library ItemKey = LibraryRepository.SelCodeLibraryObject(new Library { Code1 = "ITEM" });
            Library placeKey = LibraryRepository.SelLibraryObject(new Library { Name = "PRODUCED_PLACE" });
            Library epartKey = LibraryRepository.SelLibraryObject(new Library { Name = "EPARTTYPE" });
            Library oemKey = LibraryRepository.SelCodeLibraryObject(new Library { Code1 = "OEM" });
            Library psizeKey = LibraryRepository.SelLibraryObject(new Library { Name = "PSIZE" });

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
            Library ItemKey = LibraryRepository.SelCodeLibraryObject(new Library { Code1 = "ITEM" });
            Library oemKey = LibraryRepository.SelCodeLibraryObject(new Library { Code1 = "OEM" });


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
            Library ItemKey = LibraryRepository.SelCodeLibraryObject(new Library { Code1 = "ITEM" });
            Library oemKey = LibraryRepository.SelCodeLibraryObject(new Library { Code1 = "OEM" });


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
            Library ItemKey = LibraryRepository.SelCodeLibraryObject(new Library { Code1 = "ITEM" });
            Library oemKey = LibraryRepository.SelCodeLibraryObject(new Library { Code1 = "OEM" });


            List<Library> ItemList = LibraryRepository.SelCodeLibrary(new Library { FromOID = ItemKey.OID });  //OEM 목록
            List<Library> oemList = LibraryRepository.SelCodeLibrary(new Library { FromOID = oemKey.OID });  //차종 목록

            ViewBag.oemList = oemList;
            ViewBag.ItemList = ItemList;
            return PartialView("Dialog/dlgSearchEPart");
        }
        public ActionResult dlgCreateEPart()
        {
            Library ItemKey = LibraryRepository.SelCodeLibraryObject(new Library { Code1 = "ITEM" });
            Library placeKey = LibraryRepository.SelLibraryObject(new Library { Name = "PRODUCED_PLACE" });
            Library epartKey = LibraryRepository.SelLibraryObject(new Library { Name = "EPARTTYPE" });
            Library oemKey = LibraryRepository.SelCodeLibraryObject(new Library { Code1 = "OEM" });
            Library psizeKey = LibraryRepository.SelLibraryObject(new Library { Name = "PSIZE" });

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
            return PartialView("Dialog/dlgCreateEPart");
        }
        #endregion

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
            DObject dobj = new DObject();
            try
            {
                DaoFactory.BeginTransaction();
                var check = CreateEPartChk(_param);
                if (check ==1)
                {
                    DaoFactory.Rollback();
                    return Json(new ResultJsonModel { isError = true, resultMessage = "품번이 이미 존재합니다.", resultDescription = "" });
                }
                
                dobj.Type = EBomConstant.TYPE_PART;
                dobj.TableNm = EBomConstant.TABLE_PART;
                dobj.Name = _param.Name;
                dobj.Description = _param.Description;
                dobj.Thumbnail = _param.Thumbnail;
                resultOid = DObjectRepository.InsDObject(Session, dobj);

                _param.OID = resultOid;

                DaoFactory.SetInsert("EBom.InsEPart", _param);

                DaoFactory.Commit();
            }
            catch (Exception ex)
            {
                DaoFactory.Rollback();
                return Json(new ResultJsonModel { isError = true, resultMessage = ex.Message, resultDescription = ex.ToString() });
            }
            return Json(dobj);
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
            DObjectRepository.UdtDObject(Session, _param);
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
                if (Data.Action == "U")
                {
                    EBomRepository.UdtAction(Data);
                }
            }
            _param.RemoveAll(VALUE => VALUE.Action == "U");

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

        #region EPart Compare
        //public JsonResult EPartCompare(int? LOID, int? ROID)
        //{
        //    List<EPart> CompareEBom = getListCompareEbom(0, LOID, ROID);
        //    return Json(CompareEBom);
        //}
        public JsonResult EPartCompare(int? LOID, int? ROID)
        {
            int level = 0;
            List<EBOM> CompareList = new List<EBOM>();
        
            EPart LPataData = EPartRepository.SelEPartObject(new EPart { OID = LOID });
            EPart RPataData = EPartRepository.SelEPartObject(new EPart { OID = ROID });

            //EBOM LEom = EPartRepository.getListEbom(0, Convert.ToInt32(LOID));
            //EBOM REom = EPartRepository.getListEbom(0, Convert.ToInt32(ROID));

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

            //EBomStructureToList(LEom, LList, level);
            //EBomStructureToList(REom, RList, level);

            //List<EBOM> LEPartList = EPartRepository.SelRootChildEBomList(new EBOM { FromOID = Convert.ToInt32(LOID) });
            //List<EBOM> REPartList = EPartRepository.SelRootChildEBomList(new EBOM { FromOID = Convert.ToInt32(ROID) });
            //
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
           //if(CheckCompareList.Count > 1)
           //{
           //    getCheckCompareEPartStructure(getCompareStructure, LOID, ROID, CompareList, CheckCompareList);
           //}
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

                    EPart LDetail = EPartRepository.SelEPartObject(new EPart { OID = tmpCompareItem.ToOID });
                    tmpCompareItem.LOId = LDetail.OID;
                    tmpCompareItem.LName = LDetail.Name;
                    tmpCompareItem.LType = LDetail.Type;
                    //tmpCompareItem.LId = LDetail.OID;
                    tmpCompareItem.LOrd = lRelation[j].Ord;
                    tmpCompareItem.LRevision = LDetail.Revision;
                    tmpCompareItem.LCar_Lib_OID = LDetail.Car_Lib_OID;
                    tmpCompareItem.LCar_Lib_NM = LDetail.Car_Lib_Nm;
                    tmpCompareItem.LThumbnail = LDetail.Thumbnail;


                    EPart RDetail = EPartRepository.SelEPartObject(new EPart { OID = tmpCompareItem.RToOld });
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
                    EPart LDetail = EPartRepository.SelEPartObject(new EPart { OID = tmpCompareItem.ToOID });
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
                        RDetail = EPartRepository.SelEPartObject(new EPart { OID = tmpCompareItem.RToOld });
                    }
                    else
                    {
                        RDetail = EPartRepository.SelEPartObject(new EPart { OID = tmpCompareItem.ToOID });
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

                    EPart LDetail = EPartRepository.SelEPartObject(new EPart { OID = tmpCompareItem.ToOID });
                    tmpCompareItem.LOId = LDetail.OID;
                    tmpCompareItem.LName = LDetail.Name;
                    tmpCompareItem.LType = LDetail.Type;
                    //tmpCompareItem.LId = LDetail.OID;
                    tmpCompareItem.LOrd = lRelation[j].Ord;


                    EPart RDetail = EPartRepository.SelEPartObject(new EPart { OID = tmpCompareItem.RToOld });
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
                    EPart LDetail = EPartRepository.SelEPartObject(new EPart { OID = tmpCompareItem.ToOID });
                    tmpCompareItem.LOId = LDetail.OID;
                    tmpCompareItem.LName = LDetail.Name;
                    tmpCompareItem.LType = LDetail.Type;
                    //tmpCompareItem.LId = LDetail.OID;
                    tmpCompareItem.LOrd = lRelation[j].Ord;
                }
                else if (tmpCompareItem.Action.Equals(PmsConstant.ACTION_RIGHT))
                {
                    tmpCompareItem.Action = PmsConstant.ACTION_DELETE_NM;
                    EPart RDetail = EPartRepository.SelEPartObject(new EPart { OID = tmpCompareItem.RToOld });
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

    }
}