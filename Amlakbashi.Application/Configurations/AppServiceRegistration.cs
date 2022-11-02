using Amlakbashi.Application.Services.ActionLogServices;
using Amlakbashi.Application.Services.ActionLogServices.Interfaces;
using Amlakbashi.Application.Services.AdvertiseServices;
using Amlakbashi.Application.Services.AdvertiseServices.Interfaces;
using Amlakbashi.Application.Services.BlogPostServices;
using Amlakbashi.Application.Services.BlogPostServices.Interfaces;
using Amlakbashi.Application.Services.Category;
using Amlakbashi.Application.Services.Category.Interfaces;
using Amlakbashi.Application.Services.CommentServices;
using Amlakbashi.Application.Services.CommentServices.Interfaces;
using Amlakbashi.Application.Services.FileServices;
using Amlakbashi.Application.Services.FileServices.Interfaces;
using Amlakbashi.Application.Services.PostServices;
using Amlakbashi.Application.Services.PostServices.Interfaces;
using Amlakbashi.Application.Services.ReserveServices;
using Amlakbashi.Application.Services.ReserveServices.Interfaces;
using Amlakbashi.Application.Services.SettingServices;
using Amlakbashi.Application.Services.SettingServices.Interfaces;
using Amlakbashi.Application.Services.SupportChatServices;
using Amlakbashi.Application.Services.SupportChatServices.Interfaces;
using Amlakbashi.Application.Services.UserServices;
using Amlakbashi.Application.Services.UserServices.Interfaces;
using Autofac;

namespace Amlakbashi.Application.Configurations
{
    internal static class AppServiceRegistration
    {
        internal static void RegisterAppServices(this ContainerBuilder builder)
        {
            builder.RegisterType<BlogPostAppService>()
                .As<IBlogPostAppService>();
            builder.RegisterType<ServiceAppService>()
                .As<IServiceAppService>();
            builder.RegisterType<PostAppService>()
                .As<IPostAppService>();
            builder.RegisterType<BankCardAppService>()
                .As<IBankCardAppService>();
            builder.RegisterType<SupportChatAppService>()
                .As<ISupportChatAppService>();
            builder.RegisterType<SupportChatMessageAppService>()
                .As<ISupportChatMessageAppService>();
            builder.RegisterType<ReportItemAppService>()
                .As<IReportItemAppService>();
            builder.RegisterType<CommentAppService>()
                .As<ICommentAppService>();
            builder.RegisterType<ReserveSupportAppService>()
                .As<IReserveSupportAppService>();
            builder.RegisterType<UserAppService>()
                .As<IUserAppService>();
            builder.RegisterType<RegionAppService>()
                .As<IRegionAppService>();
            builder.RegisterType<DiscountTableAppService>()
                .As<IDiscountTableAppService>();
            builder.RegisterType<PriceTableAppService>()
                .As<IPriceTableAppService>();
            builder.RegisterType<AdvertiseReportAppService>()
                .As<IAdvertiseReportAppService>();
            builder.RegisterType<AdvertiseAppService>()
                .As<IAdvertiseAppService>();
            builder.RegisterType<FileAppService>()
                .As<IFileAppService>();
            builder.RegisterType<ActionLogAppService>()
                .As<IActionLogAppService>();
            builder.RegisterType<CategoryAppService>()
                .As<ICategoryAppService>();
            builder.RegisterType<ReserveAppService>()
                .As<IReserveAppService>();
            builder.RegisterType<ExtrinsicReserveAppService>()
                .As<IExtrinsicReserveAppService>();
            builder.RegisterType<ChatAppService>()
                .As<IChatAppService>();
            builder.RegisterType<ReserveAutoCancelAppService>()
                .As<IReserveAutoCancelAppService>();
            builder.RegisterType<ReserveSendSmsAppService>()
                .As<IReserveSendSmsAppService>();
            builder.RegisterType<SettingAppService>()
                .As<ISettingAppService>();
            builder.RegisterType<TagAppService>()
                .As<ITagAppService>();
        }
    }
}
