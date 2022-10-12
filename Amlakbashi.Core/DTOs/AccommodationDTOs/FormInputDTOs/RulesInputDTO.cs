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
        public bool Party { get; set; }
        public bool Pets { get; set; }
        public bool Smoking { get; set; }
        public string RequiredEvidence { get; set; }
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
                AccDTOHelper.GenerateAccCheckbox(Property.Party, Party),
                AccDTOHelper.GenerateAccCheckbox(Property.Pets, Pets),
                AccDTOHelper.GenerateAccCheckbox(Property.Smoking, Smoking)
            };
        }
    }
}
