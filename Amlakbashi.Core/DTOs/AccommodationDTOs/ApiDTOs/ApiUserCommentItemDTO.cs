using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amlakbashi.Core.DTOs.AccommodationDTOs.ApiDTOs
{
    [Serializable]
    public class ApiUserCommentItemDTO
    {
        public long id { get; set; }
        public string fullName { get; set; }
        public string firstName { get; set; }
        public long photoId { get; set; }
        public string comment { get; set; }
        public float score { get; set; }
        public ApiUserRatingItemDTO ratingDetail { get; set; }
        public DateTime? date { get; set; }
        public string persianDate { get; set; }
        public ApiUserCommentItemDTO reply { get; set; }
    }
}
