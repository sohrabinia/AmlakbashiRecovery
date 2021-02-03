using Amlakbashi.Mediator.Events.AdvertiseEvents;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using Amlakbashi.Mediator.Commands.AdvertiseCommands;
using Amlakbashi.Core.Common.Extensions;

namespace Amlakbashi.Application.Services.AdvertiseServices.EventHandlers
{
    internal class CategoryEventHandler :
        INotificationHandler<ChangeAdvertiseActiveEvent>,
        INotificationHandler<ChangeAdvertiseAddressEvent>
    {
        private readonly IMediator mediator;
        public CategoryEventHandler(IMediator mediator)
        {
            this.mediator = mediator;
        }

        public Task Handle(ChangeAdvertiseActiveEvent notification, CancellationToken cancellationToken)
        {
            switch (notification.activeState)
            {
                case ChangeAdvertiseActiveEvent.ActiveChangeState.Activated:
                    mediator.Enqueue(new UpdateAdvertiseCategoriesCommand(notification.advertiseId));
                    break;
                case ChangeAdvertiseActiveEvent.ActiveChangeState.Deactivated:
                    mediator.Enqueue(new ClearAdvertiseCategoriesCommand(notification.advertiseId));
                    break;
            }
            return Task.CompletedTask;
        }

        public Task Handle(ChangeAdvertiseAddressEvent notification, CancellationToken cancellationToken)
        {
            mediator.Send(new UpdateAdvertiseCategoriesCommand(notification.advertiseId));
            return Task.CompletedTask;
        }
    }
}
