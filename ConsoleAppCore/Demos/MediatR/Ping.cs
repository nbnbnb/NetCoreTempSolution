using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleAppCore.Demos.MediatR
{
    /// <summary>
    /// 有返回值的 Request
    /// </summary>
    public class Ping : IRequest<string>
    {
        public int MsgId { get; set; }
    }
}
