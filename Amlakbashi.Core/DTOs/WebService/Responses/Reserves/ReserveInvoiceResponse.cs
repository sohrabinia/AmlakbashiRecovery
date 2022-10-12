using Amlakbashi.Core.Common.Utilities;
using Amlakbashi.Core.Entities;
using Amlakbashi.Core.Infrastructure.LocalizationHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amlakbashi.Core.DTOs.WebService.Responses.Reserves
{
    public class ReserveInvoiceResponse
    {
        public long reserveId { get; set; }
        public List<ReserveInvoiceServiceResponse> services { get; set; }
        public long totalServicePrice { get; set; }
        public long finalPrice { get; set; }
        public long payablePrice { get; set; }

        public static implicit operator ReserveInvoiceResponse(Reserve reserve)
        {
            var response = new ReserveInvoiceResponse()
            {
                reserveId = reserve.Id,
                totalServicePrice = reserve.TotalPrice,
                finalPrice = reserve.TotalPayablePrice,
                payablePrice = reserve.TotalPayablePrice,
                services = new List<ReserveInvoiceServiceResponse>()
                {
                    new ReserveInvoiceServiceResponse()
                    {
                        service = "رزرو اقامتگاه",
                        count = 1,
                        unitPrice = 0,
                        totalPrice = reserve.TotalPrice
                    }
                }
            };
            return response;
        }
    }

    public class ReserveInvoiceServiceResponse
    {
        public string service { get; set; }
        public int count { get; set; }
        public int unitPrice { get; set; }
        public long totalPrice { get; set; }
    }
}
