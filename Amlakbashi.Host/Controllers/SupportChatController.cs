using System;
using System.Collections.Generic;
using System.Linq;
using Amlakbashi.Application.Services.SupportChatServices.Interfaces;
using Amlakbashi.Core.Entities;
using log4net;
using AutoMapper;
using Newtonsoft.Json;
using Amlakbashi.Core.DTOs.SupportChatDTOs;
using Amlakbashi.Application.Services.ReserveServices.Interfaces;
using Amlakbashi.Core.Common.Utilities;
using Amlakbashi.Core.Infrastructure.LocalizationHelpers;
using Amlakbashi.Application.Services.ReserveServices.ReserveSupportManager;
using Amlakbashi.Core.Infrastructure.HtmlHelpers;
using Amlakbashi.Application.Services.UserServices.Interfaces;
using Amlakbashi.Core.Common.StaticData;
using Amlakbashi.Application.Services.SettingServices.Interfaces;
using Amlakbashi.Host.Controllers.Base;
using Amlakbashi.Host.Authentication;
using Microsoft.AspNetCore.Mvc;
using Amlakbashi.Host.Hubs.Admin.HubServers;
using Amlakbashi.Host.Hubs.Portal.HubServers;

namespace Amlakbashi.Host.Controllers
{
    public class SupportChatController : BaseController
    {
        private readonly ISupportChatMessageAppService supportChatMessageService;
        private readonly ISupportChatAppService supportChatService;
        private readonly IReserveAppService reserveService;
        private readonly IReserveSupportManager reserveSupportManager;
        private readonly IReserveSupportAppService reserveSupportService;
        private readonly IUserAppService userService;
        private readonly ISettingAppService settingService;
        private readonly ILog logger;
        private readonly IMapper mapper;
        private readonly IUserAccessor userAccessor;
        private readonly ISupportChatAdminHubServer supportChatAdminHubServer;
        private readonly IPortalHubServer portalHubServer;
        public SupportChatController(ISupportChatMessageAppService supportChatMessageService,
            ISupportChatAppService supportChatService,
            IReserveAppService reserveService,
            IReserveSupportManager reserveSupportManager,
            IReserveSupportAppService reserveSupportService,
            IUserAppService userService,
            ISettingAppService settingService,
            ISupportChatAdminHubServer supportChatAdminHubServer,
            IPortalHubServer portalHubServer,
            IUserAccessor userAccessor,
            ILog logger,
            IMapper mapper)
        {
            this.supportChatMessageService = supportChatMessageService;
            this.supportChatService = supportChatService;
            this.reserveService = reserveService;
            this.reserveSupportManager = reserveSupportManager;
            this.reserveSupportService = reserveSupportService;
            this.userService = userService;
            this.settingService = settingService;
            this.supportChatAdminHubServer = supportChatAdminHubServer;
            this.portalHubServer = portalHubServer;
            this.userAccessor = userAccessor;
            this.logger = logger;
            this.mapper = mapper;
        }

