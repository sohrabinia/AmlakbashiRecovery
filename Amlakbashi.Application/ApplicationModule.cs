using Amlakbashi.Accounting;
using Amlakbashi.Data;
using Autofac;
using Amlakbashi.Application.Services.ReserveServices.ReserveSupportManager;
using Amlakbashi.Application.Services.SettingServices.SettingManager;
using Amlakbashi.Core.Infrastructure.FilterHelpers;
using Amlakbashi.Core.Infrastructure.FilterHelpers.Interfaces;
using Amlakbashi.Application.Configurations;
using Amlakbashi.Core;

namespace Amlakbashi.Application
{
    public class ApplicationModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterMediatorHandlers();
            builder.RegisterModule<CoreModule>();
            builder.RegisterModule<DataModule>();
            builder.RegisterUserContact();
            builder.RegisterModule<AccountingModule>();
            builder.RegisterReserveState();
            builder.RegisterAppServices();
            builder.RegisterType<BackgroundStartup>();
            builder.RegisterType<ReserveSupportManager>().As<IReserveSupportManager>();
            builder.RegisterType<SettingManager>().As<ISettingManager>();
            builder.RegisterType(typeof(AdvertiseFilterHelper)).As(typeof(IAdvertiseFilterHelper));

            base.Load(builder);
        }
    }
}
