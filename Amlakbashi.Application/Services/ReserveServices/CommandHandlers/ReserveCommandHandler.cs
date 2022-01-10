using Amlakbashi.Accounting;
using Amlakbashi.Application.Services.ReserveServices.ReserveState.Interfaces;
using Amlakbashi.Core.Common.Extensions;
using Amlakbashi.Core.Common.Repository;
using Amlakbashi.Core.Common.Utilities;
using Amlakbashi.Core.Entities;
using Amlakbashi.Core.Identity.Entities;
using Amlakbashi.Core.Infrastructure.UserContact;
using Amlakbashi.Core.Infrastructure.UserContact.Interfaces;
using Amlakbashi.Mediator.Commands.AdvertiseCommands;
using Amlakbashi.Mediator.Commands.ReserveCommands;
using Amlakbashi.Mediator.Commands.UserCommands;
using log4net;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static Amlakbashi.Core.Entities.Reserve;
using static Amlakbashi.Core.Entities.ReservePayment;

namespace Amlakbashi.Application.Services.ReserveServices.CommandHandlers
{
    public class ReserveCommandHandler :
        IRequestHandler<FinalizeReserveCommand, ReserveStatus>,
        IRequestHandler<HostCanceledForReservedMessageCommand>,
        IRequestHandler<FinishStayMessageCommand>,
        IRequestHandler<SystemCancelReserveCommand, bool>,
        IRequestHandler<SetReserveStatusCommand, bool>,
        IRequestHandler<RejectRequestsInTimeCommand>,
        IRequestHandler<RejectGuestRequestsInTimeCommand>,
        IRequestHandler<SetHostResponseCommand, bool>,
        IRequestHandler<ScheduleReservePaymentCommand>,
        IRequestHandler<ScheduleSendReserveRequestCallCommand, TimeSpan>,
        IRequestHandler<SendReserveRequestCallCommand>,
        IRequestHandler<SendPayReserveCallCommand>, 
        IRequestHandler<UpdateReserveArchivesCommand>
    {
        private readonly IRepository<Reserve, long> reserveRepository;
        private readonly IUserContactFacade userContact;
        private readonly IReserveStateContext reserveState;
        private readonly IMediator mediator;
        private readonly IAccountingFacade accounting;
        private readonly UserManager<AppUser> userManager;
        private readonly ILog logger;
        public ReserveCommandHandler(
            IRepository<Reserve, long> reserveRepository,
            IUserContactFacade userContact,
            IReserveStateContext reserveState,
            IMediator mediator,
            UserManager<AppUser> userManager,
            IAccountingFacade accounting,
            ILog logger)
        {
            this.reserveRepository = reserveRepository;
            this.userContact = userContact;
            this.reserveState = reserveState;
            this.mediator = mediator;
            this.userManager = userManager;
            this.accounting = accounting;
            this.logger = logger;
        }

        public Task<Unit> Handle(ScheduleReservePaymentCommand request, CancellationToken cancellationToken)
        {
            var delay = DateTimeUtility.DelayAvoidingNightTime(new TimeSpan(0, 5, 0));
            mediator.Schedule(new SendPayReserveCallCommand(request.reserveId), delay);
            mediator.Send(new ScheduleReserveAutoCancelCommand(request.reserveId, delay.Add(new TimeSpan(0, 30, 0))));
            return Task.FromResult(Unit.Value);
        }

        public Task<Unit> Handle(HostCanceledForReservedMessageCommand request, CancellationToken cancellationToken)
        {
            var reserve = reserveRepository.Find(request.reserveId);
            var hostlerUser = reserveRepository.Find<User, int>(reserve.Advertise.UserID);
            var hostlerIdentityUser = userManager.FindByNameAsync(hostlerUser.MainMobile).Result;
            var contact = new UserContactDTO()
            {
                UserMainMobile = hostlerUser.MainMobile,
                UserAppNotificationToken = hostlerUser.AppNotificationToken,
                UserEmail = hostlerIdentityUser.Email,
                EmailConfirmed = hostlerIdentityUser.EmailConfirmed,
                UserFcmAppNotificationToken = hostlerUser.FcmAppNotificationToken,
                UserNotificationToken = hostlerUser.NotificationToken,
                Type = UserContactType.HostReserveRejectedForReserved,
                AdvertiseId = reserve.AdvertiseID.ToString(),
                ReserveId = reserve.Id.ToString()
            };
            mediator.Enqueue(new SendMessageCommand(contact));
            return Task.FromResult(Unit.Value);
        }

        public Task<bool> Handle(SystemCancelReserveCommand request, CancellationToken cancellationToken)
        {
            var reserve = reserveRepository.Find(request.reserveId);
            if (reserve.Status != ReserveStatus.WaitForResponse &&
                reserve.Status != ReserveStatus.WaitForReserve)
            {
                return Task.FromResult(true);
            }
            if (reserve.DisableAutoCancel && !request.force)
                return Task.FromResult(false);
            if (reserve.Status == ReserveStatus.WaitForResponse && (reserve.EndDate - reserve.StartDate).TotalDays <= 5)
            {
                mediator.Send(new SetExtrinsicReserveForWaitForResponseCommand(reserve.AdvertiseID, reserve.Id,
                    reserve.StartDate, reserve.EndDate));
            }
            reserveState.UseReserve(request.reserveId)
                .SetStatus(ReserveStatus.CanceledBySystem, request.sendSms,
                ActionLog.ActionSourceEnum.Background, 0);
            return Task.FromResult(true);
        }

        public Task<Unit> Handle(FinishStayMessageCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var reserve = reserveRepository.Find(request.reserveId);
                if (reserve.Status != ReserveStatus.Started)
                {
                    return Task.FromResult(Unit.Value);
                }
                var guestUser = reserve.GuestUser;
                var guestIdentityUser = userManager.FindByNameAsync(guestUser.MainMobile).Result;
                var contact = new UserContactDTO()
                {
                    UserMainMobile = guestUser.MainMobile,
                    UserAppNotificationToken = guestUser.AppNotificationToken,
                    UserEmail = guestIdentityUser.Email,
                    EmailConfirmed = guestIdentityUser.EmailConfirmed,
                    UserFcmAppNotificationToken = guestUser.FcmAppNotificationToken,
                    UserNotificationToken = guestUser.NotificationToken,
                    Type = UserContactType.FinishStay,
                    ReserveId = reserve.Id.ToString(),
                    Extra1 = ".",
                    Extra2 = guestUser.FullName
                };
                mediator.Enqueue(new SendMessageCommand(contact));
            }
            catch(Exception exc)
            {
                logger.Error("ReserveCommandHandler.FinishStayMessageCommand", exc);
            }
            return Task.FromResult(Unit.Value);
        }

