using Amlakbashi.Core.Common.AppService;
using Amlakbashi.Core.Common.Repository;
using Amlakbashi.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using Amlakbashi.Application.Services.SupportChatServices.Interfaces;
using MediatR;
using Amlakbashi.Core.Common.Extensions;
using Amlakbashi.Mediator.Commands.SupportChatCommands;

namespace Amlakbashi.Application.Services.SupportChatServices
{
    internal class SupportChatAppService : AppServiceBase<SupportChat, long>, ISupportChatAppService
    {
        private readonly IMediator mediator;
        public SupportChatAppService(IRepository<SupportChat, long> repository, IMediator mediator) : base(repository)
        {
            this.mediator = mediator;
        }

        public IList<SupportChat> GetLastItems(int count, int currentItemCount = 0)
        {
            return Repository.Query(q => q.OrderByDescending(x => x.LastMessageTime)
            .Skip(currentItemCount).Take(count).ToList());
        }

        public SupportChat Find(long id)
        {
            return Repository.Query(q => q.FirstOrDefault(x => x.Id == id));
        }

        public SupportChat GetByUserId(int userId)
        {
            var halfHourBefore = DateTime.Now.AddMinutes(-30);
            return Repository.Query(q => q.FirstOrDefault(x => x.UserID == userId &&
              x.CreateTime > halfHourBefore));
        }

        public SupportChat Insert(int userId)
        {
            SupportChat supportChat = new SupportChat();
            supportChat.CreateTime = DateTime.Now;
            supportChat.LastMessageTime = DateTime.Now;
            supportChat.UserID = userId;
            Repository.Insert(supportChat);
            Repository.Save();
            return supportChat;
        }

        public void ScheduleSendSupporterNewMsgNotif(int delay, long messageId, long supportChatId, string[] supportersNotifToken)
        {
            var delayTimeSpan = new TimeSpan(0, 0, delay);
            mediator.Schedule(new SendSupporterMessageCommand(delay, messageId, supportChatId, supportersNotifToken), delayTimeSpan);
        }
    }
}
