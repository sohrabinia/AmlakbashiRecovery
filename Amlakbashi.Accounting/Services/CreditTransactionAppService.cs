using Amlakbashi.Accounting.Services.Interfaces;
using Amlakbashi.Core.Common.AppService;
using Amlakbashi.Core.Common.Repository;
using Amlakbashi.Core.DTOs.WalletDTOs;
using Amlakbashi.Core.Entities;
using System.Collections.Generic;
using System.Linq;

namespace Amlakbashi.Accounting.Services
{
    internal class CreditTransactionAppService : AppServiceBase<CreditTransaction, long>, ICreditTransactionAppService
    {
        public CreditTransactionAppService(IRepository<CreditTransaction, long> repository) : base(repository)
        {
        }

        public void Filter(CreditTransactionIndexDTO dto)
        {
            var data = Repository.Query(q => q);
            if (dto.creditTransactionId > 0)
            {
                data = data.Where(w => w.Id == dto.creditTransactionId);
            }
            if (dto.userId > 0)
            {
                data = data.Where(w => w.UserID == dto.userId);
            }
            if (dto.reserveId > 0)
            {
                data = data.Where(w => w.ReserveID == dto.reserveId);
            }
            if (dto.transactionId > 0)
            {
                data = data.Where(w => w.BankTransactionID == dto.transactionId);
            }
            //dto.Model = data.OrderByDescending(o => o.Date).Skip((dto.page - 1) * dto.pageModelCount).Take(dto.pageModelCount).ToList();
            dto.CreditTransactionList = data.OrderByDescending(o => o.Date).ToList();
        }

        public IList<CreditTransaction> GetListByUserId(int userId)
        {
            return Repository.Query(q => q.Where(x => x.UserID == userId).OrderByDescending(x => x.Date).ToList());
        }

        public CreditTransaction GetCanselInstantReserve(int userId, int tranCause, long id)
        {
            var data = Repository.Query(q => q.Where(x =>
                      x.UserID == userId && x.TransactionCause == (CreditTransaction.WalletTransactionReason)tranCause));
            return data.FirstOrDefault(
                x => x.TransactionCauseString.Contains(id.ToString()) &&
                x.TransactionCauseString.Contains("جریمه لغو رزرو آنی"));
        }

        public CreditTransaction Find(long id)
        {
            return Repository.Find(id);
        }

        public CreditTransaction Insert(CreditTransaction newCredit)
        {
            Repository.Insert(newCredit);
            Repository.Save();
            return newCredit;
        }

        public CreditTransaction Update(CreditTransaction editedCreditTransaction)
        {
            var creditTransaction = Repository.Find(editedCreditTransaction.Id);
            creditTransaction.BankTransactionID = editedCreditTransaction.BankTransactionID;
            creditTransaction.TransactionCause = editedCreditTransaction.TransactionCause;
            creditTransaction.TransactionCauseString = editedCreditTransaction.TransactionCauseString;
            Repository.Update(creditTransaction);
            Repository.Save();
            return creditTransaction;
        }

        public void UpdateBySuccessAutoClearingWallet(long id, long amount, long remainedAmount, long bankTransactionId)
        {
            var creditTransaction = Repository.Find(id);
            creditTransaction.Price = amount;
            creditTransaction.RemainedPrice = remainedAmount;
            creditTransaction.BankTransactionID = bankTransactionId;
            Repository.Update(creditTransaction);
            Repository.Save();
        }

        public void UpdateByFailedAutoClearingWallet(long id, string transactionCause)
        {
            var creditTransaction = Repository.Find(id);
            creditTransaction.TransactionCauseString = transactionCause;
            Repository.Update(creditTransaction);
            Repository.Save();
        }
    }
}
