using Amlakbashi.Accounting.Services.Interfaces;
using Amlakbashi.Core.Common.AppService;
using Amlakbashi.Core.Common.Caching;
using Amlakbashi.Core.Common.Repository;
using Amlakbashi.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amlakbashi.Accounting.Services
{
    internal class DiscountCouponAppService : AppServiceBase<DiscountCoupon, long>, IDiscountCouponAppService
    {
        public DiscountCouponAppService(IRepository<DiscountCoupon, long> repository, ICacheManager<DiscountCoupon> cache) : base(repository, cache)
        {

        }

        public DiscountCoupon Find(long id)
        {
            return Repository.Query(q => q.FirstOrDefault(f => f.Id == id));
        }

        public DiscountCoupon GetMostValuableCouponIfAny(int userId)
        {
            var coupons = Repository.Query(q => q.Where(x => x.UserID == userId &&
                  x.Status == DiscountCoupon.StatusEnum.NotUsed));
            if (!coupons.Any())
            {
                return null;
            }
            return coupons.OrderByDescending(x => x.Percent).First();
        }

        public DiscountCoupon Insert(int userId, DiscountCoupon.DiscountCouponType type,
            int percent, int presentorUserID = 0)
        {
            var newDiscountCoupon = new DiscountCoupon()
            {
                UserID = userId,
                CreateTime = DateTime.Now,
                Type = type,
                Status = DiscountCoupon.StatusEnum.NotUsed,
                Percent = percent,
                PresentorUserID = presentorUserID
            };
            Repository.Insert(newDiscountCoupon);
            Repository.Save();
            return newDiscountCoupon;
        }

        public void UpdateCouponUsing(long couponId, long reserveId, DiscountCoupon.StatusEnum status)
        {
            var coupon = Repository.Query(q => q.FirstOrDefault(f => f.Id == couponId));
            coupon.Status = status;
            if (status == DiscountCoupon.StatusEnum.Used)
            {
                coupon.UsingReserveID = reserveId;
            }
            else
            {
                coupon.UsingReserveID = 0;
            }
            Repository.Update(coupon);
            Repository.Save();
        }

        public long CalculateCouponPrice(int couponPercent, long couponCalculationPrice)
        {
            var discountMultiplier = couponPercent / 100f;
            var result = (long)(couponCalculationPrice * discountMultiplier);
            return result;
        }
    }
}