        [Auth(UserRoles.Admin)]
        public ActionResult Index(long id = 0)
        {
            var model = supportChatService.GetLastItems(10);
            var dtoList = new List<SupportChatDTO>();
            foreach (var data in model)
            {
                var dto = new SupportChatDTO();
                Reserve relatedReserve = null;
                bool isHost = false;
                if (data.UserID != null)
                {
                    relatedReserve = reserveService.GetRelatedReserveByUser((int)data.UserID, out isHost);
                }
                var newCount = data.Messages.Count(x =>
                    x.TypeInt == (int)SupportChatMessage.TypeEnum.User &&
                    x.ReadStatusInt == (int)SupportChatMessage.ReadStatusEnum.NotRead);
                var user = data.UserID != null ? userService.Find(data.UserID) : null;
                var name = "ناشناس";
                if (user != null)
                {
                    name = user.FullName;
                    if (string.IsNullOrEmpty(name))
                    {
                        name = user.Id.ToString();
                    }
                }
                string suppName = "";
                long suppPhotoId = 0;
                int relatedSupporterId = 0;
                string relatedSupporterName = "";
                long relatedSupporterPhotoId = 0;
                if (data.SupporterID != null)
                {
                    var supporterUser = userService.Find(data.SupporterID);
                    suppName = supporterUser.FullName;
                    if (string.IsNullOrEmpty(suppName))
                    {
                        suppName = supporterUser.Id.ToString();
                    }
                    suppPhotoId = supporterUser.PhotoID == null ? 0 : (long)supporterUser.PhotoID;
                }
                if (relatedReserve != null)
                {
                    var reserveSupports = reserveSupportService.GetRelatedSupports(relatedReserve.Id);
                    reserveSupports = reserveSupports.Where(x => x.SupporterID != null).ToList();
                    Amlakbashi.Core.Entities.User relatedSupporter = null;
                    if (reserveSupports.Any())
                    {
                        var relatedSupport = reserveSupports.OrderByDescending(x => x.CreateDate).FirstOrDefault();
                        relatedSupporter = userService.Find((int)relatedSupport.SupporterID);
                    }

                    if (relatedSupporter != null)
                    {
                        relatedSupporterId = relatedSupporter.Id;
                        relatedSupporterName = relatedSupporter.FullName;
                        if (string.IsNullOrEmpty(relatedSupporterName))
                        {
                            relatedSupporterName = relatedSupporter.Id.ToString();
                        }
                        relatedSupporterPhotoId = relatedSupporter.PhotoID == null ? 0 : (long)relatedSupporter.PhotoID;
                    }
                }

                dto.id = data.Id;
                dto.userId = data.UserID == null ? 0 : (int)data.UserID;
                dto.userTitle = relatedReserve == null ? "کاربر" :
                    (isHost ? "میزبان" : "مهمان");
                dto.userPhotoId = user != null && user.PhotoID != null ? (long)user.PhotoID : 0;
                dto.userName = name;
                dto.supporterId = data.SupporterID == null ? 0 : (int)data.SupporterID;
                dto.supporterPhotoId = suppPhotoId;
                dto.supporterName = suppName;
                dto.newMessageCount = newCount;
                dto.reserveId = relatedReserve != null ?
                    relatedReserve.Id : 0;
                dto.advertiseId = relatedReserve != null ?
                    relatedReserve.AdvertiseID : 0;
                dto.reserveSupporterId = relatedSupporterId;
                dto.reserveSupporterName = relatedSupporterName;
                dto.reserveSupporterPhotoId = relatedSupporterPhotoId;
                dtoList.Add(dto);
            }
            ViewBag.id = id;
            return View(dtoList);
        }

