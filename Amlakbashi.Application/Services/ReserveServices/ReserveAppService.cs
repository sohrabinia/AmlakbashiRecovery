using Amlakbashi.Core.Common.AppService;
using Amlakbashi.Core.Common.Repository;
using Amlakbashi.Application.Services.ReserveServices.Interfaces;
using Amlakbashi.Core.Common.Caching;
using Amlakbashi.Core.Common.Utilities;
using Amlakbashi.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using static Amlakbashi.Core.Entities.ReservePayment;
using static Amlakbashi.Core.Entities.Reserve;
using MediatR;
using static Amlakbashi.Core.Entities.ActionLog;
using Amlakbashi.Accounting;
using Amlakbashi.Mediator.Commands.ReserveCommands;
using Amlakbashi.Mediator.Commands.AdvertiseCommands;
using Microsoft.EntityFrameworkCore;
using Amlakbashi.Core.Infrastructure.LocalizationHelpers;
using Amlakbashi.Mediator.Events.UserEvents;
using Amlakbashi.Core.Common.Extensions;
using Amlakbashi.Core.DTOs.ReserveDTOs;

namespace Amlakbashi.Application.Services.ReserveServices
{
    internal class ReserveAppService : AppServiceBase<Reserve, long>, IReserveAppService
    {
        private readonly IMediator mediator;
        private readonly IAccountingFacade accounting;
        public ReserveAppService(
            IMediator mediator,
            IAccountingFacade accounting,
            IRepository<Reserve, long> repository,
            ICacheManager<Reserve> cache) : base(repository, cache)
        {
            this.mediator = mediator;
            this.accounting = accounting;
        }

