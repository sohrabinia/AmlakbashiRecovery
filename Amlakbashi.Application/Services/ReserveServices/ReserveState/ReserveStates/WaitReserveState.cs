using Amlakbashi.Core.Common.Extensions;
using Amlakbashi.Core.Common.Repository;
using Amlakbashi.Core.Entities;
using Amlakbashi.Core.Infrastructure.UserContact;
using Amlakbashi.Mediator.Commands.AdvertiseCommands;
using Amlakbashi.Mediator.Commands.UserCommands;
using MediatR;
using static Amlakbashi.Core.Entities.Reserve;

namespace Amlakbashi.Application.Services.ReserveServices.ReserveState.ReserveStates
{
    public class WaitReserveState : ReserveState
    {
        private readonly IMediator mediator;
        public WaitReserveState(
            IMediator mediator,
            IRepository<Reserve, long> Repository) : base(Repository)
        {
            this.mediator = mediator;
        }

        public override bool CanTransitTo(Reserve.ReserveStatus status)
        {
            switch (status)
            {
                case Reserve.ReserveStatus.Reserved:
                case Reserve.ReserveStatus.CashPay:
                case Reserve.ReserveStatus.Started:
                case Reserve.ReserveStatus.Completed:
                case Reserve.ReserveStatus.CanceledByGuest:
                case Reserve.ReserveStatus.CanceledBySystem:
                case Reserve.ReserveStatus.CanceledByHost:
                    return true;
                default:
                    return false;
            }
        }

        public override void OnTransition(Reserve.ReserveStatus prevStatus, bool sendSms, ActionLog.ActionSourceEnum actionSource, int doerUserId)
        {
            var reserve = Repository.Find(ReserveId);
            reserve.Status = ReserveStatus.WaitForReserve;
            Repository.Update(reserve);
            Repository.Save();
            if (sendSms)
            {
                var guestUser = reserve.GuestUser;
                var contact = new UserContactDTO()
                {
                    UserLoginPriority = guestUser.LoginPriority,
                    UserMainMobile = guestUser.MainMobile,
                    UserAppNotificationToken = guestUser.AppNotificationToken,
                    UserEmail = guestUser.Email,
                    UserFcmAppNotificationToken = guestUser.FcmAppNotificationToken,
                    UserNotificationToken = guestUser.NotificationToken,
                    Type = UserContactType.GuestPayReserve,
                    AdvertiseId = reserve.AdvertiseID.ToString(),
                    UserId = guestUser.Id.ToString(),
                    ReserveId = reserve.Id.ToString()
                };
                mediator.Enqueue(new SendMessageCommand(contact));
            }
            mediator.Send(new ScheduleReservePaymentCommand(ReserveId));
        }
    }
}
