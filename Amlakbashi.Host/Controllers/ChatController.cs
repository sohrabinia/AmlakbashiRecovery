using Amlakbashi.Application.Services.AdvertiseServices.Interfaces;
using Amlakbashi.Application.Services.ReserveServices.Interfaces;
using Amlakbashi.Application.Services.UserServices.Interfaces;
using Amlakbashi.Core.DTOs.ReserveChatDTOs;
using Amlakbashi.Core.Entities;
using Amlakbashi.Core.Infrastructure.LocalizationHelpers;
using Amlakbashi.Host.Authentication;
using Amlakbashi.Host.Controllers.Base;
using Amlakbashi.Host.Hubs.Admin.HubServers;
using Amlakbashi.Host.Hubs.Dashboard.HubServers;
using log4net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using X.PagedList;

namespace Amlakbashi.Host.Controllers
{
    public class ChatController : BaseController
    {
        private readonly ILog logger;
        private readonly IUserAppService userService;
        private readonly IAdvertiseAppService advertiseService;
        private readonly IChatAppService chatService;
        private readonly IReserveAppService reserveService;
        private readonly IUserAccessor userAccessor;
        private readonly IReserveDashboardHubServer reserveDashboardHubServer;
        private readonly IReserveAdminHubServer reserveAdminHubServer;
        public ChatController(ILog logger,
            IUserAppService userService,
            IReserveAppService reserveService,
            IChatAppService chatService,
            IAdvertiseAppService advertiseService,
            IUserAccessor userAccessor,
            IReserveAdminHubServer reserveAdminHubServer,
            IReserveDashboardHubServer reserveDashboardHubServer)
        {
            this.logger = logger;
            this.chatService = chatService;
            this.userService = userService;
            this.reserveService = reserveService;
            this.advertiseService = advertiseService;
            this.userAccessor = userAccessor;
            this.reserveDashboardHubServer = reserveDashboardHubServer;
            this.reserveAdminHubServer = reserveAdminHubServer;
        }

        [Auth(UserRoles.Admin)]
        public ActionResult Index(int? page, long chat_id = -1, long reserve_id = -1,
            int user_id = -1, int chat_status = -1)
        {
            try
            {
                var model = chatService.Filter(chat_id, reserve_id, user_id, chat_status);

                var PageNumber = page ?? 1;
                var onePageOfModel = model.ToPagedList(PageNumber, 10);

                ViewBag.chat_id = chat_id;
                ViewBag.reserve_id = reserve_id;
                ViewBag.user_id = user_id;
                ViewBag.chat_status = chat_status;

                ViewBag.RowIndexStart = (PageNumber * 10) - 10;
                return View(onePageOfModel);
            }
            catch (Exception exc)
            {
                logger.Error("Chat.Index", exc);
                return Redirect(Request.Headers["Referer"].ToString());
            }
        }

        [Auth(UserRoles.Admin)]
        [HttpGet]
        public ActionResult Edit(int chat_id = -1)
        {
            try
            {
                ViewBag.msg = TempData["msg"];
                var model = chatService.Find(chat_id);
                return View(model);
            }
            catch (Exception exc)
            {
                logger.Error("Chat.Edit(get)", exc);
                return Redirect(Request.Headers["Referer"].ToString());
            }
        }

        [Auth(UserRoles.Admin)]
        [HttpPost]
        public ActionResult Edit(Chat chat)
        {
            try
            {
                if (string.IsNullOrEmpty(chat.Text))
                {
                    TempData["msg"] = "متن پیام نمیتواند خالی باشد .";
                    return RedirectToAction("Edit");
                }
                else
                {
                    chatService.Update(chat);
                }
                return RedirectToAction("Index");
            }
            catch (Exception exc)
            {
                logger.Error("Chat.Edit(post)", exc);
                return Redirect(Request.Headers["Referer"].ToString());
            }
        }

        [Auth(UserRoles.Admin)]
        public JsonResult Delete(long chat_id)
        {
            try
            {
                chatService.Delete(chat_id);
                return GenerateJsonResult(new
                {
                    status = 1,
                    val = ""
                });
            }
            catch (Exception exc)
            {
                logger.Error("Chat.Delete", exc);
                return GenerateJsonResult(new
                {
                    status = 0,
                    val = ""
                });
            }
        }

        [Authorize]
        public ActionResult GetGuestChatPopup(long reserve_id)
        {
            var reserve = reserveService.Find(reserve_id);
            if (userAccessor.CurrentUser.Id != reserve.UserID)
                return null;
            return GetChatPopup(reserve_id);
        }

