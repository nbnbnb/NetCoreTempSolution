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
    public class GenericControllerTest : IClassFixture<TestServerFixture>
    {
        private readonly TestServerFixture testServerFixture;

        public GenericControllerTest(TestServerFixture fixture)
        {
            testServerFixture = fixture;
        }

        [Fact]
        public async Task GenericController_Test()
        {
            foreach (string controller in new[] { "Sprocket", "Widget" })
            {
                var response = await testServerFixture.httpClient.GetAsync($"/{controller}");
                response.EnsureSuccessStatusCode();
                var content = await response.Content.ReadAsStringAsync();
                Assert.Equal($"Hello from a generic {controller} controller.", content);
            }

        }
    }
}