        public JsonResult SendTextUser(int user_id, long id, string text, int questionNumber = -1)
        {
            try
            {
                SupportChat supportChat = null;
                if (id == 0)
                {
                    if (user_id > 0)
                    {
                        supportChat = supportChatService.GetByUserId(user_id);
                    }
                    if (supportChat == null)
                    {
                        supportChat = supportChatService.Insert(user_id);
                    }
                }
                else
                {
                    supportChat = supportChatService.Find(id);
                }

                var messageId = supportChatMessageService.Insert(text, SupportChatMessage.TypeEnum.User,
                    supportChat.Id, user_id);

                supportChatAdminHubServer.AddChatMessageFromServer(supportChat.Id, messageId);

                var msgCount = supportChat.Messages.Count(
                    x => x.TypeInt == (int)SupportChatMessage.TypeEnum.Supporter &&
                    x.ReadStatusInt == (int)SupportChatMessage.ReadStatusEnum.NotRead);

                if (questionNumber > -1)
                {
                    var qText = SupportChatLocalization.GetQuestionText((SupportChat.AutoQuestion)questionNumber);
                    qText += SupportChatHtmlHelper.GenerateOpenChatButton("چت آنلاین با پشتیبان");
                    var msgId = supportChatMessageService.Insert(qText, SupportChatMessage.TypeEnum.Supporter,
                        supportChat.Id, null, SupportChatMessage.ReadStatusEnum.Read);
                    supportChatAdminHubServer.AddChatMessageFromServer(supportChat.Id, msgId);
                    portalHubServer.ReloadSupportChatFromServer(supportChat.Id, msgCount, (int)supportChat.UserID);
                }
                if (GeneralData.IsSupportOnline())
                {
                    if (questionNumber < 0)
                    {
                        supportChatService.ScheduleSendSupporterNewMsgNotif(3, messageId, supportChat.Id);
                    }
                }
                else
                {
                    var autoText = "کاربر گرامی، ساعت کاری شرکت از 9 صبح تا 11/30 شب میباشد.";
                    autoText += "با کلیک روی سوال زیر پاسخ دریافت می کنید.";
                    autoText += "در غیر اینصورت شماره موبایل خود را بفرستید تا در اولین فرصت با شما تماس بگیریم";

                    var msgId = supportChatMessageService.Insert(autoText, SupportChatMessage.TypeEnum.Supporter,
                        supportChat.Id, null, SupportChatMessage.ReadStatusEnum.Read);
                    supportChatAdminHubServer.AddChatMessageFromServer(supportChat.Id, msgId);
                    portalHubServer.ReloadSupportChatFromServer(supportChat.Id, msgCount, (int)supportChat.UserID);

                    var autoText2 = SupportChatHtmlHelper.GenerateQuestionButtonList(supportChat.Id, null);

                    msgId = supportChatMessageService.Insert(autoText2, SupportChatMessage.TypeEnum.Supporter,
                        supportChat.Id, null, SupportChatMessage.ReadStatusEnum.Read);
                    supportChatAdminHubServer.AddChatMessageFromServer(supportChat.Id, msgId);
                    portalHubServer.ReloadSupportChatFromServer(supportChat.Id, msgCount, (int)supportChat.UserID);
                }
                return GenerateJsonResult(new { status = 1, id = supportChat.Id });
            }
            catch (Exception exc)
            {
                logger.Error("SendTextUser", exc);
                return GenerateJsonResult(new { status = 0 });
            }
        }

        [Auth(UserRoles.Admin)]
        public JsonResult SendTextSupporter(long id, string text)
        {
            try
            {
                var supportChat = supportChatService.Find(id);
                supportChat.SupporterID = userAccessor.CurrentUser.Id;
                if (supportChat.Messages == null)
                {
                    supportChat.Messages = new List<SupportChatMessage>();
                }
                foreach (var supportMessage in supportChat.Messages.Where(x => x.TypeInt == (int)SupportChatMessage.TypeEnum.User))
                {
                    supportMessage.ReadStatus = SupportChatMessage.ReadStatusEnum.Read;
                }

                var message = supportChatMessageService.Insert(text,
                    SupportChatMessage.TypeEnum.Supporter, id, userAccessor.CurrentUser.Id);

                supportChatAdminHubServer.AddChatMessageFromServer(supportChat.Id, message);
                portalHubServer.ReloadSupportChatFromServer(id, supportChat.Messages.Count(
                    x => x.TypeInt == (int)SupportChatMessage.TypeEnum.Supporter &&
                    x.ReadStatusInt == (int)SupportChatMessage.ReadStatusEnum.NotRead), supportChat.UserID == null ? 0 : (int)supportChat.UserID);
                return GenerateJsonResult(new { status = 1 });
            }
            catch (Exception exc)
            {
                logger.Error("SendTextSupporter", exc);
                return GenerateJsonResult(new { status = 0,
                    msg = "متاسفانه عملیات با خطای فنی مواجه شد" });
            }
        }

