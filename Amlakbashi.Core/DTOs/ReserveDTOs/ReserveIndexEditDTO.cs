using Amlakbashi.Core.Common.Utilities;
using Amlakbashi.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amlakbashi.Core.DTOs.ReserveDTOs
{
    public class ReserveIndexEditDTO
    {
        public long Id { get; set; }
        public string PersinaStartDate { get; set; }
        public string PersinaEndDate { get; set; }
        public int GuestCount { get; set; }
        public long TotalPrice { get; set; }
        public long DepositePrice { get; set; }
        public Reserve.HostResponseEnum HostResponse { get; set; }
        public Reserve.ReserveStatus Status { get; set; }
        public string CancelReason { get; set; }

        public static implicit operator ReserveIndexEditDTO(Reserve reserve)
        {
            var dto = new ReserveIndexEditDTO()
            {
                Id = reserve.Id,
                TotalPrice = reserve.TotalPrice,
                DepositePrice = reserve.DepositPrice,
                CancelReason = reserve.CancelReason,
                GuestCount = reserve.NumberOfGuests,
                HostResponse = reserve.HostResponse,
                Status = reserve.Status,
                PersinaStartDate = DateTimeUtility.GregorianToPersianDate(reserve.StartDate),
                PersinaEndDate = DateTimeUtility.GregorianToPersianDate(reserve.EndDate)
            };
            return dto;
        }
    }
}
