using Amlakbashi.Core.Common.Utilities;
using Amlakbashi.Core.Infrastructure.AdvertiseBuilder.Parts;
using System;

namespace Amlakbashi.Core.DTOs.AccommodationDTOs.FormInputDTOs
{
    [Serializable]
    public class PhotoInputDTO
    {
        public long? MainPhotoId { get; set; }
        public string AlbumPhoto { get; set; }
        public bool available { get; set; }
        public long accId { get; set; }
        public string accTitle { get; set; }
        public long[] albumPhotosArray { get; set; }

        public PhotoInputDTO(bool available)
        {
            this.available = available;
        }

        public static implicit operator PhotoInputDTO(PhotoPart part)
        {
            PhotoInputDTO dto = null;
            if (part != null)
            {
                dto = new PhotoInputDTO(true);
                PropertyCopier<PhotoPart, PhotoInputDTO>.Copy(part, dto);
            }
            dto.albumPhotosArray = part.AlbumPhotosArray;
            return dto;
        }
    }
}
