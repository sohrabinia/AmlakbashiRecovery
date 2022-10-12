using Amlakbashi.Application.Services.SupportChatServices.Interfaces;
using Amlakbashi.Application.Services.UserServices.Interfaces;
using Amlakbashi.Core.Common.StaticData;
using Amlakbashi.Core.DTOs.WebService.Requests.SupportChats;
using Amlakbashi.Core.DTOs.WebService.Responses.SupportChats;
using Amlakbashi.Core.Entities;
using Amlakbashi.Core.Infrastructure.LocalizationHelpers;
using Amlakbashi.Host.Controllers.Base;
using Amlakbashi.Host.Extensions;
using Amlakbashi.Host.Hubs.Admin.HubServers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Amlakbashi.Host.Controllers.WebService
{
    [ApiController]
    [Route("api/supportchat")]
    public class ApiSupportChatController : ApiBaseController
    {
        private readonly ISupportChatAppService supportChatService;
        private readonly ISupportChatMessageAppService supportChatMessageService;
        private readonly IUserAppService userService;
        private readonly ISupportChatAdminHubServer supportChatAdminHubServer;
        public ApiSupportChatController(ISupportChatAppService supportChatService,
            ISupportChatMessageAppService supportChatMessageService,
            IUserAppService userService,
            ISupportChatAdminHubServer supportChatAdminHubServer)
        {
            this.supportChatService = supportChatService;
            this.supportChatMessageService = supportChatMessageService;
            this.userService = userService;
            this.supportChatAdminHubServer = supportChatAdminHubServer;
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet]
        public SupportChatResponse Get()
        {
            SupportChat supportChat = supportChatService.GetByUserId(User.GetId());
            if (supportChat != null)
            {
                var supportChatMessageIds = supportChatService.UpdateMessagesReadStatus(supportChat.Id);
                foreach (var item in supportChatMessageIds)
                {
                    supportChatAdminHubServer.UpdateChatMessageFromServer(supportChat.Id, item);
                }
            }

            var response = new SupportChatResponse();
            if (supportChat != null && supportChat.Messages.Any())
            {
                response.messages.AddRange(supportChat.Messages.Select(x => new SupportChatMessageResponse()
                {
                    message = x.Text,
                    time = $"{x.CreateTime.Hour}:{x.CreateTime.Minute}",
                    forUser = x.Type == SupportChatMessage.TypeEnum.User,
                    viewed = x.ReadStatus == SupportChatMessage.ReadStatusEnum.Read
                }));
            }
            return response;
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public IActionResult Post(SupportChatPostMessageRequest request)
        {
            SupportChat supportChat = supportChatService.GetByUserId(User.GetId());
            if (supportChat == null)
            {
                supportChat = supportChatService.Insert(User.GetId());
            }

            var messageId = supportChatMessageService.Insert(request.message, SupportChatMessage.TypeEnum.User,
                supportChat.Id, User.GetId());
            supportChatAdminHubServer.AddChatMessageFromServer(supportChat.Id, messageId);

            string autoMessage = string.Empty;
            if (GeneralData.IsSupportersOnline())
            {
                var allSupportEmployees = userService.GetAllSupportEmployees();
                var supporterNotifs = userService.IdentityUsersToUsers(allSupportEmployees)
                    .Select(s => s.NotificationToken).ToArray();
                supportChatService.ScheduleSendSupporterNewMsgNotif(3, messageId, supportChat.Id, supporterNotifs);
            }
            else
            {
                autoMessage = SupportChatLocalization.GetClosedCompanyMessage();
                var msgId = supportChatMessageService.Insert(autoMessage, SupportChatMessage.TypeEnum.Supporter,
                    supportChat.Id, null, SupportChatMessage.ReadStatusEnum.Read);
                supportChatAdminHubServer.AddChatMessageFromServer(supportChat.Id, msgId);
            }
            return Created("/api/supportchat", new { message = autoMessage });
        }

        [HttpGet("faq")]
        public IActionResult GetFaq()
        {
            var supportChatAutoQuestions = Enum.GetValues<SupportChat.AutoQuestion>().ToList();
            Dictionary<string, string> questions = new Dictionary<string, string>();
            foreach (var item in supportChatAutoQuestions)
            {
                questions.Add(SupportChatLocalization.GetQuestionTitle(item),
                    SupportChatLocalization.GetQuestionText(item));
            }
            return Ok(questions);
        }
    }
}
