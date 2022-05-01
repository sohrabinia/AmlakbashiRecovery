using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Amlakbashi.Core.DTOs.WebService.Requests.Advertises
{
    public class AdvertisePutGeneralInfoRequest
    {
        [Range(1, int.MaxValue)]
        public long advertiseId { get; set; }

        [Range(1, int.MaxValue)]
        public int province { get; set; }

        [Range(1, int.MaxValue)]
        public int city { get; set; }

        [Range(0, int.MaxValue)]
        public int area { get; set; }

        [Required]
        public string address { get; set; }
        public double longitude { get; set; }
        public double latitude { get; set; }

        [Required]
        public string title { get; set; }

        [Required]
        public string description { get; set; }

        [BindNever]
        public int userId { get; set; }

        public bool IsValid(ModelStateDictionary modelState)
        {
            //if (province < 1)
            //{
            //    modelState.AddModelError(nameof(province), "value is incorrect");
            //}
            //if (city < 1)
            //{
            //    modelState.AddModelError(nameof(city), "value is incorrect");
            //}
            //if (area < 1)
            //{
            //    modelState.AddModelError(nameof(area), "value is incorrect");
            //}
            //if (string.IsNullOrEmpty(address))
            //{
            //    modelState.AddModelError(nameof(address), "value is incorrect");
            //}
            //if (string.IsNullOrEmpty(title))
            //{
            //    modelState.AddModelError(nameof(title), "value is incorrect");
            //}
            //if (string.IsNullOrEmpty(description))
            //{
            //    modelState.AddModelError(nameof(description), "value is incorrect");
            //}
            return modelState.IsValid;
        }
    }
}
