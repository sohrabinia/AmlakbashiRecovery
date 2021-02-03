using Amlakbashi.Core.Infrastructure.UserContact;
using MediatR;

namespace Amlakbashi.Mediator.Commands.AccountingCommands
{
    public class ScheduleSendMessageGroupPaymentCommand : IRequest
    {
        public UserContactDTO UserContactDTO { get; set; }

        public ScheduleSendMessageGroupPaymentCommand(UserContactDTO userContact)
        {
            this.UserContactDTO = userContact;
        }
    }
}
