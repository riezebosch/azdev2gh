using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Functions.Activities;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;

namespace Functions.Orchestrations
{
    public static class Migrate
    {
        public static async Task Run([OrchestrationTrigger]IDurableOrchestrationContext context)
        {
            var (github ,  azdo) = context.GetInput< (GitHubData, AzureDevOpsData)>();
            
            var repository = await context.CallActivityAsync<long>(nameof(CreateRepository), github);
            var items = await context.CallActivityAsync<IEnumerable<int>>(nameof(GetProductBacklogItems), azdo);

            await Task.WhenAll(items.Select(id => context.CallActivityAsync<object>(nameof(CreateIssueFromWorkItem), (id, repository, github, azdo))));
        }
    }
}