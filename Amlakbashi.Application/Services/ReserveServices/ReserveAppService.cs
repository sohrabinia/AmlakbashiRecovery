using Amlakbashi.Core.Common.AppService;
using Amlakbashi.Core.Common.Repository;
using Amlakbashi.Application.Services.ReserveServices.Interfaces;
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
using Amlakbashi.Core.Infrastructure.StyleHelpers;
using Amlakbashi.Core.DTOs.WebService.Responses.Reserves;
using Amlakbashi.Core.DTOs.WebService.Requests.Reserves;
using Amlakbashi.Application.DTOs;
using System.Threading.Tasks;
using Amlakbashi.Core.Infrastructure.PriceHelpers.Interfaces;
using Amlakbashi.Mediator.Commands.UserCommands;
using Amlakbashi.Mediator.Events.ReserveEvents;
using Amlakbashi.Core.Infrastructure.UserContact;

namespace Amlakbashi.Application.Services.ReserveServices
{
    internal class ReserveAppService : AppServiceBase<Reserve, long>, IReserveAppService
    {
        private readonly IMediator mediator;
        private readonly IAccountingFacade accounting;
        private readonly IReserveSupportAppService reserveSupportService;
        private readonly IPriceCalculator priceCalculator;
        public ReserveAppService(IRepository<Reserve, long> repository,
            IMediator mediator,
            IAccountingFacade accounting,
            IReserveSupportAppService reserveSupportService,
            IPriceCalculator priceCalculator) : base(repository)
        {
            this.mediator = mediator;
            this.accounting = accounting;
            this.reserveSupportService = reserveSupportService;
            this.priceCalculator = priceCalculator;
        }

