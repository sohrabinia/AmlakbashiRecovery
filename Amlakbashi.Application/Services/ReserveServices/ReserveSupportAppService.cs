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

namespace Amlakbashi.Application.Services.ReserveServices
{
    internal class ReserveSupportAppService : AppServiceBase<ReserveSupport, int>, IReserveSupportAppService
    {
        public ReserveSupportAppService(IRepository<ReserveSupport, int> repository, ICacheManager<ReserveSupport> cache) : base(repository, cache)
        {
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
    }
}
