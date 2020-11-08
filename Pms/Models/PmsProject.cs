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

    }

    public static class PmsProjectRepository
    {

        public static int InsPmsProject(PmsProject _param)
        {
            return DaoFactory.SetInsert("Pms.InsPmsProject", _param);
        }

        public static int UdtPmsProject(PmsProject _param)
        {
            return DaoFactory.SetUpdate("Pms.UdtPmsProject", _param);
        }

        public static PmsProject SelPmsObject(PmsProject _param)
        {
            if (_param.Type == null)
            {
                _param.Type = PmsConstant.TYPE_PROJECT;
            }
            PmsProject pmsProject = DaoFactory.GetData<PmsProject>("Pms.SelPmsProject", _param);
            pmsProject.BPolicy = BPolicyRepository.SelBPolicy(new BPolicy { Type = pmsProject.Type, OID = pmsProject.BPolicyOID }).First();
            pmsProject.Calendar = CalendarRepository.SelCalendar(new Calendar { Type = CommonConstant.TYPE_CALENDAR, OID = pmsProject.CalendarOID });
            PmsRelationship Member = PmsRelationshipRepository.SelPmsRelationship(new PmsRelationship { Type = PmsConstant.RELATIONSHIP_MEMBER, RootOID = pmsProject.OID, RoleOID = BDefineRepository.SelDefine(new BDefine { Module = PmsConstant.MODULE_PMS, Type = CommonConstant.DEFINE_ROLE, Name = PmsConstant.ROLE_PM }).OID }).First();
            pmsProject.PMNm = PersonRepository.SelPerson(new Person { OID = Member.ToOID }).Name;
            return pmsProject;
        }

        public static List<PmsProject> SelPmsObjects(PmsProject _param)
        {
            if (_param.Type == null)
            {
                _param.Type = PmsConstant.TYPE_PROJECT;
            }
            List<PmsProject> lPmsProject = DaoFactory.GetList<PmsProject>("Pms.SelPmsProject", _param);
            lPmsProject.ForEach(dObj =>
            {
                dObj.BPolicy = BPolicyRepository.SelBPolicy(new BPolicy { Type = dObj.Type, OID = dObj.BPolicyOID }).First();
            });
            return lPmsProject;
        }
    }
}
