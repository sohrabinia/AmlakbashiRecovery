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
                var hostUser = reserve.HostUser;
                var startDate = DateTimeUtility.GregorianToPersianDate(reserve.StartDate).Replace(',', '/');
                var endDate = DateTimeUtility.GregorianToPersianDate(reserve.EndDate).Replace(',', '/');
                ReserveSendSms newSms = new ReserveSendSms()
                {
                    ScheduledTime = DateTime.Now.Add(new TimeSpan(0, 5, 0)),
                    initial = false,
                    userId = hostUser.Id,
                    type = (int)UserContactType.ReserveRequest,
                    advertise_id = reserve.AdvertiseID.ToString(),
                    //user_id = hostUser.Id.ToString(),
                    user_id = string.Format("{0:n0}", reserve.TotalPrice - (reserve.TotalPrice * 0.1f)), // به جای کد مهمان، در این فیلد سهم میزبان فرستاده می شود
                    reserve_id = reserve.Id.ToString(),
                    extra_1 = startDate,
                    extra_2 = endDate + Environment.NewLine + "به مدت " + (reserve.EndDate - reserve.StartDate).TotalDays + " شب" +
                            Environment.NewLine + "مبلغ: " + string.Format("{0:n0}", reserve.TotalPrice) + " تومان",
                    extra_3 = reserve.NumberOfGuests.ToString() + " نفر" + Environment.NewLine + "کد رزرو: " + reserve.Id
                };
                mediator.Send(new ScheduleReserveSendSmsCommand(newSms));
                var delay = mediator.Send(new ScheduleSendReserveRequestCallCommand(reserve.Id)).Result;

                var scheduleItem = new ReserveAutoCancel()
                {
                    ReserveId = reserve.Id,
                    ScheduledTime = DateTime.Now.Add(delay.Add(new TimeSpan(0, 22, 0))),
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
