using Amlakbashi.Core.Entities;
using System;
using static Amlakbashi.Core.Entities.Advertise;

namespace Amlakbashi.Core.Infrastructure.LocalizationHelpers
{
    public static class AdvertiseSeoLocalization
    {
        public static string GetTitle(int advertiseType)
        {
            switch ((AdvertiseType)advertiseType)
            {
                case AdvertiseType.All:
                    return "اجاره روزانه ویلا، سوئیت و آپارتمان مبله";
                case AdvertiseType.Apartment:
                    return "اجاره روزانه آپارتمان مبله";
                case AdvertiseType.Villa:
                    return "اجاره روزانه ویلا";
                case AdvertiseType.Hotel:
                    return "رزرو هتل";
                case AdvertiseType.HotelApartment:
                    return "رزرو هتل آپارتمان";
                case AdvertiseType.Camp:
                    return "رزرو کمپ";
                case AdvertiseType.TourismAccommodation:
                    return "رزرو اقامتگاه بومگردی";
                case AdvertiseType.House:
                    return "اجاره روزانه خانه ویلایی مبله";
                case AdvertiseType.SuitAndRoom:
                    return "اجاره روزانه اتاق و سوئیت مبله";
                case AdvertiseType.Inn:
                    return "رزرو مسافرخانه";
                case AdvertiseType.Pansion:
                    return "اجاره روزانه پانسیون";
                case AdvertiseType.Complex:
                    return "اجاره روزانه مجتمع";
                case AdvertiseType.Hut:
                    return "اجاره روزانه کلبه";
                case AdvertiseType.None:
                    return "انتخاب نوع اقامتگاه";
                default:
                    return "";
            }
        }

        public static string GetTitle(int mostAccType, int advertiseType, string province, string city,
            string area, string country_direction_string)
        {
            if (string.IsNullOrEmpty(province) && string.IsNullOrEmpty(city) && string.IsNullOrEmpty(area) && string.IsNullOrEmpty(country_direction_string))
                return GetTitle(advertiseType);
            var location_string = AdvertiseMainLocalization.GetLocationString(province, city, area, country_direction_string);
            string output;
            if (mostAccType > 0 && advertiseType == 81)
            {
                switch (mostAccType)
                {
                    case 82:
                        //output = " اجاره روزانه خانه ، سوئیت و آپارتمان مبله در " + location_string;
                        output = "اجاره روزانه، هفتگی و ماهانه خانه، آپارتمان مبله و سوئیت در " + location_string;
                        break;
                    case 83:
                        //output = "اجاره روزانه ویلا و سوئیت در " + location_string;
                        output = "اجاره ویلا و سوئیت در " + location_string;
                        break;
                    default:
                        output = "اجاره روزانه ویلا، سوئیت و آپارتمان مبله در " + location_string;
                        break;
                }
            }
            else
            {
                switch ((AdvertiseType)advertiseType)
                {
                    case AdvertiseType.All:
                        output = "اجاره روزانه ویلا، سوئیت و آپارتمان مبله در " + location_string;
                        break;
                    case AdvertiseType.Apartment:
                        output = "اجاره روزانه آپارتمان مبله " + location_string;
                        break;
                    case AdvertiseType.Villa:
                        output = "اجاره ویلا " + location_string;
                        break;
                    case AdvertiseType.Hotel:
                        output = "رزرو هتل " + location_string;
                        break;
                    case AdvertiseType.HotelApartment:
                        output = "رزرو هتل آپارتمان " + location_string;
                        break;
                    case AdvertiseType.Camp:
                        output = "رزرو کمپ " + location_string;
                        break;
                    case AdvertiseType.TourismAccommodation:
                        output = "رزرو اقامتگاه بومگردی " + location_string;
                        break;
                    case AdvertiseType.House:
                        output = "اجاره روزانه خانه ویلایی مبله " + location_string;
                        break;
                    case AdvertiseType.SuitAndRoom:
                        output = "اجاره روزانه اتاق و سوئیت مبله " + location_string;
                        break;
                    case AdvertiseType.Inn:
                        output = "رزرو مسافرخانه " + location_string;
                        break;
                    case AdvertiseType.Pansion:
                        output = "اجاره روزانه پانسیون " + location_string;
                        break;
                    case AdvertiseType.Complex:
                        output = "اجاره روزانه مجتمع " + location_string;
                        break;
                    case AdvertiseType.Hut:
                        output = "اجاره روزانه کلبه " + location_string;
                        break;
                    default:
                        return "";
                }
            }
            return output;
        }

