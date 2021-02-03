using Amlakbashi.Core.Entities;
using System.Collections.Generic;
using System.Linq;
using static Amlakbashi.Core.Entities.ReserveSupport;

namespace Amlakbashi.Application.Services.ReserveServices.ReserveSupportManager
{
    public interface IReserveSupportManager
    {
        void ReserveAddHandle(long reserveId, SupportStatus? forceStatus = null);
        void ReserveDoneHandle(long reserveId);
        void AddSupporterToReserve(long reserveId, int supporterId, string transferReason = null);
        bool IsInSupporterStatus(long reserveId, SupporterStatus supporterStatus, int yourUserID);
        bool IsInSupporterStatus(Reserve reserve, SupporterStatus supporterStatus, int yourUserID);
        void ReserveCancelAfterDoneHandler(long reserveId);
        IQueryable<Reserve> FilterBySupporterStatus(int yourUserID, IQueryable<Reserve> reserves, SupporterStatus supporterStatus);
        SupporterStatus Analyze(long reserveId, out ReserveSupport currentReserveSupport, int yourUserID = 0);
        SupporterStatus Analyze(Reserve reserve, out ReserveSupport currentReserveSupport, int yourUserID = 0);
        SupporterStatus Analyze(long reserveId, IList<ReserveSupport> supports, out ReserveSupport currentReserveSupport, int yourUserID = 0);
    }
}
