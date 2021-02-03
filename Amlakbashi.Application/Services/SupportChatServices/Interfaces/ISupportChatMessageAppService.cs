using Amlakbashi.Core.Common.AppService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amlakbashi.Core.Entities;
using static Amlakbashi.Core.Entities.SupportChatMessage;

namespace Amlakbashi.Application.Services.SupportChatServices.Interfaces
{
    public interface ISupportChatMessageAppService : IAppService<SupportChatMessage, long>
    {
        SupportChatMessage Find(long id);
        long Insert(string text, TypeEnum type,
            long supportChatId, int? userId, ReadStatusEnum initialRead = ReadStatusEnum.NotRead);
        void UpdateReadStatus(long id);
        void UpdateReadStatusList(IList<long> listId);
    }
}