        public IList<Reserve> Filter(long reserve_id = -1, long advertise_id = -1,
            int host_user_id = -1, int guest_user_id = -1, int reserve_status = -1,
            int host_response_status = -1, int general_status = -1,
            string site_clearing_date = "", int site_cleared_status = -1,
            string reserve_from_date = "", string reserve_to_date = "",
            string reserve_end_date = "", int stay_duration_from = -1, int stay_duration_to = -1,
            int reserve_support_status = 0, bool shouldFollow = false,
            int supporter_id = -1, int host_card_status = -1,
            int mainFilter = 0, int instantReserveFilter = 2,
            bool disableAutoCancel = false, bool accVisited = false)
        {
            IQueryable<Reserve> allReserves;
            if (reserve_support_status > 0 && reserve_support_status != 3)
            {
                allReserves = Repository.Query(q => q.Include("GuestUser.ReserveSupportsAsGuest"));
            }
            else
            {
                allReserves = Repository.Query(q => q);
            }
            if (mainFilter == 0)
            {
                allReserves = allReserves.Where(x => !x.Archive);
            }
            else if (mainFilter == 2)
            {
                allReserves = allReserves.Where(x => x.Archive);
            }
            if (instantReserveFilter == 0)
            {
                allReserves = allReserves.Where(x => !x.InstantReserve);
            }
            else if (instantReserveFilter == 1)
            {
                allReserves = allReserves.Where(x => x.InstantReserve);
            }
            IQueryable<Reserve> model = allReserves.Where(u => u.Status != Reserve.ReserveStatus.Deleted);
            if (shouldFollow)
            {
                model = model.Where(x => x.shouldFollow);
            }
            if (disableAutoCancel)
            {
                model = model.Where(x => x.DisableAutoCancel);
            }
            if (accVisited)
            {
                model = model.Where(x => x.AccVisitedByGuest);
            }
            if (reserve_id > 0)
            {
                model = model.Where(x => x.Id == reserve_id);
            }
            if (advertise_id > 0)
            {
                model = model.Where(x => x.AdvertiseID == advertise_id);
            }
            if (guest_user_id > 0)
            {
                model = model.Where(x => x.UserID == guest_user_id);
            }
            if (reserve_status > -1)
            {
                model = model.Where(x => (int)x.Status == reserve_status);
            }
            if (host_response_status > -1)
            {
                var hostResp = (HostResponseEnum)host_response_status;
                model = model.Where(x => x.HostResponse == hostResp);
            }
            if (general_status > -1)
            {
                if (general_status == 0)
                {
                    model = model.Where(x => x.ReservePayments.Any(a => a.PaymentType == (int)ReservePaymentType.GuestClearing
                        || a.PaymentType == (int)ReservePaymentType.GuestDeposite));
                }
            }
            if (host_user_id > 0)
            {
                model = model.Where(x => x.Advertise.UserID == host_user_id);
            }
            if (!string.IsNullOrEmpty(site_clearing_date))
            {
                model = model.Where(x => x.Status == Reserve.ReserveStatus.Reserved ||
                    x.Status == Reserve.ReserveStatus.Started ||
                    x.Status == Reserve.ReserveStatus.Completed ||
                    x.Status == Reserve.ReserveStatus.CashPay);
                var gregorian_clearing_date = DateTimeUtility.PersianDateToGregorian(
                    StringUtility.PersianNumberToEnglish(site_clearing_date).Replace('/', ','));
                model = model.Where(w => (EF.Functions.DateDiffDay(w.StartDate, w.EndDate) > 1 ?
                    w.StartDate.AddDays(2) : w.EndDate) <= gregorian_clearing_date);
            }
            if (stay_duration_from > 0)
            {
                model = model.Where(w => EF.Functions.DateDiffDay(w.StartDate, w.EndDate) >= stay_duration_from);
            }
            if (stay_duration_to > 0)
            {
                model = model.Where(w => EF.Functions.DateDiffDay(w.StartDate, w.EndDate) <= stay_duration_to);
            }
            if (!string.IsNullOrEmpty(reserve_from_date))
            {
                var gregorian_date = DateTimeUtility.PersianDateToGregorian(
                    StringUtility.PersianNumberToEnglish(reserve_from_date).Replace('/', ','));
                model = model.Where(x => x.StartDate >= gregorian_date);
            }
            if (!string.IsNullOrEmpty(reserve_to_date))
            {
                var gregorian_date = DateTimeUtility.PersianDateToGregorian(
                    StringUtility.PersianNumberToEnglish(reserve_to_date).Replace('/', ','));
                model = model.Where(x => x.EndDate <= gregorian_date);
            }
            if (!string.IsNullOrEmpty(reserve_end_date))
            {
                var gregorian_date = DateTimeUtility.PersianDateToGregorian(
                    StringUtility.PersianNumberToEnglish(reserve_end_date).Replace('/', ','));
                model = model.Where(x => x.EndDate == gregorian_date);
            }
            if (site_cleared_status == 0)//payed
            {
                model = model.Where(w => w.ReservePayments.Any(a => a.PaymentType == (int)ReservePaymentType.SiteClearingToHost));
            }
            else if (site_cleared_status == 1)//not payed
            {
                model = model.Where(w => (w.Status == Reserve.ReserveStatus.Reserved ||
                    w.Status == Reserve.ReserveStatus.Started ||
                    w.Status == Reserve.ReserveStatus.Completed ||
                    w.Status == Reserve.ReserveStatus.CashPay) &&
                    !w.ReservePayments.Any(a => a.PaymentType == (int)ReservePaymentType.SiteClearingToHost));

                var matchedIds = new List<long>();
                foreach (var item in model)
                {
                    var guestPaidAmount = accounting.GetReservePaidAmount(item.Id,
                        StatusStringType.Guest);
                    if (PriceUtility.CalculateHostPayablePrice(item.TotalPrice, guestPaidAmount, item.CouponPrice, item.PrizePrice) > 0)
                    {
                        matchedIds.Add(item.Id);
                    }
                }
                model = model.Where(x => matchedIds.Contains(x.Id));
            }
            else if (site_cleared_status == 2)//refund done
            {
                var matchedIds = new List<long>();
                foreach (var item in model)
                {
                    bool refundDone;
                    bool result;
                    if (item.ReservePayments.Any(x => x.PaymentType == (int)ReservePaymentType.SiteRefundToGuest))
                    {
                        refundDone = true;
                        result = true;
                    }
                    else
                    {
                        refundDone = false;
                        result =
                            item.Status == Reserve.ReserveStatus.WaitForResponse ||
                            item.Status == Reserve.ReserveStatus.WaitForReserve ||
                            item.Status == Reserve.ReserveStatus.Rejected ||
                            item.Status == Reserve.ReserveStatus.Reserved ||
                            item.Status == Reserve.ReserveStatus.CashPay ||
                            item.Status == Reserve.ReserveStatus.Started ||
                            item.Status == Reserve.ReserveStatus.Completed ||
                            item.Status == Reserve.ReserveStatus.CancelRequestByHost ||
                            item.Status == Reserve.ReserveStatus.CancelRequestByGuest ? false :
                            item.ReservePayments.Any(x => x.PaymentType == (int)ReservePaymentType.GuestClearing ||
                            x.PaymentType == (int)ReservePaymentType.GuestDeposite);
                    }
                    if (result && refundDone)
                    {
                        matchedIds.Add(item.Id);
                    }
                }
                model = model.Where(x => matchedIds.Contains(x.Id));
            }
            else if (site_cleared_status == 3)//should refund
            {
                var matchedIds = new List<long>();
                var tempModel = model.Where(w => w.ReservePayments.Any(x =>
                    x.PaymentType == (int)ReservePaymentType.GuestClearing ||
                    x.PaymentType == (int)ReservePaymentType.GuestDeposite));
                foreach (var item in tempModel)
                {
                    bool refundDone;
                    bool result;
                    if (item.ReservePayments.Any(x => x.PaymentType == (int)ReservePaymentType.SiteRefundToGuest))
                    {
                        refundDone = true;
                        result = true;
                    }
                    else
                    {
                        refundDone = false;
                        result =
                            item.Status == Reserve.ReserveStatus.WaitForResponse ||
                            item.Status == Reserve.ReserveStatus.WaitForReserve ||
                            item.Status == Reserve.ReserveStatus.Rejected ||
                            item.Status == Reserve.ReserveStatus.Reserved ||
                            item.Status == Reserve.ReserveStatus.CashPay ||
                            item.Status == Reserve.ReserveStatus.Started ||
                            item.Status == Reserve.ReserveStatus.Completed ||
                            item.Status == Reserve.ReserveStatus.CancelRequestByHost ||
                            item.Status == Reserve.ReserveStatus.CancelRequestByGuest ? false :
                            item.ReservePayments.Any(x => x.PaymentType == (int)ReservePaymentType.GuestClearing ||
                            x.PaymentType == (int)ReservePaymentType.GuestDeposite);
                    }
                    if (result && !refundDone)
                    {
                        matchedIds.Add(item.Id);
                    }
                }
                model = model.Where(x => matchedIds.Contains(x.Id));
            }
            return model.ToList();
        }

