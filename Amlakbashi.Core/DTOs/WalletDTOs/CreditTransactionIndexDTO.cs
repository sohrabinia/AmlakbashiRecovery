using Amlakbashi.Core.Entities;
using System.Collections.Generic;

namespace Amlakbashi.Core.DTOs.WalletDTOs
{
    public class CreditTransactionIndexDTO
    {
        public IEnumerable<CreditTransaction> CreditTransactionList { get; set; }
        public int page { get; set; } = 1;
        public int pageModelCount { get; set; } = 20;
        public long creditTransactionId { get; set; }
        public int userId { get; set; }
        public long reserveId { get; set; }
        public long transactionId { get; set; }
    }
}
