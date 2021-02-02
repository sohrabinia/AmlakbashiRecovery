using Amlakbashi.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Amlakbashi.Core.DTOs.AccommodationDTOs
{
    public class UserRatingDTO
    {
        public long advertiseId;
        public string advertiseTitle;
        public string cityName;
        public List<UserRatingItemDTO> items;

        public static UserRatingDTO Generate(List<ReportItem> reportItems,
            long advertiseId, string advertiseTitle, string cityName)
        {
            var allTypes = Enum.GetValues(typeof(Comment.UserRatingType))
                as Comment.UserRatingType[];
            var result = new UserRatingDTO();
            result.items = new List<UserRatingItemDTO>();
            if (reportItems == null)
                return result;
            reportItems.ForEach(item => result.items.Add(item));
            foreach (var item in allTypes.Where(w => result.items.Any(a => a.type == w) == false))
            {
                result.items.Add(new UserRatingItemDTO() {
                    type = item,
                    rating = 0,
                    typeString = ReportItem.GetUserRatingTypeString(item)
                });
            }
            result.advertiseId = advertiseId;
            result.advertiseTitle = advertiseTitle;
            result.cityName = cityName;
            return result;
        }
    }
}
