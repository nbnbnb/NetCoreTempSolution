using MediatR;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleAppCore.Demos.MediatR
{
    public class Ping03NotificationHandler : INotificationHandler<PingNotification>
    {
        public Task Handle(PingNotification notification, CancellationToken cancellationToken)
        {
            Console.WriteLine("Ping03 Notificatioin");
            return Task.CompletedTask;
        }
    }
}
