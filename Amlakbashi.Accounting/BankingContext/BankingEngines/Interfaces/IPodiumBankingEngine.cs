using Amlakbashi.Core.Common.BankingEngines.PodiumEngine.GeneralInfos;
using Amlakbashi.Core.DTOs.PaymentDTOs.BankingDTOs;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Amlakbashi.Accounting.BankingContext.BankingEngines.Interfaces
{
    internal interface IPodiumBankingEngine
    {
        ShebaVerificationResultDTO ShebaVerification(string sheba);
        ShebaPaymentResultDTO ShebaPayment(ShebaPaymentRequestDTO requestDTO);
        CheckShebaPaymentResultDTO CheckShebaPaymentStatus(string date, string paymentId);
    }
}
