using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Amlakbashi.Core.Entities.Advertise;

namespace Amlakbashi.Core.DTOs.AccommodationDTOs.AccPagesDTOs
{
    public class AmenitiesDTO
    {
        public bool Oven { get; set; }
        public bool Refrigerator { get; set; }
        public bool KitchenHood { get; set; }
        public bool KitchenUtensils { get; set; }
        public bool TeaMaker { get; set; }
        public bool MicrowaveOven { get; set; }
        public HeatingSystemItems HeatingSystem { get; set; }
        public CoolingSystemItems CoolingSystem { get; set; }
        public bool Wifi { get; set; }
        public bool TV { get; set; }
        public bool SoundSystem { get; set; }
        public bool Golf { get; set; }
        public bool Bathroom { get; set; }
        public bool WashingMachine { get; set; }
        public bool Hairdryer { get; set; }
        public WCItems WC { get; set; }
        public EuropeanToiletTypeEnum EuropeanToiletType { get; set; }
        public bool PoolTable { get; set; }
        public bool Foosball { get; set; }
        public bool Sauna { get; set; }
        public bool Jacuzzi { get; set; }
        public bool Pool { get; set; }
        public bool Balcony { get; set; }
        public bool Filming { get; set; }
    }
}
