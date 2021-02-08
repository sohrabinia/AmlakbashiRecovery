using Microsoft.AspNetCore.SignalR;
using Amlakbashi.Core.Entities;
using Amlakbashi.Core.Infrastructure.StyleHelpers;
using Amlakbashi.Core.Infrastructure.LocalizationHelpers;

namespace Amlakbashi.Host.Hubs.Admin
{
    public class ReserveAdminHub : Hub
    {
        public void AddSupporterInfoToReserve(long reserve_id, string text)
        {
            Clients.All.SendAsync("addSupporterInfo", reserve_id, text);
        }
        public void PayReserveWithCreditHost(long reserve_id)
        {
            Clients.All.SendAsync("payReserveWithCreditHost", reserve_id);
        }
        public void ReserveCleared(long reserve_id)
        {
            Clients.All.SendAsync("reserveCleared", reserve_id);
        }
        public void ReserveRefunded(long reserve_id)
        {
            Clients.All.SendAsync("reserveRefunded", reserve_id);
        }
        public void ReserveSupporterAdded(long reserve_id,
            string supporterName, string supporterPhoto)
        {
            Clients.All.SendAsync("reserveSupporterAdded", reserve_id,
                supporterName, supporterPhoto);
        }
        public void ToggleShouldFollow(long reserve_id, bool new_status)
        {
            Clients.All.SendAsync("toggleShouldFollow",reserve_id,
                new_status);
        }
        public void ChangeCallState(long reserve_id, string hostOrGuest, int new_state, string new_state_color)
        {
            Clients.All.SendAsync("changeCallState",reserve_id,
                hostOrGuest, new_state, new_state_color);
        }
        public void CancelReserve(long reserve_id)
        {
            ChangeStatus(reserve_id,
                (int)Reserve.ReserveStatus.CanceledBySystem);
        }
        public void ChangeStatus(long reserve_id, int status)
        {
            Clients.All.SendAsync("changeStatus",reserve_id,
                ReserveLocalization.GetStatusString(
                    status, Reserve.StatusStringType.Site, reserve_id),
                ReserveStyleHelper.GetStatusColor(status)
            );
        }
        public void DeleteReserve(long reserve_id)
        {
            Clients.All.SendAsync("deleteReserve",reserve_id);
        }
    }
}