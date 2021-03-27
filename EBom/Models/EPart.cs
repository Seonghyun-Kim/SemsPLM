﻿using Common.Interface;
using Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Constant;
using Common.Factory;
using System.Security.Cryptography;
using System.Web;
using Common.Models.File;

namespace EBom.Models
{
    public class EPart : DObject, IDObject, IObjectFile
    {

        public string Rep_Part_No { get; set; }
        public string Rep_Part_No2 { get; set; }
        public string Eo_No { get; set; }
        public DateTime? Eo_No_ApplyDt { get; set; }
        public string Eo_No_History { get; set; }
        public string Etc { get; set; }
        public int? ApprovOID { get; set; }
        public string Sel_Eo { get; set; }
        public DateTime? Sel_Eo_Dt { get; set; }
        public string Spec { get; set; }
        public string Surface { get; set; }
        public int? Pms_OID { get; set; }
        public int? Prod_Lib_Lev1_OID { get; set; }
        public int? Prod_Lib_Lev2_OID { get; set; }
        public int? Prod_Lib_Lev3_OID { get; set; }
        public string CO { get; set; }
        public string Etc_Delivery { get; set; }
        
        public string Prod_Lib_Lev1_NM { get; set; }
        public string Prod_Lib_Lev2_NM { get; set; }
        public string Prod_Lib_Lev3_NM { get; set; }



        public string Pms_NM { get; set; }
        public string TaskNm { get; set; }
        public string Title { get; set; }
        public int? EPartType { get; set; } //제품 구분
        public int? Oem_Lib_OID { get; set; }
        public int? Car_Lib_OID { get; set; } //차종
        public string Oem_Lib_Nm { get; set; }
        public string Car_Lib_Nm { get; set; }
        public int? Material_OID { get; set; }
        public string Material_Nm { get; set; }
        public string Division { get; set; } //구분
        public string DivisionNm { get; set; } //구분 한글명
        public int? ITEM_No { get; set; } //ITEM_NO
        public string ITEM_NoNm { get; set; } //ITEM_NO
        public int? ITEM_Middle { get; set; } //ITEM_Middle
        public string ITEM_MiddleNm { get; set; }
        public int? Production_Place { get; set; } //생산지 PRODUCTION_PLACE
        public string Production_PlaceNm { get; set; }
        public int? Block_No { get; set; } //BLOCK_NO
        public string Block_NoNm { get; set; } //BLOCK_NONm
        public string Serial { get; set; } //시리얼 
        public string Sel_Revision { get; set; } //고객 리비전
        public string Standard { get; set; } //규격
        public string EPartTypeNm { get; set; }
        public int? RootOID { get; set; }
        public int? FromOID { get; set; }
        public int? ToOID { get; set; }
        public List<HttpPostedFileBase> Files { get; set; }
        public List<HttpFile> delFiles { get; set; }
        public int? isDuplicate { get; set; }
        public string StartCreateDt { get; set; }
        public string EndCreateDt { get; set; }
        public int? Orgin_OID { get; set; }
        public string NewCheck { get; set; }
        public int? OldOID { get; set; }
    }

    public static class EPartRepository
    {
        #region 1레벨 부모 미포함 EPart 리스트 검색
        public static List<EPart> SelNot1LvParentEPart(HttpSessionStateBase Context, EPart _param)
        {
            _param.Type = EBomConstant.TYPE_PART;
            List<EPart> lEPart = DaoFactory.GetList<EPart>("EBom.SelNot1LvParentEPart", _param);
            List<EPart> ViewEPart = new List<EPart>();
            lEPart.ForEach(obj =>
            {
                if (obj.Car_Lib_OID != null)
                {
                    obj.Car_Lib_Nm = LibraryRepository.SelCodeLibraryObject(new Library { OID = obj.Car_Lib_OID }).KorNm;
                }
                if (obj.Block_No != null)
                {
                    if (obj.Division == Common.Constant.EBomConstant.DIV_ASSEMBLY)
                    {
                        obj.DivisionNm = Common.Constant.EBomConstant.DIV_ASSEMBLY_NM;
                        obj.Block_NoNm = LibraryRepository.SelCodeLibraryObject(new Library { OID = obj.Block_No }).KorNm;
                    }
                    else
                    {
                        if (obj.Division == Common.Constant.EBomConstant.DIV_SINGLE)
                        {
                            obj.DivisionNm = Common.Constant.EBomConstant.DIV_SINGLE_NM;
                        }
                        else
                        {
                            obj.DivisionNm = Common.Constant.EBomConstant.DIV_STANDARD_NM;
                        }
                        obj.Block_NoNm = LibraryRepository.SelLibraryObject(new Library { OID = obj.Block_No }).KorNm;
                    }
                }
                if (obj.Oem_Lib_OID != null)
                {
                    obj.Oem_Lib_Nm = LibraryRepository.SelCodeLibraryObject(new Library { OID = obj.Oem_Lib_OID }).KorNm;
                }
                if (obj.Material_OID != null)
                {
                    obj.Material_Nm = LibraryRepository.SelLibraryObject(new Library { OID = obj.Material_OID }).KorNm;
                }
                if (obj.ITEM_No != null)
                {
                    obj.ITEM_NoNm = LibraryRepository.SelCodeLibraryObject(new Library { OID = obj.ITEM_No }).KorNm;
                }
                if (obj.EPartType != null)
                {
                    obj.EPartTypeNm = LibraryRepository.SelLibraryObject(new Library { OID = obj.EPartType }).KorNm;
                }
                obj.CreateUsNm = PersonRepository.SelPerson(Context, new Person { OID = obj.CreateUs }).Name;
                obj.BPolicy = BPolicyRepository.SelBPolicy(new BPolicy { Type = obj.Type, OID = obj.BPolicyOID }).First();
                obj.BPolicyNm = obj.BPolicy.Name;
                obj.BPolicyAuths = BPolicyAuthRepository.MainAuth(Context, obj, null);

                if (obj.BPolicyAuths.FindAll(item => item.AuthNm == CommonConstant.AUTH_VIEW).Count > 0)
                {
                    ViewEPart.Add(obj);
                }
            });
            return ViewEPart;
        }
        #endregion


