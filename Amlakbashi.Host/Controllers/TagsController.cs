using Amlakbashi.Application.Services.AdvertiseServices.Interfaces;
using Amlakbashi.Core.DTOs.TagDTOs;
using Amlakbashi.Core.Identity;
using Amlakbashi.Host.Controllers.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace Amlakbashi.Host.Controllers
{
    public class TagsController : BaseController
    {
        private readonly ITagAppService tagService;
        public TagsController(ITagAppService tagService)
        {
            this.tagService = tagService;
        }

        [Authorize(Policy = Policies.Advertise_Edit)]
        public async Task<IActionResult> List(TagListDTO dto)
        {
            await tagService.GetListAsync(dto);
            return View(dto);
        }

        [Authorize(Policy = Policies.Advertise_Edit)]
        [HttpPost]
        public async Task<IActionResult> Add(string title)
        {
            var result = await tagService.AddByAdminAsync(title, Core.Entities.Tag.TagStatusEnum.Active);
            return GenerateResult(result);
        }

        [Authorize(Policy = Policies.Advertise_Edit)]
        [HttpPost]
        public async Task<IActionResult> EditTitle(int id, string title)
        {
            var result = await tagService.UpdateTitleAsync(id, title);
            return GenerateResult(result);
        }

        [Authorize(Policy = Policies.Advertise_Edit)]
        [HttpPost]
        public async Task<IActionResult> Activate(int id)
        {
            var result = await tagService.UpdateStatusAsync(id, Core.Entities.Tag.TagStatusEnum.Active);
            return GenerateResult(result);
        }

        [Authorize(Policy = Policies.Advertise_Edit)]
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await tagService.DeleteAsync(id);
            return GenerateResult(result);
        }

        public async Task<IActionResult> Search(string title, string cityName = null)
        {
            var searchedTags = await tagService.GetListAsync(title);
            if (string.IsNullOrEmpty(cityName) == false)
            {
                searchedTags = searchedTags.OrderByDescending(x => x.Title.Contains(cityName)).ToList();
            }
            searchedTags = searchedTags.Take(10).ToList();
            return View("_Search", searchedTags);
        }

        [HttpPost]
        public async Task<IActionResult> AddInResidence(int residenceId, string title)
        {
            var result = await tagService.AddByUserAsync(residenceId, title);
            return GenerateResult(result, new { id = result.Result?.Id, title = result.Result?.Title });
        }

        public async Task<IActionResult> GetResidences(TagResidencesDTO dto, bool ajax)
        {
            var result = await tagService.GetTagResidences(dto);
            if (result.CheckHasError)
            {
                return NotFound(result.FirstError);
            }
            if (ajax)
            {
                return PartialView("_TagResidencesList", dto);
            }
            return View(dto);
        }
    }
}
