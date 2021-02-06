using Amlakbashi.Core.Common.AppService;
using Amlakbashi.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amlakbashi.Application.Services.SupportChatServices.Interfaces
{
    public interface ISupportChatAppService : IAppService<SupportChat, long>
    {
        IList<SupportChat> GetLastItems(int count, int currentItemCount = 0);
        SupportChat Find(long id);
        SupportChat GetByUserId(int userId);
        SupportChat Insert(int userId);
        void ScheduleSendSupporterNewMsgNotif(int delay, long messageId, long supportChatId);
    }
}
