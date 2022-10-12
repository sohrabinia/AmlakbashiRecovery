using System;
using System.Collections.Generic;
using System.Text;
using Amlakbashi.Core.Services;
using Amlakbashi.Core.Services.Interfaces;
using Autofac;

namespace Amlakbashi.Core
{
    public class CoreModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<EmailSenderService>()
                .As<IEmailSenderService>();
            base.Load(builder);
        }
    }
}
