using Amlakbashi.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amlakbashi.Core.DTOs.PaymentDTOs
{
    public class GroupPaymentDetailsDTO
    {
        public IList<GroupPaymentItemDTO> GroupPaymentItems { get; set; }
        public GroupPayment GroupPayment { get; set; }
    }
}
