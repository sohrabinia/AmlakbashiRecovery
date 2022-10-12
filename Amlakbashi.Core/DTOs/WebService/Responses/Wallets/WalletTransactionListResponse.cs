using System;
using System.Collections.Generic;
using System.Text;

namespace Amlakbashi.Core.DTOs.WebService.Responses.Wallets
{
    public class WalletTransactionListResponse
    {
        public List<WalletTransactionListItemResponse> transactionList { get; set; } = new List<WalletTransactionListItemResponse>();
        public PagingInfo pagingInfo { get; set; }
    }

    public class WalletTransactionListItemResponse
    {
        public long id { get; set; }
        public string date { get; set; }
        public long price { get; set; }
        public string traceNumber { get; set; }
        public string description { get; set; }
    }
}
