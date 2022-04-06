using Amlakbashi.Core.Common.Repository;
using Amlakbashi.Core.Entities;
using MediatR;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Amlakbashi.Mediator.Events.SupportChatEvents;

namespace Amlakbashi.Application.Services.SupportChatServices.EventHandlers
{
    internal class InsertMessageEventHandler : INotificationHandler<InsertMessageEvent>
    {
        private readonly IRepository<SupportChat, long> repository;
        public InsertMessageEventHandler(IRepository<SupportChat, long> repository)
        {
            this.repository = repository;
        }
        public Task Handle(InsertMessageEvent notification, CancellationToken cancellationToken)
        {

            var supportChat = repository.Find(notification.SupportChatId);
            supportChat.LastMessageTime = DateTime.Now;
            repository.Update(supportChat);
            repository.Save();
            return Task.CompletedTask;
        }
    }
}
