using Amlakbashi.Application.Services.AdvertiseServices.Interfaces;
using Amlakbashi.Application.Services.CommentServices.Interfaces;
using Amlakbashi.Core.DTOs.CommentDTOs;
using Amlakbashi.Core.Entities;
using Amlakbashi.Core.Infrastructure.LocalizationHelpers;
using Amlakbashi.Host.Area.App.Controllers.Base;
using Amlakbashi.Host.Authentication;
using log4net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Amlakbashi.Host.Areas.App.Controllers
{
    [Area("App")]
    [Route("app/comment/[action]")]
    public class AppCommentController : AppBaseController
    {
        private readonly IUserAccessor userAccessor;
        private readonly IAdvertiseAppService advertiseService;
        private readonly ICommentAppService commentService;
        private readonly IReportItemAppService reportItemService;
        private readonly ILog logger;
        public AppCommentController(IUserAccessor userAccessor,
            IAdvertiseAppService advertiseService,
            ICommentAppService commentService,
            IReportItemAppService reportItemService,
            ILog logger)
        {
            this.userAccessor = userAccessor;
            this.advertiseService = advertiseService;
            this.commentService = commentService;
            this.reportItemService = reportItemService;
            this.logger = logger;
        }

        [Authorize]
        public ActionResult List()
        {
            try
            {
                int userid = userAccessor.CurrentUser.Id;
                ViewBag.user_id = userid;
                AdvertiseCommentOverviewDataDTO dto = new AdvertiseCommentOverviewDataDTO();

                IQueryable<Advertise> advertises = advertiseService.GetAllAsIQueriable();
                advertises = advertises.Where(x => x.UserId == userid);
                advertises = advertises.Where(x => x.Status != Advertise.AdvertiseStatus.Deleted);
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
                        advertise.UnitCount > 0 ? parent.Slug : advertise.Slug);
                    typeString = AdvertiseMainLocalization.GetAdvertiseTypePersianNameForUser(
                        (Advertise.AdvertiseType)(parent != null ? parent.TypeID : advertise.TypeID));
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
                            parent.Childs.ElementAt(0).UnitCount == 0;
                        if (isComplex)
                        {
                            if (advertise.Floor != Advertise.FloorItems.Unset)
                            {
                                selfTitle = "طبقه: " + AdvertiseMainLocalization.GetEnumPersianName(advertise.Floor);
                            }
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(advertise.Title))
                            {
                                selfTitle = AdvertiseMainLocalization.GetHotelUnitPersianName(parent.TypeID) + " " + advertise.Title;
                            }
                        }
                    }
                    dto.advertisesOverviews.Add(new CommentOverviewDTO(
                        advertise.Id, parent != null ? parent.Id : 0, url,
                        advertise.MainPhotoId == null ? 0 : (long)advertise.MainPhotoId, advertise.Title,
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
    }
}
