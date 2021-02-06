using Amlakbashi.Core.Common.Repository;
using Amlakbashi.Mediator.Events.AdvertiseEvents;
using Amlakbashi.Core.Entities;
using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static Amlakbashi.Core.Entities.Advertise;
using static Amlakbashi.Core.Entities.User;
using Amlakbashi.Mediator.Events.AccountingEvents;
using Amlakbashi.Mediator.Events.UserEvents;

namespace Amlakbashi.Application.Services.UserServices.EventHandlers
{
    internal class UserEventHandler :
        INotificationHandler<ChangeInstantReserveStatusEvent>,
        INotificationHandler<CreateAdvertiseBasicEvent>,
        INotificationHandler<PrizeCreditUpdateEvent>,
        INotificationHandler<PresentorPrizeGivenEvent>,
        INotificationHandler<AppreciateDiscountGivenEvent>,
        INotificationHandler<CreditTransactionUpdateEvent>
    {
        private readonly IRepository<User, int> repository;
        private readonly IMediator mediator;
        public UserEventHandler(IRepository<User, int> repository, IMediator mediator)
        {
            this.repository = repository;
            this.mediator = mediator;
        }
        public Task Handle(ChangeInstantReserveStatusEvent notification, CancellationToken cancellationToken)
        {
            if (notification.newStatus == InstantReserveStatusEnum.Requested)
            {
                var user = repository.Query(q => q.FirstOrDefault(f => f.Id == notification.userId));
                var oldUser = user.ShallowCopy();
                user.InstantReserveAccess = InstantReserveAccessEnum.Requested;
                repository.Update(user);
                repository.Save();
                mediator.Publish(new UserUpdateEvent(oldUser, user, notification.actionSource, notification.doerUserId));
            }
            return Task.CompletedTask;
        }

        public Task Handle(CreateAdvertiseBasicEvent notification, CancellationToken cancellationToken)
        {
            var user = repository.Query(q => q.FirstOrDefault(f => f.Id == notification.userId));
            if (user.UserGeneralType == (int)UserGeneralTypeEnum.Guest)
            {
                user.UserGeneralType = (int)UserGeneralTypeEnum.Host;
                repository.Update(user);
                repository.Save();
            }
            return Task.CompletedTask;
        }

        public Task Handle(PrizeCreditUpdateEvent notification, CancellationToken cancellationToken)
        {
            var user = repository.Find(notification.UserId);
            var oldUser = user.ShallowCopy();
            if (user.PrizeCreditTransactions != null &&
                user.PrizeCreditTransactions.Any())
            {
                user.PrizeCredit = user.PrizeCreditTransactions.OrderByDescending(o => o.Id)
                    .FirstOrDefault().RemainedPrice;
                repository.Update(user);
                repository.Save();

                if (notification.ActionSource != ActionLog.ActionSourceEnum.Undefined)
                {
                    mediator.Publish(new UserUpdateEvent(oldUser, user,
                        notification.ActionSource, notification.CurrentUserId));
                }
            }
            return Task.CompletedTask;
        }

        public Task Handle(PresentorPrizeGivenEvent notification, CancellationToken cancellationToken)
        {
            var user = repository.Find(notification.UserId);
            var oldUser = user.ShallowCopy();
            user.PresentorPrizeGiven = true;
            repository.Update(user);
            repository.Save();
            mediator.Publish(new UserUpdateEvent(oldUser, user,
                notification.ActionSource, notification.DoerUserId));
            return Task.CompletedTask;
        }

        public Task Handle(AppreciateDiscountGivenEvent notification, CancellationToken cancellationToken)
        {
            var user = repository.Find(notification.userId);
            var oldUser = user.ShallowCopy();
            user.RecieveAppreciateDiscount = true;
            repository.Update(user);
            repository.Save();
            mediator.Publish(new UserUpdateEvent(oldUser, user,
                notification.actionSource, notification.doerUserId));
            return Task.CompletedTask;
        }

        public Task Handle(CreditTransactionUpdateEvent notification, CancellationToken cancellationToken)
        {
            var user = repository.Find(notification.UserId);
            var oldUser = user.ShallowCopy();
            user.Credit = notification.currentCredit;
            repository.Update(user);
            repository.Save();

            if (notification.ActionSource != ActionLog.ActionSourceEnum.Undefined)
            {
                mediator.Publish(new UserUpdateEvent(oldUser, user,
                    notification.ActionSource, notification.CurrentUserId));
            }
            return Task.CompletedTask;
        }
    }
}
