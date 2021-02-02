using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Amlakbashi.Core.Entities
{
    public class Region : Entity<int>
    {
        [Column("RegionID")]
        public override int Id { get; set; }
        public int? ParentID { get; set; }
        public string EnglishName { get; set; }
        public string PersianName { get; set; }
        public int Type { get; set; }
        public string Related { get; set; }
        public int CountAdvertise { get; set; }

        [ForeignKey("ParentID")]
        public virtual Region Parent { get; set; }

        [JsonIgnore]
        public virtual ICollection<Region> Childs { get; set; }

        public static string GetCountryDirectionString(CountryDirection countryDirection)
        {
            switch (countryDirection)
            {
                case CountryDirection.North:
                    return "شمال";
                default:
                    return null;
            }
        }

        public enum AdvertiseRegion
        {
            Province = 0,
            City = 1,
            Area = 2
        }

        public enum RegionStatus
        {
            All = -1,
            Empty = 0,
            HasAdvertise = 1
        }

        public enum CountryDirection
        {
            Unset = 0,
            North = 1
        }

        public enum RegionSortOrder
        {
            Default = 0,
            PersianName = 1
        }
    }
}
