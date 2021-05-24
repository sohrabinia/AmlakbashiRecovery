using Amlakbashi.Core.Common.StaticData;
using Amlakbashi.Mediator.Commands.UserCommands;
using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Amlakbashi.Host.Handlers
{
    public class HostCommandHandler : IRequestHandler<DeleteImpersonationCookiesCommand>
    {
        private readonly IHttpContextAccessor httpContextAccessor;
        public HostCommandHandler(IHttpContextAccessor httpContextAccessor)
        {
            this.httpContextAccessor = httpContextAccessor;
        }

        public Task<Unit> Handle(DeleteImpersonationCookiesCommand request, CancellationToken cancellationToken)
        {
            httpContextAccessor.HttpContext.Response.Cookies.Delete(ImpersonateData.ImpersonateCookieName);
            return Task.FromResult(Unit.Value);
        }
    }
}
