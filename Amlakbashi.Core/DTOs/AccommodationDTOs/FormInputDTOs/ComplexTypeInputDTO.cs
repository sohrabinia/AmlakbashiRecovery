using Amlakbashi.Core.Infrastructure.DTOHelpers;
using Amlakbashi.Core.Infrastructure.LocalizationHelpers;
using System;
using System.Collections.Generic;
using static Amlakbashi.Core.Entities.Advertise;

namespace Amlakbashi.Core.DTOs.AccommodationDTOs.FormInputDTOs
{
    [Serializable]
    public class ComplexTypeInputDTO
    {
        public AdvertiseType TypeID { get; set; }
        public string complexTypeString { get; set; }
        public List<DTOSelectItem> complexTypeSelectItems { get; set; }
        public ComplexTypeInputDTO(AdvertiseType type)
        {
            complexTypeString = AdvertiseMainLocalization.GetAdvertiseTypeUserString(TypeID);
            complexTypeSelectItems = GenerateAdvertiseTypeItems(type);
        }

        private static List<DTOSelectItem> GenerateAdvertiseTypeItems(AdvertiseType type)
        {
            var advertiseTypes = GetComplexSupportedAdvertiseTypes(type);
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
