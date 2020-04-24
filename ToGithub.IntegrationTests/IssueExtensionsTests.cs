using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using Xunit;

namespace ToGithub.IntegrationTests
{
    public class TasksToList : IClassFixture<TestConfig>, IClassFixture<TemporaryTeamProject>, IAsyncLifetime
    {
        private readonly TemporaryTeamProject _project;
        private readonly VssConnection _connection;

        public TasksToList(TestConfig config, TemporaryTeamProject project)
        {
            _project = project;
            _connection = new VssConnection(new Uri($"https://dev.azure.com/{config.AzureDevOps.Organization}"),
                new VssBasicCredential("", config.AzureDevOps.Token));
        }

        [Fact]
        public async Task Do()
        {
            using var client = _connection.GetClient<WorkItemTrackingHttpClient>();
            var query = await client.QueryByWiqlAsync(new Wiql { Query = 
                $"SELECT [System.Id] FROM WorkItems WHERE [System.TeamProject] = '{_project.Name}'" });

            query
                .WorkItems
                .Should()
                .HaveCount(2);
        }

        public async Task InitializeAsync()
        {
            var parent = await _project.CreateProductBacklogItem("Sample PBI");
            var child = await _project.CreateTask("Sample task");

            await _project.CreateLink(parent, child);
        }

        public Task DisposeAsync() => Task.CompletedTask;
    }
}