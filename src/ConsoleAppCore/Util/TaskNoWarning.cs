using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleAppCore.Util
{
    public static class TaskNoWarning
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]  // 造成编译器优化调用
        public static void NoWarning(this Task task)
        {
            // 为了取消警告，需要将方法返回的 Task 赋值给一个 Task 变量，然后忽略该变量
            // 这里使用了扩展函数的方式
            // 更简洁

            // 这里没有代码
        }
    }
}
