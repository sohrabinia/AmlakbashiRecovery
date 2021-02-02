using Amlakbashi.Core.Infrastructure.AdvertiseBuilder.Base;
using Amlakbashi.Core.Base.Builder;
using Amlakbashi.Core.Common.Localization;
using System.Collections.Generic;
using System.Linq;

namespace Amlakbashi.Core.Infrastructure.AdvertiseBuilder.Parts
{
    public class OwnershipPart : IPart, IValidator
    {
        [Important]
        public int OwnershipType { get; set; }
        [Important]
        public int OwnerID { get; set; }

        public bool Validate(out Dictionary<string, string> errors, out string msg)
        {
            errors = new Dictionary<string, string>();
            if (OwnershipType < 1)
            {
                errors.Add("OwnershipType", LocalizationStringData.Get("ACC_VALIDATION_OWNERSHIP"));
            }
            msg = null;
            return errors.Any() == false;
        }
    }
}
