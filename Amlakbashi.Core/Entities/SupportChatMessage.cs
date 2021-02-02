using Amlakbashi.Core.Common.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amlakbashi.Core.Entities
{
    [SoftDelete(EntityDefaults.IsDeleted)]
    public class SupportChatMessage : Entity<long>, ISoftDelete
    {
        [Column("ID")]
        public override long Id { get; set; }
        public long SupportChatID { get; set; }
        public int? UserID { get; set; }
        public int TypeInt { get; set; }
        public int ReadStatusInt { get; set; }
        public string Text { get; set; }
        public DateTime CreateTime { get; set; }
        public bool IsDeleted { get; set; }

        [ForeignKey("UserID")]
        public virtual User User { get; set; }

        [ForeignKey("SupportChatID")]
        public virtual SupportChat SupportChat { get; set; }

        public TypeEnum Type
        {
            get
            {
                return (TypeEnum)TypeInt;
            }
            set
            {
                TypeInt = (int)value;
            }
        }
        public ReadStatusEnum ReadStatus
        {
            get
            {
                return (ReadStatusEnum)ReadStatusInt;
            }
            set
            {
                ReadStatusInt = (int)value;
            }
        }
        public enum TypeEnum
        {
            User = 0,
            Supporter = 1
        }
        public enum ReadStatusEnum
        {
            NotRead = 0,
            Read = 1
        }
    }
}
