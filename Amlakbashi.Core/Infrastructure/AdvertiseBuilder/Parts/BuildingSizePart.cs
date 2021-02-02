using Amlakbashi.Core.Infrastructure.AdvertiseBuilder.Base;
using Amlakbashi.Core.Base.Builder;
using Amlakbashi.Core.Common.Localization;
using System.Collections.Generic;
using System.Linq;

namespace Amlakbashi.Core.Infrastructure.AdvertiseBuilder.Parts
{
    public class BuildingSizePart : IPart, IValidator
    {
        public int Metrazh { get; set; }

        public bool Validate(out Dictionary<string, string> errors, out string msg)
        {
            errors = new Dictionary<string, string>();
            if (Metrazh < 1)
            {
                errors.Add("Metrazh", LocalizationStringData.Get("ACC_VALIDATION_BUILDING_SIZE"));
            }
            msg = null;
            return errors.Any() == false;
        }
    }
}
