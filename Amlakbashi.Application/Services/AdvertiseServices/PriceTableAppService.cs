using Amlakbashi.Core.Common.AppService;
using Amlakbashi.Core.Common.Repository;
using Amlakbashi.Application.Services.AdvertiseServices.Interfaces;
using Amlakbashi.Core.Common.Caching;
using Amlakbashi.Core.Common.Utilities;
using Amlakbashi.Core.Entities;
using Amlakbashi.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Amlakbashi.Application.Services.AdvertiseServices
{
    internal class PriceTableAppService : AppServiceBase<PriceTable, int>, IPriceTableAppService
    {
        public PriceTableAppService(IRepository<PriceTable, int> repository, ICacheManager<PriceTable> cache) : base(repository, cache)
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
    }
}
