using Amlakbashi.Accounting;
using Amlakbashi.Core.Common.Extensions;
using Amlakbashi.Core.Common.Repository;
using Amlakbashi.Core.Entities;
using Amlakbashi.Core.Identity.Entities;
using Amlakbashi.Core.Infrastructure.UserContact;
using Amlakbashi.Core.Infrastructure.UserContact.Interfaces;
using Amlakbashi.Mediator.Commands.AdvertiseCommands;
using Amlakbashi.Mediator.Commands.UserCommands;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System;
using static Amlakbashi.Core.Entities.Reserve;

namespace Amlakbashi.Application.Services.ReserveServices.ReserveState.ReserveStates
{
    public class GuestCancelState : ReserveState
    {
        private readonly IMediator mediator;
        private readonly IAccountingFacade accounting;
        private readonly UserManager<AppUser> userManager;
        public GuestCancelState(IRepository<Reserve, long> Repository,
            IAccountingFacade accounting,
            UserManager<AppUser> userManager,
            IMediator mediator) : base(Repository)
        {
            this.mediator = mediator;
            this.accounting = accounting;
            this.userManager = userManager;
        }

        public override bool CanTransitTo(ReserveStatus status)
        {
            return false;
        }

        public override void OnTransition(Reserve.ReserveStatus prevStatus,
            bool sendSms, ActionLog.ActionSourceEnum actionSource, int doerUserId)
        {
            var reserve = Repository.Find(ReserveId);
            reserve.Status = ReserveStatus.CanceledByGuest;
            if (reserve.CancelDate == null)
                reserve.CancelDate = DateTime.Now;
            if (StatusIsCanceled(prevStatus) == false &&
                StatusIsCanceling(prevStatus) == false)
            {
                reserve.CancelState = prevStatus;
            }
            Repository.Update(reserve);
            Repository.Save();
            accounting.RefundCouponIfAny(reserve.Id);
            accounting.RefundPrizeCreditIfAny(reserve.Id);
            if (sendSms)
            {
                var user = Repository.Find<User, int>(reserve.HostUserID);
                var hostIdentityUser = userManager.FindByNameAsync(user.PhoneNumber).Result;
                mediator.Enqueue(new SendMessageCommand(new UserContactDTO()
                {
                    UserMainMobile = user.GetNoticesPhoneNumber(),
                    UserAppNotificationToken = user.AppNotificationToken,
                    UserEmail = hostIdentityUser.Email,
                    EmailConfirmed = hostIdentityUser.EmailConfirmed,
                    UserFcmAppNotificationToken = user.FcmAppNotificationToken,
                    UserNotificationToken = user.NotificationToken,
                    Type = UserContactType.GuestReserveCanceled,
                    AdvertiseId = reserve.AdvertiseID.ToString(),
                    ReserveId = reserve.Id.ToString()
                }));
            }
            mediator.Enqueue(new UpdateAdvertiseScoreCommand(reserve.AdvertiseID));
        }
    }
}
