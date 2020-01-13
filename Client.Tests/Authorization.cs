using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using Flurl.Http;
using Xunit;

namespace AzureDevOpsRestClient.Tests
{
    public class Authorization : IClassFixture<TestConfig>
    {
        private readonly TestConfig _config;

        public Authorization(TestConfig config) => _config = config;

        [Fact]
        public async Task Private_Project_WithToken_Ok()
        {
            var client = new Client(_config.Organization, _config.Token);
            await client.GetAsync(new Request<object>($"/_apis/projects", "5.1"));
        }
        
        [Fact]
        public async Task PrivateProject_WithoutToken_Unauthorized()
        {
            var client = new Client(_config.Organization, new string('x', 52));
            var ex = await client
                .Invoking(x => x.GetAsync(new Request<object>($"/_apis/projects", "5.1")))
                .Should()
                .ThrowAsync<FlurlHttpException>();

            ex.Which.Call.HttpStatus.Should().Be(HttpStatusCode.Unauthorized);
        }
    }
}