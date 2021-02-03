using Amlakbashi.Core.Common.Repository;
using Amlakbashi.Data.Repositories;
using Autofac;
using Autofac.Core.Lifetime;
using Hangfire;

namespace Amlakbashi.Data
{
    public class DataModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<AmlakbashiDB>()
                .As<IDbContext>()
                .InstancePerBackgroundJob(MatchingScopeLifetimeTags.RequestLifetimeScopeTag);
            builder.RegisterGeneric(typeof(GenericRepository<,>))
                .As(typeof(IRepository<,>))
                .InstancePerLifetimeScope();
            builder.RegisterType<AccountingRepository>()
                .As<IAccountingRepository>()
                .InstancePerLifetimeScope();
            base.Load(builder);
        }
    }
}
