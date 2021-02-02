using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amlakbashi.Core.DTOs.AccommodationDTOs.ApiDTOs
{
    [Serializable]
    public class ApiHomePageCarouselDTO
    {
        public string title { get; set; }
        public int cid { get; set; }
        public int type { get; set; } //0: category, 1: discount
        public List<ApiAdvertiseItemDTO> items { get; set; }
    }
}
