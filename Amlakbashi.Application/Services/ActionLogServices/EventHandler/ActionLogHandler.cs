using Amlakbashi.Core.Common.Repository;
using Amlakbashi.Core.Entities;
using Amlakbashi.Mediator.Events.AdvertiseEvents;
using Amlakbashi.Mediator.Events.UserEvents;
using MediatR;
using Newtonsoft.Json;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Amlakbashi.Application.Services.ActionLogServices.EventHandler
{
    internal class ActionLogHandler :
        INotificationHandler<UserUpdateEvent>,
        INotificationHandler<BankCardUpdateEvent>,
        INotificationHandler<AdvertiseUpdateEvent>
    {
        private readonly IRepository<ActionLog, long> repository;
        public ActionLogHandler(IRepository<ActionLog, long> repository)
        {
            this.repository = repository;
        }

        public Task Handle(UserUpdateEvent notification, CancellationToken cancellationToken)
        {
            var log = new ActionLog()
            {
                Date = DateTime.Now,
                ActionSource = (int)notification.ActionSource,
                Type = (int)ActionLog.ActionTypeEnum.User,
                RelatedID = notification.CurrentData.Id,
                UserID = notification.CurrentUserId,
                PreviousData = notification.PreviusData == null ? null :
                    JsonConvert.SerializeObject(notification.PreviusData),
                CurrentData = notification.CurrentData == null ? null :
                    JsonConvert.SerializeObject(notification.CurrentData)
            };
            repository.Insert(log);
            repository.Save();
            return Task.CompletedTask;
        }

        public Task Handle(BankCardUpdateEvent notification, CancellationToken cancellationToken)
        {
            var log = new ActionLog()
            {
                Date = DateTime.Now,
                ActionSource = (int)notification.ActionSource,
                Type = (int)ActionLog.ActionTypeEnum.BankCard,
                RelatedID = notification.CurrentData.UserID,
                UserID = notification.CurrentUserId,
                PreviousData = notification.PreviusData == null ? null :
                    JsonConvert.SerializeObject(notification.PreviusData),
                CurrentData = notification.CurrentData == null ? null :
                    JsonConvert.SerializeObject(notification.CurrentData)
            };
            repository.Insert(log);
            repository.Save();
            return Task.CompletedTask;
        }

        public Task Handle(AdvertiseUpdateEvent notification, CancellationToken cancellationToken)
        {
            var log = new ActionLog()
            {
                Date = DateTime.Now,
                ActionSource = (int)notification.ActionSource,
                Type = (int)ActionLog.ActionTypeEnum.Advertise,
                RelatedID = notification.CurrentData.Id,
                UserID = notification.CurrentUserId,
                PreviousData = notification.PreviusData == null ? null :
                    JsonConvert.SerializeObject(notification.PreviusData),
                CurrentData = notification.CurrentData == null ? null :
                    JsonConvert.SerializeObject(notification.CurrentData)
            };
            repository.Insert(log);
            repository.Save();
            return Task.CompletedTask;
        }
    }
}