        public static string GetMetaTitle(int mostAccType, int advertiseType, string province, string city,
            string area, string country_direction_string)
        {
            var output = GetTitle(mostAccType, advertiseType, province, city, area,
                country_direction_string);
            //if (mostAccType > 0 && advertiseType == 81)
            //{
            //    var hasCountString = countAdvertise >= 10;
            //    if (hasCountString)
            //    {
            //        if (countAdvertise > 20)
            //        {
            //            countAdvertise = ((int)Math.Round(countAdvertise / 10.0)) * 10;
            //        }
            //        output += " - " + countAdvertise + " خانه";
            //    }
            //    if (minPrice > 0 && output.Length <= 48)
            //    {
            //        if (!hasCountString)
            //        {
            //            output += " -";
            //        }
            //        output += " قیمت از " + PriceUtility.PriceToSpecialString(minPrice);
            //    }
            //}
            if (mostAccType == 83 && advertiseType == 81)
            {
                output = output + " | روزانه، هفتگی و ماهانه";
            }
            return output + " | املاک باشی";
        }

        public static string GetKeywords(int advertiseType)
        {
            var regionList = (PositionType[])Enum.GetValues(typeof(PositionType));
            var str = "";
            switch ((AdvertiseType)advertiseType)
            {
                case AdvertiseType.All:
                    return GetKeywords((int)AdvertiseType.Apartment) + "," + GetKeywords((int)AdvertiseType.Villa) + "," + GetKeywords((int)AdvertiseType.Hotel);
                case AdvertiseType.Apartment:
                    str = "اجاره آپارتمان مبله، اجاره روزانه هفتگی ماهانه و سالانه خانه، اجاره روزانه هفتگی ماهانه و سالانه منزل، اجاره روزانه هفتگی ماهانه و سالانه سوئیت";
                    foreach (var item in regionList)
                    {
                        if (item != PositionType.none)
                            str += ("،اجاره روزانه آپارتمان مبله " + AdvertiseMainLocalization.GetPositionTypeString((int)item));
                    }
                    return str;
                case AdvertiseType.Villa:
                    str = "اجاره ویلا، اجاره روزانه ویلا";
                    foreach (var item in regionList)
                    {
                        if (item != PositionType.none)
                            str += ("،ویلا " + AdvertiseMainLocalization.GetPositionTypeString((int)item));
                    }
                    str += "ویلا استخردار";
                    return str;
                case AdvertiseType.Hotel:
                    str = "رزرو هتل";
                    foreach (var item in regionList)
                    {
                        if (item != PositionType.none)
                            str += ("،رزرو هتل " + AdvertiseMainLocalization.GetPositionTypeString((int)item));
                    }
                    return str;
                case AdvertiseType.HotelApartment:
                    str = "رزرو هتل آپارتمان";
                    foreach (var item in regionList)
                    {
                        if (item != PositionType.none)
                            str += ("،رزرو هتل آپارتمان " + AdvertiseMainLocalization.GetPositionTypeString((int)item));
                    }
                    return str;
                case AdvertiseType.Camp:
                    str = "رزرو کمپ";
                    foreach (var item in regionList)
                    {
                        if (item != PositionType.none)
                            str += ("،رزرو کمپ " + AdvertiseMainLocalization.GetPositionTypeString((int)item));
                    }
                    return str;
                case AdvertiseType.TourismAccommodation:
                    str = "رزرو اقامتگاه بومگردی";
                    foreach (var item in regionList)
                    {
                        if (item != PositionType.none)
                            str += ("،اقامتگاه بومگردی " + AdvertiseMainLocalization.GetPositionTypeString((int)item));
                    }
                    return str;
                case AdvertiseType.House:
                    str = "اجاره خانه ویلایی، اجاره روزانه هفتگی ماهانه و سالانه خانه، اجاره روزانه هفتگی ماهانه و سالانه منزل، اجاره روزانه هفتگی ماهانه و سالانه خانه ویلایی";
                    foreach (var item in regionList)
                    {
                        if (item != PositionType.none)
                            str += ("،اجاره روزانه خانه ویلایی " + AdvertiseMainLocalization.GetPositionTypeString((int)item));
                    }
                    return str;
                case AdvertiseType.SuitAndRoom:
                    str = "اجاره روزانه اتاق و سوئیت";
                    foreach (var item in regionList)
                    {
                        if (item != PositionType.none)
                            str += ("،اجاره روزانه اتاق و سوئیت " + AdvertiseMainLocalization.GetPositionTypeString((int)item));
                    }
                    return str;
                case AdvertiseType.Inn:
                    str = "رزرو مسافرخانه";
                    foreach (var item in regionList)
                    {
                        if (item != PositionType.none)
                            str += ("،رزرو مسافرخانه " + AdvertiseMainLocalization.GetPositionTypeString((int)item));
                    }
                    return str;
                case AdvertiseType.Pansion:
                    str = "اجاره روزانه پانسیون";
                    foreach (var item in regionList)
                    {
                        if (item != PositionType.none)
                            str += ("،اجاره روزانه پانسیون " + AdvertiseMainLocalization.GetPositionTypeString((int)item));
                    }
                    return str;
                case AdvertiseType.Complex:
                    str = "اجاره روزانه مجتمع";
                    foreach (var item in regionList)
                    {
                        if (item != PositionType.none)
                            str += ("،اجاره روزانه مجتمع " + AdvertiseMainLocalization.GetPositionTypeString((int)item));
                    }
                    return str;
                case AdvertiseType.Hut:
                    str = "اجاره روزانه کلبه";
                    foreach (var item in regionList)
                    {
                        if (item != PositionType.none)
                            str += ("،اجاره روزانه کلبه " + AdvertiseMainLocalization.GetPositionTypeString((int)item));
                    }
                    return str;
                default:
                    return "";
            }
        }

