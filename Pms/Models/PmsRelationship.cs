using Common.Constant;
using Common.Factory;
using Common.Models;
using Document.Models;
using DocumentClassification.Models;
using Pms.Auth;
using Pms.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

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

        public int? TaskOID { get; set; }

        public string CheckListEtc { get; set; }

        //Temp Data
        public string ObjName { get; set; }

        public string ObjType { get; set; }

        public int? ObjSt { get; set; }

        public string ObjStNm { get; set; }

        public string ObjStDisNm { get; set; }

        public string ObjDescription { get; set; }

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

        public string No { get; set; }

        public string CreateUsNm { get; set; }

        //산출물 오브젝트 데이터
        public string ProjectNm { get; set; }

        public string TaskNm { get; set; }

        public string ViewUrl { get; set; }

        public string DocClassNm { get; set; }

        public string DocNm { get; set; }

        public string DocRev { get; set; }

        public string DocStNm { get; set; }

        public List<BPolicyAuth> BPolicyAuths { get; set; }
        public string Oem_Lib_Nm { get; set; }
        public string Car_Lib_Nm { get; set; }
        public string PartNm { get; set; }
        public string TotalTestDt { get; set; }
        public int? TotalTestItem { get; set; }
        public int? WaitingNum { get; set; }
        public int? ProgressNum { get; set; }
        public int? CompleteNum { get; set; }
        public int? NGNum { get; set; }
    }

    public static class PmsRelationshipRepository
    {

        public static int InsPmsRelationship(HttpSessionStateBase Context, PmsRelationship _param)
        {
            List<PmsRelationship> duplication = PmsRelationshipRepository.SelPmsRelationship(Context, _param);
            if (duplication != null && duplication.Count > 0)
            {
                return -1;
            }
            _param.CreateUs = Convert.ToInt32(Context["UserOID"]);

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

        public static int UdtPmsRelationship(HttpSessionStateBase Context, PmsRelationship _param)
        {
            return DaoFactory.SetUpdate("Pms.UdtPmsRelationship", _param);
        }

        public static List<PmsRelationship> SelPmsRelationship(HttpSessionStateBase Context, PmsRelationship _param)
        {
            return DaoFactory.GetList<PmsRelationship>("Pms.SelPmsRelationship", _param);
        }

        public static int DelPmsRelaionship(HttpSessionStateBase Context, PmsRelationship _param)
        {
            _param.DeleteUs = Convert.ToInt32(Context["UserOID"]);
            if (_param.OID == null)
            {
                return DaoFactory.SetUpdate("Pms.DelPmsRelationshipByData", _param);
            }
            return DaoFactory.SetUpdate("Pms.DelPmsRelationship", _param);
        }

        #region -- API : WBS Tree

        public static PmsRelationship GetProjWbsStructure(HttpSessionStateBase Context, int _level, int _fromOID, PmsProject _proj)
        {
            List<DateTime> lHoliday = CalendarDetailRepository.SelCalendarDetails(new CalendarDetail { CalendarOID = _proj.OID, IsHoliday = 1 }).Select(val => DateTime.Parse(val.Year + "-" + val.Month + "-" + val.Day)).ToList();
            string strHoliday = string.Join(",", CalendarDetailRepository.SelCalendarDetails(new CalendarDetail { CalendarOID = _proj.OID, IsHoliday = 1 }).Select(value => value.FullDate.ToString().ToArray()));
            List<PmsRelationship> lAllWbsData = PmsRelationshipRepository.SelPmsRelationship(Context, new PmsRelationship { RootOID = _fromOID, Type = PmsConstant.RELATIONSHIP_WBS });

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
            if (_proj.BPolicy.Name == PmsConstant.POLICY_PROJECT_PREPARE || _proj.BPolicy.Name == PmsConstant.POLICY_PROJECT_PAUSED || _proj.BPolicy.Name == PmsConstant.POLICY_PROJECT_TEMP_EXIST || _proj.BPolicy.Name == PmsConstant.POLICY_PROJECT_TEMP_DISPOSAL)
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
                    if (_proj.BPolicy.Name == PmsConstant.POLICY_PROJECT_STARTED)
                    {
                        getStructure.Delay = PmsUtils.CalculateDelay(Convert.ToDateTime(_proj.EstEndDt), Convert.ToDateTime(string.Format("{0:yyyy-MM-dd}", DateTime.Now)), Convert.ToInt32(_proj.WorkingDay), lHoliday);
                    }
                    else
                    {
                        getStructure.Delay = PmsUtils.CalculateDelay(Convert.ToDateTime(_proj.EstStartDt), Convert.ToDateTime(string.Format("{0:yyyy-MM-dd}", DateTime.Now)), Convert.ToInt32(_proj.WorkingDay), lHoliday);
                    }
                }
            }
            GetWbsStructure(Context, getStructure, _fromOID, Convert.ToInt32(_proj.WorkingDay), strHoliday, _proj, lHoliday, lAllWbsData);
            return getStructure;
        }

        public static PmsRelationship GetProcWbsStructure(HttpSessionStateBase Context, int _Level, PmsProcess _Proc)
        {
            PmsProject proj = PmsProjectRepository.SelPmsObject(Context, new PmsProject { OID = _Proc.RootOID });
            List<DateTime> lHoliday = CalendarDetailRepository.SelCalendarDetails(new CalendarDetail { CalendarOID = proj.OID, IsHoliday = 1 }).Select(val => DateTime.Parse(val.Year + "-" + val.Month + "-" + val.Day)).ToList();
            string strHoliday = string.Join(",", CalendarDetailRepository.SelCalendarDetails(new CalendarDetail { CalendarOID = proj.OID, IsHoliday = 1 }).Select(value => value.FullDate.ToString().ToArray()));

            PmsRelationship getStructure = new PmsRelationship();
            getStructure.ToOID = _Proc.OID;
            getStructure.ObjName = _Proc.Name;
            getStructure.Description = _Proc.Description;
            getStructure.ObjType = _Proc.Type;
            getStructure.EstDuration = _Proc.EstDuration;
            getStructure.EstStartDt = _Proc.EstStartDt != null ? Convert.ToDateTime(string.Format("{0:yyyy-MM-dd}", _Proc.EstStartDt)) : _Proc.EstStartDt;
            getStructure.EstEndDt = _Proc.EstEndDt != null ? Convert.ToDateTime(string.Format("{0:yyyy-MM-dd}", _Proc.EstEndDt)) : _Proc.EstEndDt;
            getStructure.ActDuration = _Proc.ActDuration;
            getStructure.ActStartDt = _Proc.ActStartDt != null ? Convert.ToDateTime(string.Format("{0:yyyy-MM-dd}", _Proc.ActStartDt)) : _Proc.ActStartDt;
            getStructure.ActEndDt = _Proc.ActEndDt != null ? Convert.ToDateTime(string.Format("{0:yyyy-MM-dd}", _Proc.ActEndDt)) : _Proc.ActEndDt;
            getStructure.ObjSt = _Proc.BPolicyOID;
            getStructure.ObjStNm = _Proc.BPolicy.StatusNm;
            getStructure.Id = _Proc.Id;
            getStructure.Dependency = _Proc.Dependency;
            getStructure.Complete = _Proc.Complete;
            getStructure.WorkingDay = proj.WorkingDay;
            getStructure.No = _Proc.No;
            getStructure.Members = new List<PmsRelationship>();
            PmsRelationshipRepository.SelPmsRelationship(Context, new PmsRelationship { FromOID = _Proc.OID, Type = PmsConstant.RELATIONSHIP_MEMBER }).ForEach(member =>
            {
                Person person = PersonRepository.SelPerson(Context, new Person { OID = member.ToOID });
                getStructure.Members.Add(new PmsRelationship { FromOID = _Proc.OID, ToOID = person.OID, PersonNm = person.Name, Thumbnail = person.Thumbnail });
                person = null;
            });

            if (_Proc.BPolicy.Name == PmsConstant.POLICY_PROJECT_PREPARE || _Proc.BPolicy.Name == PmsConstant.POLICY_PROJECT_PAUSED || _Proc.BPolicy.Name == PmsConstant.POLICY_PROJECT_TEMP_EXIST || _Proc.BPolicy.Name == PmsConstant.POLICY_PROJECT_TEMP_DISPOSAL)
            {
                getStructure.diseditable = PmsConstant.DISEDITABLE;
            }
            else
            {
                getStructure.diseditable = PmsConstant.FLOWEDITABLE;
                if (_Proc.ActEndDt != null)
                {
                    getStructure.Delay = PmsUtils.CalculateDelay(Convert.ToDateTime(_Proc.EstEndDt), Convert.ToDateTime(_Proc.ActEndDt), Convert.ToInt32(proj.WorkingDay), lHoliday);
                }
                else
                {
                    if (_Proc.BPolicy.Name == PmsConstant.POLICY_PROCESS_STARTED)
                    {
                        getStructure.Delay = PmsUtils.CalculateDelay(Convert.ToDateTime(_Proc.EstEndDt), Convert.ToDateTime(string.Format("{0:yyyy-MM-dd}", DateTime.Now)), Convert.ToInt32(proj.WorkingDay), lHoliday);
                    }
                    else
                    {
                        getStructure.Delay = PmsUtils.CalculateDelay(Convert.ToDateTime(_Proc.EstStartDt), Convert.ToDateTime(string.Format("{0:yyyy-MM-dd}", DateTime.Now)), Convert.ToInt32(proj.WorkingDay), lHoliday);
                    }
                }
            }
            GetWbsStructure(Context, getStructure, Convert.ToInt32(proj.OID), Convert.ToInt32(proj.WorkingDay), strHoliday, proj, lHoliday, null);
            return getStructure;
        }

        public static void GetWbsStructure(HttpSessionStateBase Context, PmsRelationship _relObj, int _projOID, int _workingDay, string _holiday, PmsProject _proj, List<DateTime> _lHoliday, List<PmsRelationship> _AllWbsData)
        {
            _relObj.RootOID = _projOID;
            //_relObj.Children = PmsRelationshipRepository.SelPmsRelationship(Context, new PmsRelationship { Type = PmsConstant.RELATIONSHIP_WBS, FromOID = _relObj.ToOID });
            if (_AllWbsData == null || _AllWbsData.Count < 1)
            {
                _relObj.Children = PmsRelationshipRepository.SelPmsRelationship(Context, new PmsRelationship { Type = PmsConstant.RELATIONSHIP_WBS, FromOID = _relObj.ToOID });
            }
            else
            {
                _relObj.Children = _AllWbsData.FindAll(data => data.FromOID == _relObj.ToOID).OrderBy(data => data.Ord).ToList();
            }

            List<int> children = _relObj.Children.Select(sel => Convert.ToInt32(sel.ToOID)).ToList();
            if (children == null || children.Count < 1)
            {
                return;
            }
            List<PmsProcess> tmpToDatas = PmsProcessRepository.SelPmsProcessOIDs(Context, new PmsProcess { OIDs = children });
            PmsProcess tmpToData = null;
            _relObj.Children.ForEach(item =>
            {
                item.Level = _relObj.Level + 1;
                //item.FromData = PmsProcessRepository.SelPmsProcess(new PmsProcess { OID = item.FromOID });
                //item.ToData = PmsProcessRepository.SelPmsProcess(new PmsProcess { OID = item.ToOID });
                //PmsProcess tmpFromData = PmsProcessRepository.SelPmsProcess(new PmsProcess { OID = item.FromOID });
                if (tmpToData != null)
                {
                    tmpToData = null;
                }
                //tmpToData = PmsProcessRepository.SelPmsProcess(Context, new PmsProcess { OID = item.ToOID });
                tmpToData = tmpToDatas.Find(data => data.OID == item.ToOID);
                if (tmpToData == null)
                {
                    tmpToData = PmsProcessRepository.SelPmsProcess(Context, new PmsProcess { OID = item.ToOID });
                }
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
                item.No = tmpToData.No;
                item.Members = new List<PmsRelationship>();
                PmsRelationshipRepository.SelPmsRelationship(Context, new PmsRelationship { FromOID = item.ToOID, Type = PmsConstant.RELATIONSHIP_MEMBER }).ForEach(member =>
                {
                    Person person = PersonRepository.SelPerson(Context, new Person { OID = member.ToOID });
                    item.Members.Add(new PmsRelationship { FromOID = item.ToOID, ToOID = person.OID, PersonNm = person.Name, Thumbnail = person.Thumbnail });
                    person = null;
                });

                if (_proj.BPolicy.Name == PmsConstant.POLICY_PROJECT_PREPARE || _proj.BPolicy.Name == PmsConstant.POLICY_PROJECT_PAUSED || _proj.BPolicy.Name == PmsConstant.POLICY_PROJECT_TEMP_EXIST || _proj.BPolicy.Name == PmsConstant.POLICY_PROJECT_TEMP_DISPOSAL)
                {
                    if (item.ObjType.Equals(PmsConstant.TYPE_PHASE))
                    {
                        //item.diseditable = PmsConstant.FLOWEDITABLE;
                        item.diseditable = PmsConstant.PFLOWEDITABLE;
                    }
                }
                else
                {
                    item.diseditable = PmsConstant.DISEDITABLE;

                    if (tmpToData.ActEndDt != null)
                    {
                        item.Delay = PmsUtils.CalculateDelay(Convert.ToDateTime(tmpToData.EstEndDt), Convert.ToDateTime(tmpToData.ActEndDt), Convert.ToInt32(_proj.WorkingDay), _lHoliday);
                    }
                    else
                    {
                        if (tmpToData.BPolicy.Name == PmsConstant.POLICY_PROCESS_STARTED)
                        {
                            item.Delay = PmsUtils.CalculateDelay(Convert.ToDateTime(tmpToData.EstEndDt), Convert.ToDateTime(string.Format("{0:yyyy-MM-dd}", DateTime.Now)), Convert.ToInt32(_proj.WorkingDay), _lHoliday);
                        }
                        else
                        {
                            item.Delay = PmsUtils.CalculateDelay(Convert.ToDateTime(tmpToData.EstStartDt), Convert.ToDateTime(string.Format("{0:yyyy-MM-dd}", DateTime.Now)), Convert.ToInt32(_proj.WorkingDay), _lHoliday);
                        }
                    }
                }
                GetWbsStructure(Context, item, _projOID, _workingDay, _holiday, _proj, _lHoliday, _AllWbsData);
            });
        }

        #endregion

        #region -- API : GANTT

        public static List<Dictionary<string, object>> GetLDGanttWbs(HttpSessionStateBase Context, string OID)
        {
            int Level = 0;
            PmsProject proj = PmsProjectRepository.SelPmsObject(Context, new PmsProject { OID = Convert.ToInt32(OID) });
            List<DateTime> lHoliday = CalendarDetailRepository.SelCalendarDetails(new CalendarDetail { CalendarOID = proj.CalendarOID, IsHoliday = 1 }).Select(val => DateTime.Parse(val.Year + "-" + val.Month + "-" + val.Day)).ToList();
            List<Dictionary<string, object>> ldPmsWbs = new List<Dictionary<string, object>>();
            List<PmsRelationship> lAllWbsData = PmsRelationshipRepository.SelPmsRelationship(Context, new PmsRelationship { Type = PmsConstant.RELATIONSHIP_WBS, RootOID = proj.OID });

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
            GetGanttSturcture(Context, dProj, ldPmsWbs, Convert.ToInt32(proj.WorkingDay), lHoliday, lAllWbsData);
            return ldPmsWbs;
        }

        public static void GetGanttSturcture(HttpSessionStateBase Context, Dictionary<string, object> _dParent, List<Dictionary<string, object>> _ldStructure, int _iWorkingDay, List<DateTime> _lHoliday, List<PmsRelationship> _lAllWbsData)
        {
            //List<PmsRelationship> children = PmsRelationshipRepository.SelPmsRelationship(Context, new PmsRelationship { FromOID = Convert.ToInt32(_dParent["oid"]), Type = PmsConstant.RELATIONSHIP_WBS });
            List<PmsRelationship> children = _lAllWbsData.FindAll(data => data.FromOID == Convert.ToInt32(_dParent["oid"]));
            if (children != null && children.Count > 0)
            {
                _dParent.Add("hasChild", true);
            }
            else
            {
                _dParent.Add("hasChild", false);
            }

            List<int> iChildrenOID = children.Select(sel => Convert.ToInt32(sel.ToOID)).ToList();
            if (iChildrenOID == null || iChildrenOID.Count < 1)
            {
                return;
            }
            List<PmsProcess> tmpToDatas = PmsProcessRepository.SelPmsProcessOIDs(Context, new PmsProcess { OIDs = iChildrenOID });
            PmsProcess tmpProcess = null;
            children.ForEach(item =>
            {
                if (tmpProcess != null)
                {
                    tmpProcess = null;
                }
                //tmpProcess = PmsProcessRepository.SelPmsProcess(Context, new PmsProcess { OID = item.ToOID });
                tmpProcess = tmpToDatas.Find(data => data.OID == item.ToOID);
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
                PmsRelationshipRepository.SelPmsRelationship(Context, new PmsRelationship { FromOID = tmpProcess.OID, Type = PmsConstant.RELATIONSHIP_MEMBER }).ForEach(member =>
                {
                    Person personData = PersonRepository.SelPerson(Context, new Person { OID = member.ToOID });
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
                GetGanttSturcture(Context, tmpChildren, _ldStructure, _iWorkingDay, _lHoliday, _lAllWbsData);
            });
        }

        #endregion

        #region -- API : WBS LIST

        public static List<PmsRelationship> GetProjWbsLIst(HttpSessionStateBase Context, string OID)
        {
            int Level = 0;
            PmsProject proj = PmsProjectRepository.SelPmsObject(Context, new PmsProject { OID = Convert.ToInt32(OID) });

            List<PmsRelationship> lWbs = new List<PmsRelationship>();
            PmsRelationship getStructure = new PmsRelationship();
            getStructure.Level = Level;
            getStructure.ToOID = proj.OID;
            getStructure.ObjName = proj.Name;
            getStructure.Description = proj.Description;
            getStructure.ObjType = proj.Type;
            getStructure.EstDuration = proj.EstDuration;
            getStructure.EstStartDt = proj.EstStartDt != null ? Convert.ToDateTime(string.Format("{0:yyyy-MM-dd}", proj.EstStartDt)) : proj.EstStartDt;
            getStructure.EstEndDt = proj.EstEndDt != null ? Convert.ToDateTime(string.Format("{0:yyyy-MM-dd}", proj.EstEndDt)) : proj.EstEndDt;
            getStructure.ActDuration = proj.ActDuration;
            getStructure.ActStartDt = proj.ActStartDt != null ? Convert.ToDateTime(string.Format("{0:yyyy-MM-dd}", proj.ActStartDt)) : proj.ActStartDt;
            getStructure.ActEndDt = proj.ActEndDt != null ? Convert.ToDateTime(string.Format("{0:yyyy-MM-dd}", proj.ActEndDt)) : proj.ActEndDt;
            getStructure.ObjSt = proj.BPolicyOID;
            getStructure.ObjStNm = proj.BPolicy.Name;
            getStructure.Complete = proj.Complete;
            getStructure.WorkingDay = proj.WorkingDay;
            getStructure.Id = null;
            getStructure.Dependency = null;
            lWbs.Add(getStructure);
            GetWbsList(Context, getStructure, lWbs);
            return lWbs;
        }

        public static List<PmsRelationship> GetProcWbsLIst(HttpSessionStateBase Context, string OID)
        {
            int Level = 0;
            PmsProcess proc = PmsProcessRepository.SelPmsProcess(Context, new PmsProcess { OID = Convert.ToInt32(OID) });

            List<PmsRelationship> lWbs = new List<PmsRelationship>();
            PmsRelationship getStructure = new PmsRelationship();
            getStructure.Level = Level;
            getStructure.ToOID = proc.OID;
            getStructure.ObjName = proc.Name;
            getStructure.Description = proc.Description;
            getStructure.ObjType = proc.Type;
            getStructure.EstDuration = proc.EstDuration;
            getStructure.EstStartDt = proc.EstStartDt != null ? Convert.ToDateTime(string.Format("{0:yyyy-MM-dd}", proc.EstStartDt)) : proc.EstStartDt;
            getStructure.EstEndDt = proc.EstEndDt != null ? Convert.ToDateTime(string.Format("{0:yyyy-MM-dd}", proc.EstEndDt)) : proc.EstEndDt;
            getStructure.ActDuration = proc.ActDuration;
            getStructure.ActStartDt = proc.ActStartDt != null ? Convert.ToDateTime(string.Format("{0:yyyy-MM-dd}", proc.ActStartDt)) : proc.ActStartDt;
            getStructure.ActEndDt = proc.ActEndDt != null ? Convert.ToDateTime(string.Format("{0:yyyy-MM-dd}", proc.ActEndDt)) : proc.ActEndDt;
            getStructure.ObjSt = proc.BPolicyOID;
            getStructure.ObjStNm = proc.BPolicy.Name;
            getStructure.Complete = proc.Complete;
            getStructure.Id = proc.Id;
            getStructure.Dependency = proc.Dependency;
            getStructure.No = proc.No;
            lWbs.Add(getStructure);
            GetWbsList(Context, getStructure, lWbs);
            return lWbs;
        }

        public static void GetWbsList(HttpSessionStateBase Context, PmsRelationship _parent, List<PmsRelationship> _ldStructure)
        {
            List<PmsRelationship> children = PmsRelationshipRepository.SelPmsRelationship(Context, new PmsRelationship { FromOID = Convert.ToInt32(_parent.ToOID), Type = PmsConstant.RELATIONSHIP_WBS });
            PmsProject tmpProject = null;
            PmsProcess tmpToData = null;
            List<DateTime> lHoliday = null;

            List<int> iChildren = children.Select(sel => Convert.ToInt32(sel.ToOID)).ToList();
            if (iChildren == null || iChildren.Count < 1)
            {
                return;
            }
            List<PmsProcess> tmpToDatas = PmsProcessRepository.SelPmsProcessOIDs(Context, new PmsProcess { OIDs = iChildren });
            children.ForEach(item =>
            {
                if (tmpToData != null)
                {
                    tmpToData = null;
                }
                //tmpToData = PmsProcessRepository.SelPmsProcess(Context, new PmsProcess { OID = item.ToOID });
                tmpToData = tmpToDatas.Find(data => data.OID == item.ToOID);
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
                item.ObjStNm = tmpToData.BPolicy.Name;
                item.ObjStDisNm = tmpToData.BPolicy.StatusNm;
                item.Id = tmpToData.Id;
                item.Dependency = tmpToData.Dependency;
                item.Complete = tmpToData.Complete;
                item.No = tmpToData.No;
                item.Level = tmpToData.Level;
                item.Members = new List<PmsRelationship>();
                PmsRelationshipRepository.SelPmsRelationship(Context, new PmsRelationship { FromOID = item.ToOID, Type = PmsConstant.RELATIONSHIP_MEMBER }).ForEach(member =>
                {
                    Person person = PersonRepository.SelPerson(Context, new Person { OID = member.ToOID });
                    item.Members.Add(new PmsRelationship { FromOID = item.ToOID, ToOID = person.OID, PersonNm = person.Name, Thumbnail = person.Thumbnail });
                    person = null;
                });

                if (tmpProject != null)
                {
                    tmpProject = null;
                }
                if (lHoliday != null)
                {
                    lHoliday = null;
                }
                tmpProject = PmsProjectRepository.SelPmsObject(Context, new PmsProject { OID = item.RootOID });
                lHoliday = CalendarDetailRepository.SelCalendarDetails(new CalendarDetail { CalendarOID = Convert.ToInt32(tmpProject.CalendarOID), IsHoliday = 1 }).Select(val => DateTime.Parse(val.Year + "-" + val.Month + "-" + val.Day)).ToList();
                if (item.ActEndDt != null)
                {

                    item.Delay = PmsUtils.CalculateDelay(Convert.ToDateTime(item.EstEndDt), Convert.ToDateTime(item.ActEndDt), Convert.ToInt32(tmpProject.WorkingDay), lHoliday);
                }
                else
                {
                    item.Delay = PmsUtils.CalculateDelay(Convert.ToDateTime(item.EstEndDt), Convert.ToDateTime(string.Format("{0:yyyy-MM-dd}", DateTime.Now)), Convert.ToInt32(tmpProject.WorkingDay), lHoliday);
                }
                _ldStructure.Add(item);
                GetWbsList(Context, item, _ldStructure);
            });
        }

        #endregion

        #region -- API : Deliveries LIST
        public static List<PmsRelationship> SelPmsDeliveriesRelationship(HttpSessionStateBase Context, int _Lev, PmsRelationship _param)
        {
            PmsProject project = PmsProjectRepository.SelPmsObject(Context, new PmsProject { OID = _param.RootOID, Type = (_param.ObjType != null ? _param.ObjType : null), IsTemplate = (_param.ObjType != null ? _param.ObjType : null) });
            _param.Type = Common.Constant.PmsConstant.RELATIONSHIP_DOC_MASTER;
            List<PmsRelationship> RelationshipList = PmsRelationshipRepository.SelPmsRelationship(Context, _param);
            //List<PmsRelationship> lProjectList = PmsRelationshipRepository.GetProjWbsLIst(Context, Convert.ToString(project.OID));
            List<PmsRelationship> lProjectList = PmsRelationshipRepository.GetWbsOidLIst(Context, Convert.ToString(project.OID));
            List<PmsRelationship> getStructureList = new List<PmsRelationship>();

            DocClass DocClas = null;
            PmsProcess Task = null;

            if (RelationshipList == null || RelationshipList.Count < 1)
            {
                return getStructureList;
            }

            List<int> PersonOIDs = RelationshipList.Select(sel => Convert.ToInt32(sel.CreateUs)).ToList();
            List<int> TaskOIDs = RelationshipList.Select(sel => Convert.ToInt32(sel.FromOID)).ToList();

            List<Person> tmpPersonDatas = PersonRepository.SelPersons(Context, new Person { OIDs = PersonOIDs });
            List<PmsProcess> tmpTaskDatas = PmsProcessRepository.SelPmsProcessOIDs(Context, new PmsProcess { OIDs = TaskOIDs });

            foreach (var obj in RelationshipList)
            {
                if (lProjectList.FindAll(item => item.ToOID == obj.FromOID).Count > 0)
                {
                    if (DocClas != null)
                    {
                        DocClas = null;
                    }
                    if (Task != null)
                    {
                        Task = null;
                    }
                    PmsRelationship getStructure = new PmsRelationship();

                    DocClas = DocClassRepository.SelDocClassObject(Context, new DocClass { OID = obj.ToOID });
                    getStructure.Level = _Lev;

                    getStructure.OID = obj.OID;
                    getStructure.RootOID = project.OID;
                    getStructure.ProjectNm = project.Name;
                    getStructure.FromOID = obj.FromOID;
                    getStructure.DocClassNm = DocClas.Name;
                    getStructure.Type = obj.Type;
                    //getStructure.CreateUsNm = PersonRepository.SelPerson(Context, new Person { OID = obj.CreateUs }).Name;
                    getStructure.CreateUsNm = tmpPersonDatas.Find(data => data.OID == obj.CreateUs).Name;
                    Task = new PmsProcess();
                    if (getStructure.FromOID != getStructure.RootOID)
                    {
                        //Task = PmsProcessRepository.SelPmsProcess(Context, new PmsProcess { OID = obj.FromOID });
                        Task = tmpTaskDatas.Find(data => data.OID == obj.FromOID);
                        getStructure.TaskNm = Task.Name;
                        getStructure.TaskOID = Task.OID;
                        getStructure.BPolicyAuths = Task.BPolicyAuths;
                    }
                    getStructure.ToOID = DocClas.OID;
                    getStructure.ViewUrl = DocClas.ViewUrl;

                    getPmsDeliveriesStructure(Context, getStructure, Convert.ToInt32(obj.RootOID), project.Name, Task.Name, getStructure.ViewUrl);
                    getStructureList.Add(getStructure);
                }
            }
            return getStructureList;
        }

        public static void getPmsDeliveriesStructure(HttpSessionStateBase Context, PmsRelationship _relData, int _rootOID, string projectNm, string TaskNm, string ViewUrl)
        {

            _relData.Children = getPmsDeliveries(Context, new PmsRelationship { RootOID = _rootOID, FromOID = _relData.ToOID, TaskOID = _relData.TaskOID, DocClassNm = _relData.DocClassNm }, _rootOID, projectNm, TaskNm, ViewUrl);
        }

        public static List<PmsRelationship> getPmsDeliveries(HttpSessionStateBase Context, PmsRelationship _param, int _rootOID, string projectNm, string TaskNm, string ViewUrl)
        {
            _param.Type = Common.Constant.PmsConstant.RELATIONSHIP_DOC_CLASS;
            List<PmsRelationship> ProjectList = new List<PmsRelationship>();
            List<PmsRelationship> delList = new List<PmsRelationship>();
            if (_param.TaskOID != null)
            {
                ProjectList = PmsRelationshipRepository.SelPmsRelationship(Context, _param);
            }
            else
            {
                ProjectList = DaoFactory.GetList<PmsRelationship>("Pms.SelPmsRelationshipTaskIsNull", _param);
            }

            foreach (PmsRelationship Obj in ProjectList)
            {
                Obj.OID = Obj.OID;
                Obj.RootOID = _rootOID;
                Obj.ProjectNm = projectNm;
                Obj.TaskNm = TaskNm;
                if (ViewUrl == null)
                {
                    Doc Doc = DocRepository.SelDocObject(Context, new Doc { OID = Obj.ToOID, DocGroup = DocClassConstant.TYPE_DOCCLASS });
                    if (Doc.IsLatest == 0 && Doc.IsReleasedLatest == 0)
                    {
                        delList.Add(Obj);
                    }
                    else
                    {
                        Obj.DocClassNm = Doc.DocType_KorNm;
                        Obj.ToOID = Doc.OID;
                        Obj.DocNm = Doc.Title;
                        Obj.DocRev = Doc.Revision;
                        Obj.DocStNm = Doc.BPolicy.StatusNm;
                        Obj.CreateDt = Doc.CreateDt;
                        Obj.CreateUsNm = Doc.CreateUsNm;
                    }
                }
                else
                {
                    if (_param.DocClassNm == Common.Constant.DocClassConstant.ATTRIBUTE_RELIABILITY)
                    {
                        PmsReliability Reliability = PmsReliabilityRepository.SelPmsReliabilityObject(Context, new PmsReliability { OID = Obj.ToOID });
                        Obj.DocClassNm = _param.DocClassNm;
                        Obj.ToOID = Reliability.OID;
                        Obj.DocNm = Reliability.Name;
                        Obj.DocRev = Reliability.Revision;
                        Obj.DocStNm = Reliability.BPolicy.StatusNm;
                        Obj.CreateDt = Reliability.CreateDt;
                        Obj.CreateUsNm = Reliability.CreateUsNm;
                        Obj.ViewUrl = ViewUrl;
                    }
                    else if (_param.DocClassNm == Common.Constant.DocClassConstant.ATTRIBUTE_RELIABILITY_REPORT)
                    {
                        PmsReliabilityReport Reliability = PmsReliabilityReportRepository.SelPmsReliabilityReportObject(Context, new PmsReliabilityReport { OID = Obj.ToOID });
                        Obj.DocClassNm = _param.DocClassNm;
                        Obj.ToOID = Reliability.OID;
                        Obj.DocNm = Reliability.Name;
                        Obj.DocRev = Reliability.Revision;
                        Obj.DocStNm = Reliability.BPolicy.StatusNm;
                        Obj.CreateDt = Reliability.CreateDt;
                        Obj.CreateUsNm = Reliability.CreateUsNm;
                        Obj.ViewUrl = ViewUrl;
                    }
                }
            }

            return ProjectList.Except(delList).ToList();
        }

        #endregion

        #region -- API : WBS LIST ROOT

        public static List<PmsRelationship> GetWbsOidLIst(HttpSessionStateBase Context, string OID)
        {
            List<PmsRelationship> lWbs = new List<PmsRelationship>();
            List<PmsRelationship> AllChildrens = PmsRelationshipRepository.SelPmsRelationship(Context, new PmsRelationship { RootOID = Convert.ToInt32(OID), Type = PmsConstant.RELATIONSHIP_WBS });
            PmsRelationship getStructure = new PmsRelationship();
            getStructure.ToOID = Convert.ToInt32(OID);
            lWbs.Add(getStructure);

            GetWbsChildOidList(Context, getStructure, lWbs, AllChildrens);
            return lWbs;
        }

        public static void GetWbsChildOidList(HttpSessionStateBase Context, PmsRelationship _parent, List<PmsRelationship> _ldStructure, List<PmsRelationship> _lAllData)
        {
            //List<PmsRelationship> children = PmsRelationshipRepository.SelPmsRelationship(Context, new PmsRelationship { FromOID = Convert.ToInt32(_parent.ToOID), Type = PmsConstant.RELATIONSHIP_WBS });
            List<PmsRelationship> children = _lAllData.FindAll(data => data.FromOID == _parent.ToOID).OrderBy(data => data.Ord).ToList();
            children.ForEach(item =>
            {
                _ldStructure.Add(item);
                GetWbsChildOidList(Context, item, _ldStructure, _lAllData);
            });
        }

        #endregion

        #region -- API : WBS LIST Mini Data ROOT

        public static List<PmsRelationship> GetProjWbsTypeOidList(HttpSessionStateBase Context, string OID)
        {
            List<PmsRelationship> lWbs = new List<PmsRelationship>();
            List<PmsRelationship> lAllData = PmsRelationshipRepository.SelPmsRelationship(Context, new PmsRelationship { RootOID = Convert.ToInt32(OID), Type = PmsConstant.RELATIONSHIP_WBS });
            List<PmsProcess> lAllProc = PmsProcessRepository.SelPmsProcessOIDs(Context, new PmsProcess { OIDs = lAllData.Select(sel => Convert.ToInt32(sel.ToOID)).ToList() });

            PmsRelationship getStructure = new PmsRelationship();
            getStructure.ToOID = Convert.ToInt32(OID);
            getStructure.ObjType = PmsConstant.TYPE_PROJECT;
            lWbs.Add(getStructure);

            GetWbsChildTypeOidList(Context, getStructure, lWbs, lAllData, lAllProc);
            return lWbs;
        }

        public static void GetWbsChildTypeOidList(HttpSessionStateBase Context, PmsRelationship _parent, List<PmsRelationship> _ldStructure, List<PmsRelationship> _lAllData, List<PmsProcess> _lAllProc)
        {
            //List<PmsRelationship> children = PmsRelationshipRepository.SelPmsRelationship(Context, new PmsRelationship { FromOID = Convert.ToInt32(_parent.ToOID), Type = PmsConstant.RELATIONSHIP_WBS });
            List<PmsRelationship> children = _lAllData.FindAll(data => data.FromOID == _parent.ToOID).OrderBy(data => data.Ord).ToList();

            List<int> iChildren = children.Select(sel => Convert.ToInt32(sel.ToOID)).ToList();
            if (iChildren == null || iChildren.Count < 1)
            {
                return;
            }
            //List<PmsProcess> tmpToDatas = PmsProcessRepository.SelPmsProcessOIDs(Context, new PmsProcess { OIDs = iChildren });
            List<PmsProcess> tmpToDatas = _lAllProc.FindAll(data => iChildren.Contains(Convert.ToInt32(data.OID)));
            PmsProcess tmpProcess = null;
            children.ForEach(item =>
            {
                if (tmpProcess != null)
                {
                    tmpProcess = null;
                }
                //tmpProcess = PmsProcessRepository.SelPmsProcess(Context, new PmsProcess { OID = item.ToOID });
                tmpProcess = tmpToDatas.Find(data => data.OID == item.ToOID);
                item.ObjType = tmpProcess.Type;
                item.ObjName = tmpProcess.Name;
                item.EstStartDt = tmpProcess.EstStartDt;
                item.EstEndDt = tmpProcess.EstEndDt;
                item.EstDuration = tmpProcess.EstDuration;
                item.ActStartDt = tmpProcess.ActStartDt;
                item.ActEndDt = tmpProcess.ActEndDt;
                item.ActDuration = tmpProcess.ActDuration;
                item.Id = tmpProcess.Id;
                item.Dependency = tmpProcess.Dependency;
                item.ObjStNm = tmpProcess.BPolicy.Name;
                _ldStructure.Add(item);
                GetWbsChildTypeOidList(Context, item, _ldStructure, _lAllData, _lAllProc);
            });
        }

        public static List<PmsRelationship> getProcWbsTypeOidList(HttpSessionStateBase Context, string OID)
        {
            List<PmsRelationship> lWbs = new List<PmsRelationship>();

            PmsRelationship getStructure = new PmsRelationship();
            getStructure.ToOID = Convert.ToInt32(OID);
            lWbs.Add(getStructure);

            GetWbsChildTypeOidList(Context, getStructure, lWbs);
            return lWbs;
        }

        public static void GetWbsChildTypeOidList(HttpSessionStateBase Context, PmsRelationship _parent, List<PmsRelationship> _ldStructure)
        {
            List<PmsRelationship> children = PmsRelationshipRepository.SelPmsRelationship(Context, new PmsRelationship { FromOID = Convert.ToInt32(_parent.ToOID), Type = PmsConstant.RELATIONSHIP_WBS });

            List<int> iChildren = children.Select(sel => Convert.ToInt32(sel.ToOID)).ToList();
            if (iChildren == null || iChildren.Count < 1)
            {
                return;
            }
            List<PmsProcess> tmpToDatas = PmsProcessRepository.SelPmsProcessOIDs(Context, new PmsProcess { OIDs = iChildren });
            PmsProcess tmpProcess = null;
            children.ForEach(item =>
            {
                if (tmpProcess != null)
                {
                    tmpProcess = null;
                }
                //tmpProcess = PmsProcessRepository.SelPmsProcess(Context, new PmsProcess { OID = item.ToOID });
                tmpProcess = tmpToDatas.Find(data => data.OID == item.ToOID);
                item.ObjType = tmpProcess.Type;
                item.ObjName = tmpProcess.Name;
                item.EstStartDt = tmpProcess.EstStartDt;
                item.EstEndDt = tmpProcess.EstEndDt;
                item.EstDuration = tmpProcess.EstDuration;
                item.ActStartDt = tmpProcess.ActStartDt;
                item.ActEndDt = tmpProcess.ActEndDt;
                item.ActDuration = tmpProcess.ActDuration;
                item.Id = tmpProcess.Id;
                item.Dependency = tmpProcess.Dependency;
                item.ObjStNm = tmpProcess.BPolicy.Name;
                _ldStructure.Add(item);
                GetWbsChildTypeOidList(Context, item, _ldStructure);
            });
        }

        #endregion

    }
}
