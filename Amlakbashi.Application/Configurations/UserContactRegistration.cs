using Amlakbashi.Core.Infrastructure.UserContact;
using Amlakbashi.Core.Infrastructure.UserContact.Interfaces;
using Autofac;

namespace Amlakbashi.Application.Configurations
{
    internal static class UserContactRegistration
    {
        internal static void RegisterUserContact(this ContainerBuilder builder)
        {
            builder.RegisterType<SmsContact>().As<ISmsContact>();
            builder.RegisterType<NotificationContact>().As<INotificationContact>();
            builder.RegisterType<AppNotificationContact>().As<IAppNotificationContact>();
            builder.RegisterType<EmailContact>().As<IEmailContact>();
            builder.RegisterType<UserContactFacade>().As<IUserContactFacade>();
        }
    }
}
