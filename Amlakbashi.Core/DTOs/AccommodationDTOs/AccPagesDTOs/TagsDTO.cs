using Amlakbashi.Core.Entities;
using ServiceStack;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amlakbashi.Core.DTOs.AccommodationDTOs.AccPagesDTOs
{
    public class TagsDTO
    {
        public Dictionary<int, string> TagsDic { get; set; } = new Dictionary<int, string>();
    }
}
