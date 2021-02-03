using Amlakbashi.Core.Infrastructure.UserContact;
using MediatR;

namespace Amlakbashi.Mediator.Commands.UserCommands
{
    public class SendMessageCommand : IRequest
    {
        public UserContactDTO UserContact { get; set; }
        public SendMessageCommand(UserContactDTO contactDTO)
        {
            UserContact = contactDTO;
        }
    }
}
