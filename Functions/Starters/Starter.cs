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
        public static async Task<HttpResponseMessage> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "POST", Route = "migrate")]HttpRequestMessage request,
            [DurableClient]IDurableOrchestrationClient client)
        {
            var data = await request.Content.ReadAsAsync<PostData>();
            var id = await client.StartNewAsync(nameof(Migrate), data);

            return await client.WaitForCompletionOrCreateCheckStatusResponseAsync(request, id);
        }
    }
}