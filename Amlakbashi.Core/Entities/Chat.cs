using Amlakbashi.Core.Common.Entity;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amlakbashi.Core.Entities
{
    [SoftDelete(EntityDefaults.IsDeleted)]
    public class Chat : Entity<long>, ISoftDelete
    {
        [Column("ChatID")]
        public override long Id { get; set; }
        public long ReserveID { get; set; }
        public int UserID { get; set; }
        public int ChatStatus { get; set; }
        public int ReadStatus { get; set; }
        public int SupportReadStatus { get; set; }
        public string Text { get; set; }
        public DateTime CreateTime { get; set; }
        public bool IsDeleted { get; set; }

        [ForeignKey("UserID")]
        public virtual User User { get; set; }

        [ForeignKey("ReserveID")]
        public virtual Reserve Reserve { get; set; }

        public enum ChatStatusEnum
        {
            Sent = 0,
            HasForbiddenCharacters = 1,
            Deleted = 3,
        }

        public enum ReadStatusEnum
        {
            NotRead = 0,
            Read = 1
        }
    }
}
