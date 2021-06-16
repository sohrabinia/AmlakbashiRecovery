using MediatR;
using System;
using static Amlakbashi.Core.Entities.ActionLog;

namespace Amlakbashi.Mediator.Commands.AdvertiseCommands
{
    public class RejectRequestsInTimeCommand : IRequest
    {
        public long advertiseId { get; set; }
        public DateTime startDate { get; set; }
        public DateTime endDate { get; set; }
        public bool exceptWaitForReserve { get; set; }
        public long exceptReserveId { get; set; }
        public bool sendSms { get; set; }
        public ActionSourceEnum actionSource { get; set; }
        public int doerUserId { get; set; }
        public bool doSystemCancel { get; set; }
        public RejectRequestsInTimeCommand(long advertiseId, DateTime startDate,
            DateTime endDate, ActionSourceEnum actionSource, int doerUserId, bool exceptWaitForReserve = false,
            long exceptReserveId = -1, bool sendSms = true, bool doSystemCancel = false)
        {
            this.advertiseId = advertiseId;
            this.startDate = startDate;
            this.endDate = endDate;
            this.exceptWaitForReserve = exceptWaitForReserve;
            this.exceptReserveId = exceptReserveId;
            this.sendSms = sendSms;
            this.actionSource = actionSource;
            this.doerUserId = doerUserId;
            this.doSystemCancel = doSystemCancel;
        }
    }
}
