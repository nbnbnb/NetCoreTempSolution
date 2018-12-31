using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleAppCore.Demos.Coroutine
{
    public class Action01 : ICoroutineUnit
    {
        public IEnumerator<Task> Do()
        {
            Console.WriteLine("开始执行 Action 163");
            yield return new HttpClient().GetAsync("https://www.163.com");
            Console.WriteLine("结束执行 Action 163");
            Console.WriteLine("开始执行 Action Sina");
            yield return new HttpClient().GetAsync("https://www.sina.com.cn");
            Console.WriteLine("结束执行 Action Sina");
        }
    }
}
