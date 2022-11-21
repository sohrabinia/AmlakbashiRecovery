using Amlakbashi.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amlakbashi.Accounting.Services.Interfaces
{
    internal interface IPrizeCreditTransactionAppService
    {
        long Insert(int userId, long amount, long newPrizeCredit, PrizeCreditTransaction.PrizeTransactionType type,
            long reserve_id = 0, string customTitle = null);
        long Increase(int userId, long amount, PrizeCreditTransaction.PrizeTransactionType type, long reserveId,
            string customTitle, int doerUserId, ActionLog.ActionSourceEnum actionSource);
    }
}
