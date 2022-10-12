using Amlakbashi.Core.Entities;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Amlakbashi.Core.DTOs.WebService.Requests.Advertises
{
    public class AdvertiseUpdatePriceRequest
    {
        [Range(1, int.MaxValue)]
        public long advertiseId { get; set; }

        [Required]
        public string fromDate { get; set; }
        public string toDate { get; set; }

        [Range(30000, int.MaxValue)]
        public int price { get; set; }

        [BindNever]
        public ActionLog.ActionSourceEnum actionSource { get; set; }

        [BindNever]
        public int userId { get; set; }
    }
}
