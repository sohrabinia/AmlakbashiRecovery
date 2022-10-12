using Amlakbashi.Core.Base.Builder;

namespace Amlakbashi.Core.Infrastructure.AdvertiseBuilder.Parts
{
    public class NorouzPart : IPart
    {
        public int NowruzPrice { get; set; }
        public long unixNorouzMinRequestDate { get; set; }
        public int MinReserveDateForNowruz { get; set; }
    }
}
