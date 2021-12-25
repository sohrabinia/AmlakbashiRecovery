using Amlakbashi.Core.Common.AppService;
using Amlakbashi.Core.Common.Repository;
using Amlakbashi.Application.Services.ReserveServices.Interfaces;
using Amlakbashi.Core.Common.Caching;
using Amlakbashi.Core.Entities;
using Amlakbashi.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amlakbashi.Mediator.Commands.ReserveCommands;
using MediatR;
using Amlakbashi.Core.Common.Extensions;

namespace Amlakbashi.Application.Services.ReserveServices
{
    internal class ReserveSupportAppService : AppServiceBase<ReserveSupport, int>, IReserveSupportAppService
    {
        private readonly IMediator mediator;
        public ReserveSupportAppService(IRepository<ReserveSupport, int> repository, IMediator mediator) : base(repository)
        {
            this.mediator = mediator;
        }

        public void Insert(ReserveSupport item)
        {
            item.CreateDate = DateTime.Now;
            item.LastModifyDate = DateTime.Now;
            Repository.Insert(item);
            Repository.Save();
        }

        public void Update(ReserveSupport item)
        {
            var reserveSupport = Repository.Find(item.Id);
            var hasChange = false;
            if (reserveSupport.Status != item.Status)
            {
                hasChange = true;
                reserveSupport.Status = item.Status;
            }
            if (reserveSupport.SupporterID != item.SupporterID)
            {
                hasChange = true;
                reserveSupport.SupporterID = item.SupporterID;
            }
            if (reserveSupport.TransferReason != item.TransferReason)
            {
                hasChange = true;
                reserveSupport.TransferReason = item.TransferReason;
            }
            if (reserveSupport.StartSupportDate != item.StartSupportDate)
            {
                hasChange = true;
                reserveSupport.StartSupportDate = item.StartSupportDate;
            }
            if (reserveSupport.ReservesSimilar != item.ReservesSimilar)
            {
                hasChange = true;
                reserveSupport.ReservesSimilar = item.ReservesSimilar;
            }
            if (reserveSupport.ReservesWaitingForSupport != item.ReservesWaitingForSupport)
            {
                hasChange = true;
                reserveSupport.ReservesWaitingForSupport = item.ReservesWaitingForSupport;
            }
            if (reserveSupport.ReservesSupporting != item.ReservesSupporting)
            {
                hasChange = true;
                reserveSupport.ReservesSupporting = item.ReservesSupporting;
            }
            if (hasChange)
            {
                reserveSupport.LastModifyDate = DateTime.Now;
                Repository.Update(reserveSupport);
                Repository.Save();
            }
        }

        public void UpdateSupporterSupportsActionDate(int supporterId)
        {
            var oneMonthAgo = DateTime.Now.Date.AddDays(-30);
            var items = Repository.Query(q => q.Where(
                w => w.SupporterID == supporterId &&
                w.CreateDate > oneMonthAgo));
            foreach (var item in items)
            {
                item.LastSupporterActionDate = DateTime.Now;
                item.LastModifyDate = DateTime.Now;
            }
            Repository.Save();
        }

        public IList<ReserveSupport> GetRelatedSupports(long reserveId)
        {
            var reserve = Repository.Find<Reserve, long>(reserveId);
            var supports = reserve.GuestUser.ReserveSupportsAsGuest.AsQueryable();
            var ids = new List<int>();
            foreach (var support in supports)
            {
                if (support.JourneyStartDate == reserve.StartDate)
                {
                    ids.Add(support.Id);
                    continue;
                }
                if (support.LastSupporterActionDate == null)
                    continue;
                if (Math.Abs(((DateTime)support.LastSupporterActionDate -
                    reserve.CreateDate).TotalMinutes) < 180)
                {
                    ids.Add(support.Id);
                }
            }
            supports = supports.Where(x => ids.Contains(x.Id));
            return supports.ToList();
        }

        public IList<ReserveSupport> GetRelatedSupports(Reserve reserve)
        {
            var supports = reserve.GuestUser.ReserveSupportsAsGuest.AsQueryable();
            var ids = new List<int>();
            foreach (var support in supports)
            {
                if (support.JourneyStartDate == reserve.StartDate)
                {
                    ids.Add(support.Id);
                    continue;
                }
                if (support.LastSupporterActionDate == null)
                    continue;
                if (Math.Abs(((DateTime)support.LastSupporterActionDate -
                    reserve.CreateDate).TotalMinutes) < 180)
                {
                    ids.Add(support.Id);
                }
            }
            supports = supports.Where(x => ids.Contains(x.Id));
            return supports.ToList();
        }

        public IList<ReserveSupport> GetListBySupporterId(int supporterId)
        {
            return Repository.Query(q => q.Where(w => w.SupporterID == supporterId)).ToList();
        }

        public IQueryable<Reserve> FilterBySupporterStatus(int yourUserID,
            IQueryable<Reserve> reserves, ReserveSupport.SupporterStatus supporterStatus)
        {
            var reserve_ids = new List<long>();
            foreach (var reserve in reserves)
            {
                if (IsInSupporterStatus(reserve, supporterStatus, yourUserID))
                {
                    reserve_ids.Add(reserve.Id);
                }
            }
            return reserves.Where(x => reserve_ids.Contains(x.Id));
        }

        public bool IsInSupporterStatus(Reserve reserve,
            ReserveSupport.SupporterStatus supporterStatus, int yourUserID)
        {
            ReserveSupport currentReserveSupport;
            var supports = GetRelatedSupports(reserve);
            var count = supports.Count;
            if (count < 1 || !supports.Any(x => x.SupporterID > 0))
            {
                if (count < 1)
                {
                    mediator.Enqueue(new ReserveAddHandleCommand(reserve.Id)); // why enqueue?
                }
                currentReserveSupport = null;
                if (supporterStatus == ReserveSupport.SupporterStatus.Free)
                    return true;
            }
            currentReserveSupport = supports.FirstOrDefault(x => x.SupporterID > 0 && (
                x.Status == ReserveSupport.SupportStatus.Supporting || x.Status == ReserveSupport.SupportStatus.Done));
            if (currentReserveSupport == null)
            {
                var rs = supports.FirstOrDefault(x => x.SupporterID > 0);
                if (rs != null && rs.Status == ReserveSupport.SupportStatus.Expired)
                {
                    currentReserveSupport = rs;
                    if (supporterStatus == ReserveSupport.SupporterStatus.Expired)
                        return true;
                }
                if (supporterStatus == ReserveSupport.SupporterStatus.Free)
                    return true;
            }
            else
            {
                if (currentReserveSupport.Status == ReserveSupport.SupportStatus.Done)
                {
                    if (supporterStatus == ReserveSupport.SupporterStatus.Done)
                        return true;
                }
                if (currentReserveSupport.SupporterID == yourUserID)
                {
                    if (supporterStatus == ReserveSupport.SupporterStatus.SupportingByYou)
                        return true;
                }
                else
                {
                    if (supporterStatus == ReserveSupport.SupporterStatus.SupportingByOthers)
                        return true;
                }
            }
            return false;
        }
    }
}
