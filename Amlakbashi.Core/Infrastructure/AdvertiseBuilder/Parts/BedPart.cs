using Amlakbashi.Core.Base.Builder;
using static Amlakbashi.Core.Entities.Advertise;

namespace Amlakbashi.Core.Infrastructure.AdvertiseBuilder.Parts
{
    public class BedPart : IPart
    {
        public int SingleBedCount { get; set; }
        public int DoubleBedCount { get; set; }
        public int BlanketAndMattressCount { get; set; }
        public ExtraBlanketCountItems ExtraBlanketCount { get; set; }
    }
}
