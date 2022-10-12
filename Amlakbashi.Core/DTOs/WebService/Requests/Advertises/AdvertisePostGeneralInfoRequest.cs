using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Amlakbashi.Core.DTOs.WebService.Requests.Advertises
{
    public class AdvertisePostGeneralInfoRequest
    {
        [Range(1, int.MaxValue)]
        public long residenceId { get; set; }

        [Range(1, int.MaxValue)]
        public int provinceId { get; set; }

        [Range(1, int.MaxValue)]
        public int cityId { get; set; }

        [Range(0, int.MaxValue)]
        public int areaId { get; set; }

        [Required]
        public string address { get; set; }
        public double longitude { get; set; }
        public double latitude { get; set; }

        [Required]
        public string title { get; set; }

        [Required]
        public string description { get; set; }

        public long mainPhotoId { get; set; }

        [BindNever]
        public int userId { get; set; }
    }
}
