using System.Net;
using System.Threading.Tasks;
using AzureDevOpsRestClient.Requests;
using FluentAssertions;
using Flurl.Http;
using Xunit;

namespace AzureDevOpsRestClient.Tests
{
    public class ApiVersion
    {
        [Fact]
        public async Task InvalidApiVersion_BadRequest()
        {
            var client = new Client("manuel");
            var ex = await client
                .Invoking(x => x.GetAsync(new Request<object>($"packer-tasks/_apis/build/definitions", "89")))
                .Should()
                .ThrowAsync<FlurlHttpException>();

            ex.Which.Call.HttpStatus.Should().Be(HttpStatusCode.BadRequest);
        }
    }
}