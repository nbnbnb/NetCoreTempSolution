using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace ConsoleAppCore.Demos.SimpleAOP
{
    public class SamepleProxy : DispatchProxy
    {
        // 从 IoC 容器中获取实现类
        private static Dictionary<Type, object> _mapping = new Dictionary<Type, object>
        {
            { typeof(ITargetInterface),new TargetClass()}
        };

        /// <summary>
        /// 当其他方法调用的时候
        /// 执行这个拦截器方法
        /// </summary>
        /// <param name="targetMethod"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        protected override object Invoke(MethodInfo targetMethod, object[] args)
        {
            Console.WriteLine("DispatchProxy Invoke");
            return targetMethod.Invoke(_mapping[targetMethod.DeclaringType], args);
        }
    }
}
