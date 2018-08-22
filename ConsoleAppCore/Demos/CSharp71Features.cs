using System;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleAppCore.Demos
{
    internal sealed class CSharp71Features
    {
        /// <summary>
        /// 1. 异步的 Main 方法
        /// </summary>
        /// <returns></returns>
        private static async Task<int> Main()
        {
            return await DoAsyncWork();
        }

        private static async Task<int> DoAsyncWork()
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
            Func<string, bool> whereClause = default;
            Console.WriteLine(whereClause == null);

            // 值类型默认为 0 或 false
            int i = default;
            bool b = default;
            Console.WriteLine(i == 0);
            Console.WriteLine(b == false);

            // 结构类型
            // 字段默认为 0 或 false
            MyStruct myStruct = default;
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
            (int count, string label) pair = (count, label);

            // pair 命名为 (int:count,string label)
            Console.WriteLine($"{pair.count} - {pair.label}");
        }

        /// <summary>
        /// 7.0 不支持泛型的 Pattern Matching
        /// 7.1 支持
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="abc"></param>
        private static void Attack<T>(T abc) where T : A
        {
            switch (abc)
            {
                case B b:
                    Console.WriteLine("b");
                    break;
                case C c:
                    Console.WriteLine("c");
                    break;
                default:
                    Console.WriteLine("a");
                    break;
            }
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

        private class A { }
        private class B : A { }
        private class C : A { }
    }



}
