using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleAppCore.Demos.MediatR
{
    /// <summary>
    /// 异步
    /// 有返回值
    /// </summary>
    public class ASyncPingHandler : IRequestHandler<Ping, string>
    {
        public Task<string> Handle(Ping request, CancellationToken cancellationToken)
        {
            Console.WriteLine("Ping Async Pong");

            return Task.FromResult("Result to -> Ping Async Pong");
        }
    }
}
