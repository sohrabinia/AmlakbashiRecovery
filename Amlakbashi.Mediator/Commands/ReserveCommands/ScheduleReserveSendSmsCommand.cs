using Amlakbashi.Core.Entities;
using MediatR;

namespace Amlakbashi.Mediator.Commands.AdvertiseCommands
{
    public class ScheduleReserveSendSmsCommand : IRequest
    {
        public ReserveSendSms Data { get; set; }
        public ScheduleReserveSendSmsCommand(ReserveSendSms data)
        {
            Data = data;
        }
    }
}
