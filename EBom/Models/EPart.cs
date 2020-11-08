using Common.Interface;
using Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Constant;
using Common.Factory;
using System.Security.Cryptography;

namespace EBom.Models
{
    public class EPart : DObject, IDObject
    {
        public string Title { get; set; }
        public string Rep_Part_No { get; set; }
        public string Rep_Part_No2 { get; set; }
        public string Eo_No { get; set; }
        public DateTime? Eo_No_ApplyDt { get; set; }
        public string Eo_No_History { get; set; }
        public string Etc { get; set; }
        public int? ApprovOID { get; set; }
        public string EPartType { get; set; }
        public string Sel_Eo { get; set; }
        public DateTime? Sel_Eo_Dt { get; set; }
        public string Spec { get; set; }
        public string Surface { get; set; }
        public int? Oem_Lib_OID { get; set; }
        public int? Car_Lib_OID { get; set; }
        public int? Pms_OID { get; set; }
        public int? Prod_Lib_Lev1_OID { get; set; }
        public int? Prod_Lib_Lev2_OID { get; set; }
        public int? Prod_Lib_Lev3_OID { get; set; }
        public int? Material_OID { get; set; }
        public string CO { get; set; }
        public string Etc_Delivery { get; set; }


        public string Oem_Lib_NM { get; set; }
        public string Car_Lib_NM { get; set; }
        public string Pms_NM { get; set; }
        public string Prod_Lib_Lev1_NM { get; set; }
        public string Prod_Lib_Lev2_NM { get; set; }
        public string Prod_Lib_Lev3_NM { get; set; }
        public string Material_NM { get; set; }


        public int? RootOID { get; set; }
        public int? FromOID { get; set; }
        public int? ToOID { get; set; }

        public int? isDuplicate { get; set; }
    }

    public static class EPartRepository
    {
        #region EPart 리스트 검색
        public static List<EPart> SelEPart(EPart _param)
        {
            _param.Type = EBomConstant.TYPE_PART;
            List<EPart> lEPart = DaoFactory.GetList<EPart>("EBom.SelEPart", _param);
            lEPart.ForEach(obj =>
            {
                obj.BPolicy = BPolicyRepository.SelBPolicy(new BPolicy { Type = obj.Type, OID = obj.BPolicyOID }).First();
                if (obj.Oem_Lib_OID != null)
                {
                    obj.Oem_Lib_NM = LibraryRepository.SelLibraryObject(new Library { OID = obj.Oem_Lib_OID }).KorNm;
                }
                if (obj.Car_Lib_OID != null)
                {
                    obj.Car_Lib_NM = LibraryRepository.SelLibraryObject(new Library { OID = obj.Car_Lib_OID }).KorNm;
                }
            });
            return lEPart;
        }
        #endregion

        #region EPart 오브젝트 검색
        public static EPart SelEPartObject(EPart _param)
        {
            EPart lEPart = DaoFactory.GetData<EPart>("EBom.SelEPart", new EPart { Type = EBomConstant.TYPE_PART, OID = _param.OID });
            lEPart.BPolicy = BPolicyRepository.SelBPolicy(new BPolicy { Type = lEPart.Type, OID = lEPart.BPolicyOID }).First();

            if (lEPart.Oem_Lib_OID != null)
            {
                lEPart.Oem_Lib_NM = LibraryRepository.SelLibraryObject(new Library { OID = lEPart.Oem_Lib_OID }).KorNm;
            }
            if (lEPart.Car_Lib_OID != null)
            {
                lEPart.Car_Lib_NM = LibraryRepository.SelLibraryObject(new Library { OID = lEPart.Car_Lib_OID }).KorNm;
            }

            if (lEPart.Material_OID != null)
            {
                lEPart.Material_NM = LibraryRepository.SelLibraryObject(new Library { OID = lEPart.Material_OID }).KorNm;
            }

            if (lEPart.Prod_Lib_Lev1_OID != null)
            {
                lEPart.Prod_Lib_Lev1_NM = LibraryRepository.SelLibraryObject(new Library { OID = lEPart.Prod_Lib_Lev1_OID }).KorNm;
            }
            if (lEPart.Prod_Lib_Lev2_OID != null)
            {
                lEPart.Prod_Lib_Lev2_NM = LibraryRepository.SelLibraryObject(new Library { OID = lEPart.Prod_Lib_Lev2_OID }).KorNm;
            }
            if (lEPart.Prod_Lib_Lev3_OID != null)
            {
                lEPart.Prod_Lib_Lev3_NM = LibraryRepository.SelLibraryObject(new Library { OID = lEPart.Prod_Lib_Lev3_OID }).KorNm;
            }


            return lEPart;
        }
        #endregion

