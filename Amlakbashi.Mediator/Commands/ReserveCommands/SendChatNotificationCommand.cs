using MediatR;

namespace Amlakbashi.Mediator.Commands.ReserveCommands
{
    public class SendChatNotificationCommand : IRequest
    {
        public long ChatId { get; set; }
        public int TargetUserId { get; set; }
        public bool IsGuest { get; set; }
        public int SenderUserId { get; set; }
        public bool IsFirstChat { get; set; }
        public SendChatNotificationCommand(long chatId, int targetUserId, bool isGuest,
            int senderUserId, bool isFirstChat)
        {
            ChatId = chatId;
            TargetUserId = targetUserId;
            IsGuest = isGuest;
            SenderUserId = senderUserId;
            IsFirstChat = isFirstChat;
        }
    }
}