        #region EPart 리스트 검색
        public static List<EPart> SelEPart(HttpSessionStateBase Context, EPart _param)
        {
            _param.Type = EBomConstant.TYPE_PART;
            List<EPart> lEPart = DaoFactory.GetList<EPart>("EBom.SelEPart", _param);
            List<EPart> ViewEPart = new List<EPart>();
            lEPart.ForEach(obj =>
            {
                if (obj.Car_Lib_OID != null)
                {
                    obj.Car_Lib_Nm = LibraryRepository.SelCodeLibraryObject(new Library { OID = obj.Car_Lib_OID }).KorNm;
                }
                if (obj.Block_No != null)
                {
                    if (obj.Division == Common.Constant.EBomConstant.DIV_ASSEMBLY)
                    {
                        obj.DivisionNm = Common.Constant.EBomConstant.DIV_ASSEMBLY_NM;
                        obj.Block_NoNm = LibraryRepository.SelCodeLibraryObject(new Library { OID = obj.Block_No }).KorNm;
                    }
                    else
                    {
                        if (obj.Division == Common.Constant.EBomConstant.DIV_SINGLE)
                        {
                            obj.DivisionNm = Common.Constant.EBomConstant.DIV_SINGLE_NM;
                        }
                        else
                        {
                            obj.DivisionNm = Common.Constant.EBomConstant.DIV_STANDARD_NM;
                        }
                        obj.Block_NoNm = LibraryRepository.SelLibraryObject(new Library { OID = obj.Block_No }).KorNm;
                    }
                }
                if (obj.Oem_Lib_OID != null)
                {
                    obj.Oem_Lib_Nm = LibraryRepository.SelCodeLibraryObject(new Library { OID = obj.Oem_Lib_OID }).KorNm;
                }
                if (obj.Material_OID != null)
                {
                    obj.Material_Nm = LibraryRepository.SelLibraryObject(new Library { OID = obj.Material_OID }).KorNm;
                }
                if (obj.ITEM_No != null)
                {
                    obj.ITEM_NoNm = LibraryRepository.SelCodeLibraryObject(new Library { OID = obj.ITEM_No }).KorNm;
                }
                if (obj.EPartType != null)
                {
                    obj.EPartTypeNm = LibraryRepository.SelLibraryObject(new Library { OID = obj.EPartType }).KorNm;
                }
                obj.CreateUsNm = PersonRepository.SelPerson(Context, new Person { OID = obj.CreateUs }).Name;
                obj.BPolicy = BPolicyRepository.SelBPolicy(new BPolicy { Type = obj.Type, OID = obj.BPolicyOID }).First();
                obj.BPolicyNm = obj.BPolicy.Name;
                obj.BPolicyAuths = BPolicyAuthRepository.MainAuth(Context, obj, null);

                if (obj.BPolicyAuths.FindAll(item => item.AuthNm == CommonConstant.AUTH_VIEW).Count > 0)
                {
                    ViewEPart.Add(obj);
                }
            });
            return ViewEPart;
        }
        #endregion

