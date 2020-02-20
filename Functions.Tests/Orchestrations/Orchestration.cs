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
            var github = fixture.Create<GitHubData>();
            var azdo = fixture.Create<AzureDevOpsData>();
            
            var context = new Mock<IDurableOrchestrationContext>(MockBehavior.Strict);
            context
                .Setup(x => x.GetInput< (GitHubData, AzureDevOpsData)>())
                .Returns((github, azdo));
            
            context
                .Setup(x => x.CallActivityAsync<long>(nameof(CreateRepository), github))
                .ReturnsAsync(repositoryId);
            
            context
                .Setup(x => x.CallActivityAsync<IEnumerable<int>>(nameof(GetProductBacklogItems), azdo))
                .ReturnsAsync(new[] { 1, 1, 1, 1 });

            var data = (1, repositoryId, github, azdo);
            context
                .Setup(x => x.CallActivityAsync<object>(nameof(CreateIssueFromWorkItem), data))
                .ReturnsAsync(null);
            
            // Act
            await Migrate.Run(context.Object);

            // Assert
            context.VerifyAll();
        }
    }
}