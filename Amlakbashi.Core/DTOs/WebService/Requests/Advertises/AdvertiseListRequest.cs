using Amlakbashi.Core.Entities;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amlakbashi.Core.DTOs.WebService.Requests.Advertises
{
    public class AdvertiseListRequest
    {
        public int regionId { get; set; }
        public int area { get; set; }
        public int city { get; set; }
        public int province { get; set; }
        public string phrase { get; set; }
        public string fromDate { get; set; }
        public string toDate { get; set; }
        public int capacity { get; set; }
        public Advertise.priceRangeTypes priceType { get; set; }
        public int minPrice { get; set; }
        public int maxPrice { get; set; }
        public Advertise.AdvertiseType residencyType { get; set; } = Advertise.AdvertiseType.All;
        public bool instantReserve { get; set; }
        public bool emptyTonight { get; set; }
        public Advertise.SortOrder sort { get; set; }
        public int bedCount { get; set; }
        public int roomCount { get; set; }
        public bool longTimeReserve { get; set; }
        public bool party { get; set; }
        public bool pets { get; set; }
        public bool smoking { get; set; }
        public List<Advertise.PositionType> locationTypes { get; set; }

        public bool parking { get; set; }
        public bool elevator { get; set; }
        public bool pool { get; set; }
        public Advertise.WCItems wcType { get; set; }
        public bool wifi { get; set; }
        public bool washingMachine { get; set; }
        public bool jacuzzi { get; set; }
        public bool poolTable { get; set; }
        public bool foosball { get; set; }
        public bool teaMaker { get; set; }
        
        public bool norouz { get; set; }
        public int page { get; set; } = 1;
        public int pageItemCount { get; set; } = 20;

        [BindNever]
        public int categoryId { get; set; }

        [BindNever]
        public int userId { get; set; }
    }
}
