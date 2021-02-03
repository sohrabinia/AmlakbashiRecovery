using Amlakbashi.Application.Services.ReserveServices.ReserveState.Interfaces;
using Amlakbashi.Core.Common.Repository;
using Amlakbashi.Core.Common.State;
using Amlakbashi.Core.Entities;
using static Amlakbashi.Core.Entities.Reserve;

namespace Amlakbashi.Application.Services.ReserveServices.ReserveState
{
    public abstract class ReserveState : StateBase<ReserveStatus>, IReserveState
    {
        protected long ReserveId;
        protected readonly IRepository<Reserve, long> Repository;
        public ReserveState(IRepository<Reserve, long> Repository)
        {
            this.Repository = Repository;
        }
        public void Initialize(long ReserveId)
        {
            this.ReserveId = ReserveId;
        }
        public bool Initialized
        {
            get { return ReserveId > 0; }
        }

        public abstract void OnTransition(ReserveStatus prevStatus, bool sendSms,
            ActionLog.ActionSourceEnum actionSource, int doerUserId);

        public abstract bool CanTransitTo(ReserveStatus status);
    }
}
