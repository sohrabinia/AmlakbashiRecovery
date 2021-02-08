using Amlakbashi.Application.Services;
using Amlakbashi.Host.Hubs.Admin;
using Amlakbashi.Host.Hubs.Admin.HubServers;
using Amlakbashi.Host.Hubs.Dashboard.HubServers;
using Amlakbashi.Host.Hubs.Portal.HubServers;
using Autofac;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Amlakbashi.Host.Configurations
{
    public class IoCConfig
    {
        public static void Config(ContainerBuilder builder)
        {
            //builder.RegisterGeneric(typeof(CacheManager<>)).
            //    As(typeof(ICacheManager<>));
            //builder.RegisterType(typeof(Localization)).
            //    As(typeof(ILocalization));
            //builder.RegisterType(typeof(PriceCalculator)).
            //    As(typeof(IPriceCalculator));
            //builder.RegisterModule<LoggingModule>();
            //builder.RegisterModule<ApplicationModule>();
            //builder.RegisterAssemblyTypes(typeof(MvcApplication).Assembly)
            //    .AsImplementedInterfaces();
            //builder.RegisterModule(new AutoMapperModule(
            //    typeof(AutoMapperModule).Assembly));
            //builder.RegisterType<MediatorHangfireBridge>()
            //    .As<IMediatorHangfireBridge>();

            //hub servers registration
            builder.RegisterType<ReserveAdminHubServer>().As<IReserveAdminHubServer>();
            builder.RegisterType<SupportChatAdminHubServer>().As<IReserveAdminHubServer>();
            builder.RegisterType<ReserveDashboardHubServer>().As<IReserveDashboardHubServer>();
            builder.RegisterType<PortalHubServer>().As<IPortalHubServer>();

            builder.Register(c => LogManager.GetLogger("Default"));
        }
    }
}
