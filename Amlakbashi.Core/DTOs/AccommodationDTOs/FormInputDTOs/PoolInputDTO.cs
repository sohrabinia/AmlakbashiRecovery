using System;
using System.Collections.Generic;
using System.Text;
using static Amlakbashi.Core.Entities.Advertise;

namespace Amlakbashi.Core.DTOs.AccommodationDTOs.FormInputDTOs
{
    public class PoolInputDTO
    {
        public bool PoolHotWater { get; set; }
        public bool PoolFiltration { get; set; }
        public bool PoolOpen { get; set; }
        public bool PoolCovered { get; set; }

        public PoolFeaturesEnum ConvertToEnum()
        {
            PoolFeaturesEnum feature = new PoolFeaturesEnum();
            if (PoolHotWater == false && PoolFiltration == false && PoolOpen == false && PoolCovered == false)
            {
                feature = PoolFeaturesEnum.None;
            }
            else
            {
                if (PoolHotWater)
                {
                    feature = PoolFeaturesEnum.HotWater;
                }
                if (PoolFiltration)
                {
                    feature = feature | PoolFeaturesEnum.Filtration;
                }
                if (PoolOpen)
                {
                    feature = feature | PoolFeaturesEnum.Open;
                }
                if (PoolCovered)
                {
                    feature = feature | PoolFeaturesEnum.Covered;
                }
            }
            return feature;
        }

        public void GenerateDTO(PoolFeaturesEnum value)
        {
            //if (value.HasFlag(PoolFeaturesEnum.None))
            //{
            //    return;
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
