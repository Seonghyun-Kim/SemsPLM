using Common.Factory;
using Common.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Common.Models.File
{
    // 2021.02.28 김성현
    public class DFileHistory
    {
        public int? SEQ { get; set; }

        public int? FileOID { get; set; }

        public string OrgNm { get; set; }

        // OBJECT
        public int? OID { get; set; }

        public string Name { get; set; }

        // OBJECT Type
        public string Type { get; set; }


        public string ActionType { get; set; }

        public int? ActionUser { get; set; }

        public string ActionUserNm { get; set; }

        public string ActionIPAddress { get; set; }

        public DateTime ActionDt { get; set; }

        public string ActionMacAddress { get; set; }

        public string SessionID { get; set; }

        public string Description { get; set; }

    }

    public class DFileHistoryRepository
    {
        public static List<DFileHistory> SelDFileHistories(DFileHistory _param)
        {
            return DaoFactory.GetList<DFileHistory>("Comm.SelDFileHistory", _param);
        }

        public static int InsDFileHistory(HttpSessionStateBase Context, string ActionType, HttpFile file)
        {
            DFileHistory _param = new DFileHistory();
            _param.FileOID = file.FileOID;
            _param.OID = file.OID;
            _param.Type = file.Type;
            _param.ActionType = ActionType;
            _param.ActionUser = Convert.ToInt32(Context["UserOID"]);
            _param.ActionIPAddress = SemsConnect.GetRemoteIP(HttpContext.Current.Request);
            _param.ActionMacAddress = SemsConnect.GetMacAddress(_param.ActionIPAddress);
            _param.SessionID = Context.SessionID;

            return DaoFactory.SetInsert("Comm.InsDFileHistory", _param);
        }

        public static int InsDFileHistory(HttpSessionStateBase Context, DFileHistory _param)
        {
            _param.ActionUser = Convert.ToInt32(Context["UserOID"]);
            _param.ActionIPAddress = SemsConnect.GetRemoteIP(HttpContext.Current.Request);
            _param.ActionMacAddress = SemsConnect.GetMacAddress(_param.ActionIPAddress);
            _param.SessionID = Context.SessionID;
            return DaoFactory.SetInsert("Comm.InsDFileHistory", _param);
        }
    }
}