        public static string GetKeywords(int advertiseType, string province, string city, string area,
            string country_direction_string)
        {
            if (string.IsNullOrEmpty(area) && string.IsNullOrEmpty(city) && string.IsNullOrEmpty(province))
                return GetKeywords(advertiseType);
            var location_string = AdvertiseMainLocalization.GetLocationString(province, city, area, country_direction_string);
            var regionList = (PositionType[])Enum.GetValues(typeof(PositionType));
            var str = "";
            switch ((AdvertiseType)advertiseType)
            {
                case AdvertiseType.All:
                    return GetKeywords((int)AdvertiseType.Apartment) + "،" + GetKeywords((int)AdvertiseType.Villa) + "،" + GetKeywords((int)AdvertiseType.Hotel);
                case AdvertiseType.Apartment:
                    str = string.Format("اجاره آپارتمان مبله {0}،اجاره روزانه، هفتگی، ماهانه و سالانه خانه {0}، اجاره روزانه، هفتگی، ماهانه و سالانه منزل {0}", location_string);
                    foreach (var item in regionList)
                    {
                        if (item != PositionType.none)
                            str += ("،اجاره روزانه آپارتمان مبله " + location_string + " " + AdvertiseMainLocalization.GetPositionTypeString((int)item));
                    }
                    return str;
                case AdvertiseType.Villa:
                    str = "اجاره ویلا، اجاره روزانه ویلا";
                    foreach (var item in regionList)
                    {
                        if (item != PositionType.none)
                            str += ("،ویلا " + location_string + " " + AdvertiseMainLocalization.GetPositionTypeString((int)item));
                    }
                    str += "ویلا استخردار";
                    return str;
                case AdvertiseType.Hotel:
                    str = "رزرو هتل";
                    foreach (var item in regionList)
                    {
                        if (item != PositionType.none)
                            str += ("،رزرو هتل " + location_string + " " + AdvertiseMainLocalization.GetPositionTypeString((int)item));
                    }
                    return str;
                case AdvertiseType.HotelApartment:
                    str = "رزرو هتل آپارتمان";
                    foreach (var item in regionList)
                    {
                        if (item != PositionType.none)
                            str += ("،رزرو هتل آپارتمان " + location_string + AdvertiseMainLocalization.GetPositionTypeString((int)item));
                    }
                    return str;
                case AdvertiseType.Camp:
                    str = "رزرو کمپ";
                    foreach (var item in regionList)
                    {
                        if (item != PositionType.none)
                            str += ("،رزرو کمپ " + location_string + AdvertiseMainLocalization.GetPositionTypeString((int)item));
                    }
                    return str;
                case AdvertiseType.TourismAccommodation:
                    str = "رزرو اقامتگاه بومگردی";
                    foreach (var item in regionList)
                    {
                        if (item != PositionType.none)
                            str += ("،اقامتگاه بومگردی " + location_string + AdvertiseMainLocalization.GetPositionTypeString((int)item));
                    }
                    return str;
                case AdvertiseType.House:
                    str = "اجاره خانه ویلایی، اجاره روزانه هفتگی ماهانه و سالانه خانه، اجاره روزانه هفتگی ماهانه و سالانه منزل،اجاره روزانه هفتگی ماهانه و سالانه خانه ویلایی";
                    foreach (var item in regionList)
                    {
                        if (item != PositionType.none)
                            str += ("،اجاره روزانه خانه ویلایی " + location_string + AdvertiseMainLocalization.GetPositionTypeString((int)item));
                    }
                    return str;
                case AdvertiseType.SuitAndRoom:
                    str = "اجاره روزانه اتاق و سوئیت";
                    foreach (var item in regionList)
                    {
                        if (item != PositionType.none)
                            str += ("،اجاره روزانه اتاق و سوئیت " + location_string + AdvertiseMainLocalization.GetPositionTypeString((int)item));
                    }
                    return str;
                case AdvertiseType.Inn:
                    str = "رزرو مسافرخانه";
                    foreach (var item in regionList)
                    {
                        if (item != PositionType.none)
                            str += ("،رزرو مسافرخانه " + location_string + AdvertiseMainLocalization.GetPositionTypeString((int)item));
                    }
                    return str;
                case AdvertiseType.Pansion:
                    str = "اجاره روزانه پانسیون";
                    foreach (var item in regionList)
                    {
                        if (item != PositionType.none)
                            str += ("،اجاره روزانه پانسیون " + location_string + AdvertiseMainLocalization.GetPositionTypeString((int)item));
                    }
                    return str;
                case AdvertiseType.Complex:
                    str = "اجاره روزانه مجتمع";
                    foreach (var item in regionList)
                    {
                        if (item != PositionType.none)
                            str += ("،اجاره روزانه مجتمع " + location_string + AdvertiseMainLocalization.GetPositionTypeString((int)item));
                    }
                    return str;
                case AdvertiseType.Hut:
                    str = "اجاره روزانه کلبه";
                    foreach (var item in regionList)
                    {
                        if (item != PositionType.none)
                            str += ("،اجاره روزانه کلبه " + location_string + AdvertiseMainLocalization.GetPositionTypeString((int)item));
                    }
                    return str;
                default:
                    return "";
            }
        }

