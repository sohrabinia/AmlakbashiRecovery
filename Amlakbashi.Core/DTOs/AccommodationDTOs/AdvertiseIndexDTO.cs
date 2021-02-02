using Amlakbashi.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amlakbashi.Core.DTOs.AccommodationDTOs
{
    public class AdvertiseIndexDTO
    {
        public Advertise Advertise { get; set; }
        public string UserPhoneNumber { get; set; }
        public long UserScore { get; set; }
        public string CityPersianName { get; set; }
    }
}
