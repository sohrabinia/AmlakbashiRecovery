using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Amlakbashi.Host.Hubs.Portal.HubServers
{
    interface IPortalHubServer
    {
        void ReloadSupportChat(long supportChatId, int newCount, int userId);
    }
}
