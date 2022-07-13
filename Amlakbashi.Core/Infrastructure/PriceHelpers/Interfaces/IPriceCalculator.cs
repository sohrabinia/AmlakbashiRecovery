using Amlakbashi.Core.DTOs.AccommodationDTOs;
using Amlakbashi.Core.DTOs.ReserveDTOs;
using Amlakbashi.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amlakbashi.Core.Infrastructure.PriceHelpers.Interfaces
{
    public interface IPriceCalculator
    {
        long CalculateReservePrice(Advertise advertise,
            string startDate, string endDate, int guestCount,
            out long priceWithoutDiscount, out long couponCalculationPrice);
        IDictionary<string, DatePriceDTO> CalculateJalaliDatePrices(
            DateTime from, DateTime to, Advertise advertise,
            out long couponCalculationPrice, int moreThanCapacity = 0);

        ReserveCancelationLossDTO CaculateReserveCancelationLoss(Reserve reserve);
    }
}
