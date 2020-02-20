using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoNSubstitute;
using AzureFunctions.TestHelpers;
using Functions.Starters;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Timers;
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
                    .AddDurableTask(options => options.HubName = nameof(Test1))
                    .AddAzureStorageCoreServices()
                    .ConfigureServices(services => 
                        services
                            .AddSingleton(fixture.Create<IFromAzureDevOps>())
                            .AddSingleton(fixture.Create<IGitHubClient>())))
                .Build();
            
            await host.StartAsync();
            var jobs = host.Services.GetService<IJobHost>();
            await jobs.Terminate()
                .Purge();

            // Act
            await jobs.CallAsync(nameof(Starter), new Dictionary<string, object>
            {
                ["request"] = new DummyHttpRequest()
            });

            await jobs
                .WaitFor(nameof(Starter))
                .ThrowIfFailed()
                .Purge();
        }
    }
}