using Amlakbashi.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amlakbashi.Core.DTOs.WebService.Responses.Advertises
{
    public class AdvertiseGetFinalInfoForUpdateResponse
    {
        public long advertiseId { get; set; }
        public int capacity { get; set; }
        public int extraCapacity { get; set; }
        public int dailyPrice { get; set; }
        public int holidayPrice { get; set; }
        public int holidayPikePrice { get; set; }
        public int extraCapacityPrice { get; set; }
        public long monthlyPrice { get; set; }
        public int norouzPrice { get; set; }
        public int metrazh { get; set; }
        public Advertise.ParkingItems parking { get; set; }
        public int roomCount { get; set; }
        public Advertise.FloorItems floor { get; set; }
        public int singleBedCount { get; set; }
        public int doublesBedCount { get; set; }
        public int blanketsAndMattressesCount { get; set; }
        public Advertise.ExtraBlanketCountItems extraBlanketCount { get; set; }

        public static implicit operator AdvertiseGetFinalInfoForUpdateResponse(Advertise advertise)
        {
            return new AdvertiseGetFinalInfoForUpdateResponse()
            {
                advertiseId = advertise.Id,
                capacity = advertise.Capacity,
                extraCapacity = advertise.MoreThanCapacity,
                dailyPrice = advertise.DailyPrice,
                holidayPrice = advertise.HolidayPrice,
                holidayPikePrice = advertise.HolidayPikePrice,
                monthlyPrice = advertise.RentPrice,
                norouzPrice = advertise.NorouzPrice,
                extraCapacityPrice = advertise.MoreThanCapacityPrice,
                metrazh = advertise.Metrazh,
                parking = advertise.Parking,
                roomCount = advertise.Room,
                floor = advertise.Floor,
                singleBedCount = advertise.SingleBed,
                doublesBedCount = advertise.DoublesBed,
                blanketsAndMattressesCount = advertise.BlanketsAndMattresses,
                extraBlanketCount = advertise.ExtraBlanketCount
            };
        }
    }
}
