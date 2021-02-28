using Common.Constant;
using Common.Models;
using Common.Utils;
using DocumentClassification.Models;
using Pms.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Pms.Trigger
{
    public class ProjectTrigger
    {

        public string CheckStartedProjectPromote(object[] args)
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
                if (action.Equals(CommonConstant.ACTION_REJECT))
                {
                    return "";
                }
                List<PmsRelationship> lProjWbs = PmsRelationshipRepository.GetProjWbsLIst(Context, oid);
                if(lProjWbs.FindAll(wbs => { return wbs.ObjType == PmsConstant.TYPE_TASK && wbs.Members.Count < 1; }).Count > 0)
                {
                    return "Member를 할당해 주세요.";
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            return "";
        }

        public string ActionStartedProjectPromote(object[] args)
        {
            object[] oArgs = args;
            HttpSessionStateBase Context = (HttpSessionStateBase)oArgs[0];
            int iUser = Convert.ToInt32(Context["UserOID"]);
            string type = Convert.ToString(oArgs[1]);
            string status = Convert.ToString(oArgs[2]);
            string oid = Convert.ToString(oArgs[3]);
            try
            {
                DObjectRepository.UdtDObject(Context, new DObject { OID = Convert.ToInt32(oid) });
                PmsProjectRepository.UdtPmsProject(Context, new PmsProject { OID = Convert.ToInt32(oid), ActStartDt = DateTime.Now });
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            return "";
        }

        public string ActionStartetdProjectMailPromote(object[] args)
        {
            object[] oArgs = args;
            HttpSessionStateBase Context = (HttpSessionStateBase)oArgs[0];
            int iUser = Convert.ToInt32(Context["UserOID"]);
            string type = Convert.ToString(oArgs[1]);
            string status = Convert.ToString(oArgs[2]);
            string oid = Convert.ToString(oArgs[3]);
            try
            {
                PmsProject tmpProj = PmsProjectRepository.SelPmsObject(Context, new PmsProject { OID = Convert.ToInt32(oid) });
                SemsSmtp smtp = new SemsSmtp();
                PmsRelationshipRepository.SelPmsRelationship(Context, new PmsRelationship { Type = PmsConstant.RELATIONSHIP_MEMBER, FromOID = Convert.ToInt32(oid) }).ForEach(member =>
                {
                    Person tmpPerson = PersonRepository.SelPerson(Context, new Person { OID = member.ToOID });
                    ProjApprovMailContent projMail = new ProjApprovMailContent(Context, tmpProj, tmpPerson);
                    smtp.SetMailInfo(projMail);
                });
                smtp.SendMail();
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            return "";
        }

        public string InsReliabilityReport(object[] args)
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
                    DObject dobj = new DObject();
                    int? resultOID = 0;

                    PmsReliability PmsReliability = PmsReliabilityRepository.SelPmsReliabilityObject(Context, new PmsReliability { OID = Convert.ToInt32(oid) });
                    List<PmsRelationship> Root = PmsRelationshipRepository.SelPmsRelationship(Context, new PmsRelationship { ToOID = PmsReliability.OID, Type = PmsConstant.RELATIONSHIP_DOC_CLASS });
                    if(Root.Count < 0)
                    {
                        return "";
                    }

                    List<TestItemList> PmsReliabilityTestItemList = PmsReliabilityRepository.SelPmsReliabilityItemList(Context, new TestItemList { FromOID = PmsReliability.OID });

                    var YYYY = DateTime.Now.ToString("yyyy");
                    var MM = DateTime.Now.ToString("MM");
                    var dd = DateTime.Now.ToString("dd");

                    dobj.Type = PmsConstant.TYPE_RELIABILITY_REPORT;
                    dobj.TableNm = PmsConstant.TABLE_RELIABILITY_REPORT;

                    var selName = "DRR" + YYYY + MM + dd + "-001";
                    var NewName = "DRR" + YYYY + MM + dd;

                    var LateName = PmsReliabilityReportRepository.SelPmsReliabilityReport(Context, new PmsReliabilityReport { Name = NewName });

                    if (LateName.Count == 0)
                    {
                        dobj.Name = selName;
                    }
                    else
                    {
                        int NUM = Convert.ToInt32(LateName.Last().Name.Substring(12, 3)) + 1;
                        dobj.Name = NewName + "-" + string.Format("{0:D3}", NUM);
                    }

                    resultOID = DObjectRepository.InsDObject(Context, dobj);

                    PmsReliabilityReport _param = new PmsReliabilityReport();

                    _param.OID = resultOID;
                    _param.FromOID = Convert.ToInt32(oid);
                    _param.RootOID = Root[0].RootOID;
                    _param.DevStep = PmsReliability.DevStep;
                    _param.TotalTestStartDt = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                    _param.TotalTestEndDt = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                    PmsReliabilityReportRepository.InsPmsReliabilityReport(Context, _param);


                    List<ReportTestItemList> _ItemListParam = new List<ReportTestItemList>();
                    PmsReliabilityTestItemList.ForEach(obj =>
                    {
                        ReportTestItemList ReportTestItem = new ReportTestItemList();
                        ReportTestItem.TestItemNm = obj.TestItemNm;
                        _ItemListParam.Add(ReportTestItem);
                    });

                    PmsReliabilityReportRepository.InsPmsReliabilityReportItemList(Context, _ItemListParam, resultOID);
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            return "";
        }

        public string CheckCompleteProjectPromote(object[] args)
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
                BPolicy contentBpolicy = null;
                if (goStatus != "")
                {
                    contentBpolicy = BPolicyRepository.SelBPolicy(new BPolicy { OID = Convert.ToInt32(goStatus) }).First();
                }

                if ((contentBpolicy != null && contentBpolicy.Name == PmsConstant.POLICY_PROCESS_COMPLETED) || action.Equals(CommonConstant.ACTION_PROMOTE))
                {
                    List<PmsRelationship> lProjWbs = PmsRelationshipRepository.GetProjWbsLIst(Context, oid);
                    if (lProjWbs.FindAll(wbs => wbs.ObjStNm != PmsConstant.POLICY_PROCESS_COMPLETED).Count > 0)
                    {
                        return "완료되지 않은 항목이 존재합니다.";
                    }
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            return "";
        }

        public string ActionCompleteProjectPromote(object[] args)
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
                BPolicy contentBpolicy = null;
                if (goStatus != "")
                {
                    contentBpolicy = BPolicyRepository.SelBPolicy(new BPolicy { OID = Convert.ToInt32(goStatus) }).First();
                }

                if ((contentBpolicy != null && contentBpolicy.Name == PmsConstant.POLICY_PROCESS_COMPLETED) || action.Equals(CommonConstant.ACTION_PROMOTE))
                {
                    List<PmsRelationship> lProjWbs = PmsRelationshipRepository.GetProjWbsLIst(Context, oid);
                    if (lProjWbs.FindAll(wbs => wbs.ObjStNm != PmsConstant.POLICY_PROCESS_COMPLETED).Count > 0)
                    {
                        return "완료되지 않은 항목이 존재합니다.";
                    }
                    PmsProject proj = PmsProjectRepository.SelPmsObject(Context, new PmsProject { OID = Convert.ToInt32(oid) });
                    List<DateTime> lHoliday = CalendarDetailRepository.SelCalendarDetails(new CalendarDetail { CalendarOID = proj.CalendarOID, IsHoliday = 1 }).Select(val => DateTime.Parse(val.Year + "-" + val.Month + "-" + val.Day)).ToList();
                    PmsProjectRepository.UdtPmsProject(Context,
                    new PmsProject
                    {
                        OID = proj.OID,
                        ActEndDt = DateTime.Now,
                        ActDuration = PmsUtils.CalculateGapFutureDuration(Convert.ToDateTime(Convert.ToDateTime(proj.ActStartDt).ToString("yyyy-MM-dd")), Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd")), Convert.ToInt32(proj.WorkingDay), lHoliday),
                        Complete = 100,
                    });
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            return "";
        }

        public string ActionAutoCompleteProjectPromote(object[] args)
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
                DObject tmpDobj = DObjectRepository.SelDObject(Context, new DObject { OID = Convert.ToInt32(oid) });
                string strNextAction = BPolicyRepository.SelBPolicy(new BPolicy { Type = tmpDobj.Type, OID = tmpDobj.BPolicyOID }).First().NextActionOID;
                List<PmsRelationship> lWbs = PmsRelationshipRepository.GetProjWbsTypeOidList(Context, oid).FindAll(wbs => wbs.ObjStNm != null && !wbs.ObjStNm.Equals(PmsConstant.POLICY_PROJECT_COMPLETED));
                if (lWbs.Count < 1)
                {
                    string strActionOID = "";
                    if (strNextAction != null && strNextAction.Length > 0)
                    {
                        strNextAction.Split(',').ToList().ForEach(item =>
                        {
                            if (item.IndexOf(CommonConstant.ACTION_NON_AUTO) > -1)
                            {
                                strActionOID = item.Substring(item.IndexOf(":") + 1);
                                return;
                            }
                        });
                        DObjectRepository.UdtDObject(Context, new DObject { OID = Convert.ToInt32(oid), BPolicyOID = Convert.ToInt32(strActionOID) });
                        PmsProject proj = PmsProjectRepository.SelPmsObject(Context, new PmsProject { OID = Convert.ToInt32(oid) });
                        List<DateTime> lHoliday = CalendarDetailRepository.SelCalendarDetails(new CalendarDetail { CalendarOID = proj.CalendarOID, IsHoliday = 1 }).Select(val => DateTime.Parse(val.Year + "-" + val.Month + "-" + val.Day)).ToList();
                        PmsProjectRepository.UdtPmsProject(Context,
                        new PmsProject
                        {
                            OID = proj.OID,
                            ActEndDt = DateTime.Now,
                            ActDuration = PmsUtils.CalculateGapFutureDuration(Convert.ToDateTime(Convert.ToDateTime(proj.ActStartDt).ToString("yyyy-MM-dd")), Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd")), Convert.ToInt32(proj.WorkingDay), lHoliday),
                            Complete = 100,
                        });
                    }
                };
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            return "";
        }


    }
}
