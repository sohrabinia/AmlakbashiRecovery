using Amlakbashi.Core.Infrastructure.AdvertiseBuilder.Base;
using Amlakbashi.Core.Base.Builder;
using Amlakbashi.Core.Common.Localization;
using System.Collections.Generic;
using System.Linq;
using static Amlakbashi.Core.Entities.Advertise;

namespace Amlakbashi.Core.Infrastructure.AdvertiseBuilder.Parts
{
    public class AmenitiesPart : IPart, IValidator
    {
        public bool? Oven { get; set; }
        public bool? Refrigerator { get; set; }
        public bool? KitchenHood { get; set; }
        public bool? KitchenUtensils { get; set; }
        public bool? TeaMaker { get; set; }
        public bool? MicrowaveOven { get; set; }
        public HeatingSystemItems HeatingSystem { get; set; }
        public CoolingSystemItems CoolingSystem { get; set; }
        public bool? Wifi { get; set; }
        public bool? TV { get; set; }
        public bool? SoundSystem { get; set; }
        public bool? Golf { get; set; }
        public bool? Bathroom { get; set; }
        public bool? WashingMachine { get; set; }
        public bool? Hairdryer { get; set; }
        public WCItems WC { get; set; }
        public bool? PoolTable { get; set; }
        public bool? Foosball { get; set; }
        public bool? Sauna { get; set; }
        public bool? Jacuzzi { get; set; }
        public bool? Pool { get; set; }
        public PoolFeaturesEnum PoolFeatures { get; set; }

        public bool Validate(out Dictionary<string, string> errors, out string msg)
        {
            errors = new Dictionary<string, string>();
            if ((int)WC < 1)
            {
                errors.Add("WC", null);
            }
            if ((int)HeatingSystem < 1)
            {
                errors.Add("HeatingSystem", null);
            }
            if ((int)CoolingSystem < 1)
            {
                errors.Add("CoolingSystem", null);
            }
            if (Oven == null)
            {
                errors.Add("Oven", null);
            }
            if (Refrigerator == null)
            {
                errors.Add("Refrigerator", null);
            }
            if (KitchenHood == null)
            {
                errors.Add("KitchenHood", null);
            }
            if (KitchenUtensils == null)
            {
                errors.Add("KitchenUtensils", null);
            }
            if (TeaMaker == null)
            {
                errors.Add("TeaMaker", null);
            }
            if (MicrowaveOven == null)
            {
                errors.Add("MicrowaveOven", null);
            }
            if (Wifi == null)
            {
                errors.Add("Wifi", null);
            }
            if (TV == null)
            {
                errors.Add("TV", null);
            }
            if (SoundSystem == null)
            {
                errors.Add("SoundSystem", null);
            }
            if (Golf == null)
            {
                errors.Add("Golf", null);
            }
            if (Bathroom == null)
            {
                errors.Add("Bathroom", null);
            }
            if (WashingMachine == null)
            {
                errors.Add("WashingMachine", null);
            }
            if (Hairdryer == null)
            {
                errors.Add("Hairdryer", null);
            }
            if (Pool == null)
            {
                errors.Add("Pool", null);
            }
            if (PoolTable == null)
            {
                errors.Add("PoolTable", null);
            }
            if (Sauna == null)
            {
                errors.Add("Sauna", null);
            }
            if (Jacuzzi == null)
            {
                errors.Add("Jacuzzi", null);
            }
            if (Foosball == null)
            {
                errors.Add("Foosball", null);
            }

            msg = errors.Any() ? LocalizationStringData.Get("ACC_VALIDATION_AMENITIES") : null;

            return errors.Any() == false;
        }
    }
}
