using Amlakbashi.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Amlakbashi.Core.DTOs.WebService.Responses.Advertises
{
    public class AdvertiseGetHotelRoomInfoForUpdateResponse
    {
        public long unitId { get; set; }
        public long parentId { get; set; }
        public List<AdvertiseHotelRoomsResponse> parentUnits { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public int capacity { get; set; }
        public int extraCapacity { get; set; }
        public int dailyPrice { get; set; }
        public int holidayPrice { get; set; }
        public int holidayPikePrice { get; set; }
        public int extraCapacityPrice { get; set; }
        public long monthlyPrice { get; set; }
        public int norouzPrice { get; set; }
        public int norouzExtraCapacityPrice { get; set; }
        public int metrazh { get; set; }
        public int count { get; set; }
        public int singleBedCount { get; set; }
        public int doublesBedCount { get; set; }
        public int blanketsAndMattressesCount { get; set; }
        public Advertise.ExtraBlanketCountItems extraBlanketCount { get; set; }

        public static implicit operator AdvertiseGetHotelRoomInfoForUpdateResponse(Advertise advertise)
        {
            return new AdvertiseGetHotelRoomInfoForUpdateResponse()
            {
                unitId = advertise.Id,
                parentId = advertise.ParentId ?? 0,
                capacity = advertise.Capacity,
                extraCapacity = advertise.MoreThanCapacity,
                dailyPrice = advertise.DailyPrice,
                holidayPrice = advertise.HolidayPrice,
                holidayPikePrice = advertise.HolidayPikePrice,
                monthlyPrice = advertise.RentPrice,
                norouzPrice = advertise.NorouzPrice,
                extraCapacityPrice = advertise.MoreThanCapacityPrice,
                norouzExtraCapacityPrice = advertise.NorouzOverCapacityPrice,
                metrazh = advertise.Metrazh,
                count = advertise.Count,
                singleBedCount = advertise.SingleBed,
                doublesBedCount = advertise.DoublesBed,
                blanketsAndMattressesCount = advertise.BlanketsAndMattresses,
                extraBlanketCount = advertise.ExtraBlanketCount,
                title = advertise.Title,
                description = advertise.Description,
                parentUnits = advertise.Parent.Childs.Select(x=>new AdvertiseHotelRoomsResponse() { 
                        unitId = x.Id,
                        title = x.Title
                    }).ToList()
            };
        }
    }

    public class AdvertiseHotelRoomsResponse
    {
        public long unitId { get; set; }
        public string title { get; set; }
    }
}
