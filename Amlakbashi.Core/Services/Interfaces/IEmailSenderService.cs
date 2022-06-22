using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Amlakbashi.Core.Services.Interfaces
{
    public interface IEmailSenderService
    {
        Task SendAsync(string to, string subject, string content);
        Task SendAsync(IEnumerable<string> to, string subject, string content);
    }
}