        public Task<bool> Handle(SetReserveStatusCommand request, CancellationToken cancellationToken)
        {
            bool done = false;
            try
            {
                //if (request.status == ReserveStatus.Started)
                //{
                //    if (request.force == false && accounting.IsReservePaidCompletely(request.reserveId) == false)
                //    {
                //        return Task.FromResult(false);
                //    }
                //}
                done = reserveState.UseReserve(request.reserveId).SetStatus(request.status,
                    request.sendSms, request.actionSource, request.doerUserId, request.force);
            }
            catch(Exception exc)
            {
                logger.Error("ReserveCommandHandler.SetReserveStatusCommand", exc);
            }
            return Task.FromResult(done);
        }

        public Task<bool> Handle(SetHostResponseCommand request, CancellationToken cancellationToken)
        {
            var reserve = reserveRepository.Find(request.reserveId);
            if (reserve.HostResponse == request.hostResponse)
                return Task.FromResult(false);
            reserve.HostResponse = request.hostResponse;
            reserve.HostResponseDate = DateTime.Now;
            reserveRepository.Update(reserve);
            reserveRepository.Save();

            switch (request.hostResponse)
            {
                case HostResponseEnum.Accepted:
                    mediator.Send(new SetReserveStatusCommand(request.reserveId,
                        ReserveStatus.WaitForReserve, request.sendSms,
                        request.actionSource, request.doerUserId, request.force));
                    break;
                case HostResponseEnum.RejectedPrice:
                case HostResponseEnum.RejectedHomeFull:
                case HostResponseEnum.Rejected:
                    mediator.Send(new SetReserveStatusCommand(request.reserveId,
                        ReserveStatus.Rejected, request.sendSms,
                        request.actionSource, request.doerUserId, request.force));
                    break;
            }

            switch (request.hostResponse)
            {
                case HostResponseEnum.RejectedPrice:
                    mediator.Send(new RejectRequestsInTimeCommand(
                        reserve.AdvertiseID, reserve.StartDate, reserve.EndDate,
                        request.actionSource, request.doerUserId, true));
                    break;
                case HostResponseEnum.RejectedHomeFull:
                    mediator.Send(new RejectRequestsInTimeCommand(
                        reserve.AdvertiseID, reserve.StartDate, reserve.EndDate,
                        request.actionSource, request.doerUserId, true));

                    if ((reserve.EndDate - reserve.StartDate).TotalDays <= 5)
                    {
                        mediator.Send(new InsertExtrinsicReserveCommand(reserve.AdvertiseID,
                            DateTimeUtility.GregorianToPersianDate(reserve.StartDate),
                            DateTimeUtility.GregorianToPersianDate(reserve.EndDate),
                            request.actionSource, request.doerUserId, reserve.Advertise.Count));
                    }
                    break;
                default:
                    break;
            }
            return Task.FromResult(true);
        }

