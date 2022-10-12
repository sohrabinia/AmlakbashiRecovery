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
        public int PeakHolidayPrice { get; set; }
        public int ExtraCapacityPrice { get; set; }
        public long MonthlyPrice { get; set; }
        public int NowruzPrice { get; set; }
        public int NowruzExtraCapacityPrice { get; set; }

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
            if (PeakHolidayPrice < 1)
            {
                errors.Add("PeakHolidayPrice", null);
            }
            else if (PeakHolidayPrice < 30000)
            {
                errors.Add("PeakHolidayPrice", string.Format(LocalizationStringData.Get("ACC_VALIDATION_PRICE_MIN"), 30000));
            }
            if (ExtraCapacityPrice < 0)
            {
                errors.Add("ExtraCapacityPrice", null);
            }
            if (NowruzPrice > 0 && NowruzPrice < 30000)
            {
                errors.Add("NowruzPrice", string.Format(LocalizationStringData.Get("ACC_VALIDATION_PRICE_MIN"), 30000));
            }
            var anyError = errors.Any();
            msg = anyError ? LocalizationStringData.Get("ACC_VALIDATION_PRICE") : null;

            return anyError == false;
        }
    }
}
