using Amlakbashi.Core.Common.AppService;
using Amlakbashi.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amlakbashi.Accounting.Services.Interfaces
{
    internal interface IDiscountCouponAppService : IAppService<ReservePayment, long>
    {
        DiscountCoupon Find(long id);
        DiscountCoupon Find(int userId, DiscountCoupon.DiscountCouponType type);
        DiscountCoupon GetMostValuableCouponIfAny(int userId);
        DiscountCoupon Insert(int userId, DiscountCoupon.DiscountCouponType type, int percent,
            int presentorUserID = 0);
        void UpdateCouponUsing(long couponId, long reserveId, DiscountCoupon.StatusEnum status);
        long CalculateCouponPrice(int couponPercent, long couponCalculationPrice);
    }
}
