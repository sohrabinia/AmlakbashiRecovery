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
    internal class SupportChatAppService : BaseAppService<SupportChat, long>, ISupportChatAppService
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
            var delayTime = DateTime.Now.AddMinutes(-120);
            return Repository.Query(q => q.FirstOrDefault(x => x.UserID == userId &&
              x.LastMessageTime > delayTime));
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

        public IList<long> UpdateMessagesReadStatus(long id, SupportChatMessage.TypeEnum type = SupportChatMessage.TypeEnum.Supporter)
        {
            var supportChat = Repository.Find(id);
            var notReadedMessages = supportChat.Messages.Where(x => x.ReadStatus == SupportChatMessage.ReadStatusEnum.NotRead &&
                x.Type == type).ToList();
            foreach (var item in notReadedMessages)
            {
                item.ReadStatus = SupportChatMessage.ReadStatusEnum.Read;
            }
            Repository.Update(supportChat);
            Repository.Save();
            return notReadedMessages.Select(x => x.Id).ToList();
        }

        public void ScheduleSendSupporterNewMsgNotif(int delay, long messageId, long supportChatId, string[] supportersNotifToken)
        {
            var delayTimeSpan = new TimeSpan(0, 0, delay);
            mediator.Schedule(new SendSupporterMessageCommand(delay, messageId, supportChatId, supportersNotifToken), delayTimeSpan);
        }
    }
}
