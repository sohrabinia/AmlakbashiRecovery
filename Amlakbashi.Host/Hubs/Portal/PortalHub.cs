using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNetCore.SignalR;

namespace Amlakbashi.Host.Hubs.Portal
{
    public class PortalHub : Hub
    {
        public void ReloadSupportChat(long supportChatId, int newCount, int userId)
        {
            Clients.All.SendAsync("reloadSupportChat", supportChatId, newCount, userId);
        }
    }
}