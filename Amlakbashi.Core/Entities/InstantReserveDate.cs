using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Amlakbashi.Core.Entities
{
    public class InstantReserveDate : Entity<int>
    {
        public long ResidenceId { get; set; }

        [Column(TypeName = "Date")]
        public DateTime Date { get; set; }

        [ForeignKey(nameof(ResidenceId))]
        public virtual Advertise Residence { get; set; }
    }
}
