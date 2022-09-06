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
                result.TraceNumber = data.BankResult.EndToEndId;
                result.Message = data.BankResult.Message;
                result.TransactionId = data.BankResult.TransactionId;
                result.RecieverFullName = data.BankResult.RecieverFullNam;
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
                result.Sheba = data.BankResult.iban;
                result.AccountStatus = data.BankResult.accountStatus;
                result.Message = data.BankResult.Message;
                result.Owners = data.BankResult.ibanAccountOwnerList.Select(s => new BankAccountOwnerDTO()
                {
                    firstName = s.firstName,
                    lastName = s.lastName
                }).ToList();
            }
            else
            {
                result.HasError = true;
                result.ErrorMessage = data.hasError ? data.message : data.BankResult.Message;
            }
            return result;
        }

        public CheckShebaPaymentResultDTO CheckShebaPaymentStatus(string transactionId, string traceNumber)
        {
            var data = new CheckShebaPaymentRequest(transactionId, traceNumber).Send();
            var result = new CheckShebaPaymentResultDTO();
            if (data.hasError == false && data.BankResult.IsSuccess)
            {
                result.RefrenceNumber = data.BankResult.ResultData.TransactionNumber;
                result.Status = data.BankResult.ResultData.State;
                result.Amount = data.BankResult.ResultData.Amount;
                result.BankName = data.BankResult.ResultData.BankName;
                result.TransactionDate = data.BankResult.ResultData.TransactionDate;
                result.TransactionTime = data.BankResult.ResultData.TransactionTime;
            }
            else
            {
                result.HasError = true;
                result.ErrorCode = data.errorCode;
                result.ErrorMessage = data.hasError ? data.message : data.BankResult.Message;
            }
            return result;
        }
    }
}
