using Amlakbashi.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amlakbashi.Core.DTOs.ReserveDTOs
{
    public class SitePaymentDTO
    {
        public long ReserveId { get; set; }
        public Reserve.ReserveStatus ReserveStatus { get; set; }
        public int Days { get; set; }
        public long TotalPrice { get; set; }
        public long GuestPayedPrice { get; set; }
        public long SitePortion { get; set; }
        public long PayablePrice { get; set; }
        public long PayablePriceRaw { get; set; }
        public string BankCardNumber { get; set; }
        public string BankCardName { get; set; }
        public int BankCardId { get; set; }
        public bool BankCardVerified { get; set; }
        public string ShebaNumber { get; set; }
        public bool ShebaVerified { get; set; }
        public int UserId { get; set; }
        public long UserCredit { get; set; }
        public string UserName { get; set; }
        public UserType UserType { get; set; }
    }

    public enum UserType
    {
        Host = 0,
        Guest = 1
    }
}
