using Amlakbashi.Accounting.PaymentContext.PaymentEngines;
using Amlakbashi.Accounting.PaymentContext.PaymentEngines.Interfaces;
using Autofac;

namespace Amlakbashi.Accounting.PaymentContext.Extensions
{
    internal static class AutofacRegisterPaymentContext
    {
        internal static void RegisterPaymentContext(this ContainerBuilder builder)
        {
            builder.RegisterType<PasargadPaymentEngine>()
                .As<IPasargadPaymentEngine>();

            builder.RegisterType<PaymentOperator>()
                .As<IPaymentOperator>();
        }
    }
}
