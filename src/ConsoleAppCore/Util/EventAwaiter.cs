﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;

namespace ConsoleAppCore.Util
{
    public sealed class EventAwaiter<TEventArgs> : INotifyCompletion
    {

        private ConcurrentQueue<TEventArgs> m_events = new ConcurrentQueue<TEventArgs>();
        private Action m_continuation;

        #region await 基础设施 - 状态机调用的成员

        /// <summary>
        /// 
        /// 001
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
        /// 002
        /// 
        /// await 基础设施
        /// 
        /// 告诉状态机是否发生了任何事件
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
        /// 003
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
        /// </summary>
        /// <returns></returns>
        public TEventArgs GetResult()
        {
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

            /*
            //　如果有一个等待进行的延续任务，该线程会运行它
            Action continuation = Interlocked.Exchange(ref m_continuation, null);

            if (continuation != null)
            {
                continuation();  // 恢复状态机
            }
            */

            // 简写
            // 如果有一个等待进行的延续任务，该线程会运行它
            Interlocked.Exchange(ref m_continuation, null)?.Invoke();  // 恢复状态机
        }
    }

}