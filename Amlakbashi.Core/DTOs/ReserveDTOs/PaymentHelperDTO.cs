using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amlakbashi.Core.DTOs.ReserveDTOs
{
    public class PaymentHelperDTO
    {
        public string title { get; set; }
        public long amount { get; set; }
        public string dateString { get; set; }
        public PaymentType type { get; set; }
        public long transactionId { get; set; }
        public enum PaymentType
        {
            Deposite = 0,
            Total = 1,
            HostSitePortion = 2,
            Clearing = 3,
            Refund = 4,
        }
    }
}
