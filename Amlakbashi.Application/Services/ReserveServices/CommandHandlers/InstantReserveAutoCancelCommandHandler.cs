using Amlakbashi.Core.Common.Repository;
using Amlakbashi.Core.Entities;
using Amlakbashi.Mediator.Commands.AdvertiseCommands;
using MediatR;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Amlakbashi.Application.Services.ReserveServices.CommandHandlers
{
    public class InstantReserveAutoCancelCommandHandler : IRequestHandler<RefreshInstantReserveAutoCancelCommand>
    {
        private readonly IRepository<InstantReserveAutoCancel, long> instantReserveAutoCancelRepository;
        private readonly IMediator mediator;
        public InstantReserveAutoCancelCommandHandler(IMediator mediator,
            IRepository<InstantReserveAutoCancel, long> instantReserveAutoCancelRepository)
        {
            this.mediator = mediator;
            this.instantReserveAutoCancelRepository = instantReserveAutoCancelRepository;
        }

        public Task<Unit> Handle(RefreshInstantReserveAutoCancelCommand request, CancellationToken cancellationToken)
        {
            var now = DateTime.Now;
            var queue = instantReserveAutoCancelRepository.Query(q => q.Where(w => w.ScheduledTime <= now)).ToList();
            foreach (var item in queue)
            {
                mediator.Send(new SystemCancelReserveCommand(item.ReserveId, item.SendSms, item.Force));
            }
            instantReserveAutoCancelRepository.Delete(q => q.ScheduledTime <= now);
            instantReserveAutoCancelRepository.Save();
            return Task.FromResult(Unit.Value);
        }
    }
}
