using Amlakbashi.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using static Amlakbashi.Core.Entities.Advertise;

namespace Amlakbashi.Core.Infrastructure.FilterHelpers.Interfaces
{
    public interface IAdvertiseFilterHelper
    {
        IQueryable<Advertise> FilterPhrase(IQueryable<Advertise> input, string phrase);
        IQueryable<Advertise> FilterParking(IQueryable<Advertise> input, string parking, bool hasParking);
        IQueryable<Advertise> FilterRoom(IQueryable<Advertise> input, string room, List<int> roomList);
        IQueryable<Advertise> FilterPrice(IQueryable<Advertise> input, priceRangeTypes priceRangeType, int frompaypernight, int topaypernight);
        IQueryable<Advertise> FilterEmptyInRange(IQueryable<Advertise> input,
            List<DateTime> range);
    }
}
