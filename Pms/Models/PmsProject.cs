using Common;
using Common.Constant;
using Common.Factory;
using Common.Interface;
using Common.Models;
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
                dObj.BPolicyAuths = BPolicyAuthRepository.MainAuth(Context, dObj);
                dObj.CreateUsNm = PersonRepository.SelPerson(Context, new Person { OID = dObj.CreateUs }).Name;
                if(dObj.BPolicyAuths.FindAll(item => item.AuthNm == CommonConstant.AUTH_VIEW).Count > 0)
                {
                    lPmsProjectes.Add(dObj);
                }
            });
            return lPmsProjectes;
        }
    }
}
