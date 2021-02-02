using Amlakbashi.Core.Base.Builder;

namespace Amlakbashi.Core.Infrastructure.AdvertiseBuilder.Parts
{
    public class NorouzPart : IPart
    {
        public int NorouzPrice { get; set; }
        public long unixNorouzMinRequestDate { get; set; }
        public int NorouzOverCapacityPrice { get; set; }
    }
}
