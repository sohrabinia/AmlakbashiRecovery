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
        ShebaVerificationResultDTO ShebaVerification(string sheba);
        ShebaPaymentResultDTO ShebaPayment(ShebaPaymentRequestDTO reqeustDTO);
        CheckShebaPaymentResultDTO CheckShebaPaymentStatus(string date, string paymentId);
    }
}
