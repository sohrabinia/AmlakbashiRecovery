using Amlakbashi.Core.Common.Repository;
using Amlakbashi.Core.Common.Utilities;
using Amlakbashi.Core.Entities;
using Amlakbashi.Core.Infrastructure.UserContact;
using Amlakbashi.Mediator.Commands.AdvertiseCommands;
using Amlakbashi.Mediator.Events.ReserveEvents;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Amlakbashi.Application.Services.ReserveServices.EventHandlers
{
    public class ReserveAutoCancelEventHandler : INotificationHandler<ReserveRequestEvent>
    {
        private readonly IMediator mediator;
        private readonly IRepository<ReserveAutoCancel, long> repository;
        public ReserveAutoCancelEventHandler(IMediator mediator,
            IRepository<ReserveAutoCancel, long> repository)
        {
            this.mediator = mediator;
            this.repository = repository;
        }
        public Task Handle(ReserveRequestEvent notification, CancellationToken cancellationToken)
        {
            var reserve = repository.Find<Reserve, long>(notification.reserveId);

            if (reserve.InstantReserve == false)
            {
                var delay = mediator.Send(new ScheduleSendReserveRequestCallCommand(reserve.Id)).Result;
                var scheduleItem = new ReserveAutoCancel()
                {
                    ReserveId = reserve.Id,
                    ScheduledTime = DateTime.Now.Add(delay.Add(new TimeSpan(0, 25, 0))),
                    SendSms = true,
                    Force = false
                };
                repository.Insert(scheduleItem);
                repository.Save();
            }
            return Task.CompletedTask;
        }
    }
}
