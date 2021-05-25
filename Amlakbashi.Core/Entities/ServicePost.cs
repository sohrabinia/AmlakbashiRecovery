using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Amlakbashi.Core.Entities
{
    /// <summary>
    /// جدول چند به چند بین پست و سرویس
    /// </summary>
    public class ServicePost : Entity<int>
    {
        [Column("ServicePostID")]
        public override int Id { get; set; }
        public long PostID { get; set; }
        public int ServiceID { get; set; }
    }
}
