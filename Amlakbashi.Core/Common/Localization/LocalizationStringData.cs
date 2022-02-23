
using System.Collections.Generic;

namespace Amlakbashi.Core.Common.Localization
{
    public static class LocalizationStringData
    {
        private static Languages currentLanguage = Languages.Persian;
        private static Dictionary<Languages, Dictionary<string, string>> localizationData =
            new Dictionary<Languages, Dictionary<string, string>>(){
                {
                    Languages.Persian,
                    new Dictionary<string, string>()
                    {
                        {"ACC_VALIDATION_PROVINCE", "استان را انتخاب کنید" },
                        {"ACC_VALIDATION_CITY", "شهر را انتخاب کنید" },
                        {"ACC_VALIDATION_AREA", "منطقه را انتخاب کنید" },
                        {"ACC_VALIDATION_ADDRESS", "آدرس را وارد کنید" },
                        {"ACC_VALIDATION_GEOLOCATION", "موقعیت اقامتگاه را در نقشه وارد کنید" },
                        {"ACC_VALIDATION_CAPACITY", "ظرفیت اقامتگاه را وارد کنید" },
                        {"ACC_VALIDATION_OWNERSHIP", "وضعیت مالکیت را مشخص کنید" },
                        {"ACC_VALIDATION_ADVERTISE_TYPE", "نوع اقامتگاه را انتخاب کنید" },
                        {"ACC_VALIDATION_POSITION", "موقعیت اقامتگاه را انتخاب کنید" },
                        {"ACC_VALIDATION_TITLE", "عنوان اقامتگاه را بنویسید" },
                        {"ACC_VALIDATION_DESC", "توضیحات اقامتگاه را بنویسید" },
                        {"ACC_VALIDATION_META_TITLE", "متای عنوان گوگل را بنویسید" },
                        {"ACC_VALIDATION_META_DESC", "متای توضیحات گوگل را بنویسید" },
                        {"ACC_VALIDATION_BUILDING_SIZE", "متراژ بنا را وارد کنید" },
                        {"ACC_VALIDATION_LAND_AREA", "متراژ زمین را وارد کنید" },
                        {"ACC_VALIDATION_ROOM", "تعداد اتاق خواب را وارد کنید" },
                        {"ACC_VALIDATION_FLOOR", "طبقه ملک را انتخاب کنید" },
                        {"ACC_VALIDATION_COUNT", "لطفا تعداد اتاق را وارد کنید" },
                        {"ACC_VALIDATION_PRICE", "لطفا تمامی قیمت ها را به طور صحیح وارد کنید" },
                        {"ACC_VALIDATION_PRICE_MIN", "حداقل قیمت {0} تومان" },
                        {"ACC_VALIDATION_AMENITIES", "لطفا تمامی امکانات را تعیین کنید" },
                        {"ACC_VALIDATION_ELEVATOR", "لطفا آسانسور را تعیین کنید" },
                        {"ACC_VALIDATION_PARKING", "لطفا تعداد پارکینگ را مشخص کنید" },
                    }
                }
        };
        public static string Get(string key)
        {
            return localizationData[currentLanguage][key];
        }
        private enum Languages
        {
            Persian = 0
        }
    }
}
