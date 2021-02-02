using Amlakbashi.Core.Entities;
using Amlakbashi.Core.Infrastructure.LocalizationHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amlakbashi.Core.DTOs.AccommodationDTOs.ApiDTOs
{
    [Serializable]
    public class ApiAmenitiesGetDTO
    {
        public static implicit operator ApiAmenitiesGetDTO(Advertise advertise)
        {
            var dto = new ApiAmenitiesGetDTO();
            dto.id = advertise.Id;
            dto.heatingSystem = new Property<int>((int)advertise.HeatingSystem,
                AdvertiseMainLocalization.GetPropertyTitle(Advertise.Property.HeatingSystem),true);
            dto.coolingSystem = new Property<int>((int)advertise.CoolingSystem,
                AdvertiseMainLocalization.GetPropertyTitle(Advertise.Property.CoolingSystem),true);
            dto.wc = new Property<int>((int)advertise.WC,
                AdvertiseMainLocalization.GetPropertyTitle(Advertise.Property.WC), true);
            dto.extraBlanketCount = new Property<int>((int)advertise.ExtraBlanketCount,
                AdvertiseMainLocalization.GetPropertyTitle(Advertise.Property.ExtraBlanketCount), true);
            dto.bathroom = new Property<bool>((bool)advertise.Bathroom,
                AdvertiseMainLocalization.GetPropertyTitle(Advertise.Property.Bathroom), true);
            dto.elevator = new Property<bool>((bool)advertise.Elevator,
                AdvertiseMainLocalization.GetPropertyTitle(Advertise.Property.Elevator), true);
            dto.pool = new Property<bool>((bool)advertise.Pool,
                AdvertiseMainLocalization.GetPropertyTitle(Advertise.Property.Pool), true);
            dto.sauna = new Property<bool>((bool)advertise.Sauna,
                AdvertiseMainLocalization.GetPropertyTitle(Advertise.Property.Sauna), true);
            dto.jacuzzi = new Property<bool>((bool)advertise.Jacuzzi,
                AdvertiseMainLocalization.GetPropertyTitle(Advertise.Property.Jacuzzi), true);
            dto.tv = new Property<bool>((bool)advertise.TV,
                AdvertiseMainLocalization.GetPropertyTitle(Advertise.Property.TV), true);
            dto.wifi = new Property<bool>((bool)advertise.Wifi,
                AdvertiseMainLocalization.GetPropertyTitle(Advertise.Property.Wifi), true);
            dto.washingMachine = new Property<bool>((bool)advertise.WashingMachine,
                AdvertiseMainLocalization.GetPropertyTitle(Advertise.Property.WashingMachine), true);
            dto.refrigerator = new Property<bool>((bool)advertise.Refrigerator,
                AdvertiseMainLocalization.GetPropertyTitle(Advertise.Property.Refrigerator), true);
            dto.oven = new Property<bool>((bool)advertise.Oven,
                AdvertiseMainLocalization.GetPropertyTitle(Advertise.Property.Oven), true);
            dto.microwaveOven = new Property<bool>((bool)advertise.MicrowaveOven,
                AdvertiseMainLocalization.GetPropertyTitle(Advertise.Property.MicrowaveOven), true);
            dto.kitchenHood = new Property<bool>((bool)advertise.KitchenHood,
                AdvertiseMainLocalization.GetPropertyTitle(Advertise.Property.KitchenHood), true);
            dto.kitchenUtensils = new Property<bool>((bool)advertise.KitchenUtensils,
                AdvertiseMainLocalization.GetPropertyTitle(Advertise.Property.KitchenUtensils), true);
            dto.teaMaker = new Property<bool>((bool)advertise.TeaMaker,
                AdvertiseMainLocalization.GetPropertyTitle(Advertise.Property.TeaMaker), true);
            dto.soundSystem = new Property<bool>((bool)advertise.SoundSystem,
                AdvertiseMainLocalization.GetPropertyTitle(Advertise.Property.SoundSystem), true);
            dto.hairDryer = new Property<bool>((bool)advertise.Hairdryer,
                AdvertiseMainLocalization.GetPropertyTitle(Advertise.Property.Hairdryer), true);
            dto.poolTable = new Property<bool>((bool)advertise.PoolTable,
                AdvertiseMainLocalization.GetPropertyTitle(Advertise.Property.PoolTable), true);
            dto.foosball = new Property<bool>((bool)advertise.Foosball,
                AdvertiseMainLocalization.GetPropertyTitle(Advertise.Property.Foosball), true);
            dto.golf = new Property<bool>((bool)advertise.Golf,
                AdvertiseMainLocalization.GetPropertyTitle(Advertise.Property.Golf), true);
            return dto;
        }

        public long id { get; set; }
        public bool group { get; set; }
        public int groupId { get; set; }
        public Property<int> heatingSystem { get; set; }
        public List<SelectItem> heatingSystemSelectItem { get; set; }
        public Property<int> coolingSystem { get; set; }
        public List<SelectItem> coolingSystemSelectItem { get; set; }
        public Property<int> wc { get; set; }
        public List<SelectItem> wcSelectItem { get; set; }
        public Property<int> extraBlanketCount { get; set; }
        public List<SelectItem> extraBlanketSelectItem { get; set; }
        public Property<bool> bathroom { get; set; }
        public Property<bool> pool { get; set; }
        public Property<bool> elevator { get; set; }
        public Property<bool> sauna { get; set; }
        public Property<bool> jacuzzi { get; set; }
        public Property<bool> tv { get; set; }
        public Property<bool> wifi { get; set; }
        public Property<bool> washingMachine { get; set; }
        public Property<bool> refrigerator { get; set; }
        public Property<bool> oven { get; set; }
        public Property<bool> microwaveOven { get; set; }
        public Property<bool> kitchenHood { get; set; }
        public Property<bool> kitchenUtensils { get; set; }
        public Property<bool> teaMaker { get; set; }
        public Property<bool> soundSystem { get; set; }
        public Property<bool> hairDryer { get; set; }
        public Property<bool> poolTable { get; set; }
        public Property<bool> foosball { get; set; }
        public Property<bool> golf { get; set; }
    }
}
