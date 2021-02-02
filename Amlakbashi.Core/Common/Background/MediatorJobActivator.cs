using Autofac;
using Hangfire;
using log4net;
using MediatR;
using System;

namespace Amlakbashi.Core.Common.Background
{
    public class MediatorJobActivator : JobActivator
    {
        private readonly IContainer container;
        public MediatorJobActivator(IContainer container)
        {
            this.container = container;
        }

        public override object ActivateJob(Type jobType)
        {
            return container.Resolve(jobType);
        }
    }
}
