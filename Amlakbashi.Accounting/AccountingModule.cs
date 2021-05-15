
using Amlakbashi.Accounting.PaymentContext;
using Amlakbashi.Accounting.PaymentContext.PaymentEngines;
using Amlakbashi.Accounting.PaymentContext.PaymentEngines.Interfaces;
using Amlakbashi.Accounting.Services;
using Amlakbashi.Accounting.Services.Interfaces;
using Amlakbashi.Accounting.BankingContext.Extensions;
using Amlakbashi.Accounting.PaymentContext.Extensions;
using Amlakbashi.Accounting.Configurations;
using Autofac;

namespace Amlakbashi.Accounting
{
    public class AccountingModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterPaymentContext();
            builder.RegisterBankingContext();
            builder.RegisterAccountingAppServices();
            builder.RegisterType<AccountingFacade>().As<IAccountingFacade>();

            base.Load(builder);
        }
    }
}
