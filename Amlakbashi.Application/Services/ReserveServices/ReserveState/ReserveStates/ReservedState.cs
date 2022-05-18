using Amlakbashi.Accounting;
using Amlakbashi.Application.Services.ReserveServices.ReserveSupportManager;
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
using MediatR;
using Microsoft.AspNetCore.Identity;
using System;
using static Amlakbashi.Core.Entities.Reserve;

namespace Amlakbashi.Application.Services.ReserveServices.ReserveState.ReserveStates
{
    public class ReservedState : ReserveState
    {
        private readonly IAccountingFacade accounting;
        private readonly IReserveSupportManager reserveSupportManager;
        private readonly IMediator mediator;
        private readonly UserManager<AppUser> userManager;
        public ReservedState(
            IAccountingFacade accounting,
            IReserveSupportManager reserveSupportManager,
            IMediator mediator,
            UserManager<AppUser> userManager,
            IRepository<Reserve, long> Repository) : base(Repository)
        {
            this.accounting = accounting;
            this.reserveSupportManager = reserveSupportManager;
            this.mediator = mediator;
            this.userManager = userManager;
        }

        public override bool CanTransitTo(Reserve.ReserveStatus status)
        {
            switch (status)
            {
                case ReserveStatus.CashPay:
                case ReserveStatus.Started:
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
            reserve.Status = ReserveStatus.Reserved;
            Repository.Update(reserve);
            Repository.Save();
            var paidPrice = accounting.GetReservePaidAmount(ReserveId, Reserve.StatusStringType.Guest);
            var isPaidCompletely = accounting.IsReservePaidCompletely(ReserveId);
            var remainedAmount = accounting.GetReserveRemainedAmount(ReserveId);
            var advertise = reserve.Advertise;
            var hostlerUser = Repository.Find<User, int>(advertise.UserID);
            var hostPhoneNumber = hostlerUser.Mobile ?? hostlerUser.MainMobile;
            var guestUser = reserve.GuestUser;
            var guestPhoneNumber = guestUser.Mobile ?? guestUser.MainMobile;
            if (isPaidCompletely)
            {
                if (sendSms)
                {
                    var guestIdentityUser = userManager.FindByNameAsync(guestUser.MainMobile).Result;
                    var guestContact = new UserContactDTO()
                    {
                        UserMainMobile = guestUser.MainMobile,
                        UserAppNotificationToken = guestUser.AppNotificationToken,
                        UserEmail = guestIdentityUser.Email,
                        EmailConfirmed = guestIdentityUser.EmailConfirmed,
                        UserFcmAppNotificationToken = guestUser.FcmAppNotificationToken,
                        Type = UserContactType.GuestReservedTotalPayed,
                        AdvertiseId = reserve.AdvertiseID.ToString(),
                        ReserveId = reserve.Id.ToString(),
                        AudienceMobile = PhoneUtility.IsNumberForIran(hostPhoneNumber) ?
                            PhoneUtility.InternationalNumberToLocal(hostPhoneNumber) :
                            PhoneUtility.InternationalNumberToCallable(hostPhoneNumber)
                    };
                    mediator.Enqueue(new SendMessageCommand(guestContact));

                    var hostIdentityUser = userManager.FindByNameAsync(hostlerUser.MainMobile).Result;
                    var hostContact = new UserContactDTO()
                    {
                        UserMainMobile = hostlerUser.MainMobile,
                        UserAppNotificationToken = hostlerUser.AppNotificationToken,
                        UserEmail = hostIdentityUser.Email,
                        EmailConfirmed = hostIdentityUser.EmailConfirmed,
                        UserFcmAppNotificationToken = hostlerUser.FcmAppNotificationToken,
                        UserNotificationToken = hostlerUser.NotificationToken,
                        Type = UserContactType.HostReservedTotalPayed,
                        AdvertiseId = reserve.AdvertiseID.ToString(),
                        ReserveId = reserve.Id.ToString(),
                        AudienceMobile = PhoneUtility.IsNumberForIran(guestPhoneNumber) ?
                            PhoneUtility.InternationalNumberToLocal(guestPhoneNumber) :
                            PhoneUtility.InternationalNumberToCallable(guestPhoneNumber)
                    };
                    mediator.Enqueue(new SendMessageCommand(hostContact));
                }
            }
            else
            {
                if (sendSms)
                {
                    var guestIdentityUser = userManager.FindByNameAsync(guestUser.MainMobile).Result;
                    var guestContact = new UserContactDTO()
                    {
                        UserMainMobile = guestUser.MainMobile,
                        UserAppNotificationToken = guestUser.AppNotificationToken,
                        UserEmail = guestIdentityUser.Email,
                        EmailConfirmed = guestIdentityUser.EmailConfirmed,
                        UserFcmAppNotificationToken = guestUser.FcmAppNotificationToken,
                        Type = UserContactType.GuestReservedDepositePayed,
                        AdvertiseId = reserve.AdvertiseID.ToString(),
                        ReserveId = reserve.Id.ToString(),
                        AudienceMobile = PhoneUtility.IsNumberForIran(hostPhoneNumber) ?
                            PhoneUtility.InternationalNumberToLocal(hostPhoneNumber) :
                            PhoneUtility.InternationalNumberToCallable(hostPhoneNumber),
                        Price = paidPrice.ToString(),
                        RemainPrice = remainedAmount.ToString()
                    };
                    mediator.Enqueue(new SendMessageCommand(guestContact));

                    var hostIdentityUser = userManager.FindByNameAsync(hostlerUser.MainMobile).Result;
                    var hostContact = new UserContactDTO()
                    {
                        UserMainMobile = hostlerUser.MainMobile,
                        UserAppNotificationToken = hostlerUser.AppNotificationToken,
                        UserEmail = hostIdentityUser.Email,
                        EmailConfirmed = hostIdentityUser.EmailConfirmed,
                        UserFcmAppNotificationToken = hostlerUser.FcmAppNotificationToken,
                        UserNotificationToken = hostlerUser.NotificationToken,
                        Type = UserContactType.HostReservedDepositePayed,
                        AdvertiseId = reserve.AdvertiseID.ToString(),
                        ReserveId = reserve.Id.ToString(),
                        AudienceMobile = PhoneUtility.IsNumberForIran(guestPhoneNumber) ?
                            PhoneUtility.InternationalNumberToLocal(guestPhoneNumber) :
                            PhoneUtility.InternationalNumberToCallable(guestPhoneNumber),
                        Price = paidPrice.ToString(),
                        RemainPrice = remainedAmount.ToString()
                    };
                    mediator.Enqueue(new SendMessageCommand(hostContact));
                }
            }
            if (advertise.Count <= 1)
            {
                mediator.Send(new RejectRequestsInTimeCommand(reserve.AdvertiseID,
                    reserve.StartDate, reserve.EndDate, actionSource, doerUserId,
                    false, reserve.Id));
                mediator.Send(new RejectGuestRequestsInTimeCommand(reserve.UserID,
                    reserve.StartDate, actionSource, doerUserId, false, reserve.Id));
            }
            if (DateTimeUtility.DateRangesHaveOverlap(DateTime.Now.Date, DateTime.Now.Date.AddDays(1), reserve.StartDate, reserve.EndDate))
            {
                advertise.TodayIsEmpty = false;
            }
            var finishDelay = new DateTime(
                reserve.EndDate.Year,
                reserve.EndDate.Month,
                reserve.EndDate.Day,
                12, 0, 0) - DateTime.Now;
            mediator.Schedule(new SetReserveStatusCommand(reserve.Id,
                ReserveStatus.Completed, sendSms, actionSource, doerUserId), finishDelay);
            mediator.Schedule(new FinishStayMessageCommand(reserve.Id), finishDelay);

            var beforeStart = new DateTime(
                reserve.StartDate.Year,
                reserve.StartDate.Month,
                reserve.StartDate.Day,
                12, 0, 0) - DateTime.Now;
            if (beforeStart.TotalMilliseconds <= 0)
            {
                mediator.Send(new SetReserveStatusCommand(ReserveId, ReserveStatus.Started,
                    sendSms, actionSource, doerUserId));
            }
            else
            {
                var onStart = beforeStart.Add(new TimeSpan(2, 0, 0));
                mediator.Schedule(new SetReserveStatusCommand(reserve.Id,
                    ReserveStatus.Started, sendSms, actionSource, doerUserId), onStart);
            }
            mediator.Enqueue(new UpdateAdvertiseScoreCommand(reserve.AdvertiseID));
            reserveSupportManager.ReserveDoneHandle(reserve.Id);
        }
    }
}