        public Task<Unit> Handle(RejectRequestsInTimeCommand request, CancellationToken cancellationToken)
        {
            var reserves = reserveRepository.Query(q => q.Where(x => x.AdvertiseID == request.advertiseId &&
                x.Id != request.exceptReserveId &&
                (x.Status == ReserveStatus.WaitForResponse ||
                (request.exceptWaitForReserve ? false : x.Status == ReserveStatus.WaitForReserve)))).ToList();
            foreach (var item in reserves)
            {
                if (DateTimeUtility.DateRangesHaveOverlap(request.startDate, request.endDate, item.StartDate, item.EndDate))
                {
                    if (request.doSystemCancel)
                    {
                        mediator.Send(new SetReserveStatusCommand(item.Id, ReserveStatus.CanceledBySystem, 
                            request.sendSms, request.actionSource, request.doerUserId, true));
                    }
                    else
                    {
                        mediator.Send(new SetHostResponseCommand(item.Id, HostResponseEnum.Rejected, request.sendSms, 
                            request.actionSource, request.doerUserId, true));
                    }
                }
            }
            return Task.FromResult(Unit.Value);
        }

        public Task<Unit> Handle(RejectGuestRequestsInTimeCommand request, CancellationToken cancellationToken)
        {
            var guestUser = reserveRepository.Find<User, int>(request.guestUserId);
            var reserves = guestUser.Reserves
                .Where(w => w.Id != request.exceptReserveId &&
               (w.Status == ReserveStatus.WaitForResponse ||
               (request.exceptWaitForReserve ? false :
               w.Status == ReserveStatus.WaitForReserve)));
            foreach (var item in reserves)
            {
                if (request.startDate == item.StartDate)
                {
                    mediator.Send(new SetReserveStatusCommand(item.Id, ReserveStatus.CanceledBySystem, false, request.actionSource, request.doerUserId));
                    mediator.Send(new HostCanceledForReservedMessageCommand(item.Id));
                }
            }
            return Task.FromResult(Unit.Value);
        }

