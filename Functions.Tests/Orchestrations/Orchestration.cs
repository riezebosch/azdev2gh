using System.Collections.Generic;
using System.Threading.Tasks;
using AutoFixture;
using Functions.Activities;
using Functions.Orchestrations;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Moq;
using Xunit;

namespace Functions.Tests.Orchestrations
{
    public static class MigrateTests
    {
        [Fact]
        public static async Task Test()
        {
            // Arrange
            var fixture = new Fixture();
            var repositoryId = fixture.Create<long>();
            var token = fixture.Create<string>();
            
            var context = new Mock<IDurableOrchestrationContext>(MockBehavior.Strict);
            context
                .Setup(x => x.GetInput<string>())
                .Returns(token);
            
            context
                .Setup(x => x.CallActivityAsync<long>(nameof(CreateRepository), token))
                .ReturnsAsync(repositoryId);
            
            context
                .Setup(x => x.CallActivityAsync<IEnumerable<int>>(nameof(GetProductBacklogItems), null))
                .ReturnsAsync(new[] { 1, 2, 3 });

            context
                .Setup(x => x.CallActivityAsync<object>(nameof(CreateIssueFromWorkItem), It.Is<(int, long)>(item => item.Item2 == repositoryId)))
                .ReturnsAsync(null);
            
            // Act
            await Migrate.Run(context.Object);

            // Assert
            context.VerifyAll();
        }
    }
}