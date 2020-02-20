using System.Net.Http;
using System.Threading.Tasks;
using Functions.Orchestrations;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;

namespace Functions.Starters
{
    public static class Starter
    {
        [FunctionName(nameof(Starter))]
        public static async Task Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "POST", Route = "migrate")]HttpRequestMessage request,
            [DurableClient]IDurableOrchestrationClient client)
        {
            var data = await request.Content.ReadAsAsync<PostData>();
            await client.StartNewAsync(nameof(Migrate), data);
        }
    }
}