        #region EPart 품번 중복체크
        public static EPart ChkEPart(EPart _param)
        {
            EPart lPartObj = DaoFactory.GetData<EPart>("EBom.ChkEPart", new EPart { Type = EBomConstant.TYPE_PART, Name = _param.Name });
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
        public static EBOM getListEbom(int _level, int _rootOID)
        {
            EBOM getStructure = new EBOM();
            getStructure.Level = _level;
            getStructure.ToOID = _rootOID;

            EPart PataData = SelEPartObject(new EPart { OID = _rootOID });
            getStructure.ToData = PataData;
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
            getStructure.ObjThumbnail = PataData.Thumbnail;
            getStructure.ObjSel_Eo = PataData.Sel_Eo;
            getStructure.ObjSel_Eo_Dt = PataData.Sel_Eo_Dt;
            getStructure.ObjSpec = PataData.Spec;
            getStructure.ObjSurface = PataData.Surface;
            getStructure.ObjOem_Lib_OID = PataData.Oem_Lib_OID;
            getStructure.ObjCar_Lib_OID = PataData.Car_Lib_OID;
            getStructure.ObjPms_OID = PataData.Pms_OID;
            getStructure.ObjProd_Lib_Lev1_OID = PataData.Prod_Lib_Lev1_OID;
            getStructure.ObjProd_Lib_Lev2_OID = PataData.Prod_Lib_Lev2_OID;
            getStructure.ObjProd_Lib_Lev3_OID = PataData.Prod_Lib_Lev3_OID;
            getStructure.ObjCar_Lib_NM = PataData.Car_Lib_NM;

            getStructure.BPolicy = PataData.BPolicy;

            getStructure.diseditable = EBomConstant.DISEDITABLE;

            getEbomStructure(getStructure, _rootOID);
            return getStructure;
        }

        public static void getEbomStructure(EBOM _relData, int _rootOID) 
        {
            _relData.Children = getEbom(new EBOM { Type = EBomConstant.TYPE_PART, FromOID = _relData.ToOID }, _rootOID);
            _relData.Children.ForEach(item =>
            {
                item.Level = _relData.Level + 1;
                getEbomStructure(item, _rootOID);
            });
        }

        public static List<EBOM> getEbom(EBOM _param, int _rootOID)
        {
            List<EBOM> EBom = DaoFactory.GetList<EBOM>("EBom.SelEBom", _param);

            foreach (EBOM Obj in EBom)
            {
                EPart PataData = SelEPartObject(new EPart { OID = Obj.ToOID });
                Obj.ToData = PataData;
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
                Obj.ObjThumbnail = PataData.Thumbnail;
                Obj.ObjSel_Eo = PataData.Sel_Eo;
                Obj.ObjSel_Eo_Dt = PataData.Sel_Eo_Dt;
                Obj.ObjSpec = PataData.Spec;
                Obj.ObjSurface = PataData.Surface;
                Obj.ObjOem_Lib_OID = PataData.Oem_Lib_OID;
                Obj.ObjCar_Lib_OID = PataData.Car_Lib_OID;
                Obj.ObjPms_OID = PataData.Pms_OID;
                Obj.ObjProd_Lib_Lev1_OID = PataData.Prod_Lib_Lev1_OID;
                Obj.ObjProd_Lib_Lev2_OID = PataData.Prod_Lib_Lev2_OID;
                Obj.ObjProd_Lib_Lev3_OID = PataData.Prod_Lib_Lev3_OID;
                Obj.ObjCar_Lib_NM = PataData.Car_Lib_NM;

                Obj.BPolicy = PataData.BPolicy;

                Obj.diseditable = EBomConstant.FLOWEDITABLE;
            }
            return EBom;
        }
        #endregion

        #region EPart 역전개
        public static EBOM getListReverseEbom(int _level, int _rootOID)
        {
            EBOM getStructure = new EBOM();
            getStructure.Level = _level;
            getStructure.FromOID = _rootOID;

            EPart PataData = SelEPartObject(new EPart { OID = _rootOID });
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
            //getStructure.ObjThumbnail = PataData.Thumbnail;
            getStructure.ObjSel_Eo = PataData.Sel_Eo;
            getStructure.ObjSel_Eo_Dt = PataData.Sel_Eo_Dt;
            getStructure.ObjSpec = PataData.Spec;
            getStructure.ObjSurface = PataData.Surface;
            getStructure.ObjOem_Lib_OID = PataData.Oem_Lib_OID;
            getStructure.ObjCar_Lib_OID = PataData.Car_Lib_OID;
            getStructure.ObjPms_OID = PataData.Pms_OID;
            getStructure.ObjProd_Lib_Lev1_OID = PataData.Prod_Lib_Lev1_OID;
            getStructure.ObjProd_Lib_Lev2_OID = PataData.Prod_Lib_Lev2_OID;
            getStructure.ObjProd_Lib_Lev3_OID = PataData.Prod_Lib_Lev3_OID;

            getStructure.ObjCar_Lib_NM = PataData.Car_Lib_NM;

            getStructure.BPolicy = PataData.BPolicy;

            getReverseEbomStructure(getStructure, _rootOID);
            return getStructure;
        }

        public static void getReverseEbomStructure(EBOM _relData, int _rootOID)
        {
            _relData.Parents = getReverseEbom(new EBOM { Type = EBomConstant.TYPE_PART, ToOID = _relData.FromOID }, _rootOID);
            _relData.Parents.ForEach(item =>
            {
                item.Level = _relData.Level + 1;
                getReverseEbomStructure(item, _rootOID);
            });
        }

        public static List<EBOM> getReverseEbom(EBOM _param, int _rootOID)
        {
            List<EBOM> EBom = DaoFactory.GetList<EBOM>("EBom.SelEBom", _param);

            foreach (EBOM Obj in EBom)
            {
                EPart PataData = SelEPartObject(new EPart { OID = Obj.FromOID });
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
                //Obj.ObjThumbnail = PataData.Thumbnail;
                Obj.ObjSel_Eo = PataData.Sel_Eo;
                Obj.ObjSel_Eo_Dt = PataData.Sel_Eo_Dt;
                Obj.ObjSpec = PataData.Spec;
                Obj.ObjSurface = PataData.Surface;
                Obj.ObjOem_Lib_OID = PataData.Oem_Lib_OID;
                Obj.ObjCar_Lib_OID = PataData.Car_Lib_OID;
                Obj.ObjPms_OID = PataData.Pms_OID;
                Obj.ObjProd_Lib_Lev1_OID = PataData.Prod_Lib_Lev1_OID;
                Obj.ObjProd_Lib_Lev2_OID = PataData.Prod_Lib_Lev2_OID;
                Obj.ObjProd_Lib_Lev3_OID = PataData.Prod_Lib_Lev3_OID;

                Obj.ObjCar_Lib_NM = PataData.Car_Lib_NM;

                Obj.BPolicy = PataData.BPolicy;
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
                if (obj.Oem_Lib_OID != null)
                {
                    obj.Oem_Lib_NM = LibraryRepository.SelLibraryObject(new Library { OID = obj.Oem_Lib_OID }).KorNm;
                }
                if (obj.Car_Lib_OID != null)
                {
                    obj.Car_Lib_NM = LibraryRepository.SelLibraryObject(new Library { OID = obj.Car_Lib_OID }).KorNm;
                }
            });

            //var ll = (from l in lSelRootChildList select l).GroupBy(x => x.OID).Select(y => y.First());

            List<EPart> RootChildList = lSelRootChildList.GroupBy(x => x.OID).Select(y => y.First()).ToList();

            //var LinqRootChildList = (from List in lSelRootChildList
            //                         select List).GroupBy(x => x.OID).Select(y => y.First());
            //LinqRootChildList = LinqRootChildList.ToList<EPart>();

            return RootChildList;
        }
        #endregion

