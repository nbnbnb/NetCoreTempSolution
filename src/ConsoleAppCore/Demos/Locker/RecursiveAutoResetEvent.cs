using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace ConsoleAppCore.Demos.Locker
{
    /// <summary>
    /// 虽然 RecursiveAutoResetEvent 类的行为和 Mutex 类完全一样
    /// 但是在一个线程视图递归地获取锁时，它的性能会好很多
    /// 因为现在跟踪线程所有权和递归都是托管代码
    /// 只有在第一次获取 AutoResetEvent，或者最后把它放弃给其他线程时，线程才需要从托管代码转换为内核代码
    /// </summary>
    internal sealed class RecursiveAutoResetEvent : IDisposable
    {
        private AutoResetEvent m_lock = new AutoResetEvent(true);
        private Int32 m_owningThreadId = 0;
        private Int32 m_recursionCount = 0;

        public void Enter()
        {
            // 获取调用线程的唯一 Int32 ID
            Int32 currentThreadId = Thread.CurrentThread.ManagedThreadId;

            // 如果调用线程拥有锁，就递增递归计数
            if (m_owningThreadId == currentThreadId)
            {
                m_recursionCount++;
                return;
            }

            // 调用线程不拥有锁，等待它
            m_lock.WaitOne();

            // 调用线程现在拥有了锁，初始化拥有线程的 ID 和递归计数
            m_owningThreadId = currentThreadId;
            m_recursionCount = 1;
        }

        public void Leave()
        {
            if (m_owningThreadId != Thread.CurrentThread.ManagedThreadId)
            {
                throw new InvalidOperationException();
            }

            if (--m_recursionCount == 0)
            {
                // 如果递归计数为 0，表明没有线程拥有锁
                m_owningThreadId = 0;

                // 唤醒一个正在等待的线程（如果有的话）
                m_lock.Set();
            }
        }

        public void Dispose()
        {
            m_lock.Dispose();
        }
    }
}
