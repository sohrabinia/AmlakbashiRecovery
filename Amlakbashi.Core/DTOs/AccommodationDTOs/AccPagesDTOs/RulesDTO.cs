using Amlakbashi.Core.Base.Builder;

namespace Amlakbashi.Core.DTOs.AccommodationDTOs.AccPagesDTOs
{
    public class RulesDTO
    {
        public bool AllowParty { get; set; }
        public bool AllowPets { get; set; }
        public bool AllowSmoking { get; set; }
        public string EvidenceRequired { get; set; }
        public string OtherRules { get; set; }
    }
}
