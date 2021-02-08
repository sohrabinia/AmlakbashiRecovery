using Amlakbashi.Host.Hubs.Dashboard;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Amlakbashi.Host.Hubs.Portal.HubServers
{
    public class PortalHubServer
    {
        private readonly IHubContext<PortalHub> hubContext;
        public PortalHubServer(IHubContext<PortalHub> hubContext)
        {
            this.hubContext = hubContext;
        }

        public void ReloadSupportChatFromServer(long supportChatId, int newCount, int userId)
        {
            hubContext.Clients.All.SendAsync("reloadSupportChat", supportChatId, newCount, userId);
        }
    }
}
