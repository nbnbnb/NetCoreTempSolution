using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace ConsoleAppCore.Demos.Locker
{
    internal sealed class SimpleWaitLock : IDisposable
    {
        private readonly AutoResetEvent m_avaliable;

        public SimpleWaitLock()
        {
            // 最开始可自由使用
            m_avaliable = new AutoResetEvent(true);
        }

        public void Enter()
        {
            m_avaliable.WaitOne();
        }

        public void Leave()
        {
            m_avaliable.Set();
        }

        public void Dispose()
        {
            m_avaliable.Dispose();
        }
    }
}
