using Amlakbashi.Core.Common.Repository;
using Amlakbashi.Core.Entities;
using Amlakbashi.Core.Infrastructure.LocalizationHelpers;
using MediatR;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static Amlakbashi.Core.Entities.Advertise;
using static Amlakbashi.Core.Entities.Region;
using Amlakbashi.Application.Services.SettingServices.SettingManager;
using Amlakbashi.Mediator.Commands.AdvertiseCommands;
using Amlakbashi.Mediator.Events.AdvertiseEvents;
using Microsoft.EntityFrameworkCore;
using Amlakbashi.Mediator.Commands.CategoryCommands;

namespace Amlakbashi.Application.Services.AdvertiseServices.EventHandlers
{
    public class AdvertiseEventHandler :
        INotificationHandler<ChangeAdvertiseStatusEvent>,
        INotificationHandler<ChangeAdvertiseTypeEvent>,
        INotificationHandler<ChangeAdvertisePositionEvent>,
        INotificationHandler<ChangeAdvertiseAddressEvent>,
        INotificationHandler<ChangeAdvertiseRulesEvent>,
        INotificationHandler<ChangeAdvertisePriceEvent>,
        INotificationHandler<ChangeInstantReserveStatusEvent>,
        INotificationHandler<ChangeStayDurationEvent>,
        INotificationHandler<ChangeNorouzPriceEvent>,
        INotificationHandler<ChangeMaxInstantReserveStartEvent>,
        INotificationHandler<CreateAdvertiseBasicEvent>,
        INotificationHandler<CreateAdvertiseGeneralEvent>,
        INotificationHandler<AddHotelChildEvent>,
        INotificationHandler<AddComplexChildEvent>
    {
        private readonly IMediator mediator;
        private readonly IRepository<Advertise, long> advertiseRepository;
        private readonly ISettingManager setting;
        public AdvertiseEventHandler(
            IMediator mediator,
            IRepository<Advertise, long> advertiseRepository,
            ISettingManager setting)
        {
            this.mediator = mediator;
            this.advertiseRepository = advertiseRepository;
            this.setting = setting;
        }

        public Task Handle(ChangeAdvertiseStatusEvent notification, CancellationToken cancellationToken)
        {
            //var acc = advertiseRepository.Query(q => q.FirstOrDefault(f => f.Id == notification.advertiseId));
            var acc = advertiseRepository.Find(notification.advertiseId);
            if (acc.Mode == AdvertiseMode.Parent && acc.Childs != null)
            {
                foreach (var item in acc.Childs)
                {
                    item.Status = acc.Status;
                }
            }
            advertiseRepository.Update(acc);
            advertiseRepository.Save();
            return Task.CompletedTask;
        }

        public Task Handle(ChangeAdvertiseTypeEvent notification, CancellationToken cancellationToken)
        {
            //var acc = advertiseRepository.Query(q => q.FirstOrDefault(f => f.Id == notification.advertiseId));
            var acc = advertiseRepository.Find(notification.advertiseId);
            switch (acc.TypeID)
            {
                case AdvertiseType.Hotel:
                case AdvertiseType.Camp:
                case AdvertiseType.TourismAccommodation:
                case AdvertiseType.Inn:
                case AdvertiseType.Pansion:
                    if (acc.Childs != null)
                    {
                        foreach (var item in acc.Childs)
                        {
                            item.TypeID = acc.TypeID;
                        }
                    }
                    advertiseRepository.Update(acc);
                    advertiseRepository.Save();
                    break;
            }
            mediator.Send(new UpdateAdvertiseCategoriesCommand(notification.advertiseId));
            return Task.CompletedTask;
        }

        public Task Handle(ChangeAdvertisePositionEvent notification, CancellationToken cancellationToken)
        {
            //var acc = advertiseRepository.Query(q => q.FirstOrDefault(f => f.Id == notification.advertiseId));
            var acc = advertiseRepository.Find(notification.advertiseId);
            foreach (var item in acc.Childs)
            {
                item.Position = acc.Position;
            }
            advertiseRepository.Update(acc);
            advertiseRepository.Save();
            return Task.CompletedTask;
        }

        public Task Handle(ChangeAdvertiseAddressEvent notification, CancellationToken cancellationToken)
        {
            //var acc = advertiseRepository.Query(q => q.FirstOrDefault(f => f.Id == notification.advertiseId));
            var acc = advertiseRepository.Find(notification.advertiseId);
            //update geographical properties
            acc.CountryDirection = (acc.Province == 1029 ||
                acc.Province == 1393 || acc.Province == 1555) ? CountryDirection.North : CountryDirection.Unset;
            acc.LocationString = RegionLocalization.GetLocationString(acc.RegionProvince.PersianName,
                acc.RegionCity.PersianName, acc.RegionArea == null ? "" : acc.RegionArea.PersianName, Region.GetCountryDirectionString((CountryDirection)acc.CountryDirection));
            foreach (var child in acc.Childs)
            {
                child.Address = acc.Address;
                child.CountryDirection = acc.CountryDirection;
                child.Province = acc.Province;
                child.City = acc.City;
                child.Area = acc.Area;
                child.LocationString = acc.LocationString;
                child.Latitude = acc.Latitude;
                child.Longitude = acc.Longitude;
            }
            advertiseRepository.Update(acc);
            advertiseRepository.Save();
            return Task.CompletedTask;
        }

        public Task Handle(ChangeAdvertiseRulesEvent notification, CancellationToken cancellationToken)
        {
            //var acc = advertiseRepository.Query(q => q.FirstOrDefault(f => f.Id == notification.advertiseId));
            var acc = advertiseRepository.Find(notification.advertiseId);
            foreach (var child in acc.Childs)
            {
                child.AllowParty = acc.AllowParty;
                child.AllowPets = acc.AllowPets;
                child.AllowSmoking = acc.AllowSmoking;
                child.EvidenceRequired = acc.EvidenceRequired;
                child.OtherRules = acc.OtherRules;
            }
            advertiseRepository.Update(acc);
            advertiseRepository.Save();
            return Task.CompletedTask;
        }

        public Task Handle(ChangeAdvertisePriceEvent notification, CancellationToken cancellationToken)
        {
            //var acc = advertiseRepository.Query(q => q.FirstOrDefault(f => f.Id == notification.advertiseId));
            var acc = advertiseRepository.Find(notification.advertiseId);
            if (acc.Mode != AdvertiseMode.Parent && acc.BasePrice != acc.DailyPrice)
            {
                acc.BasePrice = acc.DailyPrice;
                advertiseRepository.Update(acc);
                if (acc.Mode == AdvertiseMode.Child)
                {
                    var parent = acc.Parent;
                    parent.BasePrice = parent.Childs.Min(m => m.DailyPrice);
                    advertiseRepository.Update(parent);
                }
                advertiseRepository.Save();
                if (acc.Categories != null && acc.Categories.Any())
                {
                    foreach (var cat in acc.Categories)
                    {
                        mediator.Send(new UpdateCategoryPriceCommand(cat.Id));
                    }
                }
                mediator.Send(new RemoveCategoryItemCacheCommand(acc.Id));
                if (acc.Mode == AdvertiseMode.Child)
                {
                    mediator.Send(new RemoveCategoryItemCacheCommand((long)acc.ParentId));
                }
            }
            return Task.CompletedTask;
        }

        public Task Handle(ChangeInstantReserveStatusEvent notification, CancellationToken cancellationToken)
        {
            //var acc = advertiseRepository.Query(q => q.FirstOrDefault(f => f.Id == notification.advertiseId));
            var acc = advertiseRepository.Find(notification.advertiseId);
            foreach (var item in acc.Childs)
            {
                item.InstantReserveStatus = acc.InstantReserveStatus;
            }
            advertiseRepository.Update(acc);
            advertiseRepository.Save();
            mediator.Send(new RemoveAdvertiseCacheCommand(acc.Id));
            return Task.CompletedTask;
        }

        public Task Handle(ChangeStayDurationEvent notification, CancellationToken cancellationToken)
        {
            //var acc = advertiseRepository.Query(q => q.FirstOrDefault(f => f.Id == notification.advertiseId));
            var acc = advertiseRepository.Find(notification.advertiseId);
            foreach (var item in acc.Childs)
            {
                item.MinReserveDays = acc.MinReserveDays;
                item.MaxReserveDays = acc.MaxReserveDays;
            }
            advertiseRepository.Update(acc);
            advertiseRepository.Save();
            return Task.CompletedTask;
        }

        public Task Handle(ChangeNorouzPriceEvent notification, CancellationToken cancellationToken)
        {
            //var acc = advertiseRepository.Query(q => q.FirstOrDefault(f => f.Id == notification.advertiseId));
            var acc = advertiseRepository.Find(notification.advertiseId);
            foreach (var item in acc.Childs)
            {
                item.NorouzPrice = acc.NorouzPrice;
                item.NorouzOverCapacityPrice = acc.NorouzOverCapacityPrice;
            }
            advertiseRepository.Update(acc);
            advertiseRepository.Save();
            return Task.CompletedTask;
        }

        public Task Handle(ChangeMaxInstantReserveStartEvent notification, CancellationToken cancellationToken)
        {
            //var acc = advertiseRepository.Query(q => q.FirstOrDefault(f => f.Id == notification.advertiseId));
            var acc = advertiseRepository.Find(notification.advertiseId);
            foreach (var item in acc.Childs)
            {
                item.MaxInstantReserveStart = acc.MaxInstantReserveStart;
            }
            advertiseRepository.Update(acc);
            advertiseRepository.Save();
            return Task.CompletedTask;
        }

        public Task Handle(CreateAdvertiseBasicEvent notification, CancellationToken cancellationToken)
        {
            //var acc = advertiseRepository.Query(q => q.FirstOrDefault(f => f.Id == notification.advertiseId));
            var acc = advertiseRepository.Find(notification.advertiseId);
            acc.AmlakbashiScore = 1000;

            var max_score = setting.MaxScore;
            long max_value = 10000;
            if (max_score > 0)
            {
                acc.AdvertiseScore = max_score + Convert.ToInt64(max_value / 5);
            }
            else
            {
                acc.AdvertiseScore = 12000;
            }

            advertiseRepository.Update(acc);
            advertiseRepository.Save();
            return Task.CompletedTask;
        }

        public Task Handle(CreateAdvertiseGeneralEvent notification, CancellationToken cancellationToken)
        {
            var acc = advertiseRepository.Query(q => q.Include(i => i.RegionCity)
                .Include(i => i.RegionProvince).FirstOrDefault(f => f.Id == notification.advertiseId));
            var cityTitle = acc.RegionCity.PersianName;
            var areaTitle = acc.Area == null ? null : acc.RegionArea.PersianName;

            if (acc.Status == AdvertiseStatus.NotCompleted || acc.Status == AdvertiseStatus.FirstReady)
            {
                acc.OldSlug = AdvertiseUrlLocalization.GetOldSlug(acc.Title, (int)acc.TypeID);
                if (notification.IsAdmin)
                {
                    acc.Slug = string.IsNullOrEmpty(acc.Slug) ? acc.Id.ToString() + "-" + acc.OldSlug : acc.Slug;
                    acc.MetaTitle = string.IsNullOrEmpty(acc.MetaTitle) ? acc.Title + " | املاک باشی" : acc.MetaTitle;
                    acc.MetaDescription = string.IsNullOrEmpty(acc.MetaDescription) ?
                        AdvertiseSeoLocalization.GetMetaDescription(acc, cityTitle, areaTitle) : acc.MetaDescription;
                }
                else
                {
                    acc.Slug = acc.Id.ToString() + "-" + acc.OldSlug;
                    acc.MetaTitle = acc.Title + " | املاک باشی";
                    acc.MetaDescription = AdvertiseSeoLocalization.GetMetaDescription(acc, cityTitle, areaTitle);
                }
            }

            //initialize geographical properties
            acc.CountryDirection = (acc.Province == 1029 ||
                acc.Province == 1393 || acc.Province == 1555) ? CountryDirection.North : CountryDirection.Unset;
            acc.LocationString = RegionLocalization.GetLocationString(acc.RegionProvince.PersianName, acc.RegionCity.PersianName,
                acc.RegionArea == null ? "" : acc.RegionArea.PersianName, Region.GetCountryDirectionString((CountryDirection)acc.CountryDirection));

            //initialize acc type
            acc.ParentAccType = (AdvertiseType)AdvertiseTypeToHeadType((int)acc.TypeID);

            advertiseRepository.Update(acc);
            advertiseRepository.Save();
            return Task.CompletedTask;
        }

        public Task Handle(AddHotelChildEvent notification, CancellationToken cancellationToken)
        {
            //var parent = advertiseRepository.Query(q => q.FirstOrDefault(f => f.Id == notification.parentId));
            var parent = advertiseRepository.Find(notification.parentId);
            //var child = advertiseRepository.Query(q => q.FirstOrDefault(f => f.Id == notification.childId));
            var child = advertiseRepository.Find(notification.childId);

            child.AmlakbashiScore = parent.AmlakbashiScore;
            child.AdvertiseScore = parent.AdvertiseScore;
            child.Address = parent.Address;
            child.CountryDirection = parent.CountryDirection;
            child.Province = parent.Province;
            child.City = parent.City;
            child.Area = parent.Area;
            child.LocationString = parent.LocationString;
            child.Latitude = parent.Latitude;
            child.Longitude = parent.Longitude;
            child.TypeID = parent.TypeID;
            child.ParentAccType = parent.ParentAccType;
            child.UserID = parent.UserID;
            child.OwnerFullName = parent.OwnerFullName;
            child.OwnerMobile = parent.OwnerMobile;
            child.OwnerID = parent.OwnerID;
            child.OwnershipType = parent.OwnershipType;
            child.Status = parent.Status;
            child.Position = parent.Position;
            child.AllowParty = parent.AllowParty;
            child.AllowPets = parent.AllowPets;
            child.AllowSmoking = parent.AllowSmoking;
            child.EvidenceRequired = parent.EvidenceRequired;
            child.OtherRules = parent.OtherRules;
            child.Mode = AdvertiseMode.Child;
            child.Available = true;
            child.BasePrice = child.DailyPrice;
            parent.BasePrice = parent.BasePrice < 1 ? child.BasePrice :
                Math.Min(parent.BasePrice, child.BasePrice);
            parent.NorouzPrice = parent.NorouzPrice < 1 ? child.NorouzPrice :
                Math.Min(parent.NorouzPrice, child.NorouzPrice);

            advertiseRepository.Update(child);
            advertiseRepository.Save();
            return Task.CompletedTask;
        }

        public Task Handle(AddComplexChildEvent notification, CancellationToken cancellationToken)
        {
            var parent = advertiseRepository.Find(notification.parentId);
            var child = advertiseRepository.Find(notification.childId);

            child.AmlakbashiScore = parent.AmlakbashiScore;
            child.AdvertiseScore = parent.AdvertiseScore;
            child.Address = parent.Address;
            child.CountryDirection = parent.CountryDirection;
            child.RegionProvince = parent.RegionProvince;
            child.RegionCity = parent.RegionCity;
            child.RegionArea = parent.RegionArea;
            child.LocationString = parent.LocationString;
            child.Latitude = parent.Latitude;
            child.Longitude = parent.Longitude;
            child.ParentAccType = (AdvertiseType)Advertise.AdvertiseTypeToHeadType((int)child.TypeID);
            child.User = parent.User;
            child.OwnerFullName = parent.OwnerFullName;
            child.OwnerMobile = parent.OwnerMobile;
            child.OwnerID = parent.OwnerID;
            child.OwnershipType = parent.OwnershipType;
            child.Status = parent.Status;
            child.Position = parent.Position;
            child.AllowParty = parent.AllowParty;
            child.AllowPets = parent.AllowPets;
            child.AllowSmoking = parent.AllowSmoking;
            child.EvidenceRequired = parent.EvidenceRequired;
            child.OtherRules = parent.OtherRules;
            child.Mode = Advertise.AdvertiseMode.Child;
            child.Available = true;
            child.BasePrice = child.DailyPrice;
            parent.BasePrice = parent.BasePrice < 1 ? child.BasePrice :
                Math.Min(parent.BasePrice, child.BasePrice);
            parent.NorouzPrice = parent.NorouzPrice < 1 ? child.NorouzPrice :
                Math.Min(parent.NorouzPrice, child.NorouzPrice);
            var cityTitle = child.RegionCity.PersianName;
            var areaTitle = child.RegionArea != null ? child.RegionArea.PersianName : null;
            child.OldSlug = AdvertiseUrlLocalization.GetOldSlug(child.Title, (int)child.TypeID);
            child.Slug = child.Id.ToString() + "-" + child.OldSlug;
            child.MetaTitle = child.Title + " | املاک باشی";
            child.MetaDescription = AdvertiseSeoLocalization.GetMetaDescription(child, cityTitle, areaTitle);

            advertiseRepository.Update(child);
            advertiseRepository.Save();
            return Task.CompletedTask;
        }
    }
}
