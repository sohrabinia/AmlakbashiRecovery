using Amlakbashi.Core.Entities;
using static Amlakbashi.Core.Entities.Reserve;

namespace Amlakbashi.Application.Services.ReserveServices.ReserveState.Interfaces
{
    public interface IReserveState : IReserveActions
    {
        void OnTransition(ReserveStatus prevStatus, bool sendSms, ActionLog.ActionSourceEnum actionSource, int doerUserId);
        bool CanTransitTo(ReserveStatus status);
        void Initialize(long ReserveId);
        bool Initialized { get; }
    }
}
