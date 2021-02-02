using Amlakbashi.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amlakbashi.Core.DTOs.PaymentDTOs
{
    public class PaymentIndexDTO
    {
        public Payment Payment { get; set; }
        public string UserPhoneNumber { get; set; }
    }
}
