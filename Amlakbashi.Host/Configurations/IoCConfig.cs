using Amlakbashi.Application.Services;
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

            builder.Register(c => LogManager.GetLogger("Default"));
        }
    }
}