        #region EPart 오브젝트 검색
        public static EPart SelEPartObject(HttpSessionStateBase Context, EPart _param)
        {
            _param.Type = EBomConstant.TYPE_PART;
            EPart lEPart = DaoFactory.GetData<EPart>("EBom.SelEPart", _param);
            if(lEPart != null)
            {
                lEPart.CreateUsNm = PersonRepository.SelPerson(Context, new Person { OID = lEPart.CreateUs }).Name;
                lEPart.BPolicy = BPolicyRepository.SelBPolicy(new BPolicy { Type = lEPart.Type, OID = lEPart.BPolicyOID }).First();
                lEPart.BPolicyNm = lEPart.BPolicy.Name;
                lEPart.BPolicyAuths = BPolicyAuthRepository.MainAuth(Context, lEPart, null);

                if (lEPart.ITEM_No != null)
                {
                    lEPart.ITEM_NoNm = LibraryRepository.SelCodeLibraryObject(new Library { OID = lEPart.ITEM_No }).KorNm;
                }
                if (lEPart.ITEM_Middle != null)
                {
                    lEPart.ITEM_MiddleNm = LibraryRepository.SelCodeLibraryObject(new Library { OID = lEPart.ITEM_Middle }).KorNm;
                }
                if (lEPart.EPartType != null)
                {
                    lEPart.EPartTypeNm = LibraryRepository.SelLibraryObject(new Library { OID = Convert.ToInt32(lEPart.EPartType) }).KorNm;
                }
                if (lEPart.Production_Place != null)
                {
                    lEPart.Production_PlaceNm = LibraryRepository.SelLibraryObject(new Library { OID = lEPart.Production_Place }).KorNm;
                }
                if (lEPart.Block_No != null)
                {
                    if (lEPart.Division == Common.Constant.EBomConstant.DIV_ASSEMBLY)
                    {
                        lEPart.DivisionNm = Common.Constant.EBomConstant.DIV_ASSEMBLY_NM;
                        lEPart.Block_NoNm = LibraryRepository.SelCodeLibraryObject(new Library { OID = lEPart.Block_No }).KorNm;
                    }
                    else
                    {
                        if (lEPart.Division == Common.Constant.EBomConstant.DIV_SINGLE)
                        {
                            lEPart.DivisionNm = Common.Constant.EBomConstant.DIV_SINGLE_NM;
                        }
                        else
                        {
                            lEPart.DivisionNm = Common.Constant.EBomConstant.DIV_STANDARD_NM;
                        }
                        lEPart.Block_NoNm = LibraryRepository.SelLibraryObject(new Library { OID = lEPart.Block_No }).KorNm;
                    }
                }
                if (lEPart.Oem_Lib_OID != null)
                {
                    lEPart.Oem_Lib_Nm = LibraryRepository.SelCodeLibraryObject(new Library { OID = lEPart.Oem_Lib_OID }).KorNm;
                }
                if (lEPart.Car_Lib_OID != null)
                {
                    lEPart.Car_Lib_Nm = LibraryRepository.SelCodeLibraryObject(new Library { OID = lEPart.Car_Lib_OID }).KorNm;
                }
                if (lEPart.Material_OID != null)
                {
                    lEPart.Material_Nm = LibraryRepository.SelLibraryObject(new Library { OID = lEPart.Material_OID }).KorNm;
                }
                if (lEPart.EPartType != null)
                {
                    lEPart.EPartTypeNm = LibraryRepository.SelLibraryObject(new Library { OID = lEPart.EPartType }).KorNm;
                }

            }

            return lEPart;
        }
        #endregion

        #region EPart 품번 중복체크
        public static EPart ChkEPart(EPart _param)
        {
            EPart lPartObj = new EPart();
            if (_param.Division == Common.Constant.EBomConstant.DIV_SINGLE)
            {
                lPartObj = DaoFactory.GetData<EPart>("EBom.ChkEPartDivision", new EPart { Type = EBomConstant.TYPE_PART, Name = _param.Name });
            }
            else
            {
                lPartObj = DaoFactory.GetData<EPart>("EBom.ChkEPart", new EPart { Type = EBomConstant.TYPE_PART, Name = _param.Name });
            }
            return lPartObj;
        }
        #endregion

        #region EBOM 구조 검색
        public static List<EBOM> EBomStructure(EBOM _param)
        {
            List<EBOM> EBom = DaoFactory.GetList<EBOM>("EBom.SelEBom", new EBOM { Type = EBomConstant.TYPE_PART, FromOID = _param.FromOID });
            return EBom;
        }
        #endregion