        [Authorize]
        public ActionResult GetHostChatPopup(long reserve_id)
        {
            var reserve = reserveService.Find(reserve_id);
            if (userAccessor.CurrentUser.Id != reserve.Advertise.UserID)
                return null;
            return GetChatPopup(reserve_id);
        }

        public ActionResult GetChatPopup(long reserve_id)
        {
            var currentUserId = userAccessor.CurrentUser.Id;
            var reserve = reserveService.Find(reserve_id);
            var chats = chatService.GetReserveChats(reserve_id);
            var notReadChats = chats.Where(
                x => x.ReadStatus == (int)Chat.ReadStatusEnum.NotRead &&
                x.UserID != currentUserId).ToList();
            bool any_change = false;
            foreach (var chat in notReadChats)
            {
                any_change = any_change || chat.ReadStatus != (int)Chat.ReadStatusEnum.Read;
                chat.ReadStatus = (int)Chat.ReadStatusEnum.Read;
            }
            if (any_change)
            {
                chatService.UpdateChatListReadStatus(notReadChats);
                reserveDashboardHubServer.ReloadChatFromServer(reserve_id);
                reserveDashboardHubServer.ReloadReserveItemsFromServer(reserve_id);
            }
            var model = ReserveChatPopupDTO.Generate(chats.ToList(),
                userAccessor.CurrentUser.Id, reserve.UserID, advertiseService.GetAllAsIQueriable());

            return PartialView("_ChatPopup", model);
        }

        public JsonResult SendText(long reserve_id, string text)
        {
            try
            {
                var reserve = reserveService.Find(reserve_id);
                var user_id = userAccessor.CurrentUser.Id;
                var guest_user_id = reserve.UserID;
                var host_user_id = reserve.Advertise.UserID;
                var is_guest = user_id == guest_user_id;

                if (user_id != guest_user_id && user_id != host_user_id)
                {
                    return GenerateJsonResult(new
                    {
                        status = 0
                    });
                }
                string outputText;
                var has_forbidden_characters = ChatLocalization.
                    TextHasForbiddenCharacters(text, out outputText,
                    advertiseService.GetAllAsIQueriable());
                //if (_db.Chats.Count(x => x.ReserveID == reserve_id &&
                //    x.ChatStatus != (int)Chat.ChatStatusEnum.Deleted) == 0)
                //{
                //    var target_user_id = is_guest ? host_user_id : guest_user_id;
                //    var target_user = _db.Users.Find(target_user_id);
                //    //var mobile = _db.Users.Find(target_user_id).UserName;
                //    //SmsEngine.VerifyLookup(mobile, user_id.ToString(), advertise.AdvertiseID.ToString(), /*advertise.Title*/"", is_guest ? "NewReserveChatHost" : "NewReserveChatGuest");
                //    UserContact.SendMessage(target_user, is_guest ? UserContactType.NewReserveChatHost : UserContactType.NewReserveChatGuest, reserve.AdvertiseID.ToString(), is_guest ? guest_user_id.ToString() : host_user_id.ToString(), reserve_id.ToString());
                //}
                var msg = new Chat()
                {
                    ReserveID = reserve_id,
                    UserID = user_id,
                    CreateTime = DateTime.Now,
                    Text = text,
                    ChatStatus = has_forbidden_characters ? (int)Chat.ChatStatusEnum.HasForbiddenCharacters : (int)Chat.ChatStatusEnum.Sent
                };
                var chat = chatService.Insert(msg);
                var reserveChatCount = chatService.GetCountByReserveId(reserve_id);
                reserveAdminHubServer.ChangeChatCountFromServer(reserve_id, reserveChatCount,
                    chatService.GetNotReadSupportCountByReserveId(reserve_id));
                var target_user_id = is_guest ? host_user_id : guest_user_id;
                var target_user = userService.Find(target_user_id);
                if (!string.IsNullOrEmpty(target_user.FcmAppNotificationToken) ||
                    !string.IsNullOrEmpty(target_user.AppNotificationToken) ||
                    !string.IsNullOrEmpty(target_user.NotificationToken))
                {
                    var is_first_msg = reserveChatCount < 1;
                    chatService.ScheduleChatNotification(chat.Id,
                        target_user_id, !is_guest,
                        is_guest ? guest_user_id : host_user_id, is_first_msg);
                }
                if (has_forbidden_characters)
                {
                    return GenerateJsonResult(new
                    {
                        status = 2
                    });
                }
                return GenerateJsonResult(new
                {
                    status = 1
                });
            }
            catch (Exception exc)
            {
                logger.Error("Chat.SendText", exc);
                return GenerateJsonResult(new
                {
                    status = 0
                });
            }
        }
    }
}
