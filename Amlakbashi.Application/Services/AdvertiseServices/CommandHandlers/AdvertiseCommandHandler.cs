using Amlakbashi.Core.Common.Repository;
using Amlakbashi.Mediator.Events.AdvertiseEvents;
using Amlakbashi.Core.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static Amlakbashi.Core.Entities.Advertise;
using Amlakbashi.Core.Common.Utilities;
using Amlakbashi.Application.Services.SettingServices.SettingManager;
using Amlakbashi.Mediator.Commands.AdvertiseCommands;
using Microsoft.EntityFrameworkCore;
using static Amlakbashi.Core.Entities.Reserve;
using Amlakbashi.Mediator.Commands.CategoryCommands;
using Amlakbashi.Core.Common.Caching;

namespace Amlakbashi.Application.Services.AdvertiseServices.CommandHandlers
{
    public class AdvertiseCommandHandler :
        IRequestHandler<UpdateAdvertiseCategoriesCommand>,
        IRequestHandler<ClearAdvertiseCategoriesCommand>,
        IRequestHandler<UpdateAdvertiseScoreCommand>,
        IRequestHandler<UpdateTodayIsEmptyRecordsCommand>,
        IRequestHandler<UpdateAccUserRatingCommand>,
        IRequestHandler<UpdateAccTidinessRatingCommand>,
        IRequestHandler<UpdateAdvertiseOccupiedCommand>,
        IRequestHandler<InsertExtrinsicReserveCommand>,
        IRequestHandler<UpdateInstantReserveStatusCommand>,
        IRequestHandler<IncreaseInstantReserveCancelCommand>,
        IRequestHandler<SetExtrinsicReserveForWaitForResponseCommand>,
        IRequestHandler<InsertExtrinsicReserveByDateListCommand>,
        IRequestHandler<RemoveCategoryItemCacheCommand>
    {
        private readonly IMediator mediator;
        private readonly IRepository<Advertise, long> advertiseRepository;
        private readonly ISettingManager setting;
        private readonly ICacheManager cacheManager;
        public AdvertiseCommandHandler(IMediator mediator,
            IRepository<Advertise, long> advertiseRepository,
            ISettingManager setting, ICacheManager cacheManager)
        {
            this.mediator = mediator;
            this.advertiseRepository = advertiseRepository;
            this.setting = setting;
            this.cacheManager = cacheManager;
        }

        public Task<Unit> Handle(UpdateAdvertiseCategoriesCommand request, CancellationToken cancellationToken)
        {
            var accIds = advertiseRepository.Query(q => q.Where(
                w => w.Id == request.advertiseId || (w.ParentId == request.advertiseId && w.Count == 0))
                .OrderBy(o => o.Id)).Select(s => s.Id).ToList();

            var catIds = new List<int>();
            foreach (var accId in accIds)
            {
                var acc = advertiseRepository.Find(accId);
                if (acc.Categories != null && acc.Categories.Any())
                {
                    catIds.AddRange(acc.Categories.Select(s => s.Id));
                }

                mediator.Send(new RemoveCategoryItemCacheCommand(accId));
                acc.Categories.Clear();
                if (acc.IsActive == false || acc.Count > 0)
                {
                    acc.LastModifyDate = DateTime.Now;
                    advertiseRepository.Update(acc);
                    advertiseRepository.Save();
                    continue;
                }

                var categoryIds = mediator.Send(new GetCategoriesFilterCommand(acc.TypeID, acc.CountryDirection,
                    acc.Province, acc.City, acc.Area)).Result.Select(s => s.Id);

                foreach (var item in categoryIds)
                {
                    acc.Categories.Add(advertiseRepository.Find<DynamicCategory, int>(item));
                }

                foreach (var cat in acc.Categories)
                {
                    cat.LastModifyDate = DateTime.Now;
                    catIds.Add(cat.Id);
                }
                acc.LastModifyDate = DateTime.Now;
                advertiseRepository.Update(acc);
                advertiseRepository.Save();
                mediator.Send(new RemoveCategoryItemCacheCommand(accId));
            }

            catIds = catIds.Distinct().ToList();
            foreach (var catId in catIds)
            {
                mediator.Send(new UpdateCategoryPriceCommand(catId));
                mediator.Send(new UpdateCategoryMostAccCommand(catId));
                mediator.Send(new UpdateCategoryAccCountCommand(catId));
                mediator.Publish(new ChangeCategoryEvent(catId));
            }
            return Task.FromResult(Unit.Value);
        }

        public Task<Unit> Handle(ClearAdvertiseCategoriesCommand request, CancellationToken cancellationToken)
        {
            var accIds = advertiseRepository.Query(q => q.Where(
                w => w.Id == request.advertiseId ||
                w.ParentId == request.advertiseId).OrderBy(o => o.Id)).Select(s => s.Id).ToList();
            var catIds = new List<int>();
            foreach (var accId in accIds)
            {
                var acc = advertiseRepository.Find(accId);
                foreach (var cat in acc.Categories)
                {
                    cat.LastModifyDate = DateTime.Now;
                    catIds.Add(cat.Id);
                }
                mediator.Send(new RemoveCategoryItemCacheCommand(accId));
                acc.LastModifyDate = DateTime.Now;
                acc.Categories.Clear();
                advertiseRepository.Update(acc);
            }

            advertiseRepository.Save();
            foreach (var catId in catIds)
            {
                mediator.Send(new UpdateCategoryPriceCommand(catId));
                mediator.Send(new UpdateCategoryMostAccCommand(catId));
                mediator.Send(new UpdateCategoryAccCountCommand(catId));
                mediator.Publish(new ChangeCategoryEvent(catId));
            }
            return Task.FromResult(Unit.Value);
        }

        public Task<Unit> Handle(UpdateAdvertiseScoreCommand request, CancellationToken cancellationToken)
        {
            if (request.AdvertiseId < 1)
            {
                return Task.FromResult(Unit.Value);
            }
            long max_score = 0;
            long last_max_score = 10000;
            long max_click = Convert.ToInt64(last_max_score / 3.2);
            long max_contact = Convert.ToInt64(last_max_score / 1.8);

            var acc = advertiseRepository.Find(request.AdvertiseId);

            var today_persian_date = DateTimeUtility.GregorianToPersianDate(DateTime.Now);
            var tomorrow_persian_date = DateTimeUtility.GregorianToPersianDate(DateTime.Now.AddDays(1));
            long score_item = acc.AmlakbashiScore;

            var reportItems = acc.ReportItems;
            var user_rate_average = reportItems.Count() > 0 ? (int)reportItems.Average(x => x.Score) : 0;

            score_item += (user_rate_average < 1 ? 0 : ((user_rate_average - 1) * 100));
            var all_reserves = acc.Reserves;
            var today_reserves = all_reserves.Where(x => x.CreateDate.Date == DateTime.Now.Date);

            var today_requested_count = today_reserves.Count(x =>
                x.Status == Reserve.ReserveStatus.WaitForResponse ||
                x.Status == Reserve.ReserveStatus.WaitForReserve);
            var today_reserved_count = today_reserves.Count(x =>
            x.Status == Reserve.ReserveStatus.Reserved ||
            x.Status == Reserve.ReserveStatus.Started ||
            x.Status == Reserve.ReserveStatus.CashPay ||
            x.Status == Reserve.ReserveStatus.Completed);

            score_item -= (today_requested_count * 250);
            score_item -= (today_reserved_count * 1000);

            var response_late_list = new List<long>();
            int response_late_score;
            foreach (var reserve in all_reserves)
            {
                if (reserve.HostResponseDate > reserve.CreateDate)
                    response_late_list.Add((long)(reserve.HostResponseDate - reserve.CreateDate).TotalMinutes);
            }
            long response_late_average = response_late_list.Count == 0 ? 0 : (long)response_late_list.Average();

            if (response_late_average > 180)
            {
                response_late_score = 0;
            }
            else if (response_late_average > 120)
            {
                response_late_score = 10;
            }
            else if (response_late_average > 60)
            {
                response_late_score = 20;
            }
            else if (response_late_average > 30)
            {
                response_late_score = 40;
            }
            else if (response_late_average > 20)
            {
                response_late_score = 60;
            }
            else if (response_late_average > 10)
            {
                response_late_score = 80;
            }
            else if (response_late_average > 0)
            {
                response_late_score = 100;
            }
            else
            {
                response_late_score = 0;
            }

            score_item += response_late_score;

            score_item -= (all_reserves.Count(x => x.HostResponse == Reserve.HostResponseEnum.Rejected ||
                x.HostResponse == Reserve.HostResponseEnum.RejectedPrice) * 10);
            score_item -= (all_reserves.Count(x => x.HostResponse == Reserve.HostResponseEnum.RejectedHomeFull) * 20);

            var user_item = advertiseRepository.Find<User, int>(acc.UserID);
            if (user_item != null)
            {
                score_item += user_item.UserScore;
            }
            acc.AdvertiseScore = score_item;
            advertiseRepository.Update(acc);
            if (score_item > max_score)
                max_score = score_item;
            advertiseRepository.Save();
            setting.MaxScore = max_score;
            return Task.FromResult(Unit.Value);
        }

        public Task<Unit> Handle(UpdateTodayIsEmptyRecordsCommand request, CancellationToken cancellationToken)
        {
            IQueryable<Advertise> notPublishedAdvertises =
                advertiseRepository.Query(q => q.Where(
                    w => w.TodayIsEmpty == true &&
                    w.Status != AdvertiseStatus.Published));
            foreach (var item in notPublishedAdvertises)
            {
                item.TodayIsEmpty = false;
                advertiseRepository.Update(item);
            }
            advertiseRepository.Save();
            IQueryable<Advertise> notInstantReserveAdvertises =
                advertiseRepository.Query(q => q.Where(w =>
                w.Status == AdvertiseStatus.Published &&
                w.TodayIsEmpty == true &&
                w.InstantReserveStatus !=
                InstantReserveStatusEnum.Confirmed));
            foreach (var item in notInstantReserveAdvertises)
            {
                item.TodayIsEmpty = false;
                advertiseRepository.Update(item);
            }
            advertiseRepository.Save();
            IQueryable<Advertise> instantReserveAdvertises = advertiseRepository
                .Query(q => q.Include(i => i.OccupiedTables).
                Where(w => w.InstantReserveStatus ==
                InstantReserveStatusEnum.Confirmed &
                w.Status == AdvertiseStatus.Published));
            var today = DateTime.Now.Date;
            foreach (var advertise in instantReserveAdvertises)
            {
                if (advertise.OccupiedDates().Any(a => a == today))
                {
                    if (advertise.TodayIsEmpty == true)
                    {
                        advertise.TodayIsEmpty = false;
                        advertiseRepository.Update(advertise);
                    }
                }
                else
                {
                    if (advertise.TodayIsEmpty == false)
                    {
                        advertise.TodayIsEmpty = true;
                        advertiseRepository.Update(advertise);
                    }
                }
            }
            advertiseRepository.Save();
            return Task.FromResult(Unit.Value);
        }

        public Task<Unit> Handle(UpdateAccUserRatingCommand request, CancellationToken cancellationToken)
        {
            var acc = advertiseRepository.Find(request.advertiseId);
            acc.AverageUserRating = (float)acc.ReportItems.Average(a => a.Score);
            advertiseRepository.Update(acc);
            advertiseRepository.Save();
            return Task.FromResult(Unit.Value);
        }

        public Task<Unit> Handle(UpdateAccTidinessRatingCommand request, CancellationToken cancellationToken)
        {
            var acc = advertiseRepository.Find(request.advertiseId);
            acc.TidinessUserRating = (float)acc.ReportItems.Where(
                w => w.ReportID == 1).Average(a => a.Score);
            advertiseRepository.Update(acc);
            advertiseRepository.Save();
            return Task.FromResult(Unit.Value);
        }

        public Task<Unit> Handle(UpdateAdvertiseOccupiedCommand request, CancellationToken cancellationToken)
        {
            var acc = advertiseRepository.Find(request.advertiseId);
            advertiseRepository.Reload(acc);
            var reservedDates = new Dictionary<Reserve, List<DateTime>>();
            var extrinsicReservedDates = new Dictionary<ExtrinsicReserve, DateTime>();
            foreach (var reserve in acc.SuccessfullReserves())
            {
                for (var d = reserve.StartDate.Date; d < reserve.EndDate.Date; d = d.AddDays(1))
                {
                    if (reservedDates.ContainsKey(reserve))
                        reservedDates[reserve].Add(d);
                    else
                        reservedDates.Add(reserve, new List<DateTime>() { d });
                }
            }
            foreach (var extReserve in acc.ExtrinsicReserves)
            {
                extrinsicReservedDates.Add(extReserve, extReserve.StartDate);
            }
            var occIdsToRemove = new List<long>();
            advertiseRepository.RemoveChildren<OccupiedTable, long,
                IQueryable<OccupiedTable>>(request.advertiseId, "OccupiedTables",
                q => q.Where(
                w => reservedDates.Values.Any(a => a.Contains(w.Date)) == false &&
                    extrinsicReservedDates.Values.Contains(w.Date) == false).AsQueryable());
            foreach (var date in reservedDates)
            {
                foreach (var d in date.Value)
                {
                    if (acc.OccupiedTables.Any(a => a.Date == d) == false)
                    {
                        acc.OccupiedTables.Add(new OccupiedTable() { Reserve = date.Key, Date = d });
                    }
                }
            }
            var accCount = Math.Max(acc.Count, 1);
            foreach (var date in extrinsicReservedDates)
            {
                if (acc.OccupiedTables.Count(a => a.Date == date.Value) < accCount)
                {
                    acc.OccupiedTables.Add(new OccupiedTable() { ExtrinsicReserve = date.Key, Date = date.Value });
                }
            }
            advertiseRepository.Update(acc);
            advertiseRepository.Save();
            return Task.FromResult(Unit.Value);

        }

        public Task<Unit> Handle(InsertExtrinsicReserveCommand request, CancellationToken cancellationToken)
        {
            var acc = advertiseRepository.Find(request.advertiseId);
            var hostUser = acc.User;
            var range_list = DateTimeUtility.PersianDateRangeToList(
                request.fromDate, request.toDate, true, false);
            DateTime gregorian_date;
            var occupiedDatesPersian = acc.OccupiedDates().Select(
                s => DateTimeUtility.GregorianToPersianDate(s));
            for (int i = 0; i < request.count; i++)
            {
                for (int j = 0; j < range_list.Count; j++)
                {
                    var persian_date = range_list[j];
                    var exist = occupiedDatesPersian.Contains(persian_date);
                    if (!exist)
                    {
                        gregorian_date = DateTimeUtility.PersianDateToGregorian(persian_date);
                        var item = new ExtrinsicReserve()
                        {
                            HostUser = hostUser,
                            NotifierUserID = request.doerUserId,
                            StartDate = gregorian_date,
                            CreateDate = DateTime.Now
                        };
                        acc.ExtrinsicReserves.Add(item);
                        mediator.Send(new RejectRequestsInTimeCommand(request.advertiseId,
                        gregorian_date, gregorian_date.AddDays(1), request.actionSource,
                        request.doerUserId));
                    }
                }
            }
            advertiseRepository.Update(acc);
            advertiseRepository.Save();
            mediator.Send(new UpdateAdvertiseOccupiedCommand(request.advertiseId));
            return Task.FromResult(Unit.Value);
        }

        public Task<Unit> Handle(UpdateInstantReserveStatusCommand request, CancellationToken cancellationToken)
        {
            var data = advertiseRepository.Find(request.advertiseId);
            data.InstantReserveStatus = request.status;
            advertiseRepository.Update(data);
            advertiseRepository.Save();
            mediator.Publish(new ChangeInstantReserveStatusEvent(request.advertiseId,
                data.UserID, request.status, request.actionSource,
                request.doerUserId));
            return Task.FromResult(Unit.Value);
        }

        public Task<Unit> Handle(IncreaseInstantReserveCancelCommand request, CancellationToken cancellationToken)
        {
            var acc = advertiseRepository.Find(request.advertiseId);
            acc.InstantReserveCancels++;
            advertiseRepository.Update(acc);
            advertiseRepository.Save();
            return Task.FromResult(Unit.Value);
        }

        public Task<Unit> Handle(SetExtrinsicReserveForWaitForResponseCommand request, CancellationToken cancellationToken)
        {
            var acc = advertiseRepository.Find(request.AdvertiseId);
            var otherReserves = acc.Reserves.Where(w => w.Id != request.SystemCanseledReserveId &&
                (w.Status == ReserveStatus.WaitForReserve || w.Status == ReserveStatus.WaitForResponse)).ToList();

            var dateRangeList = DateTimeUtility.PersianDateRangeToList(
                DateTimeUtility.GregorianToPersianDate(request.FromDate),
                DateTimeUtility.GregorianToPersianDate(request.ToDate),
                true, false);

            if (otherReserves.Any(a => a.Status == ReserveStatus.WaitForReserve))
            {
                var waitForReserveList = otherReserves.Where(w => w.Status == ReserveStatus.WaitForReserve).ToList();
                foreach (var item in waitForReserveList)
                {
                    var waitForReserveDateRange = DateTimeUtility.PersianDateRangeToList(
                        DateTimeUtility.GregorianToPersianDate(item.StartDate),
                        DateTimeUtility.GregorianToPersianDate(item.EndDate),
                        true, false);

                    foreach (var date in waitForReserveDateRange)
                    {
                        if (dateRangeList.Any(a => a == date))
                        {
                            dateRangeList.Remove(date);
                        }
                    }
                }
            }

            mediator.Send(new InsertExtrinsicReserveByDateListCommand(request.AdvertiseId,
                request.SystemCanseledReserveId, dateRangeList, acc.UserID, ActionLog.ActionSourceEnum.Background));

            return Task.FromResult(Unit.Value);
        }

        public Task<Unit> Handle(InsertExtrinsicReserveByDateListCommand request, CancellationToken cancellationToken)
        {
            var acc = advertiseRepository.Find(request.AdvertiseId);
            var hostUser = acc.User;
            DateTime gregorianDate;
            var occupiedDatesPersian = acc.OccupiedDates().Select(s => DateTimeUtility.GregorianToPersianDate(s));
            foreach(var persianDate in request.Dates)
            {
                var exist = occupiedDatesPersian.Contains(persianDate);
                if (!exist)
                {
                    gregorianDate = DateTimeUtility.PersianDateToGregorian(persianDate);
                    var item = new ExtrinsicReserve()
                    {
                        HostUser = hostUser,
                        NotifierUserID = request.DoerUserId,
                        StartDate = gregorianDate,
                        CreateDate = DateTime.Now
                    };
                    acc.ExtrinsicReserves.Add(item);
                    mediator.Send(new RejectRequestsInTimeCommand(request.AdvertiseId, gregorianDate,
                        gregorianDate.AddDays(1), request.ActionSource, request.DoerUserId,
                        true, request.SystemCanseledReserveId, true, true));
                }
            }
            advertiseRepository.Update(acc);
            advertiseRepository.Save();
            mediator.Send(new UpdateAdvertiseOccupiedCommand(request.AdvertiseId));
            return Task.FromResult(Unit.Value);
        }

        public Task<Unit> Handle(RemoveCategoryItemCacheCommand request, CancellationToken cancellationToken)
        {
            var advertise = advertiseRepository.Find(request.AdvertiseId);
            foreach (var category in advertise.Categories)
            {
                var advertiseList = category.Advertises.OrderByDescending(o => o.AdvertiseScore).Take(12);
                if (advertiseList.Contains(advertise))
                {
                    cacheManager.Remove($"{CacheNames.Category_Item_}{category.Id}");
                }
            }
            return Task.FromResult(Unit.Value);
        }
    }
}
