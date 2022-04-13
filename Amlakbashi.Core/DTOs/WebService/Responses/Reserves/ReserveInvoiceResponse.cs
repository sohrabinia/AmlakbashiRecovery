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
        public ReserveAdvertiseResponse residencyInfo { get; set; }
        public string fromDate { get; set; }
        public string toDate { get; set; }
        public int guestCount { get; set; }
        public string hostName { get; set; }
        public string hostImageUrl { get; set; }

        public static implicit operator ReserveInvoiceResponse(Reserve reserve)
        {
            var response = new ReserveInvoiceResponse()
            {
                reserveId = reserve.Id,
                fromDate = DateTimeUtility.GregorianToPersianDate(reserve.StartDate),
                toDate = DateTimeUtility.GregorianToPersianDate(reserve.EndDate),
                guestCount = reserve.NumberOfGuests,
                hostName = reserve.HostUser.FullName,
                hostImageUrl = reserve.HostUser.PhotoID == null ? "" : $"/عکس-پروفایل_کوچک-{reserve.HostUser.PhotoID}",
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
                },
                residencyInfo = new ReserveAdvertiseResponse()
                {
                    id = reserve.AdvertiseID,
                    title = reserve.Advertise.Title,
                    type = AdvertiseMainLocalization.GetAdvertiseTypeUserString(reserve.Advertise.TypeID),
                    roomCount = reserve.Advertise.Room,
                    provinceName = reserve.Advertise.RegionProvince?.PersianName,
                    cityName = reserve.Advertise.RegionCity?.PersianName,
                    imageUrl = $"/file/accthumbxxxlarge?accid={reserve.AdvertiseID}&fileid={reserve.Advertise.PhotoID}"
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
