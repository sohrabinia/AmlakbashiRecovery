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
    public class HostCancelState : ReserveState
    {
        private readonly IMediator mediator;
        private readonly IAccountingFacade accounting;
        private readonly UserManager<AppUser> userManager;
        public HostCancelState(IRepository<Reserve, long> Repository,
            IAccountingFacade accounting,
            IMediator mediator,
            UserManager<AppUser> userManager
            ) : base(Repository)
        {
            this.mediator = mediator;
            this.accounting = accounting;
            this.userManager = userManager;
        }

        public override bool CanTransitTo(ReserveStatus status)
        {
            return false;
        }

        public override void OnTransition(Reserve.ReserveStatus prevStatus, bool sendSms, ActionLog.ActionSourceEnum actionSource, int doerUserId)
        {
            var reserve = Repository.Find(ReserveId);
            reserve.Status = ReserveStatus.CanceledByHost;
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
                var identityUser = userManager.FindByNameAsync(reserve.GuestUser.PhoneNumber).Result;
                mediator.Enqueue(new SendMessageCommand(new UserContactDTO()
                {
                    UserMainMobile = reserve.GuestUser.GetNoticesPhoneNumber(),
                    UserAppNotificationToken = reserve.GuestUser.AppNotificationToken,
                    UserEmail = identityUser.Email,
                    EmailConfirmed = identityUser.EmailConfirmed,
                    UserFcmAppNotificationToken = reserve.GuestUser.FcmAppNotificationToken,
                    UserNotificationToken = reserve.GuestUser.NotificationToken,
                    Type = UserContactType.HostReserveCanceled,
                    AdvertiseId = reserve.AdvertiseID.ToString(),
                    ReserveId = reserve.Id.ToString()
                }));
            }
            mediator.Enqueue(new UpdateUserScoreCommand(reserve.Advertise.UserID));
            mediator.Enqueue(new UpdateAdvertiseScoreCommand(reserve.AdvertiseID));
        }
    }
}
