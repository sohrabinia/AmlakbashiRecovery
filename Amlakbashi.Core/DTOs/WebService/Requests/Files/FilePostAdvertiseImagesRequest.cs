using Amlakbashi.Core.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amlakbashi.Core.DTOs.WebService.Requests.Files
{
    public class FilePostAdvertiseImagesRequest
    {
        public long advertiseId { get; set; }
        public IFormCollection images { get; set; }

        [BindNever]
        public int userId { get; set; }

        public bool IsValid(ModelStateDictionary modelState)
        {
            if (advertiseId < 1)
            {
                modelState.AddModelError(nameof(advertiseId), "advertise id is incorrect");
            }
            if (images.Files.Count == 0)
            {
                modelState.AddModelError(nameof(images), "images is empty");
            }
            foreach (var item in images.Files)
            {
                if (File.IsValidImageContentType(item.ContentType) == false)
                {
                    modelState.AddModelError(nameof(images), "image(s) has incorrect content type");
                }
            }
            return modelState.IsValid;
        }
    }
}