        public IList<Reserve> GetListByUserId(int userId,
            Reserve.ReserveManagerSelectType selectType = Reserve.ReserveManagerSelectType.All)
        {
            var data = Repository.Query(q => q);
            var model = new List<Reserve>();
            if (selectType == Reserve.ReserveManagerSelectType.Host ||
                selectType == Reserve.ReserveManagerSelectType.All)
            {
                model.AddRange(data.Where(w => w.Advertise.UserID == userId));
            }
            if (selectType == Reserve.ReserveManagerSelectType.Guest ||
                selectType == Reserve.ReserveManagerSelectType.All)
            {
                model.AddRange(data.Where(w => w.UserID == userId));
            }
            return data.ToList();
        }

        public IList<Reserve> GetListByUserId(int userId, bool isHost = false)
        {
            if (isHost)
            {
                return Repository.Query(q => q.Where(w => w.Advertise.UserID == userId).ToList());
            }
            else
            {
                return Repository.Query(q => q.Where(w => w.UserID == userId).ToList());
            }
        }

        public IList<Reserve> GetListByUserId(int userId, Reserve.ReserveStatus status, bool RatingShownToGuest,
            bool isHost = false)
        {
            if (isHost)
            {
                return Repository.Query(q => q.Where(w => w.Advertise.UserID == userId && w.Status == status &&
                    w.RatingShownToGuest == RatingShownToGuest).ToList());
            }
            else
            {
                return Repository.Query(q => q.Where(w => w.UserID == userId && w.Status == status &&
                    w.RatingShownToGuest == RatingShownToGuest).ToList());
            }
        }

        public IList<Reserve> GetListByUserId(int userId, int category, bool isHost = false)
        {
            IQueryable<Reserve> data;
            if (isHost)
            {
                data = Repository.Query(q => q.Where(w => w.Advertise.UserID == userId));
            }
            else
            {
                data = Repository.Query(q => q.Where(w => w.UserID == userId));
            }
            switch (category)
            {
                case -1:
                    break;
                case 0:
                    data = data.Where(x => x.Status ==
                        Reserve.ReserveStatus.WaitForResponse);
                    break;
                case 1:
                    data = data.Where(x => x.Status ==
                        Reserve.ReserveStatus.WaitForReserve);
                    break;
                case 2:
                    data = data.Where(x => x.Status ==
                        Reserve.ReserveStatus.Reserved ||
                        x.Status == Reserve.ReserveStatus.Started ||
                        x.Status == Reserve.ReserveStatus.CashPay ||
                        x.Status == Reserve.ReserveStatus.CancelRequestByGuest ||
                        x.Status == Reserve.ReserveStatus.CancelRequestByHost);
                    break;
                case 3:
                    data = data.Where(x => x.Status ==
                        Reserve.ReserveStatus.Completed);
                    break;
                case 4:
                    data = data.Where(x => x.Status ==
                        Reserve.ReserveStatus.Rejected ||
                        x.Status == Reserve.ReserveStatus.CanceledByGuest ||
                        x.Status == Reserve.ReserveStatus.CanceledByHost ||
                        x.Status == Reserve.ReserveStatus.CanceledBySystem);
                    break;
            }
            return data.OrderByDescending(x => x.CreateDate).ToList();
        }

        public Reserve Find(long id)
        {
            return Repository.Query(q => q.FirstOrDefault(f => f.Id == id));
        }

        public IList<Reserve> Find(IEnumerable<long> ids)
        {
            var result = new List<Reserve>();
            var allReserves = Repository.Query(q => q);
            foreach (var id in ids)
            {
                result.Add(allReserves.FirstOrDefault(f => f.Id == id));
            }
            return result;
        }

        public IList<Reserve> GetByUserId(int userId)
        {
            return Repository.Query(q => q.Where(w => w.Advertise.UserID == userId &&
                w.Status != ReserveStatus.Deleted)).ToList();
        }

        public Reserve FirstHavingUserId(int userId, ReserveStatus status)
        {
            return Repository.Query(q => q.FirstOrDefault(f => f.Advertise.UserID == userId && f.Status == status));
        }

        public Reserve GetRelatedReserveByUser(int userId, out bool isHost)
        {
            isHost = false;
            var minDate = DateTime.Now.AddDays(-5).Date;
            var hostReserves = Repository.Query(q => q.Where(w => w.HostUserID == userId && w.EndDate >= minDate));
            var guestReserves = Repository.Query(q => q.Where(w => w.UserID == userId && w.EndDate >= minDate));
            Reserve relatedReserve = null;
            if (hostReserves.Any())
            {
                isHost = true;
                relatedReserve = hostReserves.OrderByDescending(x => x.CreateDate).FirstOrDefault();
            }
            else if (guestReserves.Any())
            {
                relatedReserve = guestReserves.OrderByDescending(x => x.CreateDate).FirstOrDefault();
            }
            return relatedReserve;
        }

