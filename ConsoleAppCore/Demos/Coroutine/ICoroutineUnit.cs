using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleAppCore.Demos.Coroutine
{
    /// <summary>
    /// 协程单元
    /// </summary>
    interface ICoroutineUnit
    {
        IEnumerator<Task> Do();
    }
}
