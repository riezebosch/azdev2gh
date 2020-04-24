using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoNSubstitute;
using AzureFunctions.TestHelpers;
using Functions.Starters;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Octokit;
using ToGithub;
using Xunit;

namespace Functions.IntegrationTests
{
    public static class UnitTest1
    {
        [Fact]
        public static async Task Test1()
        {
            var fixture = new Fixture().Customize(new AutoNSubstituteCustomization { ConfigureMembers = true });
            using var host = new HostBuilder()
                .ConfigureWebJobs(builder => builder
                    .AddHttp()
                    .AddDurableTask(options =>
                    {
                        options.HubName = nameof(Test1);
                        options.NotificationUrl = new Uri("https://google.com");
                    })
                    .AddAzureStorageCoreServices()
                    .ConfigureServices(services => 
                        services
                            .AddSingleton<Func<AzureDevOpsData, IFromAzureDevOps>>((AzureDevOpsData x) => fixture.Create<IFromAzureDevOps>())
                            .AddSingleton<Func<GitHubData, IGitHubClient>>((GitHubData x) => fixture.Create<IGitHubClient>())))
                .Build();
            
            await host.StartAsync();
            var jobs = host.Services.GetService<IJobHost>();
            await jobs.Terminate()
                .Purge();

            var request =
                new DummyHttpRequest(
                    "{ \"github\": { \"token\": \"xxx\" }, \"azureDevOps\": { \"token\": \"xxx\", \"organization\": \"test\", \"areaPath\": \"test\" } }");
            
            // Act
            await jobs.CallAsync(nameof(Starter), new Dictionary<string, object>
            {
                ["request"] = request
            });

            await jobs
                .WaitFor(nameof(Starter))
                .ThrowIfFailed()
                .Purge();
        }
    }
}