        public bool Update(Reserve reserve, string start_date,
            string end_date, out string msg, int doerUserId,
            ActionSourceEnum actionSource)
        {
            var objReserve = Repository.Query(q => q.Include("Advertise.User.Advertises").FirstOrDefault(f => f.Id == reserve.Id));
            if ((objReserve.Status < Reserve.ReserveStatus.Reserved ||
                objReserve.Status == Reserve.ReserveStatus.CanceledBySystem)
                &&
                (
                    reserve.Status == Reserve.ReserveStatus.Reserved ||
                    reserve.Status == Reserve.ReserveStatus.Started ||
                    reserve.Status == Reserve.ReserveStatus.CashPay ||
                    reserve.Status == Reserve.ReserveStatus.Completed ||
                    reserve.Status == Reserve.ReserveStatus.CancelRequestByGuest ||
                    reserve.Status == Reserve.ReserveStatus.CancelRequestByHost
                )
                &&
                reserve.DepositPrice > 0)
            {
                if (accounting.GetReservePaidAmount(objReserve.Id, StatusStringType.Guest) < objReserve.DepositPrice - objReserve.CouponPrice - objReserve.PrizePrice)
                {
                    msg = "مبلغ بیعانه هنوز پرداخت نشده، تغییر به وضعیت " + ReserveLocalization.GetStatusString((int)reserve.Status, Reserve.StatusStringType.Site) + " امکان پذیر نیست";
                    return false;
                }
            }
            if (start_date != null)
            {
                objReserve.StartDate = DateTimeUtility.PersianDateToGregorian(start_date);
            }
            if (end_date != null)
            {
                objReserve.EndDate = DateTimeUtility.PersianDateToGregorian(end_date);
            }

            if (objReserve.InstantReserve &&
                !objReserve.InstantReserveCancelHost &&
                reserve.Status == Reserve.ReserveStatus.CanceledByHost &&
                accounting.GetReservePaidAmount(objReserve.ReservePayments.ToList(), Reserve.StatusStringType.Guest) > 0)
            {
                var acc = objReserve.Advertise;
                var hostUser = objReserve.HostUser;
                var hostAccs = hostUser.Advertises;
                var hostCancelCount = hostAccs.Sum(x => x.InstantReserveCancels);
                int penaltyPrice = 0;
                if (hostCancelCount == 0)
                {
                    penaltyPrice = (int)Math.Floor(objReserve.TotalPrice * 0.1f);
                }
                else if (hostCancelCount == 1)
                {
                    penaltyPrice = (int)Math.Floor(objReserve.TotalPrice * 0.15f);
                }
                else
                {
                    penaltyPrice = 0;
                }
                if (penaltyPrice > 0)
                {
                    long newCredit;
                    accounting.DecreaseCredit(hostUser.Id, penaltyPrice, 0, 0, out newCredit, User.CreditTransactionCause.Other, "جریمه لغو رزرو آنی کد " + reserve.Id, 0, doerUserId, ActionLog.ActionSourceEnum.AdminPanel);
                }
                objReserve.InstantReserveCancelHost = true;

                mediator.Send(new IncreaseInstantReserveCancelCommand(acc.Id));
                if (hostCancelCount > hostUser.CancelInstantReserveLimit - 1)
                {
                    mediator.Send(new ChangeInstantReserveAccessCommand(hostUser.Id,
                        User.InstantReserveAccessEnum.Banned, doerUserId, actionSource));
                    mediator.Send(new UpdateInstantReserveStatusCommand(acc.Id, Advertise.InstantReserveStatusEnum.None, doerUserId, actionSource));
                }
            }
            objReserve.NumberOfGuests = reserve.NumberOfGuests;
            objReserve.TotalPrice = reserve.TotalPrice;
            objReserve.DepositPrice = reserve.DepositPrice;
            objReserve.CancelReason = reserve.CancelReason;
            objReserve.InstantReserveCancelHost = reserve.InstantReserveCancelHost;
            Repository.Update(objReserve);
            Repository.Save();
            msg = "";
            return true;
        }

        public void UpdateShouldFollow(long id, string text, User user)
        {
            var data = Repository.Query(q => q.FirstOrDefault(f => f.Id == id));
            data.shouldFollow = !data.shouldFollow;
            if (data.shouldFollow)
            {
                data.AddSupportInfo(text, user);
            }
            Repository.Update(data);
            Repository.Save();
        }

        public void UpdateSupporterInfo(long id, string text, User user)
        {
            var data = Repository.Query(q => q.FirstOrDefault(f => f.Id == id));
            data.AddSupportInfo(text, user);
            Repository.Update(data);
            Repository.Save();
        }

        public void UpdateRatingShownToGuest(long id, bool showRate)
        {
            var data = Repository.Query(q => q.FirstOrDefault(f => f.Id == id));
            data.RatingShownToGuest = showRate;
            Repository.Update(data);
            Repository.Save();
        }

        public void UpdatePaymentGTAGRegistered(long id, bool value)
        {
            var data = Repository.Query(q => q.FirstOrDefault(f => f.Id == id));
            data.PaymentGTAGRegistered = value;
            Repository.Update(data);
            Repository.Save();
        }

        public void UpdateDisableAutoCancel(long id, bool value)
        {
            var data = Repository.Query(q => q.FirstOrDefault(f => f.Id == id));
            data.DisableAutoCancel = value;
            Repository.Update(data);
            Repository.Save();
        }

        public void UpdateAccVisitedByGuest(long id, bool value)
        {
            var data = Repository.Query(q => q.FirstOrDefault(f => f.Id == id));
            data.AccVisitedByGuest = value;
            Repository.Update(data);
            Repository.Save();
        }

        public void UpdateHostCallDate(long id, DateTime value)
        {
            var data = Repository.Query(q => q.FirstOrDefault(f => f.Id == id));
            data.HostCallDate = value;
            Repository.Update(data);
            Repository.Save();
        }

