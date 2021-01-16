using ChangeRequest.Models;
using Common.Factory;
using Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace ChangeOrder.Models
{
    public class EO : DRelationship
    {
        public int? RootOID { get; set; }
        public string PNo { get; set; }
        public string PName { get; set; }
        public string BfThumbnail { get; set; }
        public string AfThumbnail { get; set; }
        public string Statement { get; set; }
        public string Description { get; set; }
        public string ProType { get; set; }
        public List<EO> CheckData { get; set; }
    }

    public static class EORepository
    {
        //EO 관련 내용 삽입
        public static int InsEOContents(HttpSessionStateBase Context, EO _param)
        {
            _param.CreateUs = Convert.ToInt32(Context["UserOID"]);
            return DaoFactory.SetInsert("EO.InsEOContents", _param);
        }

        //EO 관련 내용 검색
        public static List<EO> SelEOContentsOID(EO _param)
        {
            List<EO> lEO = DaoFactory.GetList<EO>("EO.SelEOContents", _param);
            return lEO;
        }
        public static EO SelEOContentsObject(EO _param)
        {
            EO lEO = DaoFactory.GetData<EO>("EO.SelEOContents", _param);
            return lEO;
        }

        public static int delEOContents(EO _param)
        {
            return DaoFactory.SetDelete("EO.delEOContents", _param);
        }

        public static List<int> partOIDList(EO _param)
        {
            List<int> OIDList = DaoFactory.GetList<int>("EO.partOIDList", _param);
            return OIDList;
        }
        public static List<ECR> SelECRRelation(List<EO> _param)
        {
            List<ECR> IECR = new List<ECR>();
            _param.ForEach(item =>
            {
                ECR data = DaoFactory.GetData<ECR>("ChangeRequest.SelChangeRequest",new ECR { OID = item.ToOID });

                data.BPolicy = BPolicyRepository.SelBPolicy(new BPolicy { Type = data.Type, OID = data.BPolicyOID }).First();

                IECR.Add(data);
            });
           
            return IECR;
        }
    }
}
