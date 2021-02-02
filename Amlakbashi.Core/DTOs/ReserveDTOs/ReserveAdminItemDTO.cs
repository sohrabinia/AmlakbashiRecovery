using Amlakbashi.Core.Common.Utilities;
using Amlakbashi.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using static Amlakbashi.Core.Entities.Reserve;
using static Amlakbashi.Core.Entities.ReserveSupport;

namespace Amlakbashi.Core.DTOs.ReserveDTOs
{
    public class ReserveAdminItemDTO : ReserveAdminBasicItemDTO
    {
        public string createDateString { get; set; }
        public string createDateStringNoTime { get; set; }
        public string hostResponseDateString { get; set; }
        public string hostResponseTimeString { get; set; }
        public string cancelDateString { get; set; }
        public long totalPrice { get; set; }
        public long depositePrice { get; set; }
        public long couponPrice { get; set; }
        public long prizePrice { get; set; }
        public string supportStateString { get; set; }
        public string supportStateColor { get; set; }
        public List<PaymentHelperDTO> payments { get; set; }
        public bool canDoClearing { get; set; }
        public bool mustDoClearing { get; set; }
        public bool mustRefund { get; set; }
        public bool shouldFollow { get; set; }
        public bool canBePaidByHost { get; set; }
        public bool canGrantSupport { get; set; }
        public long depositePaidPrice { get; set; }
        public long totalPaidPrice { get; set; }
        public float guestScore { get; set; }
        public string guestComment { get; set; }
        public bool systemCalledToHost { get; set; }
        public bool systemCalledToGuest { get; set; }
        public bool disableAutoCancel { get; set; }
        public bool accVisitedByGuest { get; set; }

