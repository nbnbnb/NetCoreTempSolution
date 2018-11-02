
using ConsoleAppCore.UnitTests.Concrete;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace ConsoleAppCore.UnitTests
{
    [TestClass]
    public class MSUnitTest : TestBase
    {

        [TestMethod]
        public void AddIntegers_FromDataSourceTest()
        {
            Assert.IsNotNull(TestContext);
            Assert.IsTrue(StaticData.IsInit);
            Assert.IsTrue((bool)GlobalTestContext.Properties["AssemblyInitialize"]);
        }

        /// <summary>
        /// �˴�ʹ�� DataTestMethod ��ע
        /// ��ʾ���Է��������Ǵ� DataRow �д����
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="result"></param>
        [DataTestMethod]
        [DataRow(1, 2, 3)]
        [DataRow(2, 3, 5)]
        [DataRow(3, 5, 8)]
        public void AdditionTest(int a, int b, int result)
        {
            Assert.AreEqual(result, a + b);
        }

    }
}
