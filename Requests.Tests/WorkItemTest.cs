using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AzureDevOpsRest.Tests;
using FluentAssertions;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;

using Xunit;

namespace AzureDevOpsRest.Requests.Tests
{
    public class WorkItemTest
    {
        private string _project = "github-migration";
        private readonly TestConfig _config = new TestConfig();

        [Fact]
        public async Task List()
        {
            var client = new Client("manuel");

            await client.GetAsync(new Request<WorkItem>($"{_project}/_apis/wit/workitems", "5.1").WithQueryParams(("ids", "1434")));
        }

        [Fact]
        public async Task ClientList()
        {
            var connection = new VssConnection(new Uri($"https://dev.azure.com/manuel"), new VssBasicCredential("", ""));
            var client = connection.GetClient<WorkItemTrackingHttpClient>();
            
            var result = await client.GetWorkItemsAsync(_project, new List<int> {1434});

            result.Should().NotBeEmpty();
        }

        [Fact]
        public async Task GetWorkItemIds()
        {
            var connection = new VssConnection(new Uri($"https://dev.azure.com/{_config.Organization}"), new VssBasicCredential("",_config.Token));
            var client = connection.GetClient<WorkItemTrackingHttpClient>();

            var wiql = new Wiql()
            {
               Query = "Select [Id], [State], [Title] " +
                "From WorkItems"
                
            };
            var result = await client.QueryByWiqlAsync(wiql);

            result.Should().NotBeNull();
            
            
        }
    }

    public class WorkItem
    {
    }
}