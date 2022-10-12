using System;
using System.Collections.Generic;
using System.Text;

namespace Amlakbashi.Core.DTOs
{
    public class PagingInfo
    {
        public PagingInfo(int totalItemsCount, int page, int pageItemCount)
        {
            this.page = page > 0 ? page : 1;
            this.pageItemCount = pageItemCount > 0 ? pageItemCount : 20;
            this.totalItemsCount = totalItemsCount;
        }

        public int page { get; set; }
        public int pageItemCount { get; set; }
        public int totalItemsCount { get; set; }
        public int pageCount => totalItemsCount % pageItemCount == 0 ? totalItemsCount / pageItemCount : (totalItemsCount / pageItemCount) + 1;
        public int rowStart => ((page * pageItemCount) - pageItemCount) + 1;
    }
}
