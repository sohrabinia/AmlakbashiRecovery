using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Amlakbashi.Core.DTOs.WebService.Requests.Reserves
{
    public class ReservePostRequest
    {
        [Range(1, int.MaxValue)]
        public long advertiseId { get; set; }

        [Required]
        public string fromDate { get; set; }

        [Required]
        public string toDate { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int numberOfGuest { get; set; }

        [BindNever]
        public int userId { get; set; }
    }
}
