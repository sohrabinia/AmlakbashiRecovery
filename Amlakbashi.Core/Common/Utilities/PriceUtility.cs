using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Amlakbashi.Core.Common.Utilities
{
    public static class PriceUtility
    {
        public static string GetPriceString(int price_val)
        {
            var str_toman = "";
            if (price_val >= 1000000000)
            {
                str_toman += Math.Floor((decimal)price_val / (decimal)1000000000) + " میلیارد";
                price_val = price_val % 1000000000;
            }
            if (price_val >= 1000000)
            {
                if (str_toman.Length > 3)
                {
                    str_toman += " و " + Math.Floor((decimal)price_val / (decimal)1000000) + " میلیون";
                }
                else
                {
                    str_toman += Math.Floor((decimal)price_val / (decimal)1000000) + " میلیون";
                }
                price_val = price_val % 1000000;
            }
            if (price_val >= 1000)
            {
                if (str_toman.Length > 3)
                {
                    str_toman += " و " + Math.Floor((decimal)price_val / (decimal)1000) + " هزار";
                }
                else
                {
                    str_toman += Math.Floor((decimal)price_val / (decimal)1000) + " هزار";
                }
                price_val = price_val % 1000;

            }
            if (price_val > 0)
            {

                if (str_toman.Length > 3)
                {
                    str_toman += " و " + price_val;
                }
                else
                {
                    str_toman += price_val;
                }
            }

            return str_toman;
        }
        public static string PriceToSpecialString(int price)
        {
            var priceStr = price.ToString();
            if (priceStr.Length > 9)
            {
                var temp = price / 1000000000;
                return temp.ToString() + " میلیارد";
            }
            else if (priceStr.Length > 6)
            {
                var temp = price / 1000000;
                return temp.ToString() + " میلیون";
            }
            else if (priceStr.Length > 3)
            {
                var temp = price / 1000;
                return temp.ToString() + " هزار";
            }
            else
            {
                return priceStr;
            }
        }

        public static int CalculateDiscountAmount(int price, int discountPercent)
        {
            if (discountPercent < 1)
                return 0;
            var discount_price = (int)(((float)price / 100f) * (float)discountPercent);
            return discount_price;
        }

        public static long CalculateHostPayablePrice(long total_price,
            long guest_payed_price, long couponPrice, long prizePrice)
        {
            var payable_price = total_price - (long)Math.Round((double)total_price / 10f);
            payable_price += couponPrice;
            payable_price += prizePrice;
            return payable_price - (total_price - guest_payed_price);
        }
    }
}
