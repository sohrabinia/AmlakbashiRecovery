using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amlakbashi.Application.Services.AdvertiseServices.Interfaces;
using Amlakbashi.Application.Services.Category.Interfaces;
using Amlakbashi.Core.DTOs.WebService.Responses.Regions;
using Amlakbashi.Core.Entities;
using Amlakbashi.Host.Controllers.Base;
using Microsoft.AspNetCore.Mvc;

namespace Amlakbashi.Host.Controllers.WebService
{
    [ApiController]
    [Route("api/region")]
    public class ApiRegionController : ApiBaseController
    {
        private readonly IRegionAppService regionService;
        private readonly ICategoryAppService categoryService;
        public ApiRegionController(IRegionAppService regionService,
            ICategoryAppService categoryService)
        {
            this.regionService = regionService;
            this.categoryService = categoryService;
        }

        [HttpGet]
        public IActionResult Get(string phrase)
        {
            var regions = regionService.GetBySearchRegion(phrase);
            if (regions.Any() == false)
            {
                return NotFound();
            }
            var response = new List<RegionListResponse>();
            foreach (var item in regions)
            {
                var responseItem = new RegionListResponse() {
                    regionId = item.Id,
                    residencyCount = categoryService.GetAdvertiseCount(item.Id, (Region.AdvertiseRegion)item.Type)
                };
                switch (item.Type)
                {
                    case 0:
                        responseItem.provinceName = item.PersianName;
                        break;
                    case 1:
                        responseItem.provinceName = item.Parent.PersianName; 
                        responseItem.cityName = item.PersianName;
                        break;
                    case 2:
                        responseItem.provinceName = item.Parent.Parent.PersianName;
                        responseItem.cityName = item.Parent.PersianName;
                        responseItem.areaName = item.PersianName;
                        break;
                    default:
                        break;
                }
                response.Add(responseItem);
            }
            return Ok(response);
        }
    }
}
