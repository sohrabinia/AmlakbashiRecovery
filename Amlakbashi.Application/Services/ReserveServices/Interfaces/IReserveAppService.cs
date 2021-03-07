using Amlakbashi.Core.Common.AppService;
using Amlakbashi.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Amlakbashi.Core.Entities.ActionLog;
using static Amlakbashi.Core.Entities.Reserve;
using static Amlakbashi.Core.Entities.ReservePayment;

namespace Amlakbashi.Application.Services.ReserveServices.Interfaces
{
    public interface IReserveAppService : IAppService<Reserve, long>
    {
        IList<Reserve> Filter(long reserve_id = -1, long advertise_id = -1,
            int host_user_id = -1, int guest_user_id = -1, int reserve_status = -1,
            int host_response_status = -1, int general_status = -1,
            string site_clearing_date = "", int site_cleared_status = -1,
            string reserve_from_date = "", string reserve_to_date = "",
            string reserve_end_date = "", int stay_duration_from = -1, int stay_duration_to = -1,
            int reserve_support_status = 0, bool shouldFollow = false,
            int supporter_id = -1, int host_card_status = -1,
            int mainFilter = 0, int instantReserveFilter = 2,
            bool disableAutoCancel = false, bool accVisited = false);
        IList<Reserve> GetListByUserId(int userId, bool isHost = false);
        IList<Reserve> GetListByUserId(int userId, int category, bool isHost = false);
        IList<Reserve> GetListByUserId(int userId, Reserve.ReserveStatus status, bool RatingShownToGuest,
            bool isHost = false);
        IList<Reserve> GetListByUserId(int userId,
            Reserve.ReserveManagerSelectType selectType = Reserve.ReserveManagerSelectType.All);
        Reserve Find(long id);
        Reserve GetReserveIncludingSupport(long id);
        IQueryable<Reserve> GetReservesIncludingSupport(List<long> ids);
        IList<Reserve> Find(IEnumerable<long> ids);
        IList<Reserve> GetByUserId(int userId);
        Reserve FirstHavingUserId(int userId, Reserve.ReserveStatus status);
        Reserve GetRelatedReserveByUser(int userId, out bool isHost);
        bool Update(Reserve reserve, string start_date,
            string end_date, out string msg, int doerUserId,
            ActionSourceEnum actionSource);
        void SetStatus(long reserveId, ReserveStatus status, bool sendSms,
            ActionLog.ActionSourceEnum actionSource, int doerUserId);
        bool SetHostResponse(long reserveId, HostResponseEnum response,
            bool sendSms, ActionLog.ActionSourceEnum actionSource, int doerUserId);
        bool CashPay(long reserveId, out string msg,
            int userId, ActionSourceEnum actionSource, int doerUserId);
        bool ConfirmCashPay(long reserveId, bool paid, out string msg,
            int userId, ActionSourceEnum actionSource, int doerUserId);
        void CancelReserve(User user, long reserve_id, int cancel_reason_code,
            string cancel_reason_string, bool is_host, out string msg,
            out bool isPending, ActionSourceEnum actionSource, int doerUserId);
        void RefuseCancelReserve(User user, long reserve_id, bool is_host, out string msg,
            ActionSourceEnum actionLog, int doerUserId);
        void UpdateShouldFollow(long id, string text, User user);
        void UpdateSupporterInfo(long id, string text, User user);
        void UpdateRatingShownToGuest(long id, bool showRate);
        void UpdatePaymentGTAGRegistered(long id, bool value);
        int UpdateCallState(long id, string hostOrGuest);
        bool StartStay(long reserveId, int user_id, out string msg,
            ActionSourceEnum actionSource, int doerUserId);
        void UpdateAccVisitedByGuest(long id, bool value);
        void UpdateDisableAutoCancel(long id, bool value);
        void UpdateExcludeGroup(long id, bool value);
        void UpdateHostCallDate(long id, DateTime value);
        void UpdateGuestCallDate(long id, DateTime value);
        void UpdatePaymentHasError(long id, bool value);
        void UpdatePaymentHasError(IList<long> ids, bool value);
        void UpdateCanselDiscussion(long id, string text, User user);
        bool Delete(long id, out string msg);
        void ExistHostGuest(int userId, out bool hasHost, out bool hasGuest);
        bool UserHasRefundInProgress(int userId);
        bool UserHasSimilarReserve(int userId, long advertiseId, DateTime startDate, DateTime endDate);
        bool CanReserveStarted(long reserveId, out DateTime canStartTime);
        bool FinishStay(long reserveId, int userId, out string msg, ActionSourceEnum actionSource,
            int doerUserId, bool sendSms = true);
        ReserveStatus FinalizeReserve(long reserveId, long transactionId,
            long paidAmount, ReservePaymentMethod paymentMethod,
            ActionSourceEnum actionSource, int doerUserId,
            int payerUserId = -1, long couponId = 0, long prizePrice = 0,
            bool sendSms = true);
        bool SystemCancelReserve(long reserveId);
        IList<Reserve> GetReserveDashboardItems(
            User currentUser, ReserveManagerSelectType selectType,
            int category, string reserve_id, int status,
            out Dictionary<ReserveCategory, int> countDict);
        void SetHangfireSchedules_GuestCall();
        void SetHangfireSchedules_HostCall();
        void SetHangfireSchedules_ReservedState();
        void SetHangfireSchedules_StartedState();
    }
}
