using Amlakbashi.Core.Infrastructure.AdvertiseBuilder.Base;
using Amlakbashi.Core.Base.Builder;
using Amlakbashi.Core.Common.Localization;
using System.Collections.Generic;
using System.Linq;

namespace Amlakbashi.Core.Infrastructure.AdvertiseBuilder.Parts
{
    public class LandAreaPart : IPart, IValidator
    {
        public int LandArea { get; set; }

        public bool Validate(out Dictionary<string, string> errors, out string msg)
        {
            errors = new Dictionary<string, string>();
            if (LandArea < 1)
            {
                errors.Add("LandArea", LocalizationStringData.Get("ACC_VALIDATION_LAND_AREA"));
            }
            msg = null;
            return errors.Any() == false;
        }
    }
}
