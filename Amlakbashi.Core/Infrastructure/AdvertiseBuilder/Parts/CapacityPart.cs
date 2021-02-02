using Amlakbashi.Core.Infrastructure.AdvertiseBuilder.Base;
using Amlakbashi.Core.Base.Builder;
using Amlakbashi.Core.Common.Localization;
using System.Collections.Generic;
using System.Linq;

namespace Amlakbashi.Core.Infrastructure.AdvertiseBuilder.Parts
{
    public class CapacityPart : IPart, IValidator
    {
        public int Capacity { get; set; }
        public int MoreThanCapacity { get; set; }

        public bool Validate(out Dictionary<string, string> errors, out string msg)
        {
            errors = new Dictionary<string, string>();
            if (Capacity < 1)
            {
                errors.Add("Capacity", LocalizationStringData.Get("ACC_VALIDATION_CAPACITY"));
            }
            msg = null;
            return errors.Any() == false;
        }
    }
}