        public IList<Reserve> Filter(ReserveIndexDTO dto, int currentUserId)
        {
            var reserves = Repository.Query(q => q.Where(w => w.Status != Reserve.ReserveStatus.Deleted));

            if (dto.MainFilter == 0)
            {
                reserves = reserves.Where(x => !x.Archive);
            }
            else if (dto.MainFilter == 2)
            {
                reserves = reserves.Where(x => x.Archive);
            }

            if (dto.InstantReserveFilter == 0)
            {
                reserves = reserves.Where(x => !x.InstantReserve);
            }
            else if (dto.InstantReserveFilter == 1)
            {
                reserves = reserves.Where(x => x.InstantReserve);
            }

            if (dto.ShouldFollow)
            {
                reserves = reserves.Where(x => x.shouldFollow);
            }
            if (dto.DisableAutoCancel)
            {
                reserves = reserves.Where(x => x.DisableAutoCancel);
            }
            if (dto.AccVisited)
            {
                reserves = reserves.Where(x => x.AccVisitedByGuest);
            }
            if (dto.ReserveId > 0)
            {
                reserves = reserves.Where(x => x.Id == dto.ReserveId);
            }
            if (dto.AdvertiseId > 0)
            {
                reserves = reserves.Where(x => x.AdvertiseID == dto.AdvertiseId);
            }
            if (dto.GuestUserId > 0)
            {
                reserves = reserves.Where(x => x.UserID == dto.GuestUserId);
            }
            if (dto.ReserveStatus > -1)
            {
                reserves = reserves.Where(x => (int)x.Status == dto.ReserveStatus);
            }
            if (dto.HostResponseStatus > -1)
            {
                var hostResp = (HostResponseEnum)dto.HostResponseStatus;
                reserves = reserves.Where(x => x.HostResponse == hostResp);
            }
            if (dto.GeneralStatus > -1)
            {
                if (dto.GeneralStatus == 0)
                {
                    reserves = reserves.Where(x => x.ReservePayments.Any(a => a.PaymentType == (int)ReservePaymentType.GuestClearing
                        || a.PaymentType == (int)ReservePaymentType.GuestDeposite));
                }
            }
            if (dto.HostUserId > 0)
            {
                reserves = reserves.Where(x => x.Advertise.UserID == dto.HostUserId);
            }
            if (!string.IsNullOrEmpty(dto.SiteClearingDate))
            {
                reserves = reserves.Where(x => x.Status == Reserve.ReserveStatus.Reserved ||
                    x.Status == Reserve.ReserveStatus.Started ||
                    x.Status == Reserve.ReserveStatus.Completed ||
                    x.Status == Reserve.ReserveStatus.CashPay);
                var gregorian_clearing_date = DateTimeUtility.PersianDateToGregorian(
                    StringUtility.PersianNumberToEnglish(dto.SiteClearingDate).Replace('/', ','));
                reserves = reserves.Where(w => (EF.Functions.DateDiffDay(w.StartDate, w.EndDate) > 1 ?
                    w.StartDate.AddDays(2) : w.EndDate) <= gregorian_clearing_date);
            }
            if (dto.StayDurationFrom > 0)
            {
                reserves = reserves.Where(w => EF.Functions.DateDiffDay(w.StartDate, w.EndDate) >= dto.StayDurationFrom);
            }
            if (dto.StayDurationTo > 0)
            {
                reserves = reserves.Where(w => EF.Functions.DateDiffDay(w.StartDate, w.EndDate) <= dto.StayDurationTo);
            }
            if (!string.IsNullOrEmpty(dto.ReserveFromDate))
            {
                var gregorian_date = DateTimeUtility.PersianDateToGregorian(
                    StringUtility.PersianNumberToEnglish(dto.ReserveFromDate).Replace('/', ','));
                reserves = reserves.Where(x => x.StartDate >= gregorian_date);
            }
            if (!string.IsNullOrEmpty(dto.ReserveToDate))
            {
                var gregorian_date = DateTimeUtility.PersianDateToGregorian(
                    StringUtility.PersianNumberToEnglish(dto.ReserveToDate).Replace('/', ','));
                reserves = reserves.Where(x => x.EndDate <= gregorian_date);
            }
            if (!string.IsNullOrEmpty(dto.ReserveEndDate))
            {
                var gregorian_date = DateTimeUtility.PersianDateToGregorian(
                    StringUtility.PersianNumberToEnglish(dto.ReserveEndDate).Replace('/', ','));
                reserves = reserves.Where(x => x.EndDate == gregorian_date);
            }
            if (dto.SiteClearedStatus == 0)//payed
            {
                reserves = reserves.Where(w => w.ReservePayments.Any(a => a.PaymentType == (int)ReservePaymentType.SiteClearingToHost) ||
                    (w.ReservePayments.Any(a => a.PaymentType == (int)ReservePaymentType.SiteDepositeToHost) &&
                    w.ReservePayments.Any(a => a.PaymentType == (int)ReservePaymentType.GuestClearing) == false));
            }
            else if (dto.SiteClearedStatus == 1)//not payed
            {
                reserves = reserves.Where(w => (w.Status == Reserve.ReserveStatus.Reserved ||
                    w.Status == Reserve.ReserveStatus.Started ||
                    w.Status == Reserve.ReserveStatus.Completed ||
                    w.Status == Reserve.ReserveStatus.CashPay) &&
                    w.ReservePayments.Any(a => a.PaymentType == (int)ReservePaymentType.SiteClearingToHost) == false);

                var matchedIds = new List<long>();
                foreach (var item in reserves)
                {
                    var guestPaidAmount = accounting.GetReservePaidAmount(item.Id, StatusStringType.Guest);
                    var payablePrice = PriceUtility.CalculateHostPayablePrice(item.TotalPrice, guestPaidAmount, item.CouponPrice, item.PrizePrice);
                    if (item.ReservePayments.Any(a => a.PaymentType == (int)ReservePaymentType.SiteDepositeToHost))
                    {
                        payablePrice -= item.ReservePayments.FirstOrDefault(f => f.PaymentType == (int)ReservePaymentType.SiteDepositeToHost).Price;
                    }
                    if (payablePrice > 0)
                    {
                        matchedIds.Add(item.Id);
                    }
                }
                reserves = reserves.Where(x => matchedIds.Contains(x.Id));
            }
            else if (dto.SiteClearedStatus == 2)//refund done
            {
                var matchedIds = new List<long>();
                foreach (var item in reserves)
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
                reserves = reserves.Where(x => matchedIds.Contains(x.Id));
            }
            else if (dto.SiteClearedStatus == 3)//should refund
            {
                var matchedIds = new List<long>();
                var tempModel = reserves.Where(w => w.ReservePayments.Any(x =>
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
                reserves = reserves.Where(x => matchedIds.Contains(x.Id));
            }
            if (dto.ReserveSupportStatus > 0)
            {
                reserves = reserves.Include("GuestUser.ReserveSupportsAsGuest");
                var st = (ReserveSupport.SupporterStatus)dto.ReserveSupportStatus;
                if (st == ReserveSupport.SupporterStatus.SupportingByYou)
                {
                    dto.SupporterId = currentUserId;
                }
                else
                {
                    if (st != ReserveSupport.SupporterStatus.Done)
                    {
                        dto.SupporterId = -1;
                    }
                    var now = DateTime.Now.Date;
                    reserves = reserves.Where(x => x.EndDate > now);
                    reserves = reserveSupportService.FilterBySupporterStatus(currentUserId, reserves, st);
                }
            }
            if (dto.SupporterId > 0)
            {
                IList<ReserveSupport> reserveSupports = reserveSupportService.GetListBySupporterId(dto.SupporterId);
                var reserve_ids = new List<long>();
                foreach (var reserveSupport in reserveSupports)
                {
                    reserve_ids.AddRange(reserveSupport.GetAllReserveIds());
                }
                reserve_ids = reserve_ids.Distinct().ToList();
                reserves = reserves.Where(x => reserve_ids.Contains(x.Id));
            }
            if (dto.HostCardStatus > -1)
            {
                reserves = reserves.Include("HostUser.BankCards").Include("ReservePayments");
                var filteredIds = new List<long>();
                if (dto.HostCardStatus == 0) //shaba
                {
                    foreach (var item in reserves)
                    {
                        if (item.HostUser.BankCards.Count > 0 && !string.IsNullOrEmpty(item.HostUser.BankCards.First().ShabaNumber))
                        {
                            filteredIds.Add(item.Id);
                        }
                    }
                }
                else if (dto.HostCardStatus == 1) // bank card
                {
                    foreach (var item in reserves)
                    {
                        if (item.HostUser.BankCards.Count > 0 && string.IsNullOrEmpty(item.HostUser.BankCards.First().ShabaNumber) &&
                            !string.IsNullOrEmpty(item.HostUser.BankCards.First().BankCardNumber))
                        {
                            filteredIds.Add(item.Id);
                        }
                    }
                }
                else if (dto.HostCardStatus == 2) // none
                {
                    foreach (var item in reserves)
                    {
                        if (item.Id == 310978)
                        {
                            var test = item.HostUser.BankCards;
                        }
                        if (item.HostUser.BankCards.Count == 0 ||
                            (string.IsNullOrEmpty(item.HostUser.BankCards.First().ShabaNumber) &&
                            string.IsNullOrEmpty(item.HostUser.BankCards.First().BankCardNumber)))
                        {
                            filteredIds.Add(item.Id);
                        }
                    }
                }
                reserves = reserves.Where(x => filteredIds.Contains(x.Id));
                //foreach (var item in reserves)
                //{
                //    item.Temp_HostPayablePrice = PriceUtility.CalculateHostPayablePrice(item.TotalPrice,
                //        accounting.GetReservePaidAmount(item.ReservePayments.ToList(),
                //            StatusStringType.Guest), item.CouponPrice, item.PrizePrice);
                //}
                //reserves = reserves.OrderByDescending(x => x.Temp_HostPayablePrice);
            }
            //else
            //{
            //    reserves = reserves.OrderByDescending(x => x.Id);
            //}

            dto.PagingInfo = new Core.DTOs.PagingDTO(dto.Page, reserves.Count());

            return reserves.OrderByDescending(x => x.Id).Skip((dto.Page - 1) * dto.PagingInfo.PageItemCount).Take(dto.PagingInfo.PageItemCount).ToList();
        }

        public ReserveListResponse Filter(ReserveGetListRequest request)
        {
            var reserves = Repository.Query(q => q);
            if (request.userId > 0)
            {
                reserves = request.panel == User.UserGeneralTypeEnum.Guest ?
                    reserves.Where(x => x.UserID == request.userId) :
                    reserves.Where(x => x.HostUserID == request.userId);
            }

            var categoryStatus = request.panel == User.UserGeneralTypeEnum.Guest ?
                Reserve.GetGuestCategoryStates(request.category) :
                Reserve.GetHostCategoryStates(request.category);
            reserves = reserves.Where(x => categoryStatus.Contains(x.Status));

            reserves = reserves.OrderByDescending(x => x.CreateDate);

            var pagedList = reserves.ToPagedList(request.page, request.pageItemCount);
            var response = new ReserveListResponse()
            {
                pagingInfo = pagedList.PagingInfo,
                reserveList = pagedList.List.Select(x => (ReserveResponse)x).ToList()
            };
            return response;
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

        public ReserveIndexDetailsInfoDTO GetReserveIndexDetailsInfo(Reserve reserve)
        {
            DateTime depositePayDate;
            long depositeTransactionId;
            var depositePaidPrice = reserve.GetReservePaymentPrice(ReservePayment.ReservePaymentType.GuestDeposite,
                out depositePayDate, out depositeTransactionId, reserve.UserID);
            var hostSitePortionPrice = reserve.TotalPrice / 10;
            DateTime totalPayDate;
            long totalTransactionId;
            var totalPaidPrice = reserve.GetReservePaymentPrice(ReservePayment.ReservePaymentType.GuestClearing,
                out totalPayDate, out totalTransactionId, 0);
            DateTime clearingPayDate;
            long clearingTransactionId;
            var clearingPaidPrice = reserve.GetReservePaymentPrice(ReservePayment.ReservePaymentType.SiteClearingToHost,
                out clearingPayDate, out clearingTransactionId, 0);
            DateTime hostClearingDepositeDate;
            long hostClearingDepositeTransactionId;
            var hostClearingDepositeAmount = reserve.GetReservePaymentPrice(ReservePayment.ReservePaymentType.SiteDepositeToHost,
                out hostClearingDepositeDate, out hostClearingDepositeTransactionId, 0);
            DateTime refundPayDate;
            long refundTransactionId;
            var refundPaidPrice = reserve.GetReservePaymentPrice(ReservePayment.ReservePaymentType.SiteRefundToGuest,
                out refundPayDate, out refundTransactionId, 0);
            var generatedPayments = new List<PaymentHelperDTO>();
            if (depositePaidPrice > 0)
            {
                generatedPayments.Add(new PaymentHelperDTO()
                {
                    title = "بیعانه",
                    type = PaymentHelperDTO.PaymentType.Deposite,
                    transactionId = depositeTransactionId,
                    amount = depositePaidPrice,
                    dateString = DateTimeUtility.GregorianToPersianDate(depositePayDate).Remove(0, 2) +
                        " " + depositePayDate.ToString("HH:mm")
                });
            }
            if (totalPaidPrice > 0)
            {
                generatedPayments.Add(new PaymentHelperDTO()
                {
                    title = "تسویه مهمان",
                    type = PaymentHelperDTO.PaymentType.Total,
                    transactionId = totalTransactionId,
                    amount = totalPaidPrice,
                    dateString = DateTimeUtility.GregorianToPersianDate(totalPayDate).Remove(0, 2) +
                        " " + totalPayDate.ToString("HH:mm")
                });
            }
            if (clearingPaidPrice > 0 || hostClearingDepositeAmount > 0)
            {
                var clearingToHostAmount = clearingPaidPrice + hostClearingDepositeAmount;
                generatedPayments.Add(new PaymentHelperDTO()
                {
                    type = PaymentHelperDTO.PaymentType.Clearing,
                    transactionId = clearingTransactionId,
                    title = "تسویه میزبان",
                    amount = clearingToHostAmount,
                    dateString = DateTimeUtility.GregorianToPersianDate(clearingPaidPrice > 0 ? clearingPayDate : hostClearingDepositeDate)
                    .Remove(0, 2) + " " + clearingPayDate.ToString("HH:mm")
                });
            }
            if (refundPaidPrice > 0)
            {
                generatedPayments.Add(new PaymentHelperDTO()
                {
                    title = "عودت مهمان",
                    type = PaymentHelperDTO.PaymentType.Refund,
                    transactionId = refundTransactionId,
                    amount = refundPaidPrice,
                    dateString = DateTimeUtility.GregorianToPersianDate(refundPayDate).Remove(0, 2) +
                        " " + refundPayDate.ToString("HH:mm")
                });
            }
            if (totalPaidPrice > 0 && hostSitePortionPrice > 0)
            {
                generatedPayments.Add(new PaymentHelperDTO()
                {
                    title = "درصد سایت",
                    type = PaymentHelperDTO.PaymentType.HostSitePortion,
                    amount = hostSitePortionPrice,
                });
            }
            var createDateString = DateTimeUtility.GregorianToPersianDate(reserve.CreateDate).Remove(0, 2);
            string lastPayTryDate;
            var reserveSupports = reserve.GetRelatedSupports();
            var generatedSupporters = new List<SupporterHelperDTO>();
            foreach (var rs in reserveSupports)
            {
                if (rs.SupporterID == null)
                {
                    continue;
                }
                var supportingReserves = rs.GetReserveIds(ReserveSupport.SupportReserveStatus.Supporting);
                var similarReserves = rs.GetReserveIds(ReserveSupport.SupportReserveStatus.Similar);
                if (supportingReserves.Contains(reserve.Id) ||
                    similarReserves.Contains(reserve.Id))
                {
                    var supporterUser = rs.Supporter;
                    var fullName = supporterUser.FullName;
                    generatedSupporters.Add(new SupporterHelperDTO()
                    {
                        name = string.IsNullOrEmpty(fullName) ?
                            supporterUser.Id.ToString() : fullName,
                        imageId = supporterUser.PhotoID == null ? 0 : (long)supporterUser.PhotoID,
                        color = similarReserves.Contains(reserve.Id) ? "#FF7F00;" : "#34A853",
                        transferReason = rs.TransferReason
                    });
                }
            }
            return new ReserveIndexDetailsInfoDTO()
            {
                Id = reserve.Id,
                TotalPrice = reserve.TotalPrice,
                DepositePrice = reserve.DepositPrice,
                TotalPaidPrice = totalPaidPrice,
                DepositePaidPrice = depositePaidPrice,
                StartDateString = DateTimeUtility.GregorianToPersianDate(
                    reserve.StartDate).Remove(0, 2),
                EndDateString = DateTimeUtility.GregorianToPersianDate(
                    reserve.EndDate).Remove(0, 2),
                GuestCount = reserve.NumberOfGuests,
                StayDays = DateTimeUtility.GetDatRangeDays(reserve.StartDate, reserve.EndDate),
                CreateDateString = createDateString + " " + reserve.CreateDate.ToString("HH:mm"),
                GuestUserId = reserve.UserID,
                Status = (int)reserve.Status,
                GuestCallState = reserve.GuestCallState,
                HostCallState = reserve.HostCallState,
                GuestCallStateColor = ReserveStyleHelper.GetCallStateColor(reserve.GuestCallState),
                HostCallStateColor = ReserveStyleHelper.GetCallStateColor(reserve.HostCallState),
                SupportInfoCount = reserve.GetSupportInfoList().Length,
                SupportInfoList = reserve.GetSupportInfoList().Reverse().ToList(),
                PaymentList = generatedPayments,
                PaymentTryCount = reserve.GetPaymentTriesCount(out lastPayTryDate),
                LastPaymentTryDate = lastPayTryDate,
                InstantReserve = reserve.InstantReserve,
                HostResponse = (int)reserve.HostResponse,
                HostResponseString = ReserveLocalization.GetHostResponseString((int)reserve.HostResponse),
                HostResponseColor = ReserveStyleHelper.GetHostResponseColor((int)reserve.HostResponse),
                HostResponseTimeString = reserve.HostResponseDate.ToString("HH:mm"),
                HostResponseDateString = DateTimeUtility.GregorianToPersianDate(reserve.HostResponseDate).Remove(0, 2),
                Supporters = generatedSupporters,
                DisableAutoCancel = reserve.DisableAutoCancel,
                AccVisitedByGuest = reserve.AccVisitedByGuest,
                ShouldFollow = reserve.shouldFollow,
            };
        }

        public ReserveIndexSupportInfoDTO GetReserveIndexSupportInfo(long reserveId)
        {
            var reserve = Repository.Find(reserveId);
            if (reserve == null)
            {
                return null;
            }
            var supportInfoList = reserve.GetSupportInfoList().Reverse().ToList();
            return new ReserveIndexSupportInfoDTO()
            {
                Id = reserveId,
                HostCallState = reserve.HostCallState,
                GuestCallState = reserve.GuestCallState,
                SupportInfoList = supportInfoList,
                SupportInfoCount = supportInfoList.Count
            };
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

        public ServiceResult<bool> Validate(ReservePostRequest request)
        {
            var serviceResult = new ServiceResult<bool>();
            var advertise = Repository.Find<Advertise, long>(request.advertiseId);
            var user = Repository.Find<User, int>(request.userId);

            if (advertise == null)
            {
                serviceResult.AddError("کد آگهی اشتباه است");
                return serviceResult;
            }
            if (request.numberOfGuest < 1)
            {
                serviceResult.AddError("لطفا تعداد مهمان را وارد کنید");
            }
            if (user.Reserves.Count(c => c.Status == ReserveStatus.WaitForResponse) >= 3)
            {
                serviceResult.AddError("شما نمی توانید همزمان بیشتر از 3 درخواست رزرو بدهید");
            }
            //var haveReservedRequest = false;
            //if (user != null && user.Reserves != null)
            //{
            //    haveReservedRequest = user.Reserves.Any(a => a.GetStateCategory() == ReserveCategory.Reserved ||
            //        a.GetStateCategory() == ReserveCategory.Finished);
            //}
            //if (((advertise.Mode == AdvertiseMode.Child && advertise.Parent.License == false) ||
            //    (advertise.Mode != AdvertiseMode.Child && advertise.License == false)) &&
            //    advertise.IsForbidden && haveReservedRequest == false)
            //{
            //    serviceResult.AddError("کاربر گرامی، طبق دستور قضایی، رزرو اقامتگاه در اصفهان فقط برای اماکن دارای مجوز از سازمان گردشگری امکان پذیر است");
            //}
            if (advertise.Status != Advertise.AdvertiseStatus.Published)
            {
                serviceResult.AddError("اقامتگاه مورد نظر در حال حاضر از دسترس خارج است");
            }
            if (request.numberOfGuest < 1)
            {
                serviceResult.AddError("لطفا تعداد نفرات را وارد کنید");
            }
            if (string.IsNullOrEmpty(request.fromDate) || string.IsNullOrEmpty(request.toDate))
            {
                serviceResult.AddError("لطفا تاریخ شروع و پایان سفر را انتخاب کنید");
            }
            if (request.fromDate == request.toDate)
            {
                serviceResult.AddError("تاریخ شروع و پایان سفر نمی توانند یکی باشند");
            }
            var startDateGregorian = DateTimeUtility.PersianDateToGregorian(request.fromDate);
            var endDateGregorian = DateTimeUtility.PersianDateToGregorian(request.toDate);
            if (startDateGregorian > endDateGregorian)
            {
                serviceResult.AddError("تاریخ ورود نمی تواند از تاریخ خروج بیشتر باشد");
            }
            var days = DateTimeUtility.GetPersianDateRangeDays(request.fromDate, request.toDate);
            if (advertise.MinReserveDays > 0 && days < advertise.MinReserveDays)
            {
                serviceResult.AddError($"برای رزرو این اقامتگاه باید حداقل {advertise.MinReserveDays} شب اقامت کنید");
            }
            if (advertise.MaxReserveDays > 0 && days > advertise.MaxReserveDays)
            {
                serviceResult.AddError($"شما می توانید حداکثر {advertise.MaxReserveDays} شب در این اقامتگاه اقامت کنید");
            }
            var todayUnix = DateTimeUtility.DateValueOfJS(DateTime.Now.Date);
            if (advertise.unixNorouzMinRequestDate > todayUnix &&
                DateTimeUtility.IsNorouz(DateTimeUtility.PersianDateRangeToList(request.fromDate, request.toDate, true, false)))
            {
                var minDateString = DateTimeUtility.GregorianToPersianDate(DateTimeUtility.JSValueToDate(advertise.unixNorouzMinRequestDate));
                serviceResult.AddError($"برای رزرو نوروزی این اقامتگاه میتوانید از تاریخ {minDateString} اقدام کنید");
            }
            var minDate = DateTime.Now.TimeOfDay.Hours > 3 ? DateTime.Now.Date : DateTime.Now.Date.AddDays(-1);
            if (startDateGregorian < minDate || endDateGregorian <= minDate)
            {
                serviceResult.AddError("تاریخ ورود و خروج گذشته است. لطفا زمان درست انتخاب کنید");
            }
            var occupiedDates = advertise.OccupiedDates().Select(s => DateTimeUtility.GregorianToPersianDate(s));
            var intersects = DateTimeUtility.PersianDateRangeToList(request.fromDate, request.toDate, true, false)
                .Intersect(occupiedDates);
            if (intersects.Any())
            {
                serviceResult.AddError("متاسفانه بعضی از روز های انتخاب شده پر هستند");
            }
            long priceWithoutDiscount, couponCalPrice;
            var total_price = priceCalculator.CalculateReservePrice(advertise, request.fromDate, request.toDate,
                request.numberOfGuest, out priceWithoutDiscount, out couponCalPrice);
            long depositePrice;
            if (days > 3)
            {
                depositePrice = (long)Math.Round(total_price * 0.3f);
            }
            else
            {
                var deposite = (long)Math.Round((double)total_price / (double)days);
                depositePrice = (long)(Math.Max(Math.Round(deposite / 1000f, 0), 1) * 1000);
            }
            if (request.userId > 0 && advertise.Count < 1 &&
                user.UserHasSimilarReserve(request.advertiseId, startDateGregorian, endDateGregorian))
            {
                serviceResult.AddError("شما یک درخواست مشابه برای این آگهی دارید");
            }
            if (serviceResult.HasError())
            {
                serviceResult.Result = false;
            }
            return serviceResult;
        }

        public async Task<ServiceResult<long>> SubmitAsync(ReservePostRequest request)
        {
            var serviceResult = new ServiceResult<long>();
            var advertise = Repository.Find<Advertise, long>(request.advertiseId);
            var user = Repository.Find<User, int>(request.userId);

            bool isInstantReserve = false;
            var startGaregorianDate = DateTimeUtility.PersianDateToGregorian(request.fromDate);
            var endGaregorianDate = DateTimeUtility.PersianDateToGregorian(request.toDate);
            if (advertise.InstantReserveStatus == Advertise.InstantReserveStatusEnum.Confirmed)
            {
                isInstantReserve = startGaregorianDate <= DateTime.Now.AddDays(advertise.MaxInstantReserveStart).Date;
            }
            long withoutDiscountPrice, couponCalculationPrice;
            var days = DateTimeUtility.GetPersianDateRangeDays(request.fromDate, request.toDate);
            var totalPrice = priceCalculator.CalculateReservePrice(advertise, request.fromDate, request.toDate, request.numberOfGuest,
                out withoutDiscountPrice, out couponCalculationPrice);
            long depositePrice;
            if (days == 1)
            {
                depositePrice = totalPrice;
            }
            else if (days > 3)
            {
                depositePrice = (long)Math.Round(totalPrice * 0.3f);
            }
            else
            {
                var deposite = (long)Math.Round((double)totalPrice / (double)days);
                depositePrice = (long)(Math.Max(Math.Round(deposite / 1000f, 0), 1) * 1000);
            }
            Reserve reserve = new Reserve()
            {
                Advertise = advertise,
                GuestUser = user,
                HostUser = advertise.User,
                StartDate = startGaregorianDate,
                EndDate = endGaregorianDate,
                CreateDate = DateTime.Now,
                HostResponseDate = DateTime.Now,
                NumberOfGuests = request.numberOfGuest,
                TotalPrice = totalPrice,
                DepositPrice = depositePrice,
                InstantReserve = isInstantReserve,
                CouponCalculationPrice = couponCalculationPrice,
                Status = isInstantReserve ? Reserve.ReserveStatus.WaitForReserve : 
                    Reserve.ReserveStatus.WaitForResponse
            };
            Insert(reserve);
            serviceResult.Result = reserve.Id;

            await mediator.Publish(new ReserveRequestEvent(reserve.Id));
            mediator.Enqueue(new UpdateAdvertiseScoreCommand(request.advertiseId));
            SendReserveRequestSmsToHost(reserve, request.fromDate, request.toDate);
            return serviceResult;
        }

        public Reserve Insert(Reserve reserve)
        {
            Repository.Insert(reserve);
            Repository.Save();
            return reserve;
        }

        public void SendReserveRequestSmsToHost(Reserve reserve, string fromDate, string toDate)
        {
            var contact = new UserContactDTO()
            {
                UserMainMobile = reserve.HostUser.MainMobile,
                UserAppNotificationToken = reserve.HostUser.AppNotificationToken,
                UserEmail = "",
                EmailConfirmed = false,
                UserFcmAppNotificationToken = reserve.HostUser.FcmAppNotificationToken,
                UserNotificationToken = reserve.HostUser.NotificationToken,
                Type = UserContactType.ReserveRequest,
                AdvertiseId = reserve.AdvertiseID.ToString(),
                UserId = string.Format("{0:n0}", reserve.TotalPrice - (reserve.TotalPrice * 0.1f)), // به جای کد مهمان، در این فیلد سهم میزبان فرستاده می شود
                ReserveId = reserve.Id.ToString(),
                Extra1 = fromDate,
                Extra2 = toDate + Environment.NewLine + "به مدت " + (reserve.EndDate - reserve.StartDate).TotalDays + " شب" +
                            Environment.NewLine + "مبلغ: " + string.Format("{0:n0}", reserve.TotalPrice) + " تومان",
                Extra3 = reserve.NumberOfGuests.ToString() + " نفر" + Environment.NewLine + "کد رزرو: " + reserve.Id
            };
            mediator.Enqueue(new SendMessageCommand(contact));
        }

        public bool UpdateNew(ReserveIndexEditDTO dto, out string msg, int doerUserId, ActionSourceEnum actionSource)
        {
            var reserve = Repository.Query(q => q.Include("Advertise.User.Advertises").FirstOrDefault(f => f.Id == dto.Id));
            var isPaidDeposit = reserve.ReservePayments.Any(a => a.PaymentType == (int)ReservePayment.ReservePaymentType.GuestDeposite ||
                a.PaymentType == (int)ReservePayment.ReservePaymentType.GuestClearing);

            //var isPaidDeposit = accounting.GetReservePaidAmount(reserve.Id, StatusStringType.Guest) >=
            //    reserve.DepositPrice - reserve.CouponPrice - reserve.PrizePrice;

            if (isPaidDeposit == false && (dto.Status == ReserveStatus.Reserved || dto.Status == ReserveStatus.CashPay ||
                dto.Status == ReserveStatus.Started || dto.Status == ReserveStatus.Completed ||
                dto.Status == ReserveStatus.CancelRequestByGuest || dto.Status == ReserveStatus.CancelRequestByHost))
            {
                msg = "مبلغ بیعانه این رزرو تسویه نشده است";
                return false;
            }

            if (isPaidDeposit && (dto.Status == ReserveStatus.WaitForReserve || dto.Status == ReserveStatus.WaitForResponse ||
                dto.Status == ReserveStatus.Rejected || dto.Status == ReserveStatus.CanceledBySystem))
            {
                msg = "وضعیت رزرو پرداخت شده نمی تواند به حالت " + ReserveLocalization.GetStatusString((int)dto.Status, Reserve.StatusStringType.Site) + " تغییر کند";
                return false;
            }

            if (string.IsNullOrEmpty(dto.PersinaStartDate) == false)
            {
                reserve.StartDate = DateTimeUtility.PersianDateToGregorian(dto.PersinaStartDate);
            }

            if (string.IsNullOrEmpty(dto.PersinaEndDate) == false)
            {
                var endDate = DateTimeUtility.PersianDateToGregorian(dto.PersinaEndDate);
                if (reserve.EndDate != endDate)
                {
                    reserve.EndDate = endDate;
                    var finishDelay = new DateTime(
                        reserve.EndDate.Year,
                        reserve.EndDate.Month,
                        reserve.EndDate.Day,
                        12, 0, 0) - DateTime.Now;
                    mediator.Schedule(new SetReserveStatusCommand(reserve.Id,
                        ReserveStatus.Completed, false, actionSource, doerUserId), finishDelay);
                    mediator.Schedule(new FinishStayMessageCommand(reserve.Id), finishDelay);
                }
            }

            if (reserve.InstantReserve &&
                !reserve.InstantReserveCancelHost &&
                dto.Status == Reserve.ReserveStatus.CanceledByHost &&
                accounting.GetReservePaidAmount(reserve.ReservePayments.ToList(), Reserve.StatusStringType.Guest) > 0)
            {
                var acc = reserve.Advertise;
                var hostUser = reserve.HostUser;
                var hostAccs = hostUser.Advertises;
                var hostCancelCount = hostAccs.Sum(x => x.InstantReserveCancels);
                int penaltyPrice = 0;
                if (hostCancelCount == 0)
                {
                    penaltyPrice = (int)Math.Floor(reserve.TotalPrice * 0.1f);
                }
                else if (hostCancelCount > 0)
                {
                    penaltyPrice = (int)Math.Floor(reserve.TotalPrice * 0.15f);
                }
                if (penaltyPrice > 0)
                {
                    long newCredit;
                    accounting.DecreaseCredit(hostUser.Id, penaltyPrice, 0, 0, out newCredit, CreditTransaction.WalletTransactionReason.Other, "جریمه لغو رزرو آنی کد " + dto.Id, null, doerUserId, ActionLog.ActionSourceEnum.AdminPanel);
                }
                reserve.InstantReserveCancelHost = true;

                mediator.Send(new IncreaseInstantReserveCancelCommand(acc.Id));
                if (hostCancelCount > hostUser.CancelInstantReserveLimit - 1)
                {
                    mediator.Send(new ChangeInstantReserveAccessCommand(hostUser.Id,
                        User.InstantReserveAccessEnum.Banned, doerUserId, actionSource));
                    mediator.Send(new UpdateInstantReserveStatusCommand(acc.Id, Advertise.InstantReserveStatusEnum.None, doerUserId, actionSource));
                }
            }
            reserve.NumberOfGuests = dto.GuestCount;
            reserve.TotalPrice = dto.TotalPrice;
            reserve.DepositPrice = dto.DepositePrice;
            reserve.CancelReason = dto.CancelReason;
            Repository.Update(reserve);
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

        public async Task<ServiceResult> StartAsync(ReservePostStartRequest request)
        {
            var serviceResult = new ServiceResult();
            var reserve = Repository.Find(request.reserveId);
            if (reserve.UserID != request.userId)
            {
                serviceResult.AddError(ReserveErrorMessages.StartAsync_UserInvalid);
            }
            if (accounting.IsReservePaidCompletely(request.reserveId) == false)
            {
                serviceResult.AddError(ReserveErrorMessages.StartAsync_NotPaid);
            }
            DateTime canStartTime;
            if (reserve.CanReserveStarted(out canStartTime) == false)
            {
                serviceResult.AddError(ReserveErrorMessages.StartAsync_DateInvalid);
            }
            if (reserve.Status != ReserveStatus.Reserved)
            {
                serviceResult.AddError(ReserveErrorMessages.StartAsync_StateInvalid);
            }
            if (serviceResult.HasError())
            {
                return serviceResult;
            }

            await mediator.Send(new SetReserveStatusCommand(request.reserveId, ReserveStatus.Started, true,
                request.actionSource, request.userId));
            return serviceResult;
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
            ActionLog.ActionSourceEnum actionSource, int doerUserId, bool force = false)
        {
            mediator.Send(new SetReserveStatusCommand(reserveId, status, sendSms,
                actionSource, doerUserId, force));
        }

        public bool SetHostResponse(long reserveId, HostResponseEnum response,
            bool sendSms, ActionLog.ActionSourceEnum actionSource, int doerUserId, bool force = false)
        {
            return mediator.Send(new SetHostResponseCommand(reserveId,
                response, sendSms, actionSource, doerUserId, force)).Result;
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

        public async Task<ServiceResult> CancelAsync(ReservePostCancelRequest request)
        {
            var serviceResult = new ServiceResult();
            var reserve = Repository.Find(request.reserveId);
            if ((request.panel == User.UserGeneralTypeEnum.Guest && reserve.UserID != request.userId) ||
                (request.panel == User.UserGeneralTypeEnum.Host && reserve.HostUserID != request.userId))
            {
                serviceResult.AddError("user is incorrect");
            }
            if (reserve.Status > ReserveStatus.Started)
            {
                serviceResult.AddError("cannot cancel this reserve");
            }
            if (serviceResult.HasError())
            {
                return serviceResult;
            }

            var targetStatus = ReserveStatus.Default;
            if (accounting.GetReservePaidAmount(reserve.Id, StatusStringType.Guest) > 0)
            {
                targetStatus = request.panel == User.UserGeneralTypeEnum.Host ?
                    ReserveStatus.CancelRequestByHost : ReserveStatus.CancelRequestByGuest;
            }
            else
            {
                targetStatus = request.panel == User.UserGeneralTypeEnum.Host ?
                    ReserveStatus.CanceledByHost : ReserveStatus.CanceledByGuest;
            }
            var changeStatusResult = await mediator.Send(new SetReserveStatusCommand(reserve.Id, targetStatus, true,
                request.actionSource, request.userId));

            if (changeStatusResult)
            {
                reserve.CancelReason = request.reason;
                Repository.Update(reserve);
                Repository.Save();
            }
            return serviceResult;
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
                var states = Reserve.GetHostCategoryStates(ReserveCategory);
                countDict[ReserveCategory] = reserves.Count(c => states.ToList().Contains(c.Status));
            }
            if (category > -1)
            {
                var states = GetHostCategoryStates((ReserveCategory)category).ToList();
                reserves = reserves.Where(x => states.Contains(x.Status));
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

        public ReserveInvoiceResponse GetInvoice(long reserveId, int currentUserId)
        {
            var reserve = Repository.Find(reserveId);
            if (reserve == null || (reserve.UserID != currentUserId && reserve.HostUserID != currentUserId) ||
                reserve.GetStateCategory() == ReserveCategory.Unsuccessful)
            {
                return null;
            }

            //var paidAmount = accounting.GetReserveGuestPaidAmount(reserve.ReservePayments)
            //    + reserve.CouponPrice + reserve.PrizePrice;

            return (ReserveInvoiceResponse)reserve;
        }

        public void SendReserveRequestCall(long reserveId)
        {
            mediator.Enqueue(new SendReserveRequestCallCommand(reserveId));
        }

        public void SendPayReserveCall(long reserveId)
        {
            mediator.Enqueue(new SendPayReserveCallCommand(reserveId));
        }

        public bool ReserveByPaymentReinquiry(long reserveId, long paymentId, out string msg)
        {
            var reserve = Repository.Find(reserveId);
            if (reserve.Status == Reserve.ReserveStatus.Reserved ||
                reserve.Status == Reserve.ReserveStatus.Started ||
                reserve.Status == Reserve.ReserveStatus.Completed ||
                reserve.Status == Reserve.ReserveStatus.CashPay)
            {
                msg = $"وضعیت این رزرو «{ReserveLocalization.GetStatusString((int)reserve.Status, StatusStringType.Site)}» می باشد.";
                return false;
            }
            var occupiedDates = reserve.Advertise.OccupiedDates();
            var reserveDateList = DateTimeUtility.DateRangeToList(reserve.StartDate, reserve.EndDate);
            foreach (var item in reserveDateList)
            {
                if (occupiedDates.Contains(item))
                {
                    msg = "تقویم آگهی مربوطه در بازه زمانی این رزرو پر می باشد";
                    return false;
                }
            }
            var payment = accounting.FindPayment(paymentId);
            if (payment.Status == Payment.PaymentStatus.NotPaid)
            {
                msg = "وضعیت پرداخت ناموفق می باشد";
                return false;
            }
            if (payment.ProductType == "Credit_Inc_Then_Res")
            {
                msg = "امکان تکمیل این رزرو وجود ندارد. مقدار پرداخت شده باید به کیف پول کاربر افزوده شود.";
                return false;
            }
            mediator.Send(new SetReserveStatusCommand(reserveId, ReserveStatus.Reserved, false,
                ActionSourceEnum.AdminPanel, payment.UserID, true));
            if (payment.CouponID > 0)
            {
                accounting.UseDiscountCouponForReserve(payment.CouponID, reserveId);
                reserve = Repository.Find(reserveId);
            }
            else if (payment.PrizePrice > 0)
            {
                accounting.UsePrizeCreditForReserve(reserveId, payment.UserID, ActionSourceEnum.AdminPanel);
                reserve = Repository.Find(reserveId);
            }
            var paymentType = payment.TotalPrice >=
                (reserve.TotalPrice - reserve.CouponPrice - reserve.PrizePrice) ?
                ReservePaymentType.GuestClearing :
                ReservePaymentType.GuestDeposite;
            var reservePayment = new ReservePayment()
            {
                CreateDate = DateTime.Now,
                UserID = reserve.UserID,
                TransactionID = long.Parse(payment.Authority),
                RefID = payment.RefID,
                ReserveID = reserve.Id,
                PaymentType = (int)paymentType,
                Price = payment.TotalPrice / 10,
                PaymentMethod = (int)ReservePaymentMethod.EPay
            };
            accounting.InsertReservePayment(reservePayment);

            msg = "عملیات با موفقیت انجام شد";
            return true;
        }
    }
}
