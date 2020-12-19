using Common.Constant;
using Common.Factory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;

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

        public string BPolicyNm { get; set; }

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

        public static string SelTdmxOID(HttpSessionStateBase Context, DObject _param)
        {
            return DaoFactory.GetData<string>("Comm.SelMaxTDMXOID", new DObject { Type = _param.Type });
        }

        public static int SelNameSeq(HttpSessionStateBase Context, DObject _param)
        {
            return DaoFactory.GetData<int>("Comm.SelNameSeq", _param);
        }

        public static int UdtLatestDObject(HttpSessionStateBase Context, DObject _param)
        {
            int result = -1;
            _param.ModifyUs = Convert.ToInt32(Context["UserOID"]);
            result = DaoFactory.SetUpdate("Comm.UdtLatestDObject", _param);
            return result;
        }

        public static int UdtReleaseLatestDObject(HttpSessionStateBase Context, DObject _param)
        {
            int result = -1;
            _param.ModifyUs = Convert.ToInt32(Context["UserOID"]);
            result = DaoFactory.SetUpdate("Comm.UdtReleasedLatestDObject", _param);
            return result;
        }

        public static DObject SelDObject(HttpSessionStateBase Context, DObject _param)
        {
            DObject dObject = DaoFactory.GetData<DObject>("Comm.SelDObject", _param);
            if (dObject != null)
            {
                dObject.BPolicy = BPolicyRepository.SelBPolicy(new BPolicy { Type = dObject.Type, OID = dObject.BPolicyOID }).First();
                dObject.BPolicyAuths = BPolicyAuthRepository.MainAuth(Context, dObject);
            }
            return dObject;
        }

        public static List<DObject> SelDObjects(HttpSessionStateBase Context, DObject _param)
        {
            List<DObject> lDObject = DaoFactory.GetList<DObject>("Comm.SelDObject", _param);
            lDObject.ForEach(dObj =>
            {
                dObj.BPolicy = BPolicyRepository.SelBPolicy(new BPolicy { Type = dObj.Type, OID = dObj.BPolicyOID }).First();
                dObj.BPolicyAuths = BPolicyAuthRepository.MainAuth(Context, dObj);
            });
            return lDObject;
        }

        public static int CopyDObject(HttpSessionStateBase Context, DObject _param)
        {
            int result = -1;
            DObject targetDobj = DObjectRepository.SelDObject(Context, new DObject { OID = _param.OID });
            targetDobj.BPolicyOID = null;
            targetDobj.Revision = null;
            if (_param.Name != null)
            {
                targetDobj.Name = _param.Name;
            }
            result = DObjectRepository.InsDObject(Context, targetDobj);
            return result;
        }

        public static int ReviseDObject(HttpSessionStateBase Context, DObject _param)
        {
            int result = -1;
            DObject targetDobj = DObjectRepository.SelDObject(Context, new DObject { OID = _param.OID });
            DObjectRepository.UdtReleaseLatestDObject(Context, new DObject { OID = _param.OID, IsReleasedLatest = 0 });
            targetDobj.BPolicyOID = null;
            targetDobj.Revision = SemsUtil.MakeMajorRevisonUp(targetDobj.Revision);
            result = DObjectRepository.InsDObject(Context, targetDobj);
            return result;
        }

        public static int CloneDObject(HttpSessionStateBase Context, DObject _param)
        {
            int result = -1;
            DObject targetDobj = DObjectRepository.SelDObject(Context, new DObject { OID = _param.OID });
            result = DObjectRepository.InsDObject(Context, targetDobj);
            return result;
        }

        public static int InsDObject(HttpSessionStateBase Context, DObject _param)
        {
            int result = -1;
            _param.CreateUs = Convert.ToInt32(Context["UserOID"]);
            if (_param.BPolicyOID == null)
            {
                _param.BPolicyOID = BPolicyRepository.SelBPolicy(new BPolicy { Type = _param.Type }).First().OID;
            }

            if (_param.Revision == null)
            {
                BPolicy tmpBPolicy = BPolicyRepository.SelBPolicy(new BPolicy { OID = _param.BPolicyOID }).First();
                if (tmpBPolicy.IsRevision != null && tmpBPolicy.IsRevision.Equals("Y"))
                {
                    _param.TdmxOID = DObjectRepository.SelTdmxOID(Context, new DObject { Type = _param.Type });
                    _param.Revision = CommonConstant.REVISION_PREFIX + CommonConstant.INIT_REVISION;
                }
            }

            _param.IsLatest = 1;
            _param.IsReleasedLatest = 0;
            result = DaoFactory.SetInsert("Comm.InsDObject", _param);
            return result;
        }

        public static int UdtDObject(HttpSessionStateBase Context, DObject _param)
        {
            int result = -1;
            _param.ModifyUs = Convert.ToInt32(Context["UserOID"]);
            result = DaoFactory.SetUpdate("Comm.UdtDObject", _param);
            return result;
        }

        public static int DelDObject(HttpSessionStateBase Context, DObject _param)
        {
            int result = -1;
            _param.DeleteUs = Convert.ToInt32(Context["UserOID"]);
            result = DaoFactory.SetUpdate("Comm.DelDObject", _param);
            return result;
        }

    }
}