        public void UpdateGuestCallDate(long id, DateTime value)
        {
            var data = Repository.Query(q => q.FirstOrDefault(f => f.Id == id));
            data.GuestCallDate = value;
            Repository.Update(data);
            Repository.Save();
        }

        public int UpdateCallState(long id, string hostOrGuest)
        {
            var data = Repository.Query(q => q.FirstOrDefault(f => f.Id == id));
            var newState = 0;
            if (hostOrGuest == "h")
            {
                if (data.HostCallState == (int)Reserve.CallState.NotCalled)
                {
                    data.HostCallState = (int)Reserve.CallState.Called;
                }
                else if (data.HostCallState == (int)Reserve.CallState.Called)
                {
                    data.HostCallState = (int)Reserve.CallState.Answered;
                }
                newState = data.HostCallState;
            }
            else if (hostOrGuest == "g")
            {
                if (data.GuestCallState == (int)Reserve.CallState.NotCalled)
                {
                    data.GuestCallState = (int)Reserve.CallState.Called;
                }
                else if (data.GuestCallState == (int)Reserve.CallState.Called)
                {
                    data.GuestCallState = (int)Reserve.CallState.Answered;
                }
                newState = data.GuestCallState;
            }
            Repository.Update(data);
            Repository.Save();
            return newState;
        }

        public bool StartStay(long reserveId, int user_id, out string msg,
            ActionSourceEnum actionSource, int doerUserId)
        {
            var reserve = Repository.Find(reserveId);
            var advertise = reserve.Advertise;
            if (reserve.UserID != user_id)
            {
                msg = "شما مجوز این کار را ندارید";
                return false;
            }
            if (accounting.IsReservePaidCompletely(reserveId) == false)
            {
                msg = "قبل از شروع سفر باید مبلغ رزرو یا مبلغ بیعانه را پرداخت کنید";
                return false;
            }
            DateTime canStartTime;
            if (reserve.CanReserveStarted(out canStartTime) == false)
            {
                var canStartDateToPersian = DateTimeUtility.GregorianToPersianDate(canStartTime);
                canStartDateToPersian += " ساعت 8 صبح";
                msg = "سفر شما هنوز شروع نشده و تا تاریخ " + canStartDateToPersian + " نمیتوانید دکمه شروع سفر را بزنید";
                return false;
            }
            mediator.Send(new SetReserveStatusCommand(reserveId, ReserveStatus.Started, true,
                actionSource, doerUserId));
            msg = "سفر شما آغاز شد";
            return true;
        }

        public void UpdateExcludeGroup(long id, bool value)
        {
            var data = Repository.Query(q => q.FirstOrDefault(f => f.Id == id));
            data.ExcludeGroupPayment = value;
            Repository.Update(data);
            Repository.Save();
        }

        public void UpdatePaymentHasError(long id, bool value)
        {
            var data = Repository.Query(q => q.FirstOrDefault(f => f.Id == id));
            data.PaymentHasError = value;
            Repository.Update(data);
            Repository.Save();
        }

        public void UpdatePaymentHasError(IList<long> ids, bool value)
        {
            var data = Repository.Query(q => q.Where(w => ids.Contains(w.Id)));
            foreach (var item in data)
            {
                item.PaymentHasError = value;
                Repository.Update(item);
            }
            Repository.Save();
        }

        public void UpdateCanselDiscussion(long id, string text, User user)
        {
            var data = Repository.Query(q => q.FirstOrDefault(f => f.Id == id));
            data.AddCancelDiscussion(text, user);
            Repository.Update(data);
            Repository.Save();
        }

        public bool Delete(long id, out string msg)
        {
            var item = Repository.Query(q => q.FirstOrDefault(f => f.Id == id));
            if (accounting.GetReservePaidAmount(id, StatusStringType.Guest) > 0)
            {
                msg = "مهمان مبلغ رزرو را پرداخت کرده است و تا زمان عودت مبلغ، قابل حذف نیست";
                return false;
            }
            var stateCategory = item.GetStateCategory();
            if (stateCategory == ReserveCategory.Reserved)
            {
                msg = "این سفر، رزرو نهایی شده است و نمیتواند حذف شود";
                return false;
            }
            if (stateCategory == ReserveCategory.Finished)
            {
                msg = "این سفر به پایان رسیده است و نمیتواند حذف شود";
                return false;
            }
            if (stateCategory == null)
            {
                msg = "";
                return true;
            }
            item.Status = ReserveStatus.Deleted;
            Repository.Update(item);
            Repository.Save();
            msg = "";
            return true;
        }

        public void SetStatus(long reserveId, ReserveStatus status, bool sendSms,
            ActionLog.ActionSourceEnum actionSource, int doerUserId)
        {
            mediator.Send(new SetReserveStatusCommand(reserveId, status, sendSms,
                actionSource, doerUserId, actionSource == ActionSourceEnum.AdminPanel));
        }

        public bool SetHostResponse(long reserveId, HostResponseEnum response,
            bool sendSms, ActionLog.ActionSourceEnum actionSource, int doerUserId)
        {
            return mediator.Send(new SetHostResponseCommand(reserveId,
                response, sendSms, actionSource, doerUserId)).Result;
        }

