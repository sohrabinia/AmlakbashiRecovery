using Amlakbashi.Application.Services.AdvertiseServices.Interfaces;
using Amlakbashi.Core.DTOs.TagDTOs;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Amlakbashi.Host.Areas.App.Controllers
{
    [Area("App")]
    [Route("app/tags/[action]")]
    public class AppTagsController : Controller
    {
        private readonly ITagAppService tagService;
        public AppTagsController(ITagAppService tagService)
        {
            this.tagService = tagService;
        }

        [HttpGet("{urltitle}/{page=1}")]
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
