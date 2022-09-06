using Amlakbashi.Core.DTOs.WebService;
using Amlakbashi.Core.Entities;
using System;
using System.Collections.Generic;
using static Amlakbashi.Core.Entities.Advertise;

namespace Amlakbashi.Core.Infrastructure.LocalizationHelpers
{
    public static class AdvertiseMainLocalization
    {
        public static string GetPositionTypePersianName(int position)
        {
            switch ((PositionType)position)
            {
                case PositionType.sahel:
                    return "ساحلی";
                case PositionType.jungle:
                    return "جنگلی";
                case PositionType.koohestani:
                    return "کوهستانی";
                case PositionType.biaban:
                    return "بیابانی";
                case PositionType.shahri:
                    return "شهری";
                case PositionType.hoome:
                    return "حومه‌ی شهر";
                case PositionType.roostaee:
                    return "روستایی";
                case PositionType.dakhele_shahrak:
                    return "داخل شهرک";
                case PositionType.ashayeri:
                    return "عشایری";
                case PositionType.SummerQuarter:
                    return "ییلاقی";
                default:
                    return "انتخاب کنید";
            }
        }

        public static string GetAdvertiseTypePersianNameForUser(AdvertiseType advertiseType)
        {
            switch (advertiseType)
            {
                case AdvertiseType.All:
                    return "اجاره روزانه";
                case AdvertiseType.Apartment:
                    return "آپارتمان مبله";
                case AdvertiseType.Villa:
                    return "ویلا";
                case AdvertiseType.Hotel:
                    return "هتل";
                case AdvertiseType.HotelApartment:
                    return "هتل آپارتمان";
                case AdvertiseType.Camp:
                    return "کمپ";
                case AdvertiseType.TourismAccommodation:
                    return "اقامتگاه بومگردی";
                case AdvertiseType.House:
                    return "خانه ویلایی مبله";
                case AdvertiseType.SuitAndRoom:
                    return "اتاق و سوئیت مبله";
                case AdvertiseType.Inn:
                    return "مسافرخانه";
                case AdvertiseType.Pansion:
                    return "پانسیون";
                case AdvertiseType.Complex:
                    return "مجتمع";
                case AdvertiseType.Hut:
                    return "کلبه";
                default:
                    return "انتخاب کنید";
            }
        }

        public static string GetAdvertiseTypePersianNameForAdminPanel(AdvertiseType advertiseType)
        {
            switch (advertiseType)
            {
                case AdvertiseType.All:
                    return "همه";
                case AdvertiseType.Apartment:
                    return "آپارتمان مبله";
                case AdvertiseType.Villa:
                    return "ویلا";
                case AdvertiseType.Hotel:
                    return "هتل";
                case AdvertiseType.HotelApartment:
                    return "هتل آپارتمان";
                case AdvertiseType.Camp:
                    return "کمپ";
                case AdvertiseType.TourismAccommodation:
                    return "اقامتگاه بومگردی";
                case AdvertiseType.House:
                    return "خانه ویلایی مبله";
                case AdvertiseType.SuitAndRoom:
                    return "اتاق و سوئیت مبله";
                case AdvertiseType.Inn:
                    return "مسافرخانه";
                case AdvertiseType.Pansion:
                    return "پانسیون";
                case AdvertiseType.Complex:
                    return "مجتمع";
                case AdvertiseType.Hut:
                    return "کلبه";
                default:
                    return "هیچ کدام";
            }
        }

