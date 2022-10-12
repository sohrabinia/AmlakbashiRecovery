using Amlakbashi.Core.Entities;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Amlakbashi.Core.DTOs.WebService.Requests.Reserves
{
    public class ReservePostStartRequest
    {
        [Range(1, int.MaxValue)]
        public int reserveId { get; set; }

        [BindNever]
        public int userId { get; set; }

        [BindNever]
        public ActionLog.ActionSourceEnum actionSource { get; set; }
    }
}
