using ConsoleAppCore.Util;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleAppCore.Demos
{
    internal sealed class MultiWebRequest
    {
        private AsyncCoordinator m_ac = new AsyncCoordinator();

        private Dictionary<String, Object> m_services = new Dictionary<string, object>
        {
            {"http://www.baidu.com/",null },
            {"http://www.google.com/",null },
            {"http://www.sohu.com/",null }
        };

        public MultiWebRequest(Int32 timeOut = Timeout.Infinite)
        {
            var httpClient = new HttpClient();

            foreach (var server in m_services.Keys)
            {
                // 通知协调器发起一个请求
                m_ac.AboutToBegin(1);
                httpClient
                    .GetByteArrayAsync(server)
                    .ContinueWith(it => ComputeResult(server, it));
            }

            // 告诉 AsyncCoordinator 所有操作都已发起
            // 并在所有操作完成、调用 Cancel 或者发生超时的时候调用 AllDone
            m_ac.AllBegun(AllDone, timeOut);
        }

        /// <summary>
        /// 每次 I/O 完成后计算结果
        /// </summary>
        /// <param name="server"></param>
        /// <param name="task"></param>
        private void ComputeResult(String server, Task<Byte[]> task)
        {
            Object result;

            if (task.Exception != null)
            {
                result = task.Exception.InnerException;
            }
            else
            {
                // 在线程池线程上处理 I/O 完成 
                // 在此添加自己的计算密集型算法

                result = task.Result.Length;  // 本例只返回长度
            }

            // 保存结果（exception/sum）
            m_services[server] = result;

            // 每次计算结果的时候
            // 需要计算整个异步序列是否完成
            // 如果全部完成，则会通知到 AllDone 状态
            m_ac.IsEnded();
        }

        /// <summary>
        /// 通知协调器取消执行
        /// 调用这个方法指出结果已无关紧要
        /// </summary>
        public void Cancel()
        {
            m_ac.Cancel();
        }

        /// <summary>
        /// 协调器通知状态的回调方法
        /// </summary>
        /// <param name="status"></param>
        private void AllDone(AsyncCoordinator.CoordinationStatus status)
        {
            switch (status)
            {
                case AsyncCoordinator.CoordinationStatus.Cancel:
                    Console.WriteLine("Operation canceled");
                    break;
                case AsyncCoordinator.CoordinationStatus.Timeout:
                    Console.WriteLine("Operation time-out");
                    break;
                case AsyncCoordinator.CoordinationStatus.AllDone:
                    Console.WriteLine("Operation completed; result below:");
                    foreach (var server in m_services)
                    {
                        Console.Write("{0} ", server.Key);
                        Object result = server.Value;
                        if (result is Exception)
                        {
                            Console.WriteLine("failed duo to {0}.", result.GetType().Name);
                        }
                        else
                        {
                            Console.WriteLine("returned {0:N0} bytes.", result);
                        }
                    }
                    break;
            }
        }
    }
}
