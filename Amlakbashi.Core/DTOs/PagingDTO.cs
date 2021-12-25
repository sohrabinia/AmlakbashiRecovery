using System;
using System.Collections.Generic;
using System.Text;

namespace Amlakbashi.Core.DTOs
{
    public class PagingDTO
    {
        public PagingDTO(int page, int totalItemsCount, int pageItemCount = 20)
        {
            this.CurrentPage = page;
            this.TotalItems = totalItemsCount;
            this.PageItemCount = pageItemCount;
        }
        public int TotalItems { get; set; }
        public int PageItemCount { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages => TotalItems % PageItemCount == 0 ? TotalItems / PageItemCount : (TotalItems / PageItemCount) + 1;
        public int PageRowStart => (CurrentPage * PageItemCount) - PageItemCount;
    }
}
