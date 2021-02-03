
using Amlakbashi.Accounting.PaymentContext;
using Amlakbashi.Accounting.PaymentContext.PaymentEngines;
using Amlakbashi.Accounting.PaymentContext.PaymentEngines.Interfaces;
using Amlakbashi.Accounting.Services;
using Amlakbashi.Accounting.Services.Interfaces;
using Autofac;

namespace Amlakbashi.Accounting
{
    public class AccountingModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            #region payment engines
            builder.RegisterType<PasargadPaymentEngine>()
                .As<IPasargadPaymentEngine>();
            #endregion

            #region payment operator
            builder.RegisterType<PaymentOperator>()
                .As<IPaymentOperator>();
            #endregion

            #region app services
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
            #endregion

            builder.RegisterType<AccountingFacade>()
                .As<IAccountingFacade>();

            base.Load(builder);
        }
    }
}
