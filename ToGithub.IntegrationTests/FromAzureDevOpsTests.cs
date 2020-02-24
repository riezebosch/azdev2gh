using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
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
            var pbi = await _project.CreateProductBacklogItem("Sample PBI");
            var task = await _project.CreateTask("Some Task");

            var source = new FromAzureDevOps(_project.Connection.GetClient<WorkItemTrackingHttpClient>())
                .As<IFromAzureDevOps>();
            var result = source.ProductBacklogItems(_project.Name).ToEnumerable();

            result
                .Should()
                .Contain(pbi.Id.Value)
                .And
                .NotContain(task.Id.Value);
        }
        
        [Fact]
        public async Task ProductBacklogItemsSkipDone()
        {
            var item = await _project.CreateProductBacklogItem("PBI");
            await _project.SetState(item, "Done");

            var source = new FromAzureDevOps(_project.Connection.GetClient<WorkItemTrackingHttpClient>())
                .As<IFromAzureDevOps>();
            source.ProductBacklogItems(_project.Name).ToEnumerable()
                .Should()
                .NotContain(item.Id.Value);
        }
        
        [Fact]
        public async Task ProductBacklogItemsSkipRemoved()
        {
            var item = await _project.CreateProductBacklogItem("PBI");
            await _project.SetState(item, "Removed");

            var source = new FromAzureDevOps(_project.Connection.GetClient<WorkItemTrackingHttpClient>())
                .As<IFromAzureDevOps>();
            source.ProductBacklogItems(_project.Name).ToEnumerable()
                .Should()
                .NotContain(item.Id.Value);
        }

        [Fact]
        public async Task TaskList()
        {
            var parent = await _project.CreateProductBacklogItem("Sample PBI");
            var child1 = await _project.CreateTask("Sample TASK1");
            var child2 = await _project.CreateTask("Sample TASK2");
            await _project.CreateTask("Sample TASK3");

            await _project.CreateLink(parent, child1);
            await _project.CreateLink(parent, child2);

            var source = new FromAzureDevOps(_project.Connection.GetClient<WorkItemTrackingHttpClient>())
                .As<IFromAzureDevOps>();
            
            var issue = await source.ToIssue(parent.Id.Value);
            issue
                .Body
                .Should()
                .Contain("Sample TASK1")
                .And
                .Contain("Sample TASK2")
                .And
                .NotContain("Sample TASK3");
        }
        
        [Fact]
        public async Task TaskListEmpty()
        {
            var parent = await _project.CreateProductBacklogItem("Sample PBI");
            var source = new FromAzureDevOps(_project.Connection.GetClient<WorkItemTrackingHttpClient>())
                .As<IFromAzureDevOps>();
            
            var issue = await source.ToIssue(parent.Id.Value);
            issue
                .Body
                .Should()
                .BeNull();
        }

        [Fact]
        public async Task Comments()
        {
            var parent = await _project.CreateProductBacklogItem("Sample PBI");
            var client = _project.Connection.GetClient<WorkItemTrackingHttpClient>();
            await client.AddCommentAsync(new CommentCreate { Text = "first comment" }, _project.Name, parent.Id.Value);
            await client.AddCommentAsync(new CommentCreate { Text = "second comment" }, _project.Name, parent.Id.Value);
            
            var source = new FromAzureDevOps(_project.Connection.GetClient<WorkItemTrackingHttpClient>())
                .As<IFromAzureDevOps>();

            source
                .ToComments(parent.Id.Value)
                .ToEnumerable()
                .Should()
                .Equal("first comment", "second comment");
        }
    }
}