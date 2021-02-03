using Amlakbashi.Core.Common.AppService;
using Amlakbashi.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amlakbashi.Application.Services.ReserveServices.Interfaces
{
    public interface IChatAppService : IAppService<Chat, long>
    {
        IList<Chat> Filter(long chat_id = -1, long reserve_id = -1,
            int user_id = -1, int chat_status = -1);
        IQueryable<Chat> GetAllAsIqueriable();
        IList<Chat> GetReserveChats(long reserveId);
        IList<Chat> GetListAgainstUserId(int userId, Chat.ChatStatusEnum status, Chat.ReadStatusEnum read,
            IList<long> reserveIds = null);
        Chat Find(int id);
        int GetCountByReserveId(long reserveId);
        int GetNotReadCountByReserveId(long reserveId, int userId);
        int GetNotReadSupportCountByReserveId(long reserveId);
        Chat Insert(Chat chat);
        void UpdateChatListReadStatus(IList<Chat> chats);
        IList<Chat> UpdateSupportReadStatusByReserveId(long reserveId);
        void Update(Chat chat);
        void Delete(long chatId);
        void ScheduleChatNotification(long chatId, int targetUserId, bool isGuest,
            int senderUserId, bool isFirstChat);
    }
}
