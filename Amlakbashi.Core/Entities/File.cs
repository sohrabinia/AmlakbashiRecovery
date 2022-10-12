using Amlakbashi.Core.Common.Entity;
using Amlakbashi.Core.Common.StaticData;
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
        public FileTypeEnum Type { get; set; }
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
        [InverseProperty(nameof(Advertise.Video))]
        public virtual Advertise ResidenceVideo { get; set; }

        [JsonIgnore]
        [InverseProperty(nameof(Advertise.LicenseFile))]
        public virtual Advertise AdvertiseLicense { get; set; }

        // ---------------------------------------------------- 

        public const string ResidenceImagesDirectory = "content/advertise";
        public static readonly string ResidenceVideosDirectory = $"{GeneralData.VideosDirectoryDrive}/residences";
        public static readonly string PendingResidenceVideosDirectory = $"{GeneralData.VideosDirectoryDrive}/residences/pending";
        public const string ResidenceLicenseImagesDirectory = "content/licenses";
        public const string UserImagesDirectory = "content/users";
        public const string ImageCacheDirectory = "content/imgcache";

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

        public static bool IsValidVideoContentType(string contentType)
        {
            contentType = contentType.ToLower();
            return contentType == "video/mp4"
                ? true : false;
        }

        public enum MinifyStatusEnum
        {
            None = 0,
            Done = 1,
            Failed = 2
        }

        public enum FileTypeEnum : byte
        {
            Unset = 0,
            UserImage = 1,
            ResidenceImage = 2,
            ResidenceLicense = 3,
            ResidenceVideo = 4,
            BlogPostImage = 5
        }
    }
}
