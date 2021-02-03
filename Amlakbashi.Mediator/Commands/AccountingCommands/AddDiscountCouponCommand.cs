using Amlakbashi.Core.Entities;
using MediatR;

namespace Amlakbashi.Mediator.Commands.AccountingCommands
{
    public class AddDiscountCouponCommand : IRequest
    {
        public int UserId { get; set; }
        public int PresentorUserId { get; set; }
        public int Percent { get; set; }
        public DiscountCoupon.DiscountCouponType Type { get; set; }
        public AddDiscountCouponCommand(int userId, int presentorId, int percent, DiscountCoupon.DiscountCouponType type)
        {
            UserId = userId;
            PresentorUserId = presentorId;
            Percent = percent;
            Type = type;
        }
    }
}
