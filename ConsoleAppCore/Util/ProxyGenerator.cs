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
            var type = ProxyUtil.CreateInterfaceProxy(interfaceType);
            return Activator.CreateInstance(type);
        }


    }
}
