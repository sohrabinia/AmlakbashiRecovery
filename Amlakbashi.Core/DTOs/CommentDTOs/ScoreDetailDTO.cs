using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amlakbashi.Core.DTOs.CommentDTOs
{
    [Serializable]
    public class ScoreDetailDTO
    {
        public ScoreDetailDTO(float tidiness, float hostBehaviour,
            float position, float infoCorrectness, float safety,
            float priceWorth)
        {
            this.tidiness = tidiness;
            this.hostBehaviour = hostBehaviour;
            this.position = position;
            this.infoCorrectness = infoCorrectness;
            this.safety = safety;
            this.priceWorth = priceWorth;
        }

        public float tidiness { get; set; }
        public float hostBehaviour { get; set; }
        public float position { get; set; }
        public float infoCorrectness { get; set; }
        public float safety { get; set; }
        public float priceWorth { get; set; }
    }
}
