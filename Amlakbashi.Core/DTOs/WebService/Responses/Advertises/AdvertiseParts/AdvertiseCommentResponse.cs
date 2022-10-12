using System;
using System.Collections.Generic;
using System.Text;

namespace Amlakbashi.Core.DTOs.WebService.Responses.Advertises.AdvertiseParts
{
    public class AdvertiseCommentResponse
    {
        public AdvertiseCommentResponse()
        {
            detailedRates = new List<AdvertiseRateItemResponse>();
            comments = new List<AdvertiseCommentItemResponse>();
        }
        public int rateCount { get; set; }
        public float rate { get; set; }
        public List<AdvertiseRateItemResponse> detailedRates { get; set; }
        public List<AdvertiseCommentItemResponse> comments { get; set; }

    }

    public class AdvertiseRateItemResponse
    {
        public AdvertiseRateItemResponse(string title, float rate)
        {
            this.title = title;
            this.rate = rate;
        }
        public string title { get; set; }
        public float rate { get; set; }
    }

    public class AdvertiseCommentItemResponse
    {
        public string name { get; set; }
        public string date { get; set; }
        public string comment { get; set; }
        public string imageUrl { get; set; }
    }
}
