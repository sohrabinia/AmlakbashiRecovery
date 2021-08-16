using Amlakbashi.Accounting.BankingContext.BankingEngines.Interfaces;
using Amlakbashi.Core.Common.BankingEngines.PodiumEngine.Requests;
using Amlakbashi.Core.DTOs.PaymentDTOs.BankingDTOs;
using System.Linq;

namespace Amlakbashi.Accounting.BankingContext.BankingEngines
{
    internal class PodiumBankingEngine : IPodiumBankingEngine
    {
        public ShebaPaymentResultDTO ShebaPayment(ShebaPaymentRequestDTO requestDTO)
        {
            var data = new ShebaPaymentRequest(requestDTO).Send();
            var result = new ShebaPaymentResultDTO();
            if (data.hasError == false && data.BankResult.IsSuccess)
            {
                result.TransactionId = data.BankResult.Data;
                result.Message = data.BankResult.Message;
            }
            else
            {
                result.HasError = true;
                result.ErrorMessage = data.hasError ? data.message : data.BankResult.Message;
            }
            return result;
        }

        public ShebaVerificationResultDTO ShebaVerification(string sheba)
        {
            var data = new ShebaVerificationRequest(sheba).Send();
            var result = new ShebaVerificationResultDTO();
            if (data.hasError == false && data.BankResult.IsSuccess)
            {
                result.Sheba = data.BankResult.Data.Sheba;
                result.AccountStatus = data.BankResult.Data.AccountStatusName;
                result.Message = data.BankResult.Message;
                result.Owners = data.BankResult.Data.AccountOwners.Select(s=>new BankAccountOwnerDTO() {
                    firstName = s.firstName,
                    lastName = s.lastName
                }) .ToList();
            }
            else
            {
                result.HasError = true;
                result.ErrorMessage = data.hasError ? data.message : data.BankResult.Message;
            }
            return result;
        }
    }
}
