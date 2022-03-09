using System;
using System.Collections.Generic;
using System.Text;

namespace Amlakbashi.Core.DTOs.PaymentDTOs
{
    public class CheckPaymentDTO
    {
        public bool Result { get; set; }
        public string PaymentId { get; set; }
        public string CreatePaymentDate { get; set; }
        public string TransactionReferenceId { get; set; }
        public string TraceNumber { get; set; }
        public string ReferenceNumber { get; set; }
        public string TransactionDate { get; set; }
        public string Amount { get; set; }

        public long ReserveId { get; set; }
        public bool ShowDoReserve { get; set; }
        public bool MustEdit { get; set; }

    }
}