        #region EPart 정전개
        public static EBOM getListEbom(HttpSessionStateBase Context, int _level, int _rootOID)
        {
            EBOM getStructure = new EBOM();
            getStructure.Level = _level;
            getStructure.ToOID = _rootOID;
            getStructure.TimeOID = 0;

            EPart PataData = SelEPartObject(Context, new EPart { OID = _rootOID });
            getStructure.OID = 0;
            getStructure.ToData = PataData;
            getStructure.Description = PataData.Description;
            getStructure.ObjName = PataData.Name;
            getStructure.ObjRevision = PataData.Revision;
            getStructure.ObjTdmxOID = PataData.TdmxOID;
            getStructure.ObjIsLatest = PataData.IsLatest;

            getStructure.ObjEPartType = PataData.EPartType;
            getStructure.ObjEPartTypeNm = PataData.EPartTypeNm;

            getStructure.ObjThumbnail = PataData.Thumbnail;
            getStructure.ObjCar_Lib_OID = PataData.Car_Lib_OID;
            getStructure.ObjCar_Lib_Nm = PataData.Car_Lib_Nm;
            getStructure.ObjDivision = PataData.Division;
            getStructure.ObjDivisionNm = PataData.DivisionNm;
            getStructure.ObjITEM_No = PataData.ITEM_No;
            getStructure.ObjITEM_NoNm = PataData.ITEM_NoNm;
            getStructure.ObjITEM_Middle = PataData.ITEM_Middle;
            getStructure.ObjITEM_MiddleNm = PataData.ITEM_MiddleNm;
            getStructure.ObjProduction_Place = PataData.Production_Place;
            getStructure.ObjProduction_PlaceNm = PataData.Production_PlaceNm;
            getStructure.ObjMaterial_OID = PataData.Material_OID;
            getStructure.ObjMaterial_Nm = PataData.Material_Nm;
            getStructure.ObjBlock_No = PataData.Block_No;
            getStructure.ObjBlock_NoNm = PataData.Block_NoNm;
            getStructure.ObjSerial = PataData.Serial;
            getStructure.ObjSel_Revision = PataData.Sel_Revision;

            getStructure.ObjTitle = PataData.Title;
            getStructure.ObjSel_Revision = PataData.Sel_Revision;
            getStructure.ObjStandard = PataData.Standard;
            getStructure.ObjOem_Lib_Nm = PataData.Oem_Lib_Nm;

            getStructure.BPolicy = PataData.BPolicy;
            getStructure.BPolicyNm = PataData.BPolicy.StatusNm;

            getStructure.diseditable = EBomConstant.DISEDITABLE;

            getEbomStructure(Context, getStructure, _rootOID, getStructure.TimeOID);
            return getStructure;
        }

        public static void getEbomStructure(HttpSessionStateBase Context, EBOM _relData, int _rootOID, int? TimeOID)
        {

            _relData.Children = getEbom(Context, new EBOM { Type = EBomConstant.TYPE_PART, FromOID = _relData.ToOID }, _rootOID, TimeOID);

            _relData.Children.ForEach(item =>
            {
                item.Level = _relData.Level + 1;
                item.TimeOID = item.Level + item.TimeOID;
                getEbomStructure(Context, item, _rootOID, item.TimeOID);
            });
        }

        public static List<EBOM> getEbom(HttpSessionStateBase Context, EBOM _param, int _rootOID, int? TimeOID)
        {
            List<EBOM> EBom = DaoFactory.GetList<EBOM>("EBom.SelEBom", _param);
            EPart PataData = new EPart();

            List<EBOM> childTempList = new List<EBOM>();
            List<EBOM> appyStruct = new List<EBOM>();
            List<EBOM> callStruct = new List<EBOM>();
            EBOM callObj = new EBOM();

            foreach (EBOM Obj in EBom)
            {
                if (PataData != null)
                {
                    PataData = null;
                }
                PataData = SelEPartObject(Context, new EPart { OID = Obj.ToOID });
                Obj.ObjRevision = PataData.Revision;
                Obj.ObjTdmxOID = PataData.TdmxOID;

                TimeOID = TimeOID + 1;
                Obj.TimeOID = TimeOID;
                Obj.ToData = PataData;
                Obj.Description = PataData.Description;
                Obj.ObjName = PataData.Name;
                Obj.ObjRevision = PataData.Revision;
                Obj.ObjTdmxOID = PataData.TdmxOID;
                Obj.ObjIsLatest = PataData.IsLatest;
                Obj.ObjEPartType = PataData.EPartType;
                Obj.ObjEPartTypeNm = PataData.EPartTypeNm;
                Obj.ObjThumbnail = PataData.Thumbnail;
                Obj.ObjCar_Lib_OID = PataData.Car_Lib_OID;
                Obj.ObjCar_Lib_Nm = PataData.Car_Lib_Nm;
                Obj.ObjDivision = PataData.Division;
                Obj.ObjDivisionNm = PataData.DivisionNm;
                Obj.ObjITEM_No = PataData.ITEM_No;
                Obj.ObjITEM_NoNm = PataData.ITEM_NoNm;
                Obj.ObjITEM_Middle = PataData.ITEM_Middle;
                Obj.ObjITEM_MiddleNm = PataData.ITEM_MiddleNm;
                Obj.ObjProduction_Place = PataData.Production_Place;
                Obj.ObjProduction_PlaceNm = PataData.Production_PlaceNm;
                Obj.ObjMaterial_OID = PataData.Material_OID;
                Obj.ObjMaterial_Nm = PataData.Material_Nm;
                Obj.ObjBlock_No = PataData.Block_No;
                Obj.ObjBlock_NoNm = PataData.Block_NoNm;
                Obj.ObjSerial = PataData.Serial;
                Obj.ObjSel_Revision = PataData.Sel_Revision;

                Obj.ObjTitle = PataData.Title;
                Obj.ObjSel_Revision = PataData.Sel_Revision;
                Obj.ObjStandard = PataData.Standard;
                Obj.ObjOem_Lib_Nm = PataData.Oem_Lib_Nm;

                Obj.BPolicy = PataData.BPolicy;
                Obj.BPolicyNm = PataData.BPolicy.StatusNm;

                Obj.diseditable = EBomConstant.FLOWEDITABLE;
            }

            foreach (EBOM Obj in EBom)
            {
                if (callStruct != null && callStruct.Count > 0)
                {
                    callStruct = null;
                }
                if (callObj != null)
                {
                    callObj = null;
                }

                callStruct = appyStruct.FindAll(value => value.ObjTdmxOID == Obj.ObjTdmxOID && value.ObjRevision != Obj.ObjRevision);

                if (callStruct.Count > 0)
                {
                    callStruct = callStruct.OrderByDescending(revVal => revVal.ObjRevision).ToList();
                    if(callStruct[0].ToOID < Obj.ToOID)
                    {
                        appyStruct.RemoveAll(v => v.OID == callStruct[0].OID);
                        appyStruct.Add(Obj);
                    }
                }
                else
                {
                    appyStruct.Add(Obj);
                }
            }

            foreach (EBOM i in childTempList)
            {
                EBom.RemoveAll(value => value.OID == i.OID);
            }


            return appyStruct;
        }
        #endregion

