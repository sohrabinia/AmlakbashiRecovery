using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amlakbashi.Mediator.Commands.UserCommands
{
    public class SendVerificationSmsCommand : IRequest
    {
        public string LocalNumber { get; set; }
        public string Code { get; set; }
        public SendVerificationSmsCommand(string localNumber, string code)
        {
            this.LocalNumber = localNumber;
            this.Code = code;
        }
    }
}
