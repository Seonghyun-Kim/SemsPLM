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

        // 차종 
        public int? CarLibOID { get; set; }

        // 제품 
        public int? ProductOID { get; set; }

        // 프로젝트 
        public int? ProjectOID { get; set; }

        // 프로젝트 단계 
        public int? ProcessOID { get; set; }
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
            _param.CreateUs = 1;
            return DaoFactory.SetInsert("Qms.InsOpenIssue", _param);
        }

        public static int UdtOpenIssue(OpenIssue _param)
        {
            _param.ModifyUs = 1;
            return DaoFactory.SetUpdate("Qms.UdtOpenIssue", _param);
        }
    }

    public class OpenIssueItem : DObject, IDObject
    {
        // 재발여부 사내 
        public string RelapseInsideFl { get; set; }

        // 재발여부 한온 
        public string RelapseHanonFl { get; set; }

        // 재발여부 자동차 
        public string RelapseCarFl { get; set; }

        // Open Issue 내용 
        public string OpenIssueDetailDesc { get; set; }

        // 발생일 
        public DateTime? OpenIssueOccurrenceDt { get; set; }

        // 계획일 
        public DateTime? OpenIssueExpectedDt { get; set; }

        // 완료일 
        public DateTime? OpenIssueCompleteDt { get; set; }

        // CLOSE 결과 
        public string OpenIssueCloseFl { get; set; }
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
            _param.CreateUs = 1;
            return DaoFactory.SetInsert("Qms.InsOpenIssueItem", _param);
        }

        public static int UdtOpenIssueItem(OpenIssueItem _param)
        {
            _param.ModifyUs = 1;
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
            _param.CreateUs = 1;
            return DaoFactory.SetInsert("Qms.InsOpenIssueRelationship", _param);
        }

        public static int DelOpenIssueRelationship(OpenIssueRelationship _param)
        {
            return DaoFactory.SetDelete("Qms.DelOpenIssueRelationship", _param);
        }
    }
}
