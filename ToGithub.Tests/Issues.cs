
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using Octokit;
using Xunit;

namespace ToGithub.Tests
{
    
    public class Tests : IClassFixture<TestConfig>, IDisposable, IAsyncLifetime
    {
        private readonly WorkItemTrackingHttpClient _client;
        private readonly GitHubClient _githubClient;
        private readonly TestConfig _config;
        private Repository _repository;

        public Tests(TestConfig config)
        {
            _config = config;
            var connection = new VssConnection(new Uri($"https://dev.azure.com/{config.AzDo.Organization}"), new VssBasicCredential("",config.AzDo.Token));
            _client = connection.GetClient<WorkItemTrackingHttpClient>();
            _githubClient = new GitHubClient(new ProductHeaderValue("azure-devops-github-migration"))
            {
                Credentials = new Credentials(config.GitHub.Token)
            };
            
            
        }

        [Fact]
        public void GetWorkItemIds()
        {
            var result =  GetWorkItems(_client).ToEnumerable().ToList();
            result
                .Should()
                .NotBeEmpty()
                .And
                .Subject
                .First()
                .Fields
                .Should()
                .ContainKeys("System.Title", "System.Description");
        }

        private static async IAsyncEnumerable<WorkItem> GetWorkItems(WorkItemTrackingHttpClient client)
        {
            var wiql = new Wiql()
            {
                Query = "Select [Id], [State], [Title] " +
                        "From WorkItems"
            };
            var result = await client.QueryByWiqlAsync(wiql);

            foreach (var workItem in result.WorkItems)
            {
                yield return await client.GetWorkItemAsync(workItem.Id, new []{"System.Id", "System.Title", "System.Description"});
            }
            
        }
        
        [Fact] 
        public async Task CreateIssueFromWorkItem()
        {
            await foreach (var item in GetWorkItems(_client))
            {
                item.Fields.TryGetValue("System.Description", out string body);
                    
                await _githubClient.Issue.Create(_repository.Id,
                    new NewIssue(item.Fields["System.Title"].ToString())
                    {
                        Body =  body
                    });
            }
        }


        public void Dispose()
        {
            _client.Dispose();
        }

        public async Task InitializeAsync()
        {
            _repository = await _githubClient.Repository.Create(new NewRepository(Guid.NewGuid().ToString().Substring(0,8)));
        }

        public async Task DisposeAsync()
        {
            await _githubClient.Repository.Delete(_repository.Id);
        }
    }
}