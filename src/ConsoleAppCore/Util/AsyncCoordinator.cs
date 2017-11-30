using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace ConsoleAppCore.Util
{
    internal sealed class AsyncCoordinator
    {
        private Int32 m_opCount = 1;
        private Int32 m_statusReported = 0;
        private Action<CoordinationStatus> m_callback;
        private Timer m_timer;

        /// <summary>
        /// 该方法必须在发起一个操作之前调用s
        /// </summary>
        /// <param name="opsToAdd"></param>
        public void AboutToBegin(Int32 opsToAdd = 1)
        {
            Interlocked.Add(ref m_opCount, opsToAdd);
        }

        /// <summary>
        /// 该方法必须在处理好一个操作的结果之后调用
        /// </summary>
        public void JustEnded()
        {
            if (Interlocked.Decrement(ref m_opCount) == 0)
            {
                ReportStatus(CoordinationStatus.AllDone);
            }
        }

        /// <summary>
        /// 该方法必须在发起所有操作之后调用
        /// </summary>
        /// <param name="callback"></param>
        /// <param name="timeout"></param>
        public void AllBegun(Action<CoordinationStatus> callback, Int32 timeout = Timeout.Infinite)
        {
            m_callback = callback;

            if (timeout != Timeout.Infinite)
            {
                m_timer = new Timer(TimeExpired, null, timeout, Timeout.Infinite);
            }
            JustEnded();
        }

        private void TimeExpired(Object o)
        {
            ReportStatus(CoordinationStatus.Timeout);
        }

        public void Cancel()
        {
            ReportStatus(CoordinationStatus.Cancel);
        }

        private void ReportStatus(CoordinationStatus status)
        {
            // 如果状态从未报告过，就报告它
            // 否则忽略它
            if (Interlocked.Exchange(ref m_statusReported, 1) == 0)
            {
                m_callback(status);
            }
        }

        public enum CoordinationStatus
        {
            Cancel,
            Timeout,
            AllDone
        }
    }
}
