using Amlakbashi.Accounting.Services.Interfaces;
using Amlakbashi.Core.Common.AppService;
using Amlakbashi.Core.Common.Repository;
using Amlakbashi.Core.Entities;
using Amlakbashi.Mediator.Events.AccountingEvents;
using MediatR;
using System;
using static Amlakbashi.Core.Entities.PrizeCreditTransaction;

namespace Amlakbashi.Accounting.Services
{
    internal class PrizeCreditTransactionAppService : BaseAppService<PrizeCreditTransaction, long>, IPrizeCreditTransactionAppService
    {
        private readonly IMediator mediator;
        public PrizeCreditTransactionAppService(IRepository<PrizeCreditTransaction, long> repository,
            IMediator mediator) : base(repository)
        {
            this.mediator = mediator;
        }

        public long Insert(int userId, long amount, long newPrizeCredit, PrizeCreditTransaction.PrizeTransactionType type,
            long reserve_id = 0, string customTitle = null)
        {
            var prizeCreditTransaction = new PrizeCreditTransaction()
            {
                UserID = userId,
                ReserveID = reserve_id,
                Date = DateTime.Now,
                Price = amount,
                RemainedPrice = newPrizeCredit,
                CustomTitle = customTitle,
                Type = type
            };
            Repository.Insert(prizeCreditTransaction);
            Repository.Save();
            return prizeCreditTransaction.Id;
        }

        public long Increase(int userId, long amount, PrizeTransactionType type, long reserveId,
            string customTitle, int doerUserId, ActionLog.ActionSourceEnum actionSource)
        {
            var user = Repository.Find<User,int>(userId);
            var newPrizeCredit = user.GiftWalletAmount + amount;
            var prizeCreditTran = new PrizeCreditTransaction()
            {
                UserID = userId,
                Price = amount,
                RemainedPrice = newPrizeCredit,
                Type = type,
                ReserveID = reserveId,
                Date = DateTime.Now,
                CustomTitle = customTitle
            };
            Repository.Insert(prizeCreditTran);
            Repository.Save();
            mediator.Publish(new PrizeCreditUpdateEvent(userId,actionSource, doerUserId));
            return prizeCreditTran.Id;
        }
    }
}
