using Amlakbashi.Core.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Amlakbashi.Core.Services
{
    public class EmailSenderService : IEmailSenderService
    {
        public async Task SendAsync(string to, string subject, string content)
        {
            await SendAsync(new string[] { to }, subject, content);
        }

        public async Task SendAsync(IEnumerable<string> to, string subject, string content)
        {
            MailMessage message = new MailMessage();
            message.From = new MailAddress("info@amlakbashi.com", "املاک باشی", Encoding.UTF8);
            foreach (string item in to)
            {
                message.To.Add(new MailAddress(item));
            }
            message.Subject = subject;
            message.SubjectEncoding = Encoding.UTF8;
            message.Body = content;
            message.BodyEncoding = Encoding.UTF8;
            message.IsBodyHtml = true;

            using (var smtp = new SmtpClient())
            {
                var credential = new NetworkCredential
                {
                    Domain = "amlakbashi.com",
                    UserName = "administrator",
                    Password = "@li#$%S0hR@b!@N98(8(0(*"
                };
                smtp.Credentials = credential;
                smtp.Host = "mail.amlakbashi.com";
                smtp.Port = 25;
                smtp.EnableSsl = false;
                await smtp.SendMailAsync(message);
            }
        }
    }
}
