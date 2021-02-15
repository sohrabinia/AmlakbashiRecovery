using Amlakbashi.Application.Services.AdvertiseServices.Interfaces;
using Amlakbashi.Application.Services.CommentServices.Interfaces;
using Amlakbashi.Application.Services.UserServices.Interfaces;
using Amlakbashi.Core.DTOs.CommentDTOs;
using Amlakbashi.Core.Entities;
using Entities = Amlakbashi.Core.Entities;
using Amlakbashi.Host.Authentication;
using Amlakbashi.Host.Controllers.Base;
using AutoMapper;
using log4net;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using X.PagedList;
using static Amlakbashi.Core.Entities.Advertise;
using Amlakbashi.Core.Infrastructure.LocalizationHelpers;
using Amlakbashi.Core.Common.Utilities;

namespace Amlakbashi.Host.Controllers
{
    public class CommentController : BaseController
    {
        private readonly IReportItemAppService reportItemService;
        private readonly ICommentAppService commentService;
        private readonly IUserAppService userService;
        private readonly IAdvertiseAppService advertiseService;
        private readonly IUserAccessor userAccessor;
        private readonly ILog logger;
        private readonly IMapper mapper;
        public CommentController(ICommentAppService commentService,
            IReportItemAppService reportItemService,
            IAdvertiseAppService advertiseService,
            IUserAppService userService,
            IUserAccessor userAccessor,
            ILog logger, IMapper mapper)
        {
            this.reportItemService = reportItemService;
            this.commentService = commentService;
            this.userService = userService;
            this.advertiseService = advertiseService;
            this.logger = logger;
            this.mapper = mapper;
            this.userAccessor = userAccessor;
        }

        [Auth(UserRoles.Admin)]
        public ActionResult Index(int? page, int status = (int)Comment.CommentStatus.ready,
            int comment_type = -1, int comment_id = -1, int sender_user_id = -1, long advertise_id = -1)
        {
            try
            {
                var model = commentService.Filter(status, comment_type,
                    comment_id, sender_user_id, advertise_id);

                ViewBag.comment_type = comment_type;
                ViewBag.status = status;
                ViewBag.comment_id = comment_id;
                ViewBag.advertise_id = advertise_id;
                ViewBag.sender_user_id = sender_user_id;
                var PageNumber = page ?? 1;
                var onePageOfModel = model.ToPagedList(PageNumber, 20);
                ViewBag.RowIndexStart = (PageNumber * 20) - 20;

                List<CommentIndexDTO> commentDTOs = new List<CommentIndexDTO>();
                foreach (var item in onePageOfModel)
                {
                    var dto = new CommentIndexDTO()
                    {
                        Comment = item,
                        UserPhoneNumber = userService.Find(item.SenderUserID).GetPhoneNumber(Entities.User.PhoneType.MainMobile)
                    };
                    commentDTOs.Add(dto);
                }
                ViewBag.dto = commentDTOs;
                return View(onePageOfModel);
            }
            catch (Exception exc)
            {
                logger.Error("Index", exc);
                return Redirect(Request.Headers["Referer"].ToString());
            }
        }

        [Auth(UserRoles.Admin)]
        [HttpGet]
        public ActionResult Edit(long id = -1)
        {
            try
            {
                return View(commentService.Find(id));
            }
            catch (Exception exc)
            {
                logger.Error("Edit(get)", exc);
                return Redirect(Request.Headers["Referer"].ToString());
            }
        }

        [Auth(UserRoles.Admin)]
        [HttpPost]
        public ActionResult Edit(Comment cm)
        {
            try
            {
                commentService.Update(cm);
                return RedirectToAction("Index");
            }
            catch (Exception exc)
            {
                logger.Error("Edit(post)", exc);
                return Redirect(Request.Headers["Referer"].ToString());
            }
        }

        [Auth(UserRoles.Admin)]
        public JsonResult Delete(long id)
        {
            try
            {
                commentService.Delete(id);
                return GenerateJsonResult(new
                {
                    status = 1,
                    val = ""
                });
            }
            catch (Exception exc)
            {
                logger.Error("Comment.Delete", exc);
                return GenerateJsonResult(new
                {
                    status = 0,
                    val = ""
                });
            }
        }

