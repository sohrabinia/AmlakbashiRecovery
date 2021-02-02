using Amlakbashi.Core.Common.Utilities;
using Amlakbashi.Core.Entities;
using Amlakbashi.Core.Infrastructure.AdvertiseBuilder;
using Amlakbashi.Core.Infrastructure.AdvertiseBuilder.Parts;
using Amlakbashi.Core.Infrastructure.LocalizationHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using static Amlakbashi.Core.Entities.Advertise;

namespace Amlakbashi.Core.DTOs.AccommodationDTOs.AccPagesDTOs
{
    public class AccommodationItemDTO
    {
        public AccommodationItemDTO(AdvertiseMode advertiseMode)
        {
            AdvertiseMode = advertiseMode;
            this.AccCategory = this.AccCategory == null ? new AccommodationCategoryDTO() : this.AccCategory;
            this.Statistics = this.Statistics == null ? new AccommodationStatisticsDTO() : this.Statistics;
            this.ReportItems = this.ReportItems == null ? new AccommodationReportItemDTO() : this.ReportItems;
            this.AdvertiseType = this.AdvertiseType == null ? new AdvertiseTypeDTO() : this.AdvertiseType;
            this.Address = this.Address == null ? new AddressDTO() : this.Address;
            this.Amenities = this.Amenities == null ? new AmenitiesDTO() : this.Amenities;
            this.Bed = this.Bed == null ? new BedDTO() : this.Bed;
            this.BuildingSize = this.BuildingSize == null ? new BuildingSizeDTO() : this.BuildingSize;
            this.Capacity = this.Capacity == null ? new CapacityDTO() : this.Capacity;
            this.Elevator = this.Elevator == null ? new ElevatorDTO() : this.Elevator;
            this.Floor = this.Floor == null ? new FloorDTO() : this.Floor;
            this.HotelUnitSpecific = this.HotelUnitSpecific == null ? new HotelUnitSpecificDTO() : this.HotelUnitSpecific;
            this.LandArea = this.LandArea == null ? new LandAreaDTO() : this.LandArea;
            this.Norouz = this.Norouz == null ? new NorouzDTO() : this.Norouz;
            this.Ownership = this.Ownership == null ? new OwnershipDTO() : this.Ownership;
            this.Parking = this.Parking == null ? new ParkingDTO() : this.Parking;
            this.Photo = this.Photo == null ? new PhotoDTO() : this.Photo;
            this.Position = this.Position == null ? new PositionDTO() : this.Position;
            this.Price = this.Price == null ? new PriceDTO() : this.Price;
            this.Reserve = this.Reserve == null ? new ReserveDTO() : this.Reserve;
            this.Rules = this.Rules == null ? new RulesDTO() : this.Rules;
            this.Room = this.Room == null ? new RoomDTO() : this.Room;
            this.TitleDesc = this.TitleDesc == null ? new TitleDescDTO() : this.TitleDesc;
            this.MetaTitleDesc = this.MetaTitleDesc == null ? new MetaTitleDescDTO() : this.MetaTitleDesc;
        }

