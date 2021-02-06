using Amlakbashi.Core.Entities;
using DocumentFormat.OpenXml.Bibliography;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Amlakbashi.Core.Entities.Region;

namespace Amlakbashi.Core.Infrastructure.LocalizationHelpers
{
    public static class RegionLocalization
    {
        public static string GetAdvertiseRegionString(int region_type)
        {
            switch ((AdvertiseRegion)region_type)
            {
                case AdvertiseRegion.Province:
                    return "استان";
                case AdvertiseRegion.City:
                    return "شهر";
                case AdvertiseRegion.Area:
                    return "منطقه";
                default:
                    return "";
            }
        }

        public static string GetAccItemRegionString(string province, string city, string area, int countryDirection)
        {
            if (area != null)
            {
                return (countryDirection > 0 ?
                    GetCountryDirectionString((CountryDirection)countryDirection) + " - " : "") +
                    city + " - " + area;
            }
            return (countryDirection > 0 ?
                   GetCountryDirectionString((CountryDirection)countryDirection) + " - " : "") +
                   province + " - " + city;
        }

        public static string GetLocationString(string province, string city, string area, string countryDirection)
        {
            if (!string.IsNullOrEmpty(area))
            {
                return city + " - " + area;
            }
            return (!string.IsNullOrEmpty(countryDirection) ? countryDirection + " - " : "")
                + province + " - " + city;
        }

        public static string GetLocationUrl(Region region)
        {
            var locationString = region.Type == 0 ? "استان-" +
                region.PersianName.Trim().Replace(" ", "-") :
                    region.PersianName.Trim().Replace(" ", "-");
            var url = "/s/" + region.Id + "/" + locationString;
            return url;
        }
    }
}
