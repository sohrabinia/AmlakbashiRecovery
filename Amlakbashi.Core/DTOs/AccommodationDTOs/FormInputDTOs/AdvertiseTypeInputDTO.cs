using Amlakbashi.Core.Entities;
using Amlakbashi.Core.Infrastructure.DTOHelpers;
using Amlakbashi.Core.Infrastructure.DTOHelpers.EntitiesDTOHelper;
using Amlakbashi.Core.Infrastructure.LocalizationHelpers;
using System;
using System.Collections.Generic;
using static Amlakbashi.Core.Entities.Advertise;

namespace Amlakbashi.Core.DTOs.AccommodationDTOs.FormInputDTOs
{
    [Serializable]
    public class AdvertiseTypeInputDTO
    {
        public AdvertiseType TypeID { get; set; }
        public string advertiseTypeString { get; set; }
        public bool available { get; set; }
        public List<DTOSelectItem> advertiseTypeSelectItems { get; set; }
        public AdvertiseTypeInputDTO(bool available)
        {
            this.available = available;
            advertiseTypeString = AdvertiseMainLocalization.GetAdvertiseTypeUserString(TypeID);
            advertiseTypeSelectItems = GenerateAdvertiseTypeItems();
        }

        private static List<DTOSelectItem> GenerateAdvertiseTypeItems()
        {
            var advertiseTypes = GetAdvertiseTypes(AdvertisePageType.Edit);
            var result = new List<DTOSelectItem>();
            foreach (var advType in advertiseTypes)
            {
                result.Add(new DTOSelectItem((int)advType,
                    AdvertiseMainLocalization.GetAdvertiseTypeUserString(advType)));
            }
            return result;
        }
    }
}
