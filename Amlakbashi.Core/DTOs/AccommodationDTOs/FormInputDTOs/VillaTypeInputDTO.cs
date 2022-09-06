using Amlakbashi.Core.Common.Utilities;
using Amlakbashi.Core.Entities;
using Amlakbashi.Core.Infrastructure.AdvertiseBuilder.Parts;
using Amlakbashi.Core.Infrastructure.DTOHelpers;
using Amlakbashi.Core.Infrastructure.DTOHelpers.EntitiesDTOHelper;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amlakbashi.Core.DTOs.AccommodationDTOs.FormInputDTOs
{
    public class VillaTypeInputDTO
    {
        public Advertise.VillaTypeEnum VillaType { get; set; }
        public bool mandatory { get; set; }
        public List<DTOSelectItem> villaTypeSelectItems { get; set; }

        public VillaTypeInputDTO(bool mandatory)
        {
            this.mandatory = mandatory;
            villaTypeSelectItems = AccDTOHelper.GenerateAccSelectList<Advertise.VillaTypeEnum>();
        }

        public static implicit operator VillaTypeInputDTO(VillaTypePart part)
        {
            VillaTypeInputDTO dto = null;
            if (part != null)
            {
                dto = new VillaTypeInputDTO(false);
                PropertyCopier<VillaTypePart, VillaTypeInputDTO>.Copy(part, dto);
            }
            return dto;
        }
    }
}
