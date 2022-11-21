using Amlakbashi.Core.DTOs.WalletDTOs;
using Amlakbashi.Core.Entities;
using System.Collections.Generic;

namespace Amlakbashi.Accounting.Services.Interfaces
{
    internal interface ICreditTransactionAppService
    {
        void Filter(CreditTransactionIndexDTO dto);
        IList<CreditTransaction> GetListByUserId(int userId);
        CreditTransaction GetCanselInstantReserve(int userId, int tranCause, long id);
        CreditTransaction Find(long id);
        CreditTransaction Insert(CreditTransaction newCredit);
        CreditTransaction Update(CreditTransaction editedCreditTransaction);
        void UpdateBySuccessAutoClearingWallet(long id, long amount, long remainedAmount, long bankTransactionId);
        void UpdateByFailedAutoClearingWallet(long id, string transactionCause);
    }
}
