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
    public class PmsReliabilityReport : DObject, IDObject
    {
        public int? FromOID { get; set; }
        public int? RootOID { get; set; }
        public int? TaskOID { get; set; }
        public int? DevStep { get; set; }

        public int TotalTestItem { get; set; }
        public int WaitingNum { get; set; }
        public int ProgressNum { get; set; }
        public int CompleteNum { get; set; }
        public int NGNum { get; set; }
        public string PartNm { get; set; }
        public string TotalTestDt
        {
            get
            {
                string returnVal = "";
                returnVal = DateTime.Parse(this.TotalTestStartDt.ToString()).ToShortDateString() + "~" + DateTime.Parse(this.TotalTestEndDt.ToString()).ToShortDateString();

                return returnVal;
            }
        }
        public DateTime? TotalTestStartDt { get; set; }
        public DateTime? TotalTestEndDt { get; set; }
        public string TestPurpose { get; set; }
        public string DevStepNm { get; set; }
        public string ReliabilityNm { get; set; }
    }

    public class ReportTestItemList : DObject, IDObject
    {
        public int? FromOID { get; set; }
        public string TestItemNm { get; set; }
        public DateTime? EstStartDt { get; set; }
        public DateTime? EstEndDt { get; set; }
        public DateTime? ActStartDt { get; set; }
        public DateTime? ActEndDt { get; set; }
        public int? ProgressResult { get; set; }
        public string ProgressResultNm { get; set; }
        public string ETC { get; set; }
    }

    public static class PmsReliabilityReportRepository
    {
        public static int InsPmsReliabilityReport(HttpSessionStateBase Context, PmsReliabilityReport _param)
        {
            return DaoFactory.SetInsert("Pms.InsPmsReliabilityReport", _param);
        }

        public static List<PmsReliabilityReport> SelPmsReliabilityReport(HttpSessionStateBase Context, PmsReliabilityReport _param)
        {
            _param.Type = Common.Constant.PmsConstant.TYPE_RELIABILITY_REPORT;
            List<PmsReliabilityReport> PmsReliabilityReport = DaoFactory.GetList<PmsReliabilityReport>("Pms.SelPmsReliabilityReport", _param);
            List<int> iPersonOIDs = PmsReliabilityReport.Select(data => Convert.ToInt32(data.CreateUs)).ToList();
            if (iPersonOIDs == null || iPersonOIDs.Count < 1)
            {
                return PmsReliabilityReport;
            }
            List<BPolicy> lBPolicy = BPolicyRepository.SelBPolicyOIDs(new BPolicy { OIDs = PmsReliabilityReport.Select(sel => Convert.ToInt32(sel.BPolicyOID)).ToList() });
            List<Person> lPerson = PersonRepository.SelPersons(Context, new Person { OIDs = iPersonOIDs });
            List<PmsReliability> lPmsReliabilityRepository = PmsReliabilityRepository.SelPmsReliability(Context, new PmsReliability { OIDs = PmsReliabilityReport.Select(sel => Convert.ToInt32(sel.FromOID)).ToList() });
            PmsReliabilityReport.ForEach(obj =>
            {
                //obj.BPolicy = BPolicyRepository.SelBPolicy(new BPolicy { Type = obj.Type, OID = obj.BPolicyOID }).First();
                //obj.CreateUsNm = PersonRepository.SelPerson(Context, new Person { OID = obj.CreateUs }).Name;
                //obj.ReliabilityNm = PmsReliabilityRepository.SelPmsReliabilityObject(Context,new PmsReliability { OID = obj.FromOID }).Name;

                obj.BPolicy = lBPolicy.Find(bpolicy => bpolicy.OID == obj.BPolicyOID);
                obj.CreateUsNm = lPerson.Find(person => person.OID == obj.CreateUs).Name;
                obj.ReliabilityNm = lPmsReliabilityRepository.Find(data => data.OID == obj.FromOID).Name;
                obj.DevStepNm = LibraryRepository.SelLibraryObject(new Library { OID = obj.DevStep }).KorNm;
                obj.BPolicyAuths = BPolicyAuthRepository.MainAuth(Context, obj, null);
            });
            return PmsReliabilityReport;
        }

        public static PmsReliabilityReport SelPmsReliabilityReportObject(HttpSessionStateBase Context, PmsReliabilityReport _param)
        {
            _param.Type = Common.Constant.PmsConstant.TYPE_RELIABILITY_REPORT;
            PmsReliabilityReport PmsReliabilityReport = DaoFactory.GetData<PmsReliabilityReport>("Pms.SelPmsReliabilityReport", _param);
            PmsReliabilityReport.CreateUsNm = PersonRepository.SelPerson(Context, new Person { OID = PmsReliabilityReport.CreateUs }).Name;
            PmsReliabilityReport.BPolicy = BPolicyRepository.SelBPolicy(new BPolicy { Type = PmsReliabilityReport.Type, OID = PmsReliabilityReport.BPolicyOID }).First();
            PmsReliabilityReport.BPolicyAuths = BPolicyAuthRepository.MainAuth(Context, PmsReliabilityReport, PmsAuth.RoleAuth(Context, PmsReliabilityReport));
            PmsReliabilityReport.DevStepNm = LibraryRepository.SelLibraryObject(new Library { OID = PmsReliabilityReport.DevStep }).KorNm;
            return PmsReliabilityReport;
        }

        public static List<ReportTestItemList> InsPmsReliabilityReportItemList(HttpSessionStateBase Context, List<ReportTestItemList> _param, int? FromOID)
        {
            _param.ForEach(obj =>
            {
                obj.FromOID = FromOID;
                DaoFactory.SetInsert("Pms.InsPmsReliabilityReportItemList", obj);
            });

            return _param;
        }

        public static List<ReportTestItemList> SelPmsReliabilityReportItemList(HttpSessionStateBase Context, ReportTestItemList _param)
        {
            List<ReportTestItemList> SelPmsReliabilityReportItemList = DaoFactory.GetList<ReportTestItemList>("Pms.SelPmsReliabilityReportItemList", _param);
            SelPmsReliabilityReportItemList.ForEach(obj =>
            {
                if (obj.ProgressResult != null)
                {
                    obj.ProgressResultNm = LibraryRepository.SelLibraryObject(new Library { OID = obj.ProgressResult }).KorNm;
                }
            });
            return SelPmsReliabilityReportItemList;
        }

        public static int UdtPmsReliabilityReport(HttpSessionStateBase Context, PmsReliabilityReport _param)
        {
            return DaoFactory.SetUpdate("Pms.UdtPmsReliabilityReport", _param);
        }
    }
}
