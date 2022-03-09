using Amlakbashi.Core.Common.Enums;
using Amlakbashi.Core.DTOs.PaymentDTOs;
using Amlakbashi.Core.DTOs.PaymentDTOs.BankEPayDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amlakbashi.Accounting.PaymentContext
{
    internal interface IPaymentOperator
    {
        EpayDTO GetPasargadPaymentData(BankEnum bank, int paymentId,
            long paymentTotalAmount, string redirectAddress);
        CheckPaymentDTO GetPasargadPaymentResult(string tref, out string result);
        Task<CheckPaymentDTO> GetPasargadPaymentResult(long paymentId, DateTime paymentDate);
        bool VerifyPasargadPayment(BankEnum bank, string paymentResult, int paymentId, long totalPayingPrice);

        Task<EpayDTO> GetSamanPaymentToken(SamanRequestTokenDTO requestToken);
        Task<string> VerifySamanEpay(string RefNum);
    }
}
