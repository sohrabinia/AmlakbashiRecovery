using Amlakbashi.Core.Common.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Amlakbashi.Core.Common.ContactEngines
{
    public static class EmailEngine
    {
        private static string WebsiteName = "Amlakbashi";
        private static string WebsiteUrl = "amlakbashi.com";
        private static string SmtpHost = "mail.amlakbashi.com";
        private static int SmtpPort = 25;
        private static string UserName = "administrator";
        private static string Password = "@li#$%S0hR@b!@N98(8(0(*";

        public static void SendEmail(EmailSenderDepartment department, List<string> to,
            string subject, string body,
            List<string> cc = null, List<string> bcc = null,
            Attachment attachment = null)
        {
            MailMessage message = new MailMessage();
            message.From = new MailAddress(department.ToString() +
                "@" + WebsiteUrl, WebsiteName, Encoding.UTF8);
            foreach (string item in to)
            {
                message.To.Add(new MailAddress(item));
            }
            message.Subject = subject;
            message.SubjectEncoding = Encoding.UTF8;
            message.Body = body;
            message.BodyEncoding = Encoding.UTF8;
            message.IsBodyHtml = true;
            SmtpClient smtp = new SmtpClient();
            var credential = new NetworkCredential
            {
                Domain = WebsiteUrl,
                UserName = UserName,
                Password = Password
            };
            smtp.Credentials = credential;
            smtp.Host = SmtpHost;
            smtp.Port = SmtpPort;
            smtp.EnableSsl = false;
            smtp.Send(message);
        }
    }

    //public enum EmailSenderDepartment
    //{
    //    Verification,
    //    Support,
    //    Info
    //}
}
