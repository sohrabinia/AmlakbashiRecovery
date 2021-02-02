using Amlakbashi.Core.Infrastructure.AdvertiseBuilder.Base;
using Amlakbashi.Core.Base.Builder;
using Amlakbashi.Core.Common.Localization;
using System.Collections.Generic;
using System.Linq;
using static Amlakbashi.Core.Entities.Advertise;

namespace Amlakbashi.Core.Infrastructure.AdvertiseBuilder.Parts
{
    public class FloorPart : IPart, IValidator
    {
        public FloorItems Floor { get; set; }

        public bool Validate(out Dictionary<string, string> errors, out string msg)
        {
            errors = new Dictionary<string, string>();
            if ((int)Floor < -1)
            {
                errors.Add("Floor", LocalizationStringData.Get("ACC_VALIDATION_FLOOR"));
            }
            msg = null;
            return errors.Any() == false;
        }
    }
}
