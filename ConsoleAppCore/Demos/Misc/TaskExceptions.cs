using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleAppCore.Demos.Misc
{
    class TaskExceptions
    {
        internal static void Run()
        {
            var task1 = Task.Run(() =>
            {
                Task.Delay(3000);
                M1();  // 抛出 AggregateException 异常，内部异常为 throw new NotSupportedException("M1")
            });
            var task2 = Task.Run(() =>
            {
                Task.Delay(2000);
                M2(); // 抛出 AggregateException 异常，内部异常为 throw new NotSupportedException("M2")
            });
            var task3 = Task.Run(() =>
            {
                Task.Delay(1000);
                M3();  // 直接抛出异常 throw new DivideByZeroException("M3 - a/b")
            });
            try
            {
                Task.WaitAll(task1, task2, task3);
            }
            catch (AggregateException exs)
            {
                Console.WriteLine("---- Demo1 ----");
                Console.WriteLine("直接遍历 AggregateException.InnerExceptions");
                // 直接遍历 AggregateException.InnerExceptions
                // IsAggregateException: True , Message: One or more errors occurred. (M1)
                // IsAggregateException: True , Message: One or more errors occurred. (M2)
                // IsAggregateException: False , Message: M3 - a / b
                foreach (Exception sb in exs.InnerExceptions)
                {
                    // 输出异常文本
                    Console.WriteLine($"IsAggregateException: {sb is AggregateException} , Message: {sb.Message}");
                }

                Console.WriteLine("\n---- Demo2 ----");
                Console.WriteLine("区分 AggregateException 和基本异常");
                foreach (Exception sb in exs.InnerExceptions)
                {
                    // 任务里面嵌套的任务，如果抛出了异常，这会封装为 AggregateException
                    // 任务里面如果直接抛出了异常，则通过 InnerExceptions 就可以遍历到（第一层解析）
                    AggregateException aex = sb as AggregateException;
                    if (aex != null)
                    {
                        Console.WriteLine("解包：" + aex.GetBaseException().Message + "---" + sb.GetType());
                    }
                    else
                    {
                        Console.WriteLine("正常：" + sb.Message + "---" + sb.GetType());
                    }
                }

                Console.WriteLine("\n---- Demo3 ----");
                Console.WriteLine("使用 Flatten() 方法将 AggregateException 都扁平化");
                // Flatten() 返回的是一个 AggregateException，扁平化后的 Exception 列表在 InnerExceptions 属性中
                // 扁平化 AggregateException
                // M3
                // M1
                // M2

                // 注意，Flatten() 返回的集合是“不透明的”
                // 目前来看，符合下面的规律
                // 外层 M3 的在前面
                // 然后是放入 WaitAll 顺序， M1 在 M2 之前
                // 和异常抛出的时间先后顺序无关
                foreach (Exception sb in exs.Flatten().InnerExceptions)
                {
                    Console.WriteLine(sb.Message + "---" + sb.GetType());
                }
                Console.WriteLine("---");
            }

            Console.WriteLine("\n---- Demo4 ----");
            Console.WriteLine("使用 GetBaseException() 获取最内层的异常");
            try
            {
                MLink();  // 内层抛出 throw new NotSupportedException("MBaseError")
            }
            catch (AggregateException ex)
            {
                // 使用 AggregateException 重写的 GetBaseException 方法返回作为问题根源的最内层的异常
                Exception baseEx = ex.GetBaseException();
                Console.WriteLine(baseEx.Message + "---" + baseEx.GetType());

                // 如果不用上面的方式，还可以通过 InnerException 层级方式获取
                Console.WriteLine("使用 InnerException 层级方式获取");
                // 注意看，此处一共获取了 3 层，刚好 3 层 Task 嵌套
                // ex 是 MLink() 方法的 AggregateException
                // 第一层是 MBase01Error() 方法的 AggregateException
                // 第二层是 MBase02Error() 方法的 AggregateException
                // 第三层才是最终的 NotSupportedException
                Exception baseExOther = ex.InnerException.InnerException.InnerException;
                Console.WriteLine(baseExOther.Message + "---" + baseExOther.GetType());
            }

            Console.WriteLine("\n---- Demo5 ----");
            Console.WriteLine("Task.WaitAll 对 GetBaseException() 的影响");
            try
            {
                Task.WaitAll(task1, task2, task3);
            }
            catch (AggregateException ex)
            {
                // 返回根源异常
                // 返回最内层异常的顺序和异常抛出的时间无关，TaskAll 会等待所有的异常
                // 现在的实现是，将“第一个任务的异常当作最内层异常返回”，此处就是 task1
                // 使用 AggregateException 重写的 GetBaseException 方法返回作为问题根源的最内层的异常
                // 第一个任务的 AggregateException 异常作为最内层异常抛出

                // 为什么没有像上面 MLink() 示例一样进行解包呢？
                // 因为 Task.WaitAll 会对异常有一个封装
                // 获得引发异常的根本原因

                // ex 是对整个 WaitAll() 方法中抛出的异常的一个封装
                Exception ex_01 = ex.GetBaseException();

                Console.WriteLine(ex_01 == ex);  // True ????

                // 其 InnerException 是 M1 抛出的 AggregateException
                Exception ex_02 = ex_01.InnerException;

                // 最后获取 M1 的 AggregateException 的内部异常
                Exception ex_03 = ex_02.InnerException;

                Console.WriteLine(ex_03.Message + "---" + ex_03.GetType());

                Console.WriteLine("\n---- Demo6 ----");
                Console.WriteLine("直接通过 InnerException.InnerException 获取层级异常（结果与上面一样）");
                // 将“第一个任务的异常当作 InnerException 返回”，此处就是 task1 的异常
                // M1 抛出的 AggregateException
                // 然后再次通过 InnerException 获取其内部异常，最后就是 throw new NotSupportedException("M1")
                Exception innerEx = ex.InnerException.InnerException;
                Console.WriteLine(innerEx.Message + "---" + innerEx.GetType());
            }
        }

        private static void M1()
        {
            Task.Run(() =>
            {
                throw new NotSupportedException("M1");
            }).Wait();  // 只有调用 Wait 才会导致这个任务抛出异常
        }
        private static void M2()
        {
            Task.Run(() =>
            {
                throw new NotSupportedException("M2");
            }).Wait(); // 只有调用 Wait 才会导致这个任务抛出异常
        }
        private static void M3()
        {
            // 这个异常是直接在方法中抛出的，不需要进行解包
            throw new DivideByZeroException("M3 - a/b");
        }
        private static void MLink()
        {
            Task.Run(() =>
            {
                // 内部触发异常
                MBase01Error();
            }).Wait();  // 只有调用 Wait 才会导致这个任务抛出异常
        }
        private static void MBase01Error()
        {
            Task.Run(() =>
            {
                MBase02Error();
            }).Wait();  // 只有调用 Wait 才会导致这个任务抛出异常
        }

        private static void MBase02Error()
        {
            Task.Run(() =>
            {
                throw new NotSupportedException("MBaseError");
            }).Wait();  // 只有调用 Wait 才会导致这个任务抛出异常
        }
    }
}
