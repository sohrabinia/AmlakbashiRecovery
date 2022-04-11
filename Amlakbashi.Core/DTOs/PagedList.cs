using System;
using System.Collections.Generic;
using System.Text;

namespace Amlakbashi.Core.DTOs
{
    public class PagedList<T>
    {
        public IList<T> List { get; set; }
        public PagingInfo PagingInfo { get; set; }
    }
}
