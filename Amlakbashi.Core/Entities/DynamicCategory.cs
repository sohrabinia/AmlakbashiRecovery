using Amlakbashi.Core.Common.Entity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Amlakbashi.Core.Entities.Advertise;
using static Amlakbashi.Core.Entities.Region;

namespace Amlakbashi.Core.Entities
{
    public class DynamicCategory : Entity<int>, ISoftDelete
    {
        [Column("CategoryID")]
        public override int Id { get; set; }

        [JsonIgnore]
        public virtual ICollection<Advertise> Advertises { get; set; }

        [ForeignKey("Province")]
        public virtual Region RegionProvince { get; set; }

        [ForeignKey("City")]
        public virtual Region RegionCity { get; set; }

        [ForeignKey("Area")]
        public virtual Region RegionArea { get; set; }

        public string Title { get; set; }
        public string URL { get; set; }
        public int? City { get; set; }
        public int? Province { get; set; }
        public int? Area { get; set; }
        public CountryDirection CountryDirection { get; set; }
        public AdvertiseType Type { get; set; }
        public int CountAdvertise { get; set; }
        public int CountView { get; set; }
        public int OldCountView { get; set; }
        public string AreaStr { get; set; }
        public DateTime LastModifyDate { get; set; }
        public string Description { get; set; }
        public string DescriptionH1 { get; set; }
        public bool ShowDescription { get; set; }
        public string CustomUrlTitle { get; set; }
        public string RelatedCategoryIds { get; set; }
        public string RegionString { get; set; }
        public string ParentRegionString { get; set; }
        public string TypeString { get; set; }
        public int CountAcc { get; set; }
        public int ParentCountAcc { get; set; }
        public long MinPrice { get; set; }
        public long MaxPrice { get; set; }
        public int ParentMinPrice { get; set; }
        public int ParentMaxPrice { get; set; }
        public string CityAreaListString { get; set; }
        public int CategoryPostID { get; set; }
        public string CategoryPostTitle { get; set; }
        public string CategoryPostText { get; set; }
        public int MostAccType { get; set; }
        public AdvertiseType ParentAccType { get; set; }
        public int RelatedItemsBehaviour { get; set; }
        public bool IsDeleted { get; set; }

        public DynamicCategory Clone()
        {
            return (DynamicCategory)this.MemberwiseClone();
        }

        public static List<DynamicCategory> GetListClone(List<DynamicCategory> source)
        {
            return source.Select(item => item.Clone())
                    .ToList();
        }

        public enum CalculationBehaviour
        {
            Auto = 0,
            Manual = 1
        }
    }
}
