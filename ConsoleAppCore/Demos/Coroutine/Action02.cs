using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleAppCore.Demos.Coroutine
{
    public class Action02 : ICoroutineUnit
    {
        public IEnumerator<Task> Do()
        {
            Console.WriteLine("开始执行 Action Baidu");
            yield return new HttpClient().GetAsync("https://www.baidu.com");
            Console.WriteLine("结束执行 Action Baidu");
            Console.WriteLine("开始执行 Action Taobao");
            yield return new HttpClient().GetAsync("https://www.taobao.com");
            Console.WriteLine("结束执行 Action Taobao");
        }
    }
}
