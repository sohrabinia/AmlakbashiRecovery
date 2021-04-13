using System;
using System.Collections.Generic;
using System.Linq;
using Amlakbashi.Core.Entities;
using Entities = Amlakbashi.Core.Entities;
using Amlakbashi.Core.DTOs.ReserveDTOs.ApiDTOs;
using Amlakbashi.Core.Common.Utilities;
using Amlakbashi.Core.Infrastructure.LocalizationHelpers;
using Amlakbashi.Host.Controllers.Base;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace Amlakbashi.Host.Controllers.API
{
    public partial class ApiController : BaseController
    {

        [Authorize(AuthenticationSchemes = bearerScheme)]
        public JsonResult GetReserveChats(string cid, long reserveId)
        {
            if (!ClientAuthenticate(cid))
            {
                return null;
            }
            try
            {
                var user = GetUser();
                if (user.Id < 1)
                {
                    return GenerateJsonResult(new { });
                }
                var all_chats = chatService.GetReserveChats(reserveId).ToList();
                var party_chats = all_chats.Where(x => x.UserID != user.Id).ToList();
                bool any_change = false;
                int read_status = (int)Chat.ReadStatusEnum.Read;
                foreach (var chat in party_chats)
                {
                    any_change = any_change || chat.ReadStatus != read_status;
                }
                if (any_change)
                {
                    chatService.UpdateChatListReadStatus(party_chats);
                    reserveDashboardHubServer.ReloadChatFromServer(reserveId);
                    reserveDashboardHubServer.ReloadReserveItemsFromServer(reserveId);
                }
                var party_user_id = party_chats.Any() ? party_chats.First().UserID : 0;
                var partyUser = party_user_id > 0 ? userService.Find(party_user_id) : null;

                List<ApiChatItemDTO> chatDtos = new List<ApiChatItemDTO>();
                string text;
                bool _self;
                foreach (var chat in all_chats)
                {
                    _self = user.Id == chat.UserID;
                    ChatLocalization.TextHasForbiddenCharacters(chat.Text,
                        out text, advertiseService.GetAllAsIQueriable());
                    chatDtos.Add(new ApiChatItemDTO()
                    {
                        id = chat.Id,
                        self = _self,
                        text = text,
                        sent = true,
                        read = chat.ReadStatus == (int)Chat.ReadStatusEnum.Read,
                        timeString = DateTimeUtility.GregorianToPersianDate(chat.CreateTime.Date).Replace(",", "/").Remove(0, 2) + "  " + chat.CreateTime.ToString("HH:mm"),
                        profileImageId = _self ?
                            (user.PhotoStatus == (int)Entities.User.UserPhotoState.publish && user.PhotoID != null ? (long)user.PhotoID : 0) :
                            (partyUser.PhotoStatus == (int)Entities.User.UserPhotoState.publish && partyUser.PhotoID != null ? (long)partyUser.PhotoID : 0),
                        profileName = _self ? user.FullName : partyUser.LName
                    });
                }
                return GenerateJsonResult(new { items = chatDtos });
            }
            catch (Exception exc)
            {
                logger.Error("ChatApi.GetReserveChats", exc);
                return GenerateJsonResult(new { });
            }
        }

        [Authorize(AuthenticationSchemes = bearerScheme)]
        public JsonResult SendReserveChatMessage(string cid, long reserveId, string text)
        {
            if (!ClientAuthenticate(cid))
            {
                return null;
            }
            try
            {
                var user = GetUser();
                if (user.Id < 1)
                {
                    return GenerateJsonResult(new { status = 0 });
                }
                var reserve = reserveService.Find(reserveId);
                var user_id = user.Id;
                var guest_user_id = reserve.UserID;
                var host_user_id = reserve.Advertise.UserID;
                var is_guest = user_id == guest_user_id;

                string newText;
                var forbidden = ChatLocalization.TextHasForbiddenCharacters(
                    text, out newText, advertiseService.GetAllAsIQueriable());
                var chat = new Chat() {
                    Text = text,
                    ChatStatus = forbidden ?
                        (int)Chat.ChatStatusEnum.HasForbiddenCharacters :
                        (int)Chat.ChatStatusEnum.Sent,
                    CreateTime = DateTime.Now,
                    UserID = user.Id,
                    ReserveID = reserveId,
                    ReadStatus = (int)Chat.ReadStatusEnum.NotRead
                };
                chatService.Insert(chat);
                int chatCount = chatService.GetCountByReserveId(reserveId);
                var notReadChatCount = chatService.GetNotReadSupportCountByReserveId(reserveId);
                reserveDashboardHubServer.ReloadChatFromServer(reserveId);
                reserveDashboardHubServer.ReloadReserveItemsFromServer(reserveId);
                reserveAdminHubServer.ChangeChatCountFromServer(reserveId, chatCount, notReadChatCount);

                var target_user_id = is_guest ? host_user_id : guest_user_id;
                var target_user = userService.Find(target_user_id);
                if (!string.IsNullOrEmpty(target_user.FcmAppNotificationToken) ||
                    !string.IsNullOrEmpty(target_user.AppNotificationToken) ||
                    !string.IsNullOrEmpty(target_user.NotificationToken))
                {
                    var is_first_msg = chatCount < 1;
                    chatService.ScheduleChatNotification(chat.Id, target_user_id, !is_guest,
                        is_guest ? guest_user_id : host_user_id, is_first_msg);
                }

                string checkedText;
                ChatLocalization.TextHasForbiddenCharacters(
                    chat.Text, out checkedText, advertiseService.GetAllAsIQueriable());
                ApiChatItemDTO dto = new ApiChatItemDTO()
                {
                    id = chat.Id,
                    self = user.Id == chat.UserID,
                    text = checkedText,
                    sent = false,
                    read = chat.ReadStatus == (int)Chat.ReadStatusEnum.Read,
                    timeString = DateTimeUtility.ConvertDate(chat.CreateTime.Date).Remove(0, 2) + "  " + chat.CreateTime.ToString("HH:mm"),
                    profileImageId = user.PhotoStatus == (int)Entities.User.UserPhotoState.publish && user.PhotoID != null ? (long)user.PhotoID : 0,
                    profileName = user.FullName
                };
                return GenerateJsonResult(new { status = 1, forbidden = forbidden, chat = dto });
            }
            catch (Exception exc)
            {
                logger.Error("ChatApi.SendReserveChatMessage", exc);
                return GenerateJsonResult(new { status = 0 });
            }
        }
    }
}