        public static implicit operator AccommodationItemDTO(AdvertiseDirector director)
        {
            var dto = new AccommodationItemDTO(director.Mode);

            var amenities = director.GetAdvertisePart<AmenitiesPart>();
            PropertyCopier<AmenitiesPart, AmenitiesDTO>.CopyWithoutCheckType(amenities == null ? new AmenitiesPart() : amenities,
                dto.Amenities);

            var bed = director.GetAdvertisePart<BedPart>();
            PropertyCopier<BedPart, BedDTO>.Copy(bed == null ? new BedPart() : bed, dto.Bed);

            var buildingSize = director.GetAdvertisePart<BuildingSizePart>();
            PropertyCopier<BuildingSizePart, BuildingSizeDTO>.Copy(buildingSize == null ? new BuildingSizePart() : buildingSize,
                dto.BuildingSize);

            var capacity = director.GetAdvertisePart<CapacityPart>();
            PropertyCopier<CapacityPart, CapacityDTO>.Copy(capacity == null ? new CapacityPart() : capacity,
                dto.Capacity);

            var elevator = director.GetAdvertisePart<ElevatorPart>();
            PropertyCopier<ElevatorPart, ElevatorDTO>.CopyWithoutCheckType(elevator == null ? new ElevatorPart() : elevator, dto.Elevator);

            var floor = director.GetAdvertisePart<FloorPart>();
            PropertyCopier<FloorPart, FloorDTO>.Copy(floor == null ? new FloorPart() : floor, dto.Floor);

            var hotelUnitSpecific = director.GetAdvertisePart<HotelUnitSpecificPart>();
            PropertyCopier<HotelUnitSpecificPart, HotelUnitSpecificDTO>.Copy(
                hotelUnitSpecific == null ? new HotelUnitSpecificPart() : hotelUnitSpecific, dto.HotelUnitSpecific);

            var landArea = director.GetAdvertisePart<LandAreaPart>();
            PropertyCopier<LandAreaPart, LandAreaDTO>.Copy(landArea == null ? new LandAreaPart() : landArea,
                dto.LandArea);

            var ownership = director.GetAdvertisePart<OwnershipPart>();
            PropertyCopier<OwnershipPart, OwnershipDTO>.Copy(ownership == null ? new OwnershipPart() : ownership,
                dto.Ownership);

            var parking = director.GetAdvertisePart<ParkingPart>();
            PropertyCopier<ParkingPart, ParkingDTO>.Copy(parking == null ? new ParkingPart() : parking, dto.Parking);

            var photo = director.GetAdvertisePart<PhotoPart>();
            PropertyCopier<PhotoPart, PhotoDTO>.Copy(photo == null ? new PhotoPart() : photo, dto.Photo);

            var price = director.GetAdvertisePart<PricePart>();
            PropertyCopier<PricePart, PriceDTO>.Copy(price == null ? new PricePart() : price, dto.Price);

            var reserve = director.GetAdvertisePart<ReservePart>();
            PropertyCopier<ReservePart, ReserveDTO>.Copy(reserve == null ? new ReservePart() : reserve, dto.Reserve);

            var room = director.GetAdvertisePart<RoomPart>();
            PropertyCopier<RoomPart, RoomDTO>.Copy(room == null ? new RoomPart() : room, dto.Room);

            PropertyCopier<NorouzPart, NorouzDTO>.Copy(director.GetAdvertisePart<NorouzPart>(), dto.Norouz);
            PropertyCopier<PositionPart, PositionDTO>.Copy(director.GetAdvertisePart<PositionPart>(), dto.Position);
            PropertyCopier<AdvertiseTypePart, AdvertiseTypeDTO>.Copy(director.GetAdvertisePart<AdvertiseTypePart>(), dto.AdvertiseType);
            PropertyCopier<AddressPart, AddressDTO>.Copy(director.GetAdvertisePart<AddressPart>(), dto.Address);
            PropertyCopier<RulesPart, RulesDTO>.Copy(director.GetAdvertisePart<RulesPart>(), dto.Rules);
            PropertyCopier<TitleDescPart, TitleDescDTO>.Copy(director.GetAdvertisePart<TitleDescPart>(), dto.TitleDesc);
            PropertyCopier<MetaTitleDescPart, MetaTitleDescDTO>.Copy(director.GetAdvertisePart<MetaTitleDescPart>(), dto.MetaTitleDesc);
            return dto;
        }


