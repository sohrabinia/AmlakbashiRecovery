using Amlakbashi.Core.Infrastructure.AdvertiseBuilder.Base;
using Amlakbashi.Core.Base.Builder;
using Amlakbashi.Core.Common.Localization;
using System.Collections.Generic;
using System.Linq;

namespace Amlakbashi.Core.Infrastructure.AdvertiseBuilder.Parts
{
    public class AddressPart : IPart, IValidator
    {
        [Important]
        public int? Province { get; set; }
        [Important]
        public int? City { get; set; }
        [Important]
        public int? Area { get; set; }
        [Important]
        public string Address { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public bool Validate(out Dictionary<string, string> errors, out string msg)
        {
            errors = new Dictionary<string, string>();
            if (Province < 1)
            {
                errors.Add("Province", LocalizationStringData.Get("ACC_VALIDATION_PROVINCE"));
            }
            if (City < 1)
            {
                errors.Add("City", LocalizationStringData.Get("ACC_VALIDATION_CITY"));
            }
            if (string.IsNullOrEmpty(Address))
            {
                errors.Add("Address", LocalizationStringData.Get("ACC_VALIDATION_ADDRESS"));
            }
            msg = null;
            return errors.Any() == false;
        }
    }
}
