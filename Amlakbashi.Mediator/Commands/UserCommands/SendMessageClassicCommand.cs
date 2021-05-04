using Amlakbashi.Core.Infrastructure.UserContact;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amlakbashi.Mediator.Commands.UserCommands
{
    public class SendMessageClassicCommand : IRequest
    {
        public bool Initial { get; set; }
        public UserContactDTO UserContact { get; set; }
        public SendMessageClassicCommand(bool initial, UserContactDTO userContact)
        {
            this.Initial = initial;
            this.UserContact = userContact;
        }
    }
}
