using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amlakbashi.Mediator.Commands.UserCommands
{
    public class ScheduleSendCustomSms : IRequest
    {
        public int Delay { get; set; }
        public string Mobile { get; set; }
        public string Template { get; set; }
        public ScheduleSendCustomSms(int delay, string mobile, string template)
        {
            Delay = delay;
            Mobile = mobile;
            Template = template;
        }
    }
}
