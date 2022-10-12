using Amlakbashi.Core.Common.AppService;
using Amlakbashi.Core.Common.Repository;
using Amlakbashi.Application.Services.SupportChatServices.Interfaces;
using Amlakbashi.Core.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using static Amlakbashi.Core.Entities.SupportChatMessage;
using Amlakbashi.Mediator.Events.SupportChatEvents;

namespace Amlakbashi.Application.Services.SupportChatServices
{
    internal class SupportChatMessageAppService : AppServiceBase<SupportChatMessage, long>, ISupportChatMessageAppService
    {
        private readonly IMediator mediator;
        public SupportChatMessageAppService(IRepository<SupportChatMessage, long> repository,
            IMediator mediator) : base(repository)
        {
            this.mediator = mediator;
        }

        public SupportChatMessage Find(long id)
        {
            return Repository.Query(q => q.FirstOrDefault(f => f.Id == id));
        }

        public long Insert(string text, TypeEnum type,
            long supportChatId, int? userId, ReadStatusEnum initialRead = ReadStatusEnum.NotRead)
        {
            var message = new SupportChatMessage()
            {
                Text = text,
                CreateTime = DateTime.Now,
                ReadStatus = initialRead,
                Type = type,
                SupportChatID = supportChatId,
                UserID = userId
            };
            Repository.Insert(message);
            Repository.Save();
            mediator.Publish(new InsertMessageEvent(supportChatId));
            return message.Id;
        }

        public void UpdateReadStatus(long id)
        {
            var data = Repository.Query(q => q.FirstOrDefault(f => f.Id == id));
            data.ReadStatus = ReadStatusEnum.Read;
            Repository.Update(data);
            Repository.Save();
        }

        public void UpdateReadStatusList(IList<long> listId)
        {
            foreach (var id in listId)
            {
                var message = Repository.Find(id);
                message.ReadStatus = ReadStatusEnum.Read;
                Repository.Update(message);
            }
            Repository.Save();
        }
    }
}
