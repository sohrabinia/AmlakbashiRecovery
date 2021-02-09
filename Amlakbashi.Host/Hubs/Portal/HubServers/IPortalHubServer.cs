using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Amlakbashi.Host.Hubs.Portal.HubServers
{
    public interface IPortalHubServer
    {
        void ReloadSupportChatFromServer(long supportChatId, int newCount, int userId);
    }
}
