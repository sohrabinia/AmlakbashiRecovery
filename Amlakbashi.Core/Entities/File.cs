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
        public bool IsDeleted { get; set; }
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

        [JsonIgnore]
        [InverseProperty(nameof(Advertise.Photos))]
        public virtual ICollection<Advertise> Advertises { get; set; }

        [JsonIgnore]
        [InverseProperty(nameof(Advertise.MainPhoto))]
        public virtual ICollection<Advertise> MainPhotos { get; set; }

        [JsonIgnore]
        [InverseProperty(nameof(Advertise.LicenseFile))]
        public virtual Advertise AdvertiseLicense { get; set; }

        // ---------------------------------------------------- 

        public const string AdvertiseImageDirectory = "content/advertise";
        public const string UserImagesDirectory = "content/users";
        public const string AdvertiseLicenseImagesDirectory = "content/licenses";
        public const string ImageChacheDerectory = "content/imgcache";

        public string CorrectedFilePath {
            get {
                return FilePath.StartsWith('~') ? FilePath.Replace("~/", "") : 
                    FilePath.StartsWith('/') ? FilePath.Substring(1) : FilePath;
            }
        }

        public File Clone()
        {
            return (File)this.MemberwiseClone();
        }

        public static bool IsValidImageContentType(string contentType)
        {
            contentType = contentType.ToLower();
            return contentType == "image/png" ||
                contentType == "image/gif" ||
                contentType == "image/jpg" ||
                contentType == "image/jpeg"
                ? true : false;
        }

        public enum MinifyStatusEnum
        {
            None = 0,
            Done = 1,
            Failed = 2
        }

        public enum FileTypes
        {
            Image = 0,
            Video = 1,
            Voice = 2,
            File = 3,
            zip = 4
        }
    }
}
