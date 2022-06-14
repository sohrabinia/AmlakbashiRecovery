using Amlakbashi.Core.Common.AppService;
using Amlakbashi.Core.Common.Repository;
using Amlakbashi.Application.Services.AdvertiseServices.Interfaces;
using Amlakbashi.Core.Common.Utilities;
using Amlakbashi.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using Amlakbashi.Application.DTOs;
using Amlakbashi.Core.DTOs.WebService.Requests.Advertises;

namespace Amlakbashi.Application.Services.AdvertiseServices
{
    internal class PriceTableAppService : AppServiceBase<PriceTable, int>, IPriceTableAppService
    {
        public PriceTableAppService(IRepository<PriceTable, int> repository) : base(repository)
        {
        }

        public bool SetAccommodationPriceInDate(long accId, string fromPersianDate, string toPersianDate, int price, out string msg)
        {
            if (price < 30000)
            {
                msg = "حداقل قیمت: 30000 تومان";
                return false;
            }
            var accPrices = Repository.Query(q => q.Where(x => x.AdvertiseID == accId));
            var persian_range = DateTimeUtility.PersianDateRangeToList(fromPersianDate, toPersianDate, true, true);
            persian_range.RemoveAt(persian_range.Count - 1);
            int persian_year, persian_month, persian_day;
            PriceTable item;
            foreach (var persian_date in persian_range)
            {
                var gregorianDate = DateTimeUtility.PersianDateToGregorian(persian_date);
                var d = Array.ConvertAll(persian_date.Split(','), x => int.Parse(x));
                persian_year = d[0];
                persian_month = d[1];
                persian_day = d[2];

                item = accPrices.FirstOrDefault(x => x.Year == persian_year
                    && x.Month == persian_month && x.Day == persian_day);
                if (item != null)
                {
                    item.Price = price;
                    Repository.Update(item);
                }
                else
                {
                    Repository.Insert(new PriceTable()
                    {
                        AdvertiseID = (int)accId,
                        Price = price,
                        Year = persian_year,
                        Month = persian_month,
                        Day = persian_day,
                        UnixDate = DateTimeUtility.DateValueOfJS(gregorianDate)
                    });
                }
            }
            Repository.Save();
            msg = null;
            return true;
        }

        public ServiceResult UpdateAdvertiseManualPrices(AdvertiseUpdatePriceRequest request)
        {
            var serviceResult = new ServiceResult();
            if (request.price < 30000)
            {
                serviceResult.AddError("minimum price is 30000");
                return serviceResult;
            }
            if (DateTimeUtility.IsValidPersianDate(request.fromDate) == false)
            {
                serviceResult.AddError("fromDate is incorrect");
                return serviceResult;
            }
            if (string.IsNullOrEmpty(request.toDate) == false &&
                (DateTimeUtility.IsValidPersianDate(request.toDate) == false ||
                DateTimeUtility.IsStartDateLowerThanEndDate(request.fromDate, request.toDate) == false))
            {
                serviceResult.AddError("toDate is incorrect");
                return serviceResult;
            }

            var advertisePrices = Repository.Query(q => q.Where(x => x.AdvertiseID == request.advertiseId));
            int persianYear, persianMonth, persianDay;
            PriceTable priceTable;
            List<string> persianDateRange = new List<string>();
            if (string.IsNullOrEmpty(request.toDate))
            {
                persianDateRange.Add(request.fromDate);
            }
            else
            {
                persianDateRange = DateTimeUtility.PersianDateRangeToList(request.fromDate, request.toDate, true, true);
            }

            foreach (var persianDate in persianDateRange)
            {
                var gregorianDate = DateTimeUtility.PersianDateToGregorian(persianDate);
                var d = Array.ConvertAll(persianDate.Split(','), x => int.Parse(x));
                persianYear = d[0];
                persianMonth = d[1];
                persianDay = d[2];

                priceTable = advertisePrices.FirstOrDefault(x => x.Year == persianYear
                    && x.Month == persianMonth && x.Day == persianDay);
                if (priceTable != null)
                {
                    priceTable.Price = request.price;
                    Repository.Update(priceTable);
                }
                else
                {
                    Repository.Insert(new PriceTable()
                    {
                        AdvertiseID = request.advertiseId,
                        Price = request.price,
                        Year = persianYear,
                        Month = persianMonth,
                        Day = persianDay,
                        UnixDate = DateTimeUtility.DateValueOfJS(gregorianDate)
                    });
                }
            }
            Repository.Save();
            return serviceResult;
        }
    }
}
