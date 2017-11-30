using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace ConsoleAppCore.Demos
{
    internal sealed class CSharp71Features
    {
        /// <summary>
        /// 1. 异步的 Main 方法
        /// </summary>
        /// <returns></returns>
        static async Task<int> Main()
        {
            return await DoAsyncWork();
        }

        static async Task<int> DoAsyncWork()
        {
            await Task.Delay(1000);
            return 123;
        }

        /// <summary>
        /// 2. Default literal 表达式
        /// </summary>
        public static void DefaultLiteral()
        {
            // 引用类型默认为 null
            Func<string, bool> whereClause = default(Func<string, bool>);
            Console.WriteLine(whereClause == null);

            // 值类型默认为 0 或 false
            int i = default;
            bool b = default;
            Console.WriteLine(i == 0);
            Console.WriteLine(b == false);

            // 结构类型
            MyStruct myStruct = default(MyStruct);
            Console.WriteLine(myStruct.A == 0);
            Console.WriteLine(myStruct.B == false);

            // CancellationToken 默认为 CancellationToken.None
            CancellationToken token = default;
            Console.WriteLine(token == CancellationToken.None);

        }

        /// <summary>
        /// 3. Tuple 命名推导
        /// </summary>
        public static void InferredTuple()
        {
            int count = 5;
            string label = "Color used in the map";
            var pair = (count, label);

            // pair 命名为 (int:count,string label)
            Console.WriteLine($"{pair.count} - {pair.label}");
        }

        private struct MyStruct
        {
            public int A { get; }
            public bool B { get; }

            public MyStruct(int a, bool b)
            {
                A = a;
                B = b;
            }
        }
    }



}
