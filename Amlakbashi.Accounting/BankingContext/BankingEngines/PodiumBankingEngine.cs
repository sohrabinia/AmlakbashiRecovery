using Amlakbashi.Accounting.BankingContext.BankingEngines.Interfaces;
using Amlakbashi.Core.Common.BankingEngines.PodiumEngine.GeneralInfos;
using Amlakbashi.Core.Common.BankingEngines.PodiumEngine.Requests;
using Amlakbashi.Core.DTOs.PaymentDTOs.BankingDTOs;
using System.Collections.Generic;
using System.Linq;
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
            dynamic data = await new ShebaPaymentRequest(CentralBankTransferEnum.CCPA, payItems, "ACH1234abcdEFGH").Send();
            var result = new ShebaPaymentResultDTO()
            {
                HasError = data.hasError,
                //errorCode = data.errorCode,
                //Message = data.message,
                //ott = data.ott,
                //referenceNumber = data.referenceNumber,
                //IsSuccess = data.result.result.IsSuccess,
                //Message = data.result.result.Message
            };
            //var dataLength = data.result.result.Data.Count;
            //result.Data = new ShebaPaymentBatchResultDTO[dataLength];
            //for (int i = 0; i < dataLength; i++)
            //{
            //    var podiumData = data.result.result.Data[i];
            //    PropertyCopier<BatchPayaResultItem, ShebaPaymentBatchResultDTO>
            //        .Copy(podiumData, result.Data[i]);
            //}
            return result;
        }

        public async Task<ShebaVerificationResultDTO> VerifySheba(string sheba)
        {
            var data = await new ShebaVerificationRequest(sheba).Send();
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
