using Amlakbashi.Accounting;
using Amlakbashi.Application.Services.ActionLogServices;
using Amlakbashi.Application.Services.ActionLogServices.Interfaces;
using Amlakbashi.Application.Services.AdvertiseServices;
using Amlakbashi.Application.Services.AdvertiseServices.Interfaces;
using Amlakbashi.Application.Services.BlogPostServices;
using Amlakbashi.Application.Services.BlogPostServices.Interfaces;
using Amlakbashi.Application.Services.Category;
using Amlakbashi.Application.Services.Category.Interfaces;
using Amlakbashi.Application.Services.CommentServices;
using Amlakbashi.Application.Services.CommentServices.Interfaces;
using Amlakbashi.Application.Services.FileServices;
using Amlakbashi.Application.Services.FileServices.Interfaces;
using Amlakbashi.Application.Services.PostServices;
using Amlakbashi.Application.Services.PostServices.Interfaces;
using Amlakbashi.Application.Services.ReserveServices;
using Amlakbashi.Application.Services.ReserveServices.Interfaces;
using Amlakbashi.Application.Services.ReserveServices.ReserveState;
using Amlakbashi.Application.Services.ReserveServices.ReserveState.Interfaces;
using Amlakbashi.Application.Services.ReserveServices.ReserveState.ReserveStates;
using static Amlakbashi.Core.Entities.Reserve;
using Amlakbashi.Application.Services.SupportChatServices;
using Amlakbashi.Application.Services.SupportChatServices.Interfaces;
using Amlakbashi.Application.Services.UserServices;
using Amlakbashi.Application.Services.UserServices.Interfaces;
using Amlakbashi.Core.Infrastructure.UserContact;
using Amlakbashi.Core.Infrastructure.UserContact.Interfaces;
using Amlakbashi.Data;
using Autofac;
using MediatR;
using Amlakbashi.Application.Services.ReserveServices.ReserveSupportManager;
using Amlakbashi.Application.Services.SettingServices;
using Amlakbashi.Application.Services.SettingServices.Interfaces;
using Amlakbashi.Application.Services.SettingServices.SettingManager;
using Amlakbashi.Core.Infrastructure.FilterHelpers;
using Amlakbashi.Core.Infrastructure.FilterHelpers.Interfaces;
using Amlakbashi.Application.Services.AdvertiseServices.CommandHandlers;
using Amlakbashi.Application.Services.ActionLogServices.EventHandler;
using Amlakbashi.Application.Services.AdvertiseServices.EventHandlers;
using Amlakbashi.Application.Services.FileServices.CommandHandlers;
using Amlakbashi.Application.Services.PostServices.EventHandlers;
using Amlakbashi.Application.Services.ReserveServices.CommandHandlers;
using Amlakbashi.Application.Services.ReserveServices.EventHandlers;
using Amlakbashi.Application.Services.SupportChatServices.CommandHandlers;
using Amlakbashi.Application.Services.SupportChatServices.EventHandlers;
using Amlakbashi.Application.Services.UserServices.CommadHandler;
using Amlakbashi.Application.Services.UserServices.EventHandlers;

namespace Amlakbashi.Application
{
    public class ApplicationModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            #region mediator

            // Mediator itself
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

            // finally register our custom code (individually, or via assembly scanning)
            // - requests & handlers as transient, i.e. InstancePerDependency()
            // - pre/post-processors as scoped/per-request, i.e. InstancePerLifetimeScope()
            // - behaviors as transient, i.e. InstancePerDependency()
            //builder.RegisterAssemblyTypes(typeof(ApplicationModule).Assembly).AsImplementedInterfaces(); // via assembly scan
            //builder.RegisterAssemblyTypes(typeof(AccountingModule).Assembly).AsImplementedInterfaces();

