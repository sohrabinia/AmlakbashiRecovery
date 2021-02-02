using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amlakbashi.Core.DTOs.PaymentDTOs
{
    public class GroupPaymentItemDTO
    {
        public long ReserveId { get; set; }
        public int HostUserId { get; set; }
        public int GuestUserId { get; set; }
        public string HostUserFullName { get; set; }
        public long HostUserCredit { get; set; }
        public string HostBankCardFullName { get; set; }
        public long HostPayablePrice { get; set; }
        public BankResultDTO BankResult { get; set; }
    }
}
