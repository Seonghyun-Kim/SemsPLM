﻿using Common.Models;
using Common.Utils;
using Pms.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Pms.Trigger
{

    public class ProcApprovPrepareMailContent : IMailContent
    {
        private string mailContentLink = string.Format(@"<br/><br/><a href='http://{0}' style='color:blue;font-family:gulim;'>※ 우리산업 PLM </a><br/><br/>", "http://plm.woory.com");

        public Person toUserModel { get; set; }

        public PmsProject toProject { get; set; }

        public PmsProcess toProcess { get; set; }

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

        public ProcApprovPrepareMailContent(HttpSessionStateBase Context, PmsProject targetProject, PmsProcess targetProcess, Person toPerson)
        {
            toProject = targetProject;
            toProcess = targetProcess;
            toUserModel = toPerson;
        }

        public override string ToString()
        {
            StringBuilder MailMessage = new StringBuilder();

            MailMessage.Append("<div style='margin:10px'>");
            MailMessage.AppendFormat("<label>안녕하세요. {0} 님<br/><br/> {1} 타스크를 진행해주세요. 하기 내역을 확인해주세요.</label><br/><br/>", toUserModel.Name, toProcess.Name);

            MailMessage.Append("<table style='width:800px;table-layout:fixed;line-height:30px;margin:20px auto;'>");
            MailMessage.Append("<colgroup><col style='width: 100px;'><col style='width: 300px;'><col style='width: 100px;'><col style='width: 300px;'></colgroup>");
            MailMessage.Append("<tbody><tr>");
            MailMessage.Append("<td style='background-color:#ddd;font-weight:bold;text-align:center;'>프로젝트 명</td>");
            MailMessage.AppendFormat("<td style='border:1px solid #ddd;padding:5px;'> {0} </td>", toProject.Name);
            MailMessage.Append("<td style='background-color:#ddd;font-weight:bold;text-align:center;'>PM</td>");
            MailMessage.AppendFormat("<td style='border: 1px solid #ddd;padding:5px;'>{0}</td></tr></tr>", toProject.PMNm);
            MailMessage.Append("<tr><td style='background-color:#ddd;font-weight:bold;text-align:center;'>타스크 명</td>");
            MailMessage.AppendFormat("<td colspan='3'  style='border:1px solid #ddd;padding:5px;'> {0} </td></tr>", toProject.Name);
            MailMessage.Append("<tr><td style='background-color:#ddd;font-weight:bold;text-align:center;'> 예상시작일 </td>");
            MailMessage.AppendFormat("<td style='border: 1px solid #ddd;padding:5px;'>{0}</td>", Convert.ToDateTime(toProcess.EstStartDt).ToString("yyyy-MM-dd"));
            MailMessage.Append("<td style='background-color:#ddd;font-weight:bold;text-align:center;'> 예상완료일 </td>");
            MailMessage.AppendFormat("<td style='border: 1px solid #ddd;padding:5px;'>{0}</td></tr>", Convert.ToDateTime(toProcess.EstEndDt).ToString("yyyy-MM-dd"));
            MailMessage.Append("<tr><td style='background-color:#ddd;font-weight:bold;text-align:center;padding:5px;vertical-align:top;'> 내용 </td>");
            MailMessage.AppendFormat("<td colspan='3' style='border:1px solid #ddd;height:200px;padding:5px;vertical-align:top;'>{0}</td></tr>", toProcess.Description);

            MailMessage.Append("</tbody></table>");
            MailMessage.Append(mailContentLink);

            return MailMessage.ToString();
        }
    }

    public class ProcApprovCompleteMailContent : IMailContent
    {
        private string mailContentLink = string.Format(@"<br/><br/><a href='http://{0}' style='color:blue;font-family:gulim;'>※ 우리산업 PLM </a><br/><br/>", "");

        public Person toUserModel { get; set; }

        public PmsProject toProject { get; set; }

        public PmsProcess toProcess { get; set; }

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

        public ProcApprovCompleteMailContent(HttpSessionStateBase Context, PmsProject targetProject, PmsProcess targetProcess, Person toPerson)
        {
            toProject = targetProject;
            toProcess = targetProcess;
            toUserModel = toPerson;
        }

        public override string ToString()
        {
            StringBuilder MailMessage = new StringBuilder();

            MailMessage.Append("<div style='margin:10px'>");
            MailMessage.AppendFormat("<label>안녕하세요. {0} 님<br/><br/> {1} 타스크가 완료되었습니다. 하기 내역을 확인해주세요.</label><br/><br/>", toUserModel.Name, toProcess.Name);

            MailMessage.Append("<table style='width:800px;table-layout:fixed;line-height:30px;margin:20px auto;'>");
            MailMessage.Append("<colgroup><col style='width: 100px;'><col style='width: 300px;'><col style='width: 100px;'><col style='width: 300px;'></colgroup>");
            MailMessage.Append("<tbody><tr>");
            MailMessage.Append("<td style='background-color:#ddd;font-weight:bold;text-align:center;'>프로젝트 명</td>");
            MailMessage.AppendFormat("<td style='border:1px solid #ddd;padding:5px;'> {0} </td>", toProject.Name);
            MailMessage.Append("<td style='background-color:#ddd;font-weight:bold;text-align:center;'>PM</td>");
            MailMessage.AppendFormat("<td style='border: 1px solid #ddd;padding:5px;'>{0}</td></tr></tr>", toProject.PMNm);
            MailMessage.Append("<tr><td style='background-color:#ddd;font-weight:bold;text-align:center;'>타스크 명</td>");
            MailMessage.AppendFormat("<td colspan='3'  style='border:1px solid #ddd;padding:5px;'> {0} </td></tr>", toProject.Name);
            MailMessage.Append("<tr><td style='background-color:#ddd;font-weight:bold;text-align:center;'> 예상시작일 </td>");
            MailMessage.AppendFormat("<td style='border: 1px solid #ddd;padding:5px;'>{0}</td>", Convert.ToDateTime(toProcess.EstStartDt).ToString("yyyy-MM-dd"));
            MailMessage.Append("<td style='background-color:#ddd;font-weight:bold;text-align:center;'> 예상완료일 </td>");
            MailMessage.AppendFormat("<td style='border: 1px solid #ddd;padding:5px;'>{0}</td></tr>", Convert.ToDateTime(toProcess.EstEndDt).ToString("yyyy-MM-dd"));
            MailMessage.Append("<tr><td style='background-color:#ddd;font-weight:bold;text-align:center;padding:5px;vertical-align:top;'> 내용 </td>");
            MailMessage.AppendFormat("<td colspan='3' style='border:1px solid #ddd;height:200px;padding:5px;vertical-align:top;'>{0}</td></tr>", toProcess.Description);

            MailMessage.Append("</tbody></table>");
            MailMessage.Append(mailContentLink);

            return MailMessage.ToString();
        }
    }
}
