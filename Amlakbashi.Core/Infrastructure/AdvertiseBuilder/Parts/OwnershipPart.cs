using Amlakbashi.Core.Infrastructure.AdvertiseBuilder.Base;
using Amlakbashi.Core.Base.Builder;
using Amlakbashi.Core.Common.Localization;
using System.Collections.Generic;
using System.Linq;
using Amlakbashi.Core.Entities;

namespace Amlakbashi.Core.Infrastructure.AdvertiseBuilder.Parts
{
    public class OwnershipPart : IPart, IValidator
    {
        [Important]
        public Advertise.OwnershipTypeEnum OwnershipType { get; set; }
        public string OwnerPhoneNumber { get; set; }
        public string OwnerFullName { get; set; }

        public bool Validate(out Dictionary<string, string> errors, out string msg)
        {
            errors = new Dictionary<string, string>();
            if ((int)OwnershipType < 1)
            {
                errors.Add("OwnershipType", LocalizationStringData.Get("ACC_VALIDATION_OWNERSHIP"));
            }
            if (OwnershipType == Advertise.OwnershipTypeEnum.Intermediary &&
                (string.IsNullOrEmpty(OwnerPhoneNumber) || string.IsNullOrEmpty(OwnerFullName)))
            {
                errors.Add("OwnerPhoneNumber", LocalizationStringData.Get("ACC_VALIDATION_OWNERMOBILENAME"));
                errors.Add("OwnerFullName", LocalizationStringData.Get("ACC_VALIDATION_OWNERMOBILENAME"));
            }
            msg = null;
            return errors.Any() == false;
        }
    }
}
