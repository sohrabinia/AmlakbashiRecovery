using Amlakbashi.Core.Infrastructure.AdvertiseBuilder.Base;
using Amlakbashi.Core.Base.Builder;
using Amlakbashi.Core.Common.Localization;
using System.Collections.Generic;
using System.Linq;

namespace Amlakbashi.Core.Infrastructure.AdvertiseBuilder.Parts
{
    public class TitleDescPart : IPart, IValidator
    {
        [Important]
        public string Title { get; set; }

        [Important]
        public string Description { get; set; }

        public bool Validate(out Dictionary<string, string> errors, out string msg)
        {
            errors = new Dictionary<string, string>();
            if (string.IsNullOrEmpty(Title))
            {
                errors.Add("Title", LocalizationStringData.Get("ACC_VALIDATION_TITLE"));
            }
            if (string.IsNullOrEmpty(Description))
            {
                errors.Add("Description", LocalizationStringData.Get("ACC_VALIDATION_DESC"));
            }
            msg = null;
            return errors.Any() == false;
        }
    }
}
