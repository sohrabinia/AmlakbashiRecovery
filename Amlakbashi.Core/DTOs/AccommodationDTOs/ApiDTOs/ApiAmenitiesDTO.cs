using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Amlakbashi.Core.Entities.Advertise;

namespace Amlakbashi.Core.DTOs.AccommodationDTOs.ApiDTOs
{
    [Serializable]
    public class ApiAmenitiesDTO
    {
        public long id { get; set; }
        public bool group { get; set; }
        public int groupId { get; set; }
        public HeatingSystemItems heatingSystem { get; set; }
        public CoolingSystemItems coolingSystem { get; set; }
        public WCItems wc { get; set; }
        public ExtraBlanketCountItems extraBlanketCount { get; set; }
        public bool bathroom { get; set; }
        public bool pool { get; set; }
        public bool elevator { get; set; }
        public bool sauna { get; set; }
        public bool jacuzzi { get; set; }
        public bool tv { get; set; }
        public bool wifi { get; set; }
        public bool washingMachine { get; set; }
        public bool refrigerator { get; set; }
        public bool oven { get; set; }
        public bool microwaveOven { get; set; }
        public bool kitchenHood { get; set; }
        public bool kitchenUtensils { get; set; }
        public bool teaMaker { get; set; }
        public bool soundSystem { get; set; }
        public bool hairDryer { get; set; }
        public bool poolTable { get; set; }
        public bool foosball { get; set; }
        public bool golf { get; set; }
    }
}
