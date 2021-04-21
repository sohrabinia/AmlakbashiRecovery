using Amlakbashi.Core.Common.Extensions;
using Amlakbashi.Core.Common.Repository;
using Amlakbashi.Core.Entities;
using Amlakbashi.Core.Infrastructure.UserContact;
using Amlakbashi.Mediator.Commands.UserCommands;
using MediatR;
using static Amlakbashi.Core.Entities.Reserve;

namespace Amlakbashi.Application.Services.ReserveServices.ReserveState.ReserveStates
{
    public class RejectedState : ReserveState
    {
        private readonly IMediator mediator;
        public RejectedState(
            IRepository<Reserve, long> Repository,
            IMediator mediator) : base(Repository)
        {
            this.mediator = mediator;
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
            var contact = new UserContactDTO()
            {
                UserMainMobile = reserve.GuestUser.MainMobile,
                UserAppNotificationToken = reserve.GuestUser.AppNotificationToken,
                UserEmail = reserve.GuestUser.Email,
                UserFcmAppNotificationToken = reserve.GuestUser.FcmAppNotificationToken,
                UserNotificationToken = reserve.GuestUser.NotificationToken,
                Type = UserContactType.GuestReserveRejected,
                AdvertiseId = reserve.AdvertiseID.ToString(),
                ReserveId = reserve.Id.ToString()
            };
            mediator.Enqueue(new SendMessageCommand(contact));
        }
    }
}
