using Amlakbashi.Application.Services.ReserveServices.ReserveSupportManager;
using Amlakbashi.Core.Common.Extensions;
using Amlakbashi.Core.Common.Repository;
using Amlakbashi.Core.Entities;
using Amlakbashi.Core.Identity.Entities;
using Amlakbashi.Core.Infrastructure.UserContact;
using Amlakbashi.Mediator.Commands.UserCommands;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System;
using static Amlakbashi.Core.Entities.Reserve;

namespace Amlakbashi.Application.Services.ReserveServices.ReserveState.ReserveStates
{
    public class GuestCancelRequestState : ReserveState
    {
        private readonly IMediator mediator;
        private readonly IReserveSupportManager reserveSupportManager;
        private readonly UserManager<AppUser> userManager;
        public GuestCancelRequestState(IRepository<Reserve, long> Repository, IMediator mediator,
            IReserveSupportManager reserveSupportManager, UserManager<AppUser> userManager)
            : base(Repository)
        {
            this.reserveSupportManager = reserveSupportManager;
            this.mediator = mediator;
            this.userManager = userManager;
        }

        public override bool CanTransitTo(ReserveStatus status)
        {
            switch (status)
            {
                case ReserveStatus.Started:
                case ReserveStatus.Reserved:
                case ReserveStatus.CashPay:
                case ReserveStatus.CanceledByGuest:
                    return true;
                default:
                    return false;
            }
        }

        public override void OnTransition(ReserveStatus prevStatus, bool sendSms, ActionLog.ActionSourceEnum actionSource, int doerUserId)
        {
            var reserve = Repository.Find(ReserveId);
            reserve.Status = ReserveStatus.CancelRequestByGuest;
            reserve.CancelDate = DateTime.Now;
            if (StatusIsCanceled(prevStatus) == false)
            {
                reserve.CancelState = prevStatus;
            }
            Repository.Update(reserve);
            Repository.Save();
            var user = Repository.Find<User, int>(reserve.HostUserID);
            var identityUser = userManager.FindByNameAsync(user.MainMobile).Result;
            var contact = new UserContactDTO()
            {
                UserMainMobile = user.MainMobile,
                UserAppNotificationToken = user.AppNotificationToken,
                UserEmail = identityUser.Email,
                EmailConfirmed = identityUser.EmailConfirmed,
                UserFcmAppNotificationToken = user.FcmAppNotificationToken,
                UserNotificationToken = user.NotificationToken,
                Type = UserContactType.HostCancelRequestSent,
                ReserveId = reserve.Id.ToString(),
                AdvertiseId = reserve.AdvertiseID.ToString()
            };
            if (sendSms)
            {
                mediator.Enqueue(new SendMessageCommand(contact));
            }
            reserveSupportManager.ReserveCancelAfterDoneHandler(ReserveId);
        }
    }
}
