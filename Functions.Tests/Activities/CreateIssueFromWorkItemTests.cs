using System.Collections.Generic;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoNSubstitute;
using Functions.Activities;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
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
            fixture.Customize<WorkItem>(x => x.With(
                y => y.Fields, new Dictionary<string, object>
                {
                    ["System.Title"] = fixture.Create<string>(),
                    ["System.Description"] = fixture.Create<string>(),
                    ["System.State"] = "To Do"
                }));
            
            var github = Substitute.For<IGitHubClient>();
            
            // Act
            await new CreateIssueFromWorkItem(github, fixture.Create<IFromAzureDevOps>()).Run((1234, 4234));

            // Assert
            await github.Issue.Received().Create(4234, Arg.Any<NewIssue>());
        }
    }
}