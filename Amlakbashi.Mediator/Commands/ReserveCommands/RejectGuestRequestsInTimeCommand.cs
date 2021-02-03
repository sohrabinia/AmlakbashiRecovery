using MediatR;
using System;
using static Amlakbashi.Core.Entities.ActionLog;

namespace Amlakbashi.Mediator.Commands.AdvertiseCommands
{
    public class RejectGuestRequestsInTimeCommand : IRequest
    {
        public int guestUserId { get; set; }
        public DateTime startDate { get; set; }
        public bool exceptWaitForReserve { get; set; }
        public long exceptReserveId { get; set; }
        public ActionSourceEnum actionSource { get; set; }
        public int doerUserId { get; set; }
        public RejectGuestRequestsInTimeCommand(int guestUserId, DateTime startDate,
            ActionSourceEnum actionSource, int doerUserId, bool exceptWaitForReserve = false,
            long exceptReserveId = -1)
        {
            this.guestUserId = guestUserId;
            this.startDate = startDate;
            this.exceptWaitForReserve = exceptWaitForReserve;
            this.exceptReserveId = exceptReserveId;
            this.actionSource = actionSource;
            this.doerUserId = doerUserId;
        }
    }
}
