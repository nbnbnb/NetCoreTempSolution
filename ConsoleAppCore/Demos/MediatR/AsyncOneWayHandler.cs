using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleAppCore.Demos.MediatR
{
    public class AsyncOneWayHandler : AsyncRequestHandler<OneWay>
    {
        protected override Task Handle(OneWay request, CancellationToken cancellationToken)
        {
            Console.WriteLine("OneWay Async Handle");

            // Twiddle thumbs
            return Task.CompletedTask;
        }
    }
}
