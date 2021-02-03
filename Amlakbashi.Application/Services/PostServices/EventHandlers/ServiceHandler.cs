using Amlakbashi.Core.Common.Repository;
using Amlakbashi.Core.Entities;
using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Amlakbashi.Mediator.Events.PostEvents;

namespace Amlakbashi.Application.Services.PostServices.EventHandlers
{
    internal class ServiceHandler : INotificationHandler<DeleteServiceEvent>
    {
        private readonly IRepository<Service, int> repository;
        private readonly IRepository<ServicePost, int> servicePostRepository;
        private readonly IMediator mediator;

        public ServiceHandler(IRepository<Service, int> repository,
            IRepository<ServicePost, int> servicePostRepository,
            IMediator mediator)
        {
            this.repository = repository;
            this.servicePostRepository = servicePostRepository;
            this.mediator = mediator;
        }

        public Task Handle(DeleteServiceEvent notification, CancellationToken cancellationToken)
        {
            var childrenIds = repository.Query(q => q.Where(w => w.ParentId == notification.ServiceId))
                .Select(s => s.Id);

            foreach (var childId in childrenIds)
            {
                mediator.Publish(new DeleteServiceEvent(childId));
            }

            servicePostRepository.Delete(q => q.ServiceID == notification.ServiceId);
            servicePostRepository.Save();
            repository.Delete(notification.ServiceId);
            repository.Save();

            return Task.CompletedTask;
        }
    }
}
