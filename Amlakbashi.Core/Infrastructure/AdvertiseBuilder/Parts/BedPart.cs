using Amlakbashi.Core.Base.Builder;
using static Amlakbashi.Core.Entities.Advertise;

namespace Amlakbashi.Core.Infrastructure.AdvertiseBuilder.Parts
{
    public class BedPart : IPart
    {
        public int SingleBed { get; set; }
        public int DoublesBed { get; set; }
        public int BlanketsAndMattresses { get; set; }
        public ExtraBlanketCountItems ExtraBlanketCount { get; set; }
    }
}
