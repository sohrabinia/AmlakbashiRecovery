using Amlakbashi.Core.Common.Repository;
using Amlakbashi.Core.Entities;
using Amlakbashi.Mediator.Commands.AdvertiseCommands;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Amlakbashi.Application.Services.ReserveServices.CommandHandlers
{
    public class ReserveAutoCancelCommandHandler :
        IRequestHandler<RefreshReserveAutoCancelCommand>,
        IRequestHandler<ScheduleReserveAutoCancelCommand>
    {
        private readonly IRepository<ReserveAutoCancel, long> repository;
        private readonly IMediator mediator;
        public ReserveAutoCancelCommandHandler(
            IRepository<ReserveAutoCancel, long> repository,
            IMediator mediator)
        {
            this.repository = repository;
            this.mediator = mediator;
        }

        public Task<Unit> Handle(RefreshReserveAutoCancelCommand request, CancellationToken cancellationToken)
        {
            var now = DateTime.Now;
            var queue = repository.Query(q => q.Where(w => w.ScheduledTime <= now)).ToList();
            foreach (var item in queue)
            {
                var result = mediator.Send(new SystemCancelReserveCommand(item.ReserveId, item.SendSms, item.Force)).Result;
                if (result)
                {
                    repository.Delete(item.Id);
                }
            }
            //repository.Delete(q => q.ScheduledTime <= now);
            repository.Save();
            return Task.FromResult(Unit.Value);
        }

        public Task<Unit> Handle(ScheduleReserveAutoCancelCommand request, CancellationToken cancellationToken)
        {
            var reserveAutoCansel = repository.Query(q => q.FirstOrDefault(f => f.ReserveId == request.reserveId));
            if (reserveAutoCansel != null)
            {
                reserveAutoCansel.ScheduledTime = DateTime.Now.Add(request.delay);
                repository.Update(reserveAutoCansel);
                repository.Save();
            }
            return Task.FromResult(Unit.Value);
        }
    }
}
