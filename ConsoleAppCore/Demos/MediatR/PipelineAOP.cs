using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace ConsoleAppCore.Demos.MediatR
{
    public class PipelineAOP<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    {
        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            Console.WriteLine("-- PipelineAOP Begin");

            TResponse response = await next();

            Console.WriteLine($"Response: {response}");

            Console.WriteLine("-- PipelineAOP  End ");
            return response;
        }
    }
}
