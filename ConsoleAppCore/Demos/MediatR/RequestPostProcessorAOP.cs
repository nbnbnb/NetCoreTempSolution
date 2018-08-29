using MediatR.Pipeline;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleAppCore.Demos.MediatR
{
    public class RequestPostProcessorAOP<TRequest, TResponse> : IRequestPostProcessor<TRequest, TResponse>
    {
        public Task Process(TRequest request, TResponse response)
        {
            Console.WriteLine("- PostProcessor AOP");
            return Task.CompletedTask;
        }
    }
}
