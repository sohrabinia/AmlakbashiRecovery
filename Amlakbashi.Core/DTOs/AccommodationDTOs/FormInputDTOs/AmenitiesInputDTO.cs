using Amlakbashi.Core.Common.Utilities;
using Amlakbashi.Core.Infrastructure.AdvertiseBuilder.Parts;
using Amlakbashi.Core.Infrastructure.DTOHelpers;
using Amlakbashi.Core.Infrastructure.DTOHelpers.EntitiesDTOHelper;
using System;
using System.Collections.Generic;
using static Amlakbashi.Core.Entities.Advertise;

namespace Amlakbashi.Core.DTOs.AccommodationDTOs.FormInputDTOs
{
    [Serializable]
    public class AmenitiesInputDTO
    {
        public bool? Oven { get; set; }
        public bool? Refrigerator { get; set; }
        public bool? KitchenHood { get; set; }
        public bool? KitchenUtensils { get; set; }
        public bool? TeaMaker { get; set; }
        public bool? MicrowaveOven { get; set; }
        public HeatingSystemItems HeatingSystem { get; set; }
        public CoolingSystemItems CoolingSystem { get; set; }
        public bool? Wifi { get; set; }
        public bool? TV { get; set; }
        public bool? SoundSystem { get; set; }
        public bool? Golf { get; set; }
        public bool? Bathroom { get; set; }
        public bool? WashingMachine { get; set; }
        public bool? Hairdryer { get; set; }
        public WCItems WC { get; set; }
        public bool? PoolTable { get; set; }
        public bool? Foosball { get; set; }
        public bool? Sauna { get; set; }
        public bool? Jacuzzi { get; set; }
        public bool? Pool { get; set; }
        public PoolInputDTO PoolFeatures { get; set; }
        public List<DTOSelectItem> heatingSelectItems { get; set; }
        public List<DTOSelectItem> coolingSelectItems { get; set; }
        public List<DTOSelectItem> wcSelectItems { get; set; }
        public List<DTOCheckbox> booleanAmenities { get; set; }

        public AmenitiesInputDTO()
        {
            heatingSelectItems = AccDTOHelper.GenerateAccSelectList<HeatingSystemItems>();
            coolingSelectItems = AccDTOHelper.GenerateAccSelectList<CoolingSystemItems>();
            wcSelectItems = AccDTOHelper.GenerateAccSelectList<WCItems>();
            SetCheckboxs();
        }

        public static implicit operator AmenitiesInputDTO(AmenitiesPart part)
        {
            AmenitiesInputDTO dto = null;
            if (part != null)
            {
                dto = new AmenitiesInputDTO();
                PropertyCopier<AmenitiesPart, AmenitiesInputDTO>.Copy(part, dto);
                dto.SetCheckboxs();
                dto.PoolFeatures = new PoolInputDTO();
                if (part.Pool == true)
                {
                    dto.PoolFeatures.GenerateDTO(part.PoolFeatures);
                }
            }
            return dto;
        }

        private void SetCheckboxs()
        {
            var booleanAmenities = new List<DTOCheckbox>() {
                AccDTOHelper.GenerateAccCheckbox(Property.Bathroom, Bathroom),
                //AccDTOHelper.GenerateAccCheckbox(Property.Pool, Pool),
                AccDTOHelper.GenerateAccCheckbox(Property.Sauna, Sauna),
                AccDTOHelper.GenerateAccCheckbox(Property.Jacuzzi, Jacuzzi),
                AccDTOHelper.GenerateAccCheckbox(Property.TV, TV),
                AccDTOHelper.GenerateAccCheckbox(Property.Wifi, Wifi),
                AccDTOHelper.GenerateAccCheckbox(Property.WashingMachine, WashingMachine),
                AccDTOHelper.GenerateAccCheckbox(Property.Refrigerator, Refrigerator),
                AccDTOHelper.GenerateAccCheckbox(Property.Oven, Oven),
                AccDTOHelper.GenerateAccCheckbox(Property.MicrowaveOven, MicrowaveOven),
                AccDTOHelper.GenerateAccCheckbox(Property.KitchenHood, KitchenHood),
                AccDTOHelper.GenerateAccCheckbox(Property.KitchenUtensils, KitchenUtensils),
                AccDTOHelper.GenerateAccCheckbox(Property.TeaMaker, TeaMaker),
                AccDTOHelper.GenerateAccCheckbox(Property.SoundSystem, SoundSystem),
                AccDTOHelper.GenerateAccCheckbox(Property.Hairdryer, Hairdryer),
                AccDTOHelper.GenerateAccCheckbox(Property.PoolTable, PoolTable),
                AccDTOHelper.GenerateAccCheckbox(Property.Foosball, Foosball),
                AccDTOHelper.GenerateAccCheckbox(Property.Golf, Golf)
            };
            this.booleanAmenities = booleanAmenities;
        }
    }
}
