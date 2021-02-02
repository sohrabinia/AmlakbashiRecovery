using Amlakbashi.Core.Base.Builder;
using Amlakbashi.Core.Common.Localization;
using Amlakbashi.Core.Infrastructure.AdvertiseBuilder.Base;
using System.Collections.Generic;
using System.Linq;

namespace Amlakbashi.Core.Infrastructure.AdvertiseBuilder.Parts
{
    public class ElevatorPart : IPart, IValidator
    {
        public bool? Elevator { get; set; }

        public bool Validate(out Dictionary<string, string> errors, out string msg)
        {
            errors = new Dictionary<string, string>();
            if (Elevator == null)
            {
                errors.Add("Elevator", null);
            }
            msg = errors.Any() ? LocalizationStringData.Get("ACC_VALIDATION_ELEVATOR") : null;
            return errors.Any() == false;
        }
    }
}
