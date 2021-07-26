using Amlakbashi.Core.Common.AppService;
using Amlakbashi.Core.Common.Repository;
using Amlakbashi.Application.Services.ReserveServices.Interfaces;
using Amlakbashi.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using MediatR;
using Amlakbashi.Mediator.Commands.ReserveCommands;
using Amlakbashi.Core.Common.Extensions;

namespace Amlakbashi.Application.Services.ReserveServices
{
    internal class ChatAppService : AppServiceBase<Chat, long>, IChatAppService
    {
        private readonly IMediator mediator;
        public ChatAppService(IRepository<Chat, long> repository, IMediator mediator) : base(repository)
        {
            this.mediator = mediator;
        }

        public IList<Chat> Filter(long chatId, long reserveId, int userId, int chatStatus)
        {
            IQueryable<Chat> model = Repository.Query(q => q);
            if (chatId > 0)
            {
                model = model.Where(x => x.Id == chatId);
            }
            if (reserveId > 0)
            {
                model = model.Where(x => x.ReserveID == reserveId);
            }
            if (userId > 0)
            {
                model = model.Where(x => x.UserID == userId);
            }
            if (chatStatus > -1)
            {
                model = model.Where(x => x.ChatStatus == chatStatus);
            }
            return model.OrderByDescending(x => x.Id).ToList();
        }

        public IList<Chat> GetReserveChats(long reserveId)
        {
            IQueryable<Chat> allChats = Repository.Query(q => q);
            allChats = allChats.Where(x => x.ReserveID == reserveId).OrderBy(x => x.CreateTime);
            return allChats.ToList();
        }

        public IList<Chat> GetListAgainstUserId(int userId, Chat.ChatStatusEnum status, Chat.ReadStatusEnum read,
            IList<long> reserveIds = null)
        {
            var data = Repository.Query(q => q);
            data = data.Where(x =>
                    x.ChatStatus == (int)status &&
                    x.ReadStatus == (int)read &&
                    x.UserID != userId);
            if (reserveIds != null)
            {
                data = data.Where(w => reserveIds.Contains(w.ReserveID));
            }
            return data.ToList();
        }

        public Chat Find(int id)
        {
            return Repository.Query(q => q.FirstOrDefault(f => f.Id == id));
        }

        public int GetCountByReserveId(long reserveId)
        {
            return Repository.Query(q => q.Count(x => x.ReserveID == reserveId));
        }

        public int GetNotReadCountByReserveId(long reserveId, int userId)
        {
            return Repository.Query(q => q.Count(x => x.ReserveID == reserveId &&
                    x.UserID != userId &&
                    x.ReadStatus == (int)Chat.ReadStatusEnum.NotRead));
        }

        public int GetNotReadSupportCountByReserveId(long reserveId)
        {
            return Repository.Query(q => q.Count(x => x.ReserveID == reserveId &&
                    x.SupportReadStatus == (int)Chat.ReadStatusEnum.NotRead));
        }

        public Chat Insert(Chat chat)
        {
            Repository.Insert(chat);
            Repository.Save();
            return chat;
        }

        public void UpdateChatListReadStatus(IList<Chat> chats)
        {
            var ids = chats.Select(s => s.Id).ToList();
            var data = Repository.Query(q => q.Where(w => ids.Contains(w.Id)));
            foreach (var item in data)
            {
                item.ReadStatus = (int)Chat.ReadStatusEnum.Read;
                Repository.Update(item);
            }
            Repository.Save();
        }

        public IList<Chat> UpdateSupportReadStatusByReserveId(long reserveId)
        {
            var data = Repository.Query(q => q.Where(w => w.ReserveID == reserveId));
            foreach (var item in data)
            {
                item.SupportReadStatus = (int)Chat.ReadStatusEnum.Read;
                Repository.Update(item);
            }
            Repository.Save();
            return data.ToList();
        }

        public void Update(Chat chat)
        {
            var data = Repository.Query(q => q.FirstOrDefault(f => f.Id == chat.Id));
            data.Text = chat.Text;
            data.ChatStatus = chat.ChatStatus;
            Repository.Update(data);
            Repository.Save();
        }

        public void Delete(long chatId)
        {
            Repository.Delete(chatId);
            Repository.Save();
        }

        public void ScheduleChatNotification(long chatId, int targetUserId, bool isGuest,
            int senderUserId, bool isFirstChat)
        {
            var delay = new TimeSpan(0, 0, isFirstChat ? 0 : 5);
            mediator.Schedule(new SendChatNotificationCommand(chatId, targetUserId, isGuest, senderUserId, isFirstChat), delay);
        }
    }
}
