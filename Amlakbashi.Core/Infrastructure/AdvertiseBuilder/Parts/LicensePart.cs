using Amlakbashi.Core.Base.Builder;
using Amlakbashi.Core.Common.Localization;
using Amlakbashi.Core.Infrastructure.AdvertiseBuilder.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Amlakbashi.Core.Infrastructure.AdvertiseBuilder.Parts
{
    public class LicensePart : IPart
    {
        public bool License { get; set; }
        public long? LicenseFileId { get; set; }
        public string LicenseNumber { get; set; }
    }
}
