using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace ConsoleAppCore.Demos.Locker
{
    internal sealed class SimpleHybridLock : IDisposable
    {
        // Int32 由基元用户模式构造（Interlocked 的方法）使用
        private Int32 m_waiters = 0;

        // AutoResetEvent 是基元内核模式构造
        private AutoResetEvent m_waiterLock = new AutoResetEvent(false);

        public void Enter()
        {
            if (Interlocked.Increment(ref m_waiters) == 1)
            {
                return; // 锁可自由使用，无竞争，直接返回
            }

            // 另一个线程拥有锁（发生竞争），使这个线程等待
            m_waiterLock.WaitOne();  // 这里产生较大的性能影响

            // WaitOne 返回后，这个线程拿到锁了
        }

        public void Leave()
        {
            if (Interlocked.Decrement(ref m_waiters) <= 0)
            {
                // 对于多次调用 Leave 的场景
                // 此处直接返回，并将计数器设置为 0e
                Volatile.Write(ref m_waiters, 0);
                return; //  没有其他线程正在等待，直接返回
            }

            // 有其他线程正在等待，唤醒其中一个
            m_waiterLock.Set();  // 这里产生较大的性能影响
        }

        public void Dispose()
        {
            m_waiterLock.Dispose();
        }
    }
}