        public bool CashPay(long reserveId, out string msg,
            int userId, ActionSourceEnum actionSource, int doerUserId)
        {
            var reserve = Repository.Find(reserveId);
            if (reserve.UserID != userId ||
                reserve.Status != ReserveStatus.Reserved)
            {
                msg = "شما مجوز این کار را ندارید";
                return false;
            }
            DateTime canStartTime;
            if (!reserve.CanReserveStarted(out canStartTime))
            {
                var canStartDateToPersian = DateTimeUtility.GregorianToPersianDate(canStartTime);
                canStartDateToPersian += " ساعت 8 صبح";
                msg = "سفر شما هنوز شروع نشده و تا تاریخ " + canStartDateToPersian + " نمیتوانید دکمه پرداخت نقدی را بزنید";
                return false;
            }
            mediator.Send(new SetReserveStatusCommand(reserveId,
                ReserveStatus.CashPay, true, actionSource, doerUserId));
            msg = "شما پرداخت نقدی را انتخاب کردید. پرداخت شما باید توسط میزبان تایید شود.";
            return true;
        }

        public bool ConfirmCashPay(long reserveId, bool paid, out string msg,
            int userId, ActionSourceEnum actionSource, int doerUserId)
        {
            var reserve = Repository.Find(reserveId);
            var advertise = reserve.Advertise;
            if (advertise.UserID != userId || reserve.Status != ReserveStatus.CashPay)
            {
                msg = "شما مجوز این کار را ندارید";
                return false;
            }
            if (paid)
            {
                DateTime canStartTime;
                if (reserve.CanReserveStarted(out canStartTime))
                {
                    mediator.Send(new SetReserveStatusCommand(reserveId,
                        ReserveStatus.Started, true, actionSource, doerUserId, true));
                }
                msg = "شما پراخت نقدی مهمان را تایید کردید";
            }
            else
            {
                mediator.Send(new SetReserveStatusCommand(reserveId,
                        ReserveStatus.Reserved, true, actionSource, doerUserId));
                msg = "شما پراخت نقدی مهمان را تایید نکردید";
            }
            return true;
        }

        public void ExistHostGuest(int userId, out bool hasHost, out bool hasGuest)
        {
            hasHost = Repository.Query(q => q.Any(w => w.Advertise.UserID == userId));
            hasGuest = Repository.Query(q => q.Any(w => w.UserID == userId));
        }

        public bool UserHasRefundInProgress(int userId)
        {
            var reserves = Repository.Query(q => q);
            reserves = reserves.Where(x => x.UserID == userId);
            return reserves.Any(x => x.Status == Reserve.ReserveStatus.CancelRequestByGuest ||
                x.Status == Reserve.ReserveStatus.CancelRequestByHost);
        }

        public void CancelReserve(User user, long reserve_id,
            int cancel_reason_code, string cancel_reason_string,
            bool is_host, out string msg, out bool isPending,
            ActionSourceEnum actionSource, int doerUserId)
        {
            var reserve = Repository.Find(reserve_id);
            string cancel_reason;
            if (cancel_reason_code < 0)
                cancel_reason = cancel_reason_string;
            else
                cancel_reason = Reserve.CancelReasons[cancel_reason_code];
            reserve.CancelReason = cancel_reason;
            Repository.Update(reserve);
            Repository.Save();
            var advertise = reserve.Advertise;
            if (accounting.GetReservePaidAmount(reserve.Id, StatusStringType.Guest) > 0
                || reserve.DepositPrice == 0)
            {
                if (is_host)
                {
                    msg = "درخواست لغو شما ارسال شد و در درست بررسی است";
                }
                else
                {
                    var hostUser = Repository.Find<User, int>(advertise.UserID);
                    var host_contact_str = "شماره تماس: " + hostUser.GetLocalPhoneNumber(User.PhoneType.OtherMobile1) +
                        (!string.IsNullOrEmpty(hostUser.Mobile2) ? " و " + hostUser.GetLocalPhoneNumber(User.PhoneType.OtherMobile2) : "");
                    msg = string.Format("لطفا با میزبان خود آقا/خانم {0} تماس بگیرید {1} و خسارت کنسلی را تایید کنید و نتیجه را به ما اعلام فرمایید. شماره تماس املاک باشی: 02632565304", hostUser.FullName, host_contact_str);
                }
                isPending = true;
                mediator.Send(new SetReserveStatusCommand(reserve_id,
                    is_host ? ReserveStatus.CancelRequestByHost :
                    ReserveStatus.CancelRequestByGuest, true, actionSource, doerUserId));
            }
            else
            {
                isPending = false;
                mediator.Send(new SetReserveStatusCommand(reserve_id,
                    is_host ? ReserveStatus.CanceledByHost :
                    ReserveStatus.CanceledByGuest, true, actionSource, doerUserId));
                msg = "درخواست رزرو شما با موفقیت لغو شد";
            }
        }

        public void RefuseCancelReserve(User user, long reserve_id,
            bool is_host, out string msg, ActionSourceEnum actionSource,
            int doerUserId)
        {
            var reserve = Repository.Find(reserve_id);
            mediator.Send(new SetReserveStatusCommand(reserve_id,
                reserve.CancelState, true, actionSource, doerUserId));
            msg = is_host ? "رزرو مورد نظر از حالت لغو خارج شد" :
                "درخواست رزرو شما با موفقیت از حالت لغو خارج شد";
        }

