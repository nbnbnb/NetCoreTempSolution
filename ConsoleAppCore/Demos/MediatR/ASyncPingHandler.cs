using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleAppCore.Demos.MediatR
{
    /// <summary>
    /// 异步有返回值的 Handler
    /// </summary>
    public class ASyncPingHandler : IRequestHandler<Ping, string>
    {
        public Task<string> Handle(Ping request, CancellationToken cancellationToken)
        {
            return Task.FromResult("Ping Async Pong");
        }
    }
}
