using Amlakbashi.Application.Services.AdvertiseServices.Interfaces;
using Amlakbashi.Application.Services.CommentServices.Interfaces;
using Amlakbashi.Core.DTOs.WebService.Requests.Comments;
using Amlakbashi.Core.Entities;
using Amlakbashi.Host.Controllers.Base;
using Amlakbashi.Host.Extensions;
using Amlakbashi.Host.Filters;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

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
        public ApiCommentController(ICommentAppService commentService,
            IReportItemAppService reportItemService,
            IAdvertiseAppService advertiseService)
        {
            this.commentService = commentService;
            this.reportItemService = reportItemService;
            this.advertiseService = advertiseService;
        }
        
        [HttpGet]
        [Panel(Core.Entities.User.UserGeneralTypeEnum.Host)]
        public IActionResult GetForHostPanel(bool seenByHost = true, int page = 1, int pageItemCount = 20)
        {
            var response = commentService.GetForHost(User.GetId(), seenByHost, page, pageItemCount);
            return Ok(response);
        }

        [HttpPost("guest")]
        [Panel(Core.Entities.User.UserGeneralTypeEnum.Guest)]
        public IActionResult SubmitGuestComment(CommentPostGuestRequest request)
        {
            var advertise = advertiseService.Find(request.advertiseId);
            var canUserSetComment = advertise?.Reserves.Any(x => x.UserID == User.GetId() &&
                    x.Status == Reserve.ReserveStatus.Completed && x.EndDate.Date.AddDays(30) >= DateTime.Now.Date);
            if (advertise == null || canUserSetComment != true)
            {
                return BadRequest();
            }

            commentService.SubmitGuestComment(User.GetId(), request.advertiseId, request.text);
            reportItemService.Submit(User.GetId(), request.advertiseId, request.scores);
            return Created("", null);
        }

        [HttpPost("host")]
        [Panel(Core.Entities.User.UserGeneralTypeEnum.Host)]
        public IActionResult SubmitHostReply(CommentPostHostRequest request)
        {
            request.userId = User.GetId();
            var result = commentService.SubmitHostReply(request);
            if (result.HasError())
            {
                return BadRequest(result.GetErrors());
            }
            return Created("", null);
        }
    }
}
