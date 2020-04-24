using System;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoNSubstitute;
using AzureFunctions.TestHelpers;
using Functions.Orchestrations;
using Functions.Starters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Moq;
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
            
            client
                .Setup(x => x.WaitForCompletionOrCreateCheckStatusResponseAsync(It.IsAny<HttpRequest>(), It.IsAny<string>(), It.IsAny<TimeSpan>(), It.IsAny<TimeSpan>()))
                .ReturnsAsync(fixture.Create<IActionResult>());

            // Act
            await Starter.Run(new DummyHttpRequest("{ \"github\": { \"token\": \"xxx\" }, \"azureDevOps\": { \"token\": \"xxx\", \"organization\": \"test\", \"areaPath\": \"test\" } }"), client.Object);
            
            // Assert
            client.VerifyAll();
        }
    }
}