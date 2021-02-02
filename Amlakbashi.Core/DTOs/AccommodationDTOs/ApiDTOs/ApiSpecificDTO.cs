using Amlakbashi.Core.Entities;
using Amlakbashi.Core.Infrastructure.LocalizationHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Amlakbashi.Core.Entities.Advertise;

namespace Amlakbashi.Core.DTOs.AccommodationDTOs.ApiDTOs
{
    [Serializable]
    public class ApiSpecificDTO
    {
        public static implicit operator ApiSpecificDTO(Advertise advertise)
        {
            var dto = new ApiSpecificDTO();
            dto.id = advertise.Id;
            dto.title = new Property<string>(advertise.Title,
                AdvertiseMainLocalization.GetPropertyTitle(Advertise.Property.Title), true);
            dto.description = new Property<string>(advertise.Description,
                AdvertiseMainLocalization.GetPropertyTitle(Advertise.Property.Description), true);
            dto.area = new Property<int>(advertise.Metrazh,
                AdvertiseMainLocalization.GetPropertyTitle(Advertise.Property.Metrazh), true);
            dto.capacity = new Property<int>(advertise.Capacity,
                AdvertiseMainLocalization.GetPropertyTitle(Advertise.Property.Capacity), true);
            dto.extraCapacity = new Property<int>(advertise.MoreThanCapacity,
                AdvertiseMainLocalization.GetPropertyTitle(Advertise.Property.MoreThanCapacity), true);
            dto.room = new Property<int>(advertise.Room,
                AdvertiseMainLocalization.GetPropertyTitle(Advertise.Property.Room), true);
            dto.parking = new Property<int>((int)advertise.Parking,
                AdvertiseMainLocalization.GetPropertyTitle(Advertise.Property.Parking), true);
            dto.floor = new Property<int>((int)advertise.Floor,
                AdvertiseMainLocalization.GetPropertyTitle(Advertise.Property.Floor), true);
            dto.singleBed = new Property<int>(advertise.SingleBed,
                AdvertiseMainLocalization.GetPropertyTitle(Advertise.Property.SingleBed), true);
            dto.doubleBed = new Property<int>(advertise.DoublesBed,
                AdvertiseMainLocalization.GetPropertyTitle(Advertise.Property.DoublesBed), true);
            dto.landArea = new Property<int>(advertise.LandArea,
                AdvertiseMainLocalization.GetPropertyTitle(Advertise.Property.LandArea), true);
            return dto;
        }

        public static void CopyToAdvertise(ApiSpecificDTO dto, Advertise advertise)
        {
            advertise.Id = dto.id;
            advertise.Title = dto.title.value;
            advertise.Description = dto.description.value;
            advertise.Metrazh = dto.area.value;
            advertise.Capacity = dto.capacity.value;
            advertise.MoreThanCapacity = dto.extraCapacity.value;
            advertise.Room = dto.room.value;
            advertise.Parking = (ParkingItems)dto.parking.value;
            advertise.Floor = (FloorItems)dto.floor.value;
            advertise.SingleBed = dto.singleBed.value;
            advertise.DoublesBed = dto.doubleBed.value;
            advertise.LandArea = dto.landArea.value;
        }

        public bool Validate(bool hasChild, out List<string> errors)
        {
            bool has_error = false;
            errors = new List<string>();
            if (string.IsNullOrEmpty(this.title.value))
            {
                has_error = true;
                errors.Add("عنوان اقامتگاه را بنویسید");
            }
            if (string.IsNullOrEmpty(this.description.value))
            {
                has_error = true;
                errors.Add("توضیحات اقامتگاه را بنویسید");
            }
            if (hasChild)
            {
                if (this.floor.value < -1)
                {
                    has_error = true;
                    errors.Add("لطفا طبقه را وارد کنید");
                }
                if (this.area.value < 1)
                {
                    has_error = true;
                    errors.Add("لطفا متراژ بنا را وارد کنید");
                }
                if (this.capacity.value < 1)
                {
                    has_error = true;
                    errors.Add("لطفا ظرفیت را وارد کنید");
                }
                if (this.room.value < 1)
                {
                    has_error = true;
                    errors.Add("لطفا تعداد اتاق خواب را وارد کنید");
                }
                if (this.parking.value < 1)
                {
                    has_error = true;
                    errors.Add("لطفا تعداد پارکینگ را وارد کنید");
                }
            }
            return !has_error;
        }

        public long id { get; set; }
        public bool group { get; set; }
        public int groupId { get; set; }
        public Property<string> title { get; set; }
        public Property<string> description { get; set; }
        public Property<int> area { get; set; }
        public Property<int> capacity { get; set; }
        public Property<int> extraCapacity { get; set; }
        public Property<int> room { get; set; }
        public Property<int> parking { get; set; }
        public List<SelectItem> parkingSelectItem { get; set; }
        public Property<int> floor { get; set; }
        public List<SelectItem> floorSelectItem { get; set; }
        public Property<int> singleBed { get; set; }
        public Property<int> doubleBed { get; set; }
        public Property<int> landArea { get; set; }
    }
}