        [Auth(UserRoles.Admin)]
        public JsonResult Suspend(long id, string reason)
        {
            try
            {
                commentService.UpdateStatus(id, Comment.CommentStatus.suspend, reason);
                return GenerateJsonResult(new
                {
                    status = 1,
                    val = ""
                });
            }
            catch (Exception exc)
            {
                logger.Error("Suspend", exc);
                return GenerateJsonResult(new
                {
                    status = 0,
                    val = ""
                });
            }
        }

        [Auth(UserRoles.Admin)]
        public JsonResult Publish(long id)
        {
            try
            {
                commentService.UpdateStatus(id, Comment.CommentStatus.publish);
                return GenerateJsonResult(new
                {
                    status = 1,
                    val = ""
                });
            }
            catch (Exception exc)
            {
                logger.Error("Publish", exc);
                return GenerateJsonResult(new
                {
                    status = 0,
                    val = ""
                });
            }
        }

        [Auth]
        public ActionResult AdvertiseCommentManager()
        {
            try
            {
                int userid = userAccessor.CurrentUser.Id;
                ViewBag.user_id = userid;
                AdvertiseCommentOverviewDataDTO dto = new AdvertiseCommentOverviewDataDTO();

                IQueryable<Advertise> advertises = advertiseService.GetAllAsIQueriable();
                advertises = advertises.Where(x => x.UserID == userid);
                advertises = advertises.Where(x => x.Status != AdvertiseStatus.Deleted);
                var advertise_ids = advertises.Select(x => x.Id).ToList();
                IQueryable<Comment> comments = commentService.GetAllAsIQueryable();
                comments = comments.Where(x => x.Status == Comment.CommentStatus.publish);
                comments = comments.Where(x => x.type == (int)Comment.CommentType.advertise);
                comments = comments.Where(x => advertise_ids.Contains(x.AdvertiseID));
                IQueryable<ReportItem> reportItems = reportItemService.GetAllAsIQueriable();
                reportItems = reportItems.Where(x => advertise_ids.Contains(x.AdvertiseID));
                var ids = new List<long>();
                foreach (var item in advertises)
                {
                    if (comments.Any(x => x.AdvertiseID == item.Id))
                    {
                        ids.Add(item.Id);
                    }
                }
                IQueryable<Advertise> WithCommentAdvertises =
                    advertises.Where(x => ids.Contains(x.Id));
                dto.advertisesOverviews = new List<CommentOverviewDTO>();
                Advertise parent;
                string url, typeString, parentTitle, selfTitle;
                bool isComplex;
                int commentCount, newCommentCount, rateCount;
                float overallScore, tidiness, hostBehaviour, position,
                    infoCorrectness, safety, priceWorth;
                IQueryable<Comment> advertiseComments;
                IQueryable<ReportItem> advertiseReportItems;
                foreach (var advertise in WithCommentAdvertises)
                {
                    parent = advertise.Parent;
                    url = string.Format("/{0}/{1}", "اجاره-روزانه",
                        advertise.Count > 0 ? parent.Slug : advertise.Slug);
                    typeString = AdvertiseMainLocalization.GetAdvertiseTypeUserString(
                        (AdvertiseType)(parent != null ? parent.TypeID : advertise.TypeID));
                    advertiseComments = comments.Where(x => x.AdvertiseID == advertise.Id);
                    commentCount = advertiseComments.Count();
                    newCommentCount = advertiseComments.Count(x => !x.SeenByHost);
                    advertiseReportItems = reportItems.Where(
                        x => x.AdvertiseID == advertise.Id);
                    rateCount = reportItemService.GetCountByAccId(advertise.Id, advertiseReportItems);
                    overallScore = advertiseReportItems.Count() > 0 ? advertiseReportItems.Average(x => (float)x.Score) : 0;
                    tidiness = advertiseReportItems.Any(x => x.ReportID == 1) ?
                        advertiseReportItems.Where(x => x.ReportID == 1).
                        Average(x => (float)x.Score) : 0;
                    hostBehaviour = advertiseReportItems.Any(x => x.ReportID == 2) ?
                        advertiseReportItems.Where(x => x.ReportID == 2).
                        Average(x => (float)x.Score) : 0;
                    position = advertiseReportItems.Any(x => x.ReportID == 3) ?
                        advertiseReportItems.Where(x => x.ReportID == 3).
                        Average(x => (float)x.Score) : 0;
                    infoCorrectness = advertiseReportItems.Any(x => x.ReportID == 4) ?
                        advertiseReportItems.Where(x => x.ReportID == 4).
                        Average(x => (float)x.Score) : 0;
                    safety = advertiseReportItems.Any(x => x.ReportID == 5) ?
                        advertiseReportItems.Where(x => x.ReportID == 5).
                        Average(x => (float)x.Score) : 0;
                    priceWorth = advertiseReportItems.Any(x => x.ReportID == 6) ?
                        advertiseReportItems.Where(x => x.ReportID == 6).
                        Average(x => (float)x.Score) : 0;
                    parentTitle = null;
                    selfTitle = null;
                    if (parent != null)
                    {
                        parentTitle = parent.Title;
                        isComplex = parent.Childs.Count > 0 &&
                            parent.Childs.ElementAt(0).Count == 0;
                        if (isComplex)
                        {
                            if ((FloorItems)advertise.Floor != FloorItems.Unset)
                            {
                                selfTitle = "طبقه: " + AdvertiseMainLocalization.GetPropertyValueTitle((FloorItems)advertise.Floor);
                            }
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(advertise.Title))
                            {
                                selfTitle = AdvertiseMainLocalization.GetHotelUnitTitle(parent.TypeID) + " " + advertise.Title;
                            }
                        }
                    }
                    dto.advertisesOverviews.Add(new CommentOverviewDTO(
                        advertise.Id, parent != null ? parent.Id : 0, url,
                        advertise.PhotoID == null ? 0 : (long)advertise.PhotoID, advertise.Title,
                        typeString, parentTitle, selfTitle,
                        commentCount, newCommentCount, overallScore, rateCount,
                        new ScoreDetailDTO(tidiness, hostBehaviour, position,
                            infoCorrectness, safety, priceWorth)));
                }
                return View(dto);
            }
            catch (Exception exc)
            {
                logger.Error("AdvertiseCommentManager", exc);
                return Redirect(Request.Headers["Referer"].ToString());
            }
        }

