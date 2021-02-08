using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Amlakbashi.Host.Hubs.Admin.HubServers
{
    public interface ISupportChatAdminHubServer
    {
        void AddChatMessageFromServer(long supportChatId, long messageId);
        void UpdateChatMessageFromServer(long supportChatId, long messageId);
    }
}
