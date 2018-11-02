using ConsoleAppCore.UnitTests.Concrete;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleAppCore.UnitTests
{
    // 基类要标记为 [TestClass]
    [TestClass]
    public class TestBase
    {
        /// <summary>
        /// 全局的 TestContext
        /// 可以在里面存储共享的数据
        /// </summary>
        public static TestContext GlobalTestContext { get; set; }

        /// <summary>
        /// 程序集初始化时执行
        /// 所有的资源初始化最前面
        /// 
        /// 解决方案中全部测试可以执行这个方法
        /// 单个项目运行时，
        /// </summary>
        [AssemblyInitialize]
        public static void Assembly_Initialize(TestContext testContext)
        {
            GlobalTestContext = testContext;
            StaticData.IsAssemblyInit = true;
            testContext.Properties["AssemblyInitialize"] = true;
        }

        /// <summary>
        /// 每个测试类共享的 TestContext
        /// 必须将属性名设置为 TestContext，因为测试框架根据这个约定进行注入
        /// </summary>
        public TestContext TestContext { get; set; }

        /// <summary>
        /// 所有测试最后的资源清理
        /// </summary>
        [AssemblyCleanup]
        public static void Assembly_Cleanup()
        {
            // Cleanup
        }
    }
}
