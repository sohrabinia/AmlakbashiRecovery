using System.Collections.Generic;
using System.Net.Mail;
using System.Net;

namespace Amlakbashi.Core.Common.Utilities
{
    public static class EmailUtility
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
            message.From = new MailAddress(department.ToString() + "@" + WebsiteUrl,
                WebsiteName, System.Text.Encoding.UTF8);
            foreach (string item in to)
            {
                message.To.Add(new MailAddress(item));
            }
            message.Subject = subject;
            message.SubjectEncoding = System.Text.Encoding.UTF8;
            message.Body = body;
            message.BodyEncoding = System.Text.Encoding.UTF8;
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

        public static bool ValidateEmail(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return false;
            }
            return EmailValidation.EmailValidator.Validate(email);
        }
    }

    public enum EmailSenderDepartment
    {
        Verification,
        Support,
        Info
    }
}
