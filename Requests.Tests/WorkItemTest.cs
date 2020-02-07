using System.Threading.Tasks;
using Microsoft.VisualStudio.Services.WebApi;
using Xunit;

namespace AzureDevOpsRest.Requests.Tests
{
    public class WorkItemTest
    {
        [Fact]
        public async Task List()
        {
            var client = new Client("manuel");
            var project = "github-migration";
            
            await client.GetAsync(new Request<WorkItem>($"{project}/_apis/wit/workitems", "5.1").WithQueryParams(("ids", "1434")));
            
        }

        [Fact]
        public async Task ClientList()
        {
            var client = VssConnection()
        }
    }

    public class WorkItem
    {
    }
}