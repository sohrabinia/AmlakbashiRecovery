using System;
using System.Collections.Generic;
using System.Linq;
using Amlakbashi.Core.Entities;
using Amlakbashi.Core.DTOs.CommentDTOs;
using Amlakbashi.Core.Infrastructure.LocalizationHelpers;
using static Amlakbashi.Core.Entities.Advertise;
using Amlakbashi.Core.Common.Utilities;
using Amlakbashi.Host.Controllers.Base;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace Amlakbashi.Host.Controllers.API
{
    public partial class ApiController : BaseController
    {

        [Authorize(AuthenticationSchemes = bearerScheme)]
        public JsonResult GetAdvertiseCommentsOverview(string cid)
        {
            if (!ClientAuthenticate(cid))
            {
                return null;
            }
            try
            {
                var user = GetUser();

                AdvertiseCommentOverviewDataDTO dto = new AdvertiseCommentOverviewDataDTO();

                var WithCommentAdvertises = advertiseService.GetAdvertisesByUserId(user.Id, true);
                dto.advertisesOverviews = new List<CommentOverviewDTO>();
                Advertise parent;
                string url, typeString, parentTitle, selfTitle;
                bool isComplex;
                int rateCount = WithCommentAdvertises.SelectMany(x => x.ReportItems).GroupBy(x => x.UserID).Count();
                int commentCount, newCommentCount;
                float overallScore, tidiness, hostBehaviour, position,
                    infoCorrectness, safety, priceWorth;
                IEnumerable<Comment> advertiseComments;
                IEnumerable<ReportItem> advertiseReportItems;
                foreach (var advertise in WithCommentAdvertises)
                {
                    parent = advertise.Parent;
                    url = string.Format("/{0}/{1}", "اجاره-روزانه",
                        advertise.Count > 0 ? parent.Slug : advertise.Slug);
                    typeString = AdvertiseMainLocalization.GetAdvertiseTypeUserString(
                        (AdvertiseType)(parent != null ? parent.TypeID : advertise.TypeID));
                    advertiseComments = advertise.Comments.Where(w => w.Status == Comment.CommentStatus.publish);
                    commentCount = advertiseComments.Count();
                    newCommentCount = advertiseComments.Count(x => !x.SeenByHost);
                    advertiseReportItems = advertise.ReportItems;
                    overallScore = advertiseReportItems.Average(x => (float)x.Score);
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

                return GenerateJsonResult(new
                {
                    done = true,
                    data = dto
                });
            }
            catch (Exception exc)
            {
                logger.Error("", exc);
                return GenerateJsonResult(new
                {
                    done = false,
                    msg = "متاسفانه عملیات با خطا مواجه شد"
                });
            }
        }

        [Authorize(AuthenticationSchemes = bearerScheme)]
        public JsonResult GetAdvertiseCommentsDetail(string cid, long advertiseId)
        {
            if (!ClientAuthenticate(cid))
            {
                return null;
            }
            try
            {
                var user = GetUser();

                AdvertiseCommentDetailDTO dto = new AdvertiseCommentDetailDTO();

                IQueryable<Advertise> advertises = advertiseService.GetAllAsIQueriable();
                var advertise = advertises.FirstOrDefault(x => x.Id == advertiseId);
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
                allReplyComments = allReplyComments.Where(x => x.AdvertiseID == advertiseId);
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

                return GenerateJsonResult(new
                {
                    done = true,
                    data = dto
                });
            }
            catch (Exception exc)
            {
                logger.Error("", exc);
                return GenerateJsonResult(new
                {
                    done = false,
                    msg = "متاسفانه عملیات با خطا مواجه شد"
                });
            }
        }

        [Authorize(AuthenticationSchemes = bearerScheme)]
        public JsonResult CheckUserRatingAvailable(string cid)
        {
            if (!ClientAuthenticate(cid))
            {
                return null;
            }
            try
            {
                var user = GetUser();
                long advertiseId = 0;
                bool ratingAvailable = false;
                var reserves = reserveService.GetListByUserId(user.Id, Reserve.ReserveStatus.Completed, false).AsQueryable();
                var comments = commentService.GetListBySenderUserId(user.Id);
                var reportItems = reportItemService.GetListByUserId(user.Id);
                var exception_ids = new List<long>();
                foreach (var item in reserves)
                {
                    if (comments.Any(x => x.AdvertiseID == item.AdvertiseID) ||
                        reportItems.Any(x => x.AdvertiseID == item.AdvertiseID))
                    {
                        exception_ids.Add(item.Id);
                    }
                }
                reserves = reserves.Where(x => !exception_ids.Contains(x.Id));
                var reserve = reserves.FirstOrDefault();
                if (reserve != null)
                {
                    ratingAvailable = true;
                    advertiseId = reserve.AdvertiseID;
                }
                return GenerateJsonResult(new
                {
                    done = true,
                    ratingAvailable = ratingAvailable,
                    advertiseId = advertiseId
                });
            }
            catch (Exception exc)
            {
                logger.Error("", exc);
                return GenerateJsonResult(new
                {
                    done = false
                });
            }
        }

        [Authorize(AuthenticationSchemes = bearerScheme)]
        public JsonResult SetReserveRatingAsDone(string cid)
        {
            if (!ClientAuthenticate(cid))
            {
                return null;
            }
            try
            {
                var user = GetUser();
                var reserves = reserveService.GetListByUserId(user.Id, Reserve.ReserveStatus.Completed, false).AsQueryable();
                var comments = commentService.GetListBySenderUserId(user.Id);
                var reportItems = reportItemService.GetListByUserId(user.Id);
                var exception_ids = new List<long>();
                foreach (var item in reserves)
                {
                    if (comments.Any(x => x.AdvertiseID == item.AdvertiseID) ||
                        reportItems.Any(x => x.AdvertiseID == item.AdvertiseID))
                    {
                        exception_ids.Add(item.Id);
                    }
                }
                reserves = reserves.Where(x => !exception_ids.Contains(x.Id));
                var reserve = reserves.FirstOrDefault();
                if (reserve != null)
                {
                    var reserveData = reserveService.Find(reserve.Id);
                    if (reserveData.UserID == user.Id)
                    {
                        reserveService.UpdateRatingShownToGuest(reserveData.Id, true);
                    }
                }
                return GenerateJsonResult(new
                {
                    done = true
                });
            }
            catch (Exception exc)
            {
                logger.Error("CommentApi.SetReserveRatingAsDone", exc);
                return GenerateJsonResult(new
                {
                    done = false
                });
            }
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = bearerScheme)]
        public JsonResult AddHostReplyComment(string cid, HostReplyDTO data)
        {
            try
            {
                var user = GetUser();
                var advertise = advertiseService.Find(data.advertiseID);
                if (advertise.UserID != user.Id)
                {
                    return GenerateJsonResult(new
                    {
                        done = false,
                        msg = "شما مجوز انجام این کار را ندارید"
                    });
                }
                commentService.SetAsSeenByHost(data.advertiseID);
                advertiseService.AddAdvertiseHostReplyComment(data.guestUserId, 
                    data.advertiseID, data.text);
                return GenerateJsonResult(new
                {
                    done = 1,
                    msg = ""
                });
            }
            catch (Exception exc)
            {
                logger.Error("", exc);
                return GenerateJsonResult(new
                {
                    done = false,
                    msg = "خطایی در سیستم رخ داده است.لطفا بعدا امتحان کنید"
                });
            }
        }
    }
}