        #region EPart 역전개
        public static EBOM getListReverseEbom(HttpSessionStateBase Context, int _level, int _rootOID)
        {
            EBOM getStructure = new EBOM();
            getStructure.Level = _level;
            getStructure.FromOID = _rootOID;

            EPart PataData = SelEPartObject(Context, new EPart { OID = _rootOID });
            getStructure.FromData = PataData;
            getStructure.Description = PataData.Description;
            getStructure.ObjName = PataData.Name;
            getStructure.ObjRevision = PataData.Revision;
            getStructure.ObjTdmxOID = PataData.TdmxOID;
            getStructure.ObjIsLatest = PataData.IsLatest;
            getStructure.ObjTitle = PataData.Title;
            getStructure.ObjRep_Part_No = PataData.Rep_Part_No;
            getStructure.ObjRep_Part_No2 = PataData.Rep_Part_No2;
            getStructure.ObjEo_No = PataData.Eo_No;
            getStructure.ObjEtc = PataData.Etc;
            getStructure.ObjApprovOID = PataData.ApprovOID;
            getStructure.ObjEPartType = PataData.EPartType;
            getStructure.ObjEPartTypeNm = PataData.EPartTypeNm;
            getStructure.ObjDivision = PataData.Division;
            getStructure.ObjDivisionNm = PataData.DivisionNm;
            getStructure.ObjITEM_No = PataData.ITEM_No;
            getStructure.ObjITEM_NoNm = PataData.ITEM_NoNm;
            getStructure.ObjITEM_Middle = PataData.ITEM_Middle;
            getStructure.ObjITEM_MiddleNm = PataData.ITEM_MiddleNm;
            getStructure.ObjProduction_Place = PataData.Production_Place;
            getStructure.ObjProduction_PlaceNm = PataData.Production_PlaceNm;
            getStructure.ObjMaterial_OID = PataData.Material_OID;
            getStructure.ObjMaterial_Nm = PataData.Material_Nm;
            getStructure.ObjBlock_No = PataData.Block_No;
            getStructure.ObjBlock_NoNm = PataData.Block_NoNm;
            getStructure.ObjSel_Eo = PataData.Sel_Eo;
            getStructure.ObjSel_Eo_Dt = PataData.Sel_Eo_Dt;
            getStructure.ObjSpec = PataData.Spec;
            getStructure.ObjSurface = PataData.Surface;
            getStructure.ObjOem_Lib_OID = PataData.Oem_Lib_OID;
            getStructure.ObjCar_Lib_OID = PataData.Car_Lib_OID;
            getStructure.ObjPms_OID = PataData.Pms_OID;


            getStructure.ObjCar_Lib_Nm = PataData.Car_Lib_Nm;

            getStructure.ObjSel_Revision = PataData.Sel_Revision;
            getStructure.ObjStandard = PataData.Standard;
            getStructure.ObjOem_Lib_Nm = PataData.Oem_Lib_Nm;

            getStructure.BPolicy = PataData.BPolicy;
            getStructure.BPolicyNm = PataData.BPolicy.StatusNm;

            getReverseEbomStructure(Context, getStructure, _rootOID);
            return getStructure;
        }

