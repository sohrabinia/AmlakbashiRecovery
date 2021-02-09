using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNetCore.SignalR;

namespace Amlakbashi.Host.Hubs.Admin
{
    public class SupportChatAdminHub : Hub
    {
        public void AddChatMessage(long supportChatId, long messageId)
        {
            Clients.All.SendAsync("addChatMessage", supportChatId, messageId);
        }

        public void UpdateChatMessage(long supportChatId, long messageId)
        {
            Clients.All.SendAsync("updateChatMessage", supportChatId, messageId);
        }
    }
}