        public ActionResult GetChatPopup(long id = 0, string questionList = null)
        {
            try
            {
                SupportChat model = null;
                if (id == 0 && userAccessor.CurrentUser.Id > 0)
                {
                    model = supportChatService.GetByUserId(userAccessor.CurrentUser.Id);
                }
                else if (id > 0)
                {
                    model = supportChatService.Find(id);
                }
                if (model != null)
                {
                    bool any_change = false;
                    var changeList = new List<long>();
                    var currentUserId = userAccessor.CurrentUser.Id;
                    foreach (var message in model.Messages.Where(
                        x => x.ReadStatus == SupportChatMessage.ReadStatusEnum.NotRead &&
                        x.UserID != currentUserId))
                    {
                        if (message.ReadStatus != SupportChatMessage.ReadStatusEnum.Read)
                        {
                            changeList.Add(message.Id);
                            any_change = true;
                        }
                    }

                    if (any_change)
                    {
                        supportChatMessageService.UpdateReadStatusList(changeList);
                        portalHubServer.ReloadSupportChatFromServer(model.Id, 0, (int)model.UserID);
                        foreach (var item in changeList)
                        {
                            supportChatAdminHubServer.UpdateChatMessageFromServer(model.Id, item);
                        }
                    }
                }

                if (questionList != null)
                {
                    var qList = JsonConvert.DeserializeObject<int[]>(questionList);
                    foreach (var q in qList)
                    {
                        var answer = SupportChatLocalization.GetQuestionText(
                                (SupportChat.AutoQuestion)q);
                        if (model == null)
                        {
                            model = new SupportChat()
                            {
                                CreateTime = DateTime.Now,
                                LastMessageTime = DateTime.Now,
                                Messages = new List<SupportChatMessage>(),
                                SupporterID = -1,
                                UserID = userAccessor.CurrentUser.Id
                            };
                        }
                        else
                        {
                            model.Messages = model.Messages.ToList();
                        }
                        (model.Messages as List<SupportChatMessage>).Insert(0, new SupportChatMessage()
                        {
                            CreateTime = DateTime.Now,
                            ReadStatus = SupportChatMessage.ReadStatusEnum.Read,
                            SupportChatID = model.Id,
                            Text = answer,
                            UserID = -1,
                            Type = SupportChatMessage.TypeEnum.Supporter,
                            TypeInt = 1,
                            ReadStatusInt = 1
                        });
                    }
                }
                ViewBag.onlineSupport = GeneralData.IsSupportOnline();
                if (model == null)
                {
                    model = new SupportChat()
                    {
                        Messages = new List<SupportChatMessage>(),
                        UserID = userAccessor.CurrentUser.Id,
                        CreateTime = DateTime.Now,
                        LastMessageTime = DateTime.Now,
                        SupporterID = -1
                    };
                }
                SupportChatPopupDTO dto = new SupportChatPopupDTO()
                {
                    SupportChat = model,
                    UserFullName = model.UserID > 0 ? userService.Find(model.UserID).FullName : null
                };

                return PartialView("_SupportChatPopup", dto);
            }
            catch (Exception exc)
            {
                logger.Error("SupportChat.GetChatPopup", exc);
                return PartialView("_SupportChatPopup", new SupportChatPopupDTO());
            }
        }

