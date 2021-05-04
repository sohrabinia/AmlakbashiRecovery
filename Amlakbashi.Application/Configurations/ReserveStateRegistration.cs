using Amlakbashi.Application.Services.ReserveServices.ReserveState;
using Amlakbashi.Application.Services.ReserveServices.ReserveState.Interfaces;
using Amlakbashi.Application.Services.ReserveServices.ReserveState.ReserveStates;
using Autofac;
using static Amlakbashi.Core.Entities.Reserve;

namespace Amlakbashi.Application.Configurations
{
    internal static class ReserveStateRegistration
    {
        internal static void RegisterReserveState(this ContainerBuilder builder)
        {
            builder.RegisterType<WaitResponseState>().Keyed<IReserveState>(ReserveStatus.WaitForResponse);
            builder.RegisterType<WaitReserveState>().Keyed<IReserveState>(ReserveStatus.WaitForReserve);
            builder.RegisterType<RejectedState>().Keyed<IReserveState>(ReserveStatus.Rejected);
            builder.RegisterType<ReservedState>().Keyed<IReserveState>(ReserveStatus.Reserved);
            builder.RegisterType<CashPayState>().Keyed<IReserveState>(ReserveStatus.CashPay);
            builder.RegisterType<StartedState>().Keyed<IReserveState>(ReserveStatus.Started);
            builder.RegisterType<CompletedState>().Keyed<IReserveState>(ReserveStatus.Completed);
            builder.RegisterType<SystemCancelState>().Keyed<IReserveState>(ReserveStatus.CanceledBySystem);
            builder.RegisterType<GuestCancelRequestState>().Keyed<IReserveState>(ReserveStatus.CancelRequestByGuest);
            builder.RegisterType<GuestCancelState>().Keyed<IReserveState>(ReserveStatus.CanceledByGuest);
            builder.RegisterType<HostCancelRequestState>().Keyed<IReserveState>(ReserveStatus.CancelRequestByHost);
            builder.RegisterType<HostCancelState>().Keyed<IReserveState>(ReserveStatus.CanceledByHost);

            builder.RegisterType<ReserveStateContext>()
                .As<IReserveStateContext>();
        }
    }
}
