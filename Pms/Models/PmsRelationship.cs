using Common.Constant;
using Common.Factory;
using Common.Models;
using Pms.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pms.Models
{
    public class PmsRelationship : DRelationship, IPmsRelationship
    {
        public int? RootOID { get; set; }

        public int? Delay { get; set; }

        public List<PmsRelationship> Children { get; set; }

        public List<string> diseditable { get; set; }

        public string expanded { get => "true"; }

        public string Action { get; set; }

        public int? RoleOID { get; set; }

        public string Description { get; set; }

        public int? BaseLineOrd { get; set; }

        //Temp Data
        public string ObjName { get; set; }

        public string ObjType { get; set; }

        public int? ObjSt { get; set; }

        public string ObjStNm { get; set; }

        public int? Id { get; set; }

        public string Dependency { get; set; }

        public int? EstDuration { get; set; }

        public DateTime? EstStartDt { get; set; }

        public DateTime? EstEndDt { get; set; }

        public int? ActDuration { get; set; }

        public DateTime? ActStartDt { get; set; }

        public DateTime? ActEndDt { get; set; }

        public List<PmsRelationship> Members { get; set; }

        public int? Complete { get; set; }

        public int? WorkingDay { get; set; }

        public string RoleOIDNm { get; set; }

        public string PersonNm { get; set; }

        public string DepartmentNm { get; set; }

        public string Thumbnail { get; set; }

    }

    public static class PmsRelationshipRepository
    {

        public static int InsPmsRelationship(PmsRelationship _param)
        {
            List<PmsRelationship> duplication = PmsRelationshipRepository.SelPmsRelationship(_param);
            if (duplication != null && duplication.Count > 0)
            {
                return -1;
            }
            _param.CreateUs = 1;

            string query = "";
            if (_param.Ord != null)
            {
                query = "Pms.InsPmsRelationship";
            }
            else
            {
                query = "Pms.InsPmsRelationshipNotOrd";
            }
            return DaoFactory.SetInsert(query, _param);
        }

        public static int UdtPmsRelationship(PmsRelationship _param)
        {
            return DaoFactory.SetUpdate("Pms.UdtPmsRelationship", _param);
        }

        public static List<PmsRelationship> SelPmsRelationship(PmsRelationship _param)
        {
            return DaoFactory.GetList<PmsRelationship>("Pms.SelPmsRelationship", _param);
        }

        public static int DelPmsRelaionship(PmsRelationship _param)
        {
            _param.DeleteUs = 1;
            if (_param.OID == null)
            {
                return DaoFactory.SetUpdate("Pms.DelPmsRelationshipByData", _param);
            }
            return DaoFactory.SetUpdate("Pms.DelPmsRelationship", _param);
        }
        
        public static PmsRelationship GetObjWbsStructure(int _level, int _fromOID, PmsProject _proj)
        {
            List<DateTime> lHoliday = CalendarDetailRepository.SelCalendarDetails(new CalendarDetail { CalendarOID = _proj.OID, IsHoliday = 1 }).Select(val => DateTime.Parse(val.Year + "-" + val.Month + "-" + val.Day)).ToList();
            string strHoliday = string.Join(",", CalendarDetailRepository.SelCalendarDetails(new CalendarDetail { CalendarOID = _proj.OID, IsHoliday = 1 }).Select(value => value.FullDate.ToString().ToArray()));

            PmsRelationship getStructure = new PmsRelationship();
            getStructure.Level = _level;
            getStructure.ToOID = _fromOID;
            getStructure.ToData = _proj;
            getStructure.ObjName = _proj.Name;
            getStructure.Description = _proj.Description;
            getStructure.ObjType = _proj.Type;
            getStructure.EstDuration = _proj.EstDuration;
            getStructure.EstStartDt = _proj.EstStartDt != null ? Convert.ToDateTime(string.Format("{0:yyyy-MM-dd}", _proj.EstStartDt)) : _proj.EstStartDt;
            getStructure.EstEndDt = _proj.EstEndDt != null ? Convert.ToDateTime(string.Format("{0:yyyy-MM-dd}", _proj.EstEndDt)) : _proj.EstEndDt;
            getStructure.ActDuration = _proj.ActDuration;
            getStructure.ActStartDt = _proj.ActStartDt != null ? Convert.ToDateTime(string.Format("{0:yyyy-MM-dd}", _proj.ActStartDt)) : _proj.ActStartDt;
            getStructure.ActEndDt = _proj.ActEndDt != null ? Convert.ToDateTime(string.Format("{0:yyyy-MM-dd}", _proj.ActEndDt)) : _proj.ActEndDt;
            getStructure.ObjSt = _proj.BPolicyOID;
            getStructure.ObjStNm = _proj.BPolicy.StatusNm;
            getStructure.Complete = _proj.Complete;
            getStructure.WorkingDay = _proj.WorkingDay;
            getStructure.Id = null;
            getStructure.Dependency = null;
            if (_proj.BPolicy.Name == PmsConstant.POLICY_PROJECT_PREPARE || _proj.BPolicy.Name == PmsConstant.POLICY_PROJECT_PAUSED)
            {
                getStructure.diseditable = PmsConstant.DISEDITABLE;
            }
            else
            {
                getStructure.diseditable = PmsConstant.FLOWEDITABLE;
                if (_proj.ActEndDt != null)
                {
                    getStructure.Delay = PmsUtils.CalculateDelay(Convert.ToDateTime(_proj.EstEndDt), Convert.ToDateTime(_proj.ActEndDt), Convert.ToInt32(_proj.WorkingDay), lHoliday);
                }
                else
                {
                    getStructure.Delay = PmsUtils.CalculateDelay(Convert.ToDateTime(_proj.EstEndDt), DateTime.Now, Convert.ToInt32(_proj.WorkingDay), lHoliday);
                }
            }
            GetWbsStructure(getStructure, _fromOID, Convert.ToInt32(_proj.WorkingDay), strHoliday, _proj, lHoliday);
            return getStructure;
        }

        public static void GetWbsStructure(PmsRelationship _relObj, int _projOID, int _workingDay, string _holiday, PmsProject _proj, List<DateTime> _lHoliday)
        {
            _relObj.RootOID = _projOID;
            _relObj.Children = PmsRelationshipRepository.SelPmsRelationship(new PmsRelationship { Type = PmsConstant.RELATIONSHIP_WBS, FromOID = _relObj.ToOID });
            _relObj.Children.ForEach(item =>
            {
                item.Level = _relObj.Level + 1;
                //item.FromData = PmsProcessRepository.SelPmsProcess(new PmsProcess { OID = item.FromOID });
                //item.ToData = PmsProcessRepository.SelPmsProcess(new PmsProcess { OID = item.ToOID });
                PmsProcess tmpFromData = PmsProcessRepository.SelPmsProcess(new PmsProcess { OID = item.FromOID });
                PmsProcess tmpToData = PmsProcessRepository.SelPmsProcess(new PmsProcess { OID = item.ToOID });
                item.ObjName = tmpToData.Name;
                item.Description = tmpToData.Description;
                item.ObjType = tmpToData.Type;
                item.EstDuration = tmpToData.EstDuration;
                item.EstStartDt = tmpToData.EstStartDt != null ? Convert.ToDateTime(string.Format("{0:yyyy-MM-dd}", tmpToData.EstStartDt)) : tmpToData.EstStartDt;
                item.EstEndDt = tmpToData.EstEndDt != null ? Convert.ToDateTime(string.Format("{0:yyyy-MM-dd}", tmpToData.EstEndDt)) : tmpToData.EstEndDt;
                item.ActDuration = tmpToData.ActDuration;
                item.ActStartDt = tmpToData.ActStartDt != null ? Convert.ToDateTime(string.Format("{0:yyyy-MM-dd}", tmpToData.ActStartDt)) : tmpToData.ActStartDt;
                item.ActEndDt = tmpToData.ActEndDt != null ? Convert.ToDateTime(string.Format("{0:yyyy-MM-dd}", tmpToData.ActEndDt)) : tmpToData.ActEndDt;
                item.ObjSt = tmpToData.BPolicyOID;
                item.ObjStNm = tmpToData.BPolicy.StatusNm;
                item.Id = tmpToData.Id;
                item.Dependency = tmpToData.Dependency;
                item.Complete = tmpToData.Complete;
                item.WorkingDay = _workingDay;
                item.Members = new List<PmsRelationship>();
                PmsRelationshipRepository.SelPmsRelationship(new PmsRelationship { FromOID = item.ToOID, Type = PmsConstant.RELATIONSHIP_MEMBER }).ForEach(member =>
                {
                    Person person = PersonRepository.SelPerson(new Person { OID = member.ToOID });
                    item.Members.Add(new PmsRelationship { FromOID = item.ToOID, ToOID = person.OID, PersonNm = person.Name, Thumbnail = person.Thumbnail });
                    person = null;
                });
                if (_proj.BPolicy.Name == PmsConstant.POLICY_PROJECT_PREPARE || _proj.BPolicy.Name == PmsConstant.POLICY_PROJECT_PAUSED)
                {
                }
                else
                {
                    item.diseditable = PmsConstant.DISEDITABLE;
                }
                if (tmpToData.BPolicy.Name == PmsConstant.POLICY_PROCESS_PAUSED)
                {
                    if (tmpToData.ActEndDt != null)
                    {
                        item.Delay = PmsUtils.CalculateDelay(Convert.ToDateTime(tmpToData.EstEndDt), Convert.ToDateTime(tmpToData.ActEndDt), Convert.ToInt32(_proj.WorkingDay), _lHoliday);
                    }
                    else
                    {
                        item.Delay = PmsUtils.CalculateDelay(Convert.ToDateTime(tmpToData.EstEndDt), DateTime.Now, Convert.ToInt32(_proj.WorkingDay), _lHoliday);
                    }
                }
                GetWbsStructure(item, _projOID, _workingDay, _holiday, _proj, _lHoliday);
            });
        }

        public static List<Dictionary<string, object>> GetLDGanttWbs(string OID)
        {
            int Level = 0;
            PmsProject proj = PmsProjectRepository.SelPmsObject(new PmsProject { OID = Convert.ToInt32(OID) });
            List<DateTime> lHoliday = CalendarDetailRepository.SelCalendarDetails(new CalendarDetail { CalendarOID = proj.CalendarOID, IsHoliday = 1 }).Select(val => DateTime.Parse(val.Year + "-" + val.Month + "-" + val.Day)).ToList();
            List<Dictionary<string, object>> ldPmsWbs = new List<Dictionary<string, object>>();

            Dictionary<string, object> dProj = new Dictionary<string, object>();
            dProj.Add("oid", proj.OID);
            dProj.Add("id", 0);
            dProj.Add("name", proj.Name);
            dProj.Add("progress", 0);
            dProj.Add("progressByWorklog", false);
            dProj.Add("relevance", 0);
            dProj.Add("type", "");
            dProj.Add("typeId", "");
            dProj.Add("description", proj.Description);
            dProj.Add("code", Level);
            dProj.Add("level", Level);
            if (proj.BPolicy.Name != PmsConstant.POLICY_PROJECT_PREPARE)
            {
                if (proj.ActEndDt != null)
                {
                    dProj.Add("status", "STATUS_DONE");
                }
                else
                {
                    int delay = PmsUtils.CalculateDelay(Convert.ToDateTime(proj.EstEndDt), DateTime.Now, Convert.ToInt32(proj.WorkingDay), lHoliday);
                    if (delay > 1 && delay <= PmsConstant.DELAY)
                    {
                        dProj.Add("status", "STATUS_WAITING");
                    }
                    else if (delay > PmsConstant.DELAY)
                    {
                        dProj.Add("status", "STATUS_FAILED");
                    }
                    else
                    {
                        dProj.Add("status", "STATUS_ACTIVE");
                    }
                }
            }
            else
            {
                dProj.Add("status", "STATUS_SUSPENDED");
            }
            dProj.Add("depends", "");
            dProj.Add("canWrite", true);
            dProj.Add("start", proj.EstStartDt);
            dProj.Add("duration", proj.EstDuration);
            dProj.Add("end", proj.EstEndDt);
            dProj.Add("ActStart", proj.ActStartDt != null ? string.Format("{0:yyyy-MM-dd}", proj.ActStartDt) : "");
            dProj.Add("ActDuration", proj.ActDuration);
            dProj.Add("ActEnd", proj.ActEndDt != null ? string.Format("{0:yyyy-MM-dd}", proj.ActEndDt) : "");
            dProj.Add("startIsMilestone", false);
            dProj.Add("endIsMilestone", false);
            dProj.Add("collapsed", false);
            dProj.Add("assigs", new List<object>());
            ldPmsWbs.Add(dProj);
            GetGanttSturcture(dProj, ldPmsWbs, Convert.ToInt32(proj.WorkingDay), lHoliday);
            return ldPmsWbs;
        }

        public static void GetGanttSturcture(Dictionary<string, object> _dParent, List<Dictionary<string, object>> _ldStructure, int _iWorkingDay, List<DateTime> _lHoliday)
        {
            List<PmsRelationship> children = PmsRelationshipRepository.SelPmsRelationship(new PmsRelationship { FromOID = Convert.ToInt32(_dParent["oid"]), Type = PmsConstant.RELATIONSHIP_WBS });
            if (children != null && children.Count > 0)
            {
                _dParent.Add("hasChild", true);
            }
            else
            {
                _dParent.Add("hasChild", false);
            }
            PmsProcess tmpProcess = null;
            children.ForEach(item =>
            {
                if (tmpProcess != null)
                {
                    tmpProcess = null;
                }
                tmpProcess = PmsProcessRepository.SelPmsProcess(new PmsProcess { OID = item.ToOID });
                Dictionary<string, object> tmpChildren = new Dictionary<string, object>();
                tmpChildren.Add("oid", tmpProcess.OID);
                tmpChildren.Add("id", tmpProcess.Id);
                tmpChildren.Add("name", tmpProcess.Name);
                tmpChildren.Add("progress", 0);
                tmpChildren.Add("progressByWorklog", false);
                tmpChildren.Add("relevance", 0);
                tmpChildren.Add("type", "");
                tmpChildren.Add("typeId", "");
                tmpChildren.Add("description", tmpProcess.Description);
                tmpChildren.Add("code", Convert.ToInt32(_dParent["level"]) + 1);
                tmpChildren.Add("level", Convert.ToInt32(_dParent["level"]) + 1);
                if (tmpProcess.BPolicy.Name != PmsConstant.POLICY_PROCESS_PREPARE)
                {
                    if (tmpProcess.ActEndDt != null)
                    {
                        tmpChildren.Add("status", "STATUS_DONE");
                    }
                    else
                    {
                        int delay = PmsUtils.CalculateDelay(Convert.ToDateTime(tmpProcess.EstEndDt), DateTime.Now, _iWorkingDay, _lHoliday);
                        if (delay > 1 && delay <= PmsConstant.DELAY)
                        {
                            tmpChildren.Add("status", "STATUS_WAITING");
                        }
                        else if (delay > PmsConstant.DELAY)
                        {
                            tmpChildren.Add("status", "STATUS_FAILED");
                        }
                        else
                        {
                            tmpChildren.Add("status", "STATUS_ACTIVE");
                        }
                    }
                }
                else
                {
                    tmpChildren.Add("status", "STATUS_SUSPENDED");
                }
                tmpChildren.Add("depends", tmpProcess.Dependency != "" && tmpProcess.Dependency != null ? tmpProcess.Dependency : "");
                tmpChildren.Add("canWrite", true);
                tmpChildren.Add("start", tmpProcess.EstStartDt);
                tmpChildren.Add("duration", tmpProcess.EstDuration);
                tmpChildren.Add("end", tmpProcess.EstEndDt);
                tmpChildren.Add("ActStart", tmpProcess.ActStartDt != null ? string.Format("{0:yyyy-MM-dd}", tmpProcess.ActStartDt) : "");
                tmpChildren.Add("ActDuration", tmpProcess.ActDuration);
                tmpChildren.Add("ActEnd", tmpProcess.ActEndDt != null ? string.Format("{0:yyyy-MM-dd}", tmpProcess.ActEndDt) : "");
                tmpChildren.Add("startIsMilestone", false);
                tmpChildren.Add("endIsMilestone", false);
                tmpChildren.Add("collapsed", false);

                List<Dictionary<string, object>> lPerson = new List<Dictionary<string, object>>();
                PmsRelationshipRepository.SelPmsRelationship(new PmsRelationship { FromOID = tmpProcess.OID, Type = PmsConstant.RELATIONSHIP_MEMBER }).ForEach(member =>
                {
                    Person personData = PersonRepository.SelPerson(new Person { OID = member.ToOID });
                    Dictionary<string, object> person = new Dictionary<string, object>();
                    person.Add("resourceId", Convert.ToInt32(personData.OID));
                    person.Add("id", Convert.ToInt32(personData.OID));
                    person.Add("name", personData.Name);
                    person.Add("Thumbnail", personData.Thumbnail);
                    lPerson.Add(person);
                    personData = null;
                });

                tmpChildren.Add("assigs", lPerson);
                _ldStructure.Add(tmpChildren);
                GetGanttSturcture(tmpChildren, _ldStructure, _iWorkingDay, _lHoliday);
            });
        }

    }
}
