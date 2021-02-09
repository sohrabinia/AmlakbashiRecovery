using Amlakbashi.Core.Infrastructure.LocalizationHelpers;
using Amlakbashi.Core.Infrastructure.StyleHelpers;
using Microsoft.AspNetCore.SignalR;
using static Amlakbashi.Core.Entities.Reserve;

namespace Amlakbashi.Host.Hubs.Admin.HubServers
{
    public class ReserveAdminHubServer : IReserveAdminHubServer
    {
        private readonly IHubContext<ReserveAdminHub> hubContext;
        public ReserveAdminHubServer(IHubContext<ReserveAdminHub> hubContext)
        {
            this.hubContext = hubContext;
        }
        public void ChangeStatusFromServer(long reserve_id,
            ReserveStatus status, HostResponseEnum hostResponse)
        {
            hubContext.Clients.All.SendAsync("changeStatus", reserve_id,
                ReserveLocalization.GetStatusString(
                    (int)status, StatusStringType.Site, reserve_id, hostResponse),
                ReserveStyleHelper.GetStatusColor((int)status));
        }

        public void ChatReadFromServer(long reserve_id, int count)
        {
            hubContext.Clients.All.SendAsync("chatRead",reserve_id, count);
        }

        public void ChangeChatCountFromServer(long reserve_id, int count, int notReadCount)
        {
            hubContext.Clients.All.SendAsync("changeChatCount", reserve_id, count, notReadCount);
        }
    }
}
