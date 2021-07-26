using Amlakbashi.Application;
using Amlakbashi.Core.Common.Background;
using Amlakbashi.Core.Common.Caching;
using Amlakbashi.Core.Common.Localization;
using Amlakbashi.Core.Common.Mapping;
using Amlakbashi.Core.Identity.Entities;
using Amlakbashi.Core.Infrastructure.PriceHelpers;
using Amlakbashi.Core.Infrastructure.PriceHelpers.Interfaces;
using Amlakbashi.Host.Authentication;
using Amlakbashi.Host.Handlers;
using Amlakbashi.Host.Hubs.Admin.HubServers;
using Amlakbashi.Host.Hubs.Dashboard.HubServers;
using Amlakbashi.Host.Hubs.HubEventHandlers;
using Amlakbashi.Host.Hubs.Portal.HubServers;
using Autofac;
using log4net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
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
            // log4net
            builder.Register(c => LogManager.GetLogger("Default"));

            builder.RegisterType(typeof(CacheManager)).
                As(typeof(ICacheManager));
            builder.RegisterType(typeof(Localization)).
                As(typeof(ILocalization));
            builder.RegisterType(typeof(PriceCalculator)).
                As(typeof(IPriceCalculator));
            //builder.RegisterModule<LoggingModule>();
            builder.RegisterModule<ApplicationModule>();
            //builder.RegisterAssemblyTypes(typeof(MvcApplication).Assembly)
            //    .AsImplementedInterfaces();
            builder.RegisterModule(new AutoMapperModule(
                typeof(AutoMapperModule).Assembly));
            builder.RegisterType<MediatorHangfireBridge>()
                .As<IMediatorHangfireBridge>();

            builder.RegisterType<HostCommandHandler>().AsImplementedInterfaces().InstancePerDependency();
            builder.RegisterType<ReserveHubEventHandlers>().AsImplementedInterfaces().InstancePerDependency();

            // hub servers registration
            builder.RegisterType<ReserveAdminHubServer>().As<IReserveAdminHubServer>();
            builder.RegisterType<SupportChatAdminHubServer>().As<ISupportChatAdminHubServer>();
            builder.RegisterType<ReserveDashboardHubServer>().As<IReserveDashboardHubServer>();
            builder.RegisterType<PortalHubServer>().As<IPortalHubServer>();

            //http context accessor
            builder.RegisterType<HttpContextAccessor>()
                .As<IHttpContextAccessor>()
                .SingleInstance();

            //user accessor
            builder.RegisterType<UserAccessor>()
                .As<IUserAccessor>();
            builder.RegisterType<CustomPasswordValidator>()
                .As<IPasswordValidator<AppUser>>();
        }
    }
}