        public static void getReverseEbomStructure(HttpSessionStateBase Context, EBOM _relData, int _rootOID)
        {
            _relData.Parents = getReverseEbom(Context, new EBOM { Type = EBomConstant.TYPE_PART, ToOID = _relData.FromOID }, _rootOID);
            _relData.Parents.ForEach(item =>
            {
                item.Level = _relData.Level + 1;
                getReverseEbomStructure(Context, item, _rootOID);
            });
        }

        public static List<EBOM> getReverseEbom(HttpSessionStateBase Context, EBOM _param, int _rootOID)
        {
            List<EBOM> EBom = DaoFactory.GetList<EBOM>("EBom.SelEBom", _param);

            foreach (EBOM Obj in EBom)
            {
                EPart PataData = SelEPartObject(Context, new EPart { OID = Obj.FromOID });
                Obj.FromData = PataData;
                Obj.Description = PataData.Description;
                Obj.ObjName = PataData.Name;
                Obj.ObjRevision = PataData.Revision;
                Obj.ObjTdmxOID = PataData.TdmxOID;
                Obj.ObjIsLatest = PataData.IsLatest;
                Obj.ObjTitle = PataData.Title;
                Obj.ObjRep_Part_No = PataData.Rep_Part_No;
                Obj.ObjRep_Part_No2 = PataData.Rep_Part_No2;
                Obj.ObjEo_No = PataData.Eo_No;
                Obj.ObjEtc = PataData.Etc;
                Obj.ObjApprovOID = PataData.ApprovOID;
                Obj.ObjEPartType = PataData.EPartType;
                Obj.ObjEPartTypeNm = PataData.EPartTypeNm;
                Obj.ObjDivision = PataData.Division;
                Obj.ObjDivisionNm = PataData.DivisionNm;
                Obj.ObjITEM_No = PataData.ITEM_No;
                Obj.ObjITEM_NoNm = PataData.ITEM_NoNm;
                Obj.ObjITEM_Middle = PataData.ITEM_Middle;
                Obj.ObjITEM_MiddleNm = PataData.ITEM_MiddleNm;
                Obj.ObjProduction_Place = PataData.Production_Place;
                Obj.ObjProduction_PlaceNm = PataData.Production_PlaceNm;
                Obj.ObjMaterial_OID = PataData.Material_OID;
                Obj.ObjMaterial_Nm = PataData.Material_Nm;
                Obj.ObjBlock_No = PataData.Block_No;
                Obj.ObjBlock_NoNm = PataData.Block_NoNm;
                Obj.ObjSel_Eo = PataData.Sel_Eo;
                Obj.ObjSel_Eo_Dt = PataData.Sel_Eo_Dt;
                Obj.ObjSpec = PataData.Spec;
                Obj.ObjSurface = PataData.Surface;
                Obj.ObjOem_Lib_OID = PataData.Oem_Lib_OID;
                Obj.ObjCar_Lib_OID = PataData.Car_Lib_OID;
                Obj.ObjPms_OID = PataData.Pms_OID;
                Obj.ObjCar_Lib_Nm = PataData.Car_Lib_Nm;

                Obj.ObjSel_Revision = PataData.Sel_Revision;
                Obj.ObjStandard = PataData.Standard;
                Obj.ObjOem_Lib_Nm = PataData.Oem_Lib_Nm;

                Obj.BPolicy = PataData.BPolicy;
                Obj.BPolicyNm = PataData.BPolicy.StatusNm;
            }
            return EBom;
        }
        #endregion

        #region 루트 기준 하위 전체 검색
        public static List<EPart> SelRootChildList(EPart _param)
        {
            List<EPart> lSelRootChildList = DaoFactory.GetList<EPart>("EBom.SelRootChildList", new EPart { Type = EBomConstant.TYPE_PART, FromOID = _param.FromOID });

            lSelRootChildList.ForEach(obj =>
            {
                obj.BPolicy = BPolicyRepository.SelBPolicy(new BPolicy { Type = obj.Type, OID = obj.BPolicyOID }).First();
                if (obj.Car_Lib_OID != null)
                {
                    obj.Car_Lib_Nm = LibraryRepository.SelCodeLibraryObject(new Library { OID = obj.Car_Lib_OID }).KorNm;
                }
                if (obj.Block_No != null)
                {
                    if (obj.Division == Common.Constant.EBomConstant.DIV_ASSEMBLY)
                    {
                        obj.Block_NoNm = LibraryRepository.SelCodeLibraryObject(new Library { OID = obj.Block_No }).KorNm;
                    }
                    else
                    {
                        obj.Block_NoNm = LibraryRepository.SelLibraryObject(new Library { OID = obj.Block_No }).KorNm;
                    }
                }
                if (obj.Material_OID != null)
                {
                    obj.Material_Nm = LibraryRepository.SelLibraryObject(new Library { OID = obj.Material_OID }).KorNm;
                }
                if (obj.ITEM_No != null)
                {
                    obj.ITEM_NoNm = LibraryRepository.SelCodeLibraryObject(new Library { OID = obj.ITEM_No }).KorNm;
                }
            });

            List<EPart> RootChildList = lSelRootChildList.GroupBy(x => x.OID).Select(y => y.First()).ToList();

            return RootChildList;
        }
        #endregion

