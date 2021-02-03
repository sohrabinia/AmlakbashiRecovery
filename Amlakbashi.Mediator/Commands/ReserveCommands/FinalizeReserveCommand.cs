using MediatR;
using static Amlakbashi.Core.Entities.ActionLog;
using static Amlakbashi.Core.Entities.Reserve;
using static Amlakbashi.Core.Entities.ReservePayment;

namespace Amlakbashi.Mediator.Commands.ReserveCommands
{
    public class FinalizeReserveCommand : IRequest<ReserveStatus>
    {
        public long reserveId { get; set; }
        public long transactionId { get; set; }
        public long paidAmount { get; set; }
        public ReservePaymentMethod paymentMethod { get; set; }
        public ActionSourceEnum actionSource { get; set; }
        public int doerUserId { get; set; }
        public int payerUserId { get; set; }
        public long couponId { get; set; }
        public long prizePrice { get; set; }
        public bool sendSms { get; set; }
        public FinalizeReserveCommand(long reserveId, long transactionId,
            long paidAmount, ReservePaymentMethod paymentMethod,
            ActionSourceEnum actionSource, int doerUserId,
            int payerUserId = -1, long couponId = 0, long prizePrice = 0,
            bool sendSms = true)
        {
            this.reserveId = reserveId;
            this.transactionId = transactionId;
            this.paidAmount = paidAmount;
            this.paymentMethod = paymentMethod;
            this.actionSource = actionSource;
            this.doerUserId = doerUserId;
            this.payerUserId = payerUserId;
            this.couponId = couponId;
            this.prizePrice = prizePrice;
            this.sendSms = sendSms;
        }
    }
}
