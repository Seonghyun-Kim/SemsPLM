﻿using Common;
using Common.Constant;
using Common.Factory;
using Common.Interface;
using Common.Models;
using Pms.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Pms.Models
{
    public class PmsProject: DObject, IDObject
    {
        public string ProjectType { get; set; }

        public string IsTemplate { get; set; }

        public DateTime? BaseDt { get; set; }

        public DateTime? EstStartDt { get; set; }

        public DateTime? EstEndDt { get; set; }

        public int? EstDuration { get; set; }

        public DateTime? ActStartDt { get; set; }

        public DateTime? ActEndDt { get; set; }

        public int? ActDuration { get; set; }

        public int? WorkingDay { get; set; }

        public int? CalendarOID { get; set; }

        public Calendar Calendar { get; set; }

        public int? Complete { get; set; }

        public string PMNm { get; set; }

        public int? Oem_Lib_OID { get;set;}
        public int? Car_Lib_OID { get;set;}
        public int? ITEM_No { get;set;}
        public int? ITEM_Middle { get;set;}
        public string ProjectGrade { get;set;}
        public int? Customer_OID { get;set;}
        public string ProductNm { get; set; }
        public string Oem_Lib_Nm { get; set; }
        public string Car_Lib_Nm { get; set; }
        public string ITEM_NoNm { get; set; } //ITEM_NO
        public string ITEM_MiddleNm { get; set; }
        public string CustomerNm { get; set; }

    }

    public static class PmsProjectRepository
    {

        public static int InsPmsProject(HttpSessionStateBase Context, PmsProject _param)
        {
            return DaoFactory.SetInsert("Pms.InsPmsProject", _param);
        }

        public static int UdtPmsProject(HttpSessionStateBase Context, PmsProject _param)
        {
            return DaoFactory.SetUpdate("Pms.UdtPmsProject", _param);
        }

        public static PmsProject SelPmsObject(HttpSessionStateBase Context, PmsProject _param)
        {
            if (_param.Type == null)
            {
                _param.Type = PmsConstant.TYPE_PROJECT;
            }
            
            PmsProject pmsProject = DaoFactory.GetData<PmsProject>("Pms.SelPmsProject", _param);
            if (pmsProject == null)
            {
                _param.Type = PmsConstant.TYPE_PROJECT_TEMP;
                _param.IsTemplate = _param.Type;
                pmsProject = DaoFactory.GetData<PmsProject>("Pms.SelPmsProject", _param);
            }

            if (pmsProject.ITEM_No != null)
            {
                pmsProject.ITEM_NoNm = LibraryRepository.SelCodeLibraryObject(new Library { OID = pmsProject.ITEM_No }).KorNm;
            }
            if (pmsProject.ITEM_Middle != null)
            {
                pmsProject.ITEM_MiddleNm = LibraryRepository.SelCodeLibraryObject(new Library { OID = pmsProject.ITEM_Middle }).KorNm;
            }
            if (pmsProject.Oem_Lib_OID != null)
            {
                pmsProject.Oem_Lib_Nm = LibraryRepository.SelCodeLibraryObject(new Library { OID = pmsProject.Oem_Lib_OID }).KorNm;
            }
            if (pmsProject.Car_Lib_OID != null)
            {
                pmsProject.Car_Lib_Nm = LibraryRepository.SelCodeLibraryObject(new Library { OID = pmsProject.Car_Lib_OID }).KorNm;
            }
            if (pmsProject.Customer_OID != null)
            {
                pmsProject.CustomerNm = LibraryRepository.SelLibraryObject(new Library { OID = pmsProject.Customer_OID }).KorNm;
            }

            pmsProject.CreateUsNm = PersonRepository.SelPerson(Context, new Person { OID = pmsProject.CreateUs }).Name;
            pmsProject.BPolicy = BPolicyRepository.SelBPolicy(new BPolicy { Type = pmsProject.Type, OID = pmsProject.BPolicyOID }).First();
            pmsProject.BPolicyNm = pmsProject.BPolicy.Name;
            pmsProject.BPolicyAuths = BPolicyAuthRepository.MainAuth(Context, pmsProject, PmsAuth.RoleAuth(Context, pmsProject));
            pmsProject.Calendar = CalendarRepository.SelCalendar(new Calendar { Type = CommonConstant.TYPE_CALENDAR, OID = pmsProject.CalendarOID });
            PmsRelationship Member = PmsRelationshipRepository.SelPmsRelationship(Context, new PmsRelationship { Type = PmsConstant.RELATIONSHIP_MEMBER, RootOID = pmsProject.OID, RoleOID = BDefineRepository.SelDefine(new BDefine { Module = PmsConstant.MODULE_PMS, Type = CommonConstant.DEFINE_ROLE, Name = PmsConstant.ROLE_PM }).OID }).First();
            pmsProject.PMNm = PersonRepository.SelPerson(Context, new Person { OID = Member.ToOID }).Name;
            return pmsProject;
        }

        public static List<PmsProject> SelPmsObjects(HttpSessionStateBase Context, PmsProject _param)
        {
            if (_param.Type == null)
            {
                _param.Type = PmsConstant.TYPE_PROJECT;
            }
            List<PmsProject> lPmsProjectes = new List<PmsProject>();
            List<PmsProject> lPmsProject = DaoFactory.GetList<PmsProject>("Pms.SelPmsProject", _param);
            lPmsProject.ForEach(dObj =>
            {
                if (dObj.ITEM_No != null)
                {
                    dObj.ITEM_NoNm = LibraryRepository.SelCodeLibraryObject(new Library { OID = dObj.ITEM_No }).KorNm;
                }
                if (dObj.ITEM_Middle != null)
                {
                    dObj.ITEM_MiddleNm = LibraryRepository.SelCodeLibraryObject(new Library { OID = dObj.ITEM_Middle }).KorNm;
                }
                if (dObj.Oem_Lib_OID != null)
                {
                    dObj.Oem_Lib_Nm = LibraryRepository.SelCodeLibraryObject(new Library { OID = dObj.Oem_Lib_OID }).KorNm;
                }
                if (dObj.Car_Lib_OID != null)
                {
                    dObj.Car_Lib_Nm = LibraryRepository.SelCodeLibraryObject(new Library { OID = dObj.Car_Lib_OID }).KorNm;
                }
                if (dObj.Customer_OID != null)
                {
                    dObj.CustomerNm = LibraryRepository.SelLibraryObject(new Library { OID = dObj.Customer_OID }).KorNm;
                }
                dObj.BPolicy = BPolicyRepository.SelBPolicy(new BPolicy { Type = dObj.Type, OID = dObj.BPolicyOID }).First();
                dObj.BPolicyAuths = BPolicyAuthRepository.MainAuth(Context, dObj, PmsAuth.RoleAuth(Context,dObj));
                dObj.CreateUsNm = PersonRepository.SelPerson(Context, new Person { OID = dObj.CreateUs }).Name;
                if(dObj.BPolicyAuths.FindAll(item => item.AuthNm == CommonConstant.AUTH_VIEW).Count > 0)
                {
                    lPmsProjectes.Add(dObj);
                }
            });
            return lPmsProjectes;
        }

        #region -- Project
        public static List<JqTreeModel> SelTotalProjMngt(HttpSessionStateBase Context, string Type)
        {
            List<Library> CodeLibrary = LibraryRepository.SelCodeLibrary(new Library { FromOID = 932 });
            List<JqTreeModel> jqTreeModelList = new List<JqTreeModel>();
            foreach (Library obj in CodeLibrary)
            {
                JqTreeModel jqTreeModel = new JqTreeModel();
                jqTreeModel.id = obj.OID;
                jqTreeModel.label = obj.KorNm;
                jqTreeModel.icon = CommonConstant.ICON_COMPANY;
                jqTreeModel.iconsize = CommonConstant.DEFAULT_ICONSIZE;
                jqTreeModel.expanded = true;
                jqTreeModel.type = PmsConstant.ATTRIBUTE_OEM;
                jqTreeModel.items = new List<JqTreeModel>();
                jqTreeModelList.Add(jqTreeModel);
            }

            for (var i = 0; i < jqTreeModelList.Count; i++)
            {
                LibraryRepository.SelCodeLibraryChild(new Library { FromOID = jqTreeModelList[i].id,Code1=CommonConstant.ATTRIBUTE_CARTYPE }).ForEach(item =>
                {
                    JqTreeModel innerJqTreeModel = new JqTreeModel();
                    innerJqTreeModel.id = item.OID;
                    innerJqTreeModel.label = item.KorNm;
                    innerJqTreeModel.icon = PmsConstant.ICON_CARTYPE;
                    innerJqTreeModel.iconsize = PmsConstant.DEFAULT_ICONSIZE;
                    innerJqTreeModel.expanded = true;
                    innerJqTreeModel.type = PmsConstant.ATTRIBUTE_CAR;
                    innerJqTreeModel.items = new List<JqTreeModel>();
                    if(Type == "Total")
                    {
                        SelCarLibrary(Context, innerJqTreeModel, item);
                    }
                    jqTreeModelList[i].items.Add(innerJqTreeModel);
                });
            }

            return jqTreeModelList;
        }
        public static void SelCarLibrary(HttpSessionStateBase Context, JqTreeModel _jqxTree, DObject _param)
        {
            PmsProjectRepository.SelPmsObjects(Context, new PmsProject { Car_Lib_OID = _param.OID }).ForEach(item =>
            {
                JqTreeModel innerJqTreeModel = new JqTreeModel();
                innerJqTreeModel.id = item.OID;
                innerJqTreeModel.label = item.Name;
                innerJqTreeModel.icon = CommonConstant.ICON_DEPARTMENT;
                innerJqTreeModel.iconsize = CommonConstant.DEFAULT_ICONSIZE;
                innerJqTreeModel.expanded = true;
                innerJqTreeModel.type = Common.Constant.PmsConstant.TYPE_PROJECT;
                _jqxTree.items.Add(innerJqTreeModel);
            });
        }
        #endregion
    }
}