        public static AccommodationItemDTO Generate(User currentUser,
            Advertise advertise, AdvertiseDirector director,
            Dictionary<AdvertiseType, IList<AdvertiseDirector>> childDirectors,
            IList<ReportItem> allUserReportItems)
        {
            AccommodationItemDTO dto = director;
            var reportItems = advertise.UserRatingDict;
            var comments = advertise.PublishedComments;
            var noTextComments = new List<Comment>();
            if (reportItems.Count > comments.Count())
            {
                var reportItemsWithoutComment = reportItems.Where(
                    x => !comments.Any(y => y.SenderUserID == x.Key));
                foreach (var rp in reportItemsWithoutComment)
                {
                    var senderUser = rp.Value.FirstOrDefault().User;
                    noTextComments.Add(new Comment()
                    {
                        SenderUser = senderUser,
                        SenderUserID = senderUser.Id,
                        CreateDate = rp.Value.Min(x => x.CreateDate),
                        LastModifyDate = rp.Value.Max(x => x.LastModifyDate),
                        Text = ""
                    });
                }
                comments = comments.OrderByDescending(x => x.CreateDate);
                noTextComments = noTextComments.OrderByDescending(x => x.CreateDate).ToList();
            }
            List<AccommodationCommentDTO> commentsList = new List<AccommodationCommentDTO>();
            foreach (var item in comments)
            {
                commentsList.Add(new AccommodationCommentDTO()
                {
                    Comment = item,
                    User = item.SenderUser,
                    ReplyComment = item.HostReply,
                    ReportItems = reportItems.FirstOrDefault(x => x.Value[0].UserID == item.SenderUserID).Value
                });
            }
            foreach (var item in noTextComments)
            {
                commentsList.Add(new AccommodationCommentDTO()
                {
                    Comment = item,
                    User = item.SenderUser,
                    ReplyComment = item.HostReply,
                    ReportItems = reportItems.FirstOrDefault(x => x.Value[0].UserID == item.SenderUserID).Value
                });
            }

            long scReserveId = 0;
            var suspendeComment = advertise.GetSuspendedComment(currentUser.Id);
            if (suspendeComment != null)
            {
                scReserveId = advertise.Reserves.FirstOrDefault(f => f.UserID == currentUser.Id).Id;
            }

            var hotelChildren = childDirectors.ContainsKey(Advertise.AdvertiseType.Hotel) ? childDirectors[Advertise.AdvertiseType.Hotel] : new List<AdvertiseDirector>();
            var apartmentChildren = childDirectors.ContainsKey(Advertise.AdvertiseType.Apartment) ? childDirectors[Advertise.AdvertiseType.Apartment] : new List<AdvertiseDirector>();
            var suitChildren = childDirectors.ContainsKey(Advertise.AdvertiseType.SuitAndRoom) ? childDirectors[Advertise.AdvertiseType.SuitAndRoom] : new List<AdvertiseDirector>();
            var villaChildren = childDirectors.ContainsKey(Advertise.AdvertiseType.Villa) ? childDirectors[Advertise.AdvertiseType.Villa] : new List<AdvertiseDirector>();
            var houseChildren = childDirectors.ContainsKey(Advertise.AdvertiseType.House) ? childDirectors[Advertise.AdvertiseType.House] : new List<AdvertiseDirector>();
            var hutChildren = childDirectors.ContainsKey(Advertise.AdvertiseType.Hut) ? childDirectors[Advertise.AdvertiseType.Hut] : new List<AdvertiseDirector>();
            dto.Id = advertise.Id;
            dto.Slug = advertise.Slug;
            dto.Comments = commentsList;
            dto.SuspendedComment = suspendeComment;
            dto.SuspendedCommentReserveId = scReserveId;
            var accUser = advertise.User;
            dto.AccUser = accUser != null ? advertise.User : new User() { FName = "", LName = "" };
            dto.TypeUrlString = AdvertiseUrlLocalization.GetAdvertiseTypeUrlString((AdvertiseType)advertise.TypeID);
            dto.TypeUserString = AdvertiseMainLocalization.GetAdvertiseTypeUserString((AdvertiseType)advertise.TypeID);
            dto.VillaChildren = villaChildren.Select(s => (AccommodationVillaItemDTO)s).ToList();
            dto.ApartmentChildren = apartmentChildren.Select(s => (AccommodationApartmentItemDTO)s).ToList();
            dto.SuitChildren = suitChildren.Select(s => (AccommodationSuitItemDTO)s).ToList();
            dto.HouseChildren = houseChildren.Select(s => (AccommodationHouseItemDTO)s).ToList();
            dto.HutChildren = hutChildren.Select(s => (AccommodationHutItemDTO)s).ToList();
            dto.HotelChildren = hotelChildren.Select(s => (AccommodationHotelItemDTO)s).ToList();

            //DynamicCategory countryDirectionCat, provinceCat, cityCat, areaCat;
            //string countryDirectionName, provinceName, cityName, areaName;
            //advertise.GetRelatedCategories(out countryDirectionCat, out provinceCat,
            //    out cityCat, out areaCat, out countryDirectionName, out provinceName, out cityName, out areaName);
            dto.AccCategory.CountryDirection = advertise.CountryDirection != Region.CountryDirection.Unset;
            dto.AccCategory.Province = advertise.Province != null;
            dto.AccCategory.City = advertise.City != null;
            var categories = advertise.Categories;
            if (categories != null && categories.Any())
            {
                var cityCat = advertise.Categories.FirstOrDefault(f => f.Area == null && f.City != null);
                dto.AccCategory.CityMostAccType = cityCat?.MostAccType;
                dto.AccCategory.CityCountAdvertise = cityCat?.CountAdvertise;
            }
            dto.AccCategory.Area = advertise.Area == null ? false : true;
            dto.AccCategory.CountryDirectionName = advertise.CountryDirection == Region.CountryDirection.Unset ? "" : Region.GetCountryDirectionString(advertise.CountryDirection);
            dto.AccCategory.ProvinceName = advertise.Province == null ? "" : advertise.RegionProvince.PersianName;
            dto.AccCategory.CityName = advertise.City == null ? "" : advertise.RegionCity.PersianName;
            dto.AccCategory.AreaName = advertise.Area == null ? "" : advertise.RegionArea.PersianName;
            dto.AccCategory.CountryDirectionUrl = advertise.CountryDirection == Region.CountryDirection.Unset ? "" : CategoryUrlLocalization.RegionToUrl(advertise.CountryDirection);
            dto.AccCategory.ProvinceUrl = advertise.Province == null ? "" : CategoryUrlLocalization.RegionToUrl(advertise.CountryDirection, advertise.RegionProvince);
            dto.AccCategory.CityUrl = advertise.City == null ? "" : CategoryUrlLocalization.RegionToUrl(advertise.CountryDirection, advertise.RegionProvince, advertise.RegionCity);
            dto.AccCategory.AreaUrl = advertise.Area == null ? "" : CategoryUrlLocalization.RegionToUrl(
                advertise.CountryDirection, advertise.RegionProvince, advertise.RegionCity, advertise.RegionArea);

            var userRatingTypes = Enum.GetValues(typeof(Comment.UserRatingType)) as Comment.UserRatingType[];
            dto.Statistics.SuccessfullReserveCount = advertise.SuccessfullReserves.Count();
            dto.Statistics.CountUserRating = reportItems.Count;
            foreach (var item in userRatingTypes)
            {
                var typeReportItems = advertise.ReportItems.Where(w => w.ReportID == (int)item);
                dto.Statistics.OverallRatingDecimal.Add(item, typeReportItems.Any() == false ? 0 : (float)typeReportItems.Average(x => x.Score));
                dto.Statistics.UserRatingTypeString.Add(item, Comment.GetUserRatingTypeString(item));
            }

            Dictionary<User, List<ReportItem>> reports = new Dictionary<User, List<ReportItem>>();
            List<ReportItem> reportItemsList = new List<ReportItem>();
            reportItemsList.AddRange(reportItems.Values.SelectMany(s => s));
            for (int i = 0; i < reportItemsList.Count(); i++)
            {
                var commentDTO = commentsList.FirstOrDefault(w => w.Comment.SenderUserID == reportItemsList[i].UserID);
                var comment = commentDTO != null ? commentDTO.Comment : null;
                var comment_str = comment != null ? comment.Text : "";
                var user_id = reportItemsList[i].UserID;
                commentDTO = commentsList.FirstOrDefault(f => f.User.Id == user_id);
                var user = commentDTO != null ? commentDTO.User : new User();
                if (string.IsNullOrEmpty(comment_str) ||
                    string.IsNullOrEmpty(user.FullName))
                {
                    reportItemsList.RemoveAt(i);
                    i--;
                }
            }
            var user_ids = reportItemsList.Select(x => x.UserID).Distinct();
            foreach (var user_id in user_ids)
            {
                var user = commentsList.First(x => x.User.Id == user_id).User;
                reports.Add(user, reportItemsList.Where(x => x.UserID == user_id).ToList());
            }
            if (reportItems.Count > 0)
            {
                dto.ReportItems.Rating = (int)reportItems.Values.Select(s => s.Average(a => a.Score)).Average();
                dto.ReportItems.FloatRating = (float)reportItems.Values.Select(s => s.Sum(x => x.Score)).Sum() /
                    (float)reportItems.Values.Select(s => s.Count).Sum();
            }
            dto.ReportItems.ReportList = reports;
            dto.ReportItems.CountReport = reports.Count;
            dto.FilteredAddress = AdvertiseMainLocalization.FilteredAddress(advertise.Address);
            dto.CityString = advertise.RegionCity == null ? null : advertise.RegionCity.PersianName;
            dto.HostUserRating = allUserReportItems.Count() > 0 ? (int)allUserReportItems.Average(x => x.Score) : 5;
            dto.CanPublish = advertise.CanPublish();
            dto.DiscountData = advertise.GetFirstDiscountData(false, true);
            dto.ReservePopupData = new ReservePopupDTO(dto.Capacity.Capacity,
                dto.Capacity.MoreThanCapacity, dto.DiscountData);
            return dto;
        }

