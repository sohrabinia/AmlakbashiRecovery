using Amlakbashi.Core.Entities;
using Amlakbashi.Core.Infrastructure.LocalizationHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amlakbashi.Core.DTOs.AccommodationDTOs.ApiDTOs
{
    [Serializable]
    public class ApiHotelUnitDTO
    {
        public static implicit operator ApiHotelUnitDTO(Advertise advertise)
        {
            var dto = new ApiHotelUnitDTO();
            dto.id = advertise.Id;
            dto.typeId = (int)advertise.TypeID;
            dto.title = new Property<string>(advertise.Title,
                AdvertiseMainLocalization.GetPropertyTitle(Advertise.Property.Title), true);
            dto.capacity = new Property<int>(advertise.Capacity,
                AdvertiseMainLocalization.GetPropertyTitle(Advertise.Property.Capacity), true);
            dto.extraCapacity = new Property<int>(advertise.MoreThanCapacity,
                AdvertiseMainLocalization.GetPropertyTitle(Advertise.Property.MoreThanCapacity), true);
            dto.singleBed = new Property<int>(advertise.SingleBed,
                AdvertiseMainLocalization.GetPropertyTitle(Advertise.Property.SingleBed), true);
            dto.doubleBed = new Property<int>(advertise.DoublesBed,
                AdvertiseMainLocalization.GetPropertyTitle(Advertise.Property.DoublesBed), true);
            dto.blanketsAndMattresses = new Property<int>(advertise.BlanketsAndMattresses,
                AdvertiseMainLocalization.GetPropertyTitle(Advertise.Property.BlanketsAndMattresses), true);
            dto.count = new Property<int>(advertise.Count, "تعداد", true);
            dto.dailyPrice = new Property<int>(advertise.DailyPrice, "", true);
            dto.holidayPrice = new Property<int>(advertise.HolidayPrice, "", true);
            dto.pikeHolidayPrice = new Property<int>(advertise.HolidayPikePrice, "", true);
            dto.moreThanCapacityPrice = new Property<int>(advertise.MoreThanCapacityPrice, "", true);
            return dto;
        }

        public bool Validate(out List<string> errors)
        {
            bool has_error = false;
            errors = new List<string>();
            var unit_title = AdvertiseMainLocalization.GetHotelUnitTitle((Advertise.AdvertiseType)this.typeId);
            if (this.count.value < 1)
            {
                has_error = true;
                errors.Add("لطفا تعداد " + unit_title + " ها را وارد کنید");
            }
            if (this.capacity.value < 1)
            {
                has_error = true;
                errors.Add("لطفا ظرفیت را وارد کنید");
            }
            if (this.dailyPrice.value < 1)
            {
                has_error = true;
                errors.Add("لطفا قیمت روز های عادی را وارد کنید");
            }
            return !has_error;
        }

        public long id { get; set; }
        public long parentId { get; set; }
        public int typeId { get; set; }
        public Property<string> title { get; set; }
        public Property<int> capacity { get; set; }
        public Property<int> extraCapacity { get; set; }
        public Property<int> singleBed { get; set; }
        public Property<int> doubleBed { get; set; }
        public Property<int> blanketsAndMattresses { get; set; }
        public Property<int> count { get; set; }
        public Property<int> dailyPrice { get; set; }
        public Property<int> holidayPrice { get; set; }
        public Property<int> pikeHolidayPrice { get; set; }
        public Property<int> moreThanCapacityPrice { get; set; }
    }
}
