using Amlakbashi.Core.Common.Repository;
using Amlakbashi.Mediator.Events.AdvertiseEvents;
using Amlakbashi.Core.Entities;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;

namespace Amlakbashi.Application.Services.AdvertiseServices.EventHandlers
{
    internal class RegionEventHandler :
        INotificationHandler<ChangeCategoryEvent>
    {
        private readonly IRepository<Region, int> regionRepository;
        public RegionEventHandler(IRepository<Region, int> regionRepository)
        {
            this.regionRepository = regionRepository;
        }

        public Task Handle(ChangeCategoryEvent notification, CancellationToken cancellationToken)
        {
            var cat = regionRepository.Find<DynamicCategory, int>(notification.categoryId);
            if (cat.Type != Advertise.AdvertiseType.All)
                return Task.CompletedTask;
            Region region = null;
            if (cat.Area != null)
            {
                region = regionRepository.Find((int)cat.Area);
            }
            else if (cat.City != null)
            {
                region = regionRepository.Find((int)cat.City);
            }
            else if (cat.Province != null)
            {
                region = regionRepository.Find((int)cat.Province);
            }
            if(region != null)
            {
                region.CountAdvertise = cat.CountAdvertise;
                regionRepository.Update(region);
                regionRepository.Save();
                if (region.Type == (int)Region.AdvertiseRegion.City
                    && region.Childs.Count > 0)
                {
                    var advertises = regionRepository.Query<Advertise, long>(q =>
                        q.Where(w => w.CityId == region.Id &&
                        w.Status == Advertise.AdvertiseStatus.Published &&
                        w.Active == true &&
                        w.HideInSearch == false &&
                        w.UnitCount < 1));
                    foreach (var item in region.Childs)
                    {
                        item.CountAdvertise = advertises.Count(c => c.AreaId == item.Id);
                    }
                }
            }
            return Task.CompletedTask;
        }
    }
}
