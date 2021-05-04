using Amlakbashi.Accounting.Services.CommandHandlers;
using Amlakbashi.Application.Services.ActionLogServices.EventHandler;
using Amlakbashi.Application.Services.AdvertiseServices.CommandHandlers;
using Amlakbashi.Application.Services.AdvertiseServices.EventHandlers;
using Amlakbashi.Application.Services.FileServices.CommandHandlers;
using Amlakbashi.Application.Services.PostServices.EventHandlers;
using Amlakbashi.Application.Services.ReserveServices.CommandHandlers;
using Amlakbashi.Application.Services.ReserveServices.EventHandlers;
using Amlakbashi.Application.Services.SupportChatServices.CommandHandlers;
using Amlakbashi.Application.Services.SupportChatServices.EventHandlers;
using Amlakbashi.Application.Services.UserServices.CommadHandler;
using Amlakbashi.Application.Services.UserServices.EventHandlers;
using Autofac;
using MediatR;

namespace Amlakbashi.Application.Configurations
{
    internal static class MediatorRegistration
    {
        internal static void RegisterMediatorHandlers(this ContainerBuilder builder)
        {
            builder
                .RegisterType<MediatR.Mediator>()
                .As<IMediator>()
                .InstancePerLifetimeScope();

            // request & notification handlers
            builder.Register<ServiceFactory>(context =>
            {
                var c = context.Resolve<IComponentContext>();
                return t => c.Resolve(t);
            });

            // command and event handlers
            builder.RegisterType<ActionLogHandler>().AsImplementedInterfaces().InstancePerDependency();
            builder.RegisterType<AdvertiseCommandHandler>().AsImplementedInterfaces().InstancePerDependency();
            builder.RegisterType<CategoryCommandHandler>().AsImplementedInterfaces().InstancePerDependency();
            builder.RegisterType<OccupiedTableCommandHandler>().AsImplementedInterfaces().InstancePerDependency();
            builder.RegisterType<FileCommandHandler>().AsImplementedInterfaces().InstancePerDependency();
            builder.RegisterType<ServiceHandler>().AsImplementedInterfaces().InstancePerDependency();
            builder.RegisterType<ChatCommandHandler>().AsImplementedInterfaces().InstancePerDependency();
            builder.RegisterType<ExtrinsicReserveCommandHandler>().AsImplementedInterfaces().InstancePerDependency();
            builder.RegisterType<InstantReserveAutoCancelCommandHandler>().AsImplementedInterfaces().InstancePerDependency();
            builder.RegisterType<ReserveAutoCancelCommandHandler>().AsImplementedInterfaces().InstancePerDependency();
            builder.RegisterType<ReserveCommandHandler>().AsImplementedInterfaces().InstancePerDependency();
            builder.RegisterType<ReserveSendSmsCommandHandler>().AsImplementedInterfaces().InstancePerDependency();
            builder.RegisterType<ReserveSupportCommandHandler>().AsImplementedInterfaces().InstancePerDependency();
            builder.RegisterType<SupportChatCommandHandler>().AsImplementedInterfaces().InstancePerDependency();
            builder.RegisterType<UserCommandHandler>().AsImplementedInterfaces().InstancePerDependency();
            builder.RegisterType<AccountingCommandHandler>().AsImplementedInterfaces().InstancePerDependency();
            builder.RegisterType<InstantReserveAutoCancelEventHandler>().AsImplementedInterfaces().InstancePerDependency();
            builder.RegisterType<ReserveAutoCancelEventHandler>().AsImplementedInterfaces().InstancePerDependency();
            builder.RegisterType<ReserveEventHandler>().AsImplementedInterfaces().InstancePerDependency();
            builder.RegisterType<AdvertiseEventHandler>().AsImplementedInterfaces().InstancePerDependency();
            builder.RegisterType<CategoryEventHandler>().AsImplementedInterfaces().InstancePerDependency();
            builder.RegisterType<RegionEventHandler>().AsImplementedInterfaces().InstancePerDependency();
            builder.RegisterType<InsertMessageEventHandler>().AsImplementedInterfaces().InstancePerDependency();
            builder.RegisterType<UserEventHandler>().AsImplementedInterfaces().InstancePerDependency();
        }
    }
}
