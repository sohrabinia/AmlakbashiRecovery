using Amlakbashi.Core.Common.Repository;
using Amlakbashi.Core.Entities;
using System;
using static Amlakbashi.Core.Entities.Reserve;

namespace Amlakbashi.Application.Services.ReserveServices.ReserveState.ReserveStates
{
    public class WaitResponseState : ReserveState
    {
        public WaitResponseState(IRepository<Reserve, long> Repository) : base(Repository)
        {
        }

        public override bool CanTransitTo(Reserve.ReserveStatus status)
        {
            switch (status)
            {
                case Reserve.ReserveStatus.WaitForReserve:
                case Reserve.ReserveStatus.Rejected:
                case Reserve.ReserveStatus.CanceledByGuest:
                case Reserve.ReserveStatus.CanceledByHost:
                case Reserve.ReserveStatus.CanceledBySystem:
                    return true;
                default:
                    return false;
            }
        }

        public override void OnTransition(Reserve.ReserveStatus prevStatus, bool sendSms, ActionLog.ActionSourceEnum actionSource, int doerUserId)
        {
            var reserve = Repository.Find(ReserveId);
            reserve.Status = ReserveStatus.WaitForResponse;
            Repository.Update(reserve);
            Repository.Save();
        }
    }
}
