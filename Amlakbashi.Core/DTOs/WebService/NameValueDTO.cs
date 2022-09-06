using Amlakbashi.Core.Entities;
using Amlakbashi.Core.Infrastructure.LocalizationHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Amlakbashi.Core.DTOs.WebService
{
    public class NameValueDTO
    {
        public NameValueDTO(string name, int value)
        {
            this.name = name;
            this.value = value;
        }

        public string name { get; set; }
        public int value { get; set; }

        public enum EnumType
        {
            keyValueType = 0,
            residenceType = 1,
            residenceLocationType = 2,
            residenceCoolingSystem = 3,
            residenceHeatingSystem = 4,
            residenceWCType = 5,
            residenceOwnershipType = 6,
            residenceParking = 7,
            residenceFloor = 8,
            residenceExtraBlanket = 9
        }

        public static IList<NameValueDTO> GetEnumNameValues(EnumType enumType)
        {
            switch (enumType)
            {
                case EnumType.keyValueType:
                    return GenerateNameValueList<EnumType>();
                case EnumType.residenceType:
                    return GenerateNameValueList<Advertise.AdvertiseType>();
                case EnumType.residenceLocationType:
                    return GenerateNameValueList<Advertise.PositionType>();
                case EnumType.residenceCoolingSystem:
                    return GenerateNameValueList<Advertise.CoolingSystemItems>();
                case EnumType.residenceHeatingSystem:
                    return GenerateNameValueList<Advertise.HeatingSystemItems>();
                case EnumType.residenceWCType:
                    return GenerateNameValueList<Advertise.WCItems>();
                case EnumType.residenceOwnershipType:
                    return GenerateNameValueList<Advertise.OwnershipTypeEnum>();
                case EnumType.residenceParking:
                    return GenerateNameValueList<Advertise.ParkingItems>();
                case EnumType.residenceFloor:
                    return GenerateNameValueList<Advertise.FloorItems>();
                case EnumType.residenceExtraBlanket:
                    return GenerateNameValueList<Advertise.ExtraBlanketCountItems>();
                default:
                    return null;
            }
        }

        private static IList<NameValueDTO> GenerateNameValueList<T>() where T : Enum
        {
            var array = (Enum.GetValues(typeof(T)) as T[]).OrderBy(x => (int)Enum.Parse(typeof(T), x.ToString())).ToArray();
            var result = new List<NameValueDTO>();
            foreach (var item in array)
            {
                result.Add(new NameValueDTO(AdvertiseMainLocalization.GetEnumPersianName(item),
                    (int)Enum.Parse(typeof(T), item.ToString())));
            }
            return result;
        }
    }
}
