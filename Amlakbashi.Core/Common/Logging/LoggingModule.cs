//using Autofac.Core;
//using Autofac.Core.Registration;
//using Autofac.Core.Resolving.Pipeline;
//using log4net;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Reflection;
//using System.Text;
//using System.Threading.Tasks;

//namespace Amlakbashi.Core.Common.Logging
//{
//    public class LoggingModule : Autofac.Module
//    {
//        private readonly IResolveMiddleware middleware;

//        public LoggingModule(IResolveMiddleware middleware)
//        {
//            this.middleware = middleware;
//        }

//        protected override void AttachToComponentRegistration(IComponentRegistryBuilder componentRegistryBuilder, IComponentRegistration registration)
//        {
//            // Attach to the registration's pipeline build.
//            registration.PipelineBuilding += (sender, pipeline) =>
//            {
//                // Add our middleware to the pipeline.
//                pipeline.Use(middleware);
//            };
//        }
//    }
//}
