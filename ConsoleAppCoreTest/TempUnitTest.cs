using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ConsoleAppCoreTest
{
    [TestClass]
    public class TempUnitTest
    {
        [DataTestMethod]
        [DataRow(1, 2, 3)]
        [DataRow(2, 3, 5)]
        [DataRow(3, 5, 8)]
        public void AdditionTest(int a, int b, int result)
        {
            Assert.AreEqual(result, a + b);
        }

        [TestMethod]
        public void TempTest()
        {

        }
    }
}
