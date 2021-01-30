using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace Common.Utils
{
    public class SemsSmtp
    {
        SmtpClient smtpClient = null;

        private string m_SmtpHost = string.Empty;
        private int m_StmpPort = 25; // Default Port

        private string m_FromAddress = string.Empty;
        private string m_ToAddress = string.Empty;

        private bool m_IsAuth = false;
        private bool m_IsPassSecu = false;

        private string m_AuthUserID = string.Empty;
        private string m_AuthPassword = string.Empty;

        private string m_MailSubjet = string.Empty;
        private string m_mailBody = string.Empty;
        private string m_SendFromName = string.Empty;
        private string m_SendToName = string.Empty;
        private string m_SendCc = string.Empty; // 참조 
        private int m_SmtpTimeOut = 30;

        List<IMailContent> MailList = new List<IMailContent>();

        public SemsSmtp()
        {
            //Request = pRequest;
            string smtpHost = System.Configuration.ConfigurationManager.AppSettings["SmtpHost"];
            int smtpPort = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["SmtpPort"]);
            string authUserID = System.Configuration.ConfigurationManager.AppSettings["SmtpUserID"];
            //string authPassword = SemsEncrypt.AESDecrypte256Text(System.Configuration.ConfigurationManager.AppSettings["SmtpPassword"], Define.ConstDefine.LoginKey);
            string authPassword = System.Configuration.ConfigurationManager.AppSettings["SmtpPassword"];
            string smtpID = System.Configuration.ConfigurationManager.AppSettings["SmtpID"];
            bool isAuthLogin = System.Configuration.ConfigurationManager.AppSettings["IsAuthLogin"] == "Y" ? true : false;
            bool isPasswordSecurity = System.Configuration.ConfigurationManager.AppSettings["IsPasswordSecurity"] == "Y" ? true : false;

            SetSmtpClient(smtpHost, smtpPort, authUserID, authPassword, isAuthLogin, isPasswordSecurity);
        }

        public SemsSmtp(string smtpHost, int smtpPort, string authUserID, string authPassword, bool isAuthLogin = false, bool isPasswordSecurity = false)
        {
            //Request = pRequest;
            SetSmtpClient(smtpHost, smtpPort, authUserID, authPassword, isAuthLogin, isPasswordSecurity);
        }

        private void SetSmtpClient(string smtpHost, int smtpPort, string authUserID, string authPassword, bool isAuthLogin, bool isPasswordSecurity)
        {
            if (System.Configuration.ConfigurationManager.AppSettings["smtpTimeOut"] != null)
            {
                m_SmtpTimeOut = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["smtpTimeOut"]);
            }

            smtpClient = new SmtpClient(smtpHost, smtpPort);
            smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtpClient.UseDefaultCredentials = false;
            smtpClient.EnableSsl = Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["EnableSSL"]);
            smtpClient.Timeout = m_SmtpTimeOut * 1000;

            if (isAuthLogin)
            {
                var secPw = new SecureString();
                foreach (var i in authPassword) secPw.AppendChar(i);

                string PASSWORD = isPasswordSecurity ? secPw.ToString() : authPassword;
                smtpClient.Credentials = new NetworkCredential(authUserID, PASSWORD);
            }
            m_AuthUserID = authUserID;
        }

        public void SetMailInfo(IMailContent mailContent)
        {
            try
            {
                MailList.Add(mailContent);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void SendMail()
        {
            if (System.Configuration.ConfigurationManager.AppSettings["IsDev"] == "Y")
            {
                return;
            }

            foreach (IMailContent mailInfo in MailList)
            {
                MailMessage mail = new MailMessage();
                mail.From = new MailAddress(m_AuthUserID, "PLM 시스템 관리자");

                mail.To.Add(new MailAddress(mailInfo.SendUserAddress, mailInfo.SendUserName));

                mail.Subject = string.Format(mailInfo.MailTitle, mailInfo.SendUserName);
                mail.Body = mailInfo.ToString();
                mail.SubjectEncoding = System.Text.Encoding.Default;
                mail.BodyEncoding = System.Text.Encoding.Default;
                mail.IsBodyHtml = true;

                smtpClient.Send(mail);
            }
        }
    }
    public interface IMailContent
    {
        string SendUserAddress { get; }

        string SendUserName { get; }

        string MailTitle { get; }

        string ToString();
    }
}
