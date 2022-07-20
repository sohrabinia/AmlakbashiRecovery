using Amlakbashi.Core.Common.Localization;
using Amlakbashi.Core.Common.Utilities;
using Amlakbashi.Core.DTOs.AccommodationDTOs;
using Amlakbashi.Core.DTOs.ReserveDTOs;
using Amlakbashi.Core.Entities;
using Amlakbashi.Core.Infrastructure.PriceHelpers.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Amlakbashi.Core.Infrastructure.PriceHelpers
{
    public class PriceCalculator : IPriceCalculator
    {
        private readonly ILocalization localization;
        public PriceCalculator(ILocalization localization)
        {
            this.localization = localization;
        }

        public long CalculateReservePrice(Advertise advertise,
            string startDate, string endDate, int guestCount,
            out long priceWithoutDiscount, out long couponCalculationPrice)
        {
            var days = DateTimeUtility.GetPersianDateRangeDays(startDate, endDate);
            couponCalculationPrice = 0;
            var moreThanCapacity = Math.Max(0, guestCount - advertise.Capacity);
            if (advertise.RentPrice > 0 && days >= 30)
            {
                var price = (long)Math.Round(((double)days * (double)((double)advertise.RentPrice / 30f) / 10000f), 0) * 10000;
                price += (moreThanCapacity * advertise.MoreThanCapacityPrice) * days;
                priceWithoutDiscount = price;
                return price;
            }
            var from = DateTimeUtility.PersianDateToGregorian(startDate);
            var to = DateTimeUtility.PersianDateToGregorian(endDate).AddDays(-1);
            var prices = CalculateJalaliDatePrices(from, to, advertise,
                out couponCalculationPrice, moreThanCapacity);
            priceWithoutDiscount = prices.Sum(s => s.Value.price + s.Value.off);
            return prices.Sum(s => s.Value.price);
        }

        public IDictionary<string, DatePriceDTO> CalculateJalaliDatePrices(
            DateTime from, DateTime to, Advertise advertise,
            out long couponCalculationPrice, int moreThanCapacity = 0)
        {
            var minDate = DateTime.Now.TimeOfDay.Hours > 3 ? from.Date : from.Date.AddDays(-1);
            var minDateUnix = DateTimeUtility.DateValueOfJS(minDate);
            var priceTables = advertise.PriceTables.Where(w => w.UnixDate >= minDateUnix);
            var discountTables = advertise.DiscountTables.Where(w => w.To >= minDate);
            var result = new Dictionary<string, DatePriceDTO>();
            couponCalculationPrice = 0;
            for (DateTime gregorianDate = from; gregorianDate <= to;
                gregorianDate = gregorianDate.AddDays(1))
            {
                var jalaliDate = DateTimeUtility.GregorianToPersianDate(gregorianDate);
                int priceWithoutDiscount = 0;
                var unixDate = DateTimeUtility.DateValueOfJS(gregorianDate);
                var priceTable = priceTables.FirstOrDefault(x => x.UnixDate == unixDate);

                bool is_holiday_or_between;
                bool is_holiday_pike;
                bool is_norouz;
                localization.GetJalaliDateHolidayStatus(jalaliDate, out is_holiday_or_between, out is_holiday_pike, out is_norouz);
                if (priceTable != null)
                {
                    priceWithoutDiscount = priceTable.Price;
                }
                else
                {
                    // TODO: temp
                    //if (gregorianDate.Month == 8 && (gregorianDate.Day == 16 || gregorianDate.Day == 17 || gregorianDate.Day == 20))
                    //{
                    //    is_holiday_pike = true;
                    //}
                    //if (DateTimeUtility.ManualHolidayPeakPersianDates.Contains(jalaliDate))
                    //{
                    //    is_holiday_pike = true;
                    //}
                    // ##########

                    if (is_norouz && advertise.NorouzPrice > 0)
                    {
                        priceWithoutDiscount = advertise.NorouzPrice;
                    }
                    else if (is_holiday_pike)
                    {
                        priceWithoutDiscount = advertise.HolidayPikePrice > 0 ? advertise.HolidayPikePrice : advertise.DailyPrice;
                    }
                    else if (is_holiday_or_between)
                    {
                        priceWithoutDiscount = advertise.HolidayPrice > 0 ? advertise.HolidayPrice : advertise.DailyPrice;
                    }
                    else
                    {
                        priceWithoutDiscount = advertise.DailyPrice;
                    }
                }
                if (moreThanCapacity > 0)
                {
                    if (is_norouz && advertise.NorouzOverCapacityPrice > 0)
                    {
                        priceWithoutDiscount += (moreThanCapacity * advertise.NorouzOverCapacityPrice);
                    }
                    else
                    {
                        priceWithoutDiscount += (moreThanCapacity * advertise.MoreThanCapacityPrice);
                    }
                }
                var discounts = discountTables.Where(f => gregorianDate >= f.From && gregorianDate < f.To);
                DiscountTable discount = null;
                if (discounts.Any())
                {
                    discount = discounts.OrderByDescending(o => o.Id).First();
                }
                var price = priceWithoutDiscount - PriceUtility.CalculateDiscountAmount(priceWithoutDiscount, discount?.Percent ?? 0);
                result.Add(unixDate.ToString(), new DatePriceDTO() { price = price, off = priceWithoutDiscount - price });
                if (DateTime.Compare(gregorianDate, from) == 0)
                {
                    couponCalculationPrice = price;
                }
            }
            return result;
        }

        public ReserveCancelationLossDTO CaculateGuestReserveCancelationLoss(Reserve reserve)
        {
            var startDate = reserve.StartDate.AddHours(14);
            var remainedHours = (startDate - DateTime.Now).TotalHours;
            var reserveDaysCount = (reserve.EndDate - reserve.StartDate).TotalDays;
            int holidayPeakDayCount = 0;
            var guestPaidAmount = reserve.GetGuestPaidAmount();

            var dto = new ReserveCancelationLossDTO()
            {
                SitePortion = (long)Math.Round(reserve.TotalPrice * 0.1, 0)
            };
            if ((reserve.StartDate - reserve.CreateDate).TotalHours < 6)
            {
                long couponPrice = 0;
                var datePrices = CalculateJalaliDatePrices(reserve.StartDate, reserve.StartDate.AddDays(1), reserve.Advertise,
                        out couponPrice, Math.Max(0, reserve.NumberOfGuests - reserve.Advertise.Capacity));
                var firstDayHostPortion = (long)Math.Round(datePrices.First().Value.price * 0.9, 0);
                dto.HostPortion = (long)Math.Round(firstDayHostPortion * 0.5, 0);
                dto.GuestPortion = guestPaidAmount - (dto.SitePortion + dto.HostPortion);
                return dto;
            }

            for (DateTime gregorianDate = reserve.StartDate; gregorianDate < reserve.EndDate; gregorianDate = gregorianDate.AddDays(1))
            {
                bool isHoliday;
                bool isHolidayPike;
                bool isNorouz;
                localization.GetJalaliDateHolidayStatus(DateTimeUtility.GregorianToPersianDate(gregorianDate),
                    out isHoliday, out isHolidayPike, out isNorouz);
                if (isHolidayPike || isNorouz)
                {
                    holidayPeakDayCount += 1;
                }
            }

            if (holidayPeakDayCount > 0 && (reserveDaysCount / holidayPeakDayCount) < 2)
            {
                if (remainedHours < 168)
                {
                    dto.HostPortion = (long)Math.Round(reserve.TotalPrice * 0.9, 0);
                }
            }
            else
            {
                if (remainedHours < 72)
                {
                    long couponPrice = 0;
                    var datePrices = CalculateJalaliDatePrices(reserve.StartDate, reserve.StartDate.AddDays(2), reserve.Advertise,
                        out couponPrice, Math.Max(0, reserve.NumberOfGuests - reserve.Advertise.Capacity));
                    var realHostPortion = (long)Math.Round(datePrices.First().Value.price * 0.9, 0);
                    if (remainedHours < 14 && reserveDaysCount > 1)
                    {
                        realHostPortion += (long)Math.Round(datePrices.ElementAt(1).Value.price * 0.9, 0);
                        if (reserveDaysCount >= 30)
                        {
                            realHostPortion += (long)Math.Round(datePrices.Last().Value.price * 0.9, 0);
                        }
                    }
                    dto.HostPortion = guestPaidAmount >= realHostPortion ? realHostPortion : guestPaidAmount - dto.SitePortion;
                }
            }
            dto.GuestPortion = guestPaidAmount - (dto.SitePortion + dto.HostPortion);
            return dto;
        }
    }
}
