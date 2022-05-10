using Amlakbashi.Core.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amlakbashi.Core.DTOs.WebService.Requests.Files
{
    public class FilePostAdvertiseLicenseImageRequest
    {
        public long advertiseId { get; set; }
        public IFormFile image { get; set; }

        [BindNever]
        public int userId { get; set; }

        public bool IsValid(ModelStateDictionary modelState)
        {
            if (advertiseId < 1)
            {
                modelState.AddModelError(nameof(advertiseId), "advertise id is incorrect");
            }
            if (image == null)
            {
                modelState.AddModelError(nameof(image), "image is empty");
            }
            else if (File.IsValidImageContentType(image.ContentType) == false)
            {
                modelState.AddModelError(nameof(image), "image has incorrect content type");
            }
            return modelState.IsValid;
        }
    }
}
