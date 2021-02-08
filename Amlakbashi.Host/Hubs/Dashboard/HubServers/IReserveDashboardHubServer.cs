using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Amlakbashi.Host.Hubs.Dashboard.HubServers
{
    public interface IReserveDashboardHubServer
    {
        void ReloadReserveItemsFromServer(long reserve_id);
        void ReloadChatFromServer(long reserve_id);
    }
}
