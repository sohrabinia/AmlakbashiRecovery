using MediatR;

namespace Amlakbashi.Mediator.Commands.SupportChatCommands
{
    public class SendSupporterMessageCommand : IRequest
    {
        public int Delay { get; set; }
        public long SupportChatId { get; set; }
        public long MessageId { get; set; }
        public SendSupporterMessageCommand(int delay, long messageId, long supportChatId)
        {
            Delay = delay;
            SupportChatId = supportChatId;
            MessageId = messageId;
        }
    }
}
