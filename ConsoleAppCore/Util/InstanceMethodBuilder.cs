using MonoMod.Utils;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace ConsoleAppCore.Util
{
    /// <summary>
    /// 根据 MethodInfo 信息
    /// 生成委托（缓存起来使用）
    /// 提高方法调用性能
    /// </summary>
    /// <typeparam name="TArgument"></typeparam>
    /// <typeparam name="TReturnValue"></typeparam>
    public static class InstanceMethodBuilder<TArgument, TReturnValue>
    {
        /// <summary>
        /// 调用时，就像 var result = func(t)
        /// </summary>
        /// <typeparam name="TInstanceType">实例类型</typeparam>
        /// <param name="instance">实例对象</param>
        /// <param name="methodInfo">方法元数据</param>
        /// <returns>Func<TArgument, TReturnValue></returns>
        public static Func<TArgument, TReturnValue> CreateInstanceMethod<TInstanceType>(TInstanceType instance, MethodInfo methodInfo)
        {
            if (instance == null)
            {
                throw new ArgumentNullException(nameof(instance));
            }
            if (methodInfo == null)
            {
                throw new ArgumentNullException(nameof(methodInfo));
            }

            return (Func<TArgument, TReturnValue>)methodInfo.CreateDelegate(typeof(Func<TArgument, TReturnValue>), instance);
        }

        /// <summary>
        /// 调用时就像 var result = func(this, t)
        /// </summary>
        /// <typeparam name="TInstanceType">实例类型</typeparam>
        /// <param name="methodInfo">方法元数据</param>
        /// <returns>Func<TInstanceType, TArgument, TReturnValue></returns>
        public static Func<TInstanceType, TArgument, TReturnValue> CreateInstanceMethod<TInstanceType>(MethodInfo methodInfo)
        {
            return (Func<TInstanceType, TArgument, TReturnValue>)methodInfo.CreateDelegate(typeof(Func<TInstanceType, TArgument, TReturnValue>));
        }

    }
}
