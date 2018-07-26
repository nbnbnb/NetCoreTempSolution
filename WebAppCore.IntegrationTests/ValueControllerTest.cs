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
    public class ValueControllerTest : IClassFixture<TestServerFixture>
    {
        private readonly TestServerFixture testServerFixture;

        public ValueControllerTest(TestServerFixture fixture)
        {
            testServerFixture = fixture;
        }

        [Fact]
        public async Task BasicTest()
        {
            var response = await testServerFixture.httpClient.GetAsync("/api/values");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            var list = JsonConvert.DeserializeObject<IEnumerable<string>>(content);
            Assert.True(list.Count() == 2);
        }
    }
}
