using Amlakbashi.Application.Services.AdvertiseServices.Interfaces;
using Amlakbashi.Application.Services.CommentServices.Interfaces;
using Amlakbashi.Core.DTOs.WebService.Requests.Comments;
using Amlakbashi.Core.Entities;
using Amlakbashi.Host.Authentication;
using Amlakbashi.Host.Controllers.Base;
using Amlakbashi.Host.Filters;
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
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
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
        
        [HttpGet]
        [Panel(Core.Entities.User.UserGeneralTypeEnum.Host)]
        public IActionResult GetForHostPanel(bool seenByHost = true, int page = 1, int pageItemCount = 20)
        {
            var user = userAccessor.CurrentUser;
            if (user.UserGeneralType != 1)
            {
                return BadRequest();
            }
            var response = commentService.GetForHost(user.Id, seenByHost, page, pageItemCount);
            return Ok(response);
        }

        [HttpPost("guest")]
        [Panel(Core.Entities.User.UserGeneralTypeEnum.Guest)]
        public IActionResult SubmitGuestComment(CommentGuestSubmitRequest request)
        {
            var advertise = advertiseService.Find(request.advertiseId);
            var canUserSetComment = advertise?.Reserves.Any(x => x.UserID == userAccessor.CurrentUser.Id &&
                    x.Status == Reserve.ReserveStatus.Completed && x.EndDate.Date.AddDays(30) >= DateTime.Now.Date);
            if (advertise == null || canUserSetComment != true)
            {
                return BadRequest();
            }

            commentService.SubmitGuestComment(userAccessor.CurrentUser.Id, request.advertiseId, request.text);
            reportItemService.Submit(userAccessor.CurrentUser.Id, request.advertiseId, request.scores);
            return Created("", null);
        }

        [HttpPost("host")]
        [Panel(Core.Entities.User.UserGeneralTypeEnum.Host)]
        public IActionResult SubmitHostReply(CommentHostSubmitRequest request)
        {
            request.userId = userAccessor.CurrentUser.Id;
            var result = commentService.SubmitHostReply(request);
            if (result.HasError())
            {
                return BadRequest(result.GetErrors());
            }
            return Created("", null);
        }
    }
}
