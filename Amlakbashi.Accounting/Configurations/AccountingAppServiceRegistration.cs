using Amlakbashi.Accounting.Services;
using Amlakbashi.Accounting.Services.Interfaces;
using Autofac;

namespace Amlakbashi.Accounting.Configurations
{
    internal static class AccountingAppServiceRegistration
    {
        internal static void RegisterAccountingAppServices(this ContainerBuilder builder)
        {
            builder.RegisterType<ReservePaymentAppService>()
                .As<IReservePaymentAppService>();
            builder.RegisterType<ReservePaymentAppService>()
              .As<IReservePaymentAppService>();
            builder.RegisterType<DiscountCouponAppService>()
                .As<IDiscountCouponAppService>();
            builder.RegisterType<CreditTransactionAppService>()
                .As<ICreditTransactionAppService>();
            builder.RegisterType<PrizeCreditTransactionAppService>()
                .As<IPrizeCreditTransactionAppService>();
            builder.RegisterType<CartAppService>()
                .As<ICartAppService>();
            builder.RegisterType<PaymentAppService>()
                .As<IPaymentAppService>();
            builder.RegisterType<GroupPaymentAppService>()
                .As<IGroupPaymentAppService>();
        }
    }
}
