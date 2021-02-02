using Amlakbashi.Core.Common.Entity;
using System.ComponentModel.DataAnnotations.Schema;
using System;

namespace Amlakbashi.Core.Entities
{
    [SoftDelete(EntityDefaults.IsDeleted)]
    public class Service : Entity<int>, ISoftDelete
    {
        [Column("ServiceID")]
        public override int Id { get; set; }
        public string Title { get; set; }
        public int ParentId { get; set; }
        public bool IsDeleted { get; set; }
    }
}
