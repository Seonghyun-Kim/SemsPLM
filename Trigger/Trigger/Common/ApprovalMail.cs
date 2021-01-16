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
    public class ApprovalMail : IMailContent
    {
        private string mailContentLink = string.Format(@"<br/><br/><a href='http://{0}' style='color:blue;font-family:gulim;'>※ 우리산업 PLM </a><br/><br/>", "");

        public Person toUserModel { get; set; }

        ApprovalTask approvalTask = null;

        public string MailTitle { get { return "PLM 결재 알림"; } }

        public string SendUserAddress
        {
            get
            {
                return toUserModel.Email;
            }
        }

        public string SendUserName
        {
            get
            {
                return toUserModel.Name;
            }
        }
        public ApprovalMail(HttpSessionStateBase Context, ApprovalTask ApprovTask)
        {
            approvalTask = ApprovTask;
            toUserModel = approvalTask.PersonObj;
        }

        public override string ToString()
        {
            StringBuilder MailMessage = new StringBuilder();

            MailMessage.Append("<div style='margin:10px'>");
            MailMessage.AppendFormat("<label>안녕하세요. {0} 님<br/><br/> PLM 결재가 진행중입니다. 하기 내역을 확인해주세요.</label><br/><br/>", toUserModel.Name);

            MailMessage.Append("<table style='width:800px;table-layout:fixed;line-height:30px;margin:20px auto;'>");
            MailMessage.Append("<colgroup><col style='width: 100px;'><col style='width: 300px;'><col style='width: 100px;'><col style='width: 300px;'></colgroup>");
            MailMessage.Append("<tbody><tr>");
            MailMessage.Append("<td style='background-color:#ddd;font-weight:bold;text-align:center;'>제목</td>");
            MailMessage.AppendFormat("<td colspan='3' style='border:1px solid #ddd;padding:5px;'> {0} </td></tr>", approvalTask.Name);
            MailMessage.Append("<tr><td style = 'background-color:#ddd;font-weight:bold;text-align:center;'> 작성자 </td>");
            MailMessage.AppendFormat("<td style='border: 1px solid #ddd;padding:5px;'>{0}</td>", approvalTask.CreateUsNm);
            MailMessage.Append("<td style='background-color:#ddd;font-weight:bold;text-align:center;'>결재상신일</td>");
            MailMessage.AppendFormat("<td style='border: 1px solid #ddd;padding:5px;'>{0}</td></tr>",  Convert.ToDateTime(approvalTask.CreateDt).ToString("yyyy-MM-dd HH:mm:ss"));
            MailMessage.Append("<tr><td style='background-color:#ddd;font-weight:bold;text-align:center;padding:5px;vertical-align:top;'> 내용 </td>");
            MailMessage.AppendFormat("<td colspan='3' style='border:1px solid #ddd;height:200px;padding:5px;vertical-align:top;'>{0}</td></tr>", approvalTask.Comment);

            MailMessage.Append("</tbody></table>");
            MailMessage.Append(mailContentLink);

            return MailMessage.ToString();
        }
    }
}
