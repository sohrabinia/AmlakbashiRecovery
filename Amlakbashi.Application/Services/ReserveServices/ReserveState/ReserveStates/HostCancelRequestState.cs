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
    public class HostCancelRequestState : ReserveState
    {
        private readonly IMediator mediator;
        private readonly IReserveSupportManager reserveSupportManager;
        private readonly UserManager<AppUser> userManager;
        public HostCancelRequestState(IRepository<Reserve, long> Repository,
            IMediator mediator,
            UserManager<AppUser> userManager,
            IReserveSupportManager reserveSupportManager
            ) :
            base(Repository)
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
                case ReserveStatus.CanceledByHost:
                    return true;
                default:
                    return false;
            }
        }

        public override void OnTransition(ReserveStatus prevStatus,
            bool sendSms, ActionLog.ActionSourceEnum actionSource, int doerUserId)
        {
            var reserve = Repository.Find(ReserveId);
            reserve.Status = ReserveStatus.CancelRequestByHost;
            reserve.CancelDate = DateTime.Now;
            if (StatusIsCanceled(prevStatus) == false)
            {
                reserve.CancelState = prevStatus;
            }
            Repository.Update(reserve);
            Repository.Save();
            if (sendSms)
            {
                var identityUser = userManager.FindByNameAsync(reserve.GuestUser.MainMobile).Result;
                var contact = new UserContactDTO()
                {
                    UserMainMobile = reserve.GuestUser.MainMobile,
                    UserAppNotificationToken = reserve.GuestUser.AppNotificationToken,
                    UserEmail = identityUser.Email,
                    UserFcmAppNotificationToken = reserve.GuestUser.FcmAppNotificationToken,
                    UserNotificationToken = reserve.GuestUser.NotificationToken,
                    Type = UserContactType.GuestCancelRequestSent,
                    ReserveId = reserve.Id.ToString(),
                    AdvertiseId = reserve.AdvertiseID.ToString()
                };
                mediator.Enqueue(new SendMessageCommand(contact));
            }
            reserveSupportManager.ReserveCancelAfterDoneHandler(ReserveId);
        }
    }
}
