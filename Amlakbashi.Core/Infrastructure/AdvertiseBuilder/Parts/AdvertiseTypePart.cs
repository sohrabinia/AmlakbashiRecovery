using Amlakbashi.Core.Infrastructure.AdvertiseBuilder.Base;
using Amlakbashi.Core.Base.Builder;
using Amlakbashi.Core.Common.Localization;
using System.Collections.Generic;
using System.Linq;
using static Amlakbashi.Core.Entities.Advertise;

namespace Amlakbashi.Core.Infrastructure.AdvertiseBuilder.Parts
{
    public class AdvertiseTypePart : IPart, IValidator
    {
        [Important]
        public AdvertiseType TypeID { get; set; }

        public bool Validate(out Dictionary<string, string> errors, out string msg)
        {
            errors = new Dictionary<string, string>();
            if ((int)TypeID < 1)
            {
                errors.Add("TypeID", LocalizationStringData.Get("ACC_VALIDATION_ADVERTISE_TYPE"));
            }
            msg = null;
            return errors.Any() == false;
        }
    }
}
