using static Amlakbashi.Core.Entities.Reserve;

namespace Amlakbashi.Host.Hubs.Admin.HubServers
{
    public interface IReserveAdminHubServer
    {
        void ChangeStatusFromServer(long reserve_id, ReserveStatus status, HostResponseEnum hostResponse);
        void ChatReadFromServer(long reserve_id, int count);
        void ChangeChatCountFromServer(long reserve_id, int count, int notReadCount);
    }
}