        public ActionResult GetSupportItem(long id = 0)
        {
            var data = supportChatService.Find(id);
            SupportChatDTO dto = new SupportChatDTO();
            Reserve relatedReserve = null;
            bool isHost = false;
            if (data.UserID != null)
            {
                relatedReserve = reserveService.GetRelatedReserveByUser((int)data.UserID, out isHost);
            }
            var newCount = data.Messages.Count(x =>
                x.TypeInt == (int)SupportChatMessage.TypeEnum.User &&
                x.ReadStatusInt == (int)SupportChatMessage.ReadStatusEnum.NotRead);
            var user = data.UserID != null ? userService.Find(data.UserID) : null;
            var name = "ناشناس";
            if (user != null)
            {
                name = user.FullName;
                if (string.IsNullOrEmpty(name))
                {
                    name = user.Id.ToString();
                }
            }
            string suppName = "";
            long suppPhotoId = 0;
            int relatedSupporterId = 0;
            string relatedSupporterName = "";
            long relatedSupporterPhotoId = 0;
            if (data.SupporterID != null)
            {
                var supporterUser = userService.Find(data.SupporterID);
                suppName = supporterUser.FullName;
                if (string.IsNullOrEmpty(suppName))
                {
                    suppName = supporterUser.Id.ToString();
                }
                suppPhotoId = supporterUser.PhotoID == null ? 0 : (long)supporterUser.PhotoID;
            }
            if (relatedReserve != null)
            {
                var reserveSupports = reserveSupportService.GetRelatedSupports(relatedReserve.Id);
                reserveSupports = reserveSupports.Where(x => x.SupporterID != null).ToList();
                Amlakbashi.Core.Entities.User relatedSupporter = null;
                if (reserveSupports.Any())
                {
                    var relatedSupport = reserveSupports.OrderByDescending(x => x.CreateDate).FirstOrDefault();
                    relatedSupporter = userService.Find((int)relatedSupport.SupporterID);
                }

                if (relatedSupporter != null)
                {
                    relatedSupporterId = relatedSupporter.Id;
                    relatedSupporterName = relatedSupporter.FullName;
                    if (string.IsNullOrEmpty(relatedSupporterName))
                    {
                        relatedSupporterName = relatedSupporter.Id.ToString();
                    }
                    relatedSupporterPhotoId = relatedSupporter.PhotoID == null ? 0 : (long)relatedSupporter.PhotoID;
                }
            }

            dto.id = data.Id;
            dto.userId = data.UserID == null ? 0 : (int)data.UserID;
            dto.userTitle = relatedReserve == null ? "کاربر" :
                (isHost ? "میزبان" : "مهمان");
            dto.userPhotoId = user != null && user.PhotoID != null ? (long)user.PhotoID : 0;
            dto.userName = name;
            dto.supporterId = data.SupporterID == null ? 0 : (int)data.SupporterID;
            dto.supporterPhotoId = suppPhotoId;
            dto.supporterName = suppName;
            dto.newMessageCount = newCount;
            dto.reserveId = relatedReserve != null ?
                relatedReserve.Id : 0;
            dto.advertiseId = relatedReserve != null ?
                relatedReserve.AdvertiseID : 0;
            dto.reserveSupporterId = relatedSupporterId;
            dto.reserveSupporterName = relatedSupporterName;
            dto.reserveSupporterPhotoId = relatedSupporterPhotoId;
            return PartialView("_SupportItem", dto);
        }

        public ActionResult GetSupportItemList(int currentItemCount)
        {
            var model = supportChatService.GetLastItems(10, currentItemCount);
            var dtoList = new List<SupportChatDTO>();
            foreach (var data in model)
            {
                var dto = new SupportChatDTO();
                Reserve relatedReserve = null;
                bool isHost = false;
                if (data.UserID != null)
                {
                    relatedReserve = reserveService.GetRelatedReserveByUser((int)data.UserID, out isHost);
                }
                var newCount = data.Messages.Count(x =>
                    x.TypeInt == (int)SupportChatMessage.TypeEnum.User &&
                    x.ReadStatusInt == (int)SupportChatMessage.ReadStatusEnum.NotRead);
                var user = data.UserID != null ? userService.Find(data.UserID) : null;
                var name = "ناشناس";
                if (user != null)
                {
                    name = user.FullName;
                    if (string.IsNullOrEmpty(name))
                    {
                        name = user.Id.ToString();
                    }
                }
                string suppName = "";
                long suppPhotoId = 0;
                int relatedSupporterId = 0;
                string relatedSupporterName = "";
                long relatedSupporterPhotoId = 0;
                if (data.SupporterID != null)
                {
                    var supporterUser = userService.Find(data.SupporterID);
                    suppName = supporterUser.FullName;
                    if (string.IsNullOrEmpty(suppName))
                    {
                        suppName = supporterUser.Id.ToString();
                    }
                    suppPhotoId = supporterUser.PhotoID == null ? 0 : (long)supporterUser.PhotoID;
                }
                if (relatedReserve != null)
                {
                    var reserveSupports = reserveSupportService.GetRelatedSupports(relatedReserve.Id);
                    reserveSupports = reserveSupports.Where(x => x.SupporterID != null).ToList();
                    Amlakbashi.Core.Entities.User relatedSupporter = null;
                    if (reserveSupports.Any())
                    {
                        var relatedSupport = reserveSupports.OrderByDescending(x => x.CreateDate).FirstOrDefault();
                        relatedSupporter = userService.Find((int)relatedSupport.SupporterID);
                    }

                    if (relatedSupporter != null)
                    {
                        relatedSupporterId = relatedSupporter.Id;
                        relatedSupporterName = relatedSupporter.FullName;
                        if (string.IsNullOrEmpty(relatedSupporterName))
                        {
                            relatedSupporterName = relatedSupporter.Id.ToString();
                        }
                        relatedSupporterPhotoId = relatedSupporter.PhotoID == null ? 0 : (long)relatedSupporter.PhotoID;
                    }
                }

                dto.id = data.Id;
                dto.userId = data.UserID == null ? 0 : (int)data.UserID;
                dto.userTitle = relatedReserve == null ? "کاربر" :
                    (isHost ? "میزبان" : "مهمان");
                dto.userPhotoId = user != null && user.PhotoID != null ? (long)user.PhotoID : 0;
                dto.userName = name;
                dto.supporterId = data.SupporterID == null ? 0 : (int)data.SupporterID;
                dto.supporterPhotoId = suppPhotoId;
                dto.supporterName = suppName;
                dto.newMessageCount = newCount;
                dto.reserveId = relatedReserve != null ?
                    relatedReserve.Id : 0;
                dto.advertiseId = relatedReserve != null ?
                    relatedReserve.AdvertiseID : 0;
                dto.reserveSupporterId = relatedSupporterId;
                dto.reserveSupporterName = relatedSupporterName;
                dto.reserveSupporterPhotoId = relatedSupporterPhotoId;
                dtoList.Add(dto);
            }
            return PartialView("_SupportItemList", dtoList);
        }

