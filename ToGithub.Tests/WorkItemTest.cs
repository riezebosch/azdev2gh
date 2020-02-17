using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Octokit;
using Xunit;

namespace ToGithub.Tests
{
    public class WorkItemTests : IClassFixture<TemporaryTeamProject>
    {
        private readonly TemporaryTeamProject _project;

        public WorkItemTests(TemporaryTeamProject project) => _project = project;

        [Fact]
        public void GetWorkItemsTest()
        {
            using var client = _project.Connection.GetClient<WorkItemTrackingHttpClient>();
            var result = client.GetWorkItems(_project.Name, "System.Id", "System.Title").ToEnumerable();

            result
                .Should()
                .NotBeEmpty()
                .And
                .Subject
                .First()
                .Fields
                .Should()
                .ContainKeys("System.Title");
        }

        [Fact]
        public void ToIssueTest()
        {
            var item = new WorkItem { Fields = new Dictionary<string, object>
                {
                    ["System.Title"] = "title",
                    ["System.Description"] = "description"
                }};

            item.ToIssue()
                .Should()
                .BeEquivalentTo(new NewIssue("title")
                {
                    Body = "description"
                });
        }
    }
}