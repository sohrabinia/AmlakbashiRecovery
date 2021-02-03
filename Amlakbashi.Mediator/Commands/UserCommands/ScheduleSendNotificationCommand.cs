using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amlakbashi.Mediator.Commands.UserCommands
{
    public class ScheduleSendNotificationCommand : IRequest
    {
        public string Token { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public string ClickAction { get; set; }

        public ScheduleSendNotificationCommand(string token, string title,
            string body, string clickAction)
        {
            this.Token = token;
            this.Title = title;
            this.ClickAction = clickAction;
            this.Body = body;
        }
    }
}
