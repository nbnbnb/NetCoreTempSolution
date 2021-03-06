﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleAppCore.Util
{
    public static class TaskLogger
    {
        private static readonly ConcurrentDictionary<Task, TaskLogEntry> s_log =
            new ConcurrentDictionary<Task, TaskLogEntry>();

        public sealed class TaskLogEntry
        {
            public Task Task { get; internal set; }

            public String Tag { get; internal set; }

            public DateTime LogTime { get; internal set; }

            public String CallerMemberName { get; internal set; }

            public String CallerFilePath { get; internal set; }

            public Int32 CallerLineNumber { get; internal set; }

            public override string ToString()
            {
                return String.Format("LogTime={0}, Tag={1}, Member={2}, File={3}({4})",
                    LogTime, Tag ?? "(none)", CallerMemberName, CallerFilePath, CallerLineNumber);
            }
        }

        public enum TaskLogLevel { None, Pending }

        public static TaskLogLevel LogLevel { get; set; }

        /// <summary>
        /// 返回并行集合中的所有未完成任务
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<TaskLogEntry> GetLogEntries()
        {
            return s_log.Values;
        }

        public static Task<TResult> Log<TResult>(this Task<TResult> task,
            String tag = null,
            [CallerMemberName] String callerMemberName = null,
            [CallerFilePath] String callerFilePath = null,
            [CallerLineNumber]  Int32 callerLineNumber = -1)
        {
            return (Task<TResult>)Log((Task)task, tag, callerMemberName, callerFilePath, callerLineNumber);
        }

        public static Task Log(this Task task,
            String tag = null,
            [CallerMemberName] String callerMemberName = null,
            [CallerFilePath] String callerFilePath = null,
            [CallerLineNumber]  Int32 callerLineNumber = -1)
        {
            if (LogLevel == TaskLogLevel.None)
            {
                return task;
            }

            var logEntry = new TaskLogEntry
            {
                Task = task,
                LogTime = DateTime.Now,
                Tag = tag,
                CallerMemberName = callerMemberName,
                CallerFilePath = callerFilePath,
                CallerLineNumber = callerLineNumber
            };

            // 保存未完成的任务信息
            s_log[task] = logEntry;

            // 在原始任务上附加一个新任务
            // 在新任务中删除等待记录

            // 此处使用了 ExecuteSynchronously 标记
            // 这样保证了在原始任务上的线程一致性
            task.ContinueWith(t =>
            {
                // 如果 Task 执行完成了
                // 则将其从日志列表中移除
                s_log.TryRemove(t, out TaskLogEntry entry);
            }, TaskContinuationOptions.ExecuteSynchronously);

            // 返回原始的任务
            return task;
        }
    }
}