        public static string GetDescription(int mostAccType, int advertiseType, string province,
            string city, string area, string country_direction)
        {
            var region_string = !string.IsNullOrEmpty(country_direction)
                ? country_direction :
                (!string.IsNullOrEmpty(area) ? area :
                (!string.IsNullOrEmpty(city) ? city : province));
            var all = string.IsNullOrEmpty(region_string);
            var end_string = " در " + (all ? "شمال، مازندران، تهران، کرج، کردان، متل قو، اصفهان، مشهد، شیراز، گیلان، چالوس، نور و تمام شهر های توریستی ایران" : region_string) + " در سایت املاک باشی";
            switch ((AdvertiseType)advertiseType)
            {
                case AdvertiseType.All:
                    if (all)
                    {
                        return string.Format("اجاره ویلا{0}، اجاره سوئیت{0}، اجاره آپارتمان مبله{0}، رزرو آنلاین ویلا{0}، اجاره روزانه ویلا{0}، اجاره روزانه، هفتگی، ماهانه و سالانه{0}، اجاره خانه مسافر{0}، رزرو هتل و مسافرخانه و هتل آپارتمان در{0}، ", all ? "" : " " + region_string)
                        + end_string;
                    }
                    else
                    {
                        switch (mostAccType)
                        {
                            case 82:
                                return string.Format("اجاره روزانه خانه{0}، اجاره ویلا و سوئیت{0}، اجاره روزانه آپارتمان{0}، رزرو هتل، مسافرخانه و هتل آپارتمان{0}، منزل مبله{0}، اجاره اتاق{0}، اجاره آپارتمان یک روزه{0}، اجاره چند روزه سوئیت{0}، اجاره آپارتمان هفتگی{0}، اجاره آپارتمان ماهانه{0}، اجاره آپارتمان سالانه{0}، از ارزان ترین تا لوکس ترین واحدها در سایت املاک باشی، ", all ? "" : " " + region_string);
                            case 83:
                                return string.Format("اجاره ویلا{0}، اجاره سوئیت{0}، رزرو ویلا{0}، اجاره روزانه خانه{0}، اجاره روزانه ویلا{0}، اجاره روزانه، هفتگی، ماهانه و سالانه آپارتمان{0}، رزرو هتل و هتل آپارتمان{0}، منزل مبله{0}، اجاره اتاق{0}، اجاره آپارتمان یک روزه{0}، اجاره چند روزه سوئیت{0}، اجاره ویلا استخردار، ساحلی، جنگلی{0}، از ارزان قیمت ترین تا لوکس ترین ویلاها در سایت املاک باشی، ", all ? "" : " " + region_string);
                            default:
                                return string.Format("اجاره ویلا{0}، اجاره سوئیت{0}، اجاره آپارتمان مبله{0}، رزرو آنلاین ویلا{0}، اجاره روزانه ویلا{0}، اجاره روزانه{0}، اجاره خانه مسافر{0}، رزرو هتل و مسافرخانه و هتل آپارتمان{0}، ", all ? "" : " " + region_string)
                                    + end_string;
                        }
                    }
                case AdvertiseType.Apartment:
                    return string.Format("اجاره روزانه خانه{0}، اجاره سوئیت در{0}، رزرو آنلاین آپارتمان مبله{0}، منزل مبله{0}، اجاره روزانه آپارتمان مبله{0}، اجاره آپارتمان مبله{0}، خانه مسافر{0}، اجاره اتاق{0}، اجاره آپارتمان یکروزه{0}، اجاره آپارتمان هفتگی{0}، اجاره آپارتمان سالانه{0}،اجاره آپارتمان ماهانه{0}", all ? "" : " " + region_string)
                        + end_string;
                case AdvertiseType.Villa:
                    return string.Format("اجاره ویلا{0}، رزرو آنلاین ویلا{0}، اجاره روزانه، هفتگی، ماهانه و سالانه ویلا{0}، استخردار، ساحلی، جنگلی، از ارزان ترین تا لوکس ترین ویلا", all ? "" : " " + region_string)
                        + end_string;
                case AdvertiseType.Hotel:
                    return string.Format("رزرو آنلاین هتل جهت اسکان موقت و کوتاه مدت برای مسافران و مهمانان", all ? "" : " " + region_string)
                        + end_string;
                case AdvertiseType.HotelApartment:
                    return string.Format("رزرو آنلاین هتل آپارتمان جهت اسکان موقت و کوتاه مدت برای مسافران و مهمانان", all ? "" : " " + region_string)
                        + end_string;
                case AdvertiseType.Camp:
                    return string.Format("رزرو آنلاین کمپ جهت اسکان موقت و کوتاه مدت برای مسافران و مهمانان", all ? "" : " " + region_string)
                        + end_string;
                case AdvertiseType.TourismAccommodation:
                    return string.Format("رزرو آنلاین اقامتگاه بومگردی جهت اسکان موقت و کوتاه مدت برای مسافران و مهمانان", all ? "" : " " + region_string)
                        + end_string;
                case AdvertiseType.House:
                    return string.Format("اجاره خانه ویلایی{0}، رزرو آنلاین خانه ویلایی{0}، اجاره روزانه، هفتگی، ماهانه و سالانه خانه ویلایی", all ? "" : " " + region_string)
                        + end_string;
                case AdvertiseType.SuitAndRoom:
                    return string.Format("اجاره سوئیت{0}، اجاره روزانه خانه{0}، آپارتمان مبله{0}، اجاره اتاق و سوئیت{0}، رزرو آنلاین اتاق و سوئیت{0}، اجاره خانه{0}، منزل مبله{0}، اتاق و سوئیت{0}، اجاره روزانه، هفتگی، ماهانه و سالانه اتاق و سوئیت", all ? "" : " " + region_string)
                        + end_string;
                case AdvertiseType.Inn:
                    return string.Format("رزرو آنلاین مسافرخانه جهت اسکان موقت و کوتاه مدت برای مسافران و مهمانان", all ? "" : " " + region_string)
                        + end_string;
                case AdvertiseType.Pansion:
                    return string.Format("اجاره پانسیون{0}، رزرو آنلاین پانسیون{0}، اجاره روزانه، هفتگی، ماهانه و سالانه پانسیون", all ? "" : " " + region_string)
                        + end_string;
                case AdvertiseType.Complex:
                    return string.Format("اجاره مجتمع{0}، رزرو آنلاین مجتمع{0}، اجاره مجتمع مسکونی{0}، اجاره روزانه، هفتگی، ماهانه و سالانه مجتمع مسکونی", all ? "" : " " + region_string)
                        + end_string;
                case AdvertiseType.Hut:
                    return string.Format("اجاره کلبه{0}، رزرو آنلاین کلبه{0}، اجاره روزانه، هفتگی، ماهانه و سالانه کلبه", all ? "" : " " + region_string)
                        + end_string;
                default:
                    return "";
            }
        }

