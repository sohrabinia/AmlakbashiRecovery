using Amlakbashi.Core.Base.Builder;
using Amlakbashi.Core.Common.Localization;
using Amlakbashi.Core.Infrastructure.AdvertiseBuilder.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amlakbashi.Core.Infrastructure.AdvertiseBuilder.Parts
{
    public class MetaTitleDescPart : IPart /*IValidator*/
    {
        [Important]
        public string MetaTitle { get; set; }

        [Important]
        public string MetaDescription { get; set; }

        public string Slug { get; set; }

        //public bool Validate(out Dictionary<string, string> errors, out string msg)
        //{
        //    errors = new Dictionary<string, string>();
        //    if (string.IsNullOrEmpty(MetaTitle))
        //    {
        //        errors.Add("MetaTitle", LocalizationStringData.Get("ACC_VALIDATION_META_TITLE"));
        //    }
        //    if (string.IsNullOrEmpty(MetaDescription))
        //    {
        //        errors.Add("MetaDescription", LocalizationStringData.Get("ACC_VALIDATION_META_DESC"));
        //    }
        //    msg = null;
        //    return errors.Any() == false;
        //}
    }
}
