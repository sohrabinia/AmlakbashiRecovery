using Amlakbashi.Core.Common.Repository;
using Amlakbashi.Core.Entities;
using Amlakbashi.Mediator.Events.ReserveEvents;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Amlakbashi.Application.Services.ReserveServices.EventHandlers
{
    public class InstantReserveAutoCancelEventHandler : INotificationHandler<ReserveRequestEvent>
    {
        private readonly IRepository<InstantReserveAutoCancel, long> repository;
        public InstantReserveAutoCancelEventHandler(
            IRepository<InstantReserveAutoCancel, long> repository)
        {
            this.repository = repository;
        }
        public Task Handle(ReserveRequestEvent notification, CancellationToken cancellationToken)
        {
            var reserve = repository.Find<Reserve, long>(notification.reserveId);
            if (reserve.InstantReserve)
            {
                repository.Insert(new InstantReserveAutoCancel()
                {
                    ReserveId = reserve.Id,
                    ScheduledTime = DateTime.Now.Add(new TimeSpan(0, 30, 0)),
                    SendSms = false,
                    Force = false
                });
                repository.Save();
            }
            return Task.CompletedTask;
        }
    }
}
