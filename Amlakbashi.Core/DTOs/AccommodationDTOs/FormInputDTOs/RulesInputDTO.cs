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
    public class RulesInputDTO
    {
        public bool AllowParty { get; set; }
        public bool AllowPets { get; set; }
        public bool AllowSmoking { get; set; }
        public string EvidenceRequired { get; set; }
        public string OtherRules { get; set; }
        public List<DTOCheckbox> booleanValues { get; set; }

        public RulesInputDTO()
        {
            SetCheckboxs();
        }

        public static implicit operator RulesInputDTO(RulesPart part)
        {
            RulesInputDTO dto = null;
            if (part != null)
            {
                dto = new RulesInputDTO();
                PropertyCopier<RulesPart, RulesInputDTO>.Copy(part, dto);
                dto.SetCheckboxs();
            }
            return dto;
        }

        private void SetCheckboxs()
        {
            this.booleanValues = new List<DTOCheckbox>()
            {
                AccDTOHelper.GenerateAccCheckbox(Property.AllowParty, AllowParty),
                AccDTOHelper.GenerateAccCheckbox(Property.AllowPets, AllowPets),
                AccDTOHelper.GenerateAccCheckbox(Property.AllowSmoking, AllowSmoking)
            };
        }
    }
}
