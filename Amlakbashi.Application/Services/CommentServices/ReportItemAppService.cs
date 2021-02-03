using Amlakbashi.Core.Common.AppService;
using Amlakbashi.Core.Common.Repository;
using Amlakbashi.Application.Services.CommentServices.Interfaces;
using Amlakbashi.Core.Common.Caching;
using Amlakbashi.Core.Entities;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System;
using MediatR;
using Amlakbashi.Core.Common.Extensions;
using Amlakbashi.Mediator.Commands.AdvertiseCommands;

namespace Amlakbashi.Application.Services.CommentServices
{
    internal class ReportItemAppService : AppServiceBase<ReportItem, long>, IReportItemAppService
    {
        private readonly IMediator mediator;
        public ReportItemAppService(IRepository<ReportItem, long> repository,
            IMediator mediator,
            ICacheManager<ReportItem> cache) : base(repository, cache)
        {
            this.mediator = mediator;
        }

        public IList<ReportItem> GetAll()
        {
            return Repository.Query(q => q).ToList();
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

        public Dictionary<int, List<ReportItem>> GetByAccId(long advertiseId)
        {
            IQueryable<ReportItem> data = Repository.Query(q =>
                q.Where(w => w.AdvertiseID == advertiseId));
            var dict = new Dictionary<int, List<ReportItem>>();
            foreach (var rp in data)
            {
                if (dict.ContainsKey(rp.UserID))
                {
                    dict[rp.UserID].Add(rp);
                }
                else
                {
                    dict.Add(rp.UserID, new List<ReportItem>() { rp });
                }
            }
            return dict;
        }

        public int GetCountByAccId(long accId, IQueryable<ReportItem> reportItems = null)
        {
            if (reportItems == null)
            {
                reportItems = Repository.Query(q => q.Where(x => x.AdvertiseID == accId));
            }
            return reportItems.GroupBy(g => g.UserID).Count();
        }

        public float GetOverallRatingDecimal(long accId, int reportId = -1)
        {
            var reportItems = Repository.Query(q => q);
            if (reportId == -1)
            {
                reportItems = reportItems.Where(x => x.AdvertiseID == accId);
            }
            else
            {
                reportItems = reportItems.Where(
                    x => x.AdvertiseID == accId && x.ReportID == reportId);
            }
            if (reportItems.Count() > 0)
            {
                var overall_rating = reportItems.Average(x => x.Score);
                return (float)overall_rating;
            }
            else
            {
                return 0;
            }
        }

        public int GetAdvertiseRatingOfUser(int userId, long advertiseId, int reportId)
        {
            var report_item = Repository.Query(q => q.FirstOrDefault(x =>
                  x.UserID == userId &&
                  x.AdvertiseID == advertiseId &&
                  x.ReportID == reportId));
            return report_item != null ? report_item.Score : 0;
        }

        public int GetAdvertiseRating(long advertise_id, out int count)
        {
            var reportItems = Repository.Query(q => q);
            reportItems = reportItems.Where(x => x.AdvertiseID == advertise_id);
            count = reportItems.GroupBy(x => x.UserID).Count();
            return count > 0 ? (int)reportItems.Average(x => x.Score) : 0;
        }

        public float GetAverageRatingForAdvertise(long advertise_id, int user_id = 0)
        {
            var reportItems = Repository.Query(q => q.Where(x => x.AdvertiseID == advertise_id));
            if (user_id > 0)
            {
                reportItems = reportItems.Where(x => x.UserID == user_id);
            }
            return reportItems.Count() == 0 ? 0 : reportItems.Average(x => (float)x.Score);
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
            ReportItem report = acc.ReportItems.SingleOrDefault(s =>
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
