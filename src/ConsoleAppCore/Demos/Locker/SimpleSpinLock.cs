using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace ConsoleAppCore.Demos.Locker
{
    internal sealed class SimpleSpinLock
    {
        private Int32 flag = 0;   // 默认可使用

        public void Enter()
        {
            while (true)
            {
                if (Interlocked.Exchange(ref flag, 1) == 0)
                {
                    return;
                }

                // 在这里添加黑科技
                Thread.Yield();
            }
        }

        public void Leave()
        {
            Volatile.Write(ref flag, 0);
        }
    }
}
