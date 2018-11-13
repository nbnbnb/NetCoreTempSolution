using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace WebAppCore.IntegrationTests
{
    public class TestServerFixture : IDisposable
    {
        private readonly TestServer testServer;
        public HttpClient httpClient;

        public TestServerFixture()
        {
            var builder = WebHost.CreateDefaultBuilder()
                  .UseEnvironment(EnvironmentName.Development)
                  //.UseStartup<StartupIntegrationTest>();  // 使用自定义的 Startup
                  .UseStartup<Startup>();  

            testServer = new TestServer(builder);
            httpClient = testServer.CreateClient();
        }

        public void Dispose()
        {
            httpClient.Dispose();
            testServer.Dispose();
        }
    }
}
