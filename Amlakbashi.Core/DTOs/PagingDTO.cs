using System;
using System.Collections.Generic;
using System.Text;

namespace Amlakbashi.Core.DTOs
{
    public class PagingDTO
    {
        public int TotalItems { get; set; }
        public int PageItems { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages => TotalItems % PageItems == 0 ? TotalItems / PageItems : (TotalItems / PageItems) + 1;
    }
}
