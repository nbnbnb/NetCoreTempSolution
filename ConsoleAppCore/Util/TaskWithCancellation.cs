using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleAppCore.Util
{
    public static class TaskWithCancellation
    {
        public static async Task<TResult> WithCancellation<TResult>(this Task<TResult> originalTask, CancellationToken ct)
        {
            // 创建在 CancellationToken 被取消时完成的一个 Task
            var cancelTask = new TaskCompletionSource<Int32>();

            // 由于没有非泛型的 TaskCompletionSource
            // 所以此处使用一个 Int32 进行模拟返回值

            // 一旦 CancellationToken 被取消，就完成 Task
            using (ct.Register(t => ((TaskCompletionSource<Int32>)t).TrySetResult(0), cancelTask))
            {
                // 创建在原始 Task 或 CancellationToken Task 完成时都完成的一个 Task
                Task any = await Task.WhenAny(originalTask, cancelTask.Task);

                // 任何 Task 因为 CancellationToken　而完成，就抛出 OperationCancelledException
                if (any == cancelTask.Task)
                {
                    ct.ThrowIfCancellationRequested();
                }
            }

            // 等待原始任务（以同步方式）；若任务失败，等待它将抛出第一个内部异常，
            // 而不是抛出 AggregateException

            // System.Threading.Tasks.UnwrapPromise`1[TResult]
            return await originalTask;
        }

        /// <summary>
        /// 在 Task 上面关联一个协助式取消
        /// 
        /// 内部使用 TaskCompletionSource 进行封装
        /// </summary>
        /// <param name="originalTask"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        public static async Task WithCancellation(this Task originalTask, CancellationToken ct)
        {
            // 创建在 CancellationToken 被取消时完成的一个 Task
            var cancelTask = new TaskCompletionSource<Int32>();

            // 由于没有非泛型的 TaskCompletionSource
            // 所以此处使用一个 Int32 进行模拟返回值

            // 注册 CancellationToken 的取消事件
            // 一旦 CancellationToken 被取消，就将 cancelTask 置完成
            using (ct.Register(t => ((TaskCompletionSource<Int32>)t).TrySetResult(new Int32()), cancelTask))
            {
                // 创建在原始 Task 或 CancellationToken Task 完成时都完成的一个 Task
                Task any = await Task.WhenAny(originalTask, cancelTask.Task);

                // 任何 Task 因为 CancellationToken 而完成，就抛出 OperationCanceledException
                if (any == cancelTask.Task)
                {
                    ct.ThrowIfCancellationRequested();
                }

                // 等待原始任务（以同步方式）；若任务失败，等待它将抛出第一个内部一次
                // 而不是抛出 AggregateException

                // System.Threading.Tasks.UnwrapPromise`1[System.Threading.Tasks.VoidTaskResult]
                await originalTask;
            }
        }
    }
}
