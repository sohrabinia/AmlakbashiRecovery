using Amlakbashi.Core.Common.Entity;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Amlakbashi.Core.Entities
{
    public class UserFavorite : Entity<int>
    {
        [Column("FavoriteID")]
        public override int Id { get; set; }
        public long AdvertiseID { get; set; }
        public DateTime SetDate { get; set; }
    }
}
