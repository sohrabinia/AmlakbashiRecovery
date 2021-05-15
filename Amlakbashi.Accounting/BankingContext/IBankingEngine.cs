using Amlakbashi.Core.DTOs.PaymentDTOs.BankingDTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Amlakbashi.Accounting.BankingContext
{
    internal interface IBankingEngine
    {
        Task<ShebaVerificationResultDTO> VerifySheba(string sheba);
        Task<ShebaPaymentResultDTO> PaySheba(string sheba, long amount, string fullName);
    }
}
