using Amlakbashi.Core.Entities;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Amlakbashi.Core.DTOs.WebService.Requests.Advertises
{
    public class AdvertisePostFinalInfoRequest
    {
        [Range(1, long.MaxValue)]
        public long residenceId { get; set; }

        [Range(1, int.MaxValue)]
        public int capacity { get; set; }

        [Range(1, int.MaxValue)]
        public int extraCapacity { get; set; }

        [Range(1, int.MaxValue)]
        public int dailyPrice { get; set; }

        [Range(1, int.MaxValue)]
        public int holidayPrice { get; set; }

        [Range(1, int.MaxValue)]
        public int peakHolidayPrice { get; set; }

        [Range(1, int.MaxValue)]
        public int extraCapacityPrice { get; set; }

        [Range(1, int.MaxValue)]
        public long monthlyPrice { get; set; }

        [Range(1, int.MaxValue)]
        public int nowruzPrice { get; set; }

        [Range(1, int.MaxValue)]
        public int nowruzExtraCapacityPrice { get; set; }

        [Range(1, int.MaxValue)]
        public int buildingArea { get; set; }

        [Range(0, int.MaxValue)]
        public int roomCount { get; set; }

        [Range(0, int.MaxValue)]
        public int singleBedCount { get; set; }

        [Range(0, int.MaxValue)]
        public int doubleBedCount { get; set; }

        [Range(0, int.MaxValue)]
        public int blanketAndMattressCount { get; set; }

        public Advertise.ParkingItems parking { get; set; }

        public Advertise.FloorItems floor { get; set; }

        public Advertise.ExtraBlanketCountItems extraBlanketCount { get; set; }

        [BindNever]
        public int userId { get; set; }

        public bool IsValid(ModelStateDictionary modelState)
        {
            if (parking == Advertise.ParkingItems.Unset || parking == Advertise.ParkingItems.Jointly ||
                Enum.IsDefined(typeof(Advertise.ParkingItems), parking) == false)
            {
                modelState.AddModelError(nameof(parking), "value is incorrect");
            }
            if (floor == Advertise.FloorItems.Unset ||
                Enum.IsDefined(typeof(Advertise.FloorItems), floor) == false)
            {
                modelState.AddModelError(nameof(floor), "value is incorrect");
            }
            if (extraBlanketCount == Advertise.ExtraBlanketCountItems.Unset ||
                Enum.IsDefined(typeof(Advertise.ExtraBlanketCountItems), extraBlanketCount) == false)
            {
                modelState.AddModelError(nameof(extraBlanketCount), "value is incorrect");
            }
            return modelState.IsValid;
        }
    }
}
