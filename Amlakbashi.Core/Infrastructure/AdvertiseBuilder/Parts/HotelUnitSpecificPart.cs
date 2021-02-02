using Amlakbashi.Core.Infrastructure.AdvertiseBuilder.Base;
using Amlakbashi.Core.Base.Builder;
using Amlakbashi.Core.Common.Localization;
using System.Collections.Generic;
using System.Linq;

namespace Amlakbashi.Core.Infrastructure.AdvertiseBuilder.Parts
{
    public class HotelUnitSpecificPart : IPart, IValidator
    {
        public int Count { get; set; }

        public bool Validate(out Dictionary<string, string> errors, out string msg)
        {
            errors = new Dictionary<string, string>();
            if (Count < 1)
            {
                errors.Add("Count", LocalizationStringData.Get("ACC_VALIDATION_COUNT"));
            }
            msg = null;
            return errors.Any() == false;
        }
    }
}