            // command and event handlers
            builder.RegisterType<ActionLogHandler>().AsImplementedInterfaces().InstancePerDependency();
            builder.RegisterType<AdvertiseCommandHandler>().AsImplementedInterfaces().InstancePerDependency();
            builder.RegisterType<CategoryCommandHandler>().AsImplementedInterfaces().InstancePerDependency();
            builder.RegisterType<OccupiedTableCommandHandler>().AsImplementedInterfaces().InstancePerDependency();
            builder.RegisterType<AdvertiseEventHandler>().AsImplementedInterfaces().InstancePerDependency();
            builder.RegisterType<CategoryEventHandler>().AsImplementedInterfaces().InstancePerDependency();
            builder.RegisterType<RegionEventHandler>().AsImplementedInterfaces().InstancePerDependency();
            builder.RegisterType<FileCommandHandler>().AsImplementedInterfaces().InstancePerDependency();
            builder.RegisterType<ServiceHandler>().AsImplementedInterfaces().InstancePerDependency();
            builder.RegisterType<ChatCommandHandler>().AsImplementedInterfaces().InstancePerDependency();
            builder.RegisterType<ExtrinsicReserveCommandHandler>().AsImplementedInterfaces().InstancePerDependency();
            builder.RegisterType<InstantReserveAutoCancelCommandHandler>().AsImplementedInterfaces().InstancePerDependency();
            builder.RegisterType<ReserveAutoCancelCommandHandler>().AsImplementedInterfaces().InstancePerDependency();
            builder.RegisterType<ReserveCommandHandler>().AsImplementedInterfaces().InstancePerDependency();
            builder.RegisterType<ReserveSendSmsCommandHandler>().AsImplementedInterfaces().InstancePerDependency();
            builder.RegisterType<ReserveSupportCommandHandler>().AsImplementedInterfaces().InstancePerDependency();
            builder.RegisterType<InstantReserveAutoCancelEventHandler>().AsImplementedInterfaces().InstancePerDependency();
            builder.RegisterType<ReserveAutoCancelEventHandler>().AsImplementedInterfaces().InstancePerDependency();
            builder.RegisterType<ReserveEventHandler>().AsImplementedInterfaces().InstancePerDependency();
            builder.RegisterType<SupportChatCommandHandler>().AsImplementedInterfaces().InstancePerDependency();
            builder.RegisterType<InsertMessageEventHandler>().AsImplementedInterfaces().InstancePerDependency();
            builder.RegisterType<UserCommandHandler>().AsImplementedInterfaces().InstancePerDependency();
            builder.RegisterType<UserEventHandler>().AsImplementedInterfaces().InstancePerDependency();
            #endregion

            //data module
            builder.RegisterModule<DataModule>();

            #region UserContact
            builder.RegisterType<SmsContact>().As<ISmsContact>();
            builder.RegisterType<NotificationContact>().As<INotificationContact>();
            builder.RegisterType<AppNotificationContact>().As<IAppNotificationContact>();
            builder.RegisterType<EmailContact>().As<IEmailContact>();
            builder.RegisterType<UserContactFacade>().As<IUserContactFacade>();
            #endregion

            //accounting module
            builder.RegisterModule<AccountingModule>();

            #region reserve state
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
            #endregion

            #region app services
            builder.RegisterType<BlogPostAppService>()
                .As<IBlogPostAppService>();
            builder.RegisterType<ServiceAppService>()
                .As<IServiceAppService>();
            builder.RegisterType<PostAppService>()
                .As<IPostAppService>();
            builder.RegisterType<BankCardAppService>()
                .As<IBankCardAppService>();
            builder.RegisterType<SupportChatAppService>()
                .As<ISupportChatAppService>();
            builder.RegisterType<SupportChatMessageAppService>()
                .As<ISupportChatMessageAppService>();
            builder.RegisterType<ReportItemAppService>()
                .As<IReportItemAppService>();
            builder.RegisterType<CommentAppService>()
                .As<ICommentAppService>();
            builder.RegisterType<ReserveSupportAppService>()
                .As<IReserveSupportAppService>();
            builder.RegisterType<UserAppService>()
                .As<IUserAppService>();
            builder.RegisterType<UserFavoriteAppService>()
                .As<IUserFavoriteAppService>();
            builder.RegisterType<RegionAppService>()
                .As<IRegionAppService>();
            builder.RegisterType<DiscountTableAppService>()
                .As<IDiscountTableAppService>();
            builder.RegisterType<PriceTableAppService>()
                .As<IPriceTableAppService>();
            builder.RegisterType<OccupiedTableAppService>()
                .As<IOccupiedTableAppService>();
            builder.RegisterType<AdvertiseReportAppService>()
                .As<IAdvertiseReportAppService>();
            builder.RegisterType<AdvertiseAppService>()
                .As<IAdvertiseAppService>();
            builder.RegisterType<FileAppService>()
                .As<IFileAppService>();
            builder.RegisterType<ActionLogAppService>()
                .As<IActionLogAppService>();
            builder.RegisterType<CategoryAppService>()
                .As<ICategoryAppService>();
            builder.RegisterType<ReserveAppService>()
                .As<IReserveAppService>();
            builder.RegisterType<ExtrinsicReserveAppService>()
                .As<IExtrinsicReserveAppService>();
            builder.RegisterType<ChatAppService>()
                .As<IChatAppService>();
            builder.RegisterType<ReserveAutoCancelAppService>()
                .As<IReserveAutoCancelAppService>();
            builder.RegisterType<InstantReserveAutoCancelAppService>()
                .As<IInstantReserveAutoCancelAppService>();
            builder.RegisterType<ReserveSendSmsAppService>()
                .As<IReserveSendSmsAppService>();
            builder.RegisterType<SettingAppService>()
                .As<ISettingAppService>();
            #endregion

            #region Background Services
            builder.RegisterType<BackgroundStartup>();
            #endregion

            #region Reserve Support Manager
            builder.RegisterType<ReserveSupportManager>()
                .As<IReserveSupportManager>();
            #endregion

            #region Setting Manager
            builder.RegisterType<SettingManager>()
                .As<ISettingManager>();
            #endregion

            #region Filter Helpers
            builder.RegisterType(typeof(AdvertiseFilterHelper))
                .As(typeof(IAdvertiseFilterHelper));
            #endregion

            base.Load(builder);
        }
    }
}
