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
    public class ApiRulesDTO
    {
        public static implicit operator ApiRulesDTO(Advertise advertise)
        {
            var dto = new ApiRulesDTO();
            dto.id = advertise.Id;
            dto.allowParty = new Property<bool?>(advertise.AllowParty,
                AdvertiseMainLocalization.GetPropertyTitle(Advertise.Property.AllowParty), true);
            dto.allowPets = new Property<bool?>(advertise.AllowPets,
                AdvertiseMainLocalization.GetPropertyTitle(Advertise.Property.AllowPets), true);
            dto.allowSmoking = new Property<bool?>(advertise.AllowSmoking,
                AdvertiseMainLocalization.GetPropertyTitle(Advertise.Property.AllowSmoking), true);
            dto.evidenceRequired = new Property<string>(advertise.EvidenceRequired,
                AdvertiseMainLocalization.GetPropertyTitle(Advertise.Property.EvidenceRequired), true);
            dto.otherRules = new Property<string>(advertise.OtherRules,
                AdvertiseMainLocalization.GetPropertyTitle(Advertise.Property.OtherRules), true);
            return dto;
        }

        public long id { get; set; }
        public Property<bool?> allowParty { get; set; }
        public Property<bool?> allowPets { get; set; }
        public Property<bool?> allowSmoking { get; set; }
        public Property<string> evidenceRequired { get; set; }
        public Property<string> otherRules { get; set; }
    }
}
