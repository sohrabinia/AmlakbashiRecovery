using Amlakbashi.Accounting;
using Amlakbashi.Core.Common.Repository;
using Amlakbashi.Core.Entities;
using Amlakbashi.Core.Infrastructure.UserContact;
using Amlakbashi.Mediator.Commands.AdvertiseCommands;
using MediatR;
using System;
using static Amlakbashi.Core.Entities.Reserve;

namespace Amlakbashi.Application.Services.ReserveServices.ReserveState.ReserveStates
{
    public class SystemCancelState : ReserveState
    {
        private readonly IAccountingFacade accounting;
        private readonly IMediator mediator;
        public SystemCancelState(
            IAccountingFacade accounting,
            IMediator mediator,
            IRepository<Reserve, long> Repository) : base(Repository)
        {
            this.accounting = accounting;
            this.mediator = mediator;
        }

        public override bool CanTransitTo(Reserve.ReserveStatus status)
        {
            return false;
        }

        public override void OnTransition(Reserve.ReserveStatus prevStatus, bool sendSms, ActionLog.ActionSourceEnum actionSource, int doerUserId)
        {
            var reserve = Repository.Find(ReserveId);
            reserve.Status = ReserveStatus.CanceledBySystem;
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
            var guestUser = reserve.GuestUser;
            var hostlerUser = Repository.Find<User, int>(reserve.Advertise.UserID);
            if (sendSms)
            {
                ReserveSendSms guestContact = new ReserveSendSms()
                {
                    ScheduledTime = DateTime.Now.Add(new TimeSpan(0, 5, 0)),
                    initial = false,
                    userId = guestUser.Id,
                    type = (int)UserContactType.ReserveRequest,
                    advertise_id = reserve.AdvertiseID.ToString(),
                    user_id = guestUser.Id.ToString(),
                    reserve_id = reserve.Id.ToString(),
                    doer_title = reserve.HostResponse == HostResponseEnum.None ?
                        "میزبان" : "شما"
                };
                mediator.Send(new ScheduleReserveSendSmsCommand(guestContact));

                ReserveSendSms hostContact = new ReserveSendSms()
                {
                    ScheduledTime = DateTime.Now.Add(new TimeSpan(0, 5, 0)),
                    initial = false,
                    userId = hostlerUser.Id,
                    type = (int)UserContactType.ReserveRequest,
                    advertise_id = reserve.AdvertiseID.ToString(),
                    user_id = hostlerUser.Id.ToString(),
                    reserve_id = reserve.Id.ToString(),
                    doer_title = reserve.HostResponse == HostResponseEnum.None ?
                        "میزبان" : "شما"
                };
                mediator.Send(new ScheduleReserveSendSmsCommand(hostContact));
            }
        }
    }
}
