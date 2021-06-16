using Amlakbashi.Core.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amlakbashi.Mediator.Commands.AdvertiseCommands
{
    public class InsertExtrinsicReserveByDateListCommand : IRequest
    {
        public long AdvertiseId { get; set; }
        public long SystemCanseledReserveId { get; set; }
        public List<string> Dates { get; set; }
        public int DoerUserId { get; set; }
        public ActionLog.ActionSourceEnum ActionSource { get; set; }

        public InsertExtrinsicReserveByDateListCommand(long advertiseId, long systemCanseledReserveId, List<string> dates,
            int doerUserid, ActionLog.ActionSourceEnum actionSource)
        {
            AdvertiseId = advertiseId;
            SystemCanseledReserveId = systemCanseledReserveId;
            Dates = dates;
            DoerUserId = doerUserid;
            ActionSource = actionSource;
        }
    }
}
