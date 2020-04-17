
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using ConsoleAppCore.Util;
using ConsoleAppCoreTest.Concrete;

namespace ConsoleAppCore.UnitTests
{
    [TestClass]
    public class MiscTest : TestBase
    {
        // 接口需要为 public
        public interface IKKKing
        {
            void Say();
        }


        [TestMethod]
        public void GeneratorProxyTest()
        {
            var testService = ProxyGenerator.Instance.CreateInterfaceProxy<IKKKing>();
            testService.Say();
        }
    }
}
