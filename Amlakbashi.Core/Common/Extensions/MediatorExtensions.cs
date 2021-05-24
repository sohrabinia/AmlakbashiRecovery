using Amlakbashi.Core.Common.Background;
using Hangfire;
using MediatR;
using System;
using System.Linq.Expressions;

namespace Amlakbashi.Core.Common.Extensions
{
    public static class MediatorExtensions
    {
        public static void Enqueue(this IMediator mediator, IRequest request)
        {
            BackgroundJob.Enqueue<IMediatorHangfireBridge>(bridge => bridge.Send(request));
        }

        public static void Schedule(this IMediator mediator, IRequest request, TimeSpan delay)
        {
            BackgroundJob.Schedule<IMediatorHangfireBridge>(bridge => bridge.Send(request), delay);
        }

        public static void Schedule<T>(this IMediator mediator, IRequest<T> request, TimeSpan delay)
        {
            BackgroundJob.Schedule<IMediatorHangfireBridge>(bridge => bridge.Send(request), delay);
        }

        public static void AddOrUpdate(this IMediator mediator, string jobId, IRequest request, string time)
        {
            RecurringJob.AddOrUpdate<IMediatorHangfireBridge>(jobId, bridge => bridge.Send(request), time, TimeZoneInfo.Local);
        }
    }
}
