using Amlakbashi.Application.Services.ReserveServices.Interfaces;
using Amlakbashi.Core.Common.Extensions;
using Amlakbashi.Core.Entities;
using Amlakbashi.Mediator.Commands.ReserveCommands;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using static Amlakbashi.Core.Entities.ReserveSupport;

namespace Amlakbashi.Application.Services.ReserveServices.ReserveSupportManager
{
    internal class ReserveSupportManager : IReserveSupportManager
    {
        private readonly IReserveSupportAppService reserveSupportService;
        private readonly IReserveAppService reserveService;
        private readonly IMediator mediator;
        public ReserveSupportManager(IMediator mediator,
            IReserveSupportAppService reserveSupportService,
            IReserveAppService reserveService)
        {
            this.reserveSupportService = reserveSupportService;
            this.reserveService = reserveService;
            this.mediator = mediator;
        }

        public void ReserveAddHandle(long reserveId, SupportStatus? forceStatus = null)
        {
            var supports = reserveSupportService.GetRelatedSupports(reserveId);
            var currentSupport = supports.FirstOrDefault(x => x.Status == SupportStatus.Supporting ||
                x.Status == SupportStatus.Done ||
                x.Status == (int)SupportStatus.WaitingForSupport);
            var reserve = reserveService.Find(reserveId);
            if (currentSupport == null)
            {
                var newSupport = new ReserveSupport()
                {
                    GuestID = reserve.UserID,
                    JourneyStartDate = reserve.StartDate
                };
                newSupport.AddReserveId(reserve.Id,
                    SupportReserveStatus.WaitingForSupport);
                var allReserves = reserveService.GetByUserId(reserve.UserID);
                allReserves = allReserves.Where(x => x.StartDate == reserve.StartDate).ToList();
                newSupport.AddReserveId(allReserves.Select(x => x.Id).ToArray(),
                    SupportReserveStatus.WaitingForSupport);
                if (forceStatus != null)
                {
                    newSupport.Status = (SupportStatus)forceStatus;
                }
                reserveSupportService.Insert(newSupport);
            }
            else
            {
                if (currentSupport.Status == (int)SupportStatus.WaitingForSupport)
                {
                    currentSupport.AddReserveId(reserve.Id, SupportReserveStatus.WaitingForSupport);
                }
                else
                {
                    currentSupport.AddReserveId(reserve.Id, SupportReserveStatus.Similar);
                }
                if (forceStatus != null)
                {
                    currentSupport.Status = (SupportStatus)forceStatus;
                }
                reserveSupportService.Update(currentSupport);
            }
        }

        public void ReserveDoneHandle(long reserveId)
        {
            var supports = reserveSupportService.GetRelatedSupports(reserveId);
            var currentSupport = supports.FirstOrDefault(x =>
                x.Status == SupportStatus.Supporting);
            if (currentSupport != null)
            {
                currentSupport.Status = SupportStatus.Done;
                reserveSupportService.Update(currentSupport);
            }
        }

        public void ReserveCancelAfterDoneHandler(long reserveId)
        {
            var supports = reserveSupportService.GetRelatedSupports(reserveId);
            var currentSupport = supports.FirstOrDefault(x =>
                x.Status == SupportStatus.Done);
            if (currentSupport != null)
            {
                currentSupport.Status = SupportStatus.Supporting;
                reserveSupportService.Update(currentSupport);
            }
        }

        public SupporterStatus Analyze(Reserve reserve,
            out ReserveSupport currentReserveSupport, int yourUserID = 0)
        {
            var supports = reserveSupportService.GetRelatedSupports(reserve);
            return Analyze(reserve.Id, supports, out currentReserveSupport, yourUserID);
        }

        public SupporterStatus Analyze(long reserveId, IList<ReserveSupport> supports, 
            out ReserveSupport currentReserveSupport, int yourUserID = 0)
        {
            if (!supports.Any() || !supports.Any(x => x.SupporterID > 0))
            {
                if (!supports.Any())
                {
                    ReserveAddHandle(reserveId);
                }
                currentReserveSupport = null;
                return SupporterStatus.Free;
            }
            currentReserveSupport = supports.FirstOrDefault(x => x.SupporterID > 0 && (
                x.Status == SupportStatus.Supporting ||
                x.Status == SupportStatus.Done));
            if (currentReserveSupport == null)
            {
                var rs = supports.FirstOrDefault(x => x.SupporterID > 0);
                if (rs.Status == SupportStatus.Expired)
                {
                    currentReserveSupport = rs;
                    return SupporterStatus.Expired;
                }
                return SupporterStatus.Free;
            }
            if (currentReserveSupport.Status == SupportStatus.Done)
            {
                return SupporterStatus.Done;
            }
            if (currentReserveSupport.SupporterID == yourUserID)
            {
                return SupporterStatus.SupportingByYou;
            }
            else
            {
                return SupporterStatus.SupportingByOthers;
            }
        }

