using Amlakbashi.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amlakbashi.Core.DTOs.AccommodationDTOs.AccPagesDTOs
{
    public class AccommodationStatisticsDTO
    {
        public AccommodationStatisticsDTO()
        {
            this.OverallRatingDecimal = this.OverallRatingDecimal == null ? new Dictionary<Comment.UserRatingType, float>() :
                this.OverallRatingDecimal;
            this.UserRatingTypeString = this.UserRatingTypeString == null ? new Dictionary<Comment.UserRatingType, string>() :
                this.UserRatingTypeString;
        }
        public int SuccessfullReserveCount { get; set; }
        public int CountUserRating { get; set; }
        public Dictionary<Comment.UserRatingType, float> OverallRatingDecimal { get; set; }
        public Dictionary<Comment.UserRatingType, string> UserRatingTypeString { get; set; }
    }
}
