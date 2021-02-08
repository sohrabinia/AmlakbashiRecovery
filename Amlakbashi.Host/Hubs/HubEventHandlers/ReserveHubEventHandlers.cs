using Amlakbashi.Host.Hubs.Admin;
using Amlakbashi.Host.Hubs.Admin.HubServers;
using Amlakbashi.Host.Hubs.Dashboard;
using Amlakbashi.Host.Hubs.Dashboard.HubServers;
using Amlakbashi.Mediator.Events.ReserveEvents;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Amlakbashi.Host.Hubs.HubEventHandlers
{
    public class ReserveHubEventHandlers : INotificationHandler<ChangeReserveStateEvent>
    {
        private readonly IReserveAdminHubServer reserveAdminHubServer;
        private readonly IReserveDashboardHubServer reserveDashboardHubServer;
        public ReserveHubEventHandlers(IReserveAdminHubServer reserveAdminHubServer,
            IReserveDashboardHubServer reserveDashboardHubServer)
        {
            this.reserveAdminHubServer = reserveAdminHubServer;
            this.reserveDashboardHubServer = reserveDashboardHubServer;
        }
        public Task Handle(ChangeReserveStateEvent notification, CancellationToken cancellationToken)
        {
            reserveAdminHubServer.ChangeStatusFromServer(notification.reserveId,
                notification.reserveStatus, notification.hostResponse);
            reserveDashboardHubServer.ReloadReserveItemsFromServer(notification.reserveId);
            return Task.CompletedTask;
        }
    }
}