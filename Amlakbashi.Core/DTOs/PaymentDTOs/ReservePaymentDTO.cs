using Amlakbashi.Core.Common.Utilities;
using Amlakbashi.Core.Entities;
using System;
using System.Collections.Generic;
using static Amlakbashi.Core.Entities.Reserve;

namespace Amlakbashi.Core.DTOs.PaymentDTOs
{
    public class ReservePaymentDTO
    {
        public long ReserveId { get; set; }
        public int UserId { get; set; }
        public long CurrentCredit { get; set; }
        public long TotalPrice { get; set; }
        public long DepositePrice { get; set; }
        public long CouponPrice { get; set; }
        public long CouponId { get; set; }
        public long PrizePrice { get; set; }
        public ReserveStatus ReserveStatus { get; set; }
        public long AvailablePrizeCredit { get; set; }
        public long AvailableCouponPrice { get; set; }
        public bool AlreadyUsedDiscount { get; set; }
        public long PaidAmount { get; set; }
        public bool hasDateInNorouzRange { get; set; } = false;

        public static ReservePaymentDTO Generate(Reserve reserve,
            long availablePrizeCredit, long availableCouponPrice,
            long couponId, long paidAmount)
        {
            var dto = new ReservePaymentDTO();
            var guestUser = reserve.GuestUser;
            dto.ReserveId = reserve.Id;
            dto.UserId = guestUser.Id;
            dto.CurrentCredit = guestUser.WalletAmount;
            dto.TotalPrice = reserve.TotalPrice;
            dto.DepositePrice = reserve.DepositPrice;
            dto.CouponId = couponId;
            dto.CouponPrice = reserve.CouponPrice;
            dto.PrizePrice = reserve.PrizePrice;
            dto.ReserveStatus = reserve.Status;
            dto.AvailablePrizeCredit = availablePrizeCredit;
            dto.AvailableCouponPrice = availableCouponPrice;
            dto.AlreadyUsedDiscount = reserve.CouponID > 0 || reserve.PrizeTransactionID > 0;
            dto.PaidAmount = paidAmount;
            dto.hasDateInNorouzRange = DateTimeUtility.IsNorouz(reserve.StartDate, reserve.EndDate);
            return dto;
        }
    }
}
