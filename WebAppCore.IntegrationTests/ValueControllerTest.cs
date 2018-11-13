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
using WebAppCore.Entities;

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
        public async Task Basic_Test()
        {
            var response = await testServerFixture.httpClient.GetAsync("/api/values");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            var list = JsonConvert.DeserializeObject<IEnumerable<string>>(content);
            Assert.True(list.Count() == 2);
        }

        [Fact]
        public async Task AuthorWithAuthorId_Test()
        {
            int id = 123;
            var response = await testServerFixture.httpClient.GetAsync($"/api/values/authorWithAuthorId/{id}");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            Author author = JsonConvert.DeserializeObject<Author>(content);
            Assert.NotNull(author);
            Assert.Equal(author.Id, id);
        }

        [Fact]
        public async Task AuthorWithId_Test()
        {
            int id = 456;
            var response = await testServerFixture.httpClient.GetAsync($"/api/values/authorWithId/{id}");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            Author author = JsonConvert.DeserializeObject<Author>(content);
            Assert.NotNull(author);
            Assert.Equal(author.Id, id);
        }
    }
}
