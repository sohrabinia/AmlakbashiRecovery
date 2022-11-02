using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Amlakbashi.Core.Entities
{
    [Index(nameof(Title), IsUnique = true)]
    public class Tag : Entity<int>
    {
        public DateTime CreateDate { get; set; }

        [MaxLength(200)]
        public string Title { get; set; }
        public TagStatusEnum Status { get; set; }

        public virtual ICollection<Advertise> Residences { get; set; } = new List<Advertise>();

        public enum TagStatusEnum : byte
        {
            Unset = 0,
            Active = 1
        }
    }
}
