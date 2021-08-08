using Amlakbashi.Core.DTOs.PaymentDTOs.BankingDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amlakbashi.Accounting.BankingContext
{
    internal interface IBankingOperator
    {
        Task<ShebaVerificationResultDTO> VerifySheba(string sheba);
        Task<ShebaPaymentResultDTO> PaySheba(string sheba, long amount, string fullName);
    }
}
