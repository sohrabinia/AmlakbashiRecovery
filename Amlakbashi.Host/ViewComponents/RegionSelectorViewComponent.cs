using Amlakbashi.Application.Services.AdvertiseServices.Interfaces;
using Amlakbashi.Core.Entities;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using static Amlakbashi.Core.Entities.Region;

namespace Amlakbashi.Host.ViewComponents
{
    public class RegionSelectorViewComponent : ViewComponent
    {
        private readonly IRegionAppService regionService;
        public RegionSelectorViewComponent(IRegionAppService regionService)
        {
            this.regionService = regionService;
        }

        public IViewComponentResult Invoke(int province = -1, int city = -1, int area = -1,
            RegionStatus region_filter_status = RegionStatus.All,
            string region_filter_item_class = "dynamic-filter__item",
            string region_filter_label_class = "dynamic-filter__label",
            string region_filter_property_class = "dynamic-filter__property",
            string provinceName = "Province",
            string cityName = "City",
            string areaName = "Area",
            bool selectEnabled = true,
            bool mandatory = true,
            bool provinceError = false, bool cityError = false)
        {
            ViewBag.province = province;
            ViewBag.city = city;
            ViewBag.area = area;
            ViewBag.region_filter_status = region_filter_status;
            ViewBag.region_filter_item_class = region_filter_item_class;
            ViewBag.region_filter_label_class = region_filter_label_class;
            ViewBag.region_filter_property_class = region_filter_property_class;
            ViewBag.provinceName = provinceName;
            ViewBag.cityName = cityName;
            ViewBag.areaName = areaName;
            ViewBag.selectEnabled = selectEnabled;
            ViewBag.mandatory = mandatory;
            ViewBag.provinces = regionService.Filter(AdvertiseRegion.Province, 0,
                region_filter_status, RegionSortOrder.PersianName);
            ViewBag.cities = province > 0 ?
                regionService.Filter(AdvertiseRegion.City, province,
                region_filter_status, RegionSortOrder.PersianName) :
                new List<Region>();
            ViewBag.areas = city > 0 ?
                regionService.Filter(AdvertiseRegion.Area, city,
                region_filter_status, RegionSortOrder.PersianName) :
                new List<Region>();
            ViewBag.guid = Guid.NewGuid();
            ViewBag.provinceError = provinceError;
            ViewBag.cityError = cityError;
            return View();
        }
    }
}
