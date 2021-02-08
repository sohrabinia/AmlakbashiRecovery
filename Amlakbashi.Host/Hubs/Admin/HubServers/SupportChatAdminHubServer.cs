using Microsoft.AspNetCore.SignalR;
using Portal.Hubs.Admin;

namespace Amlakbashi.Host.Hubs.Admin.HubServers
{
    public class SupportChatAdminHubServer
    {
        private readonly IHubContext<SupportChatAdminHub> hubContext;
        public SupportChatAdminHubServer(IHubContext<SupportChatAdminHub> hubContext)
        {
            this.hubContext = hubContext;
        }

        public void AddChatMessageFromServer(long supportChatId, long messageId)
        {
            hubContext.Clients.All.SendAsync("addChatMessage", supportChatId, messageId);
        }

        public void UpdateChatMessageFromServer(long supportChatId, long messageId)
        {
            hubContext.Clients.All.SendAsync("updateChatMessage", supportChatId, messageId);
        }
    }
}
