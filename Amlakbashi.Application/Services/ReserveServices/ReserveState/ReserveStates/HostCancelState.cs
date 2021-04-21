using Amlakbashi.Accounting;
using Amlakbashi.Core.Common.Extensions;
using Amlakbashi.Core.Common.Repository;
using Amlakbashi.Core.Entities;
using Amlakbashi.Core.Infrastructure.UserContact;
using Amlakbashi.Core.Infrastructure.UserContact.Interfaces;
using Amlakbashi.Mediator.Commands.AdvertiseCommands;
using Amlakbashi.Mediator.Commands.UserCommands;
using MediatR;
using System;
using static Amlakbashi.Core.Entities.Reserve;

namespace Amlakbashi.Application.Services.ReserveServices.ReserveState.ReserveStates
{
    public class HostCancelState : ReserveState
    {
        private readonly IMediator mediator;
        private readonly IAccountingFacade accounting;
        public HostCancelState(IRepository<Reserve, long> Repository,
            IAccountingFacade accounting,
            IMediator mediator
            ) : base(Repository)
        {
            this.mediator = mediator;
            this.accounting = accounting;
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
                var contact = new UserContactDTO()
                {
                    UserMainMobile = reserve.GuestUser.MainMobile,
                    UserAppNotificationToken = reserve.GuestUser.AppNotificationToken,
                    UserEmail = reserve.GuestUser.Email,
                    UserFcmAppNotificationToken = reserve.GuestUser.FcmAppNotificationToken,
                    UserNotificationToken = reserve.GuestUser.NotificationToken,
                    Type = UserContactType.GuestReserveCanceledByHost,
                    AdvertiseId = reserve.AdvertiseID.ToString(),
                    ReserveId = reserve.Id.ToString()
                };
                mediator.Enqueue(new SendMessageCommand(contact));
            }
            mediator.Enqueue(new UpdateUserScoreCommand(reserve.Advertise.UserID));
            mediator.Enqueue(new UpdateAdvertiseScoreCommand(reserve.AdvertiseID));
        }

        public override bool CanTransitTo(ReserveStatus status)
        {
            return false;
        }
    }
}
