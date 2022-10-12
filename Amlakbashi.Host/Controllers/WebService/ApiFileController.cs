using Amlakbashi.Application.Services.AdvertiseServices.Interfaces;
using Amlakbashi.Application.Services.FileServices.Interfaces;
using Amlakbashi.Application.Services.UserServices.Interfaces;
using Amlakbashi.Core.DTOs.WebService.Requests.Files;
using Amlakbashi.Core.Entities;
using Amlakbashi.Host.Controllers.Base;
using Amlakbashi.Host.Extensions;
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
        private readonly IWebHostEnvironment webHostEnvironment;
        public ApiFileController(IFileAppService fileService,
            IUserAppService userService,
            IWebHostEnvironment webHostEnvironment)
        {
            this.fileService = fileService;
            this.userService = userService;
            this.webHostEnvironment = webHostEnvironment;
        }

        [HttpGet("user")]
        public IActionResult GetUserProfileImage()
        {
            var user = userService.Find(User.GetId());
            if (user.PhotoID == null)
            {
                return NotFound();
            }
            return File(user.Photo.CorrectedFilePath);
        }

        [AllowAnonymous]
        [HttpGet("user/{userId:int}")]
        public IActionResult GetUserProfileImage(int userId)
        {
            var user = userService.Find(userId);
            if (user == null || user.PhotoID == null || user.PhotoStatus != 2)
            {
                return NotFound();
            }
            return File(user.Photo.CorrectedFilePath);
        }

        [HttpPost("user")]
        public async Task<IActionResult> UpdateUserProfileImage(IFormFile image)
        {
            if (image == null || Core.Entities.File.IsValidImageContentType(image.ContentType) == false)
            {
                return BadRequest();
            }
            var result = await fileService.UpdateUserProfileImageAsync(User.GetId(), image);
            if (result.HasError())
            {
                return BadRequest(result.GetErrors());
            }
            userService.UpdateProfilePhoto(User.GetId(), result.Result,
                Core.Entities.User.UserPhotoState.ready_publish);
            return CreatedAtAction(nameof(GetUserProfileImage), null);
        }

        [AllowAnonymous]
        [HttpGet("advertise/{advertiseId:long}/{fileId:long}")]
        public IActionResult GetAdvertiseImage(long advertiseId, long fileId)
        {
            var path = Advertise.GetImageFileAddress(advertiseId, fileId, Advertise.ImageType.Xxxlarge);
            return File(path);
        }

        [HttpPost("advertise")]
        public async Task<IActionResult> AddAdvertiseImage([FromForm] FilePostAdvertiseImagesRequest request,
            [FromServices] IAdvertiseAppService advertiseService)
        {
            if (request.IsValid(ModelState) == false)
            {
                return BadRequest(ModelState);
            }
            request.userId = User.GetId();
            var result = await fileService.AddAdvertiseImagesAsync(request);
            if (result.HasError())
            {
                return BadRequest(result.GetErrors());
            }
            advertiseService.UpdateAlbumPhoto(request.advertiseId);
            return Created("", "");
        }

        [HttpDelete("advertise/{advertiseId:long}/{fileId:long}")]
        public async Task<IActionResult> DeleteAdvertiseImage(long advertiseId, long fileId,
            [FromServices] IAdvertiseAppService advertiseService)
        {
            var result = await fileService.DeleteAdvertiseImage(advertiseId, fileId, User.GetId());
            if (result.HasError())
            {
                return BadRequest(result.GetErrors());
            }
            advertiseService.UpdateAlbumPhoto(advertiseId);
            return Ok();
        }

        [HttpGet("advertise/license/{advertiseId:long}")]
        public IActionResult GetAdvertiseLicenseImage(long advertiseId, [FromServices] IAdvertiseAppService advertiseService)
        {
            var advertise = advertiseService.Find(advertiseId);
            if (advertise == null || advertise.LicenseFile == null)
            {
                return NotFound();
            }
            return File(advertise.LicenseFile.CorrectedFilePath);
        }

        [HttpPost("advertise/license")]
        public async Task<IActionResult> UpdateAdvertiseLicenseImage([FromForm] FilePostAdvertiseLicenseImageRequest request)
        {
            if (request.IsValid(ModelState) == false)
            {
                return BadRequest(ModelState);
            }
            request.userId = User.GetId();
            var result = await fileService.UpdateAdvertiseLicenseImageAsync(request);
            if (result.HasError())
            {
                return BadRequest(result.GetErrors());
            }
            return Created("", "");
        }

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
