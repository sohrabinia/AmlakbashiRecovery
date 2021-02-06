using Amlakbashi.Core.Common.Repository;
using Amlakbashi.Core.Entities;
using Amlakbashi.Mediator.Commands.AdvertiseCommands;
using Amlakbashi.Mediator.Commands.ReserveCommands;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Amlakbashi.Application.Services.ReserveServices.CommandHandlers
{
    public class ExtrinsicReserveCommandHandler : IRequestHandler<RemoveOldExtrinsicReserveCommand>
    {
        private readonly IRepository<ExtrinsicReserve, long> repository;
        private readonly IMediator mediator;
        public ExtrinsicReserveCommandHandler(IRepository<ExtrinsicReserve, long> repository,
            IMediator mediator)
        {
            this.repository = repository;
            this.mediator = mediator;
        }

        public Task<Unit> Handle(RemoveOldExtrinsicReserveCommand request, CancellationToken cancellationToken)
        {
            mediator.Send(new RemoveOldOccupiedTableCommand());
            var oldDate = DateTime.Now.Date.AddDays(-3);
            repository.Delete(e => e.StartDate < oldDate);
            repository.Save();
            return Task.FromResult(Unit.Value);
        }
    }
}
