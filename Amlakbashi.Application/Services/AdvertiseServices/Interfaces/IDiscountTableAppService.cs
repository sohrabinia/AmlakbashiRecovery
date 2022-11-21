using Amlakbashi.Core.Entities;
using System;
using System.Collections.Generic;

namespace Amlakbashi.Application.Services.AdvertiseServices.Interfaces
{
    public interface IDiscountTableAppService
    {
        DiscountTable Find(int id);
        bool Insert(long accId, DateTime from, DateTime to, int percent, out List<string> msg);
        bool Update(long accId, IEnumerable<DiscountTable> items, out List<string> msg);
        IList<DiscountTable> GetDiscountsOfAccommodation(long accId);
        void Delete(int id);
        IList<Advertise> GetMostDiscountAdvertises(int count);
    }
}
