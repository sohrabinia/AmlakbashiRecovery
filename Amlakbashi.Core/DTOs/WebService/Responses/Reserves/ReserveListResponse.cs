using System;
using System.Collections.Generic;
using System.Text;

namespace Amlakbashi.Core.DTOs.WebService.Responses.Reserves
{
    public class ReserveListResponse
    {
        public List<ReserveResponse> reserveList { get; set; } = new List<ReserveResponse>();
        public PagingInfo pagingInfo { get; set; }
    }
}
