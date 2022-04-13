using Amlakbashi.Core.Common.AppService;
using Amlakbashi.Core.Common.Repository;
using Amlakbashi.Application.Services.CommentServices.Interfaces;
using Amlakbashi.Core.Entities;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System;
using MediatR;
using Amlakbashi.Core.Common.Extensions;
using Amlakbashi.Mediator.Commands.AdvertiseCommands;
using Amlakbashi.Core.DTOs.WebService.Requests.Comments;

namespace Amlakbashi.Application.Services.CommentServices
{
    internal class ReportItemAppService : AppServiceBase<ReportItem, long>, IReportItemAppService
    {
        private readonly IMediator mediator;
        public ReportItemAppService(IRepository<ReportItem, long> repository,
            IMediator mediator) : base(repository)
        {
            this.mediator = mediator;
        }

        //TODO: Remove
        public IQueryable<ReportItem> GetAllAsIQueriable()
        {
            return Repository.Query(q => q);
        }

        public IList<ReportItem> GetAccUserRatings(long accId, int userId)
        {
            return Repository.Query(q => q.Where(w => w.UserID == userId)
                .Where(w => w.AdvertiseID == accId)).ToList();
        }

        public IList<ReportItem> GetByAccList(IList<long> accIds)
        {
            return Repository.Query(q => q.Where(w => accIds.Contains(w.Id)).ToList());
        }

        public IList<ReportItem> GetListByUserId(int userId)
        {
            return Repository.Query(q => q.Where(w => w.UserID == userId).ToList());
        }

        public int GetCountByAccId(long accId, IQueryable<ReportItem> reportItems = null)
        {
            if (reportItems == null)
            {
                reportItems = Repository.Query(q => q.Where(x => x.AdvertiseID == accId));
            }
            return reportItems.GroupBy(g => g.UserID).Count();
        }

        public int GetAdvertiseRatingOfUser(int userId, long advertiseId, int reportId)
        {
            var report_item = Repository.Query(q => q.FirstOrDefault(x =>
                  x.UserID == userId &&
                  x.AdvertiseID == advertiseId &&
                  x.ReportID == reportId));
            return report_item != null ? report_item.Score : 0;
        }


        public void Submit(int userId, long advertiseId, IList<CommentPostScoresRequest> scores)
        {
            var advertise = Repository.Find<Advertise, long>(advertiseId);
            foreach (var item in scores)
            {
                var report = advertise.ReportItems.FirstOrDefault(s => s.UserID == userId && s.ReportID == (int)item.type);
                if (report == null)
                {
                    report = new ReportItem()
                    {
                        AdvertiseID = advertiseId,
                        CreateDate = DateTime.Now,
                        LastModifyDate = DateTime.Now,
                        LastModifyDatetick = DateTime.Now.Ticks,
                        ReportID = (int)item.type,
                        Score = item.score,
                        UserID = userId
                    };
                    Repository.Insert(report);
                }
                else
                {
                    report.Score = item.score;
                    report.LastModifyDate = DateTime.Now;
                    report.LastModifyDatetick = DateTime.Now.Ticks;
                    Repository.Update(report);
                }
            }
            Repository.Save();
            mediator.Enqueue(new UpdateAccUserRatingCommand(advertise.Id));
            mediator.Enqueue(new UpdateAccTidinessRatingCommand(advertise.Id));
        }

        public void SubmitAdvertiseScore(int userId, long advertiseId, int reportId,
            int score, int operatorId = 0)
        {
            var acc = Repository.Find<Advertise, long>(advertiseId);
            var reserve = acc.Reserves.FirstOrDefault(x => x.UserID == userId);
            if (reserve == null)
            {
                return;
            }
            ReportItem report = acc.ReportItems.FirstOrDefault(s =>
                s.UserID == userId && s.ReportID == reportId);
            if (report == null)
            {
                report = new ReportItem();
                report.AdvertiseID = advertiseId;
                report.CreateDate = DateTime.Now;
                report.LastModifyDate = DateTime.Now;
                report.LastModifyDatetick = DateTime.Now.Ticks;
                report.ReportID = reportId;
                report.Score = score;
                report.OperatorID = operatorId;
                report.UserID = userId;
                Repository.Insert(report);
                Repository.Save();
            }
            else
            {
                report.Score = score;
                report.LastModifyDate = DateTime.Now;
                report.LastModifyDatetick = DateTime.Now.Ticks;
                report.OperatorID = operatorId;
                Repository.Update(report);
                Repository.Save();
            }
            mediator.Enqueue(new UpdateAccUserRatingCommand(acc.Id));
            if (reportId == 1)
            {
                mediator.Enqueue(new UpdateAccTidinessRatingCommand(acc.Id));
            }
        }
    }
}
