using Amlakbashi.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amlakbashi.Core.DTOs.WebService.Responses.Advertises
{
    public class AdvertiseGetFinalInfoForUpdateResponse
    {
        public long residenceId { get; set; }
        public int capacity { get; set; }
        public int extraCapacity { get; set; }
        public int dailyPrice { get; set; }
        public int holidayPrice { get; set; }
        public int peakHolidayPrice { get; set; }
        public int extraCapacityPrice { get; set; }
        public long monthlyPrice { get; set; }
        public int nowruzPrice { get; set; }
        public int buildingArea { get; set; }
        public Advertise.ParkingItems parking { get; set; }
        public int roomCount { get; set; }
        public Advertise.FloorItems floor { get; set; }
        public int singleBedCount { get; set; }
        public int doubleBedCount { get; set; }
        public int blanketAndMattressCount { get; set; }
        public Advertise.ExtraBlanketCountItems extraBlanketCount { get; set; }

        public static implicit operator AdvertiseGetFinalInfoForUpdateResponse(Advertise advertise)
        {
            return new AdvertiseGetFinalInfoForUpdateResponse()
            {
                residenceId = advertise.Id,
                capacity = advertise.Capacity,
                extraCapacity = advertise.ExtraCapacity,
                dailyPrice = advertise.DailyPrice,
                holidayPrice = advertise.HolidayPrice,
                peakHolidayPrice = advertise.PeakHolidayPrice,
                monthlyPrice = advertise.MonthlyPrice,
                nowruzPrice = advertise.NowruzPrice,
                extraCapacityPrice = advertise.ExtraCapacityPrice,
                buildingArea = advertise.BuildingArea,
                parking = advertise.Parking,
                roomCount = advertise.RoomCount,
                floor = advertise.Floor,
                singleBedCount = advertise.SingleBedCount,
                doubleBedCount = advertise.DoubleBedCount,
                blanketAndMattressCount = advertise.BlanketAndMattressCount,
                extraBlanketCount = advertise.ExtraBlanketCount
            };
        }
    }
}
