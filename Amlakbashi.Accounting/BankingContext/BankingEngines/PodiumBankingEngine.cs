using Amlakbashi.Accounting.BankingContext.BankingEngines.Interfaces;
using Amlakbashi.Core.Common.BankingEngines.PodiumEngine.Requests;
using Amlakbashi.Core.DTOs.PaymentDTOs.PodiumDTOs;
using System;
using System.Linq;

namespace Amlakbashi.Accounting.BankingContext.BankingEngines
{
    internal class PodiumBankingEngine : IPodiumBankingEngine
    {
        public PayaPaymentResponseDTO PayaPayment(PayaPaymentRequestDTO requestDTO)
        {
            var data = new ShebaPaymentRequest(requestDTO).Send();
            var result = new PayaPaymentResponseDTO();
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

        public ShebaVerificationResponseDTO ShebaVerification(string sheba)
        {
            var data = new ShebaVerificationRequest(sheba).Send();
            var result = new ShebaVerificationResponseDTO();
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

        public CheckPayaPaymentResponseDTO CheckPayaPayment(string transactionId, string traceNumber)
        {
            var data = new CheckShebaPaymentRequest(transactionId, traceNumber).Send();
            var result = new CheckPayaPaymentResponseDTO();
            if (data.hasError == false && data.BankResult.IsSuccess)
            {
                result.RefrenceNumber = data.BankResult.ResultData.TransactionNumber;
                result.Status = Enum.Parse<PayaPaymentStatusEnum>(data.BankResult.ResultData.State);
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

        public CheckTransactionStatusResponseDTO CheckTransactionStatus(string transactionId)
        {
            var data = new CheckTransactionStatusRequest(transactionId).Send();
            var result = new CheckTransactionStatusResponseDTO();
            if (data.hasError == false && data.BankResult.IsSuccess)
            {
                result.TransactionDate = data.BankResult.ResultData.TransactionDate;
                result.TransactionCode = data.BankResult.ResultData.TransactionCode;
                result.TransactionStatus = Enum.Parse<TransactionStatusEnum>(data.BankResult.ResultData.TransactionStatus);
            }
            else
            {
                result.HasError = true;
                result.ErrorAgent = data.hasError ? ExpenditurePaymentErrorAgent.Podium : ExpenditurePaymentErrorAgent.PasargadBank;
                result.ErrorCode = data.hasError ? data.errorCode : data.BankResult.RsCode;
                result.ErrorMessage = data.hasError ? data.message : data.BankResult.Message;
            }
            return result;
        }
    }
}
