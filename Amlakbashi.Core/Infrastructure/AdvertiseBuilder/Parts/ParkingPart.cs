using Amlakbashi.Core.Base.Builder;
using Amlakbashi.Core.Common.Localization;
using Amlakbashi.Core.Infrastructure.AdvertiseBuilder.Base;
using System.Collections.Generic;
using System.Linq;
using static Amlakbashi.Core.Entities.Advertise;

namespace Amlakbashi.Core.Infrastructure.AdvertiseBuilder.Parts
{
    public class ParkingPart : IPart, IValidator
    {
        public ParkingItems Parking { get; set; }

        public bool Validate(out Dictionary<string, string> errors, out string msg)
        {
            errors = new Dictionary<string, string>();
            if (Parking == ParkingItems.Unset)
            {
                errors.Add("Parking", null);
            }
            msg = errors.Any() ? LocalizationStringData.Get("ACC_VALIDATION_PARKING") : null;
            return errors.Any() == false;
        }
    }
}
