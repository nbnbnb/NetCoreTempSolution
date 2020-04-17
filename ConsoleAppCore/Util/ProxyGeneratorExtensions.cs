using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleAppCore.Util
{
    public static class ProxyGeneratorExtensions
    {
        public static IInterface CreateInterfaceProxy<IInterface>(this ProxyGenerator proxyGenerator) where IInterface : class
            => (IInterface)proxyGenerator.CreateInterfaceProxy(typeof(IInterface));
    }
}

