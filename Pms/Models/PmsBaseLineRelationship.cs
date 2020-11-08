using Common.Constant;
using Common.Factory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pms.Models
{
    public class PmsBaseLineRelationship : PmsRelationship
    {
        public int? RelBaseLineOID { get; set; }
        public int? RootBaseLineOID { get; set; }
        public string ProjectBaseLineNm { get; set; }
        public PmsRelationship BaseData { get; set; }
        public List<PmsBaseLineRelationship> BaseLineChildren { get; set; }
        public List<PmsBaseLineRelationship> BaseLIneMembers { get; set; }
    }

    public static class PmsBaseLineRelationshipRepository
    {
        public static List<PmsBaseLineRelationship> SelPmsBaseLineRelationship(PmsBaseLineRelationship _param)
        {
            return DaoFactory.GetList<PmsBaseLineRelationship>("Pms.SelPmsBaseLineRelationship", _param);
        }

        public static int InsPmsBaseLineRelationship(PmsBaseLineRelationship _param)
        {
            _param.CreateUs = 1;
            return DaoFactory.SetInsert("Pms.InsPmsBaseLineRelationship", _param);
        }

        public static PmsBaseLineRelationship getListBaseLineWbsStructure(int _level, int _FromOID, int _RootBaseLineOID, PmsBaseLineProject _proj)
        {
            PmsBaseLineRelationship getStructure = new PmsBaseLineRelationship();
            getStructure.Level = _level;
            getStructure.RootBaseLineOID = _RootBaseLineOID;
            getStructure.ToOID = _FromOID;
            getStructure.ObjName = _proj.ProjectNm;
            getStructure.ProjectBaseLineNm = _proj.Name;
            getStructure.ObjType = PmsConstant.TYPE_PROJECT;
            getStructure.EstDuration = _proj.EstDuration;
            getStructure.EstStartDt = _proj.EstStartDt;
            getStructure.EstEndDt = _proj.EstEndDt;
            getStructure.ActDuration = _proj.ActDuration;
            getStructure.ActStartDt = _proj.ActStartDt;
            getStructure.ActEndDt = _proj.ActEndDt;
            getStructure.Id = null;
            getStructure.WorkingDay = _proj.WorkingDay;
            getBaseLineWbsStructure(getStructure, _FromOID, _RootBaseLineOID, Convert.ToInt32(_proj.WorkingDay));
            return getStructure;
        }

        public static void getBaseLineWbsStructure(PmsBaseLineRelationship _relObj, int _projOID, int _RootBaseLineOID, int _workingDay)
        {
            _relObj.RootBaseLineOID = _RootBaseLineOID;
            _relObj.WorkingDay = _workingDay;
            _relObj.BaseLineChildren = PmsBaseLineRelationshipRepository.SelPmsBaseLineRelationship(new PmsBaseLineRelationship { FromOID = _relObj.ToOID, Type = PmsConstant.RELATIONSHIP_WBS, RootBaseLineOID = _relObj.RootBaseLineOID });
            _relObj.BaseLineChildren.ForEach(item =>
            {
                item.Level = _relObj.Level + 1;
                PmsBaseLineProcess ToDetail = PmsBaseLineProcessRepository.SelPmsBaseLIneProcess(new PmsBaseLineProcess { ProcessOID = item.ToOID, RootBaseLineOID = _RootBaseLineOID });
                item.ObjName = ToDetail.ProcessNm;
                item.ObjType = ToDetail.ProcessType;
                item.EstDuration = ToDetail.EstDuration;
                item.EstStartDt = ToDetail.EstStartDt;
                item.EstEndDt = ToDetail.EstEndDt;
                item.ActDuration = ToDetail.ActDuration;
                item.ActStartDt = ToDetail.ActStartDt;
                item.ActEndDt = ToDetail.ActEndDt;
                item.Id = ToDetail.Id;
                item.Dependency = ToDetail.Dependency;
                getBaseLineWbsStructure(item, _projOID, _RootBaseLineOID, _workingDay);
            });
        }


    }
}
