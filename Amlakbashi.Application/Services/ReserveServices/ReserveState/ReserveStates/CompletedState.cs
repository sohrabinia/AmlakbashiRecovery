using Amlakbashi.Accounting;
using Amlakbashi.Core.Common.Repository;
using Amlakbashi.Core.Entities;
using System;
using static Amlakbashi.Core.Entities.Reserve;

namespace Amlakbashi.Application.Services.ReserveServices.ReserveState.ReserveStates
{
    public class CompletedState : ReserveState
    {
        private readonly IAccountingFacade accounting;
        public CompletedState(
            IAccountingFacade accounting,
            IRepository<Reserve, long> Repository) : base(Repository)
        {
            this.accounting = accounting;
        }

        public override bool CanTransitTo(ReserveStatus status)
        {
            return false;
        }

        public override void OnTransition(ReserveStatus prevStatus, bool sendSms,
            ActionLog.ActionSourceEnum actionSource, int doerUserId)
        {
            var reserve = Repository.Find(ReserveId);
            if (reserve.EndDate.Date == DateTime.Now.Date)
            {
                reserve.Status = ReserveStatus.Completed;
                Repository.Update(reserve);
                Repository.Save();
                accounting.GivePresentorPrizeIfAny(reserve.Id, actionSource, doerUserId);
                accounting.GiveAppreciateDiscountIfDeserve(reserve.Id, actionSource, doerUserId);
            }
        }
    }
}