        public void AddSupporterToReserve(long reserveId, int supporterId,
            string transferReason = null)
        {
            var supports = reserveSupportService.GetRelatedSupports(reserveId);
            var currentSupport = supports.FirstOrDefault(x =>
                x.Status == SupportStatus.Supporting ||
                x.Status == SupportStatus.WaitingForSupport ||
                x.Status == SupportStatus.Done ||
                x.Status == SupportStatus.Expired);
            var reserve = reserveService.Find(reserveId);
            if (currentSupport != null &&
                currentSupport.SupporterID == supporterId)
            {
                if (currentSupport.Status == SupportStatus.Expired)
                {
                    transferReason = null;
                }
                else
                {
                    return;
                }
            }
            switch (currentSupport.Status)
            {
                case SupportStatus.WaitingForSupport:
                    currentSupport.SupporterID = supporterId;
                    currentSupport.StartSupportDate = DateTime.Now;
                    var reserveIds = currentSupport.GetReserveIds(
                        SupportReserveStatus.WaitingForSupport);
                    foreach (var id in reserveIds)
                    {
                        if (id == reserve.Id)
                        {
                            currentSupport.AddReserveId(id,
                                SupportReserveStatus.Supporting);
                        }
                        else
                        {
                            currentSupport.AddReserveId(id,
                                SupportReserveStatus.Similar);
                        }
                    }
                    currentSupport.ReservesWaitingForSupport = null;
                    currentSupport.Status = SupportStatus.Supporting;
                    break;
                case SupportStatus.Supporting:
                case SupportStatus.Done:
                case SupportStatus.Expired:
                    var newSupport = new Amlakbashi.Core.Entities.ReserveSupport()
                    {
                        GuestID = currentSupport.GuestID,
                        JourneyStartDate = currentSupport.JourneyStartDate,
                        SupporterID = supporterId,
                        StartSupportDate = DateTime.Now,
                    };
                    var allReserveIds = currentSupport.GetAllReserveIds().ToList();
                    if (currentSupport.Status == SupportStatus.Done)
                    {
                        var allReserves = reserveService.GetByUserId(currentSupport.GuestID);
                        Reserve tempReserve;
                        long tempReserveId;
                        for (int i = 0; i < allReserveIds.Count; i++)
                        {
                            tempReserveId = allReserveIds[i];
                            tempReserve = allReserves.FirstOrDefault(x => (int)x.Id == tempReserveId);
                            if (tempReserve == null || ((int)tempReserve.Status > 4 && (int)tempReserve.Status < 9))
                            {
                                allReserveIds.RemoveAt(i);
                                i--;
                            }
                        }
                    }
                    newSupport.AddReserveId(allReserveIds.Where(x => x != reserve.Id).ToArray(),
                        SupportReserveStatus.Similar);
                    newSupport.AddReserveId(reserve.Id, SupportReserveStatus.Supporting);
                    if (currentSupport.Status == SupportStatus.Done)
                    {
                        newSupport.Status = SupportStatus.Done;
                    }
                    else
                    {
                        newSupport.Status = SupportStatus.Supporting;
                    }
                    reserveSupportService.Insert(newSupport);

                    currentSupport.Status = SupportStatus.Transfered;
                    currentSupport.TransferReason = transferReason;
                    break;
            }
            reserveSupportService.Update(currentSupport);
            reserveSupportService.UpdateSupporterSupportsActionDate(supporterId);
        }

        public bool IsInSupporterStatus(Reserve reserve,
            SupporterStatus supporterStatus, int yourUserID)
        {
            ReserveSupport currentReserveSupport;
            var supports = reserveSupportService.GetRelatedSupports(reserve);
            var count = supports.Count;
            if (count < 1 || !supports.Any(x => x.SupporterID > 0))
            {
                if (count < 1)
                {
                    mediator.Enqueue(new ReserveAddHandleCommand(reserve.Id));
                }
                currentReserveSupport = null;
                if (supporterStatus == SupporterStatus.Free)
                    return true;
            }
            currentReserveSupport = supports.FirstOrDefault(x => x.SupporterID > 0 && (
                x.Status == SupportStatus.Supporting || x.Status == SupportStatus.Done));
            if (currentReserveSupport == null)
            {
                var rs = supports.FirstOrDefault(x => x.SupporterID > 0);
                if (rs != null && rs.Status == SupportStatus.Expired)
                {
                    currentReserveSupport = rs;
                    if (supporterStatus == SupporterStatus.Expired)
                        return true;
                }
                if (supporterStatus == SupporterStatus.Free)
                    return true;
            }
            else
            {
                if (currentReserveSupport.Status == SupportStatus.Done)
                {
                    if (supporterStatus == SupporterStatus.Done)
                        return true;
                }
                if (currentReserveSupport.SupporterID == yourUserID)
                {
                    if (supporterStatus == SupporterStatus.SupportingByYou)
                        return true;
                }
                else
                {
                    if (supporterStatus == SupporterStatus.SupportingByOthers)
                        return true;
                }
            }
            return false;
        }

        public IQueryable<Reserve> FilterBySupporterStatus(int yourUserID,
            IQueryable<Reserve> reserves, SupporterStatus supporterStatus)
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
    }
}
