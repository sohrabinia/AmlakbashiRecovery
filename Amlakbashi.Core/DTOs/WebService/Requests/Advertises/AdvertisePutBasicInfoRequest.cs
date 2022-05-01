using Amlakbashi.Core.Entities;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Amlakbashi.Core.DTOs.WebService.Requests.Advertises
{
    public class AdvertisePutBasicInfoRequest
    {
        [Range(1, int.MaxValue)]
        public long advertiseId { get; set; }
        public Advertise.AdvertiseType type { get; set; }
        public Advertise.PositionType locationType { get; set; }

        [BindNever]
        public int userId { get; set; }

        public bool IsValid(ModelStateDictionary modelState)
        {
            if (type == Advertise.AdvertiseType.All || type == Advertise.AdvertiseType.None)
            {
                modelState.AddModelError(nameof(type), "value is incorrect");
            }
            if (locationType == Advertise.PositionType.none ||
                Enum.IsDefined(typeof(Advertise.PositionType), locationType) == false)
            {
                modelState.AddModelError(nameof(locationType), "value is incorrect");
            }
            return modelState.IsValid;
        }
    }
}