        [Auth]
        public ActionResult AdvertiseCommentDetail(int id)
        {
            var adv = advertiseService.Find(id);
            if (adv.UserID != userAccessor.CurrentUser.Id)
            {
                return RedirectToAction("AccessDenied", "Errors");
            }
            if (adv.UserID == userAccessor.CurrentUser.Id)
            {
                commentService.SetAsSeenByHost(id);
            }
            ViewBag.user_id = adv.UserID;
            ViewBag.UrlReferrer = string.IsNullOrEmpty(Request.Headers["Referer"]) ? "/" : Request.Headers["Referer"].ToString();

            AdvertiseCommentDetailDTO dto = new AdvertiseCommentDetailDTO();
            IQueryable<Advertise> advertises = advertiseService.GetAllAsIQueriable();
            var advertise = advertises.FirstOrDefault(x => x.Id == id);
            advertises = advertises.Where(x => x.UserID == advertise.UserID);
            advertises = advertises.Where(x => x.Status != AdvertiseStatus.Deleted);
            var advertise_ids = advertises.Select(x => x.Id).ToList();
            IQueryable<Comment> comments = commentService.GetAllAsIQueryable();
            comments = comments.Where(x => x.Status == Comment.CommentStatus.publish);
            comments = comments.Where(x => x.type == Comment.CommentType.advertise);
            comments = comments.Where(x => advertise_ids.Contains(x.AdvertiseID));
            IQueryable<ReportItem> reportItems = reportItemService.GetAllAsIQueriable();
            reportItems = reportItems.Where(x => advertise_ids.Contains(x.AdvertiseID));
            var ids = new List<long>();
            foreach (var item in advertises)
            {
                if (comments.Any(x => x.AdvertiseID == item.Id))
                {
                    ids.Add(item.Id);
                }
            }
            IQueryable<Advertise> WithCommentAdvertises =
                advertises.Where(x => ids.Contains(x.Id));
            Advertise parent;
            string url, typeString, parentTitle, selfTitle;
            bool isComplex;
            int commentCount, newCommentCount, rateCount;
            float overallScore, tidiness, hostBehaviour, position,
                infoCorrectness, safety, priceWorth;
            IQueryable<Comment> advertiseComments;
            IQueryable<ReportItem> advertiseReportItems;
            parent = advertise.Parent;
            url = string.Format("/{0}/{1}", "اجاره-روزانه",
                advertise.Count > 0 ? parent.Slug : advertise.Slug);
            typeString = AdvertiseMainLocalization.GetAdvertiseTypeUserString(
                (AdvertiseType)(parent != null ? parent.TypeID : advertise.TypeID));
            advertiseComments = comments.Where(x => x.AdvertiseID == advertise.Id);
            commentCount = advertiseComments.Count();
            newCommentCount = advertiseComments.Count(x => !x.SeenByHost);
            advertiseReportItems = reportItems.Where(
                x => x.AdvertiseID == advertise.Id);
            rateCount = reportItemService.GetCountByAccId(advertise.Id, advertiseReportItems);
            overallScore = !advertiseReportItems.Any() ? 0 : advertiseReportItems.Average(x => (float)x.Score);
            tidiness = advertiseReportItems.Any(x => x.ReportID == 1) ?
                advertiseReportItems.Where(x => x.ReportID == 1).
                Average(x => (float)x.Score) : 0;
            hostBehaviour = advertiseReportItems.Any(x => x.ReportID == 2) ?
                advertiseReportItems.Where(x => x.ReportID == 2).
                Average(x => (float)x.Score) : 0;
            position = advertiseReportItems.Any(x => x.ReportID == 3) ?
                advertiseReportItems.Where(x => x.ReportID == 3).
                Average(x => (float)x.Score) : 0;
            infoCorrectness = advertiseReportItems.Any(x => x.ReportID == 4) ?
                advertiseReportItems.Where(x => x.ReportID == 4).
                Average(x => (float)x.Score) : 0;
            safety = advertiseReportItems.Any(x => x.ReportID == 5) ?
                advertiseReportItems.Where(x => x.ReportID == 5).
                Average(x => (float)x.Score) : 0;
            priceWorth = advertiseReportItems.Any(x => x.ReportID == 6) ?
                advertiseReportItems.Where(x => x.ReportID == 6).
                Average(x => (float)x.Score) : 0;
            parentTitle = null;
            selfTitle = null;
            if (parent != null)
            {
                parentTitle = parent.Title;
                isComplex = parent.Childs.Count > 0 &&
                    parent.Childs.ElementAt(0).Count == 0;
                if (isComplex)
                {
                    if ((FloorItems)advertise.Floor != FloorItems.Unset)
                    {
                        selfTitle = "طبقه: " + AdvertiseMainLocalization.GetPropertyValueTitle((FloorItems)advertise.Floor);
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(advertise.Title))
                    {
                        selfTitle = AdvertiseMainLocalization.GetHotelUnitTitle(parent.TypeID) + " " + advertise.Title;
                    }
                }
            }
            dto.Overview = new CommentOverviewDTO(
                advertise.Id, parent != null ? parent.Id : 0, url,
                advertise.PhotoID == null ? 0 : (long)advertise.PhotoID,
                advertise.Title, typeString, parentTitle, selfTitle,
                commentCount, newCommentCount, overallScore, rateCount,
                new ScoreDetailDTO(tidiness, hostBehaviour, position,
                    infoCorrectness, safety, priceWorth));
            var commentItems = new List<CommentItemDTO>();
            Comment relatedComment;
            IQueryable<ReportItem> relatedReportItems;
            var guestUserIds = advertiseReportItems.Select(x => x.UserID).ToList();
            guestUserIds.AddRange(advertiseComments.Select(x => x.SenderUserID).ToList());
            guestUserIds = guestUserIds.Distinct().ToList();
            IQueryable<User> guestUsers = userService.GetAllAsIQueryable();
            guestUsers = guestUsers.Where(x => guestUserIds.Contains(x.Id));
            string _name, _dateString, _text;
            DateTime _date;
            float _overallScore, _tidiness, _hostBehaviour, _position,
                _infoCorrectness, _safety, _priceWorth;
            ReportItem tempReportItem;
            CommentItemDTO _reply;
            Comment replyComment;
            IQueryable<Comment> allReplyComments = commentService.GetAllAsIQueryable();
            allReplyComments = allReplyComments.Where(x => x.AdvertiseID == id);
            allReplyComments = allReplyComments.Where(x => x.type == Comment.CommentType.advertiseHostReply);
            var hostUser = userService.Find(advertise.UserID);
            foreach (var guestUser in guestUsers)
            {
                relatedComment = advertiseComments.FirstOrDefault(x => x.SenderUserID == guestUser.Id);
                replyComment = relatedComment == null ? null : commentService.GetHostReply(relatedComment.AdvertiseID, relatedComment.SenderUserID);
                relatedReportItems = advertiseReportItems.Where(x => x.UserID == guestUser.Id);
                _name = guestUser.FullName;
                _date = relatedComment != null ? relatedComment.LastModifyDate :
                    relatedReportItems.Max(x => x.LastModifyDate);
                _dateString = DateTimeUtility.GregorianToPersianDate(_date);
                _dateString = _dateString.Replace(',', '/');
                _dateString = _dateString.Substring(2);
                _dateString = StringUtility.EnglishNumberToPersian(_dateString);
                _overallScore = relatedReportItems.Count() > 0 ?
                    relatedReportItems.Average(x => (float)x.Score) : 0;
                tempReportItem = relatedReportItems.FirstOrDefault(x =>
                    x.ReportID == 1);
                _tidiness = tempReportItem == null ? 0 : tempReportItem.Score;
                tempReportItem = relatedReportItems.FirstOrDefault(x =>
                    x.ReportID == 2);
                _hostBehaviour = tempReportItem == null ? 0 : tempReportItem.Score;
                tempReportItem = relatedReportItems.FirstOrDefault(x =>
                    x.ReportID == 3);
                _position = tempReportItem == null ? 0 : tempReportItem.Score;
                tempReportItem = relatedReportItems.FirstOrDefault(x =>
                    x.ReportID == 4);
                _infoCorrectness = tempReportItem == null ? 0 : tempReportItem.Score;
                tempReportItem = relatedReportItems.FirstOrDefault(x =>
                    x.ReportID == 5);
                _safety = tempReportItem == null ? 0 : tempReportItem.Score;
                tempReportItem = relatedReportItems.FirstOrDefault(x =>
                    x.ReportID == 6);
                _priceWorth = tempReportItem == null ? 0 : tempReportItem.Score;
                _text = relatedComment != null ? relatedComment.Text : "";
                _reply = null;
                if (replyComment != null)
                {
                    var replyDate = replyComment.LastModifyDate;
                    var replyDateString = DateTimeUtility.GregorianToPersianDate(replyDate);
                    replyDateString = replyDateString.Replace(',', '/');
                    replyDateString = replyDateString.Substring(2);
                    replyDateString = StringUtility.EnglishNumberToPersian(replyDateString);
                    _reply = new CommentItemDTO(replyComment.Id, replyComment.SenderUserID, hostUser.FullName,
                        hostUser.PhotoID == null ? 0 : (long)hostUser.PhotoID, replyDateString, 0, replyComment.Text,
                        null);
                }
                commentItems.Add(new CommentItemDTO(relatedComment != null ? relatedComment.Id :
                    1000000 + relatedReportItems.OrderByDescending(x => x.Id).First().Id,
                    relatedComment != null ? relatedComment.SenderUserID : relatedReportItems.First().UserID,
                    _name, guestUser.PhotoID == null ? 0 : (long)guestUser.PhotoID,
                    _dateString, _overallScore, _text, new ScoreDetailDTO(
                        _tidiness, _hostBehaviour, _position, _infoCorrectness,
                        _safety, _priceWorth), _reply));
            }
            dto.Detail = new CommentDetailDTO(commentItems);
            return View(dto);
        }
    }
}
