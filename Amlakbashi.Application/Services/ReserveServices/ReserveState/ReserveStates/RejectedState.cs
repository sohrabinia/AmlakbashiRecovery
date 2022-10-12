using Amlakbashi.Core.Common.Extensions;
using Amlakbashi.Core.Common.Repository;
using Amlakbashi.Core.Entities;
using Amlakbashi.Core.Identity.Entities;
using Amlakbashi.Core.Infrastructure.UserContact;
using Amlakbashi.Mediator.Commands.UserCommands;
using MediatR;
using Microsoft.AspNetCore.Identity;
using static Amlakbashi.Core.Entities.Reserve;

namespace Amlakbashi.Application.Services.ReserveServices.ReserveState.ReserveStates
{
    public class RejectedState : ReserveState
    {
        private readonly IMediator mediator;
        private readonly UserManager<AppUser> userManager;
        public RejectedState(
            IRepository<Reserve, long> Repository,
            UserManager<AppUser> userManager,
            IMediator mediator) : base(Repository)
        {
            this.mediator = mediator;
            this.userManager = userManager;
        }

        public override bool CanTransitTo(Reserve.ReserveStatus status)
        {
            return false;
        }

        public override void OnTransition(Reserve.ReserveStatus prevStatus, bool sendSms, ActionLog.ActionSourceEnum actionSource, int doerUserId)
        {
            var reserve = Repository.Find(ReserveId);
            reserve.Status = ReserveStatus.Rejected;
            Repository.Update(reserve);
            Repository.Save();
            if (sendSms == false)
                return;
            var identityUser = userManager.FindByNameAsync(reserve.GuestUser.PhoneNumber).Result;
            mediator.Enqueue(new SendMessageCommand(new UserContactDTO()
            {
                UserMainMobile = reserve.GuestUser.GetNoticesPhoneNumber(),
                UserAppNotificationToken = reserve.GuestUser.AppNotificationToken,
                UserEmail = identityUser.Email,
                EmailConfirmed = identityUser.EmailConfirmed,
                UserFcmAppNotificationToken = reserve.GuestUser.FcmAppNotificationToken,
                UserNotificationToken = reserve.GuestUser.NotificationToken,
                Type = UserContactType.GuestReserveRejected,
                AdvertiseId = reserve.AdvertiseID.ToString(),
                ReserveId = reserve.Id.ToString()
            }));
        }
    }
}
