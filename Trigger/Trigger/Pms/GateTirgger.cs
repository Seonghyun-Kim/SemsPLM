using Common.Constant;
using Common.Models;
using Pms.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Pms.Trigger
{
    public class GateTirgger
    {
        public string CheckGateStartedProcessPromote(object[] args)
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
                List<PmsRelationship> lProjWbs = PmsRelationshipRepository.GetProjWbsLIst(Context, Convert.ToString(RootOID));

                List<PmsRelationship> gettingWbs = new List<PmsRelationship>();
                bool bContinue = true;
                lProjWbs.ForEach(wbs =>
                {
                    if (wbs.ObjType != PmsConstant.TYPE_PROJECT && bContinue)
                    {                        
                        if (wbs.ToOID == dobj.OID)
                        {
                            bContinue = false;
                            return;
                        }
                        gettingWbs.Add(wbs);
                    }
                });

                if (gettingWbs.FindAll(wbs => wbs.ObjStNm != PmsConstant.POLICY_PROCESS_COMPLETED).Count > 0)
                {
                    return "완료되지 않은 항목이 존재합니다.";
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            return "";
        }

        public string ActionGateStartedProcessPromote(object[] args)
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
                DObject dobj = DObjectRepository.SelDObject(Context, new DObject { OID = Convert.ToInt32(oid) });
                if (dobj.BPolicy.Name == PmsConstant.POLICY_PROCESS_COMPLETED && action.Equals(CommonConstant.ACTION_PROMOTE))
                {
                    int RootOID = Convert.ToInt32(PmsRelationshipRepository.SelPmsRelationship(Context, new PmsRelationship { Type = PmsConstant.RELATIONSHIP_WBS, ToOID = dobj.OID }).First().RootOID);
                    PmsProject proj = PmsProjectRepository.SelPmsObject(Context, new PmsProject { OID = RootOID });
                    List<DateTime> lHoliday = CalendarDetailRepository.SelCalendarDetails(new CalendarDetail { CalendarOID = proj.CalendarOID, IsHoliday = 1 }).Select(val => DateTime.Parse(val.Year + "-" + val.Month + "-" + val.Day)).ToList();
                    PmsProcess tmpProcess = PmsProcessRepository.SelPmsProcess(Context, new PmsProcess { OID = dobj.OID });
                    PmsProcessRepository.UdtPmsProcess(Context,
                    new PmsProcess
                    {
                        OID = dobj.OID,
                        ActEndDt = DateTime.Now,
                        ActDuration = PmsUtils.CalculateGapFutureDuration(Convert.ToDateTime(Convert.ToDateTime(tmpProcess.ActStartDt).ToString("yyyy-MM-dd")), Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd")), Convert.ToInt32(proj.WorkingDay), lHoliday),
                        Complete = 100,
                        Dependency = tmpProcess.Dependency
                    });
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            return "";
        }
    }
}
