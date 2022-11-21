using Amlakbashi.Core.Common.Utilities;
using Amlakbashi.Core.DTOs.AccommodationDTOs;
using Amlakbashi.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amlakbashi.Core.DTOs.TagDTOs
{
    public class TagResidencesDTO
    {
        public PagedList<AccommodationCardDTO> pagedList { get; set; } = new PagedList<AccommodationCardDTO>();
        public IList<Tag> similarTags { get; set; } = new List<Tag>();
        public string urlTitle { get; set; }
        public string title { get { return StringUtility.GetTagTitle(urlTitle); } }
        public int page { get; set; } = 1;
        public byte pageItemCount { get; set; } = 20;
    }
}
