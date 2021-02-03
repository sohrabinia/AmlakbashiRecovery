using MediatR;

namespace Amlakbashi.Mediator.Events.SupportChatEvents
{
    public class InsertMessageEvent : INotification
    {
        public long SupportChatId { get; set; }

        public InsertMessageEvent(long supportChatId)
        {
            SupportChatId = supportChatId;
        }
    }
}
