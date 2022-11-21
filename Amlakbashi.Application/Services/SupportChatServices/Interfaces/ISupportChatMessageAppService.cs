using System.Collections.Generic;
using Amlakbashi.Core.Entities;

namespace Amlakbashi.Application.Services.SupportChatServices.Interfaces
{
    public interface ISupportChatMessageAppService
    {
        SupportChatMessage Find(long id);
        long Insert(string text, SupportChatMessage.TypeEnum type, long supportChatId, int? userId,
            SupportChatMessage.ReadStatusEnum initialRead = SupportChatMessage.ReadStatusEnum.NotRead);
        void UpdateReadStatus(long id);
        void UpdateReadStatusList(IList<long> listId);
    }
}
