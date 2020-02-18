using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Octokit;
using Xunit;

namespace ToGithub.IntegrationTests
{
    public class WorkItemTests : IClassFixture<TemporaryTeamProject>
    {
        private readonly TemporaryTeamProject _project;

        public WorkItemTests(TemporaryTeamProject project) => _project = project;

        [Fact]
        public async Task GetWorkItemsTest()
        {
            await _project.CreateProductBacklogItem("Sample PBI");
            
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
    }
}