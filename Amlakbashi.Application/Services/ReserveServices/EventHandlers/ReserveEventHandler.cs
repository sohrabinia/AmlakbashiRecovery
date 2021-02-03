using Amlakbashi.Application.Services.ReserveServices.ReserveSupportManager;
using Amlakbashi.Core.Common.Repository;
using Amlakbashi.Core.Entities;
using Amlakbashi.Mediator.Events.AccountingEvents;
using Amlakbashi.Mediator.Events.ReserveEvents;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Amlakbashi.Application.Services.ReserveServices.EventHandlers
{
    public class ReserveEventHandler : INotificationHandler<SetReserveCouponEvent>,
        INotificationHandler<SetReservePrizeCreditEvent>,
        INotificationHandler<ReserveRequestEvent>
    {
        private readonly IRepository<Reserve, long> repository;
        private readonly IReserveSupportManager reserveSupportManager;
        public ReserveEventHandler(IRepository<Reserve, long> repository,
            IReserveSupportManager reserveSupportManager)
        {
            this.repository = repository;
            this.reserveSupportManager = reserveSupportManager;
        }

        public Task Handle(SetReserveCouponEvent notification, CancellationToken cancellationToken)
        {
            var reserve = repository.Find(notification.ReserveId);
            reserve.CouponID = notification.CouponId;
            reserve.CouponPrice = notification.CouponPrice;
            repository.Update(reserve);
            repository.Save();
            return Task.CompletedTask;
        }

        public Task Handle(SetReservePrizeCreditEvent notification, CancellationToken cancellationToken)
        {
            var reserve = repository.Find(notification.ReserveId);
            reserve.PrizePrice = notification.PrizePrice;
            reserve.PrizeTransactionID = notification.PrizeCreditId;
            repository.Update(reserve);
            repository.Save();
            return Task.CompletedTask;
        }

        public Task Handle(ReserveRequestEvent notification, CancellationToken cancellationToken)
        {
            reserveSupportManager.ReserveAddHandle(notification.reserveId);
            return Task.CompletedTask;
        }
    }
}
