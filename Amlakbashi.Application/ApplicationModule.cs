using Amlakbashi.Accounting;
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
using Amlakbashi.Application.Services.ReserveServices.ReserveState;
using Amlakbashi.Application.Services.ReserveServices.ReserveState.Interfaces;
using Amlakbashi.Application.Services.ReserveServices.ReserveState.ReserveStates;
using static Amlakbashi.Core.Entities.Reserve;
using Amlakbashi.Application.Services.SupportChatServices;
using Amlakbashi.Application.Services.SupportChatServices.Interfaces;
using Amlakbashi.Application.Services.UserServices;
using Amlakbashi.Application.Services.UserServices.Interfaces;
using Amlakbashi.Core.Infrastructure.UserContact;
using Amlakbashi.Core.Infrastructure.UserContact.Interfaces;
using Amlakbashi.Data;
using Autofac;
using MediatR;
using Amlakbashi.Application.Services.ReserveServices.ReserveSupportManager;
using Amlakbashi.Application.Services.SettingServices;
using Amlakbashi.Application.Services.SettingServices.Interfaces;
using Amlakbashi.Application.Services.SettingServices.SettingManager;
using Amlakbashi.Core.Infrastructure.FilterHelpers;
using Amlakbashi.Core.Infrastructure.FilterHelpers.Interfaces;
using Amlakbashi.Application.Services.AdvertiseServices.CommandHandlers;
using Amlakbashi.Application.Services.ActionLogServices.EventHandler;
using Amlakbashi.Application.Services.AdvertiseServices.EventHandlers;
using Amlakbashi.Application.Services.FileServices.CommandHandlers;
using Amlakbashi.Application.Services.PostServices.EventHandlers;
using Amlakbashi.Application.Services.ReserveServices.CommandHandlers;
using Amlakbashi.Application.Services.ReserveServices.EventHandlers;
using Amlakbashi.Application.Services.SupportChatServices.CommandHandlers;
using Amlakbashi.Application.Services.SupportChatServices.EventHandlers;
using Amlakbashi.Application.Services.UserServices.CommadHandler;
using Amlakbashi.Application.Services.UserServices.EventHandlers;
using Amlakbashi.Accounting.Services.CommandHandlers;
using Amlakbashi.Application.Configurations;

namespace Amlakbashi.Application
{
    public class ApplicationModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterMediatorHandlers();
            builder.RegisterModule<DataModule>();
            builder.RegisterUserContact();
            builder.RegisterModule<AccountingModule>();
            builder.RegisterReserveState();
            builder.RegisterAppServices();
            builder.RegisterType<BackgroundStartup>();
            builder.RegisterType<ReserveSupportManager>().As<IReserveSupportManager>();
            builder.RegisterType<SettingManager>().As<ISettingManager>();
            builder.RegisterType(typeof(AdvertiseFilterHelper)).As(typeof(IAdvertiseFilterHelper));

            base.Load(builder);
        }
    }
}
