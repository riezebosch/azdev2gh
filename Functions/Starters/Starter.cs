using System.Text.Json;
using System.Threading.Tasks;
using Functions.Orchestrations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;

namespace Functions.Starters
{
    public static class Starter
    {
        [FunctionName(nameof(Starter))]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "POST", Route = "migrate")]HttpRequest request,
            [DurableClient]IDurableOrchestrationClient client)
        {
            var data = await JsonSerializer.DeserializeAsync<PostData>(request.Body, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
            var id = await client.StartNewAsync(nameof(Migrate), data);

            return await client.WaitForCompletionOrCreateCheckStatusResponseAsync(request, id);
        }
    }
}