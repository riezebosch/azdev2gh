using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.WebApi.Patch;
using Microsoft.VisualStudio.Services.WebApi.Patch.Json;
using Xunit;

namespace ToGithub.IntegrationTests
{
    public class FromAzureDevOpsTests : IClassFixture<TemporaryTeamProject>
    {
        private readonly TemporaryTeamProject _project;

        public FromAzureDevOpsTests(TemporaryTeamProject project) => _project = project;

        [Fact]
        public async Task ProductBacklogItems()
        {
            await _project.CreateProductBacklogItem("Sample PBI");
            await _project.CreateTask("Some Task");
            
            var client = _project.Connection.GetClient<WorkItemTrackingHttpClient>();

            var source = new FromAzureDevOps(client);
            var result = source.ProductBacklogItems(_project.Name, "System.Id", "System.Title").ToEnumerable();

            result
                .Should()
                .Contain(x => (string)x.Fields["System.Title"] == "Sample PBI")
                .And
                .NotContain(x => (string)x.Fields["System.Title"] == "Some Task")
                .And
                .Subject
                .First()
                .Fields
                .Keys
                .Should()
                .BeEquivalentTo("System.Id", "System.Title");;
        }
        
        [Fact]
        public async Task ProductBacklogItemsSkipDone()
        {
            var item = await _project.CreateProductBacklogItem("PBI");
            await _project.SetState(item, "Done");

            var source = new FromAzureDevOps(_project.Connection.GetClient<WorkItemTrackingHttpClient>());
            source.ProductBacklogItems(_project.Name, "System.Id", "System.Title", "System.State").ToEnumerable()
                .Should()
                .NotContain(x => (string) x.Fields["System.State"] == "Done");
        }
        
        [Fact]
        public async Task ProductBacklogItemsSkipRemoved()
        {
            var item = await _project.CreateProductBacklogItem("PBI");
            await _project.SetState(item, "Removed");

            var source = new FromAzureDevOps(_project.Connection.GetClient<WorkItemTrackingHttpClient>());
            source.ProductBacklogItems(_project.Name, "System.Id", "System.Title", "System.State").ToEnumerable()
                .Should()
                .NotContain(x => (string) x.Fields["System.State"] == "Removed");
        }

        [Fact]
        public async Task ChildrenFor()
        {
            var parent = await _project.CreateProductBacklogItem("Sample PBI");
            var child1 = await _project.CreateTask("Sample TASK1");
            var child2 = await _project.CreateTask("Sample TASK2");
            await _project.CreateTask("Sample TASK3");

            await _project.CreateLink(parent, child1);
            await _project.CreateLink(parent, child2);
            
            var client = _project.Connection.GetClient<WorkItemTrackingHttpClient>();
            var source = new FromAzureDevOps(client);
            var related = source.For(parent)
                .ToEnumerable()
                .ToList();

            related
                .Select(x => x.Id)
                .Should()
                .Equal(child1.Id, child2.Id);
            
            related
                .First()
                .Fields
                .Keys
                .Should()
                .BeEquivalentTo("System.Title", "System.State");;
        }
        
        [Fact]
        public async Task ChildrenForEmpty()
        {
            var parent = await _project.CreateProductBacklogItem("Sample PBI");

            var client = _project.Connection.GetClient<WorkItemTrackingHttpClient>();
            var source = new FromAzureDevOps(client);
            var related = source.For(parent)
                .ToEnumerable()
                .ToList();

            related
                .Should()
                .BeEmpty();
        }
    }
}