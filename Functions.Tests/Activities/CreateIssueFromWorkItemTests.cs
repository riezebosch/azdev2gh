using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoNSubstitute;
using Functions.Activities;
using NSubstitute;
using Octokit;
using ToGithub;
using Xunit;

namespace Functions.Tests.Activities
{
    public class CreateIssueFromWorkItemTests
    {
        [Fact]
        public async Task Test()
        {
            // Arrange
            var fixture = new Fixture().Customize(new AutoNSubstituteCustomization { ConfigureMembers =  true});
            var github = fixture.Create<IGitHubClient>();
            var azdo = Substitute.For<IFromAzureDevOps>();
            azdo
                .ProductBacklogItems(Arg.Any<string>())
                .Returns(fixture.CreateMany<int>().ToAsyncEnumerable());
            
            azdo
                .ToComments(Arg.Any<int>())
                .Returns(fixture.CreateMany<string>().ToAsyncEnumerable());
            
            // Act
            var function = new CreateIssueFromWorkItem((token) => github, data => azdo);
            await function.Run((1234, 4234, fixture.Create<GitHubData>(), fixture.Create<AzureDevOpsData>()));

            // Assert
            await github.Issue.Received().Create(4234, Arg.Any<NewIssue>());
            await github.Issue.Comment.Received().Create(4234, Arg.Any<int>(), Arg.Any<string>());
        }
    }
}