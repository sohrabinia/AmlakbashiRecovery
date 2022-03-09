using System;
using System.Collections.Generic;
using System.Text;

namespace Amlakbashi.Core.DTOs.WebService.Responses
{
    public class HomePageResponse
    {
        public Dictionary<string, int> residencyTypes { get; set; }
        public List<HomePageMostViewedResponse> mostViewed { get; set; }
        public List<HomePageLastSecondsResponse> lastSeconds { get; set; }
        public List<HomePageMostLikedResponse> mostLiked { get; set; }
        public List<HomePageInstantResponse> instant { get; set; }
        public List<HomePageMagResponse> mag { get; set; }
    }

    public class HomePageMostViewedResponse
    {
        public string cityName { get; set; }
        public string imageUrl { get; set; }
        public int residencyCount { get; set; }
    }

    public class HomePageLastSecondsResponse
    {
        public string title { get; set; }
        public string imageUrl { get; set; }
        public List<string> tags { get; set; }
        public int nightlyPrice { get; set; }
        public int discountPercent { get; set; }
        public double discountPrice { get; set; }
    }

    public class HomePageMostLikedResponse
    {
        public string title { get; set; }
        public string imageUrl { get; set; }
        public List<string> tags { get; set; }
        public int nightlyPrice { get; set; }
        public double rating { get; set; }
        public int commentsCount { get; set; }
    }

    public class HomePageInstantResponse
    {
        public string title { get; set; }
        public string imageUrl { get; set; }
        public List<string> tags { get; set; }
        public int nightlyPrice { get; set; }
        public double rating { get; set; }
        public int commentsCount { get; set; }
        public string badgeText { get; set; }
    }

    public class HomePageMagResponse
    {
        public string title { get; set; }
        public string imageUrl { get; set; }
        public string summary { get; set; }
        public string link { get; set; }
    }
}
