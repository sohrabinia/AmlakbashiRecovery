using Amlakbashi.Core.Infrastructure.UserContact;
using MediatR;

namespace Amlakbashi.Mediator.Commands.UserCommands
{
    public class SendSmsCommand : IRequest
    {
        public UserContactDTO UserContact { get; set; }
        public SendSmsCommand(UserContactDTO contactDTO)
        {
            UserContact = contactDTO;
        }
    }
}
