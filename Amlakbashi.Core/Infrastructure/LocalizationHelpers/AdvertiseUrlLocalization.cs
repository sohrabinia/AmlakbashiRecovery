using System;
using System.IO;
using static Amlakbashi.Core.Entities.Advertise;

namespace Amlakbashi.Core.Infrastructure.LocalizationHelpers
{
    public static class AdvertiseUrlLocalization
    {
        public static string GetAdvertiseTypeUrlString(AdvertiseType type)
        {
            switch (type)
            {
                case AdvertiseType.All:
                    return "اجاره-روزانه";
                case AdvertiseType.Apartment:
                    return "آپارتمان";
                case AdvertiseType.Villa:
                    return "ویلایی";
                case AdvertiseType.Hotel:
                    return "رزرو-هتل";
                case AdvertiseType.SuitAndRoom:
                    return "اتاق-سوئیت-مبله";
                case AdvertiseType.House:
                    return "خانه-مبله";
                case AdvertiseType.Camp:
                    return "کمپ";
                case AdvertiseType.TourismAccommodation:
                    return "بومگردی";
                case AdvertiseType.HotelApartment:
                    return "هتل-آپارتمان";
                case AdvertiseType.Inn:
                    return "رزرو-مسافرخانه";
                case AdvertiseType.Pansion:
                    return "پانسیون";
                case AdvertiseType.Complex:
                    return "مجتمع";
                case AdvertiseType.Hut:
                    return "کلبه";
                default:
                    return "";
            }
        }

        public static string AdvertiseTypeToUrlString(int type)
        {
            switch (type)
            {
                case 82:
                case 1:
                case 8:
                    return "آپارتمان";
                case 83:
                case 2:
                case 9:
                    return "ویلا";
                case 87:
                case 6:
                case 5:
                case 7:
                    return "رزرو-هتل";
                case 4:
                case 3:
                    return "بومگردی";
                default:
                    return "";
            }
        }

        public static string GetCategoryUrl(AdvertiseType type, string province, string city,
            string country_direction_string)
        {
            var location_string = AdvertiseMainLocalization.GetLocationString(province, city, null, country_direction_string);
            return GetAdvertiseTypeUrlString(type) + "-" + location_string.Replace(" ", "-");
        }

        public static string SlugToAdvertiseUrl(string slug)
        {
            return string.Format("/{0}/{1}", "اجاره-روزانه", slug);
        }

        public static string GetOldSlug(string accTitle, int accType)
        {
            try
            {
                string title = "";
                if (string.IsNullOrEmpty(accTitle))
                {
                    var tmpTitle = "اجاره روزانه " + AdvertiseMainLocalization.GetAdvertiseTypePersianString(accType);
                    title = tmpTitle;
                }
                else
                {
                    title = accTitle;
                }

                title = title.Replace(" ", "-");
                string invalid = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars());
                invalid = invalid + ".+*&^%$#@!";
                foreach (char c in invalid)
                {
                    title = title.Replace(c.ToString(), "");
                }

                return title;
            }
            catch
            {
                return "";
            }
        }
    }
}
