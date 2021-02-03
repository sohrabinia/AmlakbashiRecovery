using MediatR;
using static Amlakbashi.Core.Entities.ActionLog;

namespace Amlakbashi.Mediator.Commands.AdvertiseCommands
{
    public class InsertExtrinsicReserveCommand : IRequest
    {
        public long advertiseId { get; set; }
        public string fromDate { get; set; }
        public string toDate { get; set; }
        public ActionSourceEnum actionSource { get; set; }
        public int doerUserId { get; set; }
        public InsertExtrinsicReserveCommand(long advertiseId, string fromDate,
            string toDate, ActionSourceEnum actionSource, int doerUserId)
        {
            this.advertiseId = advertiseId;
            this.fromDate = fromDate;
            this.toDate = toDate;
            this.actionSource = actionSource;
            this.doerUserId = doerUserId;
        }
    }
}
