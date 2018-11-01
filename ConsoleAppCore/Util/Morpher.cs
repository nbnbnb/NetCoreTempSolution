using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace ConsoleAppCore.Util
{    
    public static class Morpher
    {
        public delegate Int32 MorpherAction<TResult, in TArgument>(Int32 startValue, TArgument argument, out TResult morphResult);

        public static TResult Run<TResult, TArgument>(ref Int32 target, TArgument argument, MorpherAction<TResult, TArgument> morpher)
        {
            // 定义返回值
            TResult morphResult;
            // 定义中间变量
            Int32 currentValue = target, startValue, desiredValue;
            do
            {
                startValue = currentValue;

                // 通过自定义函数计算一个期望的值
                // 这过程中，target 的值可能改变
                desiredValue = morpher(startValue, argument, out morphResult);

                // 如果 target == startValue，表示在 morpher 这个执行期间，target 没有改变
                // 然后则将 desiredValue 赋值给 target，同时将 target 赋值给 currentValue
                // 此时应该是 startValue == target == currentValue
                // 所以，将会跳出 do...while
                currentValue = Interlocked.CompareExchange(ref target, desiredValue, startValue);

                // 如果 target != startValue，表示在 morpher 这个执行期间，target 发生了改变
                // Interlocked.CompareExchange 将返回最新的 target，然后赋值给 currentValue
                // 所以 startValue != currentValue，将会继续 do...while
            } while (startValue != currentValue);

            return morphResult;
        }
    }

}
