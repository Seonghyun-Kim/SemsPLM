using Common.Constant;
using Common.Models;
using Common.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Common.Trigger
{
    public class ReleasedTrigger
    {
        public string ReleasedLatestUpdate(object[] args)
        {
            object[] oArgs = args;
            HttpSessionStateBase Context = (HttpSessionStateBase)oArgs[0];
            string type = Convert.ToString(oArgs[1]);
            string status = Convert.ToString(oArgs[2]);
            string oid = Convert.ToString(oArgs[3]);
            string action = Convert.ToString(oArgs[5]);
            try
            {
                if (action.Equals(CommonConstant.ACTION_PROMOTE))
                {
                    var Data = DObjectRepository.SelDObject(Context, new DObject { OID = Convert.ToInt32(oid) });
                    DObjectRepository.UdtReleaseLatestDObject(Context, new DObject { OID = Data.OID, IsReleasedLatest = 1 });

                    List<DObject> Tdmx = DObjectRepository.SelDObjects(Context, new DObject { TdmxOID = Data.TdmxOID });
                    List<DObject> appyStruct = new List<DObject>();

                    Tdmx.ForEach(Obj =>
                    {
                        if(Obj.IsReleasedLatest == 1 && Obj.Revision != Data.Revision)
                        {
                            appyStruct.Add(Obj);
                        }
                    });
                    appyStruct = appyStruct.OrderByDescending(revVal => revVal.Revision).ToList();

                    if (appyStruct.Count > 0)
                    {
                        appyStruct.ForEach(v => {
                            DObjectRepository.UdtReleaseLatestDObject(Context, new DObject { OID = v.OID, IsReleasedLatest = 0 });
                            DObjectRepository.UdtLatestDObject(Context, new DObject { OID = v.OID });
                        });
                    }
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
