using Amlakbashi.Core.Common.Entity;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Amlakbashi.Core.Entities
{
    public class BlogPost : Entity<int>, ISoftDelete
    {
        public int UserID { get; set; }
        public int LastModifyUserID { get; set; }
        public long PhotoID { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
        public string BlogLink { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime LastModifyTime { get; set; }
        public BlogPostStatus Status { get; set; }
        public PlaceEnum ShowingPlace { get; set; }
        public int Province { get; set; }
        public int City { get; set; }
        public int Area { get; set; }
        public int AccommodationType { get; set; }
        public int PositionType { get; set; }
        public PoolStatusEnum PoolStatus { get; set; }

        [ForeignKey("UserID")]
        public virtual User User { get; set; }

        public bool IsDeleted { get; set; }

        public bool AnyFilter()
        {
            return (PositionType > 0) ||
                   PoolStatus != PoolStatusEnum.Unset;
        }

        public enum BlogPostStatus
        {
            Draft = 0,
            Published = 1,
            Scrap = 3,
            All = -1
        }

        public enum PlaceEnum
        {
            Unset = -1,
            Accommodation = 0,
            HomePage = 1
        }
        public enum PoolStatusEnum
        {
            Unset = 0,
            WithPool = 1
        }
        public enum SortOrdersEnum
        {
            ID_Descending = 0,
            ID_Ascending = 1
        }

        public static string GetStatusString(BlogPostStatus status)
        {
            switch (status)
            {
                case BlogPostStatus.Draft:
                    return "پیش نویس";
                case BlogPostStatus.Published:
                    return "انتشار یافته";
                case BlogPostStatus.Scrap:
                    return "زباله دان";
                case BlogPostStatus.All:
                    return "همه";
                default:
                    return "";
            }
        }

        public static string GetStatusColor(BlogPostStatus status)
        {
            switch (status)
            {
                case BlogPostStatus.Draft:
                    return "#FABB17";
                case BlogPostStatus.Published:
                    return "#34A853";
                case BlogPostStatus.Scrap:
                    return "#707070";
                case BlogPostStatus.All:
                    return "#242424";
                default:
                    return "";
            }
        }

        public static string GetPlaceString(PlaceEnum place)
        {
            switch (place)
            {
                case PlaceEnum.HomePage:
                    return "صفحه اول";
                case PlaceEnum.Accommodation:
                    return "صفحه اقامتگاه";
                default:
                    return "";
            }
        }
    }
}