        #region EBOM 편집 추가 버튼 클릭시 나타나는 페이지
        public static List<EBOM> getListEbomAddChild(HttpSessionStateBase Context, int _level, string _Name, EPart _Param)
        {
            List<EPart> EPartList = EPartRepository.SelNot1LvParentEPart(Context, _Param);

            if(_Param.NewCheck != null)
            {
                EPartList = EPartList.FindAll(value => (value.BPolicy.Name == EBomConstant.POLICY_EPART_PREPARE) || (value.BPolicy.Name == EBomConstant.POLICY_EPART_REJECT) || (value.BPolicy.Name == EBomConstant.POLICY_EPART_COMPLETED && value.IsReleasedLatest == 1));
            }

            List<EBOM> getStructureList = new List<EBOM>();

            int? TimeOID = Convert.ToInt32(DateTimeOffset.Now.ToUnixTimeSeconds());
            //int? TimeOID = 0;

            foreach (var obj in EPartList)
            {
                TimeOID = TimeOID + 1;
                EBOM getStructure = new EBOM();
                getStructure.Level = _level;
                getStructure.OID = TimeOID;
                getStructure.ToOID = obj.OID;
                getStructure.ToData = obj;
                getStructure.Description = obj.Description;
                getStructure.ObjName = obj.Name;
                getStructure.ObjRevision = obj.Revision;
                getStructure.ObjTdmxOID = obj.TdmxOID;
                getStructure.ObjIsLatest = obj.IsLatest;
                getStructure.ObjEPartType = obj.EPartType;
                getStructure.ObjEPartTypeNm = obj.EPartTypeNm;
                getStructure.ObjThumbnail = obj.Thumbnail;
                getStructure.ObjCar_Lib_OID = obj.Car_Lib_OID;
                getStructure.ObjCar_Lib_Nm = obj.Car_Lib_Nm;
                getStructure.ObjDivision = obj.Division;
                getStructure.ObjDivisionNm = obj.DivisionNm;
                getStructure.ObjITEM_No = obj.ITEM_No;
                getStructure.ObjITEM_NoNm = obj.ITEM_NoNm;
                getStructure.ObjITEM_Middle = obj.ITEM_Middle;
                getStructure.ObjITEM_MiddleNm = obj.ITEM_MiddleNm;
                getStructure.ObjProduction_Place = obj.Production_Place;
                getStructure.ObjProduction_PlaceNm = obj.Production_PlaceNm;
                getStructure.ObjMaterial_OID = obj.Material_OID;
                getStructure.ObjMaterial_Nm = obj.Material_Nm;
                getStructure.ObjBlock_No = obj.Block_No;
                getStructure.ObjBlock_NoNm = obj.Block_NoNm;
                getStructure.ObjSerial = obj.Serial;
                getStructure.ObjSel_Revision = obj.Sel_Revision;

                getStructure.ObjTitle = obj.Title;
                getStructure.ObjSel_Revision = obj.Sel_Revision;
                getStructure.ObjStandard = obj.Standard;
                getStructure.ObjOem_Lib_Nm = obj.Oem_Lib_Nm;

                getStructure.BPolicy = obj.BPolicy;
                getStructure.BPolicyNm = obj.BPolicy.StatusNm;


                getEbomStructure(Context, getStructure, Convert.ToInt32(obj.OID), TimeOID);

                getStructureList.Add(getStructure);
            }
            return getStructureList;
        }
        #endregion

        #region EPart 수정
        public static EPart UdtEPartObject(EPart _param)
        {
            _param.Type = EBomConstant.TYPE_PART;
            DaoFactory.SetUpdate("EBom.UpdateEPart", _param);

            return _param;
        }
        #endregion

        #region 루트 기준 하위 BOM 전체 검색
        public static List<EBOM> SelRootChildEBomList(EBOM _param)
        {
            List<EBOM> lSelRootChildList = DaoFactory.GetList<EBOM>("EBom.SelRootChildEBomList", new EBOM { Type = EBomConstant.TYPE_PART, FromOID = _param.FromOID });
            //List<EBOM> RootChildList = lSelRootChildList.GroupBy(x => x.OID).Select(y => y.First()).ToList();

            return lSelRootChildList;
        }
        #endregion

