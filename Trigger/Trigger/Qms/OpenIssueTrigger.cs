using Common.Constant;
using Common.Factory;
using Common.Models;
using Pms.Models;
using Qms.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Qms.Trigger
{
    public class OpenIssueTrigger
    {
        public string OpenIssueCreatePromote(object[] args)
        {
            object[] oArgs = args;
            HttpSessionStateBase Context = (HttpSessionStateBase)oArgs[0];
            int iAdmin = Convert.ToInt32(Context["UserOID"]);
            string type = Convert.ToString(oArgs[1]);
            string status = Convert.ToString(oArgs[2]);
            string oid = Convert.ToString(oArgs[3]);
            string action = Convert.ToString(oArgs[5]);
            try
            {
                if (action.Equals(CommonConstant.ACTION_REJECT))
                {
                    return "";
                }
                OpenIssue _openIssue = OpenIssueRepository.SelOpenIssue(new OpenIssue() { ProjectOID = Convert.ToInt32(oid) });

                // 해당 프로젝트의 OPEN ISSUE 가 있을 경우는 패스함.
                if(_openIssue != null)
                {
                    return "";
                }

                //PmsProject pmsProject = DaoFactory.GetData<PmsProject>("Pms.SelPmsProject", new PmsProject() { OID = Convert.ToInt32(oid), Type = type });
                PmsProject pmsProject = PmsProjectRepository.SelPmsObject(Context, new PmsProject() { OID = Convert.ToInt32(oid), Type = type });

                OpenIssue openIssue = new OpenIssue();

                DObject dobj = new DObject();
                dobj.Type = QmsConstant.TYPE_OPEN_ISSUE;
                dobj.Name = QmsConstant.TYPE_OPEN_ISSUE;

                openIssue.OID = DObjectRepository.InsDObject(Context, dobj);
                openIssue.CustomerLibOID = pmsProject.Customer_OID;
                openIssue.OemLibOID = pmsProject.Oem_Lib_OID;
                openIssue.CarLibOID = pmsProject.Car_Lib_OID;
                openIssue.ProjectOID = pmsProject.OID;

                OpenIssueRepository.InsOpenIssue(openIssue);
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            return "";
        }

        public string OpenIssueItemCntPromote(object[] args)
        {
            object[] oArgs = args;
            HttpSessionStateBase Context = (HttpSessionStateBase)oArgs[0];
            int iAdmin = Convert.ToInt32(Context["UserOID"]);
            string type = Convert.ToString(oArgs[1]);
            string status = Convert.ToString(oArgs[2]);
            string oid = Convert.ToString(oArgs[3]);

            try
            {

                OpenIssue _openIssue = OpenIssueRepository.SelOpenIssue(new OpenIssue() { ProjectOID = Convert.ToInt32(oid) });

                List<OpenIssueItem> openIssueItems = OpenIssueItemRepository.SelOpenIssueItems(new OpenIssueItem() { OpenIssueOID = _openIssue.OID });

                if(openIssueItems == null && openIssueItems.Count() <= 0)
                {
                    return "";
                }

                int SuspenseCnt = 0;
                int UncommittedCnt = 0;
                int CompleatedCnt = 0;

                openIssueItems.ForEach(v =>
                {
                    if(v.BPolicyNm == QmsConstant.POLICY_OPENISSUE_ITEM_COMPLETED)
                    {
                        CompleatedCnt++;
                    }
                    else if (v.BPolicyNm == QmsConstant.POLICY_OPENISSUE_ITEM_UNCOMMITTED)
                    {
                        UncommittedCnt++;
                    }
                    else if (v.BPolicyNm == QmsConstant.POLICY_OPENISSUE_ITEM_SUSPENSE)
                    {
                        SuspenseCnt++;
                    }
                });

                _openIssue.CompleatedCnt = CompleatedCnt;
                _openIssue.UncommittedCnt = UncommittedCnt;
                _openIssue.SuspenseCnt = SuspenseCnt;

                DaoFactory.SetUpdate("Qms.UdtOpenIssueCnt", _openIssue);

            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            return "";
        }

    }
}
