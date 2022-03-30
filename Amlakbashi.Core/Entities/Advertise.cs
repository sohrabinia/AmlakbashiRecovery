using Amlakbashi.Core.Common.Utilities;
using Amlakbashi.Core.DTOs.AccommodationDTOs.AccPagesDTOs;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using static Amlakbashi.Core.Entities.Region;

namespace Amlakbashi.Core.Entities
{
    public class Advertise : Entity<long>
    {
        #region Properties
        [Column("AdvertiseID")]
        public override long Id { get; set; }
        public string Title { get; set; }

        [Column("Advertise_AdvertiseID")]
        public long? ParentId { get; set; }
        public string Description { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime LastModifyDate { get; set; }
        public int UserID { get; set; }
        public AdvertiseStatus Status { get; set; }
        public long? PhotoID { get; set; }
        public string AlbumPhoto { get; set; }
        public int? Province { get; set; }
        public int? City { get; set; }
        public int? Area { get; set; }
        public CountryDirection CountryDirection { get; set; }
        public int WebVisit { get; set; }
        public int Overview { get; set; }
        public int ContactClick { get; set; }
        public string MetaTitle { get; set; }
        public string OldSlug { get; set; }
        public string Slug { get; set; }
        public string MetaDescription { get; set; }
        public string Address { get; set; }
        public AdvertiseType TypeID { get; set; }
        public AdvertiseType ParentAccType { get; set; }

        [Column("Region")]
        public PositionType Position { get; set; }
        public int OwnershipType { get; set; }
        public int OwnerID { get; set; }
        public string OwnerMobile { get; set; }
        public string OwnerFullName { get; set; }
        public string NotVerifyReasons { get; set; }
        public long AdvertiseScore { get; set; }
        public int AmlakbashiScore { get; set; }

        [Column("AdvertiseMode")]
        public AdvertiseMode Mode { get; set; }
        public bool IsContactAvailable { get; set; }
        public bool AllowParty { get; set; }
        public bool AllowPets { get; set; }
        public bool AllowSmoking { get; set; }
        public string EvidenceRequired { get; set; }
        public string OtherRules { get; set; }
        public bool TodayIsEmpty { get; set; }
        public int Metrazh { get; set; }
        public bool? Elevator { get; set; }
        public ParkingItems Parking { get; set; }
        public int Room { get; set; }
        public bool? Pool { get; set; }
        public PoolFeaturesEnum PoolFeatures { get; set; }
        public int Capacity { get; set; }
        public int MoreThanCapacity { get; set; }
        public int DailyPrice { get; set; }
        public int NorouzPrice { get; set; }
        public long RentPrice { get; set; }
        public int HolidayPrice { get; set; }
        public int HolidayPikePrice { get; set; }
        public int MoreThanCapacityPrice { get; set; }
        public int LandArea { get; set; }
        public FloorItems Floor { get; set; }
        public int SingleBed { get; set; }
        public int DoublesBed { get; set; }
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
        public int BlanketsAndMattresses { get; set; }
        public ExtraBlanketCountItems ExtraBlanketCount { get; set; }
        public HeatingSystemItems HeatingSystem { get; set; }
        public CoolingSystemItems CoolingSystem { get; set; }
        public WCItems WC { get; set; }
        public int Count { get; set; }
        public bool Available { get; set; }
        public bool HideInCategory { get; set; }
        public float AverageUserRating { get; set; }
        public float TidinessUserRating { get; set; }
        public string LocationString { get; set; }
        public int BasePrice { get; set; }
        public string SupportInfo { get; set; }
        public int InstantReserveCancels { get; set; }
        public InstantReserveStatusEnum InstantReserveStatus { get; set; }
        public int MaxInstantReserveStart { get; set; }
        public int MinReserveDays { get; set; }
        public int MaxReserveDays { get; set; }
        public long unixNorouzMinRequestDate { get; set; }
        public int NorouzOverCapacityPrice { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public HygieneProtocolStatus? HygieneProtocol { get; set; }
        public bool License { get; set; }
        public long? LicenseFileId { get; set; }
        public string LicenseNumber { get; set; }

        [JsonIgnore]
        [ForeignKey(nameof(Province))]
        public virtual Region RegionProvince { get; set; }

        [JsonIgnore]
        [ForeignKey(nameof(City))]
        public virtual Region RegionCity { get; set; }

        [JsonIgnore]
        [ForeignKey(nameof(Area))]
        public virtual Region RegionArea { get; set; }

        [JsonIgnore]
        [ForeignKey(nameof(PhotoID))]
        public virtual File MainPhoto { get; set; }

        [JsonIgnore]
        [ForeignKey(nameof(UserID))]
        public virtual User User { get; set; }

        [JsonIgnore]
        [ForeignKey(nameof(ParentId))]
        public virtual Advertise Parent { get; set; }

        [JsonIgnore]
        [ForeignKey(nameof(LicenseFileId))]
        public virtual File LicenseFile { get; set; }

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
        #endregion

        public bool HasDiscount;
        public Advertise()
        {
            MaxInstantReserveStart = 30;
        }

        #region Functions

        public Advertise ShallowCopy()
        {
            return (Advertise)this.MemberwiseClone();
        }

        [NotMapped]
        [JsonIgnore]
        public bool IsActive
        {
            get
            {
                return (AdvertiseStatus)Status == AdvertiseStatus.Published
                    && Available == true && HideInCategory == false;
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
                return TodayIsEmpty || (Childs != null && Childs.Any(x => x.TodayIsEmpty));
            }
        }

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
            if (string.IsNullOrEmpty(SupportInfo))
            {
                SupportInfo = info;
            }
            else
            {
                SupportInfo += "," + info;
            }
        }

        public string[] GetSupportInfoList()
        {
            var output = new List<string>();
            if (!string.IsNullOrEmpty(SupportInfo))
            {
                return SupportInfo.Split(',');
            }
            else
            {
                return new string[0];
            }
        }

        public bool CanPublish()
        {
            return Status == AdvertiseStatus.Published && Available;
        }

        public List<DateTime> OccupiedDates()
        {
            //if (IsForbidden)
            //{
            //    var result = new List<DateTime>();
            //    var d = DateTime.Now.Date;
            //    for (int i = 0; i < 100; i++)
            //    {
            //        result.Add(d);
            //        d = d.AddDays(1);
            //    }
            //    return result;
            //}
            var yesterday = DateTime.Now.Date.AddDays(-1);
            if (Count > 1)
            {
                return OccupiedTables.Where(w => w.Date >= yesterday)
                    .GroupBy(g => g.Date)
                    .Where(s => s.Count() >= Count)
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
                return City == 794 && IsfahanForbiddenTypes.Contains(ParentOrSelf.TypeID);
                //return false;
            }
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

        public enum Property
        {
            TypeID,
            Region,
            Province,
            City,
            Area,
            Address,
            Latitude,
            Longitude,
            Count,
            Title,
            Description,


            DailyPrice,
            HolidayPrice,
            HolidayPikePrice,
            NorouzPrice,
            RentPrice,
            MoreThanCapacityPrice,

            Metrazh,
            LandArea,
            Capacity,
            MoreThanCapacity,
            Room,
            Parking,
            SingleBed,
            DoublesBed,

            Floor,
            BuildingDirection,

            Elevator,
            Pool,

            PhotoID,
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

            BlanketsAndMattresses,
            ExtraBlanketCount,
            HeatingSystem,
            CoolingSystem,
            WC,

            AllowSmoking,
            AllowPets,
            AllowParty,
            EvidenceRequired,
            OtherRules,

            OwnershipType,
            OwnerID
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
            None = 0,
            Requested = 1,
            Confirmed = 2
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

        #endregion
    }
}
