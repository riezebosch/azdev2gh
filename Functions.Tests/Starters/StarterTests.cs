using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoNSubstitute;
using Functions.Orchestrations;
using Functions.Starters;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Moq;
using NSubstitute;
using Xunit;

namespace Functions.Tests.Starters
{
    public static class StarterTests
    {
        [Fact]
        public static async Task Test()
        {
            // Arrange
            var fixture = new Fixture().Customize(new AutoNSubstituteCustomization { ConfigureMembers = true });
            
            var client = new Mock<IDurableClient>(MockBehavior.Strict);
            client
                .Setup(x => x.StartNewAsync<object>(nameof(Migrate), string.Empty, It.Is<PostData>(y => y.AzureDevOps.Organization == "test")))
                .ReturnsAsync(fixture.Create<string>());

            // Act
            await using var stream = new MemoryStream(Encoding.UTF8.GetBytes(
                "{ 'github': { 'token': 'xxx' }, 'azureDevOps': { 'token': 'xxx', 'organization': 'test', 'areaPath': 'test' } }"));
            {
                var request = Substitute.For<HttpRequestMessage>();
                request.Content = new StreamContent(stream);
                request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                await Starter.Run(request, client.Object);
            }
            
            // Assert
            client.VerifyAll();
        }
    }
}