        public static ReserveAdminItemDTO Generate(Reserve reserve,
            SupporterStatus supportStatus, long guestPaidPrice,
            bool canDoClearing, bool mustRefund, bool refundDone)
        {
            var advertise = reserve.Advertise;
            var linkAdvertise = advertise.ParentOrSelf;
            bool canGrantSupport; 
            switch (supportStatus)
            {
                case SupporterStatus.Free:
                case SupporterStatus.SupportingByOthers:
                case SupporterStatus.Done:
                case SupporterStatus.Expired:
                    canGrantSupport = (int)reserve.Status < 5 || (int)reserve.Status > 8;
                    break;
                default:
                    canGrantSupport = false;
                    break;
            }
            DateTime depositePayDate;
            long depositeTransactionId;
            var depositePaidPrice = reserve.GetReservePaymentPrice(
                ReservePayment.ReservePaymentType.GuestDeposite,
                out depositePayDate, out depositeTransactionId,
                reserve.UserID);
            DateTime hostSitePortionDate;
            long hostSitePortionTransactionId;
            var hostSitePortionPrice = reserve.GetReservePaymentPrice(
                ReservePayment.ReservePaymentType.GuestDeposite,
                out hostSitePortionDate, out hostSitePortionTransactionId, linkAdvertise.UserID);
            DateTime totalPayDate;
            long totalTransactionId;
            var totalPaidPrice = reserve.GetReservePaymentPrice(
                ReservePayment.ReservePaymentType.GuestClearing,
                out totalPayDate, out totalTransactionId, 0);
            DateTime clearingPayDate;
            long clearingTransactionId;
            var clearingPaidPrice = reserve.GetReservePaymentPrice(
                ReservePayment.ReservePaymentType.SiteClearingToHost,
                out clearingPayDate, out clearingTransactionId, 0);
            DateTime refundPayDate;
            long refundTransactionId;
            var refundPaidPrice = reserve.GetReservePaymentPrice(
                ReservePayment.ReservePaymentType.SiteRefundToGuest,
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
                    "_" + depositePayDate.ToString("HH:mm")
                });
            }
            if (totalPaidPrice > 0)
            {
                generatedPayments.Add(new PaymentHelperDTO()
                {
                    title = "کل مبلغ",
                    type = PaymentHelperDTO.PaymentType.Total,
                    transactionId = totalTransactionId,
                    amount = totalPaidPrice,
                    dateString = DateTimeUtility.GregorianToPersianDate(totalPayDate).Remove(0, 2) +
                    "_" + totalPayDate.ToString("HH:mm")
                });
            }
            if (hostSitePortionPrice > 0)
            {
                generatedPayments.Add(new PaymentHelperDTO()
                {
                    title = "درصد سایت",
                    type = PaymentHelperDTO.PaymentType.HostSitePortion,
                    transactionId = hostSitePortionTransactionId,
                    amount = hostSitePortionPrice,
                    dateString = DateTimeUtility.GregorianToPersianDate(hostSitePortionDate).Remove(0, 2) +
                    "_" + hostSitePortionDate.ToString("HH:mm")
                });
            }
            if (clearingPaidPrice > 0)
            {
                generatedPayments.Add(new PaymentHelperDTO()
                {
                    type = PaymentHelperDTO.PaymentType.Clearing,
                    transactionId = clearingTransactionId,
                    title = "سهم میزبان",
                    amount = clearingPaidPrice,
                    dateString = DateTimeUtility.GregorianToPersianDate(clearingPayDate).Remove(0, 2) +
                    "_" + clearingPayDate.ToString("HH:mm")
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
                    "_" + refundPayDate.ToString("HH:mm")
                });
            }
            var hostPayablePrice = PriceUtility.CalculateHostPayablePrice(
                reserve.TotalPrice, guestPaidPrice, reserve.CouponPrice, reserve.PrizePrice);
            var canBePaidByHost = hostPayablePrice < 0 && (
                reserve.Status != ReserveStatus.WaitForResponse &&
                reserve.Status != ReserveStatus.Rejected &&
                reserve.Status != ReserveStatus.CanceledByGuest &&
                reserve.Status != ReserveStatus.CanceledByHost &&
                reserve.Status != ReserveStatus.CanceledBySystem &&
                reserve.Status != ReserveStatus.CancelRequestByGuest &&
                reserve.Status != ReserveStatus.CancelRequestByHost);
            var mustDoClearing = canDoClearing &&
                DateTimeUtility.GetSiteClearingDate(reserve.StartDate, reserve.EndDate) <= DateTime.Now;
            mustRefund = mustRefund && !refundDone;
            var comment = reserve.Advertise.GetCommentBySenderUser(reserve.UserID, Comment.CommentType.advertise, true);
            var infoList = reserve.GetSupportInfoList();
            var createDateString = DateTimeUtility.GregorianToPersianDate(
                    reserve.CreateDate).Remove(0, 2);
            var dto = new ReserveAdminItemDTO()
            {
                totalPrice = reserve.TotalPrice,
                depositePrice = reserve.DepositPrice,
                couponPrice = reserve.CouponPrice,
                prizePrice = reserve.PrizePrice,
                shouldFollow = reserve.shouldFollow,
                createDateString = createDateString + "_" + reserve.CreateDate.ToString("HH:mm"),
                createDateStringNoTime = createDateString,
                cancelDateString = reserve.CancelDate == null ? "-" :
                    DateTimeUtility.GregorianToPersianDate((DateTime)reserve.CancelDate).Remove(0, 2) +
                    "_" + ((DateTime)reserve.CancelDate).ToString("HH:mm"),
                hostResponseDateString = DateTimeUtility.GregorianToPersianDate(
                    reserve.HostResponseDate).Remove(0, 2),
                hostResponseTimeString = reserve.HostResponseDate.ToString("HH:mm"),
                supportStateString = GetSupportStatusString(supportStatus),
                supportStateColor = GetSupportStatusColor(supportStatus),
                canGrantSupport = canGrantSupport,
                canBePaidByHost = canBePaidByHost,
                canDoClearing = canDoClearing,
                mustDoClearing = mustDoClearing,
                mustRefund = mustRefund,
                payments = generatedPayments,
                depositePaidPrice = depositePaidPrice,
                totalPaidPrice = totalPaidPrice,
                guestScore = reserve.Advertise.GetAverageUserRating(reserve.UserID),
                guestComment = comment == null ? "" : comment.Text,
                systemCalledToHost = infoList.Any(x => x.Contains("توسط سیستم با میزبان تماس گرفته شد")),
                systemCalledToGuest = infoList.Any(x => x.Contains("توسط سیستم با مهمان تماس گرفته شد")),
                instantReserve = reserve.InstantReserve,
                disableAutoCancel = reserve.DisableAutoCancel,
                accVisitedByGuest = reserve.AccVisitedByGuest
            };
            var basic = (ReserveAdminBasicItemDTO)reserve;
            PropertyCopier<ReserveAdminBasicItemDTO, ReserveAdminItemDTO>.Copy(basic, dto);
            return dto;
        }
    }
}
