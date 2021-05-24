using System;
using System.Collections.Generic;
using System.Text;
using static Amlakbashi.Core.Entities.Advertise;

namespace Amlakbashi.Core.DTOs.AccommodationDTOs.AccPagesDTOs
{
    public class PoolDTO
    {
        public bool? Pool { get; set; }
        public bool PoolHotWater { get; set; }
        public bool PoolFiltration { get; set; }
        public bool PoolOpen { get; set; }
        public bool PoolCovered { get; set; }

        public void GenerateDTO(PoolFeaturesEnum value)
        {
            //if (value.HasFlag(PoolFeaturesEnum.None))
            //{
            //    return dto;
            //}
            if (value.HasFlag(PoolFeaturesEnum.HotWater))
            {
                this.PoolHotWater = true;
            }
            if (value.HasFlag(PoolFeaturesEnum.Filtration))
            {
                this.PoolFiltration = true;
            }
            if (value.HasFlag(PoolFeaturesEnum.Open))
            {
                this.PoolOpen = true;
            }
            if (value.HasFlag(PoolFeaturesEnum.Covered))
            {
                this.PoolCovered = true;
            }
        }
    }
}
