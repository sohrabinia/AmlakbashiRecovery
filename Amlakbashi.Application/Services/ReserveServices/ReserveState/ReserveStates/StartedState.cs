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
    public class StartedState : ReserveState
    {
        private readonly IMediator mediator;
        private readonly UserManager<AppUser> userManager;
        public StartedState(
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
                case ReserveStatus.Completed:
                case ReserveStatus.CancelRequestByGuest:
                case ReserveStatus.CanceledByGuest:
                case ReserveStatus.CanceledByHost:
                case ReserveStatus.CancelRequestByHost:
                    return true;
                default:
                    return false;
            }
        }

        public override void OnTransition(Reserve.ReserveStatus prevStatus, bool sendSms, ActionLog.ActionSourceEnum actionSource, int doerUserId)
        {
            var reserve = Repository.Find(ReserveId);
            reserve.Status = ReserveStatus.Started;
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
                    EmailConfirmed = identityUser.EmailConfirmed,
                    UserFcmAppNotificationToken = reserve.GuestUser.FcmAppNotificationToken,
                    UserNotificationToken = reserve.GuestUser.NotificationToken,
                    Type = UserContactType.GuestStayStarted,
                    AdvertiseId = reserve.AdvertiseID.ToString(),
                    ReserveId = reserve.Id.ToString()
                };
                mediator.Enqueue(new SendMessageCommand(contact));
            }
        }
    }
}
