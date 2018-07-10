using ConsoleAppCoreTest.Concrete;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WebApiClient;
using WebApiClient.Attributes;

namespace ConsoleAppCoreTest
{
    [TestClass]
    public class WebApiClientTest
    {
        [TestMethod]
        public async Task BasicGetTest()
        {
            var client = HttpApiClient.Create<IMyWebApi>();
            var content = await client.GetContent();
            Assert.IsTrue(content.Contains("<!DOCTYPE html>", StringComparison.OrdinalIgnoreCase));
        }

    }
}
