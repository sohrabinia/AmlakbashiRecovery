using Amlakbashi.Core.DTOs.AccommodationDTOs;
using Amlakbashi.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using static Amlakbashi.Core.Entities.Advertise;

namespace Amlakbashi.Core.DTOs.CategoryDTOs
{
    public class CategoryItemDTO
    {
        public List<AccommodationCardDTO> AdvertiseItems { get; set; }
        public string RawUrl { get; set; }
        public string UrlWithParameters { get; set; }
        public DynamicCategory Category { get; set; }
        public int Area { get; set; }
        public string ProvinceString { get; set; }
        public string CityString { get; set; }
        public string AreaString { get; set; }
        public string CountryDirectionString { get; set; }
        public string CategoryH1Title { get; set; }
        public string Phrase { get; set; }
        public string FromPayPerNight { get; set; }
        public string ToPayPerNight { get; set; }
        public string FromMetrazh { get; set; }
        public string ToMetrazh { get; set; }
        public int Parking { get; set; }
        public bool HygieneProtocol { get; set; }
        public string Region { get; set; }
        public string Capacity { get; set; }
        public string Room { get; set; }
        public string Elevator { get; set; }
        public string Pool { get; set; }
        public int PriceRangeType { get; set; }
        public int WcType { get; set; }
        public int Wifi { get; set; }
        public int WashingMachine { get; set; }
        public int Jacuzzi { get; set; }
        public int PoolTable { get; set; }
        public int Foosball { get; set; }
        public int TeaMaker { get; set; }
        public int RulesPets { get; set; }
        public int RulesParty { get; set; }
        public int RulesSmoking { get; set; }
        public bool NorouzSpecial { get; set; }
        public bool TodayEmptyHomes { get; set; }
        public string EmptyRangeFrom { get; set; }
        public string EmptyRangeTo { get; set; }
        public bool DiscountHomes { get; set; }
        public bool InstantReserve { get; set; }
        public string RoomList { get; set; }
        public int Type { get; set; }
        public int T { get; set; }
        public int Sort { get; set; }
        public IList<DynamicCategory> RelatedCategories { get; set; }
        public bool AnyTodayEmpty { get; set; }
        public double PagesCount { get; set; }
        public int CurrentPageNumber { get; set; }
        public string Title { get; set; }
        public string Keywords { get; set; }
        public string Description { get; set; }
        public string AccTypeUrlString { get; set; }
        public string CategoryFaqTrustQuestion { get; set; }
        public string CategoryFaqTrustAnswer { get; set; }
        public string CategoryFaqPriceQuestion { get; set; }
        public string CategoryFaqPriceAnswer { get; set; }
        public string CategoryFaqAreasQuestion { get; set; }
        public string CategoryFaqAreasAnswer { get; set; }
        public string CategoryFaqEvidenceQuestion { get; set; }
        public string CategoryFaqEvidenceAnswer { get; set; }
        public string CategoryFaqReserveQuestion { get; set; }
        public string CategoryFaqReserveAnswer { get; set; }
        public string CategoryFaqHostQuestion { get; set; }
        public string CategoryFaqHostAnswer { get; set; }
        public int[] PriceOptions { get; set; }
        public int[] MonthlyPriceOptions { get; set; }
        public string DateString { get; set; }
        public int PriceMin { get; set; }
        public int PriceMax { get; set; }
        public IList<PositionType> PositionItems { get; set; }
        public string RegionString { get; set; }
        public string PriceString { get; set; }
        public string AccTypeString { get; set; }
        public List<int> RoomListIds { get; set; }
        public string QueryString { get; set; }
        public IList<Region> Provinces { get; set; }
        public IList<Region> Cities { get; set; }
        public IList<Region> Areas { get; set; }
        public bool ForAdvertisePage { get; set; }
    }
}
