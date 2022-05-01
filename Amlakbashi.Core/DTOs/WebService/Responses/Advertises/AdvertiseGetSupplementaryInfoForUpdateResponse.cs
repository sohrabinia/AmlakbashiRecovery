using Amlakbashi.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amlakbashi.Core.DTOs.WebService.Responses.Advertises
{
    public class AdvertiseGetSupplementaryInfoForUpdateResponse
    {
        public long id { get; set; }
        public Advertise.HeatingSystemItems heatingSystem { get; set; }
        public Advertise.CoolingSystemItems coolingSystem { get; set; }
        public Advertise.WCItems wc { get; set; }
        public bool elevator { get; set; }
        public bool pool { get; set; }
        public Advertise.PoolFeaturesEnum poolFeatures { get; set; }
        public bool sauna { get; set; }
        public bool jacuzzi { get; set; }
        public bool bathroom { get; set; }
        public bool wifi { get; set; }
        public bool washingMachine { get; set; }
        public bool microwaveOven { get; set; }
        public bool soundSystem { get; set; }
        public bool golf { get; set; }
        public bool poolTable { get; set; }
        public bool foosball { get; set; }
        public bool hairdryer { get; set; }
        public bool tv { get; set; }
        public bool oven { get; set; }
        public bool refrigerator { get; set; }
        public bool kitchenHood { get; set; }
        public bool kitchenUtensils { get; set; }
        public bool teaMaker { get; set; }
        public bool party { get; set; }
        public bool pets { get; set; }
        public bool smoking { get; set; }
        public string evidenceRequired { get; set; }
        public string otherRules { get; set; }
        public int ownershipStatus { get; set; }
        public string ownerPhoneNumber { get; set; }
        public string ownerFullName { get; set; }
        public bool license { get; set; }
        public string licenseNumber { get; set; }
        public string licenseImageUrl { get; set; }

        public static implicit operator AdvertiseGetSupplementaryInfoForUpdateResponse(Advertise advertise)
        {
            return new AdvertiseGetSupplementaryInfoForUpdateResponse()
            {
                id = advertise.Id,
                heatingSystem = advertise.HeatingSystem,
                coolingSystem = advertise.CoolingSystem,
                wc = advertise.WC,
                party = advertise.AllowParty,
                pets = advertise.AllowPets,
                smoking = advertise.AllowSmoking,
                evidenceRequired = advertise.EvidenceRequired,
                otherRules = advertise.OtherRules,
                ownershipStatus = advertise.OwnershipType,
                ownerFullName = advertise.OwnerFullName,
                ownerPhoneNumber = advertise.OwnerMobile,
                pool = advertise.Pool ?? false,
                poolFeatures = advertise.PoolFeatures,
                elevator = advertise.Elevator ?? false,
                bathroom = advertise.Bathroom ?? false,
                foosball = advertise.Foosball ?? false,
                golf = advertise.Golf ?? false,
                hairdryer = advertise.Hairdryer ?? false,
                jacuzzi = advertise.Jacuzzi ?? false,
                kitchenHood = advertise.KitchenHood ?? false,
                kitchenUtensils = advertise.KitchenUtensils ?? false,
                microwaveOven = advertise.MicrowaveOven ?? false,
                oven = advertise.Oven ?? false,
                poolTable = advertise.PoolTable ?? false,
                refrigerator = advertise.Refrigerator ?? false,
                sauna = advertise.Sauna ?? false,
                soundSystem = advertise.SoundSystem ?? false,
                teaMaker = advertise.TeaMaker ?? false,
                tv = advertise.TV ?? false,
                washingMachine = advertise.WashingMachine ?? false,
                wifi = advertise.Wifi ?? false,
                license = advertise.License,
                licenseNumber = advertise.LicenseNumber,
                licenseImageUrl = $"/api/file/advertise/license/{advertise.Id}"
            };
        }
    }
}
