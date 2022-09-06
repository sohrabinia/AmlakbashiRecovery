using Amlakbashi.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amlakbashi.Core.DTOs.WebService.Responses.Advertises
{
    public class AdvertiseGetBasicInfoForUpdateResponse
    {
        public long residenceId { get; set; }
        public Advertise.AdvertiseType type { get; set; }
        public Advertise.PositionType locationType { get; set; }

        public static implicit operator AdvertiseGetBasicInfoForUpdateResponse(Advertise advertise)
        {
            return new AdvertiseGetBasicInfoForUpdateResponse()
            {
                residenceId = advertise.Id,
                type = advertise.TypeID,
                locationType = advertise.LocationType
            };
        }
    }
}
