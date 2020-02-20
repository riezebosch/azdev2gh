using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Functions.Activities;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;

namespace Functions.Orchestrations
{
    public static class Migrate
    {
        [FunctionName(nameof(Migrate))]
        public static async Task Run([OrchestrationTrigger]IDurableOrchestrationContext context)
        {
            var data = context.GetInput<PostData>();
            
            var repository = await context.CallActivityAsync<long>(nameof(Crository), data.GitHub);
            var items = await context.CallActivityAsync<IEnumerable<int>>(nameof(GetProductBacklogItems), data.AzureDevOps);

            await Task.WhenAll(items.Select(id => context.CallActivityAsync<object>(nameof(CreateIssueFromWorkItem), (id, repository, data.GitHub, data.AzureDevOps))));
        }
    }
}