        public ActionResult GetChatItem(long id, long supportChatId = 0)
        {
            var data = supportChatMessageService.Find(id);
            var supportChat = supportChatService.Find(supportChatId);
            if (userAccessor.CurrentUser.Id > 0 && userAccessor.CurrentUser.Id == supportChat.SupporterID)
            {
                if (data.Type == SupportChatMessage.TypeEnum.User)
                {
                    supportChatMessageService.UpdateReadStatus(data.Id);
                    var newCount = supportChat.Messages.Count(
                        x => x.TypeInt == (int)SupportChatMessage.TypeEnum.Supporter &&
                        x.ReadStatus == SupportChatMessage.ReadStatusEnum.NotRead);
                    portalHubServer.ReloadSupportChatFromServer(supportChatId, newCount, supportChat.UserID == null ? 0 : (int)supportChat.UserID);
                    supportChatAdminHubServer.UpdateChatMessageFromServer(supportChatId, id);
                }
            }
            SupportChatMessageDTO dto = new SupportChatMessageDTO();
            var user = data.UserID != null ? userService.Find(data.UserID) : null;
            var name = "ناشناس";
            if (user != null)
            {
                name = user.FullName;
                if (string.IsNullOrEmpty(name))
                {
                    name = user.Id.ToString();
                }
            }

            dto.id = data.Id;
            dto.userId = data.UserID == null ? 0 : (int)data.UserID;
            dto.userPhotoId = user != null && user.PhotoID != null ? (long)user.PhotoID : 0;
            dto.userName = name;
            dto.text = data.Text;
            dto.sent = data.Id > 0;
            dto.self = data.Type == SupportChatMessage.TypeEnum.Supporter;
            dto.read = data.ReadStatus == SupportChatMessage.ReadStatusEnum.Read;
            dto.dateString = data.Id < 1 ? "" :
                DateTimeUtility.GregorianToPersianDate(data.CreateTime).Replace(",", "/") + " " +
                data.CreateTime.ToString("HH:mm");
            return PartialView("_ChatItem", dto);
        }

