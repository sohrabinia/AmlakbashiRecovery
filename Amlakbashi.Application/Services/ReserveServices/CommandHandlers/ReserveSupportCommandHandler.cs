using Amlakbashi.Application.Services.ReserveServices.ReserveSupportManager;
using Amlakbashi.Core.Common.Repository;
using Amlakbashi.Core.Entities;
using Amlakbashi.Mediator.Commands.ReserveCommands;
using MediatR;
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Amlakbashi.Application.Services.ReserveServices.CommandHandlers
{
    public class ReserveSupportCommandHandler : IRequestHandler<ReserveAddHandleCommand>,
        IRequestHandler<UpdateReserveSupportExpirationCommand>
    {
        private readonly IReserveSupportManager reserveSupportManager;
        private readonly IRepository<ReserveSupport, int> reserveSupportRepository;
        public ReserveSupportCommandHandler(IRepository<ReserveSupport, int> reserveSupportRepository,
            IReserveSupportManager reserveSupportManager)
        {
            this.reserveSupportManager = reserveSupportManager;
            this.reserveSupportRepository = reserveSupportRepository;
        }

        public Task<Unit> Handle(ReserveAddHandleCommand request, CancellationToken cancellationToken)
        {
            reserveSupportManager.ReserveAddHandle(request.ReserveId, null);
            return Task.FromResult(Unit.Value);
        }

        public Task<Unit> Handle(UpdateReserveSupportExpirationCommand request, CancellationToken cancellationToken)
        {
            IQueryable<ReserveSupport> reserveSupports = reserveSupportRepository.Query(q => q.Include("Guest.Reserves"));
            reserveSupports = reserveSupports.Where(x => x.Status != ReserveSupport.SupportStatus.Expired);
            reserveSupports = reserveSupports.Where(x => x.Status < ReserveSupport.SupportStatus.Done);
            List<long> tempReserveIds;
            var now = DateTime.Now;
            DateTime tempLastDate;
            var maxDuration = new TimeSpan(48, 0, 0);
            foreach (var rs in reserveSupports)
            {
                tempReserveIds = rs.GetAllReserveIds().ToList();
                var tempReserves = rs.Guest.Reserves.ToList();
                tempReserves = tempReserves.Where(x => tempReserveIds.Contains(x.Id)).ToList();
                if (!tempReserves.Any(x => (int)x.Status > 4 && (int)x.Status < 9))
                {
                    tempLastDate = tempReserves.Any() ? tempReserves.Max(x => x.CreateDate) : DateTime.Now;
                    if (now - tempLastDate > maxDuration)
                    {
                        rs.Status = ReserveSupport.SupportStatus.Expired;
                        reserveSupportRepository.Update(rs);
                    }
                }
            }
            reserveSupportRepository.Save();
            return Task.FromResult(Unit.Value);
        }
    }
}
