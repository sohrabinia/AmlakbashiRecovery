using Amlakbashi.Core.Common.Extensions;
using Amlakbashi.Core.Common.Repository;
using Amlakbashi.Core.Entities;
using Amlakbashi.Core.Identity.Entities;
using Amlakbashi.Core.Infrastructure.UserContact;
using Amlakbashi.Mediator.Commands.AdvertiseCommands;
using Amlakbashi.Mediator.Commands.UserCommands;
using MediatR;
using Microsoft.AspNetCore.Identity;
using static Amlakbashi.Core.Entities.Reserve;

namespace Amlakbashi.Application.Services.ReserveServices.ReserveState.ReserveStates
{
    public class WaitReserveState : ReserveState
    {
        private readonly IMediator mediator;
        private readonly UserManager<AppUser> userManager;
        public WaitReserveState(
            IMediator mediator,
            UserManager<AppUser> userManager,
            IRepository<Reserve, long> Repository) : base(Repository)
        {
            this.mediator = mediator;
            this.userManager = userManager;
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
                var identityUser = userManager.FindByNameAsync(guestUser.MainMobile).Result;
                var contact = new UserContactDTO()
                {
                    UserMainMobile = guestUser.MainMobile,
                    UserAppNotificationToken = guestUser.AppNotificationToken,
                    UserEmail = identityUser.Email,
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
