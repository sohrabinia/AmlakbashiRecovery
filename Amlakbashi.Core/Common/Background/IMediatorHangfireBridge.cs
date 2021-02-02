using log4net;
using MediatR;
using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Amlakbashi.Core.Common.Background
{
    public interface IMediatorHangfireBridge
    {
        Task Send(IRequest request);
    }
}
