using Amlakbashi.Accounting.BankingContext.BankingEngines.Interfaces;
using Amlakbashi.Core.Common.BankingEngines.PodiumEngine.GeneralInfos;
using Amlakbashi.Core.Common.BankingEngines.PodiumEngine.Requests;
using Amlakbashi.Core.Common.Utilities;
using Amlakbashi.Core.DTOs.PaymentDTOs.BankingDTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Amlakbashi.Accounting.BankingContext.BankingEngines
{
    internal class PodiumBankingEngine : IPodiumBankingEngine
    {
        public async Task<ShebaPaymentResultDTO> PaySheba(string sheba, long amount, string fullName)
        {
            var payItems = new List<BatchPayaRequestItem>
                {
                    new BatchPayaRequestItem()
                    {
                        Amount = amount.ToString(),
                        BeneficiaryFullName = fullName,
                        DestShebaNumber = "IR" + sheba,
                        BillNumber = "",
                        Description = "تست انتقال وجه توسط پایا"
                    }
                };
            var data = await new ShebaPaymentRequest(CentralBankTransferEnum.CCPA, payItems, "ACH1234abcdEFGH").Send();
            var result = new ShebaPaymentResultDTO()
            {
                hasError = data.hasError,
                errorCode = data.errorCode,
                message = data.message,
                ott = data.ott,
                referenceNumber = data.referenceNumber,
                IsSuccess = data.result.result.IsSuccess,
                Message = data.result.result.Message
            };
            var dataLength = data.result.result.Data.Count;
            result.Data = new ShebaPaymentBatchResultDTO[dataLength];
            for (int i = 0; i < dataLength; i++)
            {
                var podiumData = data.result.result.Data[i];
                PropertyCopier<BatchPayaResultItem, ShebaPaymentBatchResultDTO>
                    .Copy(podiumData, result.Data[i]);
            }
            return result;
        }

        public async Task<ShebaVerificationResultDTO> VerifySheba(string sheba)
        {
            var data = await new ShebaVerificationRequest(sheba).Send();
            var result = new ShebaVerificationResultDTO()
            {
                 hasError = data.hasError,
                 errorCode = data.errorCode,
                 message = data.message,
                referenceNumber = data.referenceNumber,
                ott = data.ott,
                sheba = data.hasError ? null : data.result.sheba
            };
            if (data.hasError == false)
            {
                var ownerLength = data.result.owners.Length;
                result.owners = new BankAccountOwnerDTO[ownerLength];
                for (int i = 0; i < ownerLength; i++)
                {
                    var owner = data.result.owners[i];
                    result.owners[i] = new BankAccountOwnerDTO()
                    {
                        firstName = owner.firstName,
                        lastName = owner.lastName
                    };
                }
            }
            return result;
        }
    }
}
