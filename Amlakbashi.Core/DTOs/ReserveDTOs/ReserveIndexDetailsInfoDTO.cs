using System;
using System.Collections.Generic;
using System.Text;

namespace Amlakbashi.Core.DTOs.ReserveDTOs
{
    public class ReserveIndexDetailsInfoDTO
    {
        public long Id { get; set; }
        public long TotalPrice { get; set; }
        public long DepositePrice { get; set; }
        public long TotalPaidPrice { get; set; }
        public long DepositePaidPrice { get; set; }
        public string StartDateString { get; set; }
        public string EndDateString { get; set; }
        public int GuestCount { get; set; }
        public int StayDays { get; set; }
        public string CreateDateString { get; set; }
        public int GuestUserId { get; set; }
        public int Status { get; set; }
        public int GuestCallState { get; set; }
        public string GuestCallStateColor { get; set; }
        public int HostCallState { get; set; }
        public string HostCallStateColor { get; set; }
        public int SupportInfoCount { get; set; }
        public List<string> SupportInfoList { get; set; }
        public List<PaymentHelperDTO> PaymentList { get; set; }
        public int PaymentTryCount { get; set; }
        public string LastPaymentTryDate { get; set; }
        public bool InstantReserve { get; set; }
        public int HostResponse { get; set; }
        public string HostResponseString { get; set; }
        public string HostResponseColor { get; set; }
        public string HostResponseDateString { get; set; }
        public string HostResponseTimeString { get; set; }
        public string SupportStateString { get; set; }
        public string SupportStateColor { get; set; }
        public bool CanGrantSupport { get; set; }
        public List<SupporterHelperDTO> Supporters { get; set; }
        public bool DisableAutoCancel { get; set; }
        public bool AccVisitedByGuest { get; set; }
        public bool ShouldFollow { get; set; }
        public bool canBeDoneCheckout { get; set; }
        public bool canBeDoneEarlyCheckout { get; set; }
        public bool mustBeDoneCheckout { get; set; }
        public bool MustRefund { get; set; }
        public bool CanBePaidByHost { get; set; }
        public string CancelDate { get; set; }
    }
}
