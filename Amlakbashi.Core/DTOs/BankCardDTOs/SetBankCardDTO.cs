using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Amlakbashi.Core.DTOs.BankCardDTOs
{
    public class SetBankCardDTO
    {
        public int UserId { get; set; }
        public string FName { get; set; }
        public string LName { get; set; }
        public string BankCardNumber { get; set; }
        public bool VerifyBankCardNumber { get; set; }
        public string ShebaNumber { get; set; }
        public bool VerifyShebaNumber { get; set; }
    }
}
