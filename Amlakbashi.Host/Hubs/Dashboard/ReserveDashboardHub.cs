using Microsoft.AspNetCore.SignalR;

namespace Amlakbashi.Host.Hubs.Dashboard
{
    public class ReserveDashboardHub : Hub
    {
        public void ReloadReserveItem(long reserve_id)
        {
            Clients.All.SendAsync("reloadReserveItem", reserve_id);
        }

        public void ReloadChat(long reserve_id)
        {
            Clients.All.SendAsync("reloadChat", reserve_id);
        }

    }
}