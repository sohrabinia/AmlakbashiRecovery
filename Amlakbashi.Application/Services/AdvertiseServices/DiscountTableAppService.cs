using Amlakbashi.Core.Common.AppService;
using Amlakbashi.Core.Common.Repository;
using Amlakbashi.Application.Services.AdvertiseServices.Interfaces;
using Amlakbashi.Core.Common.Caching;
using Amlakbashi.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Amlakbashi.Application.Services.AdvertiseServices
{
    internal class DiscountTableAppService : AppServiceBase<DiscountTable, int>, IDiscountTableAppService
    {
        public DiscountTableAppService(IRepository<DiscountTable, int> repository, ICacheManager<DiscountTable> cache) : base(repository, cache)
        {
        }

        public DiscountTable Find(int id)
        {
            return Repository.Query(q => q.FirstOrDefault(f => f.Id == id));
        }

        public bool Insert(long accId, DateTime from, DateTime to, int percent, out List<string> msg)
        {
            var item = new DiscountTable()
            {
                AdvertiseID = accId,
                From = from,
                To = to,
                Percent = percent
            };
            if (item.Validate(out msg))
            {
                Repository.Insert(item);
                Repository.Save();
                return true;
            }
            return false;
        }

        public IList<DiscountTable> GetDiscountsOfAccommodation(long accId)
        {
            return Repository.Query(q => q.Where(w => w.AdvertiseID == accId)).ToList();
        }

        public void Delete(int id)
        {
            Repository.Delete(id);
            Repository.Save();
        }

        public bool Update(long accId, IEnumerable<DiscountTable> items, out List<string> msg)
        {
            var accDiscounts = Repository.Query(q => q.Where(w => w.AdvertiseID == accId));
            msg = new List<string>();
            var discount_ids = items.Select(x => x.Id).ToList();
            Repository.Delete(q => q.AdvertiseID == accId && !discount_ids.Contains(q.Id));
            foreach (var discount in items)
            {
                if (discount.Id > 0)
                {
                    var existedItem = accDiscounts.FirstOrDefault(
                        x => x.Id == discount.Id);
                    existedItem.From = discount.From;
                    existedItem.To = discount.To;
                    existedItem.Percent = discount.Percent;

                    List<string> errorMsgs;
                    if (existedItem.Validate(out errorMsgs))
                    {
                        Repository.Update(existedItem);
                    }
                    else
                    {
                        msg.AddRange(errorMsgs);
                    }
                }
                else
                {
                    var newItem = new DiscountTable()
                    {
                        From = discount.From,
                        To = discount.To,
                        Percent = discount.Percent,
                        AdvertiseID = accId
                    };
                    List<string> errorMsgs;
                    if (newItem.Validate(out errorMsgs))
                    {
                        Repository.Insert(newItem);
                    }
                    else
                    {
                        msg.AddRange(errorMsgs);
                    }
                }
            }
            if (msg.Any())
            {
                return false;
            }
            Repository.Save();
            return true;
        }

        public IList<Advertise> GetMostDiscountAdvertises(int count)
        {
            var today = DateTime.Now.Date;
            IQueryable<DiscountTable> discounts = Repository.Query(q => q.Include(i => i.Advertise));
            discounts = discounts.Where(x => x.Percent > 4 &&
                x.To > today && x.Advertise.Available &&
                x.Advertise.Status ==
                Advertise.AdvertiseStatus.Published &&
                x.Advertise.HideInCategory == false &&
                x.Advertise.Count < 1);
            discounts = discounts.OrderByDescending(x => x.Percent).ThenBy(x => x.From);
            return discounts.Select(x => x.Advertise).Distinct().Take(count).ToList();
        }
    }
}
