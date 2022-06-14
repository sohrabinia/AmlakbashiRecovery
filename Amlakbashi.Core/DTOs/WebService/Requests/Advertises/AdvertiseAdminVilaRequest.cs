using Amlakbashi.Core.Entities;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Amlakbashi.Core.DTOs.WebService.Requests.Advertises
{
    public class AdvertiseAdminVilaRequest
    {
        public List<AdvertiseAdminVilaSiteItemsRequest> sites { get; set; }
        public List<AdvertiseAdminVilaDateItemsRequest> dates { get; set; }
    }

    public class AdvertiseAdminVilaSiteItemsRequest
    {
        public string siteName { get; set; }
        public string vilaNo { get; set; }
    }

    public class AdvertiseAdminVilaDateItemsRequest
    {
        [Required]
        public string date { get; set; }

        [Range(30000, int.MaxValue)]
        public int price { get; set; }

        public bool reserved { get; set; }
    }
}
