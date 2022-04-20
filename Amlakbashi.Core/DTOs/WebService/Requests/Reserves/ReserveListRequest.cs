using Amlakbashi.Core.Entities;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amlakbashi.Core.DTOs.WebService.Requests.Reserves
{
    public class ReserveListRequest
    {
        public Reserve.ReserveCategory category { get; set; }
        public int sort { get; set; }
        public int page { get; set; } = 1;
        public int pageItemCount { get; set; } = 20;

        [BindNever]
        public int userId { get; set; } = 0;

        [BindNever]
        public Entities.User.UserGeneralTypeEnum panel { get; set; }
    }
}
