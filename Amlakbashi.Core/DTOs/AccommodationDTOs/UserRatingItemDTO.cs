using Amlakbashi.Core.Entities;
using static Amlakbashi.Core.Entities.Comment;

namespace Amlakbashi.Core.DTOs.AccommodationDTOs
{
    public class UserRatingItemDTO
    {
        public UserRatingType type;
        public string typeString;
        public int rating;

        public static implicit operator UserRatingItemDTO(ReportItem reportItem)
        {
            var result = new UserRatingItemDTO();
            result.type = (UserRatingType)reportItem.ReportID;
            result.typeString = ReportItem.GetUserRatingTypeString(result.type);
            result.rating = reportItem.Score;
            return result;
        }
    }
}
