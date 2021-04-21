using Amlakbashi.Core.Common.Extensions;
using Amlakbashi.Core.Common.Repository;
using Amlakbashi.Core.Entities;
using Amlakbashi.Core.Infrastructure.UserContact.Interfaces;
using Amlakbashi.Mediator.Commands.ReserveCommands;
using Amlakbashi.Mediator.Commands.UserCommands;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using static Amlakbashi.Core.Entities.Chat;

namespace Amlakbashi.Application.Services.ReserveServices.CommandHandlers
{
    public class ChatCommandHandler : IRequestHandler<SendChatNotificationCommand>
    {
        private readonly IUserContactFacade userContact;
        private readonly IRepository<Chat, long> repository;
        private readonly IMediator mediator;
        public ChatCommandHandler(IRepository<Chat, long> repository, IUserContactFacade userContact, IMediator mediator)
        {
            this.userContact = userContact;
            this.repository = repository;
            this.mediator = mediator;
        }

        public Task<Unit> Handle(SendChatNotificationCommand request, CancellationToken cancellationToken)
        {
            var chat = repository.Find(request.ChatId);
            if (chat.ReadStatus == (int)ReadStatusEnum.NotRead)
            {
                var user = repository.Find<User, int>(request.TargetUserId);
                if (!string.IsNullOrEmpty(user.FcmAppNotificationToken) ||
                    !string.IsNullOrEmpty(user.AppNotificationToken) ||
                    !string.IsNullOrEmpty(user.NotificationToken))
                {
                    mediator.Enqueue(new SendMessageCommand(new Core.Infrastructure.UserContact.UserContactDTO()
                    {
                        UserMainMobile = user.MainMobile,
                        UserAppNotificationToken = user.AppNotificationToken,
                        UserEmail = user.Email,
                        UserFcmAppNotificationToken = user.FcmAppNotificationToken,
                        UserNotificationToken = user.NotificationToken,
                        Type = request.IsGuest ? Core.Infrastructure.UserContact.UserContactType.NewReserveChatGuest :
                            Core.Infrastructure.UserContact.UserContactType.NewReserveChatHost,
                        UserId = request.SenderUserId.ToString(),
                        ReserveId = chat.ReserveID.ToString()
                    }));
                }
            }
            return Task.FromResult(Unit.Value);
        }
    }
}
