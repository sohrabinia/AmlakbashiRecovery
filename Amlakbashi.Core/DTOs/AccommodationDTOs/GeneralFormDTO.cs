using Amlakbashi.Core.DTOs.AccommodationDTOs.FormInputDTOs;
using Amlakbashi.Core.Entities;
using Amlakbashi.Core.Infrastructure.AdvertiseBuilder;
using Amlakbashi.Core.Infrastructure.AdvertiseBuilder.Parts;
using Amlakbashi.Core.Infrastructure.LocalizationHelpers;
using System;

namespace Amlakbashi.Core.DTOs.AccommodationDTOs
{
    [Serializable]
    public class GeneralFormDTO
    {
        public long Id { get; set; }
        public Advertise.AdvertiseMode advertiseMode { get; set; }
        public AddressInputDTO address { get; set; }
        public FloorInputDTO floor { get; set; }
        public PhotoInputDTO photo { get; set; }
        public TitleDescInputDTO titleAndDesc { get; set; }
        public MetaTitleDescInputDTO metaTitleAndDesc { get; set; }
        public VillaTypeInputDTO villaType { get; set; }

        public static GeneralFormDTO Generate(AdvertiseDirector director, long id)
        {
            var model = new GeneralFormDTO()
            {
                Id = id
            };
            model.advertiseMode = director.Mode;
            model.address = director.GetAdvertisePart<AddressPart>();
            model.floor = director.GetAdvertisePart<FloorPart>();
            model.photo = director.GetAdvertisePart<PhotoPart>();
            model.titleAndDesc = director.GetAdvertisePart<TitleDescPart>();
            model.metaTitleAndDesc = director.GetAdvertisePart<MetaTitleDescPart>();
            model.villaType = director.GetAdvertisePart<VillaTypePart>();
            model.photo.accId = id;
            model.photo.accTitle = AdvertiseMainLocalization.GetPhotoPersianTitle(director.AdvertiseType, director.Mode);
            return model;
        }
    }
}
