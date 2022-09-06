using Amlakbashi.Core.Common.Utilities;
using Amlakbashi.Core.DTOs.AccommodationDTOs.FormInputDTOs;
using Amlakbashi.Core.Entities;
using Amlakbashi.Core.Infrastructure.AdvertiseBuilder;
using Amlakbashi.Core.Infrastructure.AdvertiseBuilder.Parts;
using System;

namespace Amlakbashi.Core.DTOs.AccommodationDTOs
{
    [Serializable]
    public class BasicFormDTO
    {
        public long Id { get; set; }
        public Advertise.AdvertiseMode advertiseMode { get; set; }
        public bool Active { get; set; }
        public AdvertiseTypeInputDTO advertiseType { get; set; }
        public PositionInputDTO position { get; set; }
        public BasicFormDTO()
        {
            advertiseType = new AdvertiseTypeInputDTO(false);
            position = new PositionInputDTO();
        }

        public static BasicFormDTO Generate(AdvertiseDirector director, long id)
        {
            var model = new BasicFormDTO()
            {
                Id = id
            };
            model.Active = director.GetAdvertisePart<IdPart>().Active;
            model.advertiseMode = director.Mode;
            PropertyCopier<AdvertiseTypePart, AdvertiseTypeInputDTO>
                .Copy(director.GetAdvertisePart<AdvertiseTypePart>(), model.advertiseType);
            PropertyCopier<PositionPart, PositionInputDTO>
                .Copy(director.GetAdvertisePart<PositionPart>(), model.position);
            return model;
        }
    }
}