        public long Id { get; set; }
        public string Slug { get; set; }
        public string TypeUrlString { get; set; }
        public string TypeUserString { get; set; }
        public string CityString { get; set; }
        public string RawUrl { get; set; }
        public string EmptyRangeFrom { get; set; }
        public string EmptyRangeTo { get; set; }
        public bool IsPreview { get; set; }
        public string RelatedLinkCapacity { get; set; }
        public double HostUserRating { get; set; }
        public bool CanPublish { get; set; }
        public AccommodationCategoryDTO AccCategory { get; set; }
        public AdvertiseMode AdvertiseMode { get; set; }
        public AdvertiseTypeDTO AdvertiseType { get; set; }
        public AddressDTO Address { get; set; }
        public AmenitiesDTO Amenities { get; set; }
        public BedDTO Bed { get; set; }
        public BuildingSizeDTO BuildingSize { get; set; }
        public CapacityDTO Capacity { get; set; }
        public ElevatorDTO Elevator { get; set; }
        public FloorDTO Floor { get; set; }
        public HotelUnitSpecificDTO HotelUnitSpecific { get; set; }
        public LandAreaDTO LandArea { get; set; }
        public NorouzDTO Norouz { get; set; }
        public OwnershipDTO Ownership { get; set; }
        public ParkingDTO Parking { get; set; }
        public PhotoDTO Photo { get; set; }
        public PositionDTO Position { get; set; }
        public PriceDTO Price { get; set; }
        public ReserveDTO Reserve { get; set; }
        public RoomDTO Room { get; set; }
        public RulesDTO Rules { get; set; }
        public TitleDescDTO TitleDesc { get; set; }
        public MetaTitleDescDTO MetaTitleDesc { get; set; }
        public List<AccommodationHotelItemDTO> HotelChildren { get; set; }
        public List<AccommodationApartmentItemDTO> ApartmentChildren { get; set; }
        public List<AccommodationSuitItemDTO> SuitChildren { get; set; }
        public List<AccommodationVillaItemDTO> VillaChildren { get; set; }
        public List<AccommodationHouseItemDTO> HouseChildren { get; set; }
        public List<AccommodationHutItemDTO> HutChildren { get; set; }
        public List<AccommodationCommentDTO> Comments { get; set; }
        public Comment SuspendedComment { get; set; }
        public long SuspendedCommentReserveId { get; set; }
        public AccommodationStatisticsDTO Statistics { get; set; }
        public User AccUser { get; set; }
        public AccommodationReportItemDTO ReportItems { get; set; }
        public string FilteredAddress { get; set; }
        public List<DynamicCategory> RelatedCategories { get; set; }
        public DiscountDTO DiscountData { get; set; }
        public ReservePopupDTO ReservePopupData { get; set; }
    }
}
