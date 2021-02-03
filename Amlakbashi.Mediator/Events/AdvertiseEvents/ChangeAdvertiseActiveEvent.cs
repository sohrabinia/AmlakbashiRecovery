using Amlakbashi.Core.Entities;
using MediatR;

namespace Amlakbashi.Mediator.Events.AdvertiseEvents
{
    public class ChangeAdvertiseActiveEvent : INotification
    {
        public long advertiseId { get; set; }
        public ActiveChangeState activeState { get; set; }
        public ChangeAdvertiseActiveEvent(Advertise prevAdvertise, Advertise newAdvertise)
        {
            advertiseId = newAdvertise.Id;
            var prevState = prevAdvertise.IsActive;
            var newState = newAdvertise.IsActive;
            if (prevState == newState)
                activeState = ActiveChangeState.Unchanged;
            else if (newState == true)
                activeState = ActiveChangeState.Activated;
            else
                activeState = ActiveChangeState.Deactivated;
        }
        public enum ActiveChangeState
        {
            Unchanged = 0,
            Activated = 1,
            Deactivated = 2
        }
    }
}
