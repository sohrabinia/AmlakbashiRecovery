using Amlakbashi.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amlakbashi.Core.Infrastructure.LocalizationHelpers
{
    public static class CategoryFaqLocalization
    {
        public static string CategoryFaqTrustQuestion(DynamicCategory category)
        {
            return string.Format("چرا به سایت املاک باشی جهت {0} در {1} اعتماد کنم ؟", category.TypeString == null ? "اجاره ویلا، سوئیت و آپارتمان مبله" : "اجاره روزانه " + category.TypeString, category.Area != null ? category.ParentRegionString : category.RegionString);
        }

        public static string CategoryFaqTrustAnswer()
        {
            return "سایت املاک باشی دارای نماد اعتماد الکترونیکی است و سالهاست که در زمینه اجاره روزانه اقامتگاه فعالیت دارد و با توجه به تجربیات مبلغ دریافتی شما را تا 24 ساعت پس از تحویل واحد نزد خود امانت نگه میدارد تا در صورت بروز هرگونه مشکل کل مبلغ به شما عودت داده شود ، همچنین مرکز پشتیبانی این سایت همه روزه حتی ایام تعطیل از ساعت 9 الی 23:30 باشماره 09360263804 و 02632565304 و 02632565296 آماده پاسخگویی به شماست.";
        }

        public static string CategoryFaqPriceQuestion(DynamicCategory category)
        {
            return string.Format("قیمت {0} در {1} چقدر است؟",
                category.TypeString == null ? "اجاره ویلا، سوئیت و آپارتمان مبله" : "اجاره روزانه " + category.TypeString, category.Area != null ? category.ParentRegionString : category.RegionString);
        }

        public static string CategoryFaqPriceAnswer(DynamicCategory category)
        {
            return string.Format("{0} در {1} با توجه به منطقه، متراژ و امکانات از شبی حداقل {2} تومان تا {3} تومان متغیر است.",
                category.TypeString == null ? "اجاره ویلا، سوئیت و آپارتمان مبله" : "اجاره روزانه " + category.TypeString, category.Area != null ? category.ParentRegionString : category.RegionString,
                string.Format("{0:n0}", category.Area != null ? category.ParentMinPrice : category.MinPrice),
                string.Format("{0:n0}", category.Area != null ? category.ParentMaxPrice : category.MaxPrice));
        }

        public static string CategoryFaqAreasQuestion(DynamicCategory category)
        {
            if (string.IsNullOrEmpty(category.CityAreaListString))
            {
                return null;
            }
            return string.Format("در کدام مناطق {0} میتوانم بصورت روزانه {1} کرایه کنم ؟",
                category.Area != null ? category.ParentRegionString : category.RegionString, category.TypeString == null ? "ویلا، سوئیت و آپارتمان مبله" : category.TypeString);
        }

        public static string CategoryFaqAreasAnswer(DynamicCategory category)
        {
            if (string.IsNullOrEmpty(category.CityAreaListString))
            {
                return null;
            }
            return string.Format("{0} سوئیت مبله ، اپارتمان مبله، خانه، ویلا و اقامتگاه برای اجاره روزانه در {1} وجود دارد، مناطقی همچون {2} و ده ها منطقه دیگر {1}",
                GenerateCountString(category.Area != null ? category.ParentCountAcc : category.CountAdvertise), category.Area != null ? category.ParentRegionString : category.RegionString,
                category.CityAreaListString);
        }

        public static string CategoryFaqEvidenceQuestion(DynamicCategory category)
        {
            return string.Format("برای {0} در {1} چه مدارکی لازم است ؟",
                category.TypeString == null ? "اجاره ویلا، سوئیت و آپارتمان مبله" : "اجاره روزانه " + category.TypeString, category.Area != null ? category.ParentRegionString : category.RegionString);
        }

        public static string CategoryFaqEvidenceAnswer()
        {
            return "از آنجا که اقامتگاه بصورت مبله با تمامی امکانات در اختیار شما قرار میگیرد ارائه حداقل یک مدرک شناسایی معتبر همانند کارت ملی یا شناسنامه در هنگام دریافت کلید الزامی است. با این حال میتوانید با میزبان در این مورد توافق کنید.";
        }

        public static string CategoryFaqReserveQuestion(DynamicCategory category)
        {
            var regionStr = !string.IsNullOrEmpty(category.ParentRegionString) ?
                category.ParentRegionString : category.RegionString;
            return string.Format("چگونه در {0} به صورت آنلاین {1} رزرو کنم ؟",
                regionStr, string.IsNullOrEmpty(category.TypeString) ? "ویلا، سوئیت و آپارتمان مبله" : category.TypeString);
        }

        public static string CategoryFaqReserveAnswer(DynamicCategory category)
        {
            var regionStr = !string.IsNullOrEmpty(category.ParentRegionString) ?
                category.ParentRegionString : category.RegionString;
            var count = category.ParentCountAcc > 0 ? category.ParentCountAcc :
                category.CountAdvertise;
            var isCity = category.City != null;
            if (isCity)
            {
                regionStr = "شهر " + regionStr;
            }
            var chooseRegionStr = "";
            if (category.Province != null || category.CountryDirection > 0)
            {
                chooseRegionStr = " " + regionStr + " را انتخاب کنید،";
            }
            return string.Format("وارد سایت املاک باشی شوید،{0} کلیه اقامتگاه های {1} ({2} مورد) به همراه قیمت و تصاویر با امکان فیلتر کردن برای شما نمایش داده میشود، اقامتگاه مورد نظر را انتخاب و پس از مشاهده جزئیات و اطمینان از شرایط ملک بر روی دکمه 'درخواست رزرو' کلیک کنید. درخواست رزرو ثبت کنید منتظر بمانید تا میزبان پاسخ دهد  از قسمت چت می توانید سوالات خود را با   میزبان مطرح کنید. همچنین در تمامی مراحل پشتیبانها همراه شما و اماده خدمات رسانی به شما هستند",
                chooseRegionStr,
                regionStr, GenerateCountString(count));
        }

        public static string CategoryFaqHostQuestion()
        {
            return "چطور در سایت املاک باشی میزبان شوم؟";
        }

        public static string CategoryFaqHostAnswer()
        {
            return "برای ثبت اگهی در سایت املاک باشی ابتدا عضو سایت شوید سپس روی ثبت اگهی کلیک کنید و اطلاعات کامل را وارد کنید در صورت نیاز به راهنمایی بیشتر با شماره های 09196218216 02632554756 تماس حاصل نمایید";
        }

        private static string GenerateCountString(int count)
        {
            if (count <= 10)
            {
                return "چندین";
            }
            if (count <= 100)
            {
                return "بیش از " + (count - count % 10);
            }
            return "بیش از " + (count - count % 100);
        }
    }
}
