using Microsoft.AspNetCore.SignalR;

namespace Amlakbashi.Host.Hubs.Dashboard.HubServers
{
    public class ReserveDashboardHubServer : IReserveDashboardHubServer
    {
        private readonly IHubContext<ReserveDashboardHub> hubContext;
        public ReserveDashboardHubServer(IHubContext<ReserveDashboardHub> hubContext)
        {
            this.hubContext = hubContext;
        }

        public void ReloadReserveItemsFromServer(long reserve_id)
        {
            hubContext.Clients.All.SendAsync("reloadReserveItem", reserve_id);
        }

        public void ReloadChatFromServer(long reserve_id)
        {
            hubContext.Clients.All.SendAsync("reloadChat", reserve_id);
        }
    }
}
