using Amlakbashi.Core.Entities;
using MediatR;
using System.Collections.Generic;
using static Amlakbashi.Core.Entities.Advertise;
using static Amlakbashi.Core.Entities.Region;

namespace Amlakbashi.Mediator.Commands.AdvertiseCommands
{
    public class GetCategoriesFilterCommand : IRequest<List<DynamicCategory>>
    {
        public AdvertiseType Type { get; set; }
        public CountryDirection CountryDirection { get; set; }
        public int? Province { get; set; }
        public int? City { get; set; }
        public int? Area { get; set; }
        public bool Save { get; set; }
        public GetCategoriesFilterCommand(AdvertiseType type, CountryDirection countryDirection, int? province,
            int? city, int? area, bool save = true)
        {
            Type = type;
            CountryDirection = countryDirection;
            Province = province;
            City = city;
            Area = area;
            Save = save;
        }
    }
}
