using Amlakbashi.Application.Services.ReserveServices.Interfaces;
using Amlakbashi.Core.DTOs.WebService.Requests.Chats;
using Amlakbashi.Core.DTOs.WebService.Responses.Chats;
using Amlakbashi.Core.Entities;
using Amlakbashi.Host.Authentication;
using Amlakbashi.Host.Controllers.Base;
using Amlakbashi.Host.Hubs.Admin.HubServers;
using Amlakbashi.Host.Hubs.Dashboard.HubServers;
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
    [Route("api/chat")]
    public class ApiChatController : ApiBaseController
    {
        private readonly IChatAppService chatService;
        private readonly IReserveAppService reserveService;
        private readonly IReserveAutoCancelAppService reserveAutoCancelService;
        private readonly IUserAccessor userAccessor;
        private readonly IReserveDashboardHubServer reserveDashboardHubServer;
        private readonly IReserveAdminHubServer reserveAdminHubServer;
        public ApiChatController(IChatAppService chatService,
            IReserveAutoCancelAppService reserveAutoCancelService,
            IUserAccessor userAccessor,
            IReserveDashboardHubServer reserveDashboardHubServer,
            IReserveAdminHubServer reserveAdminHubServer)
        {
            this.chatService = chatService;
            this.reserveAutoCancelService = reserveAutoCancelService;
            this.userAccessor = userAccessor;
            this.reserveDashboardHubServer = reserveDashboardHubServer;
            this.reserveAdminHubServer = reserveAdminHubServer;
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("{reserveId:{long}")]
        public IList<ChatResponse> Get(long reserveId)
        {
            var chats = chatService.GetReserveChats(reserveId);
            if (chats != null)
            {
                chatService.UpdateReserveChatsReadStatus(reserveId, userAccessor.CurrentUser.Id);
                reserveDashboardHubServer.ReloadChatFromServer(reserveId);
            }

            List<ChatResponse> response = new List<ChatResponse>();
            response.AddRange(chats.Select(x => new ChatResponse()
            {
                message = x.Text,
                time = $"{x.CreateTime.Hour}:{x.CreateTime.Minute}",
                viewed = x.IsViewed == Chat.ReadStatusEnum.Read,
                forUser = x.UserID == userAccessor.CurrentUser.Id
            }));
            return response;
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost]
        public IActionResult Post(ChatPostMessageRequest request)
        {
            var result = chatService.Insert(request.reserveId, userAccessor.CurrentUser.Id, request.message);
            if (result.IsValid == false)
            {
                return BadRequest(result.ErrorMessages);
            }
            var reserve = reserveService.Find(request.reserveId);
            if (reserve.InstantReserve == false)
            {
                reserveAutoCancelService.UpdateScheduledTime(request.reserveId);
            }
            var reserveChatCount = chatService.GetCountByReserveId(request.reserveId);
            reserveAdminHubServer.ChangeChatCountFromServer(request.reserveId, reserveChatCount,
                chatService.GetNotReadSupportCountByReserveId(request.reserveId));
            return CreatedAtAction($"/api/chat/{request.reserveId}", null);
        }
    }
}
