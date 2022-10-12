using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Amlakbashi.Core.DTOs.WebService.Requests.Reserves
{
    public class ReservePostDiscountCodeRequest
    {
        [Required]
        public long reserveId { get; set; }

        [Required]
        public string discountCode { get; set; }
    }
}