        public bool UserHasSimilarReserve(int userId, long advertiseId, DateTime startDate, DateTime endDate)
        {
            if (userId < 1)
                return false;
            return Repository.Find<User, int>(userId).UserHasSimilarReserve(advertiseId, startDate, endDate);
        }

        public bool CanReserveStarted(long reserveId, out DateTime canStartTime)
        {
            var reserve = Repository.Query(q => q.FirstOrDefault(f => f.Id == reserveId));
            canStartTime = new DateTime(reserve.StartDate.Year, reserve.StartDate.Month,
                reserve.StartDate.Day, 8, 0, 0);
            return DateTime.Now > canStartTime;
        }

        public bool FinishStay(long reserveId, int userId, out string msg, ActionSourceEnum actionSource, int doerUserId,
            bool sendSms = true)
        {
            var reserve = Repository.Query(q => q.FirstOrDefault(f => f.Id == reserveId));
            if (reserve.UserID != userId ||
                reserve.Status != ReserveStatus.Started)
            {
                msg = "شما مجوز این کار را ندارید";
                return false;
            }
            DateTime canFinishTime;
            canFinishTime = new DateTime(reserve.EndDate.Year, reserve.EndDate.Month,
                reserve.EndDate.Day, 8, 0, 0);

            if (!(DateTime.Now > canFinishTime))
            {
                var canFinishDateToPersian = DateTimeUtility.GregorianToPersianDate(canFinishTime);
                canFinishDateToPersian += " ساعت 8 صبح";
                msg = "سفر شما هنوز تمام نشده و تا تاریخ " + canFinishDateToPersian + " نمیتوانید دکمه پایان سفر را بزنید";
                return false;
            }
            mediator.Send(new SetReserveStatusCommand(reserveId, ReserveStatus.Completed, sendSms,
                actionSource, doerUserId));
            msg = "سفر شما پایان یافت. لطفا با امتیاز دهی و نظر درباره اقامتگاه دیگران را راهنمایی کنید";
            return true;
        }

        public ReserveStatus FinalizeReserve(long reserveId, long transactionId,
            long paidAmount, ReservePaymentMethod paymentMethod,
            ActionSourceEnum actionSource, int doerUserId,
            int payerUserId = -1, long couponId = 0, long prizePrice = 0,
            bool sendSms = true)
        {
            return mediator.Send(new FinalizeReserveCommand(reserveId, transactionId,
                paidAmount, paymentMethod, actionSource, doerUserId, payerUserId,
                couponId, prizePrice, sendSms)).Result;
        }

        public bool SystemCancelReserve(long reserveId)
        {
            return mediator.Send(new SystemCancelReserveCommand(reserveId, true, true)).Result;
        }

        public IList<Reserve> GetReserveDashboardItems(
            User currentUser, ReserveManagerSelectType selectType,
            int category, string reserve_id, int status,
            out Dictionary<ReserveCategory, int> countDict)
        {
            if (selectType == ReserveManagerSelectType.All)
            {
                if (currentUser.UserGeneralType == (int)User.UserGeneralTypeEnum.Guest)
                    selectType = ReserveManagerSelectType.Guest;
                else
                    selectType = ReserveManagerSelectType.Host;
            }
            if (category < 0 && string.IsNullOrEmpty(reserve_id))
            {
                category = 0;
            }
            reserve_id = StringUtility.PersianNumberToEnglish(reserve_id);
            long reserveIdLong = -1;
            if (!string.IsNullOrEmpty(reserve_id))
                reserveIdLong = long.Parse(reserve_id);
            IEnumerable<Reserve> reserves = null;
            if (selectType != ReserveManagerSelectType.Host)
            {
                reserves = Repository.Query(q => q.Where(w => w.UserID == currentUser.Id));
            }
            if (selectType != ReserveManagerSelectType.Guest)
            {
                reserves = Repository.Query(q => q.Where(w => w.HostUserID == currentUser.Id));
            }
            if (reserveIdLong > 0)
            {
                reserves = reserves.Where(w => w.Id == reserveIdLong);
            }
            if (status > -1)
            {
                reserves = reserves.Where(x => (int)x.Status == status ||
                        (status == 1 ? x.Status == 0 : false));
            }
            countDict = new Dictionary<ReserveCategory, int>();
            var categoryEnumList = Enum.GetValues(typeof(ReserveCategory)) as ReserveCategory[];
            foreach (var ReserveCategory in categoryEnumList)
            {
                var states = GetReserveCategoryStates(ReserveCategory);
                countDict[ReserveCategory] = reserves.Count(c => states.ToList().Contains((int)c.Status));
            }
            if (category > -1)
            {
                var states = GetReserveCategoryStates(
                    (ReserveCategory)category).ToList();
                reserves = reserves.Where(x => states.Contains((int)x.Status));
            }
            reserves = reserves.ToList();
            if (selectType == ReserveManagerSelectType.Host)
            {
                foreach (var item in reserves)
                {
                    item.InitialPriority = GetReserveInitialPriorityHost((int)item.Status);
                    item.Priority = GetReservePriorityHost((int)item.Status);
                }
            }
            else
            {
                foreach (var item in reserves)
                {
                    item.InitialPriority = GetReserveInitialPriorityGuest((int)item.Status);
                    item.Priority = GetReservePriorityGuest((int)item.Status);
                }
            }
            return reserves.OrderBy(x => x.InitialPriority)
                    .ThenBy(x => x.Priority)
                    .ThenByDescending(x => x.Id).ToList();
        }

