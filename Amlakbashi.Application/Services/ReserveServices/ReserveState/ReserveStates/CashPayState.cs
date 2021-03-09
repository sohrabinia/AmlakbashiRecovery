using Amlakbashi.Core.Common.Extensions;
using Amlakbashi.Core.Common.Repository;
using Amlakbashi.Core.Entities;
using Amlakbashi.Core.Infrastructure.UserContact;
using Amlakbashi.Mediator.Commands.UserCommands;
using MediatR;
using static Amlakbashi.Core.Entities.Reserve;

namespace Amlakbashi.Application.Services.ReserveServices.ReserveState.ReserveStates
{
    public class CashPayState : ReserveState
    {
        private readonly IMediator mediator;
        public CashPayState(IRepository<Reserve, long> Repository,
            IMediator mediator) : base(Repository)
        {
            this.mediator = mediator;
        }

        public override bool CanTransitTo(ReserveStatus status)
        {
            switch (status)
            {
                case ReserveStatus.Started:
                case ReserveStatus.Completed:
                case ReserveStatus.CancelRequestByGuest:
                case ReserveStatus.CancelRequestByHost:
                case ReserveStatus.CanceledByGuest:
                case ReserveStatus.CanceledByHost:
                case ReserveStatus.Reserved:
                    return true;
                default:
                    return false;
            }
        }

        public override void OnTransition(ReserveStatus prevStatus, bool sendSms,
            ActionLog.ActionSourceEnum actionSource, int doerUserId)
        {
            var reserve = Repository.Find(ReserveId);
            reserve.Status = ReserveStatus.CashPay;
            Repository.Update(reserve);
            Repository.Save();
            if (sendSms)
            {
                var hostlerUser = Repository.Find<User, int>(reserve.Advertise.UserID);
                var contact = new UserContactDTO()
                {
                    UserLoginPriority = hostlerUser.LoginPriority,
                    UserMainMobile = hostlerUser.MainMobile,
                    UserAppNotificationToken = hostlerUser.AppNotificationToken,
                    UserEmail = hostlerUser.Email,
                    UserFcmAppNotificationToken = hostlerUser.FcmAppNotificationToken,
                    UserNotificationToken = hostlerUser.NotificationToken,
                    Type = UserContactType.HostReserveCashPay,
                    AdvertiseId = reserve.AdvertiseID.ToString(),
                    UserId = hostlerUser.Id.ToString(),
                    ReserveId = reserve.Id.ToString()
                };
                mediator.Enqueue(new SendMessageCommand(contact));
            }
        }
    }
}
