using Common.Factory;
using Common.Interface;
using Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qms.Models
{
    public class OpenIssue : DObject, IDObject
    {
        // 고객사 
        public int? CustomerLibOID { get; set; }

        public string CustomerLibNm { get; set; }

        public int? OemLibOID { get; set; }

        public string OemLibNm { get; set; }

        // 차종 
        public int? CarLibOID { get; set; }

        public string CarLibNm { get; set; }

        // 프로젝트 
        public int? ProjectOID { get; set; }

        public string ProjectNm { get; set; }

        public List<OpenIssueItem> OpenIssueItems { get; set; }

        public int CompleatedCnt { get; set; }

        public int UncommittedCnt { get; set; }

        public int SuspenseCnt { get; set; }
        // 제품 
        //public int? ProductOID { get; set; }

        // 프로젝트 단계 
        //public int? ProcessOID { get; set; }
    }

    public static class OpenIssueRepository
    {
        public static OpenIssue SelOpenIssue(OpenIssue _param)
        {
            return DaoFactory.GetData<OpenIssue>("Qms.SelOpenIssue", _param);
        }

        public static List<OpenIssue> SelOpenIssues(OpenIssue _param)
        {
            return DaoFactory.GetList<OpenIssue>("Qms.SelOpenIssue", _param);
        }

        public static int InsOpenIssue(OpenIssue _param)
        {
            return DaoFactory.SetInsert("Qms.InsOpenIssue", _param);
        }

        public static int UdtOpenIssueSuspenseCnt(OpenIssue _param)
        {
            return DaoFactory.SetUpdate("Qms.UdtOpenIssueSuspenseCnt", _param);
        }

        public static int UdtOpenIssueDelSuspenseCnt(OpenIssue _param)
        {
            return DaoFactory.SetUpdate("Qms.UdtOpenIssueDelSuspenseCnt", _param);
        }
    }

    public class OpenIssueItem : DObject, IDObject
    {
        public int? OpenIssueOID { get; set; }

        public string StatusNm { get; set; }

        public string OpenIssueTitle { get; set; }

        // 재발여부 사내 
        public string RelapseInsideFl { get; set; }

        // 재발여부 한온 
        public string RelapseHanonFl { get; set; }

        // 재발여부 자동차 
        public string RelapseCarFl { get; set; }

        // Open Issue 내용 
        public string OpenIssueDetailDesc { get; set; }

        // 발생일 
        public string OpenIssueOccurrenceDt { get; set; }

        // 계획일 
        public string OpenIssueExpectedDt { get; set; }

        // 완료일 
        public string OpenIssueCompleteDt { get; set; }

        // CLOSE 결과 
        public string OpenIssueCloseFl { get; set; }

        public string IsDel { get; set; }
    }

    public static class OpenIssueItemRepository
    {
        public static OpenIssueItem SelOpenIssueItem(OpenIssueItem _param)
        {
            return DaoFactory.GetData<OpenIssueItem>("Qms.SelOpenIssueItem", _param);
        }

        public static List<OpenIssueItem> SelOpenIssueItems(OpenIssueItem _param)
        {
            return DaoFactory.GetList<OpenIssueItem>("Qms.SelOpenIssueItem", _param);
        }

        public static int InsOpenIssueItem(OpenIssueItem _param)
        {
            return DaoFactory.SetInsert("Qms.InsOpenIssueItem", _param);
        }

        public static int UdtOpenIssueItem(OpenIssueItem _param)
        {
            return DaoFactory.SetUpdate("Qms.UdtOpenIssueItem", _param);
        }
    }

    public class OpenIssueRelationship
    {
        // Open Issue OID 
        public int? FromOID { get; set; }

        // Open Issue Item OR 신속대응 
        public int? ToOID { get; set; }

        // 연결 TYPE 
        public string type { get; set; }

        // 작성일 
        public DateTime? CreateDt { get; set; }

        // 작성자 
        public int? CreateUs { get; set; }
    }

    public static class OpenIssueRelationshipRepository
    {
        public static OpenIssueRelationship SelOpenIssueRelationship(OpenIssueRelationship _param)
        {
            return DaoFactory.GetData<OpenIssueRelationship>("Qms.SelOpenIssueRelationship", _param);
        }

        public static List<OpenIssueRelationship> SelOpenIssueRelationships(OpenIssueRelationship _param)
        {
            return DaoFactory.GetList<OpenIssueRelationship>("Qms.SelOpenIssueRelationship", _param);
        }

        public static int InsOpenIssueRelationship(OpenIssueRelationship _param)
        {
            return DaoFactory.SetInsert("Qms.InsOpenIssueRelationship", _param);
        }

        public static int DelOpenIssueRelationship(OpenIssueRelationship _param)
        {
            return DaoFactory.SetDelete("Qms.DelOpenIssueRelationship", _param);
        }
    }
}
