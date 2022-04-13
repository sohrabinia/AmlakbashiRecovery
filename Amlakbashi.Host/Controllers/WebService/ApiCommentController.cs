using Amlakbashi.Application.Services.AdvertiseServices.Interfaces;
using Amlakbashi.Application.Services.CommentServices.Interfaces;
using Amlakbashi.Core.DTOs.WebService.Requests.Comments;
using Amlakbashi.Core.Entities;
using Amlakbashi.Host.Authentication;
using Amlakbashi.Host.Controllers.Base;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Amlakbashi.Host.Controllers.WebService
{
    [ApiController]
    [Route("api/comment")]
    public class ApiCommentController : ApiBaseController
    {
        private readonly ICommentAppService commentService;
        private readonly IReportItemAppService reportItemService;
        private readonly IAdvertiseAppService advertiseService;
        private readonly IUserAccessor userAccessor;
        public ApiCommentController(ICommentAppService commentService,
            IReportItemAppService reportItemService,
            IAdvertiseAppService advertiseService,
            IUserAccessor userAccessor)
        {
            this.commentService = commentService;
            this.reportItemService = reportItemService;
            this.advertiseService = advertiseService;
            this.userAccessor = userAccessor;
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost]
        public IActionResult Submit(CommentPostRequest request)
        {
            var advertise = advertiseService.Find(request.advertiseId);
            var canUserSetComment = advertise?.Reserves.Any(x => x.UserID == userAccessor.CurrentUser.Id &&
                    x.Status == Reserve.ReserveStatus.Completed && x.EndDate.Date.AddDays(30) >= DateTime.Now.Date);
            if (advertise == null || canUserSetComment != true)
            {
                return BadRequest();
            }

            commentService.Submit(userAccessor.CurrentUser.Id, request.advertiseId, request.text);
            reportItemService.Submit(userAccessor.CurrentUser.Id, request.advertiseId, request.scores);
            return Created("", null);
        }
    }
}