        public Task<TimeSpan> Handle(ScheduleSendReserveRequestCallCommand request, CancellationToken cancellationToken)
        {
            var now = DateTime.Now;
            var delay = new TimeSpan(0, 5, 0);
            var callTime = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second);
            callTime = callTime.AddTicks(delay.Ticks);
            var canselCall = false;
            if (callTime.Hour < 10)
            {
                callTime = new DateTime(callTime.Year, callTime.Month, callTime.Day, 10, 0, 0);
                delay = new TimeSpan(callTime.Ticks - now.Ticks);
                canselCall = true;
            }
            else if (callTime.Hour >= 23)
            {
                callTime = new DateTime(callTime.Year, callTime.Month, callTime.Day, 10, 0, 0);
                callTime = callTime.AddDays(1);
                delay = new TimeSpan(callTime.Ticks - now.Ticks);
                canselCall = true;
            }
            callTime = DateTime.Now + delay;
            if (callTime.Hour < 10)
                callTime = new DateTime(callTime.Year, callTime.Month, callTime.Day, 10, 0, 0);
            delay = callTime - DateTime.Now;
            if (canselCall == false)
            {
                mediator.Schedule(new SendReserveRequestCallCommand(request.ReserveId), delay);
            }
            return Task.FromResult(delay);
        }

        public Task<ReserveStatus> Handle(FinalizeReserveCommand request, CancellationToken cancellationToken)
        {
            var reserve = reserveRepository.Find(request.reserveId);
            if (request.couponId > 0)
            {
                accounting.UseDiscountCouponForReserve(request.couponId,
                    request.reserveId);
                reserve = reserveRepository.Find(request.reserveId);
            }
            else if (request.prizePrice > 0)
            {
                accounting.UsePrizeCreditForReserve(request.reserveId, request.doerUserId, request.actionSource);
                reserve = reserveRepository.Find(request.reserveId);
            }
            if (request.payerUserId < 1)
                request.payerUserId = reserve.UserID;
            var paymentType = request.paidAmount >=
                (reserve.TotalPrice - reserve.CouponPrice - reserve.PrizePrice) ?
                ReservePaymentType.GuestClearing :
                ReservePaymentType.GuestDeposite;
            accounting.InsertReservePayment(request.payerUserId,
                request.reserveId, request.transactionId, 0,
                paymentType, request.paidAmount, request.paymentMethod);
            if (reserve.Status == ReserveStatus.WaitForReserve)
            {
                mediator.Send(new SetReserveStatusCommand(request.reserveId,
                    ReserveStatus.Reserved, request.sendSms, request.actionSource,
                    request.doerUserId));
                return Task.FromResult(ReserveStatus.Reserved);
            }
            else if (reserve.Status == ReserveStatus.Reserved)
            {
                DateTime canStartTime;
                if (reserve.CanReserveStarted(out canStartTime))
                {
                    mediator.Send(new SetReserveStatusCommand(request.reserveId,
                        ReserveStatus.Started, request.sendSms, request.actionSource,
                        request.doerUserId));
                    return Task.FromResult(ReserveStatus.Started);
                }
                else
                {
                    return Task.FromResult(ReserveStatus.Reserved);
                }
            }
            else if (reserve.Status == ReserveStatus.CashPay)
            {
                DateTime canStartTime;
                if (reserve.CanReserveStarted(out canStartTime))
                {
                    mediator.Send(new SetReserveStatusCommand(request.reserveId,
                        ReserveStatus.Started, request.sendSms,
                        request.actionSource, request.doerUserId));
                    return Task.FromResult(ReserveStatus.Started);
                }
                else
                {
                    return Task.FromResult(ReserveStatus.CashPay);
                }
            }
            else
            {
                return Task.FromResult(reserve.Status);
            }
        }

        public Task<Unit> Handle(SendReserveRequestCallCommand request, CancellationToken cancellationToken)
        {
            var reserve = reserveRepository.Find(request.ReserveId);
            if (reserve.Status == ReserveStatus.WaitForResponse)
            {
                var advertise = reserve.Advertise;
                var hostlerUser = reserveRepository.Find<User, int>(advertise.UserID);
                if (PhoneUtility.IsNumberForIran(hostlerUser.MainMobile))
                {
                    userContact.SendReserveRequestCall(hostlerUser, reserve.AdvertiseID);
                }
            }
            return Task.FromResult(Unit.Value);
        }

        public Task<Unit> Handle(SendPayReserveCallCommand request, CancellationToken cancellationToken)
        {
            var reserve = reserveRepository.Find(request.ReserveId);
            if (reserve.Status == ReserveStatus.WaitForReserve)
            {
                var guestUser = reserve.GuestUser;
                if (PhoneUtility.IsNumberForIran(guestUser.MainMobile))
                {
                    userContact.SendPayReserveCall(guestUser, reserve.AdvertiseID);
                }
            }
            return Task.FromResult(Unit.Value);
        }

        public Task<Unit> Handle(UpdateReserveArchivesCommand request, CancellationToken cancellationToken)
        {
            var twoDaysAgo = DateTime.Now - new TimeSpan(2, 0, 0, 0);
            var outdatedReserves = reserveRepository.Query(
                q => q.Include(i => i.ReservePayments)
                .Where(x => x.Archive == false && x.EndDate < twoDaysAgo));
            bool refundDone;
            bool shouldRefund;
            foreach (var item in outdatedReserves)
            {
                var reservePayments = item.ReservePayments;
                if (reservePayments.Any(x => x.PaymentType == (int)ReservePayment.ReservePaymentType.SiteRefundToGuest))
                {
                    refundDone = true;
                    shouldRefund = true;
                }
                else
                {
                    refundDone = false;
                    switch (item.Status)
                    {
                        case Reserve.ReserveStatus.WaitForResponse:
                        case Reserve.ReserveStatus.WaitForReserve:
                        case Reserve.ReserveStatus.Rejected:
                        case Reserve.ReserveStatus.Reserved:
                        case Reserve.ReserveStatus.CashPay:
                        case Reserve.ReserveStatus.Started:
                        case Reserve.ReserveStatus.Completed:
                        case Reserve.ReserveStatus.CancelRequestByGuest:
                        case Reserve.ReserveStatus.CancelRequestByHost:
                            shouldRefund = false;
                            break;
                        default:
                            shouldRefund = reservePayments.Any(x => x.PaymentType == (int)ReservePayment.ReservePaymentType.GuestClearing ||
                                x.PaymentType == (int)ReservePayment.ReservePaymentType.GuestDeposite);
                            break;
                    }
                }
                shouldRefund = shouldRefund && !refundDone;

                bool canClear = false;
                if (item.Status != Reserve.ReserveStatus.Reserved &&
                    item.Status != Reserve.ReserveStatus.Started &&
                    item.Status != Reserve.ReserveStatus.Completed &&
                    item.Status != Reserve.ReserveStatus.CashPay &&
                    reservePayments.Any(x => x.PaymentType == (int)ReservePayment.ReservePaymentType.SiteClearingToHost))
                {
                    canClear = false;
                }
                var guestPaidAmount = accounting.GetReservePaidAmount(item.Id,
                        StatusStringType.Guest);
                canClear = PriceUtility.CalculateHostPayablePrice(item.TotalPrice, guestPaidAmount, item.CouponPrice, item.PrizePrice) <= 0 ? false : true;
                if (shouldRefund || canClear)
                {
                    continue;
                }
                item.Archive = true;
                reserveRepository.Update(item);
            }
            reserveRepository.Save();
            return Task.FromResult(Unit.Value);
        }
    }
}
