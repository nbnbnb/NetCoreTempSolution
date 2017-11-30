using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace ConsoleAppCore.Demos.Locker
{
    internal sealed class AnotherHybridLock : IDisposable
    {
        // Int32 由基元用户模式构造（Interlocked 的方法）使用
        private Int32 m_waiters = 0;

        // AutoResetEvent 是基元内核模式构造
        private AutoResetEvent m_waiterLock = new AutoResetEvent(false);

        // 这个字段控制自旋，希望能提升性能
        private Int32 m_spincount = 4000;  // 随便选择一个计数

        // 这些字段指出哪个线程拥有锁，以及拥有了它多少次
        private Int32 m_owningThreadId = 0, m_recursion = 0;

        public void Enter()
        {
            Int32 threadId = Thread.CurrentThread.ManagedThreadId;

            //  如果调用线程已经拥有锁，递增递归计数并返回
            if (threadId == m_owningThreadId)
            {
                m_recursion++;
                return;
            }

            // 调用线程不拥有锁，尝试获取它
            SpinWait spinWait = new SpinWait();
            for (Int32 spinCount = 0; spinCount < m_spincount; spinCount++)
            {
                // 如果锁可以自用使用了，这个线程就获得它
                // 设置一些状态并返回
                if (Interlocked.CompareExchange(ref m_waiters, 1, 0) == 0)
                {
                    goto GotLock;
                }

                // 黑科技：给其他线程运行的机会，希望锁会被释放
                spinWait.SpinOnce();
            }

            // 自旋结束，锁仍未获得，再试一次
            if (Interlocked.Increment(ref m_waiters) > 1)
            {
                // 仍然是竞态条件，这个线程必须阻塞
                m_waiterLock.WaitOne();  // 等待锁，性能有损失

                // 等这个线程醒来时，它拥有锁
                // 执行到下面的 GotLock 标签（设置一些状态，并返回）
            }

            GotLock: // 设置一些状态，并返回
            // 一个线程获得锁时，我们记录它的 ID
            // 并指出线程拥有锁一次
            m_owningThreadId = threadId;
            m_recursion = 1;
        }

        public void Leave()
        {
            // 如果调用线程不拥有锁，表明存在 Bug
            Int32 threadId = Thread.CurrentThread.ManagedThreadId;
            if (threadId != m_owningThreadId)
            {
                throw new SynchronizationLockException("Lock not owned by calling thread");
            }

            // 递减递归计数
            // 如果这个线程仍然拥有锁，那么直接返回
            if (--m_recursion > 0)
            {
                return;
            }

            m_owningThreadId = 0; // 现在没有线程拥有锁

            // 如果没有其他线程在等待，直接返回
            if (Interlocked.Decrement(ref m_waiters) == 0)
            {
                return;
            }

            // 有其他线程正在等待，唤醒其中一个
            m_waiterLock.Set(); // 这里有较大的性能损失
        }

        public void Dispose()
        {
            m_waiterLock.Dispose();
        }
    }
}
