using Amlakbashi.Core.Common.StaticData;
using Amlakbashi.Core.Common.Utilities;
using Amlakbashi.Core.DTOs.AccommodationDTOs.AccPagesDTOs;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace Amlakbashi.Core.Entities
{
    [Table("Residences")]
    public class Advertise : Entity<long>
    {
        #region Properties
        public override long Id { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime LastModifiedDate { get; set; }
        public long? ParentId { get; set; }
        public int UserId { get; set; }
        public AdvertiseStatus Status { get; set; }
        public bool Active { get; set; }
        public bool HideInSearch { get; set; }

        [Column("Type")]
        public AdvertiseType TypeID { get; set; }
        public AdvertiseMode Mode { get; set; }

        // Location *************************************
        public int? ProvinceId { get; set; }
        public int? CityId { get; set; }
        public int? AreaId { get; set; }
        public Region.CountryDirection CountryDirection { get; set; }
        public string Address { get; set; }
        public string RegionsPersianTitle { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public PositionType LocationType { get; set; }

        // Basic Info ****************************
        public string Title { get; set; }
        public string Description { get; set; }
        public string MetaTitle { get; set; }
        public string MetaDescription { get; set; }
        public string Slug { get; set; }
        public string OldSlug { get; set; }
        public int BuildingArea { get; set; }
        public int LandArea { get; set; }
        public FloorItems Floor { get; set; }
        public VillaTypeEnum VillaType { get; set; }
        public int RoomCount { get; set; }
        public int UnitCount { get; set; }
        public int SingleBedCount { get; set; }
        public int DoubleBedCount { get; set; }
        public int BlanketAndMattressCount { get; set; }
        public ExtraBlanketCountItems ExtraBlanketCount { get; set; }
        public int Capacity { get; set; }
        public int ExtraCapacity { get; set; }
        public long? MainPhotoId { get; set; }
        public long? VideoId { get; set; }
        public VideoStatusEnum VideoStatus { get; set; }

        [MaxLength(1000)]
        public string ReasonForNotConfirmingVideo { get; set; }
        public string AlbumPhoto { get; set; }

        // Ownership and Lisence ********************************
        public OwnershipTypeEnum OwnershipType { get; set; }
        public string OwnerPhoneNumber { get; set; }
        public string OwnerFullName { get; set; }
        public bool License { get; set; }
        public string LicenseNumber { get; set; }
        public long? LicenseFileId { get; set; }

        // Rules ************************************
        public bool Party { get; set; }
        public bool Pets { get; set; }
        public bool Smoking { get; set; }
        public string RequiredEvidence { get; set; }
        public string OtherRules { get; set; }

        // Prices ***********************************
        public int DailyPrice { get; set; }
        public int HolidayPrice { get; set; }
        public int PeakHolidayPrice { get; set; }
        public long MonthlyPrice { get; set; }
        public int NowruzPrice { get; set; }
        public int ExtraCapacityPrice { get; set; }
        public int NowruzExtraCapacityPrice { get; set; }
        public int BasePrice { get; set; }

        // Amenities ***********************************
        public bool? Elevator { get; set; }
        public ParkingItems Parking { get; set; }
        public bool? Pool { get; set; }
        public PoolFeaturesEnum PoolFeatures { get; set; }
        public HeatingSystemItems HeatingSystem { get; set; }
        public CoolingSystemItems CoolingSystem { get; set; }
        public WCItems WC { get; set; }
        public EuropeanToiletTypeEnum EuropeanToiletType { get; set; }
        public bool? Sauna { get; set; }
        public bool? Jacuzzi { get; set; }
        public bool? Bathroom { get; set; }
        public bool? Wifi { get; set; }
        public bool? WashingMachine { get; set; }
        public bool? MicrowaveOven { get; set; }
        public bool? SoundSystem { get; set; }
        public bool? Golf { get; set; }
        public bool? PoolTable { get; set; }
        public bool? Foosball { get; set; }
        public bool? Hairdryer { get; set; }
        public bool? TV { get; set; }
        public bool? Oven { get; set; }
        public bool? Refrigerator { get; set; }
        public bool? KitchenHood { get; set; }
        public bool? KitchenUtensils { get; set; }
        public bool? TeaMaker { get; set; }
        public HygieneProtocolStatus? HygieneProtocol { get; set; }
        public bool? Balcony { get; set; }
        public bool? Filming { get; set; }

        // Scores ************************************
        public long ResidenceScore { get; set; }
        public int AmlakbashiScore { get; set; }
        public float AverageUsersScore { get; set; }
        public float CleaningScore { get; set; }

        // Others **********************************
        public string SupportDescription { get; set; }
        public InstantReserveStatusEnum InstantReserveStatus { get; set; }
        public int MaxInstantReserveStartTimeInterval { get; set; } = 30;
        public int MinReserveDuration { get; set; }
        public int MaxReserveDuration { get; set; }
        public long MinReserveDateForNowruz { get; set; }
        public string NotVerifyReasons { get; set; }
        public int View { get; set; }
        public bool EmptyTonight { get; set; }

        [JsonIgnore]
        [ForeignKey(nameof(UserId))]
        public virtual User User { get; set; }

        [JsonIgnore]
        [ForeignKey(nameof(ProvinceId))]
        public virtual Region RegionProvince { get; set; }

        [JsonIgnore]
        [ForeignKey(nameof(CityId))]
        public virtual Region RegionCity { get; set; }

        [JsonIgnore]
        [ForeignKey(nameof(AreaId))]
        public virtual Region RegionArea { get; set; }

        [JsonIgnore]
        [ForeignKey(nameof(MainPhotoId))]
        public virtual File MainPhoto { get; set; }

        [JsonIgnore]
        [ForeignKey(nameof(LicenseFileId))]
        public virtual File LicenseFile { get; set; }

        [JsonIgnore]
        [ForeignKey(nameof(VideoId))]
        public virtual File Video { get; set; }

        [JsonIgnore]
        [ForeignKey(nameof(ParentId))]
        public virtual Advertise Parent { get; set; }

        [JsonIgnore]
        public virtual ICollection<Advertise> Childs { get; set; }

        [JsonIgnore]
        public virtual ICollection<PriceTable> PriceTables { get; set; }

        [JsonIgnore]
        public virtual ICollection<OccupiedTable> OccupiedTables { get; set; }

        [JsonIgnore]
        public virtual ICollection<DiscountTable> DiscountTables { get; set; }

        [JsonIgnore]
        public virtual ICollection<DynamicCategory> Categories { get; set; }

        [JsonIgnore]
        public virtual ICollection<Comment> Comments { get; set; }

        [JsonIgnore]
        public virtual ICollection<Reserve> Reserves { get; set; }

        [JsonIgnore]
        public virtual ICollection<ExtrinsicReserve> ExtrinsicReserves { get; set; }

        [JsonIgnore]
        public virtual ICollection<ReportItem> ReportItems { get; set; }

        [JsonIgnore]
        public virtual ICollection<AdvertiseReport> AdvertiseReports { get; set; }

        [JsonIgnore]
        public virtual ICollection<Cart> Carts { get; set; }

        [JsonIgnore]
        public virtual ICollection<File> Photos { get; set; }

        [JsonIgnore]
        public virtual ICollection<InstantReserveDate> InstantReserveDates { get; set; } = new List<InstantReserveDate>();

        [JsonIgnore]
        public virtual ICollection<Tag> Tags { get; set; } = new List<Tag>();

        #endregion

        #region Functions

        [NotMapped]
        [JsonIgnore]
        public bool IsActive
        {
            get
            {
                return Status == AdvertiseStatus.Published
                    && Active == true && HideInSearch == false;
            }
        }

        [NotMapped]
        [JsonIgnore]
        public Advertise ParentOrSelf
        {
            get
            {
                return Parent != null ? Parent : this;
            }
        }

        [NotMapped]
        [JsonIgnore]
        public bool AnyChildrenOrSelfIsEmpty
        {
            get
            {
                return EmptyTonight || (Childs != null && Childs.Any(x => x.EmptyTonight));
            }
        }

        [NotMapped]
        [JsonIgnore]
        public MainTypeEnum MainType { 
            get {
                return Mode == AdvertiseMode.Single ? MainTypeEnum.Single :
                    (GetHotelTypes().Contains(TypeID) ? MainTypeEnum.Hotel : MainTypeEnum.Complex);
            }
        }

        [NotMapped]
        [JsonIgnore]
        public string VideoUrl { 
            get {
                return VideoStatus switch
                {
                    VideoStatusEnum.Unset => string.Empty,
                    VideoStatusEnum.Pending => $"/video/residences/pending/pendingResidenceVideo_{Id}.mp4?t={DateTime.Now.Ticks}",
                    _ => $"/video/residences/residenceVideo_{Id}.mp4?t={DateTime.Now.Ticks}"
                };
            }
        }

        public Advertise ShallowCopy() => (Advertise)this.MemberwiseClone();

        public void SetNotVerifyReasons(IList<NotVerifyReasonsEnum> list)
        {
            this.NotVerifyReasons = string.Join(",", list.Cast<int>().ToArray());
        }

        public IEnumerable<Reserve> SuccessfullReserves()
        {
            if (Childs != null && Childs.Any())
            {
                var result = new List<Reserve>();
                foreach (var child in Childs)
                {
                    result.AddRange(child.Reserves.Where(w =>
                        w.Status == Reserve.ReserveStatus.CancelRequestByHost ||
                        (w.Status > Reserve.ReserveStatus.Rejected &&
                        w.Status < Reserve.ReserveStatus.CanceledByGuest)));
                }
                return result;
            }
            else
            {
                return Reserves.Where(w =>
                        w.Status == Reserve.ReserveStatus.CancelRequestByHost ||
                        (w.Status > Reserve.ReserveStatus.Rejected &&
                        w.Status < Reserve.ReserveStatus.CanceledByGuest));
            }
        }

        public IEnumerable<Comment> PublishedComments()
        {
            return Comments.Where(w => w.Status == Comment.CommentStatus.publish &&
                    w.type == Comment.CommentType.advertise)
                    .OrderByDescending(o => o.CreateDate);
        }

        public Dictionary<int, List<ReportItem>> UserRatingDict()
        {
            var dict = new Dictionary<int, List<ReportItem>>();
            //var userRatingItems = ReportItems;
            foreach (var rp in ReportItems)
            {
                if (dict.ContainsKey(rp.UserID))
                {
                    dict[rp.UserID].Add(rp);
                }
                else
                {
                    dict.Add(rp.UserID, new List<ReportItem>() { rp });
                }
            }
            return dict;
        }

        public DiscountDTO GetFirstDiscountData(bool shortenedDate = false,
            bool shortenedYear = false)
        {
            var today = DateTime.Now.Date;
            var discounts = DiscountTables.Where(w => w.Percent > 2 &&
                w.To > today);
            if (discounts.Any() == false)
            {
                return new DiscountDTO()
                {
                    Percent = 0,
                    DateString = ""
                };
            }
            DiscountTable discount;
            if (discounts.Count() == 1)
            {
                discount = discounts.First();
            }
            else
            {
                discount = discounts.OrderBy(x => x.From).First();
            }
            var from_string = DateTimeUtility.GregorianToPersianDate(
                discount.From).Replace(",", "/");
            var to_string = DateTimeUtility.GregorianToPersianDate(
                discount.To).Replace(",", "/");
            if (shortenedDate)
            {
                from_string = from_string.Remove(0, 5);
                to_string = to_string.Remove(0, 5);
            }
            else if (shortenedYear)
            {
                from_string = from_string.Remove(0, 2);
                to_string = to_string.Remove(0, 2);
            }
            return new DiscountDTO()
            {
                Percent = discount.Percent,
                DateString = from_string + " تا " + to_string
            };
        }

        public Comment GetSuspendedComment(int userId)
        {
            return Comments.Where(w => w.SenderUserID == userId &&
                w.Status == Comment.CommentStatus.suspend &&
                w.type == Comment.CommentType.advertise).FirstOrDefault();
        }

        public object GetPropertyValue(Property property)
        {
            return GetType().GetProperty(property.ToString()).GetValue(this);
        }

        public void AddSupportInfo(string text, User supporter)
        {
            var now = DateTime.Now;
            var date = DateTimeUtility.GregorianToPersianDate(now) + " " + now.ToString("HH:mm");
            text = text.Replace(",", "");
            date = date.Replace(",", "/");
            var supporter_name = !string.IsNullOrEmpty(supporter.FullName) ?
                supporter.FullName : supporter.Id.ToString();
            var info = string.Format("{0} - {1} : {2}", date, supporter_name, text);
            if (string.IsNullOrEmpty(SupportDescription))
            {
                SupportDescription = info;
            }
            else
            {
                SupportDescription += "," + info;
            }
        }

        public string[] GetSupportInfoList()
        {
            var output = new List<string>();
            if (!string.IsNullOrEmpty(SupportDescription))
            {
                return SupportDescription.Split(',');
            }
            else
            {
                return new string[0];
            }
        }

        public bool CanPublish()
        {
            return Status == AdvertiseStatus.Published && Active;
        }

        public List<DateTime> OccupiedDates()
        {
            var yesterday = DateTime.Now.Date.AddDays(-1);
            if (UnitCount > 1)
            {
                return OccupiedTables.Where(w => w.Date >= yesterday)
                    .GroupBy(g => g.Date)
                    .Where(s => s.Count() >= UnitCount)
                    .Select(s => s.Key).Distinct().ToList();
            }
            return OccupiedTables.Where(w => w.Date >= yesterday)
                .Select(s => s.Date).Distinct().ToList();
        }

        public List<DateTime> ReservedDates()
        {
            return OccupiedTables.Where(w => w.ReserveID != null)
                .Select(s => s.Date).Distinct().ToList();
        }

        public List<DateTime> ReserveRequestDates()
        {
            return OccupiedTables.Where(w => w.Reserve != null &&
                w.Reserve.Status == Reserve.ReserveStatus.WaitForResponse)
                .Select(s => s.Date).Distinct().ToList();
        }

        public List<DateTime> AcceptedReserveDates()
        {
            if (OccupiedTables == null || !OccupiedTables.Any())
                return new List<DateTime>();
            var list = OccupiedTables.Where(w => w.Reserve != null &&
                 w.Reserve.Status == Reserve.ReserveStatus.WaitForReserve);
            if (list == null || !list.Any())
                return new List<DateTime>();
            return list.Select(s => s.Date).Distinct().ToList();
        }

        public List<Property> GetActiveAmeneties()
        {
            List<Property> activeAmenities = new List<Property>();

            if (Pool == true)
            {
                activeAmenities.Add(Property.Pool);
            }
            if (Oven == true)
            {
                activeAmenities.Add(Property.Oven);
            }
            if (Refrigerator == true)
            {
                activeAmenities.Add(Property.Refrigerator);
            }
            if (KitchenHood == true)
            {
                activeAmenities.Add(Property.KitchenHood);
            }
            if (KitchenUtensils == true)
            {
                activeAmenities.Add(Property.KitchenUtensils);
            }
            if (TeaMaker == true)
            {
                activeAmenities.Add(Property.TeaMaker);
            }
            if (MicrowaveOven == true)
            {
                activeAmenities.Add(Property.MicrowaveOven);
            }
            if (Wifi == true)
            {
                activeAmenities.Add(Property.Wifi);
            }
            if (TV == true)
            {
                activeAmenities.Add(Property.TV);
            }
            if (SoundSystem == true)
            {
                activeAmenities.Add(Property.SoundSystem);
            }
            if (Golf == true)
            {
                activeAmenities.Add(Property.Golf);
            }
            if (Bathroom == true)
            {
                activeAmenities.Add(Property.Bathroom);
            }
            if (WashingMachine == true)
            {
                activeAmenities.Add(Property.WashingMachine);
            }
            if (Hairdryer == true)
            {
                activeAmenities.Add(Property.Hairdryer);
            }
            if (PoolTable == true)
            {
                activeAmenities.Add(Property.PoolTable);
            }
            if (Foosball == true)
            {
                activeAmenities.Add(Property.Foosball);
            }
            if (Sauna == true)
            {
                activeAmenities.Add(Property.Sauna);
            }
            if (Jacuzzi == true)
            {
                activeAmenities.Add(Property.Jacuzzi);
            }
            if (Balcony == true)
            {
                activeAmenities.Add(Property.Balcony);
            }
            if (Filming == true)
            {
                activeAmenities.Add(Property.Filming);
            }
            return activeAmenities;
        }

        private static List<AdvertiseType> IsfahanForbiddenTypes =
            new List<AdvertiseType>() { AdvertiseType.Apartment,
                AdvertiseType.SuitAndRoom, AdvertiseType.House,
                AdvertiseType.Villa, AdvertiseType.Complex };

        public bool IsForbidden
        {
            get
            {
                return CityId == 794 && IsfahanForbiddenTypes.Contains(ParentOrSelf.TypeID);
                //return false;
            }
        }

        public void UpdateStatusAfterChangeInfo(bool hasImportantChange = false)
        {
            switch (Status)
            {
                case AdvertiseStatus.FirstReady:
                case AdvertiseStatus.NotCompleted:
                case AdvertiseStatus.ReadyToPublish:
                case AdvertiseStatus.Deleted:
                case AdvertiseStatus.Unset:
                    break;
                case AdvertiseStatus.NotVerified:
                case AdvertiseStatus.Archived:
                    Status = AdvertiseStatus.ReadyToPublish;
                    break;
                case AdvertiseStatus.Published:
                    if (hasImportantChange)
                    {
                        Status = AdvertiseStatus.ReadyToPublish;
                    }
                    break;
            }
        }

        public Comment GetCommentBySenderUser(long senderUserId, Comment.CommentType type, bool onlyPublished)
        {
            var comments = Comments.Where(f =>
                f.SenderUserID == senderUserId && f.type == type);
            if (onlyPublished)
            {
                comments = comments.Where(x => x.Status == Comment.CommentStatus.publish);
            }
            return comments.FirstOrDefault();
        }

        public float GetAverageUserRating(int user_id = 0)
        {
            var reportItems = ReportItems.AsQueryable();
            if (user_id > 0)
            {
                reportItems = reportItems
                    .Where(x => x.UserID == user_id);
                return reportItems.Any() ? reportItems
                    .Average(a => (float)a.Score) : 0;
            }
            return reportItems.Any() ? reportItems
                .Average(x => (float)x.Score) : 0;
        }

        public string GetMainImageUrl()
        {
            return MainPhotoId == null ? null : $"{GeneralData.WebsiteUrl}/api/file/advertise/{Id}/{MainPhotoId}";
        }

        public List<string> GetImagesUrls()
        {
            var urls = new List<string>();
            foreach (var item in Photos)
            {
                urls.Add($"{GeneralData.WebsiteUrl}/api/file/advertise/{Id}/{item.Id}");
            }
            return urls;
        }

        public Dictionary<long, string> GetImagesIdAndUrls()
        {
            var dic = new Dictionary<long, string>();
            foreach (var item in Photos)
            {
                dic.Add(item.Id, $"{GeneralData.WebsiteUrl}/api/file/advertise/{Id}/{item.Id}");
            }
            return dic;
        }

        public bool IsReserveInstant(DateTime fromDate, DateTime toDate)
        {
            if (InstantReserveStatus != InstantReserveStatusEnum.Calendar)
            {
                return InstantReserveStatus == InstantReserveStatusEnum.InActive ? false : true;
            }
            var dateList = DateTimeUtility.DateRangeToList(fromDate, toDate);
            foreach (var item in dateList)
            {
                if (InstantReserveDates.Any(x=> x.Date == item.Date) == false)
                {
                    return false;
                }
            }
            return true;
        }

        #endregion

        #region Static Functions

        public static AdvertiseType[] GetAdvertiseTypes(
            AdvertisePageType page_type = AdvertisePageType.Undefined, AdvertiseType parent_type = AdvertiseType.None)
        {
            var result = new AdvertiseType[] { };
            switch (page_type)
            {
                case AdvertisePageType.Filter:
                    result = new AdvertiseType[] {
                        AdvertiseType.Apartment,
                        AdvertiseType.Villa,
                        AdvertiseType.SuitAndRoom,
                        AdvertiseType.House,
                        AdvertiseType.TourismAccommodation,
                        AdvertiseType.Hut,
                        AdvertiseType.HotelApartment,
                        AdvertiseType.Complex,
                        AdvertiseType.Pansion,
                        AdvertiseType.Hotel,
                        AdvertiseType.Inn,
                        AdvertiseType.Camp
                    };
                    break;
                case AdvertisePageType.Edit:
                    result = new AdvertiseType[] {
                        AdvertiseType.None,
                        AdvertiseType.SuitAndRoom,
                        AdvertiseType.Apartment,
                        AdvertiseType.Complex,
                        AdvertiseType.House,
                        AdvertiseType.Villa,
                        AdvertiseType.Hut,
                        AdvertiseType.TourismAccommodation,
                        AdvertiseType.Hotel,
                        AdvertiseType.Camp,
                        AdvertiseType.HotelApartment,
                        AdvertiseType.Inn,
                        AdvertiseType.Pansion
                    };
                    break;
                default:
                    return Enum.GetValues(typeof(AdvertiseType)) as AdvertiseType[];
            }
            if (parent_type == AdvertiseType.Complex)
            {
                var forbiddenTypes = new List<AdvertiseType>() {
                    AdvertiseType.Complex,
                    AdvertiseType.Hotel,
                    AdvertiseType.HotelApartment,
                    AdvertiseType.Inn,
                    AdvertiseType.Camp,
                    AdvertiseType.Pansion,
                    AdvertiseType.TourismAccommodation
                };
                result = result.Where(x => !forbiddenTypes.Contains(x)).ToArray();
            }
            else if (parent_type == AdvertiseType.HotelApartment)
            {
                var forbiddenTypes = new List<AdvertiseType>() {
                    AdvertiseType.Complex,
                    AdvertiseType.Hotel,
                    AdvertiseType.HotelApartment,
                    AdvertiseType.Inn,
                    AdvertiseType.Camp,
                    AdvertiseType.Pansion,
                    AdvertiseType.TourismAccommodation,
                    AdvertiseType.Villa,
                    AdvertiseType.House,
                    AdvertiseType.Hut
                };
                result = result.Where(x => !forbiddenTypes.Contains(x)).ToArray();
            }
            return result;
        }

        public static int UrlStringToAdvertiseType(string urlString)
        {
            switch (urlString)
            {
                case "":
                    return 81;
                case "آپارتمان":
                    return 82;
                case "ویلا":
                    return 83;
                case "رزرو-هتل":
                    return 87;
                case "بومگردی":
                    return 4;
                default:
                    return -1;
            }
        }

        public static int AdvertiseTypeToHeadType(int type)
        {
            switch (type)
            {
                case 82:
                case 1:
                case 8:
                    return 82;
                case 83:
                case 2:
                case 9:
                    return 83;
                case 87:
                case 6:
                case 5:
                case 7:
                    return 87;
                case 4:
                case 3:
                    return 4;
                default:
                    return 81;
            }
        }

        public static AdvertiseType[] GetComplexSupportedAdvertiseTypes(AdvertiseType complexType)
        {
            switch (complexType)
            {
                case AdvertiseType.HotelApartment:
                    return new AdvertiseType[] {
                        AdvertiseType.None,
                        AdvertiseType.Apartment,
                        AdvertiseType.SuitAndRoom
                    };
                default:
                    return new AdvertiseType[] {
                        AdvertiseType.None,
                        AdvertiseType.Apartment,
                        AdvertiseType.SuitAndRoom,
                        AdvertiseType.Villa,
                        AdvertiseType.House,
                        AdvertiseType.Hut
                    };
            }
        }

        public static List<AdvertiseType> GetHotelTypes()
        {
            return new List<AdvertiseType>()
            {
                AdvertiseType.Hotel,
                AdvertiseType.Inn,
                AdvertiseType.Pansion,
                AdvertiseType.TourismAccommodation,
                AdvertiseType.Camp
            };
        }

        public static AdvertiseMode GetModeByType(AdvertiseType type)
        {
            switch (type)
            {
                case AdvertiseType.Hotel:
                case AdvertiseType.Camp:
                case AdvertiseType.TourismAccommodation:
                case AdvertiseType.Inn:
                case AdvertiseType.Pansion:
                case AdvertiseType.HotelApartment:
                case AdvertiseType.Complex:
                    return AdvertiseMode.Parent;
                    break;
                default:
                    return AdvertiseMode.Single;
                    break;
            }
        }

        public static Array GetPropertyItems(Property property, AdvertiseType parent_type = AdvertiseType.None)
        {
            switch (property)
            {
                case Property.TypeID:
                    return GetAdvertiseTypes(AdvertisePageType.Edit, parent_type);
                case Property.Region:
                    return (Enum.GetValues(typeof(PositionType)) as PositionType[]).
                        OrderBy(x => (int)x).ToArray();
                case Property.Parking:
                    return (Enum.GetValues(typeof(ParkingItems)) as ParkingItems[]).
                        OrderBy(x => (int)x).ToArray();
                case Property.BuildingDirection:
                    return (Enum.GetValues(typeof(BuildingDirectionItems)) as BuildingDirectionItems[]).
                        OrderBy(x => (int)x).ToArray();
                case Property.ExtraBlanketCount:
                    return (Enum.GetValues(typeof(ExtraBlanketCountItems)) as ExtraBlanketCountItems[]).
                        OrderBy(x => (int)x).ToArray();
                case Property.HeatingSystem:
                    return (Enum.GetValues(typeof(HeatingSystemItems)) as HeatingSystemItems[]).
                        OrderBy(x => (int)x).ToArray();
                case Property.CoolingSystem:
                    return (Enum.GetValues(typeof(CoolingSystemItems)) as CoolingSystemItems[]).
                        OrderBy(x => (int)x).ToArray();
                case Property.WC:
                    return (Enum.GetValues(typeof(WCItems)) as WCItems[]).
                        OrderBy(x => (int)x).ToArray();
                case Property.Floor:
                    return (Enum.GetValues(typeof(FloorItems)) as FloorItems[]).
                        OrderBy(x => (int)x).ToArray();
                default:
                    return null;
            }
        }

        public static PoolFeaturesEnum GetPoolFeatureFlag(bool hotWater, bool filteration, bool open, bool covered)
        {
            PoolFeaturesEnum feature = new PoolFeaturesEnum();
            if (hotWater == false && filteration == false && open == false && covered == false)
            {
                feature = PoolFeaturesEnum.None;
            }
            else
            {
                if (hotWater)
                {
                    feature = PoolFeaturesEnum.HotWater;
                }
                if (filteration)
                {
                    feature = feature | PoolFeaturesEnum.Filtration;
                }
                if (open)
                {
                    feature = feature | PoolFeaturesEnum.Open;
                }
                if (covered)
                {
                    feature = feature | PoolFeaturesEnum.Covered;
                }
            }
            return feature;
        }

        public static string GetImageFileAddress(long advertiseId, long fileId, ImageType type = ImageType.Orginal)
        {
            switch (type)
            {
                case ImageType.Orginal:
                    return $"content/advertise/advertise_{advertiseId}_{fileId}.jpg";
                case ImageType.Card:
                    return $"content/accthumb/{advertiseId}/{fileId}/card.jpg";
                case ImageType.Xsmall:
                    return $"content/accthumb/{advertiseId}/{fileId}/xsmall.jpg";
                case ImageType.Small:
                    return $"content/accthumb/{advertiseId}/{fileId}/small.jpg";
                case ImageType.Medium:
                    return $"content/accthumb/{advertiseId}/{fileId}/medium.jpg";
                case ImageType.Large:
                    return $"content/accthumb/{advertiseId}/{fileId}/large.jpg";
                case ImageType.Xlarge:
                    return $"content/accthumb/{advertiseId}/{fileId}/xlarge.jpg";
                case ImageType.Xxlarge:
                    return $"content/accthumb/{advertiseId}/{fileId}/xxlarge.jpg";
                case ImageType.Xxxlarge:
                    return $"content/accthumb/{advertiseId}/{fileId}/xxxlarge.jpg";
                default:
                    return "";
            }
        }

        #endregion

        #region Enums
        public enum AdvertiseType
        {
            None = 0,
            All = 81,
            Apartment = 82,
            Villa = 83,
            Hotel = 87,
            SuitAndRoom = 1,
            House = 2,
            Camp = 3,
            TourismAccommodation = 4,
            HotelApartment = 5,
            Inn = 6,
            Pansion = 7,
            Complex = 8,
            Hut = 9
        }

        public enum AdvertiseMode
        {
            Single = 0,
            Parent = 1,
            Child = 2
        }

        public enum MainTypeEnum
        {
            Single = 0,
            Complex = 1,
            Hotel = 2
        }

        public enum AdvertiseStatus
        {
            Unset = -1,
            ReadyToPublish = 0,
            Published = 1,
            Archived = 2, // suspend
            Deleted = 3,
            NotVerified = 4,
            NotCompleted = 5,
            FirstReady = 6
        }

        public enum AdvertisePageType
        {
            Undefined,
            Filter,
            Edit
        }

        public enum PositionType
        {
            none = 0,
            sahel = 1,
            jungle = 2,
            koohestani = 3,
            biaban = 4,
            shahri = 5,
            hoome = 6,
            roostaee = 7,
            dakhele_shahrak = 8,
            ashayeri = 9,
            SummerQuarter = 10
        }

        public enum OwnershipTypeEnum
        {
            Unset = 0,
            Owner = 1,
            Intermediary = 2
        }

        public enum Property
        {
            TypeID,
            Region,
            ProvinceId,
            CityId,
            AreaId,
            Address,
            Latitude,
            Longitude,
            UnitCount,
            Title,
            Description,


            DailyPrice,
            HolidayPrice,
            PeakHolidayPrice,
            NowruzPrice,
            MonthlyPrice,
            ExtraCapacityPrice,

            BuildingArea,
            LandArea,
            Capacity,
            ExtraCapacity,
            RoomCount,
            Parking,
            SingleBedCount,
            DoubleBedCount,

            Floor,
            BuildingDirection,

            Elevator,
            Pool,

            MainPhotoId,
            AlbumPhoto,

            Sauna,
            Jacuzzi,
            Bathroom,
            Wifi,
            WashingMachine,
            MicrowaveOven,
            SoundSystem,
            Golf,
            PoolTable,
            Foosball,
            Hairdryer,
            TV,
            Oven,
            Refrigerator,
            KitchenHood,
            KitchenUtensils,
            TeaMaker,
            Balcony,
            Filming,

            BlanketAndMattressCount,
            ExtraBlanketCount,
            HeatingSystem,
            CoolingSystem,
            WC,

            Smoking,
            Pets,
            Party,
            RequiredEvidence,
            OtherRules,

            OwnershipType
        }

        public enum ParkingItems
        {
            Unset = 0,
            One = 76,
            Two = 77,
            Three = 78,
            MoreThanThree = 79,
            Jointly = 80,
            NoParking = 2155
        }

        public enum BuildingDirectionItems
        {
            Unset = 0,
            Western = 34,
            Eastern = 35,
            Northern = 36,
            Southern = 37,
            TwoSided = 38
        }

        public enum ExtraBlanketCountItems
        {
            Unset = 0,
            One = 2206,
            Two = 2207,
            Three = 2208,
            Four = 2209,
            Five = 2210,
            MoreThanFive = 2211
        }

        public enum HeatingSystemItems
        {
            Unset = 0,
            Heater = 108,
            Package = 109,
            Radiator = 110,
            AirConditioner = 2127,
            FirePlace = 2128,
            Other = 111,
            None = 2129
        }

        public enum CoolingSystemItems
        {
            Unset = 0,
            Chiller = 112,
            Fancoel = 113,
            WaterCooler = 114,
            Splitter = 115,
            AirConditioner = 116,
            SplitterAndWaterCooler = 117,
            Fan = 2130,
            Other = 118,
            None = 2052
        }

        public enum WCItems
        {
            Unset = 0,
            Persian = 127,
            Europian = 129,
            EuropianAndPersian = 128,
        }

        public enum EuropeanToiletTypeEnum
        {
            Unset = 0,
            Fixed = 1,
            Portable = 2,
            FixedAndPortable = 3
        }

        public enum FloorItems
        {
            Unset = -2,
            Underground = -1,
            Ground = 0,
            Floor1st = 1,
            Floor2nd = 2,
            Floor3rd = 3,
            Floor4th = 4,
            Floor5th = 5,
            Floor6th = 6,
            Floor7th = 7,
            Floor8th = 8,
            Floor9th = 9,
            Floor10th = 10,
            MoreThan10th = 1000
        }

        public enum InstantReserveStatusEnum
        {
            Calendar = 0,
            Permanent = 1,
            InActive = 2
        }

        public enum NotVerifyReasonsEnum
        {
            Default = 0,
            Reason_1 = 2137,
            Reason_2 = 2138,
            Reason_3 = 2140,
            Reason_4 = 2141,
            Reason_5 = 2142,
            Reason_6 = 2144,
            Reason_7 = 2145,
            Reason_8 = 2146,
            Reason_9 = 2148,
            Reason_10 = 2149,
            Reason_11 = 2150,
            Reason_12 = 2151,
            Reason_13 = 2152,
            Reason_14 = 2153,
            Reason_15 = 2167,
            Reason_16 = 2168,
            Reason_17 = 2171,
            Reason_18 = 2172,
            Reason_19 = 2173,
            Reason_20 = 2176,
            Reason_21 = 2181,
            Reason_22 = 2182,
            Reason_23 = 2183,
            Reason_24 = 2184,
            Reason_25 = 2185,
            Reason_26 = 2212,
            Reason_27 = 2453,
            Reason_28 = 2454,
            Reason_29 = 2455,
            Reason_30 = 2456,
            Reason_31 = 2457,
            Reason_32 = 2458,
            Reason_33 = 2459,
        }

        public enum priceRangeTypes
        {
            Daily = 0,
            Holiday = 1,
            HolidayPeak = 2,
            Monthly = 3,
            Norouz = 4
        }

        public enum SortOrder
        {
            Default = 0,
            MoreExpensive = 1,
            Cheaper = 2,
            UserRate = 3,
            Clean = 4
        }

        public enum HygieneProtocolStatus
        {
            NotConsider = 0,
            Consider = 1,
            Verified = 2,
            NotVerified = 3
        }

        [Flags]
        public enum PoolFeaturesEnum
        {
            None = 0,
            HotWater = 1,
            Filtration = 2,
            Open = 4,
            Covered = 8
        }

        public enum ImageType
        {
            Orginal = 0,
            Card = 1,
            Xsmall = 2,
            Small = 3,
            Medium = 4,
            Large = 5,
            Xlarge = 6,
            Xxlarge = 7,
            Xxxlarge = 8
        }

        public enum VillaTypeEnum
        {
            Unset = 0,
            Exclusive = 1,
            Common = 2
        }

        public enum VideoStatusEnum : byte
        {
            Unset = 0,
            Pending = 1,
            Confirmed = 2,
            NotConfirmed = 3
        }

        #endregion
    }
}
