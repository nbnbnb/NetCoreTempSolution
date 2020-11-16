using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleAppCore.Demos.Misc
{

    public class AwaiterInsider
    {
        internal sealed class Type1 { };
        internal sealed class Type2 { };

        private static async Task<Type1> Method1Async()
        {
            await Task.Delay(100);
            return new Type1();
        }

        private static async Task<Type2> Method2Async()
        {
            await Task.Delay(100);
            return new Type2();
        }

        public static Task<String> MyMethodAsyncConvert(Int32 argument)
        {
            // 创建状态机实例并初始化它
            StateMachine stateMachine = new StateMachine()
            {
                // 创建 builder，从这个存根方法返回 Task<String>
                // 状态机访问 builder 来设置 Task 完成/异常
                m_builder = AsyncTaskMethodBuilder<String>.Create(),

                m_state = -1, // 初始化状态机位置
                m_argument = argument // 将实参拷贝到状态机字段
            };

            // 开始执行状态机
            // 注意，由于 StateMachine 是一个结构类型
            // 所以此处传递的是 ref 引用
            stateMachine.m_builder.Start(ref stateMachine);

            // 返回状态机的 Task
            return stateMachine.m_builder.Task;
        }

        // 注意，这是一个 struct
        private struct StateMachine : IAsyncStateMachine
        {
            // 代表状态机 builder (Task) 及其位置的字段
            public AsyncTaskMethodBuilder<String> m_builder;
            public Int32 m_state;

            // 实参和局部变量现在成了字段
            public Int32 m_argument, m_local, m_x;
            public Type1 m_result_Type1;
            public Type2 m_result_Type2;

            // 每个 awaiter 类型一个字段
            private TaskAwaiter<Type1> m_awaiter_Type1;
            private TaskAwaiter<Type2> m_awaiter_Type2;

            #region 继承的方法

            void IAsyncStateMachine.MoveNext()
            {
                String result = null; // Task 的结果值

                // 编译器插入 try 块来确保状态机的任务完成
                try
                {
                    Boolean executeFinally = true;   //  先假定逻辑上离开 try 块
                    if (m_state == -1)  // 如果第一次在状态机方法中
                    {
                        m_local = m_argument; // 原始方法就从头开始
                    }

                    try // 原始代码中的 try 块
                    {
                        // 这个是最新的 Awaiter
                        // 包含最新的状态和代码位置信息
                        TaskAwaiter<Type1> awaiterType1 = default;
                        TaskAwaiter<Type2> awaiterType2 = default;

                        switch (m_state)
                        {
                            case -1:
                                awaiterType1 = Method1Async().GetAwaiter();
                                if (!awaiterType1.IsCompleted)
                                {
                                    Console.WriteLine("Method1 No Completed");

                                    // 等 ContinuWith 调用的时候
                                    // 会匹配 m_state = 0 这个 switch
                                    m_state = 0;  // Method1Async 要以异步方式完成

                                    m_awaiter_Type1 = awaiterType1; // 保存 awaiter 以便将来返回

                                    // 告诉 awaiter 在操作完成时调用 this.MoveNext
                                    m_builder.AwaitUnsafeOnCompleted(ref awaiterType1, ref this);
                                    // 上述代码调用 awaiterType1 的 OnCompleted，它会在被等待任务上
                                    // 调用 ContinueWith(t=>t.MoveNext())  t：此处传递的 this 状态机
                                    // 所以，当 awaiter 任务完成后，将继续调用调用状态机的 MoveNext

                                    executeFinally = false;  // 逻辑上不离开 try 块

                                    // 注意
                                    // 这里会将线程返回至调用者
                                    return;
                                }

                                // Method1Async 以同步方法完成了
                                break;

                            case 0:
                                // 赋值最新的 Awaiter
                                awaiterType1 = m_awaiter_Type1; // 恢复最新的 awaiter
                                break;

                            case 1:
                                // 赋值最新的 Awaiter
                                awaiterType2 = m_awaiter_Type2; // 恢复最新的 awaiter
                                goto ForLoopEpilog;
                        }

                        // 在第一个 await 后，我们捕捉结果并启动 for 循环

                        // 获取 awaiter 的结果
                        m_result_Type1 = awaiterType1.GetResult();

                    ForLoopPrologue:
                        m_x = 0; // for 循环初始化
                        goto ForLoopBody; // 跳到 for 循环主体

                    ForLoopEpilog:
                        Console.WriteLine("x++");
                        m_result_Type2 = awaiterType2.GetResult();
                        m_x++;
                        goto ForLoopBody; // 跳到 for 循环主体

                    ForLoopBody:
                        if (m_x < 3)  // for 循环测试
                        {
                            // 调用 Method2Async 并获取它的 awaiter
                            awaiterType2 = Method2Async().GetAwaiter();
                            if (!awaiterType2.IsCompleted)
                            {
                                Console.WriteLine("Method2 No Completed");
                                m_state = 1; // Method2Async 要以异步方式完成
                                m_awaiter_Type2 = awaiterType2; // 保存 awaiter 以便将来返回

                                // 告诉 awaiter 在操作完成时调用 this.MoveNext
                                m_builder.AwaitUnsafeOnCompleted(ref awaiterType2, ref this);
                                executeFinally = false; // 逻辑上不离开 try 块

                                // 注意
                                // 这里，线程返回至调用者
                                // 不再向下执行
                                return;
                            }

                            // Method2Async 以同步方法完成了
                            goto ForLoopEpilog;  // 以同步方式完成就再次循环
                        }
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("Catch");
                    }
                    finally
                    {
                        // 只要线程物理上离开 try 就会执行 finally
                        // 我们希望在线程逻辑上离开 try 时才执行这些代码
                        if (executeFinally)
                        {
                            Console.WriteLine("Finally");
                        }
                    }

                    result = "Done";  // 这是最终从异步函数返回的东西
                }
                catch (Exception ex)
                {
                    // 未处理的异常：通过设置异常来完成状态机的 Task
                    m_builder.SetException(ex);
                    return;
                }

                // 无异常，通过返回结果来完成状态机的 Task
                m_builder.SetResult(result);
            }

            void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine stateMachine)
            {
                m_builder.SetStateMachine(stateMachine);
            }
            #endregion


        }
    }
}
