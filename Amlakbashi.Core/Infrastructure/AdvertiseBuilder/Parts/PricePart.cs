using Amlakbashi.Core.Infrastructure.AdvertiseBuilder.Base;
using Amlakbashi.Core.Base.Builder;
using Amlakbashi.Core.Common.Localization;
using System.Collections.Generic;
using System.Linq;
using System;
using Amlakbashi.Core.Entities;

namespace Amlakbashi.Core.Infrastructure.AdvertiseBuilder.Parts
{
    public class PricePart : IPart, IValidator
    {
        public int DailyPrice { get; set; }
        public int HolidayPrice { get; set; }
        public int HolidayPikePrice { get; set; }
        public int MoreThanCapacityPrice { get; set; }
        public long RentPrice { get; set; }
        public int NorouzPrice { get; set; }
        public int NorouzOverCapacityPrice { get; set; }

        public bool Validate(out Dictionary<string, string> errors, out string msg)
        {
            errors = new Dictionary<string, string>();
            if (DailyPrice < 1)
            {
                errors.Add("DailyPrice", null);
            }
            else if (DailyPrice < 30000)
            {
                errors.Add("DailyPrice", string.Format(LocalizationStringData.Get("ACC_VALIDATION_PRICE_MIN"), 30000));
            }
            if (HolidayPrice < 1)
            {
                errors.Add("HolidayPrice", null);
            }
            else if (HolidayPrice < 30000)
            {
                errors.Add("HolidayPrice", string.Format(LocalizationStringData.Get("ACC_VALIDATION_PRICE_MIN"), 30000));
            }
            if (HolidayPikePrice < 1)
            {
                errors.Add("HolidayPikePrice", null);
            }
            else if (HolidayPikePrice < 30000)
            {
                errors.Add("HolidayPikePrice", string.Format(LocalizationStringData.Get("ACC_VALIDATION_PRICE_MIN"), 30000));
            }
            if (MoreThanCapacityPrice < 0)
            {
                errors.Add("MoreThanCapacityPrice", null);
            }
            var anyError = errors.Any();
            msg = anyError ? LocalizationStringData.Get("ACC_VALIDATION_PRICE") : null;

            return anyError == false;
        }
    }
}
