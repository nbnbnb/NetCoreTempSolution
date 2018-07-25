using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleAppCore.Demos.MediatR
{
    /// <summary>
    /// 同步，有返回值的 Handler
    /// </summary>
    public class SyncPongHandler : RequestHandler<Pong, string>
    {
        protected override string Handle(Pong request)
        {
            return "Pong Sync Pong";
        }
    }
}
