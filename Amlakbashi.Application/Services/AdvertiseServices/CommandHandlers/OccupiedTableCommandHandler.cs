using Amlakbashi.Core.Common.Repository;
using Amlakbashi.Core.Entities;
using Amlakbashi.Mediator.Commands.AdvertiseCommands;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Amlakbashi.Application.Services.AdvertiseServices.CommandHandlers
{
    public class OccupiedTableCommandHandler : IRequestHandler<RemoveOldOccupiedTableCommand>
    {
        private readonly IRepository<OccupiedTable, long> repository;
        public OccupiedTableCommandHandler(IRepository<OccupiedTable, long> repository)
        {
            this.repository = repository;
        }

        public Task<Unit> Handle(RemoveOldOccupiedTableCommand request, CancellationToken cancellationToken)
        {
            var oldDate = DateTime.Now.Date.AddDays(-3);
            repository.Delete(e => e.Date < oldDate);
            repository.Save();
            return Task.FromResult(Unit.Value);
        }
    }
}
