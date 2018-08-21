using System;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using System.Threading;

namespace ConsoleAppCore.Util
{
    /// <summary>
    /// 001，002，003 是初始化步骤
    /// </summary>
    /// <typeparam name="TEventArgs"></typeparam>
    public sealed class EventAwaiter<TEventArgs> : INotifyCompletion
    {

        private ConcurrentQueue<TEventArgs> m_events = new ConcurrentQueue<TEventArgs>();
        private Action m_continuation;

        #region await 基础设施 - 状态机调用的成员

        /// <summary>
        /// 
        /// 001（Init）
        /// 
        /// await 语法将会识别这个方法
        /// 
        /// 状态机先调用这个来获得 awaiter：我们自己返回自己
        /// </summary>
        /// <returns></returns>
        public EventAwaiter<TEventArgs> GetAwaiter()
        {
            return this;
        }

        /// <summary>
        /// 
        /// 002（Init）
        /// 
        /// await 基础设施
        /// 
        /// 告诉状态机是否发生了任何事件
        /// 
        /// 返回 false 时，然后注册 003， OnCompleted，保存下一步调用
        /// 此时线程返回
        /// </summary>
        public Boolean IsCompleted
        {
            get
            {
                return m_events.Count > 0;
            }
        }

        /// <summary>
        /// 
        /// 003（Init）（INotifyCompletion 接口实现）
        /// 
        /// await 基础设施
        /// 
        /// 状态机告诉我们以后要调用什么方法：我们把它保存起来
        /// </summary>
        public void OnCompleted(Action continuation)
        {
            Volatile.Write(ref m_continuation, continuation);
        }

        /// <summary>
        /// 
        /// 005
        /// 
        /// await 基础设施
        /// 
        /// 状态机查询结果：这是 await 操作符的结果
        /// 
        /// 返回结果之后，继续执行 001，002，003 步骤
        /// </summary>
        /// <returns></returns>
        public TEventArgs GetResult()
        {
            // 获取结果
            m_events.TryDequeue(out TEventArgs e);
            return e;
        }

        #endregion

        /// <summary>
        /// 
        /// 004
        /// 
        /// 如果都引发了事件，多个线程可能同时调用
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        public void EventRaised(Object sender, TEventArgs eventArgs)
        {
            m_events.Enqueue(eventArgs); // 保存 EventArgs 以便从 GetResult/await 返回

            // 简写
            // 如果有一个等待进行的延续任务，该线程会运行它
            // 将 m_continuation 设置为 null
            // 并返回原始的 m_continuation 值
            // 当原始 m_continuation 值不为 null 时，执行它
            Interlocked.Exchange(ref m_continuation, null)?.Invoke();  // 恢复状态机
        }
    }

}