        public static string GetMetaDescription(Advertise acc, string cityTitle, string areaTitle)
        {
            try
            {
                string tmpTitle = "اجاره روزانه";
                tmpTitle += " " + AdvertiseMainLocalization.GetAdvertiseTypePersianString((int)acc.TypeID);

                tmpTitle += " " + AdvertiseMainLocalization.GetPositionTypeString((int)acc.Position);

                if (acc.Room > 0)
                    tmpTitle += " " + acc.Room.ToString() + " خوابه";

                if (acc.Metrazh > 0)
                    tmpTitle += " " + acc.Metrazh.ToString() + " متری";


                if (acc.City > 0)
                {
                    if (!string.IsNullOrEmpty(cityTitle))
                        tmpTitle += " در " + cityTitle;
                }
                if (acc.Area != null)
                {
                    if (!string.IsNullOrEmpty(areaTitle))
                        tmpTitle += "، " + areaTitle;
                }

                if (acc.Pool != null && acc.Pool == true)
                    tmpTitle += "، استخردار";

                if (acc.Capacity > 0)
                    tmpTitle += "، ظرفیت " + acc.Capacity.ToString() + " نفر";


                if ((acc.Elevator != null && acc.Elevator == true) || (int)acc.Parking > 70)
                {
                    tmpTitle += "، دارای ";
                    if (acc.Elevator != null && acc.Elevator == true)
                        tmpTitle += " آسانسور";

                    if ((acc.Elevator != null && acc.Elevator == true) && (int)acc.Parking > 70)
                    {
                        tmpTitle += " و ";
                    }

                    if ((int)acc.Parking > 70)
                        tmpTitle += " پارکینگ";
                }

                

                tmpTitle += " | یک روزه، چند روزه، هفتگی، ماهانه و سالانه";

                tmpTitle += " | اجاره روزانه خانه، ویلا و سوئیت در سایت املاک باشی";

                return tmpTitle;
            }
            catch
            {
                return "";
            }
        }
    }
}
