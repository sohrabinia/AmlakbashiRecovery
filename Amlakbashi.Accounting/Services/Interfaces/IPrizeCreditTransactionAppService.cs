using Amlakbashi.Core.Common.AppService;
using Amlakbashi.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Amlakbashi.Core.Entities.PrizeCreditTransaction;

namespace Amlakbashi.Accounting.Services.Interfaces
{
    internal interface IPrizeCreditTransactionAppService : IAppService<PrizeCreditTransaction, long>
    {
        long Insert(int userId, long amount, long newPrizeCredit, PrizeCreditTransaction.PrizeTransactionType type,
            long reserve_id = 0, string customTitle = null);
        long Increase(int userId, long amount, PrizeTransactionType type, long reserveId,
            string customTitle, int doerUserId, ActionLog.ActionSourceEnum actionSource);
    }
}
