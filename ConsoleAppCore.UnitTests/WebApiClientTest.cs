using ConsoleAppCoreTest.Concrete;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WebApiClient;
using WebApiClient.Attributes;

namespace ConsoleAppCore.UnitTests
{
    [TestClass]
    public class WebApiClientTest
    {
        [TestMethod]
        public async Task BasicGetTest()
        {
            var client = HttpApi.Create<IMyWebApi>();
            var content = await client.GetContentAsync();
            Assert.IsTrue(content.Contains("<html>", StringComparison.OrdinalIgnoreCase));
        }
    }
}
