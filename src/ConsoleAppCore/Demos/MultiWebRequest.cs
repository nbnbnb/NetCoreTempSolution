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
                m_ac.AboutToBegin(1);
                httpClient.GetByteArrayAsync(server)
                    .ContinueWith(it => ComputeResult(server, it));
            }

            // 告诉 AsyncCoordinator 所有操作都已发起
            // 并在所有操作完成、调用 Cancel 或者发生超时的时候调用 AllDone
            m_ac.AllBegun(AllDone, timeOut);
        }

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

            // 保存结果（exception/sum），指出 1 个操作完成
            m_services[server] = result;
            m_ac.JustEnded();
        }

        // 调用这个方法指出结果已无关紧要
        public void Cancel()
        {
            m_ac.Cancel();
        }

        private void AllDone(AsyncCoordinator.CoordinationStatus status)
        {
            switch(status)
            {
                case AsyncCoordinator.CoordinationStatus.Cancel:
                    Console.WriteLine("Operation canceled");
                    break;
                case AsyncCoordinator.CoordinationStatus.Timeout:
                    Console.WriteLine("Operation time-out");
                    break;
                case AsyncCoordinator.CoordinationStatus.AllDone:
                    Console.WriteLine("Operation completed; result below:");
                    foreach(var server in m_services)
                    {
                        Console.Write("{0} ", server.Key);
                        Object result = server.Value;
                        if(result is Exception)
                        {
                            Console.WriteLine("failed duo to {0}.", result.GetType().Name);
                        }
                        else
                        {
                            Console.WriteLine("returned {0:N0} bytes.",result);
                        }
                    }
                    break;
            }
        }
    }
}