        public ActionResult GenerateUnsentChatItem(long supportChatID, string text)
        {
            var data = new SupportChatMessage()
            {
                CreateTime = DateTime.Now,
                Id = 0,
                ReadStatus = SupportChatMessage.ReadStatusEnum.NotRead,
                Type = SupportChatMessage.TypeEnum.Supporter,
                UserID = userAccessor.CurrentUser.Id,
                Text = text,
                SupportChatID = supportChatID
            };
            SupportChatMessageDTO dto = new SupportChatMessageDTO();
            var user = data.UserID > 0 ? userService.Find(data.UserID) : null;
            var name = "ناشناس";
            if (user != null)
            {
                name = user.FullName;
                if (string.IsNullOrEmpty(name))
                {
                    name = user.Id.ToString();
                }
            }

            dto.id = data.Id;
            dto.userId = data.UserID == null ? 0 : (int)data.UserID;
            dto.userPhotoId = user != null && user.PhotoID != null ? (long)user.PhotoID : 0;
            dto.userName = name;
            dto.text = data.Text;
            dto.sent = data.Id > 0;
            dto.self = data.Type == SupportChatMessage.TypeEnum.Supporter;
            dto.read = data.ReadStatus == SupportChatMessage.ReadStatusEnum.Read;
            dto.dateString = data.Id < 1 ? "" :
                DateTimeUtility.GregorianToPersianDate(data.CreateTime).Replace(",", "/") + " " +
                data.CreateTime.ToString("HH:mm");
            return PartialView("_ChatItem", dto);
        }

        public ActionResult GetChatItemList(long id = 0)
        {
            var model = supportChatService.Find(id);
            if (userAccessor.CurrentUser.Id > 0 && userAccessor.CurrentUser.Id == model.SupporterID)
            {
                List<long> listId = new List<long>();
                foreach (var supportMessage in model.Messages.Where(x => x.TypeInt == (int)SupportChatMessage.TypeEnum.User))
                {
                    listId.Add(supportMessage.Id);
                }
                supportChatMessageService.UpdateReadStatusList(listId);
                var newCount = model.Messages.Count(
                    x => x.TypeInt == (int)SupportChatMessage.TypeEnum.Supporter &&
                    x.ReadStatusInt == (int)SupportChatMessage.ReadStatusEnum.NotRead);
                portalHubServer.ReloadSupportChatFromServer(id, newCount, model.UserID == null ? 0 : (int)model.UserID);
                supportChatAdminHubServer.UpdateChatMessageFromServer(id, 0);
            }
            List<SupportChatMessageDTO> dtoList = new List<SupportChatMessageDTO>();
            foreach (var data in model.Messages.OrderByDescending(o => o.CreateTime))
            {
                var dto = new SupportChatMessageDTO();
                var user = data.UserID > 0 ? userService.Find(data.UserID) : null;
                var name = "ناشناس";
                if (user != null)
                {
                    name = user.FullName;
                    if (string.IsNullOrEmpty(name))
                    {
                        name = user.Id.ToString();
                    }
                }

                dto.id = data.Id;
                dto.userId = data.UserID == null ? 0 : (int)data.UserID;
                dto.userPhotoId = user != null && user.PhotoID != null ? (long)user.PhotoID : 0;
                dto.userName = name;
                dto.text = data.Text;
                dto.sent = data.Id > 0;
                dto.self = data.Type == SupportChatMessage.TypeEnum.Supporter;
                dto.read = data.ReadStatus == SupportChatMessage.ReadStatusEnum.Read;
                dto.dateString = data.Id < 1 ? "" :
                    DateTimeUtility.GregorianToPersianDate(data.CreateTime).Replace(",", "/") + " " +
                    data.CreateTime.ToString("HH:mm");
                dtoList.Add(dto);
            }
            return PartialView("_ChatItemList", dtoList);
        }

        public ActionResult GetSupportChatUser()
        {
            ViewBag.onlineSupport = GeneralData.IsSupportOnline();
            return PartialView("_SupportChatUser");
        }

        //[Auth]
        //public JsonResult GetSupporterName(long id)
        //{
        //    var supportChat = _db.SupportChats.Find(id);
        //    return new JsonResult()
        //    {
        //        JsonRequestBehavior = JsonRequestBehavior.AllowGet,
        //        Data = new { name = UserDepend.GetUserInfo(supportChat.SupporterID).LName }
        //    };
        //}
    }
}