        #region 자식 기준 최상위 품번 검색
        public static List<EPart> SelChildTopParentList(HttpSessionStateBase Context, EBOM _param)
        {
            List<EBOM> lSelRootChildList = DaoFactory.GetList<EBOM>("EBom.SelEBom", new EBOM { Type = EBomConstant.TYPE_PART, ToOID = _param.ToOID });
            List<EPart> SelChildTopParentList = new List<EPart>();

            lSelRootChildList.ForEach(item =>
            {
                getParentStructure(Context, item, SelChildTopParentList, _param.ToOID);
            });

            SelChildTopParentList.ForEach(v =>
            {
                var Tdmx = SelEPart(Context, new EPart { TdmxOID = v.TdmxOID });

                if (Tdmx[0].OID > v.OID)
                {
                    v.isDuplicate = 1;
                }

                if (v.BPolicyNm == CommonConstant.POLICY_APPROVAL_PREPARE || v.BPolicyNm == CommonConstant.POLICY_APPROVAL_REJECTED)
                {
                    v.isDuplicate = 1;
                }

            });

            return SelChildTopParentList;
        }

        public static void getParentStructure(HttpSessionStateBase Context, EBOM _relData, List<EPart> SelChildTopParentList, int? RootOID)
        {
            EPart ParentData = getParentEbom(Context, new EBOM { Type = EBomConstant.TYPE_PART, ToOID = _relData.ToOID, FromOID = _relData.FromOID }, SelChildTopParentList, RootOID);

            if (ParentData != null)
            {
                SelChildTopParentList.Add(ParentData);
            }
            else
            {
                List<EBOM> ParentList = DaoFactory.GetList<EBOM>("EBom.SelEBom", new EBOM { Type = EBomConstant.TYPE_PART, ToOID = _relData.FromOID });

                ParentList.ForEach(item =>
                {
                    getParentStructure(Context, item, SelChildTopParentList, RootOID);
                });
            }
        }

        public static EPart getParentEbom(HttpSessionStateBase Context, EBOM _param, List<EPart> SelChildTopParentList, int? RootOID)
        {
            List<EBOM> EBOM = DaoFactory.GetList<EBOM>("EBom.SelEBom", new EBOM { Type = EBomConstant.TYPE_PART, ToOID = _param.FromOID });

            if (EBOM.Count > 0)
            {
                return null;
            }
            else
            {
                return SelEPartObject(Context, new EPart { OID = _param.FromOID });
            }
        }

        public static List<EBOM> ReverseStructure(HttpSessionStateBase Context, List<EPart> param, int? RootOID)
        {
            List<EBOM> Structure = new List<EBOM>();
            List<EBOM> getStructure = new List<EBOM>();
            EPart PataData = SelEPartObject(Context, new EPart { OID = RootOID });

            List<EBOM> ParentList = DaoFactory.GetList<EBOM>("EBom.SelEBom", new EBOM { Type = EBomConstant.TYPE_PART, ToOID = RootOID });
            ParentList.ForEach(item =>
            {
                if (getStructure != null)
                {
                    getStructure = new List<EBOM>();
                }

                getStructure.Add(item);

                getReverseStructure(Context, param, RootOID, getStructure, Structure, item);

            });

            return Structure;
        }

        public static void getReverseStructure(HttpSessionStateBase Context, List<EPart> param, int? RootOID, List<EBOM> getStructure, List<EBOM> Structure, EBOM item)
        {
            List<EBOM> List = DaoFactory.GetList<EBOM>("EBom.SelEBom", new EBOM { Type = EBomConstant.TYPE_PART, ToOID = item.FromOID });

            if (List.Count == 0)
            {
                for (var i = 0; i < param.Count; i++)
                {
                    if (item.FromOID == param[i].OID)
                    {
                        getStructure.ForEach(val =>
                        {
                            val.Action = "U";
                            if (val.FromOID == item.FromOID)
                            {
                                val.Action = "ENDU";
                            }
                            Structure.Add(val);
                        });
                        getStructure.Clear();
                        break;
                    }
                }
                //getStructure.RemoveAll(v => v.OID == item.OID);
                //getStructure = new List<EBOM>();
            }
            else
            {
                List.ForEach(val =>
                {
                    getStructure.Add(val);
                    getReverseStructure(Context, param, RootOID, getStructure, Structure, val);
                    getStructure.RemoveAll(v => v.OID == val.OID);
                });
            }

        }
        #endregion

        #region
        //자식기준 부모까지 전체 연결구조 찾기
        public static List<EBOM> SelAllParentStructure(HttpSessionStateBase Context, EBOM _param)
        {
            List<EBOM> lSelAllParentStructure = DaoFactory.GetList<EBOM>("EBom.SelAllParentStructure", _param);

            return lSelAllParentStructure;
        }
        #endregion

        #region Temp EPart 오브젝트 검색
        public static EPart SelTempEPartObject(HttpSessionStateBase Context, EPart _param)
        {
            return DaoFactory.GetData<EPart>("EBom.SelTempEPartObject", new EPart { Type = EBomConstant.TYPE_PART, Orgin_OID = _param.OID });
        }
        #endregion
    }
}
