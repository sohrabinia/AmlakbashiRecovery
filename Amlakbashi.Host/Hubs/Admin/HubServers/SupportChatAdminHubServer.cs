using Microsoft.AspNetCore.SignalR;

namespace Amlakbashi.Host.Hubs.Admin.HubServers
{
    public class SupportChatAdminHubServer : ISupportChatAdminHubServer
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
