using Microsoft.AspNetCore;
using System;
using System.Threading.Tasks;
using Xunit;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Http;

namespace WebAppCore.IntegrationTests
{
    public class HomeControllerTest : IClassFixture<TestServerFixture>
    {
        private readonly TestServerFixture testServerFixture;

        public HomeControllerTest(TestServerFixture fixture)
        {
            testServerFixture = fixture;
        }

        [Fact]
        public async Task XmlReault_Test()
        {
            var response = await testServerFixture.httpClient.GetAsync("/Home/GetUser");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            Assert.StartsWith("<?xml version=\"1.0\"?>", content, StringComparison.OrdinalIgnoreCase);
        }
    }
}
