using Amlakbashi.Core.Base.Builder;
using Amlakbashi.Core.Common.Localization;
using Amlakbashi.Core.Infrastructure.AdvertiseBuilder.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Amlakbashi.Core.Infrastructure.AdvertiseBuilder.Parts
{
    public class LicensePart : IPart, IValidator
    {
        [Important]
        public bool License { get; set; }
        public long? LicenseFileId { get; set; }
        [Important]
        public string LicenseNumber { get; set; }

        public bool Validate(out Dictionary<string, string> errors, out string msg)
        {
            errors = new Dictionary<string, string>();
            if (License == true && string.IsNullOrEmpty(LicenseNumber))
            {
                errors.Add("LicenseNumber", LocalizationStringData.Get("ACC_VALIDATION_LICENSENUMBER"));
            }
            msg = null;
            return errors.Any() == false;
        }
    }
}
