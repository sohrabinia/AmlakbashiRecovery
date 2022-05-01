using Amlakbashi.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amlakbashi.Core.DTOs.WebService.Responses.Advertises
{
    public class AdvertiseGetBasicInfoForUpdateResponse
    {
        public long advertiseId { get; set; }
        public Advertise.AdvertiseType type { get; set; }
        public Advertise.PositionType locationType { get; set; }

        public static implicit operator AdvertiseGetBasicInfoForUpdateResponse(Advertise advertise)
        {
            return new AdvertiseGetBasicInfoForUpdateResponse()
            {
                advertiseId = advertise.Id,
                type = advertise.TypeID,
                locationType = advertise.Position
            };
        }
    }
}
