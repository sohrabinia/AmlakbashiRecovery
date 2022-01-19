using Amlakbashi.Core.DTOs.PaymentDTOs.BankEPayDTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Amlakbashi.Accounting.PaymentContext.BankEngines.Interfaces
{
    internal interface ISamanEngine
    {
        Task<EpayDTO> GetPaymentToken(SamanRequestTokenDTO requestToken);
        Task<string> VerifyEpay(string RefNum);
    }
}
