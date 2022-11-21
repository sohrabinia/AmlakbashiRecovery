using Amlakbashi.Core.Entities;

namespace Amlakbashi.Data.Repositories
{
    public class DiscountCouponRepository : GenericRepository<DiscountCoupon, long>
    {
        public DiscountCouponRepository(AmlakbashiDB context) : base(context)
        {
        }
    }
}
