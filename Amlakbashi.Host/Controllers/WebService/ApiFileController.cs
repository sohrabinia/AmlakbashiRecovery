using Amlakbashi.Application.Services.AdvertiseServices.Interfaces;
using Amlakbashi.Application.Services.FileServices.Interfaces;
using Amlakbashi.Application.Services.UserServices.Interfaces;
using Amlakbashi.Core.DTOs.WebService.Requests.Files;
using Amlakbashi.Core.Entities;
using Amlakbashi.Host.Authentication;
using Amlakbashi.Host.Controllers.Base;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Amlakbashi.Host.Controllers.WebService
{
    [ApiController]
    [Route("api/file")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ApiFileController : ApiBaseController
    {
        private readonly IFileAppService fileService;
        private readonly IUserAppService userService;
        private readonly IUserAccessor userAccessor;
        private readonly IWebHostEnvironment webHostEnvironment;
        public ApiFileController(IFileAppService fileService,
            IUserAppService userService,
            IUserAccessor userAccessor,
            IWebHostEnvironment webHostEnvironment)
        {
            this.fileService = fileService;
            this.userService = userService;
            this.userAccessor = userAccessor;
            this.webHostEnvironment = webHostEnvironment;
        }

        [HttpGet("user")]
        public IActionResult GetCurrentUserProfileImage()
        {
            if (userAccessor.CurrentUser.PhotoID == null)
            {
                return NotFound();
            }
            return File(userAccessor.CurrentUser.Photo.FilePathWithoutTildeAndSlash);
        }

        [AllowAnonymous]
        [HttpGet("user/{userId:int}")]
        public IActionResult GetUserProfileImage(int userId)
        {
            var user = userService.Find(userId);
            if (user == null || user.PhotoID == null)
            {
                return NotFound();
            }
            return File(user.Photo.FilePathWithoutTildeAndSlash);
        }

        [AllowAnonymous]
        [HttpGet("advertise/{advertiseId:long}/{fileId:long}")]
        public IActionResult GetAdvertiseImage(long advertiseId, long fileId)
        {
            var path = Advertise.GetImageFileAddress(advertiseId, fileId, Advertise.ImageType.Xxxlarge);
            return File(path);
        }

        [HttpPost("advertise")]
        public IActionResult AddAdvertiseImage([FromForm] FilePostAdvertiseImagesRequest request,
            [FromServices] IAdvertiseAppService advertiseService)
        {
            if (request.IsValid(ModelState) == false)
            {
                return BadRequest(ModelState);
            }
            request.userId = userAccessor.CurrentUser.Id;
            var result = fileService.AddAdvertiseImages(request);
            if (result.HasError())
            {
                return BadRequest(result.GetErrors());
            }
            advertiseService.UpdateAlbumPhoto(request.advertiseId);
            return Created("", "");
        }

        //[HttpDelete("advertise")]
        //public IActionResult RemoveAdvertiseImage()
        //{

        //}

        //[HttpPost("advertise/license")]
        //public IActionResult AddAdvertiseLicenseImage(long advertiseId, IFormFile licenseImage)
        //{

        //}

        private IActionResult File(string fileAddress)
        {
            if (System.IO.File.Exists(Path.Combine(webHostEnvironment.WebRootPath, fileAddress)) == false)
            {
                return NotFound();
            }
            return File(fileAddress, "image/jpeg");
        }
    }
}
