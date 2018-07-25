using Microsoft.AspNetCore;
using System;
using System.Threading.Tasks;
using Xunit;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;

namespace WebAppCore.IntegrationTests
{
    public class ValueControllerTest
    {
        [Fact]
        public async Task BasicTest()
        {
            var builder = WebHost.CreateDefaultBuilder()
                .UseEnvironment("Development")
                .UseStartup<Startup>();

            using (var server = new TestServer(builder))
            {
                var httpClient = server.CreateClient();
                var response = await httpClient.GetAsync("/api/values");
                response.EnsureSuccessStatusCode();
                var content = await response.Content.ReadAsStringAsync();
                Assert.NotEmpty(content);
            }

        }
    }
}
