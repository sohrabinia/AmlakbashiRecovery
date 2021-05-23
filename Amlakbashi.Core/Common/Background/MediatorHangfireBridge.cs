using log4net;
using MediatR;
using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Amlakbashi.Core.Common.Background
{
    public class MediatorHangfireBridge : IMediatorHangfireBridge
    {
        private readonly IMediator mediator;
        public MediatorHangfireBridge(IMediator mediator)
        {
            this.mediator = mediator;
        }

        public async Task Send(IRequest request)
        {
            await mediator.Send(request);
        }

        public async Task Send<T>(IRequest<T> request)
        {
            await mediator.Send(request);
        }
    }
}
