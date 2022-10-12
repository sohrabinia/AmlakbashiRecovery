using Amlakbashi.Core.Base.Builder;
using Amlakbashi.Core.Common.Localization;
using Amlakbashi.Core.Entities;
using Amlakbashi.Core.Infrastructure.AdvertiseBuilder.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Amlakbashi.Core.Infrastructure.AdvertiseBuilder.Parts
{
    public class VillaTypePart : IPart, IValidator
    {
        [Important]
        public Advertise.VillaTypeEnum VillaType { get; set; }

        public bool Validate(out Dictionary<string, string> errors, out string msg)
        {
            errors = new Dictionary<string, string>();
            if (VillaType == Advertise.VillaTypeEnum.Unset)
            {
                errors.Add("VillaType", LocalizationStringData.Get("ACC_VALIDATION_VILLATYPE"));
            }
            msg = null;
            return errors.Any() == false;
        }
    }
}
