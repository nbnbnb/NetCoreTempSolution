using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleAppCore.Util
{
    public static class TaskWithCancellation
    {
        // 因为没有非泛型的 TaskCompletionSource 类
        private struct Void
        {

        }

        public static async Task<TResult> WithCancellation<TResult>(this Task<TResult> originalTask,
            CancellationToken ct)
        {
            // 创建在 CancellationToken 被取消时完成的一个 Task
            var cancelTask = new TaskCompletionSource<Void>();

            // 一旦 CancellationToken 被取消，就完成 Task
            using (ct.Register(t => ((TaskCompletionSource<Void>)t).TrySetResult(new Void()), cancelTask))
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
            return await originalTask;
        }

        public static async Task WithCancellation(this Task originalTask, CancellationToken ct)
        {
            // 创建在 CancellationToken 被取消时完成的一个 Task
            var cancelTask = new TaskCompletionSource<Void>();

            // 一旦 CancellationToken 被取消，就完成 Task
            using (ct.Register(t => ((TaskCompletionSource<Void>)t).TrySetResult(new Void()), cancelTask))
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
                await originalTask;
            }
        }
    }
}
