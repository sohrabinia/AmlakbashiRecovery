using System;
using System.Collections.Generic;

namespace Amlakbashi.Core.DTOs.ReserveDTOs
{
    public class InvoiceItemDTO
    {
        public long id { get; set; }
        public long accommodationId { get; set; }
        public DateTime clearingDate { get; set; }
        public DateTime startDate { get; set; }
        public DateTime endDate { get; set; }
        public string startDateString { get; set; }
        public string endDateString { get; set; }
        public string guestName { get; set; }
        public string createDateString { get; set; }
        public long totalPrice { get; set; }
        public long sitePortion { get; set; }
        public long couponPrice { get; set; }
        public long prizePrice { get; set; }
        public int stayDays { get; set; }
        public long depositePaidPrice { get; set; }
        public long totalPaidPrice { get; set; }
        public long netPaidPrice { get; set; }
        public long hostPaidPrice { get; set; }
        public long guestPaidPrice { get; set; }
        public long hostPayablePrice { get; set; }
        public bool isCleared { get; set; }
        public string clearingDateString { get; set; }
        public long clearingTransactionId { get; set; }
        public bool hasInstantReservePenalty { get; set; }
        public long instantReservePenaltyPrice { get; set; }
    }
}
