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
            return false;
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
                var identityUser = userManager.FindByNameAsync(reserve.GuestUser.PhoneNumber).Result;
                mediator.Enqueue(new SendMessageCommand(new UserContactDTO()
                {
                    UserMainMobile = reserve.GuestUser.GetNoticesPhoneNumber(),
                    UserAppNotificationToken = reserve.GuestUser.AppNotificationToken,
                    UserEmail = identityUser.Email,
                    EmailConfirmed = identityUser.EmailConfirmed,
                    UserFcmAppNotificationToken = reserve.GuestUser.FcmAppNotificationToken,
                    UserNotificationToken = reserve.GuestUser.NotificationToken,
                    Type = UserContactType.GuestCancelRequestSent,
                    ReserveId = reserve.Id.ToString(),
                    AdvertiseId = reserve.AdvertiseID.ToString()
                }));
            }
            reserveSupportManager.ReserveCancelAfterDoneHandler(ReserveId);
        }
    }
}
