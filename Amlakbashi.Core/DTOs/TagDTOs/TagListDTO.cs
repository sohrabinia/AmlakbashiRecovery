using Amlakbashi.Core.Entities;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amlakbashi.Core.DTOs.TagDTOs
{
    public class TagListDTO
    {
        [BindNever]
        public PagedList<Tag> pagedList { get; set; } = new PagedList<Tag>();
        public string title { get; set; }
        public Tag.TagStatusEnum? status { get; set; } = null;
        public int page { get; set; } = 1;
        public byte pageItemCount { get; set; } = 20;
    }
}