        public static string GetAdvertiseTypePersianName(int advertiseType)
        {
            switch ((AdvertiseType)advertiseType)
            {
                case AdvertiseType.All:
                    return "اجاره روزانه";
                case AdvertiseType.Apartment:
                    return "آپارتمان";
                case AdvertiseType.Villa:
                    return "ویلا";
                case AdvertiseType.Hotel:
                    return "رزرو هتل";
                case AdvertiseType.HotelApartment:
                    return "رزرو هتل آپارتمان";
                case AdvertiseType.Camp:
                    return "کمپ";
                case AdvertiseType.TourismAccommodation:
                    return "رزرو اقامتگاه بومگردی";
                case AdvertiseType.House:
                    return "خانه ویلایی";
                case AdvertiseType.SuitAndRoom:
                    return "اتاق و سوئیت";
                case AdvertiseType.Inn:
                    return "مسافرخانه";
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

        public static string GetAdvertiseStatusPersianName(int status, bool shortened = false)
        {
            switch ((AdvertiseStatus)status)
            {
                case AdvertiseStatus.ReadyToPublish:
                    return "در انتظار تایید" + (shortened ? "" : " کارشناس");
                case AdvertiseStatus.Published:
                    return "تایید شده";
                case AdvertiseStatus.Archived:
                    return "غیر فعال";
                case AdvertiseStatus.Deleted:
                    return "پاک شده";
                case AdvertiseStatus.NotVerified:
                    return "تایید نشده";
                case AdvertiseStatus.NotCompleted:
                    return shortened ? "تکمیل نشده" : "اطلاعات آگهی را تکمیل کنید";
                case AdvertiseStatus.FirstReady:
                    return "در انتظار تایید" + (shortened ? "" : " کارشناس");
                default:
                    return "";
            }
        }

        public static string GetInstantReserveStatusPersianName(InstantReserveStatusEnum status)
        {
            switch (status)
            {
                case InstantReserveStatusEnum.Calendar:
                    return "تقویمی";
                case InstantReserveStatusEnum.Permanent:
                    return "دائمی";
                case InstantReserveStatusEnum.InActive:
                    return "غیرفعال";
                default:
                    return "";
            }
        }

        public static string GetLocationString(string province, string city, string area,
            string country_direction_string)
        {
            var location_string = "";
            if (!string.IsNullOrEmpty(country_direction_string))
            {
                location_string = country_direction_string;
            }
            else if (!string.IsNullOrEmpty(area))
            {
                location_string = area;
            }
            else if (!string.IsNullOrEmpty(city))
            {
                location_string = city;
            }
            else
            {
                location_string = "استان " + province;
            }
            return location_string;
        }

        public static string GetPropertyTitle(Property property)
        {
            switch (property)
            {
                case Property.TypeID:
                    return "نوع اقامتگاه";
                case Property.Region:
                    return "موقعیت اقامتگاه";
                case Property.Title:
                    return "عنوان";
                case Property.Description:
                    return "توضیحات";
                case Property.UnitCount:
                    return "تعداد اتاق";
                case Property.ProvinceId:
                    return "استان";
                case Property.CityId:
                    return "شهر";
                case Property.AreaId:
                    return "منطقه";
                case Property.Address:
                    return "آدرس";
                case Property.DailyPrice:
                    return "قیمت پایه برای روزهای عادی";
                case Property.HolidayPrice:
                    return "قیمت روزهای تعطیل";
                case Property.PeakHolidayPrice:
                    return "قیمت روزهای پیک تعطیلات";
                case Property.NowruzPrice:
                    return "قیمت روزهای نوروز";
                case Property.MonthlyPrice:
                    return "قیمت ماهیانه";
                case Property.ExtraCapacityPrice:
                    return "قیمت به ازای هر مهمان بیشتر از ظرفیت";
                case Property.BuildingArea:
                    return "متراژ بنا";
                case Property.LandArea:
                    return "متراژ زمین";
                case Property.Capacity:
                    return "ظرفیت اقامتگاه";
                case Property.ExtraCapacity:
                    return "حداکثر مهمان اضافه";
                case Property.RoomCount:
                    return "تعداد اتاق خواب";
                case Property.Parking:
                    return "تعداد پارکینگ اقامتگاه";
                case Property.SingleBedCount:
                    return "تعداد تخت یک نفره";
                case Property.DoubleBedCount:
                    return "تعداد تخت دو نفره";
                case Property.Floor:
                    return "طبقه ملک";
                case Property.BuildingDirection:
                    return "جهت ملک";
                case Property.Elevator:
                    return "آسانسور";
                case Property.Pool:
                    return "استخر";
                case Property.Sauna:
                    return "سونا";
                case Property.Jacuzzi:
                    return "جکوزی";
                case Property.Bathroom:
                    return "حمام";
                case Property.Wifi:
                    return "وای فای";
                case Property.WashingMachine:
                    return "ماشین لباسشویی";
                case Property.MicrowaveOven:
                    return "مایکروویو";
                case Property.SoundSystem:
                    return "ضبط و باند";
                case Property.Golf:
                    return "گلف";
                case Property.PoolTable:
                    return "بیلیارد";
                case Property.Foosball:
                    return "فوتبال دستی";
                case Property.Hairdryer:
                    return "سشوار";
                case Property.TV:
                    return "تلویزیون";
                case Property.Oven:
                    return "اجاق گاز";
                case Property.Refrigerator:
                    return "یخچال";
                case Property.KitchenHood:
                    return "هود آشپرخانه";
                case Property.KitchenUtensils:
                    return "ظروف آشپرخانه";
                case Property.TeaMaker:
                    return "چای ساز";
                case Property.Balcony:
                    return "بالکن";
                case Property.Filming:
                    return "فیلم برداری";
                case Property.ExtraBlanketCount:
                    return "تعداد پتو اضافه";
                case Property.HeatingSystem:
                    return "سیستم گرمایشی";
                case Property.CoolingSystem:
                    return "سیستم سرمایشی";
                case Property.WC:
                    return "سرویس بهداشتی";
                case Property.BlanketAndMattressCount:
                    return "تعداد تشک و پتو";
                case Property.Smoking:
                    return "استعمال دخانیات در داخل اقامتگاه";
                case Property.Pets:
                    return "آوردن حیوانات خانگی";
                case Property.Party:
                    return "گرفتن مهمانی در این اقامتگاه";
                case Property.RequiredEvidence:
                    return "مدارک مورد نیاز";
                case Property.OtherRules:
                    return "سایر شرایط و قوانین اقامتگاه";
                default:
                    return "";
            }
        }

        public static string GetHotelUnitPersianName(AdvertiseType type)
        {
            switch (type)
            {
                case AdvertiseType.Hotel:
                case AdvertiseType.Inn:
                case AdvertiseType.Pansion:
                    return "اتاق";
                case AdvertiseType.Camp:
                    return "چادر";
                case AdvertiseType.TourismAccommodation:
                    return "واحد";
                default:
                    return "";
            }
        }

        public static string GetParkingPersianName(Advertise.ParkingItems item)
        {
            switch (item)
            {
                case ParkingItems.Unset:
                    return "انتخاب تعداد پارکینگ";
                case ParkingItems.One:
                    return "1 پارکینگ";
                case ParkingItems.Two:
                    return "2 پارکینگ";
                case ParkingItems.Three:
                    return "3 پارکینگ";
                case ParkingItems.MoreThanThree:
                    return "بیشتر از 3";
                case ParkingItems.Jointly:
                    return "مشاع";
                case ParkingItems.NoParking:
                    return "بدون پارکینگ";
                default:
                    return "";
            }
        }

        public static string GetParkingText(ParkingItems item)
        {
            switch (item)
            {
                case ParkingItems.One:
                    return "1 پارکینگ";
                case ParkingItems.Two:
                    return "2 پارکینگ";
                case ParkingItems.Three:
                    return "3 پارکینگ";
                case ParkingItems.MoreThanThree:
                    return "بیشتر از 3 پارکینگ";
                case ParkingItems.Jointly:
                    return "پارکینگ مشاع";
                case ParkingItems.NoParking:
                    return "بدون پارکینگ";
                default:
                    return "همه";
            }
        }

        public static string GetExtraBlanketCountPersianName(Advertise.ExtraBlanketCountItems item)
        {
            switch (item)
            {
                case ExtraBlanketCountItems.Unset:
                    return "انتخاب کنید";
                case ExtraBlanketCountItems.One:
                    return "یک عدد";
                case ExtraBlanketCountItems.Two:
                    return "دو عدد";
                case ExtraBlanketCountItems.Three:
                    return "سه عدد";
                case ExtraBlanketCountItems.Four:
                    return "چهار عدد";
                case ExtraBlanketCountItems.Five:
                    return "پنج عدد";
                case ExtraBlanketCountItems.MoreThanFive:
                    return "بیشتر از پنج عدد";
                default:
                    return "";
            }
        }

        public static string GetBuildingDirectionPersianName(Advertise.BuildingDirectionItems item)
        {
            switch (item)
            {
                case BuildingDirectionItems.Unset:
                    return "انتخاب کنید";
                case BuildingDirectionItems.Western:
                    return "غربی";
                case BuildingDirectionItems.Eastern:
                    return "شرقی";
                case BuildingDirectionItems.Northern:
                    return "شمالی";
                case BuildingDirectionItems.Southern:
                    return "جنوبی";
                case BuildingDirectionItems.TwoSided:
                    return "دوبر";
                default:
                    return "";
            }
        }

        public static string GetHeatingSystemPersianName(Advertise.HeatingSystemItems item)
        {
            switch (item)
            {
                case HeatingSystemItems.Unset:
                    return "انتخاب کنید";
                case HeatingSystemItems.Heater:
                    return "بخاری";
                case HeatingSystemItems.Package:
                    return "پکیج";
                case HeatingSystemItems.Radiator:
                    return "شوفاژ";
                case HeatingSystemItems.AirConditioner:
                    return "هواساز";
                case HeatingSystemItems.FirePlace:
                    return "شومینه";
                case HeatingSystemItems.Other:
                    return "غیره";
                case HeatingSystemItems.None:
                    return "ندارد";
                default:
                    return "";
            }
        }

        public static string GetCoolingSystemPersianName(Advertise.CoolingSystemItems item)
        {
            switch (item)
            {
                case CoolingSystemItems.Unset:
                    return "انتخاب کنید";
                case CoolingSystemItems.Chiller:
                    return "چیلر";
                case CoolingSystemItems.Fancoel:
                    return "فنکوئل";
                case CoolingSystemItems.WaterCooler:
                    return "کولر آبی";
                case CoolingSystemItems.Splitter:
                    return "کولر گازی";
                case CoolingSystemItems.AirConditioner:
                    return "هواساز";
                case CoolingSystemItems.SplitterAndWaterCooler:
                    return "کولر آبی و گازی";
                case CoolingSystemItems.Fan:
                    return "پنکه";
                case CoolingSystemItems.Other:
                    return "غیره";
                case CoolingSystemItems.None:
                    return "ندارد";
                default:
                    return "";
            }
        }

        public static string GetWCPersianName(Advertise.WCItems item)
        {
            switch (item)
            {
                case WCItems.Unset:
                    return "انتخاب کنید";
                case WCItems.Persian:
                    return "ایرانی";
                case WCItems.Europian:
                    return "فرنگی";
                case WCItems.EuropianAndPersian:
                    return "ایرانی و فرنگی";
                default:
                    return "";
            }
        }

        public static string GetEuropeanToiletTypePersianName(Advertise.EuropeanToiletTypeEnum item)
        {
            switch (item)
            {
                case EuropeanToiletTypeEnum.Unset:
                    return "انتخاب کنید";
                case EuropeanToiletTypeEnum.Fixed:
                    return "ثابت";
                case EuropeanToiletTypeEnum.Portable:
                    return "سیار";
                case EuropeanToiletTypeEnum.FixedAndPortable:
                    return "هر دو (ثابت و سیار)";
                default:
                    return "";
            }
        }

        public static string GetFloorPersianName(Advertise.FloorItems item)
        {
            switch (item)
            {
                case FloorItems.Unset:
                    return "انتخاب کنید";
                case FloorItems.Underground:
                    return "زیرزمین";
                case FloorItems.Ground:
                    return "همکف";
                case FloorItems.MoreThan10th:
                    return "بالاتر از 10";
                default:
                    return ((int)(item)).ToString();
            }
        }

        public static string GetOwnershipPersianName(Advertise.OwnershipTypeEnum item)
        {
            switch (item)
            {
                case OwnershipTypeEnum.Unset:
                    return "انتخاب کنید";
                case OwnershipTypeEnum.Owner:
                    return "مالک";
                case OwnershipTypeEnum.Intermediary:
                    return "واسطه";
                default:
                    return "";
            }
        }

        public static string GetVillaTypePersianName(Advertise.VillaTypeEnum item)
        {
            switch (item)
            {
                case VillaTypeEnum.Unset:
                    return "انتخاب کنید";
                case VillaTypeEnum.Exclusive:
                    return "دربست";
                case VillaTypeEnum.Common:
                    return "مشترک";
                default:
                    return "";
            }
        }

        public static string GetEnumTypePersianName(NameValueDTO.EnumType item)
        {
            switch (item)
            {
                case NameValueDTO.EnumType.keyValueType:
                    return "انواع لیست ها";
                case NameValueDTO.EnumType.residenceType:
                    return "لیست نوع اقامتگاه";
                case NameValueDTO.EnumType.residenceLocationType:
                    return "لیست موقعیت اقامتگاه";
                case NameValueDTO.EnumType.residenceCoolingSystem:
                    return "لیست سیستم سرمایشی";
                case NameValueDTO.EnumType.residenceHeatingSystem:
                    return "لیست سیستم گرمایشی";
                case NameValueDTO.EnumType.residenceWCType:
                    return "لیست سرویس بهداشتی";
                case NameValueDTO.EnumType.residenceOwnershipType:
                    return "لیست نوع مالکیت";
                case NameValueDTO.EnumType.residenceParking:
                    return "لیست پارکینگ";
                case NameValueDTO.EnumType.residenceFloor:
                    return "لیست طبقات";
                case NameValueDTO.EnumType.residenceExtraBlanket:
                    return "لیست تعداد پتوی اضافه";
                default:
                    return "";
            }
        }

        public static string GetEnumPersianName(object item)
        {
            if (item.GetType() == typeof(Advertise.AdvertiseType))
            {
                return GetAdvertiseTypePersianNameForUser((AdvertiseType)item);
            }
            if (item.GetType() == typeof(Advertise.PositionType))
            {
                return GetPositionTypePersianName((int)(PositionType)item);
            }
            if (item.GetType() == typeof(Advertise.ParkingItems))
            {
                return GetParkingPersianName((Advertise.ParkingItems)item);
            }
            if (item.GetType() == typeof(Advertise.BuildingDirectionItems))
            {
                return GetBuildingDirectionPersianName((Advertise.BuildingDirectionItems)item);
            }
            if (item.GetType() == typeof(Advertise.ExtraBlanketCountItems))
            {
                return GetExtraBlanketCountPersianName((Advertise.ExtraBlanketCountItems)item);
            }
            if (item.GetType() == typeof(Advertise.HeatingSystemItems))
            {
                return GetHeatingSystemPersianName((Advertise.HeatingSystemItems)item);
            }
            if (item.GetType() == typeof(Advertise.CoolingSystemItems))
            {
                return GetCoolingSystemPersianName((Advertise.CoolingSystemItems)item);
            }
            if (item.GetType() == typeof(Advertise.WCItems))
            {
                return GetWCPersianName((Advertise.WCItems)item);
            }
            if (item.GetType() == typeof(Advertise.EuropeanToiletTypeEnum))
            {
                return GetEuropeanToiletTypePersianName((Advertise.EuropeanToiletTypeEnum)item);
            }
            if (item.GetType() == typeof(Advertise.FloorItems))
            {
                return GetFloorPersianName((Advertise.FloorItems)item);
            }
            if (item.GetType() == typeof(Advertise.OwnershipTypeEnum))
            {
                return GetOwnershipPersianName((Advertise.OwnershipTypeEnum)item);
            }
            if (item.GetType() == typeof(Advertise.VillaTypeEnum))
            {
                return GetVillaTypePersianName((Advertise.VillaTypeEnum)item);
            }
            if (item.GetType() == typeof(NameValueDTO.EnumType))
            {
                return GetEnumTypePersianName((NameValueDTO.EnumType)item);
            }
            return null;
        }

        public static string CategoryTitle { get { return "اجاره-روزانه"; } }

        public static string GetNotVerifyReasonPersianDesc(int reason)
        {
            switch ((NotVerifyReasonsEnum)reason)
            {
                case NotVerifyReasonsEnum.Reason_1:
                    return "قیمت وارد شده مورد تایید نمی باشد";
                case NotVerifyReasonsEnum.Reason_2:
                    return "آگهی تکراری مورد تایید نیست , لطفا اگهی قبلی را ویرایش کنید";
                case NotVerifyReasonsEnum.Reason_3:
                    return "آگهی های فاقد عکس مورد تایید نمی باشد";
                case NotVerifyReasonsEnum.Reason_4:
                    return "لطفا وضعیت مالکیت خود را صحیح وارد کنید";
                case NotVerifyReasonsEnum.Reason_5:
                    return "عکس آپلود شده مورد تایید نمی باشد";
                case NotVerifyReasonsEnum.Reason_6:
                    return "لطفا اطلاعات اقامتگاه تکمیل کنید";
                case NotVerifyReasonsEnum.Reason_7:
                    return "قیمت پایه برای روزهای عادی (به تومان) را وارد کنید";
                case NotVerifyReasonsEnum.Reason_8:
                    return "عنوان آگهی مورد تایید نمی باشد";
                case NotVerifyReasonsEnum.Reason_9:
                    return "لطفا جهت بازخورد بهتر در هر اگهی عکس و اطلاعات *یک* اقامتگاه را کامل ثبت کنید";
                case NotVerifyReasonsEnum.Reason_10:
                    return "لطفا فقط عکس اقامتگاه مربوط به اگهی را اپلود کنید";
                case NotVerifyReasonsEnum.Reason_11:
                    return "لطفا عنوانی متناسب با اگهی انتخاب کنید";
                case NotVerifyReasonsEnum.Reason_12:
                    return "لطفا عکس ها را جداگانه آپلود کنید";
                case NotVerifyReasonsEnum.Reason_13:
                    return "کاربر گرامی عکسهای لوگو دار مورد تایید نمیباشند";
                case NotVerifyReasonsEnum.Reason_14:
                    return "عکس  با کادر مشکی مورد تایید نمیباشد";
                case NotVerifyReasonsEnum.Reason_15:
                    return "درج لینک در اگهی مجاز نمیباشد اگر مایل به تبلیغ وبسایت خود هستید میتوانید از طریق خرید بنر اقدام نماید. در صورت نیاز به اطلاعات بیشتر با 02632565304 تماس بگیرید";
                case NotVerifyReasonsEnum.Reason_16:
                    return "جهت راهنمایی و یا اطلاعات بیشتر با شماره 02632565304 تماس حاصل فرمایید و یا به تلگرام 09360263804 پیام ارسال کنید";
                case NotVerifyReasonsEnum.Reason_17:
                    return "اقامتگاه ثبت شده مورد تایید نمیباشد";
                case NotVerifyReasonsEnum.Reason_18:
                    return "درج لینک سایت ، کانال و... در اگهی امکان پذیر نمیباشد";
                case NotVerifyReasonsEnum.Reason_19:
                    return "شماره تماس شما در اگهی نیست لطفا با ما تماس بگیرید 02632565304";
                case NotVerifyReasonsEnum.Reason_20:
                    return "لطفا شهر و منطقه اقامتگاه را به درستی وارد کنید";
                case NotVerifyReasonsEnum.Reason_21:
                    return "آگهی تکراری مورد تایید نیست ، میتوانید از پنل ویژه یا بروزرسانی جهت بازدید بیشتر اگهی استفاده کنید";
                case NotVerifyReasonsEnum.Reason_22:
                    return "لطفا اطلاعات پروفایل خود را صحیح وارد کنید";
                case NotVerifyReasonsEnum.Reason_23:
                    return "لطفا اطلاعات پروفایل خود را کامل کنید ،برای تکمیل اطلاعات پروفایل وارد حساب من شوید از منو پروفایل را انتخاب کنید";
                case NotVerifyReasonsEnum.Reason_24:
                    return "این اقامتگاه توسط مالک قبلا ثبت گردیده";
                case NotVerifyReasonsEnum.Reason_25:
                    return "لطفا از همه ی فضاهای اقامتگاه عکس اپلود کنید";
                case NotVerifyReasonsEnum.Reason_26:
                    return "این اقامتگاه قبلا در سایت ثبت گردیده، اگر مالک اقامتگاه هستید لطفا مدرکی دال بر مالکیت ارسال نمایید ، تا اگهی به حساب شما انتقال یابد";
                case NotVerifyReasonsEnum.Reason_27:
                    return "لطفا ادرس را به درستی وارد کنید";
                case NotVerifyReasonsEnum.Reason_28:
                    return "اگهی شما به دلیل عدم پاسخگویی شما به تلفنتان تایید نشد";
                case NotVerifyReasonsEnum.Reason_29:
                    return "لطفا قیمت واحد را به روز کنید";
                case NotVerifyReasonsEnum.Reason_30:
                    return "درج آپارتمان غیر مبله امکان پذیر نیست";
                case NotVerifyReasonsEnum.Reason_31:
                    return "لطفا مدرک مالکیت واحد را ارسال نمایید";
                case NotVerifyReasonsEnum.Reason_32:
                    return "عدم همکاری شما در پاسخگویی به درخواست رزرو";
                case NotVerifyReasonsEnum.Reason_33:
                    return "لطفا دسته بندی (نوع اقامتگاه) آگهی را به درستی وارد کنید";
                default:
                    return "";
            }
        }

        public static string FilteredAddress(string address)
        {
            if (address == null)
                return "";
            if (address.Contains("پلاک"))
            {
                int indexOfExp = address.IndexOf("پلاک");
                if (indexOfExp >= 0)
                    address = address.Remove(indexOfExp);
            }
            else if (address.Contains(" پ "))
            {
                int indexOfExp = address.IndexOf(" پ ");
                if (indexOfExp >= 0)
                    address = address.Remove(indexOfExp);
            }
            return address;
        }

        public static string GetPhotoPersianTitle(Advertise.AdvertiseType type, Advertise.AdvertiseMode mode)
        {
            switch (mode)
            {
                case AdvertiseMode.Single:
                    switch (type)
                    {
                        case AdvertiseType.Apartment:
                            return "آپارتمان";
                        case AdvertiseType.Villa:
                            return "ویلا";
                        case AdvertiseType.SuitAndRoom:
                            return "سوییت";
                        case AdvertiseType.House:
                            return "خانه ویلایی";
                        case AdvertiseType.Hut:
                            return "کلبه";
                        default:
                            return null;
                    }
                case AdvertiseMode.Parent:
                    switch (type)
                    {
                        case AdvertiseType.Hotel:
                            return "هتل";
                        case AdvertiseType.Camp:
                            return "کمپ";
                        case AdvertiseType.TourismAccommodation:
                            return "اقامتگاه بومگردی";
                        case AdvertiseType.HotelApartment:
                            return "هتل آپارتمان";
                        case AdvertiseType.Inn:
                            return "مسافرخانه";
                        case AdvertiseType.Pansion:
                            return "پانسیون";
                        case AdvertiseType.Complex:
                            return "مجتمع";
                        default:
                            return null;
                    }
                case AdvertiseMode.Child:
                    switch (type)
                    {
                        case AdvertiseType.Apartment:
                            return "آپارتمان";
                        case AdvertiseType.Villa:
                            return "ویلا";
                        case AdvertiseType.SuitAndRoom:
                            return "سوییت";
                        case AdvertiseType.House:
                            return "خانه ویلایی";
                        case AdvertiseType.Hut:
                            return "کلبه";
                        default:
                            return null;
                    }
                default:
                    return null;
            }
        }

        public static IList<string> GetReserveCancelationRules()
        {
            var rules = new List<string>();
            rules.Add("کنسل نمودن رزرو توسط مهمان تا ۷۲ ساعت مانده به شروع اقامت: کسر ۱۰٪ از مبلغ کل رزرو و بازگشت باقی‌مانده مبلغ");
            rules.Add("کنسل نمودن رزرو توسط مهمان کمتر از ۷۲ ساعت مانده به شروع اقامت: کسر ۱۰٪ از مبلغ کل و مبلغ اولین شب رزرو و بازگشت باقی‌مانده مبلغ");
            rules.Add("کنسل نمودن رزرو توسط مهمان در روز شروع اقامت: کسر ۱۰٪ از مبلغ کل و مبلغ ۲ شب اول رزرو و بازگشت باقی‌مانده مبلغ");
            rules.Add("در ایام پیک تعطیلات، بازه‌ی ۷۲ ساعت، ۱ هفته محاسبه شده و امکان کنسلی وجود ندارد");
            return rules;
        }

        public static IList<string> GetNowruzReserveCancelationRules()
        {
            var rules = new List<string>();
            rules.Add("رزروهای مربوط به ایام نوروز فقط با رضایت میزبان قابل لغو می باشند");
            rules.Add("همچنین در صورتی که به دستور مقامات و سازمان های دولتی و به دلیل همه گیری ویروس کرونا، امکان سفر به مقصد مورد نظر طی روز های نوروز میسر نباشد، کلیه مبلغ رزرو به مهمان عودت می گردد");
            return rules;
        }

        public static string GetHygieneProtocolStatusPersianName(HygieneProtocolStatus hygieneProtocolStatus)
        {
            switch (hygieneProtocolStatus)
            {
                case HygieneProtocolStatus.NotConsider:
                    return "عدم رعایت";
                case HygieneProtocolStatus.Consider:
                    return "رعایت پروتکل ها";
                case HygieneProtocolStatus.Verified:
                    return "تایید پشتیبان";
                case HygieneProtocolStatus.NotVerified:
                    return "عدم تایید";
                default:
                    return "";
            }
        }
    }
}
