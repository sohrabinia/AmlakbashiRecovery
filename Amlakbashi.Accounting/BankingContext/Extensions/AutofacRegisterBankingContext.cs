using Amlakbashi.Accounting.BankingContext.BankingEngines;
using Amlakbashi.Accounting.BankingContext.BankingEngines.Interfaces;
using Autofac;

namespace Amlakbashi.Accounting.BankingContext.Extensions
{
    internal static class AutofacRegisterBankingContext
    {
        internal static void RegisterBankingContext(this ContainerBuilder builder)
        {
            builder.RegisterType<PodiumBankingEngine>().As<IPodiumBankingEngine>();
            builder.RegisterType<BankingOperator>().As<IBankingOperator>();
        }
    }
}
