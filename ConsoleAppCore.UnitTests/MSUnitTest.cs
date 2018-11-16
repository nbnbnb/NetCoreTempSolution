
using ConsoleAppCore.UnitTests.Concrete;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace ConsoleAppCore.UnitTests
{
    [TestClass]
    //[Ignore]
    public class MSUnitTest : TestBase
    {
        [ClassInitialize]
        public static void ClassInitialize_Test(TestContext testContext)
        {
            StaticData.IsClassInit = true;
        }

        [TestInitialize]
        public void TestInitialize_Test()
        {

        }

        [TestCleanup]
        public void TestCleanup_Test()
        {

        }

        [TestMethod]
        public void TestBase_Init_Test()
        {
            Assert.IsNotNull(TestContext);
            Assert.IsTrue(StaticData.IsAssemblyInit);
            Assert.IsTrue(StaticData.IsClassInit);
        }

        /// <summary>
        /// 此处使用 DataTestMethod 标注
        /// 表示测试方法数据是从 DataRow 中传入的
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="result"></param>
        [DataTestMethod]
        [DataRow(1, 2, 3)]
        [DataRow(2, 3, 5)]
        [DataRow(3, 5, 8)]
        public void DataTestMethod_Test(int a, int b, int result)
        {
            Assert.AreEqual(result, a + b);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ExpectedException_Test()
        {
            throw new ArgumentNullException();
        }
    }
}
