using Amlakbashi.Core.Entities;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Amlakbashi.Core.DTOs.WebService.Requests.Reserves
{
    public class ReservePostCancelRequest
    {
        [Range(1, int.MaxValue)]
        public int reserveId { get; set; }

        [Required]
        public string reason { get; set; }

        [BindNever]
        public int userId { get; set; }

        [BindNever]
        public Entities.User.UserGeneralTypeEnum panel { get; set; }

        [BindNever]
        public ActionLog.ActionSourceEnum actionSource { get; set; }
    }
}
