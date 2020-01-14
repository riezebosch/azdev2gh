using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using Flurl.Http;
using Xunit;

namespace AzureDevOpsRest.Tests
{
    public class Authorization : IClassFixture<TestConfig>
    {
        private readonly TestConfig _config;

        public Authorization(TestConfig config) => 
            _config = config;

        [Fact]
        public async Task PrivateProject_Authorized()
        {
            var client = new Client(_config.Organization, _config.Token);
            await client.GetAsync(new TestRequest($"/_apis/projects"));
        }
        
        [Fact]
        public void PrivateProject_EnumerableRequest_Authorized()
        {
            var client = new Client(_config.Organization, _config.Token);
            client
                .GetAsync(new TestEnumerableRequest($"/_apis/projects"))
                .ToEnumerable()
                .Should()
                .NotBeEmpty();
        }

        [Fact]
        public async Task PrivateProject_WrongToken_Unauthorized()
        {
            var client = new Client(_config.Organization, new string('x', 52));
            var ex = await client
                .Invoking(x => x.GetAsync(new TestRequest($"/_apis/projects")))
                .Should()
                .ThrowAsync<FlurlHttpException>();

            ex.Which.Call.HttpStatus.Should().Be(HttpStatusCode.Unauthorized);
        }
        
        [Fact]
        public async Task PrivateProject_NonAuthoritativeInformation_Unauthorized()
        {
            var client = new Client(_config.Organization);
            var ex = await client
                .Invoking(x => x.GetAsync(new TestRequest($"/_apis/projects")))
                .Should()
                .ThrowAsync<FlurlHttpException>();

            ex.Which.Call.HttpStatus.Should().Be(HttpStatusCode.Unauthorized);
        }
        
        [Fact]
        public async Task PublicProject_EmptyToken()
        {
            var client = new Client("manuel", "");
            await client.GetAsync(new TestRequest($"packer-tasks/_apis/build/builds"));
        }
        
        [Fact]
        public void InvalidToken_ArgumentException()
        {
            var ex = FluentActions.Invoking(() => new Client(_config.Organization, "asdf"))
                .Should()
                .Throw<ArgumentException>();
            
            ex.Which
                .ParamName
                .Should()
                .Be("token");

            ex.Which
                .Message
                .Should()
                .Contain("expected to be null or empty");
        }
    }
}