        //TODO: temp
        public void SetHangfireSchedules_GuestCall()
        {
            var nowDate = DateTime.Now;
            //var date = new DateTime(nowDate.Year, nowDate.Month, nowDate.Day - 1, 22, 51, 0);
            var date = new DateTime(nowDate.Year, nowDate.Month, nowDate.Day, 8, 0, 0);
            var callTime = new DateTime(nowDate.Year, nowDate.Month, nowDate.Day, nowDate.Hour, nowDate.Minute, nowDate.Second);
            var reserves = Repository.Query(q => q.Where(w => w.Status == ReserveStatus.WaitForReserve && w.HostResponseDate > date).ToList());

            foreach (var item in reserves)
            {
                var delay = new TimeSpan(0, 8, 0);
                delay = DateTimeUtility.DelayAvoidingNightTime(delay);
                mediator.Schedule(new SendPayReserveCallCommand(item.Id), delay);
            }
        }

        //TODO: temp
        public void SetHangfireSchedules_HostCall()
        {
            var nowDate = DateTime.Now;
            //var date = new DateTime(nowDate.Year, nowDate.Month, nowDate.Day - 1, 22, 51, 0);
            var date = new DateTime(nowDate.Year, nowDate.Month, nowDate.Day, 8, 0, 0);
            var callTime = new DateTime(nowDate.Year, nowDate.Month, nowDate.Day, nowDate.Hour, nowDate.Minute, nowDate.Second);
            var reserves = Repository.Query(q => q.Where(w => w.Status == ReserveStatus.WaitForResponse && w.CreateDate > date).ToList());

            foreach (var item in reserves)
            {
                var delay = new TimeSpan(0, 8, 0);
                delay = DateTimeUtility.DelayAvoidingNightTime(delay);
                callTime = DateTime.Now + delay;
                if (callTime.Hour < 8)
                    callTime = new DateTime(callTime.Year, callTime.Month, callTime.Day, 8, 0, 0);
                delay = callTime - DateTime.Now;
                mediator.Schedule(new SendReserveRequestCallCommand(item.Id), delay);
            }
        }

        //TODO: temp
        public void SetHangfireSchedules_ReservedState()
        {
            var reserves = Repository.Query(q => q.Where(w => (w.Status == ReserveStatus.Reserved || w.Status == ReserveStatus.CashPay) && w.EndDate >= DateTime.Now).ToList());
            foreach (var item in reserves)
            {
                var finishDelay = new DateTime(item.EndDate.Year, item.EndDate.Month, item.EndDate.Day, 12, 0, 0) - DateTime.Now;

                mediator.Schedule(new SetReserveStatusCommand(item.Id,
                    ReserveStatus.Completed, true, ActionSourceEnum.Background, 0), finishDelay);
                mediator.Schedule(new FinishStayMessageCommand(item.Id), finishDelay);

                var beforeStart = new DateTime(item.StartDate.Year, item.StartDate.Month, item.StartDate.Day, 12, 0, 0) - DateTime.Now;
                if (beforeStart.TotalMilliseconds > 0)
                {
                    var onStart = beforeStart.Add(new TimeSpan(2, 0, 0));
                    mediator.Schedule(new SetReserveStatusCommand(item.Id,
                        ReserveStatus.Started, true, ActionSourceEnum.Background, 0), onStart);
                }
            }
        }

        //TODO: temp
        public void SetHangfireSchedules_StartedState()
        {
            var reserves = Repository.Query(q => q.Where(w => w.Status == ReserveStatus.Started && w.EndDate >= DateTime.Now).ToList());
            foreach (var item in reserves)
            {
                var finishDelay = new DateTime(item.EndDate.Year, item.EndDate.Month, item.EndDate.Day, 12, 0, 0) - DateTime.Now;

                mediator.Schedule(new SetReserveStatusCommand(item.Id,
                    ReserveStatus.Completed, true, ActionSourceEnum.Background, 0), finishDelay);
                mediator.Schedule(new FinishStayMessageCommand(item.Id), finishDelay);
            }
        }

        public Reserve GetReserveIncludingSupport(long id)
        {
            return Repository.Query(q =>
                q.Where(f => f.Id == id))
                .Include("GuestUser.ReserveSupportsAsGuest")
                .FirstOrDefault();
        }

        public IQueryable<Reserve> GetReservesIncludingSupport(List<long> ids)
        {
            return Repository.Query(q =>
                q.Where(f => ids.Contains(f.Id))
                .Include("GuestUser.ReserveSupportsAsGuest"));
        }

        public VoucherDTO GenerateVoucher(long reserveId, int currentUserId)
        {
            var reserve = Repository.Find(reserveId);
            if (reserve.UserID != currentUserId)
            {
                return null;
            }
            var notReserved = (reserve.GetStateCategory() != ReserveCategory.Reserved &&
                reserve.GetStateCategory() != ReserveCategory.Finished) ||
                reserve.Status == ReserveStatus.CancelRequestByGuest ||
                reserve.Status == ReserveStatus.CancelRequestByHost;
            if (notReserved == true && reserve.Status != Reserve.ReserveStatus.WaitForReserve)
            {
                return null;
            }
            var paidAmount = notReserved == false ? accounting.GetReserveGuestPaidAmount(reserve.ReservePayments)
                + reserve.CouponPrice + reserve.PrizePrice : 0;
            return VoucherDTO.Generate(reserve, paidAmount, notReserved == true);
        }
    }
}
