﻿using Common.Constant;
using Common.Models;
using Pms.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Trigger;

namespace Pms.Trigger
{
    public class ProcessTrigger
    {
        public string ActionDefaultProcessPromote(object[] args)
        {
            object[] oArgs = args;
            HttpSessionStateBase Context = (HttpSessionStateBase)oArgs[0];
            int iUser = Convert.ToInt32(Context["UserOID"]);
            string type = Convert.ToString(oArgs[1]);
            string status = Convert.ToString(oArgs[2]);
            string oid = Convert.ToString(oArgs[3]);
            string action = Convert.ToString(oArgs[5]);
            string goStatus = "";
            if (oArgs.Length > 7)
            {
                goStatus = Convert.ToString(oArgs[7]);
            }

            try
            {
                DObject dobj = DObjectRepository.SelDObject(Context, new DObject { OID = Convert.ToInt32(oid) });
                if (dobj.Type == PmsConstant.TYPE_PHASE)
                {
                    PmsProcess tmpProcess = PmsProcessRepository.SelPmsProcess(Context, new PmsProcess { OID = dobj.OID });
                    if (tmpProcess.ActStartDt == null)
                    {
                        PmsProcessRepository.UdtPmsProcess(Context, new PmsProcess { OID = dobj.OID, ActStartDt = DateTime.Now, Dependency = tmpProcess.Dependency });
                        tmpProcess = null;
                    }
                    else
                    {
                        int RootOID = Convert.ToInt32(PmsRelationshipRepository.SelPmsRelationship(Context, new PmsRelationship { Type = PmsConstant.RELATIONSHIP_WBS, ToOID = dobj.OID }).First().RootOID);
                        PmsProcess proc = PmsProcessRepository.SelPmsProcess(Context, new PmsProcess { OID = dobj.OID });

                        List<PmsRelationship> lChildren = PmsRelationshipRepository.GetProcWbsLIst(Context, Convert.ToString(proc.OID)).FindAll(child => child.ObjStNm != PmsConstant.POLICY_PROCESS_COMPLETED);
                        if (lChildren.Count == 1)
                        {
                            PmsProject proj = PmsProjectRepository.SelPmsObject(Context, new PmsProject { OID = RootOID });
                            List<DateTime> lHoliday = CalendarDetailRepository.SelCalendarDetails(new CalendarDetail { CalendarOID = proj.CalendarOID, IsHoliday = 1 }).Select(val => DateTime.Parse(val.Year + "-" + val.Month + "-" + val.Day)).ToList();
                            PmsProcessRepository.UdtPmsProcess(Context,
                            new PmsProcess
                            {
                                OID = dobj.OID,
                                ActEndDt = DateTime.Now,
                                ActDuration = PmsUtils.CalculateGapFutureDuration(Convert.ToDateTime(Convert.ToDateTime(proc.ActStartDt).ToString("yyyy-MM-dd")), Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd")), Convert.ToInt32(proj.WorkingDay), lHoliday),
                                Complete = 100,
                                Dependency = proc.Dependency
                            });
                            DObjectRepository.UdtDObject(Context, new DObject { BPolicyOID = BPolicyRepository.SelBPolicy(new BPolicy { Type = dobj.Type, Name = PmsConstant.POLICY_PROCESS_COMPLETED }).First().OID, OID = dobj.OID });
                        }
                    } 
                }

                if (dobj.Type == PmsConstant.TYPE_TASK && (action.Equals(CommonConstant.ACTION_PROMOTE) || goStatus != null))
                {
                    if (dobj.BPolicy.Name == PmsConstant.POLICY_PROCESS_COMPLETED)
                    {
                        int RootOID = Convert.ToInt32(PmsRelationshipRepository.SelPmsRelationship(Context, new PmsRelationship { Type = PmsConstant.RELATIONSHIP_WBS, ToOID = dobj.OID }).First().RootOID);
                        PmsProject proj = PmsProjectRepository.SelPmsObject(Context, new PmsProject { OID = RootOID });
                        List<DateTime> lHoliday = CalendarDetailRepository.SelCalendarDetails(new CalendarDetail { CalendarOID = proj.CalendarOID, IsHoliday = 1 }).Select(val => DateTime.Parse(val.Year + "-" + val.Month + "-" + val.Day)).ToList();
                        PmsProcess proc = PmsProcessRepository.SelPmsProcess(Context, new PmsProcess { OID = dobj.OID });
                        PmsProcessRepository.UdtPmsProcess(Context,
                        new PmsProcess
                        {
                            OID = dobj.OID,
                            ActEndDt = DateTime.Now,
                            ActDuration = PmsUtils.CalculateGapFutureDuration(Convert.ToDateTime(Convert.ToDateTime(proc.ActStartDt).ToString("yyyy-MM-dd")), Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd")), Convert.ToInt32(proj.WorkingDay), lHoliday),
                            Complete = 100,
                            Dependency = proc.Dependency
                        });

                        List<PmsRelationship> lParent = PmsRelationshipRepository.SelPmsRelationship(Context, new PmsRelationship { Type = PmsConstant.RELATIONSHIP_WBS, ToOID = Convert.ToInt32(oid) });
                        if (lParent.Count > 0)
                        {
                            PmsRelationship parent = lParent.First();
                            PmsProcess parentProc = PmsProcessRepository.SelPmsProcess(Context, new PmsProcess { OID = parent.FromOID });
                            if (parentProc.Type == PmsConstant.TYPE_PHASE)
                            {
                                string result = TriggerUtil.StatusObjectPromote(Context, false, parentProc.Type, Convert.ToString(parentProc.BPolicyOID), null, Convert.ToInt32(parentProc.OID), Convert.ToInt32(parentProc.OID), CommonConstant.ACTION_PROMOTE, "");
                                if (result != null && result.Length > 0)
                                {
                                    return result;
                                }
                            }
                        }
                        lParent = null;
                    }
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            return "";
        }

        public string CheckTaskStartedProcessPromote(object[] args)
        {
            object[] oArgs = args;
            HttpSessionStateBase Context = (HttpSessionStateBase)oArgs[0];
            int iUser = Convert.ToInt32(Context["UserOID"]);
            string type = Convert.ToString(oArgs[1]);
            string status = Convert.ToString(oArgs[2]);
            string oid = Convert.ToString(oArgs[3]);
            try
            {
                DObject dobj = DObjectRepository.SelDObject(Context, new DObject { OID = Convert.ToInt32(oid) });
                PmsRelationship parent = PmsRelationshipRepository.SelPmsRelationship(Context, new PmsRelationship { Type = PmsConstant.RELATIONSHIP_WBS, ToOID = dobj.OID }).First();
                int RootOID = Convert.ToInt32(parent.RootOID);
                PmsProcess proc = PmsProcessRepository.SelPmsProcess(Context, new PmsProcess { OID = dobj.OID });

                // Auth
                if(proc.BPolicyAuths.FindIndex(item => item.Name != CommonConstant.AUTH_PROMOTE ) < 0)
                {
                    return "담당 Task가 아닙니다.";
                }

                //Gate Check
                List<PmsRelationship> lProjWbs = PmsRelationshipRepository.GetProjWbsLIst(Context, Convert.ToString(RootOID));
                int frontGateOid = -1;
                for (int index = 0, size = lProjWbs.Count; index < size; index++)
                {
                    if (lProjWbs[index].ObjType == PmsConstant.TYPE_GATE)
                    {
                        frontGateOid = Convert.ToInt32(lProjWbs[index].ToOID);
                    }
                    if (lProjWbs[index].ToOID == dobj.OID)
                    {
                        break;
                    }
                }

                if (frontGateOid > -1)
                {
                    DObject gateDobj =  DObjectRepository.SelDObject(Context, new DObject { OID = Convert.ToInt32(frontGateOid) });
                    if(gateDobj.BPolicy.Name != PmsConstant.POLICY_PROCESS_COMPLETED)
                    {
                        return gateDobj.Name + "  게이트가 완료가 되지 않았습니다.";
                    }
                    gateDobj = null;
                }

                //Parent Task
                DObject parentObj = DObjectRepository.SelDObject(Context, new DObject { OID = Convert.ToInt32(parent.FromOID) });
                if (parentObj.Type == PmsConstant.TYPE_TASK)
                {
                    if (parentObj.BPolicy.Name == PmsConstant.POLICY_PROCESS_PREPARE)
                    {
                        return  parentObj.Name + " 타스크가 진행되지 않아 실행할 수 없습니다.";
                    }
                }
                parentObj = null;

                //Dependency
                List<int> depency = new List<int>();
                if (proc.Dependency != null && proc.Dependency.Length > 0)
                {
                    proc.Dependency.Split(',').ToList().ForEach(dep =>
                    {
                        if (dep.IndexOf(":") > -1)
                        {
                            depency.Add(Convert.ToInt32(dep.Substring(0, dep.IndexOf(":"))));
                        }
                        else
                        {
                            depency.Add(Convert.ToInt32(dep));
                        }
                    });
                }

                if (depency.Count < 1)
                {
                    return "";
                }

                bool Complete = true;
                lProjWbs.ForEach(wbs =>
                {
                    if (depency.Contains(Convert.ToInt32(wbs.Id)))
                    {
                        if (wbs.ActEndDt == null)
                        {
                            Complete = false;
                        }
                    }
                });

                if (!Complete)
                {
                    return "의존 Task가 완료되지 않았습니다.";
                }

            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            return "";
        }

        public string ActionTaskStartedProjectPromote(object[] args)
        {
            object[] oArgs = args;
            HttpSessionStateBase Context = (HttpSessionStateBase)oArgs[0];
            int iUser = Convert.ToInt32(Context["UserOID"]);
            string type = Convert.ToString(oArgs[1]);
            string status = Convert.ToString(oArgs[2]);
            string oid = Convert.ToString(oArgs[3]);
            try
            {
                List<PmsRelationship> lParent = PmsRelationshipRepository.SelPmsRelationship(Context, new PmsRelationship { Type = PmsConstant.RELATIONSHIP_WBS, ToOID = Convert.ToInt32(oid) });
                if (lParent.Count > 0)
                {
                    PmsRelationship parent = lParent.First();
                    PmsProcess parentProc = PmsProcessRepository.SelPmsProcess(Context, new PmsProcess { OID = parent.FromOID });
                    if (parentProc.Type == PmsConstant.TYPE_PHASE)
                    {
                        string result = TriggerUtil.StatusObjectPromote(Context, false, parentProc.Type, Convert.ToString(parentProc.BPolicyOID), null, Convert.ToInt32(parentProc.OID), Convert.ToInt32(parentProc.OID), CommonConstant.ACTION_PROMOTE, "");
                        if (result != null && result.Length > 0)
                        {
                            return result;
                        }
                    }
                }
                lParent = null;

                PmsProcess tmpProc = PmsProcessRepository.SelPmsProcess(Context, new PmsProcess { OID = Convert.ToInt32(oid) });
                PmsProcessRepository.UdtPmsProcess(Context, new PmsProcess { OID = Convert.ToInt32(oid), ActStartDt = DateTime.Now, Dependency = tmpProc.Dependency });
                tmpProc = null;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            return "";
        }

        public string ActionTaskRelationshipPromote(object[] args)
        {
            object[] oArgs = args;
            HttpSessionStateBase Context = (HttpSessionStateBase)oArgs[0];
            int iUser = Convert.ToInt32(Context["UserOID"]);
            string type = Convert.ToString(oArgs[1]);
            string status = Convert.ToString(oArgs[2]);
            string oid = Convert.ToString(oArgs[3]);
            string action = Convert.ToString(oArgs[5]);
            try
            {
                if (action.Equals(CommonConstant.ACTION_PROMOTE))
                {
                    List<BPolicy> bPolicies = BPolicyRepository.SelBPolicy(new BPolicy { Type = DocumentConstant.TYPE_DOCUMENT });
                    int comIdx = bPolicies.FindIndex(x => x.Name == DocumentConstant.POLICY_DOCUMENT_COMPLETED);
                    List<PmsRelationship> lChildren = PmsRelationshipRepository.SelPmsRelationship(Context, new PmsRelationship { TaskOID = Convert.ToInt32(oid) });
                    DObject dobj = null;
                    lChildren.ForEach(child =>
                    {
                        if (dobj != null)
                        {
                            dobj = null;
                        }
                        dobj = DObjectRepository.SelDObject(Context, new DObject { OID = child.ToOID });
                        DObjectRepository.UdtDObject(Context, new DObject { OID = dobj.OID, BPolicyOID = Convert.ToInt32(bPolicies[comIdx].OID) });

                        //TriggerUtil.StatusObjectPromote(Context, false, dobj.Type, Convert.ToString(dobj.BPolicyOID), null, Convert.ToInt32(dobj.OID), Convert.ToInt32(dobj.OID), action, "");
                    });
                }
            }
            catch(Exception ex)
            {
                return ex.Message;
            }
            return "";
        }

    }
}