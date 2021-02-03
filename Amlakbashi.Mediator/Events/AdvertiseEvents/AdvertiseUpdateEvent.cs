using Amlakbashi.Core.Entities;
using MediatR;

namespace Amlakbashi.Mediator.Events.AdvertiseEvents
{
    public class AdvertiseUpdateEvent : INotification
    {
        public Advertise PreviusData { get; set; }
        public Advertise CurrentData { get; set; }
        public ActionLog.ActionSourceEnum ActionSource { get; set; }
        public int CurrentUserId { get; set; }

        public AdvertiseUpdateEvent(Advertise previeusData, Advertise currentData, ActionLog.ActionSourceEnum actionSource
            , int currentUserId)
        {
            this.PreviusData = previeusData;
            this.CurrentData = currentData;
            this.ActionSource = actionSource;
            this.CurrentUserId = currentUserId;
        }
    }
}
