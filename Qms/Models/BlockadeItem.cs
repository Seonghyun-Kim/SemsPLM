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
    public class BlockadeItem : DObject, IDObject
    {        
        // 모듈 OID (일정수립) 
        public int? ModuleOID { get; set; }

        // 대상범위 
        public string TargetScope { get; set; }

        // 조치방법 
        public string Act { get; set; }

        public string ActName { get; set; }

        // 대상수량 
        public int? TargetCnt { get; set; }

        // 조치부서 
        public int? ActDepartmentOID { get; set; }
        public string ActDepartmentNm { get; set; }

        // 조치담당자 
        public int? ActUserOID { get; set; }

        public string ActUserNm { get; set; }

        // 기한-시작일 
        public DateTime? ActStartDt { get; set; }

        // 기한-종료일 
        public DateTime? ActEndDt { get; set; }

        // 선별-적합 
        public int? SortSuitableCnt { get; set; }

        // 선별-부적합 
        public int? SortIncongruityCnt { get; set; }

        // 재작업 
        public int? ReworkCnt { get; set; }

        // 폐기 
        public int? DisuseCnt { get; set; }

        // 특채 
        public int? SpecialCnt { get; set; }

        // 기타 
        public int? EtcCnt { get; set; }

    }

    public static class BlockadeItemRepository
    {
        public static BlockadeItem SelBlockadeItem(BlockadeItem _param)
        {
            return DaoFactory.GetData<BlockadeItem>("Qms.SelBlockadeItem", _param);
        }

        public static List<BlockadeItem> SelBlockadeItems(BlockadeItem _param)
        {
            return DaoFactory.GetList<BlockadeItem>("Qms.SelBlockadeItem", _param);
        }

        public static int InsBlockadeItem(BlockadeItem _param)
        {
            return DaoFactory.SetInsert("Qms.InsBlockadeItem", _param);
        }

        public static int UdtBlockadeItem(BlockadeItem _param)
        {
            return DaoFactory.SetUpdate("Qms.UdtBlockadeItem", _param);
        }
    }
}
