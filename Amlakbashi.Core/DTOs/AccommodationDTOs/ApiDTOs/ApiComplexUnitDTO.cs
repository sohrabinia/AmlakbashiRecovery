using Amlakbashi.Core.DTOs.AccommodationDTOs.FormInputDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amlakbashi.Core.DTOs.AccommodationDTOs.ApiDTOs
{
    public class ApiComplexUnitDTO
    {
        public long id { get; set; }
        public long parentId { get; set; }
        public int userId { get; set; }
        public int Type { get; set; }
        public ApiPositionDTO position { get; set; }
        public ApiPhotoDTO photos { get; set; }
        public ApiRulesDTO rules { get; set; }
        public ApiSpecificDTO specifics { get; set; }
        public ApiAmenitiesGetDTO amenities { get; set; }
        public PriceInputDTO prices { get; set; }
    }
}
