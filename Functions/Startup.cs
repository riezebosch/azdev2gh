using System;
using Functions;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using Octokit;
using ToGithub;

[assembly: WebJobsStartup(typeof(Startup))]
namespace Functions
{
    public class Startup : IWebJobsStartup
    {
        public void Configure(IWebJobsBuilder builder)
        {
            builder.Services.AddScoped<Func<GitHubData, IGitHubClient>>(provider => data => new GitHubClient(new ProductHeaderValue("azure-devops-github-migration"))
            {
                Credentials = new Credentials(data.Token)
            });

            builder.Services.AddScoped<Func<AzureDevOpsData, VssConnection>>(provider => data => 
                new VssConnection(new Uri($"https://dev.azure.com/{data.Organization}"), new VssBasicCredential("", data.Token)));
                
            builder.Services.AddScoped<Func<AzureDevOpsData, IFromAzureDevOps>>(provider => data =>
            {
                var connection = provider.GetService<Func<string, string, VssConnection>>()(data.Organization, data.Token);
                return new FromAzureDevOps(connection.GetClient<WorkItemTrackingHttpClient>());
            });
        }
    }
}