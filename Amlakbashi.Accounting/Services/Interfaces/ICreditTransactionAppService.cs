using Amlakbashi.Core.Common.AppService;
using Amlakbashi.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amlakbashi.Accounting.Services.Interfaces
{
    internal interface ICreditTransactionAppService : IAppService<CreditTransaction, long>
    {
        IList<CreditTransaction> GetListByUserId(int userId);
        CreditTransaction GetCanselInstantReserve(int userId, int tranCause, long id);
        CreditTransaction Find(long id);
        CreditTransaction Insert(CreditTransaction newCredit);
    }
}
