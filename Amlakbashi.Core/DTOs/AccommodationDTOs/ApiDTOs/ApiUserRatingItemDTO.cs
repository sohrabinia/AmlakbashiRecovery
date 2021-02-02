using Amlakbashi.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amlakbashi.Core.DTOs.AccommodationDTOs.ApiDTOs
{
    [Serializable]
    public class ApiUserRatingItemDTO
    {
        public float tidiness { get; set; }
        public float hostBehaviour { get; set; }
        public float position { get; set; }
        public float infoCorrectness { get; set; }
        public float safety { get; set; }
        public float priceWorth { get; set; }

        public static ApiUserRatingItemDTO Generate(Advertise advertise,
            int userId, out float average)
        {
            var advertiseReportItems = advertise.ReportItems.Where(x => x.UserID == userId);
            var tidiness = advertiseReportItems.FirstOrDefault(x => x.ReportID == 1);
            var hostBehaviour = advertiseReportItems.FirstOrDefault(x => x.ReportID == 2);
            var position = advertiseReportItems.FirstOrDefault(x => x.ReportID == 3);
            var infoCorrectness = advertiseReportItems.FirstOrDefault(x => x.ReportID == 4);
            var safety = advertiseReportItems.FirstOrDefault(x => x.ReportID == 5);
            var priceWorth = advertiseReportItems.FirstOrDefault(x => x.ReportID == 6);
            var output = new ApiUserRatingItemDTO()
            {
                tidiness = tidiness != null ? tidiness.Score : 0,
                hostBehaviour = hostBehaviour != null ? hostBehaviour.Score : 0,
                position = position != null ? position.Score : 0,
                infoCorrectness = infoCorrectness != null ? infoCorrectness.Score : 0,
                safety = safety != null ? safety.Score : 0,
                priceWorth = priceWorth != null ? priceWorth.Score : 0
            };
            average = advertiseReportItems.Any() ? advertiseReportItems.Average(x => (float)x.Score) : 0;
            return output;
        }
    }
}
