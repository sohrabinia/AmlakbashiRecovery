using Amlakbashi.Core.Entities;
using System.Collections.Generic;

namespace Amlakbashi.Application.Services.SupportChatServices.Interfaces
{
    public interface ISupportChatAppService
    {
        IList<SupportChat> GetLastItems(int count, int currentItemCount = 0);
        SupportChat Find(long id);
        SupportChat GetByUserId(int userId);
        SupportChat Insert(int userId);
        IList<long> UpdateMessagesReadStatus(long id, SupportChatMessage.TypeEnum type = SupportChatMessage.TypeEnum.Supporter);
        void ScheduleSendSupporterNewMsgNotif(int delay, long messageId, long supportChatId, string[] supportersNotifToken);
    }
}
