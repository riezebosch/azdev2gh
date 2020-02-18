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
        
        [Fact]
        public async Task HierarchyForwardRelatedItems()
        {
            var parent = await _project.CreateProductBacklogItem("Sample PBI");
            var child1 = await _project.CreateTask("Sample TASK1");
            var child2 = await _project.CreateTask("Sample TASK2");
            await _project.CreateTask("Sample TASK3");

            await _project.CreateLink(parent, child1);
            await _project.CreateLink(parent, child2);
            
            using var client = _project.Connection.GetClient<WorkItemTrackingHttpClient>();

            var related = client.HierarchyForwardRelatedItems(parent)
                .ToEnumerable()
                .ToList();

            related
                .Select(x => x.Target.Id)
                .Should()
                .Equal(parent.Id, child1.Id, child2.Id);
        }
    }
}