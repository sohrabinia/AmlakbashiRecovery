using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amlakbashi.Mediator.Commands.AdvertiseCommands
{
    public class SetExtrinsicReserveForWaitForResponseCommand : IRequest
    {
        public long AdvertiseId { get; set; }
        public long SystemCanseledReserveId { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public SetExtrinsicReserveForWaitForResponseCommand(long advertiseId, long systemCanseledReserveId,
            DateTime fromDate, DateTime toDate)
        {
            AdvertiseId = advertiseId;
            SystemCanseledReserveId = systemCanseledReserveId;
            FromDate = fromDate;
            ToDate = toDate;
        }
    }
}
