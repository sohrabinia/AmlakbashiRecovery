using Amlakbashi.Core.Common.Background;
using Autofac;
using Hangfire;
using Hangfire.Common;
using log4net;
using MediatR;

namespace Amlakbashi.Core.Common.Extensions
{
    public static class HangfireExtensions
    {
        public static IGlobalConfiguration UseMediatR(this IGlobalConfiguration config, IContainer container)
        {
            config.UseActivator(new MediatorJobActivator(container));
            GlobalConfiguration.Configuration.UseSerializerSettings(new Newtonsoft.Json.JsonSerializerSettings
            {
                TypeNameHandling = Newtonsoft.Json.TypeNameHandling.Objects
            });
            return config;
        }
    }
}
