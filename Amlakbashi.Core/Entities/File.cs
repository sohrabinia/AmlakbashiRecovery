using Amlakbashi.Core.Common.Entity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amlakbashi.Core.Entities
{
    public class File : Entity<long>, ISoftDelete
    {
        [Column("FileID")]
        public override long Id { get; set; }
        public DateTime PostDate { get; set; }
        public DateTime LastModifyDate { get; set; }
        public int UserID { get; set; }
        public string FilePath { get; set; }
        public int MinifyStatusInt { get; set; }
        public int MinifyMaxWidth { get; set; }
        public long MinifyQualityPercent { get; set; }

        [JsonIgnore]
        [InverseProperty("Photos")]
        public virtual ICollection<Advertise> Advertises { get; set; }

        public MinifyStatusEnum MinifyStatus
        {
            get
            {
                return (MinifyStatusEnum)MinifyStatusInt;
            }
            set
            {
                this.MinifyStatusInt = (int)value;
            }
        }

        public bool IsDeleted { get; set; }

        public enum MinifyStatusEnum
        {
            None = 0,
            Done = 1,
            Failed = 2
        }

        public File Clone()
        {
            return (File)this.MemberwiseClone();
        }

        public static List<File> GetListClone(List<File> source)
        {
            return source.Select(item => item.Clone()).ToList();
        }

        public enum FileTypes
        {
            Image = 0,
            Video = 1,
            Voice = 2,
            File = 3,
            zip = 4
        }

        public enum FileGroup
        {
            Post = 0,
            Advertise = 1
        }
    }
}
