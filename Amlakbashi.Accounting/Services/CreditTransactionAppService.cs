using Amlakbashi.Accounting.Services.Interfaces;
using Amlakbashi.Core.Common.AppService;
using Amlakbashi.Core.Common.Repository;
using Amlakbashi.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amlakbashi.Accounting.Services
{
    internal class CreditTransactionAppService : AppServiceBase<CreditTransaction, long>, ICreditTransactionAppService
    {
        public CreditTransactionAppService(IRepository<CreditTransaction, long> repository) : base(repository)
        {

        }

        public IList<CreditTransaction> GetListByUserId(int userId)
        {
            return Repository.Query(q => q.Where(x => x.UserID == userId).OrderByDescending(x => x.Date).ToList());
        }

        public CreditTransaction GetCanselInstantReserve(int userId, int tranCause, long id)
        {
            var data = Repository.Query(q=>q.Where(x =>
                    x.UserID == userId && x.TransactionCause == tranCause));
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
    }
}
