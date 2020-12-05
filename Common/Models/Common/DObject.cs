using Common.Constant;
using Common.Factory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Common.Models
{
    public class DObject
    {
        public int? OID { get; set; }

        public string Name { get; set; }

        public string Type { get; set; }

        public string Description { get; set; }

        public string TableNm { get; set; }

        public int? BPolicyOID { get; set; }

        public BPolicy BPolicy { get; set; }

        public List<BPolicyAuth> BPolicyAuths { get; set; }

        public string Revision { get; set; }

        public string TdmxOID { get; set; }

        public int? IsLatest { get; set; }

        public int? IsReleasedLatest { get; set; }

        public string Thumbnail { get; set; }

        public DateTime? CreateDt { get; set; }

        public int? CreateUs { get; set; }

        public string CreateUsNm { get; set; }

        public DateTime? ModifyDt { get; set; }

        public int? ModifyUs { get; set; }

        public string ModifyUsNm { get; set; }

        public DateTime? DeleteDt { get; set; }

        public int? DeleteUs { get; set; }
    }

    public static class DObjectRepository
    {

        public static string SelTdmxOID(DObject _param)
        {
            return DaoFactory.GetData<string>("Comm.SelMaxTDMXOID", new DObject { Type = _param.Type });
        }

        public static int SelNameSeq(DObject _param)
        {
            return DaoFactory.GetData<int>("Comm.SelNameSeq", _param);
        }

        public static int UdtLatestDObject(DObject _param)
        {
            int result = -1;
            _param.ModifyUs = 1;
            result = DaoFactory.SetUpdate("Comm.UdtLatestDObject", _param);
            return result;
        }

        public static int UdtReleaseLatestDObject(DObject _param)
        {
            int result = -1;
            _param.ModifyUs = 1;
            result = DaoFactory.SetUpdate("Comm.UdtReleasedLatestDObject", _param);
            return result;
        }

        public static DObject SelDObject(DObject _param)
        {
            DObject dObject = DaoFactory.GetData<DObject>("Comm.SelDObject", _param);
            if (dObject != null)
            {
                dObject.BPolicy = BPolicyRepository.SelBPolicy(new BPolicy { Type = dObject.Type, OID = dObject.BPolicyOID }).First();
            }
            return dObject;
        }

        public static List<DObject> SelDObjects(DObject _param)
        {
            List<DObject> lDObject = DaoFactory.GetList<DObject>("Comm.SelDObject", _param);
            lDObject.ForEach(dObj =>
            {
                dObj.BPolicy = BPolicyRepository.SelBPolicy(new BPolicy { Type = dObj.Type, OID = dObj.BPolicyOID }).First();
            });
            return lDObject;
        }

        public static int CopyDObject(DObject _param)
        {
            int result = -1;
            DObject targetDobj = DObjectRepository.SelDObject(new DObject { OID = _param.OID });
            targetDobj.BPolicyOID = null;
            targetDobj.Revision = null;
            if (_param.Name != null)
            {
                targetDobj.Name = _param.Name;
            }
            result = DObjectRepository.InsDObject(targetDobj);
            return result;
        }

        public static int ReviseDObject(DObject _param)
        {
            int result = -1;
            DObject targetDobj = DObjectRepository.SelDObject(new DObject { OID = _param.OID });
            DObjectRepository.UdtReleaseLatestDObject(new DObject { OID = _param.OID, IsReleasedLatest = 0 });
            targetDobj.BPolicyOID = null;
            targetDobj.Revision = SemsUtil.MakeMajorRevisonUp(targetDobj.Revision);
            result = DObjectRepository.InsDObject(targetDobj);
            return result;
        }

        public static int CloneDObject(DObject _param)
        {
            int result = -1;
            DObject targetDobj = DObjectRepository.SelDObject(new DObject { OID = _param.OID });
            result = DObjectRepository.InsDObject(targetDobj);
            return result;
        }

        public static int InsDObject(DObject _param)
        {
            int result = -1;
            _param.CreateUs = 1;
            if (_param.BPolicyOID == null)
            {
                _param.BPolicyOID = BPolicyRepository.SelBPolicy(new BPolicy { Type = _param.Type }).First().OID;
            }

            if (_param.Revision == null)
            {
                BPolicy tmpBPolicy = BPolicyRepository.SelBPolicy(new BPolicy { OID = _param.BPolicyOID }).First();
                if (tmpBPolicy.IsRevision != null && tmpBPolicy.IsRevision.Equals("Y"))
                {
                    _param.TdmxOID = DObjectRepository.SelTdmxOID(new DObject { Type = _param.Type });
                    _param.Revision = CommonConstant.REVISION_PREFIX + CommonConstant.INIT_REVISION;
                }
            }

            _param.IsLatest = 1;
            _param.IsReleasedLatest = 0;
            result = DaoFactory.SetInsert("Comm.InsDObject", _param);
            return result;
        }

        public static int UdtDObject(DObject _param)
        {
            int result = -1;
            _param.ModifyUs = 1;
            result = DaoFactory.SetUpdate("Comm.UdtDObject", _param);
            return result;
        }

        public static int DelDObject(DObject _param)
        {
            int result = -1;
            _param.DeleteUs = 1;
            result = DaoFactory.SetUpdate("Comm.DelDObject", _param);
            return result;
        }

        public static string StatusCheckPromote(string RelType, string CurrentStatus, int OID, int RootOID, string Action, string Comment)
        {
            string result = "";
            List<Dictionary<string,string>> checkProgram = BPolicyRepository.SelCheckProgram(new BPolicy { Type = RelType, OID = Convert.ToInt32(CurrentStatus) });
            if (checkProgram != null && checkProgram.Count > 0)
            {
                checkProgram.ForEach(item =>
                {
                    if (result == null || result.Length < 1)
                    {
                        string returnMessage = SemsUtil.Invoke(item[CommonConstant.POLICY_TRIGGER_CLASS], item[CommonConstant.POLICY_TRIGGER_FUNCTION], new string[] { RelType, CurrentStatus, Convert.ToString(OID), Convert.ToString(RootOID), Action, Comment });
                        if (returnMessage != null && returnMessage.Length > 0)
                        {
                            result = returnMessage;
                        }
                    }
                });
            }
            return result;
        }

        public static string StatusPromote(bool Transaction, string RelType, string CurrentStatus, int OID, int RootOID, string Action, string Comment)
        {
            string result = "";
            try
            {
                if (Transaction)
                {
                    DaoFactory.BeginTransaction();
                }
                string checkProgram = StatusCheckPromote(RelType, CurrentStatus, OID, RootOID, Action, Comment);
                if (checkProgram != null && checkProgram.Length > 0)
                {
                    throw new Exception(checkProgram);
                }

                List<Dictionary<string, string>> actionProgram = BPolicyRepository.SelActionProgram(new BPolicy { Type = RelType, OID = Convert.ToInt32(CurrentStatus) });
                if (actionProgram != null && actionProgram.Count > 0)
                {
                    actionProgram.ForEach(item =>
                    {
                        string returnMessage = SemsUtil.Invoke(item[CommonConstant.POLICY_TRIGGER_CLASS], item[CommonConstant.POLICY_TRIGGER_FUNCTION], new string[] { RelType, CurrentStatus, Convert.ToString(OID), Convert.ToString(RootOID), Action, Comment });
                        if (returnMessage != null && returnMessage.Length > 0)
                        {
                            throw new Exception(returnMessage);
                        }
                    });
                }
                
                string strNextAction = BPolicyRepository.SelBPolicy(new BPolicy { Type = RelType, OID = Convert.ToInt32(CurrentStatus) }).First().NextActionOID;
                string strActionOID = "";
                if (strNextAction != null)
                {
                    strNextAction.Split(',').ToList().ForEach(action =>
                    {
                        if(action.IndexOf(Action) > -1)
                        {
                            strActionOID = action.Substring(action.IndexOf(":") + 1);
                            return;
                        }
                    });
                }
                DObjectRepository.UdtDObject(new DObject { OID = OID, BPolicyOID = Convert.ToInt32(strActionOID) });
                if (Transaction)
                {
                    DaoFactory.Commit();
                }
            }
            catch(Exception ex)
            {
                if (Transaction)
                {
                    DaoFactory.Rollback();
                }
                result = ex.Message;
            }
            return result;
        }

    }
}
