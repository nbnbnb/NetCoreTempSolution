using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleAppCore.Demos.Coroutine
{
    /// <summary>
    /// 协程容器
    /// </summary>
    interface ICoroutineContainer
    {
        void Register(ICoroutineUnit unit);

        void Run();
    }
}
