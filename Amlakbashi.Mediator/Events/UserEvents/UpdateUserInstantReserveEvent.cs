using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amlakbashi.Mediator.Events.UserEvents
{
    public class UpdateUserInstantReserveEvent : INotification
    {
        public int UserId { get; set; }
        public bool IsDisabled { get; set; }
        public UpdateUserInstantReserveEvent(int userId, bool isDisabled)
        {
            this.UserId = userId;
            this.IsDisabled = isDisabled;
        }
    }
}