        #region EBOM 편집 추가 버튼 클릭시 나타나는 페이지
        public static List<EBOM> getListEbomAddChild(int _level, string _Name, EPart _Param)
        {
            List<EPart> EPartList = EPartRepository.SelEPart(_Param);

            List<EBOM> getStructureList = new List<EBOM>();

            int? TimeOID = Convert.ToInt32(DateTimeOffset.Now.ToUnixTimeSeconds());

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
                getStructure.ObjTitle = obj.Title;
                getStructure.ObjRep_Part_No = obj.Rep_Part_No;
                getStructure.ObjRep_Part_No2 = obj.Rep_Part_No2;
                getStructure.ObjEo_No = obj.Eo_No;
                getStructure.ObjEtc = obj.Etc;
                getStructure.ObjApprovOID = obj.ApprovOID;
                getStructure.ObjEPartType = obj.EPartType;
                getStructure.ObjThumbnail = obj.Thumbnail;
                getStructure.ObjSel_Eo = obj.Sel_Eo;
                getStructure.ObjSel_Eo_Dt = obj.Sel_Eo_Dt;
                getStructure.ObjSpec = obj.Spec;
                getStructure.ObjSurface = obj.Surface;
                getStructure.ObjOem_Lib_OID = obj.Oem_Lib_OID;
                getStructure.ObjCar_Lib_OID = obj.Car_Lib_OID;
                getStructure.ObjPms_OID = obj.Pms_OID;
                getStructure.ObjProd_Lib_Lev1_OID = obj.Prod_Lib_Lev1_OID;
                getStructure.ObjProd_Lib_Lev2_OID = obj.Prod_Lib_Lev2_OID;
                getStructure.ObjProd_Lib_Lev3_OID = obj.Prod_Lib_Lev3_OID;

                getStructure.ObjCar_Lib_NM = obj.Car_Lib_NM;

                getStructure.BPolicy = obj.BPolicy;


                getEbomStructure(getStructure, Convert.ToInt32(obj.OID));

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
    }
}
