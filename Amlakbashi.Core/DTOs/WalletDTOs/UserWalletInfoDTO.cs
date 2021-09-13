using System;
using System.Collections.Generic;
using System.Text;

namespace Amlakbashi.Core.DTOs.WalletDTOs
{
    public class UserWalletInfoDTO
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public long WalletCredit { get; set; }
        public string ShebaNumber { get; set; }
        public bool ShebaVerified { get; set; }
        public long BankCardId { get; set; }
        public string BankCardNumber { get; set; }
        public string BankCardName { get; set; }
        public bool BankCardVerified { get; set; }
    }
}