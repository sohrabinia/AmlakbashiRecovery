using MediatR;

namespace Amlakbashi.Mediator.Events.AccountingEvents
{
    public class SetReserveCouponEvent : INotification
    {
        public long ReserveId { get; set; }
        public long CouponId { get; set; }
        public long CouponPrice { get; set; }
        public SetReserveCouponEvent(long reserveId, long couponId, long couponPrice)
        {
            ReserveId = reserveId;
            CouponId = couponId;
            CouponPrice = couponPrice;
        }
    }
}
