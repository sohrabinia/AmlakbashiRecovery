using Amlakbashi.Accounting.PaymentContext.BankEngines;
using Amlakbashi.Accounting.PaymentContext.BankEngines.Interfaces;
using Autofac;

namespace Amlakbashi.Accounting.PaymentContext.Extensions
{
    internal static class AutofacRegisterPaymentContext
    {
        internal static void RegisterPaymentContext(this ContainerBuilder builder)
        {
            builder.RegisterType<PasargadEngine>()
                .As<IPasargadEngine>();

            builder.RegisterType<SamanEngine>()
                .As<ISamanEngine>();

            builder.RegisterType<PaymentOperator>()
                .As<IPaymentOperator>();
        }
    }
}
