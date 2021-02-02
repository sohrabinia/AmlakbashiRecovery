using Amlakbashi.Core.Entities;
using static Amlakbashi.Core.Entities.Region;

namespace Amlakbashi.Core.Infrastructure.LocalizationHelpers
{
    public static class CategoryUrlLocalization
    {
        public static string CategoryToUrl(DynamicCategory category)
        {
            string partOne;
            if (category.Province != null)
            {
                if (category.Area != null)
                {
                    partOne = "/s/" + category.Area + "/" + category.RegionString.Replace("  ", " ").Replace(" ", "-");
                }
                else if (category.City != null)
                {
                    partOne = "/s/" + category.City + "/" + category.RegionString.Replace("  ", " ").Replace(" ", "-");
                }
                else
                {
                    partOne = "/s/" + category.Province + "/" + category.RegionString.Replace("  ", " ").Replace(" ", "-");
                }
            }
            else
            {
                if (category.CountryDirection == Region.CountryDirection.North)
                {
                    partOne = "/شمال";
                }
                else
                {
                    partOne = "/ایران";
                }
            }
            string partTwo = category.Type == Advertise.AdvertiseType.All ? "" : "/" + AdvertiseUrlLocalization.AdvertiseTypeToUrlString((int)category.Type);
            return partOne + partTwo;
        }

        public static string RegionToUrl(CountryDirection countryDirection = CountryDirection.Unset,
            Region province = null, Region city = null, Region area = null)
        {
            string url;
            if (province != null)
            {
                if (area != null)
                {
                    url = "/s/" + area.Id + "/" + area.PersianName.Replace("  ", " ").Replace(" ", "-");
                }
                else if (city != null)
                {
                    url = "/s/" + city.Id + "/" + city.PersianName.Replace("  ", " ").Replace(" ", "-");
                }
                else
                {
                    url = "/s/" + province.Id + "/استان-" + province.PersianName.Replace("  ", " ").Replace(" ", "-");
                    url = url.Replace("--", "-");
                }
            }
            else
            {
                if (countryDirection == Region.CountryDirection.North)
                {
                    url = "/شمال";
                }
                else
                {
                    url = "/ایران";
                }
            }
            return url;
        }
    }
}
