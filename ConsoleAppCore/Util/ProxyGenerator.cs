using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleAppCore.Util
{
    public class ProxyGenerator
    {
        public static readonly ProxyGenerator Instance = new ProxyGenerator();

        public object CreateInterfaceProxy(Type interfaceType)
        {
            // 创建动态程序集
            // 然后构建对应的接口实现类
            // 最后返回这个类型
            var type = ProxyUtil.CreateInterfaceProxy(interfaceType);
            return Activator.CreateInstance(type);
        }


    }
}
